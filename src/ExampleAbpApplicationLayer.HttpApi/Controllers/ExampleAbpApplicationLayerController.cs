using ExampleAbpApplicationLayer.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace ExampleAbpApplicationLayer.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class ExampleAbpApplicationLayerController : AbpControllerBase
{
    protected ExampleAbpApplicationLayerController()
    {
        LocalizationResource = typeof(ExampleAbpApplicationLayerResource);
    }
}
