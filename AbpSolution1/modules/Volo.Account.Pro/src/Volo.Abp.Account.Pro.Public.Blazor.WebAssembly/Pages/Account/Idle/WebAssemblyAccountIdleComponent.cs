using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Volo.Abp.Account.Pro.Public.Blazor.Shared.Pages.Account.Idle;

namespace Volo.Abp.Account.Pro.Public.Blazor.WebAssembly.Pages.Account.Idle;

public class WebAssemblyAccountIdleComponent : AccountIdleComponent
{
    protected override void NavigateToLogout()
    {
        if (AspNetCoreComponentsWebOptions.Value.IsBlazorWebApp)
        {
            NavigationManager.NavigateTo(AuthenticationOptions.Value.LogoutUrl, forceLoad: true);
        }
        else
        {
            NavigationManager.NavigateToLogout(AuthenticationOptions.Value.LogoutUrl);
        }
    }
}