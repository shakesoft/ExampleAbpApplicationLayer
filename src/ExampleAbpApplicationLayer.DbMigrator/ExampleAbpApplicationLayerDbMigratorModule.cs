using ExampleAbpApplicationLayer.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace ExampleAbpApplicationLayer.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(ExampleAbpApplicationLayerEntityFrameworkCoreModule),
    typeof(ExampleAbpApplicationLayerApplicationContractsModule)
)]
public class ExampleAbpApplicationLayerDbMigratorModule : AbpModule
{
}
