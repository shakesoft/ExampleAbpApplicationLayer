using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ExampleAbpApplicationLayer.EntityFrameworkCore;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands) */
public class ExampleAbpApplicationLayerDbContextFactory : IDesignTimeDbContextFactory<ExampleAbpApplicationLayerDbContext>
{
    public ExampleAbpApplicationLayerDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();
        
        ExampleAbpApplicationLayerEfCoreEntityExtensionMappings.Configure();

        var builder = new DbContextOptionsBuilder<ExampleAbpApplicationLayerDbContext>()
            .UseSqlServer(configuration.GetConnectionString("Default"));
        
        return new ExampleAbpApplicationLayerDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../ExampleAbpApplicationLayer.DbMigrator/"))
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
