using Volo.Abp.Modularity;

namespace ExampleAbpApplicationLayer;

public abstract class ExampleAbpApplicationLayerApplicationTestBase<TStartupModule> : ExampleAbpApplicationLayerTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
