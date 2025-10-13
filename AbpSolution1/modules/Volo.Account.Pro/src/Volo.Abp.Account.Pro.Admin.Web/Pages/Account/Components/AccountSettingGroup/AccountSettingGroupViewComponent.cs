using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Features;
using Volo.Abp.Identity.Features;
using Volo.Abp.MultiTenancy;

namespace Volo.Abp.Account.Admin.Web.Pages.Account.Components.AccountSettingGroup;

public class AccountSettingGroupViewComponent : AbpViewComponent
{
    public AccountSettingsViewModel SettingsViewModel { get; set; }

    protected IAccountSettingsAppService AccountSettingsAppService { get; }
    protected IFeatureChecker FeatureChecker { get; }
    protected ICurrentTenant CurrentTenant  { get; }

    public AccountSettingGroupViewComponent(
        IAccountSettingsAppService accountSettingsAppService,
        IFeatureChecker featureChecker,
        ICurrentTenant currentTenant)
    {
        AccountSettingsAppService = accountSettingsAppService;
        FeatureChecker = featureChecker;
        CurrentTenant = currentTenant;
    }

    public virtual async Task<IViewComponentResult> InvokeAsync()
    {
        SettingsViewModel = new AccountSettingsViewModel
        {
            AccountSettings = await AccountSettingsAppService.GetAsync(),
            AccountRecaptchaSettings = await AccountSettingsAppService.GetRecaptchaAsync(),
            AccountExternalProviderSettings = await AccountSettingsAppService.GetExternalProviderAsync(),
            AccountIdleSettingsDto = await AccountSettingsAppService.GetIdleAsync()
        };

        if (await IdentityProTwoFactorBehaviourFeatureHelper.Get(FeatureChecker) == IdentityProTwoFactorBehaviour.Optional)
        {
            SettingsViewModel.AccountTwoFactorSettings = await AccountSettingsAppService.GetTwoFactorAsync();
        }

        return View("~/Pages/Account/Components/AccountSettingGroup/Default.cshtml", SettingsViewModel);
    }

    public class AccountSettingsViewModel
    {
        public AccountSettingsDto AccountSettings { get; set; }

        public AccountTwoFactorSettingsDto AccountTwoFactorSettings { get; set; }

        public AccountRecaptchaSettingsDto AccountRecaptchaSettings { get; set; }

        public AccountExternalProviderSettingsDto AccountExternalProviderSettings { get; set; }

        public AccountIdleSettingsDto AccountIdleSettingsDto { get; set; }
    }
}
