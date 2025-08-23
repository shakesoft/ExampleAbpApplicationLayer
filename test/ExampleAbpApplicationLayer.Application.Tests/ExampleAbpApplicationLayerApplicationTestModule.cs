using Volo.Abp.Modularity;

namespace ExampleAbpApplicationLayer;

[DependsOn(
    typeof(ExampleAbpApplicationLayerApplicationModule),
    typeof(ExampleAbpApplicationLayerDomainTestModule)
)]
public class ExampleAbpApplicationLayerApplicationTestModule : AbpModule
{

}
