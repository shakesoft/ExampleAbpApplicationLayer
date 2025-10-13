using Volo.Abp.Bundling;

namespace Volo.Abp.Account.Pro.Public.Blazor.WebAssembly;

public class AccountPublicBundleContributor : IBundleContributor
{
    public void AddScripts(BundleContext context)
    {
        context.Add("_content/Volo.Abp.Account.Pro.Public.Blazor.Shared/IdleTracker.js");
    }

    public void AddStyles(BundleContext context)
    {
       
    }
}