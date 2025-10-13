namespace Volo.Abp.Account.Pro.Public.Web.Shared.Pages.Account.Idle;

public class AccountIdleWebOptions
{
    public string LogoutUrl { get; set; }

    public AccountIdleWebOptions()
    {
        LogoutUrl = "Account/Logout";
    }
}
