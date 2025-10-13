using Volo.Abp.AspNetCore.Mvc.UI.Bundling;

namespace Volo.Chat.Blazor.WebAssembly.Bundling;

public class ChatScriptBundleContributor : BundleContributor
{
    public override void ConfigureBundle(BundleConfigurationContext context)
    {
        context.Files.AddIfNotContains("_content/Volo.Chat.Blazor/libs/AvatarManager.js");
    }
}