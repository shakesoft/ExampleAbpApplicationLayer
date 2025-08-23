using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ExampleAbpApplicationLayer.Data;
using Volo.Abp.DependencyInjection;

namespace ExampleAbpApplicationLayer.EntityFrameworkCore;

public class EntityFrameworkCoreExampleAbpApplicationLayerDbSchemaMigrator
    : IExampleAbpApplicationLayerDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreExampleAbpApplicationLayerDbSchemaMigrator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the ExampleAbpApplicationLayerDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<ExampleAbpApplicationLayerDbContext>()
            .Database
            .MigrateAsync();
    }
}
