using Volo.Abp.AspNetCore.Components.MauiBlazor.Theming.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.Modularity;

namespace Volo.Abp.AuditLogging.Blazor.MauiBlazor.Bundling;

[DependsOn(
    typeof(AbpAspNetCoreComponentsMauiBlazorThemingBundlingModule)
)]
public class AbpAuditLoggingBlazorMauiBlazorBundlingModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpBundlingOptions>(options =>
        {
            var globalStyles = options.StyleBundles.Get(MauiBlazorStandardBundles.Styles.Global);
            globalStyles.AddContributors(typeof(AuditLoggingStyleBundleContributor));

            var globalScript = options.ScriptBundles.Get(MauiBlazorStandardBundles.Scripts.Global);
            globalScript.AddContributors(typeof(AuditLoggingScriptBundleContributor));
        });
    }
}