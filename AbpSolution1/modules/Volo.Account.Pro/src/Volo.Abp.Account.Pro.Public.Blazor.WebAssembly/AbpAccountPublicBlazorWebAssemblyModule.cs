using Volo.Abp.Account.Pro.Public.Blazor.Shared;
using Volo.Abp.Account.Pro.Public.Blazor.WebAssembly.Pages.Account.Idle;
using Volo.Abp.AspNetCore.Components.WebAssembly.Theming;
using Volo.Abp.Modularity;
using Volo.Abp.Ui.LayoutHooks;

namespace Volo.Abp.Account.Pro.Public.Blazor.WebAssembly;

[DependsOn(
    typeof(AbpAccountPublicBlazorSharedModule),
    typeof(AbpAccountPublicHttpApiClientModule),
    typeof(AbpAspNetCoreComponentsWebAssemblyThemingModule)
)]
public class AbpAccountPublicBlazorWebAssemblyModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpLayoutHookOptions>(options =>
        {
            options.Add(LayoutHooks.Body.Last, typeof(WebAssemblyAccountIdleComponent));
        });
    }
}