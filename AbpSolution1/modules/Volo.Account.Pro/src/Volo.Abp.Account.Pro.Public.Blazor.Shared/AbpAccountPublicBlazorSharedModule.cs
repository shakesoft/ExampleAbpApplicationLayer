using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AspNetCore.Components.Web.Theming;
using Volo.Abp.Modularity;
using Volo.Abp.Ui.LayoutHooks;
using Volo.Abp.Account.Pro.Public.Blazor.Shared.Pages.Account.Idle;

namespace Volo.Abp.Account.Pro.Public.Blazor.Shared;

[DependsOn(typeof(AbpAspNetCoreComponentsWebThemingModule))]
[DependsOn(typeof(AbpAccountPublicApplicationContractsModule))]
public class AbpAccountPublicBlazorSharedModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddScoped<AbpIdleTrackerService<AccountIdleComponent>>();
    }
}
