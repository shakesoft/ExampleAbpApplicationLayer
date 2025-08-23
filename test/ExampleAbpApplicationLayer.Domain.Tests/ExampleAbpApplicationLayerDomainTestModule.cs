using Volo.Abp.Modularity;

namespace ExampleAbpApplicationLayer;

[DependsOn(
    typeof(ExampleAbpApplicationLayerDomainModule),
    typeof(ExampleAbpApplicationLayerTestBaseModule)
)]
public class ExampleAbpApplicationLayerDomainTestModule : AbpModule
{

}
