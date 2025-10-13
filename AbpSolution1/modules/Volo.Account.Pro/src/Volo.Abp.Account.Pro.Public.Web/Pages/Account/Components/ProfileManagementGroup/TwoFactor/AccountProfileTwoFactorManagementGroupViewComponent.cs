using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Identity;

namespace Volo.Abp.Account.Public.Web.Pages.Account.Components.ProfileManagementGroup.TwoFactor;

public class AccountProfileTwoFactorManagementGroupViewComponent : AbpViewComponent
{
    protected IProfileAppService ProfileAppService;
    protected IAccountAppService AccountAppService;
    protected IdentityProTwoFactorManager IdentityProTwoFactorManager;

    public AccountProfileTwoFactorManagementGroupViewComponent(
        IProfileAppService profileAppService,
        IAccountAppService accountAppService,
        IdentityProTwoFactorManager identityProTwoFactorManager)
    {
        ProfileAppService = profileAppService;
        AccountAppService = accountAppService;
        IdentityProTwoFactorManager = identityProTwoFactorManager;
    }

    public virtual async Task<IViewComponentResult> InvokeAsync()
    {
        var authenticatorInfo = await AccountAppService.GetAuthenticatorInfoAsync();
        var model = new ChangeTwoFactorModel
        {
            TwoFactorForcedEnabled = await IdentityProTwoFactorManager.IsForcedEnableAsync(),
            TwoFactorEnabled = await ProfileAppService.GetTwoFactorEnabledAsync(),
            HasAuthenticator = await AccountAppService.HasAuthenticatorAsync(),
            SharedKey = authenticatorInfo.Key,
            AuthenticatorUri = authenticatorInfo.Uri
        };

        return View("~/Pages/Account/Components/ProfileManagementGroup/TwoFactor/Default.cshtml", model);
    }

    public class ChangeTwoFactorModel
    {
        public bool TwoFactorForcedEnabled { get; set; }

        public bool TwoFactorEnabled { get; set; }

        public bool HasAuthenticator { get; set; }

        public string SharedKey { get; set; }

        public string AuthenticatorUri { get; set; }

        public string Code { get; set; }
    }
}
