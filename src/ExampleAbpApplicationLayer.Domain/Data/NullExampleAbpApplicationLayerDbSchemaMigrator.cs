using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace ExampleAbpApplicationLayer.Data;

/* This is used if database provider does't define
 * IExampleAbpApplicationLayerDbSchemaMigrator implementation.
 */
public class NullExampleAbpApplicationLayerDbSchemaMigrator : IExampleAbpApplicationLayerDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
