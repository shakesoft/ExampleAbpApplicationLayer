using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Owl.reCAPTCHA;
using Volo.Abp.Account.Localization;
using Volo.Abp.Account.Settings;
using Volo.Abp.AspNetCore.ExceptionHandling;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Volo.Abp.AspNetCore.WebClientInfo;
using Volo.Abp.Clients;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ExceptionHandling;
using Volo.Abp.Identity;
using Volo.Abp.Identity.AspNetCore;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Claims;
using Volo.Abp.Settings;
using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace Volo.Abp.Account.Public.Web.Pages.Account;

public abstract class AccountPageModel : AbpPageModel
{
    public IAccountAppService AccountAppService => LazyServiceProvider.LazyGetRequiredService<IAccountAppService>();
    public IAccountExternalLoginAppService AccountExternalLoginAppService => LazyServiceProvider.LazyGetRequiredService<IAccountExternalLoginAppService>();
    public AbpSignInManager SignInManager => LazyServiceProvider.LazyGetRequiredService<AbpSignInManager>();
    public IdentitySessionManager IdentitySessionManager => LazyServiceProvider.LazyGetRequiredService<IdentitySessionManager>();
    public ICurrentClient CurrentClient => LazyServiceProvider.LazyGetRequiredService<ICurrentClient>();
    public IWebClientInfoProvider WebClientInfoProvider => LazyServiceProvider.LazyGetRequiredService<IWebClientInfoProvider>();
    public IdentityUserManager UserManager => LazyServiceProvider.LazyGetRequiredService<IdentityUserManager>();
    public IdentitySecurityLogManager IdentitySecurityLogManager => LazyServiceProvider.LazyGetRequiredService<IdentitySecurityLogManager>();
    public IIdentityLinkUserAppService IdentityLinkUserAppService => LazyServiceProvider.LazyGetRequiredService<IIdentityLinkUserAppService>();
    public IOptions<IdentityOptions> IdentityOptions => LazyServiceProvider.LazyGetRequiredService<IOptions<IdentityOptions>>();
    public IOptionsSnapshot<reCAPTCHAOptions> ReCaptchaOptions => LazyServiceProvider.LazyGetRequiredService<IOptionsSnapshot<reCAPTCHAOptions>>();
    public IExceptionToErrorInfoConverter ExceptionToErrorInfoConverter => LazyServiceProvider.LazyGetRequiredService<IExceptionToErrorInfoConverter>();
    public IdentityDynamicClaimsPrincipalContributorCache IdentityDynamicClaimsPrincipalContributorCache => LazyServiceProvider.LazyGetRequiredService<IdentityDynamicClaimsPrincipalContributorCache>();
    public AbpAccountOptions AccountOptions => LazyServiceProvider.LazyGetRequiredService<IOptions<AbpAccountOptions>>().Value;
    public ITenantStore TenantStore => LazyServiceProvider.LazyGetRequiredService<ITenantStore>();
    public IWebHostEnvironment WebHostEnvironment => LazyServiceProvider.LazyGetRequiredService<IWebHostEnvironment>();
    public ICurrentPrincipalAccessor CurrentPrincipalAccessor => LazyServiceProvider.LazyGetRequiredService<ICurrentPrincipalAccessor>();

    protected AccountPageModel()
    {
        ObjectMapperContext = typeof(AbpAccountPublicWebModule);
        LocalizationResourceType = typeof(AccountResource);
    }

    protected virtual void CheckCurrentTenant(ExternalLoginInfo externalLoginInfo)
    {
        var props = externalLoginInfo?.AuthenticationProperties;
        if (props?.Items == null)
        {
            return;
        }

        if (!props.Items.TryGetValue(TenantResolverConsts.DefaultTenantKey, out var tenantString))
        {
            return;
        }

        if (Guid.TryParse(tenantString, out var tenantGuid))
        {
            CheckCurrentTenant(tenantGuid);
        }
    }

    protected virtual void CheckCurrentTenant(Guid? tenantId)
    {
        if (CurrentTenant.Id != tenantId)
        {
            throw new ApplicationException($"Current tenant is different than given tenant. CurrentTenant.Id: {CurrentTenant.Id}, given tenantId: {tenantId}");
        }
    }

    protected virtual void CheckIdentityErrors(IdentityResult identityResult)
    {
        if (!identityResult.Succeeded)
        {
            throw new AbpIdentityResultException(identityResult);
        }
    }

    protected virtual string GetLocalizeExceptionMessage(Exception exception)
    {
        if (exception is ILocalizeErrorMessage || exception is IHasErrorCode)
        {
            return ExceptionToErrorInfoConverter.Convert(exception, false).Message;
        }

        return exception.Message;
    }

    protected virtual async Task StoreConfirmUser(IdentityUser user)
    {
        var identity = new ClaimsIdentity(ConfirmUserModel.ConfirmUserScheme);
        identity.AddClaim(new Claim(AbpClaimTypes.UserId, user.Id.ToString()));
        if (user.TenantId.HasValue)
        {
            identity.AddClaim(new Claim(AbpClaimTypes.TenantId, user.TenantId.ToString()));
        }
        await HttpContext.SignInAsync(ConfirmUserModel.ConfirmUserScheme, new ClaimsPrincipal(identity));
    }

    protected virtual async Task StoreChangePasswordUser(IdentityUser user)
    {
        var identity = new ClaimsIdentity(ChangePasswordModel.ChangePasswordScheme);
        identity.AddClaim(new Claim(AbpClaimTypes.UserId, user.Id.ToString()));
        if (user.TenantId.HasValue)
        {
            identity.AddClaim(new Claim(AbpClaimTypes.TenantId, user.TenantId.ToString()));
        }
        await HttpContext.SignInAsync(ChangePasswordModel.ChangePasswordScheme, new ClaimsPrincipal(identity));
    }

    protected virtual async Task StoreLockedUser(IdentityUser user)
    {
        var identity = new ClaimsIdentity(LockedOut.LockedUserScheme);
        identity.AddClaim(new Claim(AbpClaimTypes.UserId, user.Id.ToString()));
        if (user.TenantId.HasValue)
        {
            identity.AddClaim(new Claim(AbpClaimTypes.TenantId, user.TenantId.ToString()));
        }
        await HttpContext.SignInAsync(LockedOut.LockedUserScheme, new ClaimsPrincipal(identity));
    }

    protected virtual async Task<IActionResult> CheckLocalLoginAsync()
    {
        var enableLocalLogin = await SettingProvider.IsTrueAsync(AccountSettingNames.EnableLocalLogin);
        if (!enableLocalLogin)
        {
            Alerts.Warning(L["LocalLoginIsNotEnabled"]);
            return Page();
        }

        return null;
    }

    protected virtual async Task<bool> IsValidReturnUrlAsync([NotNull] string returnUrl)
    {
        return Url.IsLocalUrl(returnUrl) ||
               returnUrl.StartsWith(UriHelper.BuildAbsolute(Request.Scheme, Request.Host, Request.PathBase).RemovePostFix("/")) ||
               await AppUrlProvider.IsRedirectAllowedUrlAsync(returnUrl);
    }

    protected virtual async Task<string> GetTenantDomainAsync()
    {
        var tenantInfo = new BasicTenantInfo(null, null);
        if (CurrentTenant.Id.HasValue)
        {
            var tenantConfiguration = await TenantStore.FindAsync(CurrentTenant.Id.Value);
            tenantInfo = new BasicTenantInfo(CurrentTenant.Id, tenantConfiguration?.Name);
        }

        return await AccountOptions.GetTenantDomain(HttpContext, tenantInfo);;
    }

    protected virtual async Task TryToSwitchUserDuringImpersonateAsync(IdentityUser oldUser, IdentityUser targetUser, bool oldUserIsBeingImpersonated)
    {
        if (!AccountOptions.SwitchUserDuringImpersonate)
        {
            return;
        }

        var currentUserAuthenticateResult = await HttpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme);
        if (currentUserAuthenticateResult.Principal != null && targetUser.Id == currentUserAuthenticateResult.Principal.FindUserId())
        {
            return;
        }

        var isPersistent = currentUserAuthenticateResult.Properties?.IsPersistent ?? false;
        await SignInManager.SignOutAsync();

        var additionalClaims = new List<Claim>();

        if (!oldUserIsBeingImpersonated)
        {
            additionalClaims.Add(new Claim(AbpClaimTypes.ImpersonatorUserId, oldUser.Id.ToString()));
            additionalClaims.Add(new Claim(AbpClaimTypes.ImpersonatorUserName, oldUser.UserName));
            if (oldUser.TenantId != null)
            {
                additionalClaims.Add(new Claim(AbpClaimTypes.ImpersonatorTenantId, oldUser.TenantId.ToString()));
                var tenantConfiguration = await TenantStore.FindAsync(oldUser.TenantId.Value);
                if (tenantConfiguration != null && !tenantConfiguration.Name.IsNullOrWhiteSpace())
                {
                    additionalClaims.Add(new Claim(AbpClaimTypes.ImpersonatorTenantName, tenantConfiguration.Name));
                }
            }
        }

        var rememberMeClaim = CurrentUser.FindClaim(AbpClaimTypes.RememberMe);
        if (rememberMeClaim != null)
        {
            additionalClaims.Add(rememberMeClaim);
        }

        await SignInManager.SignInWithClaimsAsync(targetUser, new AuthenticationProperties
        {
            IsPersistent = isPersistent
        }, additionalClaims);

        //save security log to admin user.
        var userPrincipal = await SignInManager.CreateUserPrincipalAsync(targetUser);
        userPrincipal.Identities.First().AddClaims(additionalClaims);
        using (CurrentPrincipalAccessor.Change(userPrincipal))
        {
            await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
            {
                Identity = IdentitySecurityLogIdentityConsts.Identity,
                Action = "ImpersonateUser"
            });
        }

        // Clear the dynamic claims cache.
        await IdentityDynamicClaimsPrincipalContributorCache.ClearAsync(oldUser.Id, oldUser.TenantId);
        await IdentityDynamicClaimsPrincipalContributorCache.ClearAsync(targetUser.Id, targetUser.TenantId);
    }
}
