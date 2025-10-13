using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Features;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;
using Volo.Abp.TextTemplating.Scriban;

namespace Volo.Abp.AuditLogging;

[DependsOn(
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationAbstractionsModule),
    typeof(AbpAuditLoggingDomainSharedModule),
    typeof(AbpFeaturesModule),
    typeof(AbpTextTemplatingScribanModule))]
public class AbpAuditLoggingApplicationContractsModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<AbpAuditLoggingApplicationContractsModule>();
        });
    }
}
