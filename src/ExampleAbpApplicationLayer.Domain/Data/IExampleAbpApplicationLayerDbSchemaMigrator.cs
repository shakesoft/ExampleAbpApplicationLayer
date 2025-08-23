using System.Threading.Tasks;

namespace ExampleAbpApplicationLayer.Data;

public interface IExampleAbpApplicationLayerDbSchemaMigrator
{
    Task MigrateAsync();
}
