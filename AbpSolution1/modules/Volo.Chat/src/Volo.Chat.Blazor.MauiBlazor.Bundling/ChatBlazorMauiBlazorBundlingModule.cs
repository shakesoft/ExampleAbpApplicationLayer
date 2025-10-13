using Volo.Abp.AspNetCore.Components.MauiBlazor.Theming.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.Modularity;

namespace Volo.Chat.Blazor.MauiBlazor.Bundling;

[DependsOn(
    typeof(AbpAspNetCoreComponentsMauiBlazorThemingBundlingModule)
    )]
public class ChatBlazorMauiBlazorBundlingModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpBundlingOptions>(options =>
        {
            options.ScriptBundles.Get(MauiBlazorStandardBundles.Scripts.Global).AddContributors(typeof(ChatScriptBundleContributor));
        });
    }
}