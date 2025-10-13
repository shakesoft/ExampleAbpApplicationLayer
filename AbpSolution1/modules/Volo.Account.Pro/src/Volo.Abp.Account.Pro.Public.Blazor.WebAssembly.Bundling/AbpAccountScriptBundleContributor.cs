using Volo.Abp.AspNetCore.Mvc.UI.Bundling;

namespace Volo.Abp.Account.Pro.Public.Blazor.WebAssembly.Bundling;

public class AbpAccountScriptBundleContributor : BundleContributor
{
    public override void ConfigureBundle(BundleConfigurationContext context)
    {
        context.Files.AddIfNotContains("_content/Volo.Abp.Account.Pro.Public.Blazor.Shared/IdleTracker.js");
    }
}