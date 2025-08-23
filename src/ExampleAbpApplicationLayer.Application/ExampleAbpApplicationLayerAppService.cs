using ExampleAbpApplicationLayer.Localization;
using Volo.Abp.Application.Services;

namespace ExampleAbpApplicationLayer;

/* Inherit your application services from this class.
 */
public abstract class ExampleAbpApplicationLayerAppService : ApplicationService
{
    protected ExampleAbpApplicationLayerAppService()
    {
        LocalizationResource = typeof(ExampleAbpApplicationLayerResource);
    }
}
