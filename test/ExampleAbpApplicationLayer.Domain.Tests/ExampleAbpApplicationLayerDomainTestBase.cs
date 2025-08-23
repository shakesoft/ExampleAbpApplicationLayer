using Volo.Abp.Modularity;

namespace ExampleAbpApplicationLayer;

/* Inherit from this class for your domain layer tests. */
public abstract class ExampleAbpApplicationLayerDomainTestBase<TStartupModule> : ExampleAbpApplicationLayerTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
