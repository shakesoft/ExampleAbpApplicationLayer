using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AutoMapper;
using Volo.Abp.BlobStoring;
using Volo.Abp.Imaging;
using Volo.Abp.Modularity;
using Volo.Abp.Sms;
using Volo.Abp.Timing;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.VirtualFileSystem;

namespace Volo.Abp.Account;

[DependsOn(
    typeof(AbpAccountSharedApplicationModule),
    typeof(AbpSmsModule),
    typeof(AbpAutoMapperModule),
    typeof(AbpBlobStoringModule),
    typeof(AbpAccountPublicApplicationContractsModule),
    typeof(AbpImagingImageSharpModule),
    typeof(AbpTimingModule)
    )]
public class AbpAccountPublicApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AppUrlOptions>(options =>
        {
            options.Applications["MVC"].Urls[AccountUrlNames.PasswordReset] = "Account/ResetPassword";
            options.Applications["MVC"].Urls[AccountUrlNames.EmailConfirmation] = "Account/EmailConfirmation";
        });

        context.Services.AddAutoMapperObjectMapper<AbpAccountPublicApplicationModule>();
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddProfile<AbpAccountPubicApplicationModuleAutoMapperProfile>();
        });

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<AbpAccountPublicApplicationModule>();
        });

        context.Services.AddHttpClient();

        var configuration = context.Services.GetConfiguration();
        context.Services.Configure<AbpRegisterEmailConfirmationCodeOptions>(options =>
        {
            options.DailySendLimit = configuration.GetValue("Account:EmailConfirmation:DailySendLimit", 15);
            options.HourlySendLimit = configuration.GetValue("Account:EmailConfirmation:HourlySendLimit", 5);
            options.CodeExpirationTime = configuration.GetValue("Account:EmailConfirmation:CodeExpirationTime", TimeSpan.FromMinutes(10));
            options.MaxFailedCheckCount = configuration.GetValue("Account:EmailConfirmation:MaxFailedCheckCount", 5);
        });
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        
    }
}
