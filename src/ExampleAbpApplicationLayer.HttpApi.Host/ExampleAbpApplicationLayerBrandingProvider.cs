using Microsoft.Extensions.Localization;
using ExampleAbpApplicationLayer.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace ExampleAbpApplicationLayer;

[Dependency(ReplaceServices = true)]
public class ExampleAbpApplicationLayerBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<ExampleAbpApplicationLayerResource> _localizer;

    public ExampleAbpApplicationLayerBrandingProvider(IStringLocalizer<ExampleAbpApplicationLayerResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
