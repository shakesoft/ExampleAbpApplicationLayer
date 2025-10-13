using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Volo.Abp.Account.ExternalProviders;
using Volo.Abp.Account.Localization;
using Volo.Abp.Account.Settings;
using Volo.Abp.Application.Services;
using Volo.Abp.Identity.Features;
using Volo.Abp.Identity.Settings;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Encryption;
using Volo.Abp.SettingManagement;
using Volo.Abp.Settings;

namespace Volo.Abp.Account;

[Authorize(AccountPermissions.SettingManagement)]
public class AccountSettingsAppService : ApplicationService, IAccountSettingsAppService
{
    protected ISettingManager SettingManager { get; }

    protected ExternalProviderSettingsHelper ExternalProviderSettingsHelper { get; }

    public AccountSettingsAppService(ISettingManager settingManager, ExternalProviderSettingsHelper externalProviderSettingsHelper)
    {
        SettingManager = settingManager;
        ExternalProviderSettingsHelper = externalProviderSettingsHelper;
        LocalizationResource = typeof(AccountResource);
    }

    public virtual async Task<AccountSettingsDto> GetAsync()
    {
        return new AccountSettingsDto
        {
            IsSelfRegistrationEnabled = await SettingProvider.GetAsync<bool>(AccountSettingNames.IsSelfRegistrationEnabled),
            EnableLocalLogin = await SettingProvider.GetAsync<bool>(AccountSettingNames.EnableLocalLogin),
            PreventEmailEnumeration = await SettingProvider.GetAsync<bool>(AccountSettingNames.PreventEmailEnumeration)
        };
    }

    public virtual async Task UpdateAsync(AccountSettingsDto input)
    {
        if (input != null)
        {
            await SettingManager.SetForCurrentTenantAsync(AccountSettingNames.IsSelfRegistrationEnabled, input.IsSelfRegistrationEnabled.ToString());
            await SettingManager.SetForCurrentTenantAsync(AccountSettingNames.EnableLocalLogin, input.EnableLocalLogin.ToString());
            await SettingManager.SetForCurrentTenantAsync(AccountSettingNames.PreventEmailEnumeration, input.PreventEmailEnumeration.ToString());
        }
    }

    public virtual async Task<AccountTwoFactorSettingsDto> GetTwoFactorAsync()
    {
        await CheckTwoFactorAvailableAsync();

        return new AccountTwoFactorSettingsDto
        {
            TwoFactorBehaviour = await IdentityProTwoFactorBehaviourSettingHelper.Get(SettingProvider),
            IsRememberBrowserEnabled = await SettingProvider.GetAsync<bool>(AccountSettingNames.TwoFactorLogin.IsRememberBrowserEnabled),
            UsersCanChange = await SettingProvider.GetAsync<bool>(IdentityProSettingNames.TwoFactor.UsersCanChange)
        };
    }

    public virtual async Task UpdateTwoFactorAsync(AccountTwoFactorSettingsDto input)
    {
        await CheckTwoFactorAvailableAsync();

        if (input != null)
        {
            await SettingManager.SetForCurrentTenantAsync(IdentityProSettingNames.TwoFactor.Behaviour, input.TwoFactorBehaviour.ToString());
            if (input.TwoFactorBehaviour == IdentityProTwoFactorBehaviour.Optional)
            {
                await SettingManager.SetForCurrentTenantAsync(IdentityProSettingNames.TwoFactor.UsersCanChange, input.UsersCanChange.ToString());
            }

            await SettingManager.SetForCurrentTenantAsync(AccountSettingNames.TwoFactorLogin.IsRememberBrowserEnabled, input.IsRememberBrowserEnabled.ToString());
        }
    }

    public virtual async Task<AccountExternalProviderSettingsDto> GetExternalProviderAsync()
    {
        var externalProviderSettings = new AccountExternalProviderSettingsDto
        {
            VerifyPasswordDuringExternalLogin = await SettingProvider.GetAsync<bool>(AccountSettingNames.VerifyPasswordDuringExternalLogin),
            ExternalProviders = await ExternalProviderSettingsHelper.GetAllAsync()
        };

        foreach (var settingsProperty in externalProviderSettings.ExternalProviders.SelectMany(x => x.SecretProperties))
        {
            settingsProperty.Value = null;
        }

        return externalProviderSettings;
    }

    public virtual async Task UpdateExternalProviderAsync(AccountExternalProviderSettingsDto input)
    {
        await SettingManager.SetForCurrentTenantAsync(AccountSettingNames.VerifyPasswordDuringExternalLogin, input.VerifyPasswordDuringExternalLogin.ToString());

        var currentSettings = await ExternalProviderSettingsHelper.GetAllAsync();
        foreach (var externalProvider in input.ExternalProviders)
        {
            if (externalProvider.UseCustomSettings && externalProvider.Properties.All(x => x.Value.IsNullOrWhiteSpace()))
            {
                externalProvider.UseCustomSettings = false;
            }

            foreach (var secretProperty in externalProvider.SecretProperties.Where(secretProperty => secretProperty.Value.IsNullOrWhiteSpace()))
            {
                secretProperty.Value = currentSettings
                    .FirstOrDefault(x => x.Name.Equals(externalProvider.Name, StringComparison.OrdinalIgnoreCase))
                    ?.SecretProperties
                    .FirstOrDefault(x => x.Name.Equals(secretProperty.Name, StringComparison.OrdinalIgnoreCase))?.Value;
            }
        }

        var settings = input.ExternalProviders.Select(setting => new ExternalProviderSettings
        {
            Name = setting.Name,
            Enabled = setting.Enabled,
            EnabledForTenantUser = setting.EnabledForTenantUser,
            UseCustomSettings = setting.UseCustomSettings,
            Properties = setting.Properties,
            SecretProperties = setting.SecretProperties
        }).ToList();

        await ExternalProviderSettingsHelper.SetAsync(settings);
    }

    public virtual async Task<AccountIdleSettingsDto> GetIdleAsync()
    {
        return new AccountIdleSettingsDto
        {
            Enabled = await SettingProvider.GetAsync<bool>(AccountSettingNames.Idle.Enabled),
            IdleTimeoutMinutes = await SettingProvider.GetAsync<int>(AccountSettingNames.Idle.IdleTimeoutMinutes)
        };
    }

    public virtual async Task UpdateIdleAsync(AccountIdleSettingsDto input)
    {
        await SettingManager.SetForCurrentTenantAsync(AccountSettingNames.Idle.Enabled, input.Enabled.ToString());

        if (input.IdleTimeoutMinutes <= 0)
        {
            throw new UserFriendlyException(L["IdleTimeoutMinutesMustBeGreaterThanZero"]);
        }
        await SettingManager.SetForCurrentTenantAsync(AccountSettingNames.Idle.IdleTimeoutMinutes, input.IdleTimeoutMinutes.ToString());
    }

    protected virtual async Task CheckTwoFactorAvailableAsync()
    {
        var behaviour = await IdentityProTwoFactorBehaviourFeatureHelper.Get(FeatureChecker);
        if (behaviour == IdentityProTwoFactorBehaviour.Disabled)
        {
            throw new UserFriendlyException(L["TwoFactorHasBeenDisabled"]);
        }
    }

    public virtual async Task<AccountRecaptchaSettingsDto> GetRecaptchaAsync()
    {
        var settings = new AccountRecaptchaSettingsDto
        {
            UseCaptchaOnLogin = await SettingProvider.GetAsync<bool>(AccountSettingNames.Captcha.UseCaptchaOnLogin),
            UseCaptchaOnRegistration = await SettingProvider.GetAsync<bool>(AccountSettingNames.Captcha.UseCaptchaOnRegistration),
            VerifyBaseUrl = await SettingProvider.GetOrNullAsync(AccountSettingNames.Captcha.VerifyBaseUrl),
            SiteKey = await SettingProvider.GetOrNullAsync(AccountSettingNames.Captcha.SiteKey),
            SiteSecret = await SettingProvider.GetOrNullAsync(AccountSettingNames.Captcha.SiteSecret),
            Version = await SettingProvider.GetAsync<int>(AccountSettingNames.Captcha.Version),
            Score = await SettingProvider.GetAsync<double>(AccountSettingNames.Captcha.Score)
        };

        if (CurrentTenant.IsAvailable)
        {
            settings.SiteKey = await SettingManager.GetOrNullForTenantAsync(AccountSettingNames.Captcha.SiteKey, CurrentTenant.GetId(), false);
            settings.SiteSecret = await SettingManager.GetOrNullForTenantAsync(AccountSettingNames.Captcha.SiteSecret, CurrentTenant.GetId(), false);
        }

        return settings;
    }

    public virtual async Task UpdateRecaptchaAsync(AccountRecaptchaSettingsDto input)
    {
        if (!CurrentTenant.IsAvailable)
        {
            if ((input.UseCaptchaOnLogin || input.UseCaptchaOnRegistration) &&
                (input.SiteKey.IsNullOrWhiteSpace() || input.SiteSecret.IsNullOrWhiteSpace()))
            {
                throw new UserFriendlyException(L["InvalidSiteKeyOrSiteSecret"]);
            }

            await SettingManager.SetGlobalAsync(AccountSettingNames.Captcha.UseCaptchaOnLogin, input.UseCaptchaOnLogin.ToString());
            await SettingManager.SetGlobalAsync(AccountSettingNames.Captcha.UseCaptchaOnRegistration, input.UseCaptchaOnRegistration.ToString());
            await SettingManager.SetGlobalAsync(AccountSettingNames.Captcha.VerifyBaseUrl, input.VerifyBaseUrl);
            await SettingManager.SetGlobalAsync(AccountSettingNames.Captcha.SiteKey, input.SiteKey);
            await SettingManager.SetGlobalAsync(AccountSettingNames.Captcha.SiteSecret, input.SiteSecret);
            await SettingManager.SetGlobalAsync(AccountSettingNames.Captcha.Version, input.Version.ToString());
            await SettingManager.SetGlobalAsync(AccountSettingNames.Captcha.Score, input.Score.ToString());
        }
        else
        {
            var globalVersion = (await SettingManager.GetOrNullGlobalAsync(AccountSettingNames.Captcha.Version)).To<int>();

            if (globalVersion != input.Version &&
                (input.SiteKey.IsNullOrWhiteSpace() || input.SiteSecret.IsNullOrWhiteSpace()))
            {
                throw new UserFriendlyException(L["InvalidSiteKeyOrSiteSecret"]);
            }

            await SettingManager.SetForTenantAsync(CurrentTenant.GetId(), AccountSettingNames.Captcha.Version, input.Version.ToString());
            await SettingManager.SetForTenantAsync(CurrentTenant.GetId(), AccountSettingNames.Captcha.SiteKey, input.SiteKey.IsNullOrWhiteSpace() ? null : input.SiteKey);
            await SettingManager.SetForTenantAsync(CurrentTenant.GetId(), AccountSettingNames.Captcha.SiteSecret, input.SiteSecret.IsNullOrWhiteSpace() ? null : input.SiteSecret);
            await SettingManager.SetForTenantAsync(CurrentTenant.GetId(), AccountSettingNames.Captcha.Score, input.Score.ToString());
        }
    }
}
