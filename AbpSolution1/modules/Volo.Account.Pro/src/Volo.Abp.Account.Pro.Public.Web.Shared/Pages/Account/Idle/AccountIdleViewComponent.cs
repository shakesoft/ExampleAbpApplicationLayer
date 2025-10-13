using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Volo.Abp.Account.Settings;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Settings;

namespace Volo.Abp.Account.Pro.Public.Web.Shared.Pages.Account.Idle;

public class AccountIdleViewComponent : AbpViewComponent
{
    private AccountIdleWebCheckService AccountIdleWebCheckService { get; }
    private ISettingProvider SettingProvider { get; }

    public AccountIdleViewComponent(AccountIdleWebCheckService accountIdleWebCheckService, ISettingProvider settingProvider)
    {
        AccountIdleWebCheckService = accountIdleWebCheckService;
        SettingProvider = settingProvider;
    }

    public virtual async Task<IViewComponentResult> InvokeAsync()
    {
        if (!await AccountIdleWebCheckService.IsEnabledAsync(HttpContext))
        {
            return Content(string.Empty);
        }

        if (!await SettingProvider.GetAsync<bool>(AccountSettingNames.Idle.Enabled))
        {
            return Content(string.Empty);
        }

        var minutes = await SettingProvider.GetAsync<int>(AccountSettingNames.Idle.IdleTimeoutMinutes);
        return View("~/Pages/Account/Idle/Default.cshtml", new AccountIdleViewModel
        {
            IdleTimeoutMinutes = minutes,
            LogoutUrl = this.HttpContext.RequestServices.GetRequiredService<IOptions<AccountIdleWebOptions>>().Value.LogoutUrl
        });
    }
}

public class AccountIdleViewModel
{
    public int IdleTimeoutMinutes { get; set; }

    public string LogoutUrl { get; set; }
}
