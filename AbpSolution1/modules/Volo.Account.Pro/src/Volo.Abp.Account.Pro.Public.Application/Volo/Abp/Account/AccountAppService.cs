using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.Account.Emailing;
using Volo.Abp.Account.Localization;
using Volo.Abp.Account.PhoneNumber;
using Volo.Abp.Account.Security.Recaptcha;
using Volo.Abp.Account.Settings;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.BlobStoring;
using Volo.Abp.Caching;
using Volo.Abp.Content;
using Volo.Abp.Identity;
using Volo.Abp.Identity.Localization;
using Volo.Abp.Identity.Settings;
using Volo.Abp.Imaging;
using Volo.Abp.Localization;
using Volo.Abp.ObjectExtending;
using Volo.Abp.SettingManagement;
using Volo.Abp.Settings;
using Volo.Abp.Users;
using Volo.Abp.VirtualFileSystem;

namespace Volo.Abp.Account;

public class AccountAppService : ApplicationService, IAccountAppService
{
    protected IIdentityRoleRepository RoleRepository { get; }
    protected IIdentitySecurityLogRepository SecurityLogRepository { get; }
    protected IdentityUserManager UserManager { get; }
    protected IAccountEmailer AccountEmailer { get; }
    protected IAccountPhoneService PhoneService { get; }
    protected IdentitySecurityLogManager IdentitySecurityLogManager { get; }
    public IAbpRecaptchaValidatorFactory RecaptchaValidatorFactory { get; set; }
    protected ISettingManager SettingManager { get; }
    protected IBlobContainer<AccountProfilePictureContainer> AccountProfilePictureContainer { get; }
    protected IOptions<IdentityOptions> IdentityOptions { get; }
    protected IImageCompressor ImageCompressor { get; }
    protected IOptions<AbpProfilePictureOptions> ProfilePictureOptions { get; }
    protected IApplicationInfoAccessor ApplicationInfoAccessor { get; }
    protected IdentityUserTwoFactorChecker IdentityUserTwoFactorChecker { get; }
    protected IDistributedCache<EmailConfirmationCodeCacheItem> EmailConfirmationCodeCache { get; }
    protected IdentityErrorDescriber IdentityErrorDescriber { get; }
    protected IOptions<AbpRegisterEmailConfirmationCodeOptions> RegisterEmailConfirmationCodeOptions { get; }

    public AccountAppService(
        IdentityUserManager userManager,
        IAccountEmailer accountEmailer,
        IAccountPhoneService phoneService,
        IIdentityRoleRepository roleRepository,
        IdentitySecurityLogManager identitySecurityLogManager,
        IBlobContainer<AccountProfilePictureContainer> accountProfilePictureContainer,
        ISettingManager settingManager,
        IOptions<IdentityOptions> identityOptions,
        IIdentitySecurityLogRepository securityLogRepository,
        IImageCompressor imageCompressor,
        IOptions<AbpProfilePictureOptions> profilePictureOptions,
        IApplicationInfoAccessor applicationInfoAccessor,
        IdentityUserTwoFactorChecker identityUserTwoFactorChecker,
        IDistributedCache<EmailConfirmationCodeCacheItem> emailConfirmationCodeCache,
        IdentityErrorDescriber identityErrorDescriber,
        IOptions<AbpRegisterEmailConfirmationCodeOptions> registerEmailConfirmationCodeOptions)
    {
        RoleRepository = roleRepository;
        IdentitySecurityLogManager = identitySecurityLogManager;
        UserManager = userManager;
        AccountEmailer = accountEmailer;
        PhoneService = phoneService;
        AccountProfilePictureContainer = accountProfilePictureContainer;
        SettingManager = settingManager;
        IdentityOptions = identityOptions;
        SecurityLogRepository = securityLogRepository;
        ImageCompressor = imageCompressor;
        ProfilePictureOptions = profilePictureOptions;
        ApplicationInfoAccessor = applicationInfoAccessor;
        IdentityUserTwoFactorChecker = identityUserTwoFactorChecker;
        EmailConfirmationCodeCache = emailConfirmationCodeCache;
        IdentityErrorDescriber = identityErrorDescriber;
        RegisterEmailConfirmationCodeOptions = registerEmailConfirmationCodeOptions;

        LocalizationResource = typeof(AccountResource);
        RecaptchaValidatorFactory = NullAbpRecaptchaValidatorFactory.Instance;
    }

    public virtual async Task<IdentityUserDto> RegisterAsync(RegisterDto input)
    {
        await CheckSelfRegistrationAsync();

        if (await UseCaptchaOnRegistration())
        {
            var reCaptchaValidator = await RecaptchaValidatorFactory.CreateAsync();
            await reCaptchaValidator.ValidateAsync(input.CaptchaResponse);
        }

        await IdentityOptions.SetAsync();

        var emailConfirmed = false;
        if (!input.Code.IsNullOrWhiteSpace())
        {
            await CheckEmailConfirmationCodeAsync(input);
            emailConfirmed = true;
        }

        var user = new IdentityUser(GuidGenerator.Create(), input.UserName, input.EmailAddress, CurrentTenant.Id);

        if (emailConfirmed)
        {
            user.SetEmailConfirmed(true);
        }

        input.MapExtraPropertiesTo(user);

        (await UserManager.CreateAsync(user, input.Password)).CheckErrors();
        (await UserManager.AddDefaultRolesAsync(user)).CheckErrors();

        if (!user.EmailConfirmed)
        {
            await SendEmailConfirmationTokenAsync(user, input.AppName, input.ReturnUrl, input.ReturnUrlHash);
        }

        await SettingManager.SetForUserAsync(user.Id, LocalizationSettingNames.DefaultLanguage, $"{CultureInfo.CurrentCulture.Name};{CultureInfo.CurrentUICulture.Name}");

        await EmailConfirmationCodeCache.RemoveAsync(input.EmailAddress);

        return ObjectMapper.Map<IdentityUser, IdentityUserDto>(user);
    }

    protected virtual async Task CheckEmailConfirmationCodeAsync(RegisterDto input)
    {
        var cacheItem = await EmailConfirmationCodeCache.GetAsync(input.EmailAddress);
        if (cacheItem == null || !cacheItem.Valid)
        {
            Logger.LogWarning($"Email confirmation code not found: {input.EmailAddress}");
            throw new UserFriendlyException(L["Volo.Account:EmailConfirmationCodeExpired"]);
        }

        // Check if the code is expired
        if (cacheItem.SendTime.Add(RegisterEmailConfirmationCodeOptions.Value.CodeExpirationTime) < Clock.Now)
        {
            Logger.LogWarning($"Email confirmation code expired: {input.EmailAddress}");
            cacheItem.Valid = false;
            await EmailConfirmationCodeCache.SetAsync(input.EmailAddress, cacheItem,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = cacheItem.AbsoluteExpirationRelativeToNow
                });
            throw new UserFriendlyException(L["Volo.Account:EmailConfirmationCodeExpired"]);
        }

        // Check if the fail count is greater than 5
        if (cacheItem.TryCount > RegisterEmailConfirmationCodeOptions.Value.MaxFailedCheckCount)
        {
            Logger.LogWarning($"Email confirmation code try count reached: {input.EmailAddress}");
            cacheItem.Valid = false;
            await EmailConfirmationCodeCache.SetAsync(input.EmailAddress, cacheItem,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = cacheItem.AbsoluteExpirationRelativeToNow
                });
            throw new UserFriendlyException(L["Volo.Account:EmailConfirmationCodeExpired"]);
        }

        // Check if the last fail time is greater than 1 minute
        if (cacheItem.LastTryTime != null && cacheItem.LastTryTime.Value.AddMinutes(1) > Clock.Now)
        {
            Logger.LogWarning($"Email confirmation code try limit reached: {input.EmailAddress}");
            cacheItem.NextTryTime = cacheItem.LastTryTime.Value.AddMinutes(1);
            await EmailConfirmationCodeCache.SetAsync(input.EmailAddress, cacheItem,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = cacheItem.AbsoluteExpirationRelativeToNow
                });
            throw new UserFriendlyException(L["Volo.Account:InvalidEmailConfirmationCode"]);
        }

        cacheItem.TryCount++;
        cacheItem.LastTryTime = Clock.Now;

        cacheItem.NextTryTime = cacheItem.LastTryTime.Value.AddMinutes(1);

        await EmailConfirmationCodeCache.SetAsync(input.EmailAddress, cacheItem,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = cacheItem.AbsoluteExpirationRelativeToNow
            });

        if (cacheItem.Code != input.Code)
        {
            Logger.LogWarning($"Email confirmation code is invalid: {input.EmailAddress}");
            throw new UserFriendlyException(L["Volo.Account:InvalidEmailConfirmationCode"]);
        }
    }

    public virtual async Task SendPasswordResetCodeAsync(SendPasswordResetCodeDto input)
    {
        var user = await UserManager.FindByEmailAsync(input.Email);
        if (user == null)
        {
            if (await SettingProvider.IsTrueAsync(AccountSettingNames.PreventEmailEnumeration))
            {
                return;
            }

            throw new UserFriendlyException(L["Volo.Account:InvalidEmailAddress", input.Email]);
        }

        var resetToken = await UserManager.GeneratePasswordResetTokenAsync(user);
        await AccountEmailer.SendPasswordResetLinkAsync(user, resetToken, input.AppName, input.ReturnUrl, input.ReturnUrlHash);
    }

    public virtual async Task<bool> VerifyPasswordResetTokenAsync(VerifyPasswordResetTokenInput input)
    {
        var user = await UserManager.GetByIdAsync(input.UserId);
        return await UserManager.VerifyUserTokenAsync(
            user,
            UserManager.Options.Tokens.PasswordResetTokenProvider,
            UserManager<IdentityUser>.ResetPasswordTokenPurpose,
            input.ResetToken);
    }

    public virtual async Task ResetPasswordAsync(ResetPasswordDto input)
    {
        await IdentityOptions.SetAsync();

        var user = await UserManager.GetByIdAsync(input.UserId);
        (await UserManager.ResetPasswordAsync(user, input.ResetToken, input.Password)).CheckErrors();

        await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
        {
            Identity = IdentitySecurityLogIdentityConsts.Identity,
            Action = IdentitySecurityLogActionConsts.ChangePassword
        });
    }

    public virtual async Task<IdentityUserConfirmationStateDto> GetConfirmationStateAsync(Guid id)
    {
        var user = await UserManager.GetByIdAsync(id);

        return new IdentityUserConfirmationStateDto
        {
            PhoneNumberConfirmed = user.PhoneNumberConfirmed,
            EmailConfirmed = user.EmailConfirmed
        };
    }

    public virtual async Task SendPhoneNumberConfirmationTokenAsync(SendPhoneNumberConfirmationTokenDto input)
    {
        await CheckIfPhoneNumberConfirmationEnabledAsync();

        var user = await UserManager.GetByIdAsync(input.UserId);

        if (!input.PhoneNumber.IsNullOrWhiteSpace())
        {
            (await UserManager.SetPhoneNumberAsync(user, input.PhoneNumber)).CheckErrors();
        }

        CheckPhoneNumber(user);

        var token = await UserManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber);
        await PhoneService.SendConfirmationCodeAsync(user, token);
    }

    public virtual async Task SendEmailConfirmationTokenAsync(SendEmailConfirmationTokenDto input)
    {
        var user = await UserManager.GetByIdAsync(input.UserId);
        await SendEmailConfirmationTokenAsync(user, input.AppName, input.ReturnUrl, input.ReturnUrlHash);
    }

    public virtual async Task<bool> VerifyEmailConfirmationTokenAsync(VerifyEmailConfirmationTokenInput input)
    {
        var user = await UserManager.GetByIdAsync(input.UserId);
        return await UserManager.VerifyUserTokenAsync(
            user,
            UserManager.Options.Tokens.EmailConfirmationTokenProvider,
            UserManager<IdentityUser>.ConfirmEmailTokenPurpose,
            input.Token);
    }

    protected virtual async Task SendEmailConfirmationTokenAsync(
        IdentityUser user,
        string applicationName,
        string returnUrl,
        string returnUrlHash)
    {
        var confirmationToken = await UserManager.GenerateEmailConfirmationTokenAsync(user);
        await AccountEmailer.SendEmailConfirmationLinkAsync(user, confirmationToken, applicationName, returnUrl, returnUrlHash);
    }

    public virtual async Task ConfirmPhoneNumberAsync(ConfirmPhoneNumberInput input)
    {
        await CheckIfPhoneNumberConfirmationEnabledAsync();

        var user = await UserManager.GetByIdAsync(input.UserId);

        CheckPhoneNumber(user);

        (await UserManager.ChangePhoneNumberAsync(user, user.PhoneNumber, input.Token)).CheckErrors();

        await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
        {
            Identity = IdentitySecurityLogIdentityConsts.Identity,
            Action = IdentitySecurityLogActionConsts.ChangePhoneNumber
        });
    }

    public virtual async Task ConfirmEmailAsync(ConfirmEmailInput input)
    {
        var user = await UserManager.GetByIdAsync(input.UserId);
        if (user.EmailConfirmed)
        {
            return;
        }

        (await UserManager.ConfirmEmailAsync(user, input.Token)).CheckErrors();
        (await UserManager.UpdateSecurityStampAsync(user)).CheckErrors();

        await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
        {
            Identity = IdentitySecurityLogIdentityConsts.Identity,
            Action = IdentitySecurityLogActionConsts.ChangeEmail
        });
    }

    public virtual async Task SendEmailConfirmationCodeAsync(SendEmailConfirmationCodeDto input)
    {
        if (await UseCaptchaOnRegistration())
        {
            var reCaptchaValidator = await RecaptchaValidatorFactory.CreateAsync();
            await reCaptchaValidator.ValidateAsync(input.CaptchaResponse);
        }

        var user = await UserManager.FindByNameAsync(input.EmailAddress) ?? await UserManager.FindByEmailAsync(input.EmailAddress);
        if (user != null)
        {
            throw new AbpIdentityResultException(IdentityResult.Failed(IdentityErrorDescriber.DuplicateEmail(input.EmailAddress)));
        }

        user = await UserManager.FindByNameAsync(input.UserName) ?? await UserManager.FindByEmailAsync(input.UserName);
        if (user != null)
        {
            throw new AbpIdentityResultException(IdentityResult.Failed(IdentityErrorDescriber.DuplicateUserName(input.UserName)));
        }

        var now = Clock.Now;
        var cacheItem = await EmailConfirmationCodeCache.GetAsync(input.EmailAddress);
        if (cacheItem != null)
        {
            // one day can send max 15 times by default
            var codesInLastDay = cacheItem.SendRecords.Count(x => x >= now.AddMinutes(-(24 * 60)));
            if (codesInLastDay >= RegisterEmailConfirmationCodeOptions.Value.DailySendLimit)
            {
                cacheItem.NextSendTime = cacheItem.SendTime.AddMinutes(24 * 60);
                await EmailConfirmationCodeCache.SetAsync(input.EmailAddress, cacheItem,
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = cacheItem.AbsoluteExpirationRelativeToNow
                    });

                var nextAvailableTime = Clock.ConvertToUserTime(cacheItem.SendTime.AddMinutes(24 * 60));
                Logger.LogWarning($"Email confirmation code limit reached: max {RegisterEmailConfirmationCodeOptions.Value.DailySendLimit} times per day. Email: {input.EmailAddress}");
                throw new UserFriendlyException(L["Volo.Account:EmailConfirmationCodeLimitReached", nextAvailableTime]);
            }

            // one hour can send max 5 times by default
            var codesInLastHour = cacheItem.SendRecords.Count(x => x >= now.AddMinutes(-60));
            if (codesInLastHour >= RegisterEmailConfirmationCodeOptions.Value.HourlySendLimit)
            {
                cacheItem.NextSendTime = cacheItem.SendTime.AddMinutes(60);
                await EmailConfirmationCodeCache.SetAsync(input.EmailAddress, cacheItem,
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = cacheItem.AbsoluteExpirationRelativeToNow
                    });

                var nextAvailableTime = Clock.ConvertToUserTime(cacheItem.SendTime.AddMinutes(60));
                Logger.LogWarning($"Email confirmation code limit reached: max {RegisterEmailConfirmationCodeOptions.Value.HourlySendLimit} times per hour. Email: {input.EmailAddress}");
                throw new UserFriendlyException(L["Volo.Account:EmailConfirmationCodeLimitReached", nextAvailableTime]);
            }

            // one minute can send only one code
            if (cacheItem.SendTime.AddMinutes(1) > now)
            {
                cacheItem.NextSendTime = cacheItem.SendTime.AddMinutes(1);
                await EmailConfirmationCodeCache.SetAsync(input.EmailAddress, cacheItem,
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = cacheItem.AbsoluteExpirationRelativeToNow
                    });

                var nextAvailableTime = Clock.ConvertToUserTime(cacheItem.SendTime.AddMinutes(1));
                Logger.LogWarning($"Email confirmation code can only be sent once per minute. Email: {input.EmailAddress}");
                throw new UserFriendlyException(L["Volo.Account:EmailConfirmationCodeLimitReached", nextAvailableTime]);
            }
        }

        var code = RandomHelper.GetRandom(100000, 999999).ToString();
        await AccountEmailer.SendEmailConfirmationCodeAsync(input.EmailAddress, code);

        var sendCount = cacheItem != null ? cacheItem.SendCount + 1 : 1;

        var nextSendTime = now.AddMinutes(1);
        if (cacheItem != null && cacheItem.SendRecords
                .Where(x => x >= now.AddMinutes(-(24 * 60)))
                .Append(now).Count() >= RegisterEmailConfirmationCodeOptions.Value.DailySendLimit)
        {
            nextSendTime = now.AddMinutes(24 * 60);
        }
        else if (cacheItem != null && cacheItem.SendRecords
                    .Where(x => x >= now.AddMinutes(-60))
                    .Append(now).Count() >= RegisterEmailConfirmationCodeOptions.Value.HourlySendLimit)
        {
            nextSendTime = now.AddMinutes(60);
        }

        var sendRecords = cacheItem != null
            ? cacheItem.SendRecords
                .Append(now)
                .ToList()
            : new List<DateTime> { now };

        await EmailConfirmationCodeCache.SetAsync(input.EmailAddress,
            new EmailConfirmationCodeCacheItem
            {
                EmailAddress = input.EmailAddress,
                Code = code,
                Valid = true,
                SendTime = now,
                SendCount = sendCount,
                SendRecords = sendRecords,
                TryCount = 0,
                LastTryTime = cacheItem?.LastTryTime,
                NextSendTime = nextSendTime,
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(48)
            }, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(48) });
    }

    public virtual async Task<EmailConfirmationCodeLimitDto> GetEmailConfirmationCodeLimitAsync(string emailAddress)
    {
        var cacheItem = await EmailConfirmationCodeCache.GetAsync(emailAddress);
        return new EmailConfirmationCodeLimitDto
        {
            NextSendTime = cacheItem?.NextSendTime,
            NextTryTime = cacheItem?.NextTryTime
        };
    }

    [Authorize]
    public virtual async Task SetProfilePictureAsync(ProfilePictureInput input)
    {
        await SettingManager.SetForUserAsync(CurrentUser.GetId(), AccountSettingNames.ProfilePictureSource, input.Type.ToString());

        var userIdText = CurrentUser.GetId().ToString();

        if (input.Type != ProfilePictureType.Image)
        {
            if (await AccountProfilePictureContainer.ExistsAsync(userIdText))
            {
                await AccountProfilePictureContainer.DeleteAsync(userIdText);
            }
        }
        else
        {
            if (input.ImageContent == null)
            {
                throw new NoImageProvidedException();
            }

            var imageStream = input.ImageContent.GetStream();

            if (ProfilePictureOptions.Value.EnableImageCompression)
            {
                try
                {
                    var compressResult = await ImageCompressor.CompressAsync(imageStream);

                    if (compressResult.Result is not null && imageStream != compressResult.Result && compressResult.Result.CanRead)
                    {
                        await imageStream.DisposeAsync();
                        imageStream = compressResult.Result;
                    }
                }
                catch (Exception e)
                {
                    Logger.LogWarning(e, "Profile picture compression failed! User ID:" + CurrentUser.GetId());
                }
            }

            await AccountProfilePictureContainer.SaveAsync(userIdText, imageStream, true);
        }
    }

    public virtual async Task<ProfilePictureSourceDto> GetProfilePictureAsync(Guid id)
    {
        var pictureSource = await SettingManager.GetOrNullForUserAsync(AccountSettingNames.ProfilePictureSource, id);

        if (pictureSource == ProfilePictureType.Gravatar.ToString())
        {
            var user = await UserManager.GetByIdAsync(id);
            var gravatar = $"https://secure.gravatar.com/avatar/{GetGravatarHash(user.Email)}";
            return new ProfilePictureSourceDto
            {
                Type = ProfilePictureType.Gravatar,
                Source = gravatar,
                FileContent = await GetAvatarFromAvatarAsync(gravatar)
            };
        }

        if (pictureSource == ProfilePictureType.Image.ToString() && await AccountProfilePictureContainer.ExistsAsync(id.ToString()))
        {
            return new ProfilePictureSourceDto
            {
                Type = ProfilePictureType.Image,
                FileContent = await AccountProfilePictureContainer.GetAllBytesAsync(id.ToString())
            };
        }

        return new ProfilePictureSourceDto
        {
            Type = ProfilePictureType.None,
            FileContent = await GetDefaultAvatarAsync()
        };
    }

    public virtual async Task<IRemoteStreamContent> GetProfilePictureFileAsync(Guid id)
    {
        var picture = await GetProfilePictureAsync(id);
        return new RemoteStreamContent(new MemoryStream(picture.FileContent), contentType: "image/jpeg", disposeStream: true);
    }

    public virtual async Task<List<string>> GetTwoFactorProvidersAsync(GetTwoFactorProvidersInput input)
    {
        var user = await UserManager.GetByIdAsync(input.UserId);
        if (await UserManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, nameof(SignInResult.RequiresTwoFactor), input.Token))
        {
            var providers = (await UserManager.GetValidTwoFactorProvidersAsync(user)).ToList();
            if (!user.HasAuthenticator())
            {
                providers.RemoveAll(x => x == TwoFactorProviderConsts.Authenticator);
            }
            return providers;
        }

        throw new UserFriendlyException(L["Volo.Account:InvalidUserToken"]);
    }

    public virtual async Task SendTwoFactorCodeAsync(SendTwoFactorCodeInput input)
    {
        var user = await UserManager.GetByIdAsync(input.UserId);
        if (await UserManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, nameof(SignInResult.RequiresTwoFactor), input.Token))
        {
            switch (input.Provider)
            {
                case TwoFactorProviderConsts.Email:
                    {
                        var code = await UserManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider);
                        await AccountEmailer.SendEmailSecurityCodeAsync(user, code);
                        return;
                    }
                case TwoFactorProviderConsts.Phone:
                    {
                        var code = await UserManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultPhoneProvider);
                        await PhoneService.SendSecurityCodeAsync(user, code);
                        return;
                    }
                case TwoFactorProviderConsts.Authenticator:
                {
                    // No need to send code. The client will use the TOTP generator.
                    return;
                }

                default:
                    throw new UserFriendlyException(L["Volo.Account:UnsupportedTwoFactorProvider"]);
            }
        }

        throw new UserFriendlyException(L["Volo.Account:InvalidUserToken"]);
    }

    [Authorize]
    public virtual async Task<PagedResultDto<IdentitySecurityLogDto>> GetSecurityLogListAsync(GetIdentitySecurityLogListInput input)
    {
        var securityLogs = await SecurityLogRepository.GetListAsync(
            sorting: input.Sorting,
            maxResultCount: input.MaxResultCount,
            skipCount: input.SkipCount,
            startTime: input.StartTime,
            endTime: input.EndTime,
            applicationName: input.ApplicationName,
            identity: input.Identity,
            action: input.Action,
            userId: CurrentUser.GetId(),
            userName: input.UserName,
            clientId: input.ClientId,
            correlationId: input.CorrelationId
        );

        var totalCount = await SecurityLogRepository.GetCountAsync(
            startTime: input.StartTime,
            endTime: input.EndTime,
            applicationName: input.ApplicationName,
            identity: input.Identity,
            action: input.Action,
            userId: CurrentUser.GetId(),
            userName: input.UserName,
            clientId: input.ClientId,
            correlationId: input.CorrelationId
        );

        var securityLogDtos = ObjectMapper.Map<List<IdentitySecurityLog>, List<IdentitySecurityLogDto>>(securityLogs);
        return new PagedResultDto<IdentitySecurityLogDto>(totalCount, securityLogDtos);
    }

    [Authorize]
    public virtual async Task<VerifyAuthenticatorCodeDto> VerifyAuthenticatorCodeAsync(VerifyAuthenticatorCodeInput input)
    {
        var user = await UserManager.GetByIdAsync(CurrentUser.GetId());
        var verificationCode = input.Code.Replace(" ", string.Empty).Replace("-", string.Empty);
        if (await UserManager.VerifyTwoFactorTokenAsync(user, UserManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode))
        {
            user.SetAuthenticator(true);
            (await UserManager.UpdateAsync(user)).CheckErrors();
            var recoveryCodes = await UserManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            if (recoveryCodes != null)
            {
                return new VerifyAuthenticatorCodeDto
                {
                    RecoveryCodes = recoveryCodes.ToList()
                };
            }
        }

        throw new UserFriendlyException(L["Volo.Account:InvalidUserToken"]);
    }

    [Authorize]
    public virtual async Task ResetAuthenticatorAsync()
    {
        var user = await UserManager.GetByIdAsync(CurrentUser.GetId());
        await UserManager.ResetAuthenticatorKeyAsync(user);
        await UserManager.ResetRecoveryCodesAsync(user);
        user.SetAuthenticator(false);
        (await UserManager.UpdateAsync(user)).CheckErrors();
        await IdentityUserTwoFactorChecker.CheckAsync(user);
    }

    [Authorize]
    public virtual async Task<bool> HasAuthenticatorAsync()
    {
        var user = await UserManager.GetByIdAsync(CurrentUser.GetId());
        return user.HasAuthenticator();
    }

    [Authorize]
    public virtual async Task<AuthenticatorInfoDto> GetAuthenticatorInfoAsync()
    {
        var user = await UserManager.GetByIdAsync(CurrentUser.GetId());
        var email = await UserManager.GetEmailAsync(user);

        var unformattedKey = await UserManager.GetAuthenticatorKeyAsync(user);
        if (string.IsNullOrEmpty(unformattedKey))
        {
            await UserManager.ResetAuthenticatorKeyAsync(user);
            unformattedKey = await UserManager.GetAuthenticatorKeyAsync(user);
        }

        var key = AuthenticatorHelper.FormatKey(unformattedKey);
        var uri = AuthenticatorHelper.GenerateQrCodeUri(email, unformattedKey, ApplicationInfoAccessor.ApplicationName);

        return new AuthenticatorInfoDto
        {
            Key = key,
            Uri = uri
        };
    }

    protected virtual string GetGravatarHash(string emailAddress)
    {
        var encodedPassword = new UTF8Encoding().GetBytes(emailAddress);

        var hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(encodedPassword);

        return BitConverter.ToString(hash)
            .Replace("-", string.Empty)
            .ToLower();
    }

    protected virtual async Task CheckSelfRegistrationAsync()
    {
        if (!await SettingProvider.IsTrueAsync(AccountSettingNames.IsSelfRegistrationEnabled))
        {
            throw new UserFriendlyException(L["Volo.Account:SelfRegistrationDisabled"]);
        }
    }

    protected virtual void CheckPhoneNumber(IdentityUser user)
    {
        if (string.IsNullOrEmpty(user.PhoneNumber))
        {
            throw new BusinessException("Volo.Account:PhoneNumberEmpty");
        }
    }

    protected virtual async Task CheckIfPhoneNumberConfirmationEnabledAsync()
    {
        if (!await SettingProvider.IsTrueAsync(IdentitySettingNames.SignIn.EnablePhoneNumberConfirmation))
        {
            throw new BusinessException("Volo.Account:PhoneNumberConfirmationDisabled");
        }
    }

    protected virtual async Task<bool> UseCaptchaOnRegistration()
    {
        return await SettingProvider.IsTrueAsync(AccountSettingNames.Captcha.UseCaptchaOnRegistration);
    }

    //TODO: cache byte[]
    protected virtual async Task<byte[]> GetAvatarFromAvatarAsync(string url)
    {
        var httpclient = LazyServiceProvider.LazyGetRequiredService<IHttpClientFactory>().CreateClient();
        var responseMessage = await httpclient.GetAsync(url);
        return await responseMessage.Content.ReadAsByteArrayAsync();
    }

    //TODO: cache byte[]
    protected virtual async Task<byte[]> GetDefaultAvatarAsync()
    {
        var virtualFileProvider = LazyServiceProvider.LazyGetRequiredService<IVirtualFileProvider>();
        using (var stream = virtualFileProvider.GetFileInfo("/Volo/Abp/Account/ProfilePictures/avatar.jpg").CreateReadStream())
        {
            return await stream.GetAllBytesAsync();
        }
    }
}
