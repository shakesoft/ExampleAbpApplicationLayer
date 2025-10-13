using Volo.Abp.AspNetCore.Mvc.UI.Bundling;

namespace Volo.Abp.AuditLogging.Blazor.WebAssembly.Bundling;

public class AuditLoggingScriptBundleContributor : BundleContributor
{
    public override void ConfigureBundle(BundleConfigurationContext context)
    {
        context.Files.AddIfNotContains("_content/Volo.Abp.AuditLogging.Blazor/libs/chart/chart.min.js");
        context.Files.AddIfNotContains("_content/Volo.Abp.AuditLogging.Blazor/libs/excel/export.js");
    }
}
