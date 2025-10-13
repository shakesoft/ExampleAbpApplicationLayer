using System;
using System.IO;
using Medallion.Threading;
using Medallion.Threading.Redis;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using AbpSolution1.Localization;
using AbpSolution1.MultiTenancy;
using AbpSolution1.Web.Public.Menus;
using AbpSolution1.Web.Public.HealthChecks;
using StackExchange.Redis;
using Volo.Abp;
using Volo.Abp.Studio;
using Volo.Abp.AspNetCore.Authentication.OpenIdConnect;
using Volo.Abp.AspNetCore.Mvc.Client;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonX;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonX.Bundling;
using Volo.Abp.LeptonX.Shared;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Toolbars;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.AutoMapper;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Caching;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.DistributedLocking;
using Volo.Abp.Http.Client.IdentityModel.Web;
using Volo.Abp.Http.Client.Web;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Claims;
using Volo.Abp.UI;
using Volo.Abp.UI.Navigation;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.VirtualFileSystem;
using Owl.reCAPTCHA;
using Volo.Abp.GlobalFeatures;
using Volo.CmsKit.GlobalFeatures;
using Volo.CmsKit.Pro.Public.Web;
using Volo.CmsKit.Pro.Public.Web.Middlewares;
using Volo.CmsKit.Comments;
using Volo.Abp.Studio.Client.AspNetCore;

namespace AbpSolution1.Web.Public;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpCachingStackExchangeRedisModule),
    typeof(AbpDistributedLockingModule),
    typeof(AbpAspNetCoreSerilogModule),
    typeof(AbpStudioClientAspNetCoreModule),
    typeof(AbpAspNetCoreMvcUiLeptonXThemeModule),
    typeof(AbpSolution1HttpApiClientModule),
    typeof(CmsKitProPublicWebModule),
    typeof(AbpSolution1HttpApiModule),
    typeof(AbpAspNetCoreAuthenticationOpenIdConnectModule),
    typeof(AbpAspNetCoreMvcClientModule),
    typeof(AbpHttpClientWebModule),
    typeof(AbpHttpClientIdentityModelWebModule)
    )]
public class AbpSolution1WebPublicModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
        {
            options.AddAssemblyResource(
                typeof(AbpSolution1Resource),
                typeof(AbpSolution1DomainSharedModule).Assembly,
                typeof(AbpSolution1ApplicationContractsModule).Assembly,
                typeof(AbpSolution1WebPublicModule).Assembly
            );
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();

        if (!configuration.GetValue<bool>("App:DisablePII"))
        {
            Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
            Microsoft.IdentityModel.Logging.IdentityModelEventSource.LogCompleteSecurityArtifact = true;
        }

        ConfigureBundles();
        ConfigureUrls(configuration);
        ConfigureCache(configuration);
        ConfigureDataProtection(context, configuration, hostingEnvironment);
        ConfigureDistributedLocking(context, configuration);
        ConfigureMultiTenancy();
        ConfigureAuthentication(context, configuration);
        ConfigureAutoMapper();
        ConfigureVirtualFileSystem(hostingEnvironment);
        ConfigureNavigationServices(configuration);
        ConfigureTheme();
        ConfigureBackgroundJobs();
        ConfigureHealthChecks(context);

        context.Services.AddreCAPTCHAV3(o =>
        {
            o.SiteKey = "test";
            o.SiteSecret = "test";
        });

        Configure<CmsKitCommentOptions>(options =>
        {
            options.EntityTypes.Add(new CommentEntityTypeDefinition("SampleArticle"));
        });
    }

    private void ConfigureBundles()
    {
        Configure<AbpBundlingOptions>(options =>
        {
            options.StyleBundles.Configure(
                LeptonXThemeBundles.Styles.Global,
                bundle =>
                {
                    bundle.AddFiles("/global-styles.css");
                }
            );

            options.ScriptBundles.Configure(
                LeptonXThemeBundles.Scripts.Global,
                bundle =>
                {
                    bundle.AddFiles("/global-scripts.js");
                }
            );
        });
    }

    private void ConfigureBackgroundJobs()
    {
        Configure<AbpBackgroundJobOptions>(options =>
        {
            options.IsJobExecutionEnabled = false;
        });
    }
    
    private void ConfigureTheme()
    {
        Configure<LeptonXThemeOptions>(options =>
        {
            options.DefaultStyle = LeptonXStyleNames.System;
        });

        Configure<LeptonXThemeMvcOptions>(options =>
        {
            options.ApplicationLayout = LeptonXMvcLayouts.PublicWebsite;
        });
    }

    private void ConfigureHealthChecks(ServiceConfigurationContext context)
    {
        context.Services.AddAbpSolution1PublicHealthChecks();
    }

    private void ConfigureUrls(IConfiguration configuration)
    {
        Configure<AppUrlOptions>(options =>
        {
            options.Applications["MVC"].RootUrl = configuration["App:SelfUrl"];
        });
    }

    private void ConfigureCache(IConfiguration configuration)
    {
        Configure<AbpDistributedCacheOptions>(options =>
        {
            options.KeyPrefix = "AbpSolution1:";
        });
    }

    private void ConfigureMultiTenancy()
    {
        Configure<AbpMultiTenancyOptions>(options => { options.IsEnabled = MultiTenancyConsts.IsEnabled; });
    }

    private void ConfigureAuthentication(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddAuthentication(options =>
        {
            options.DefaultScheme = "Cookies";
            options.DefaultChallengeScheme = "oidc";
        })
        .AddCookie("Cookies", options =>
        {
            options.ExpireTimeSpan = TimeSpan.FromDays(365);
            options.CheckTokenExpiration();
        })
        .AddAbpOpenIdConnect("oidc", options =>
        {
            options.Authority = configuration["AuthServer:Authority"];
            options.RequireHttpsMetadata = configuration.GetValue<bool>("AuthServer:RequireHttpsMetadata");
            options.ResponseType = OpenIdConnectResponseType.CodeIdToken;

            options.ClientId = configuration["AuthServer:ClientId"];
            options.ClientSecret = configuration["AuthServer:ClientSecret"];

            options.SaveTokens = true;
            options.GetClaimsFromUserInfoEndpoint = true;

            options.Scope.Add("roles");
            options.Scope.Add("email");
            options.Scope.Add("phone");
            options.Scope.Add("AbpSolution1");
        });
        /*
        * This configuration is used when the AuthServer is running on the internal network such as docker or k8s.
        * Configuring the redirectin URLs for internal network and the web
        */
        if (configuration.GetValue<bool>("AuthServer:IsOnK8s"))
        {
            context.Services.Configure<OpenIdConnectOptions>("oidc", options =>
            {
                options.TokenValidationParameters.ValidIssuers = new[] {
                    configuration["AuthServer:MetaAddress"]!.EnsureEndsWith('/'),
                    configuration["AuthServer:Authority"]!.EnsureEndsWith('/')
                };

                options.MetadataAddress = configuration["AuthServer:MetaAddress"]!.EnsureEndsWith('/') +
                                            ".well-known/openid-configuration";

                var previousOnRedirectToIdentityProvider = options.Events.OnRedirectToIdentityProvider;
                options.Events.OnRedirectToIdentityProvider = async ctx =>
                {
                    // Intercept the redirection so the browser navigates to the right URL in your host
                    ctx.ProtocolMessage.IssuerAddress = configuration["AuthServer:Authority"]!.EnsureEndsWith('/') + "connect/authorize";

                    if (previousOnRedirectToIdentityProvider != null)
                    {
                        await previousOnRedirectToIdentityProvider(ctx);
                    }
                };
                var previousOnRedirectToIdentityProviderForSignOut =
                    options.Events.OnRedirectToIdentityProviderForSignOut;
                options.Events.OnRedirectToIdentityProviderForSignOut = async ctx =>
                {
                    // Intercept the redirection for signout so the browser navigates to the right URL in your host
                    ctx.ProtocolMessage.IssuerAddress = configuration["AuthServer:Authority"]!.EnsureEndsWith('/') + "connect/endsession";

                    if (previousOnRedirectToIdentityProviderForSignOut != null)
                    {
                        await previousOnRedirectToIdentityProviderForSignOut(ctx);
                    }
                };
            });
        }

        context.Services.Configure<AbpClaimsPrincipalFactoryOptions>(options =>
        {
            options.IsDynamicClaimsEnabled = true;
        });
    }

    private void ConfigureAutoMapper()
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<AbpSolution1WebPublicModule>();
        });
    }

    private void ConfigureVirtualFileSystem(IWebHostEnvironment hostingEnvironment)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<AbpSolution1WebPublicModule>();

            if (hostingEnvironment.IsDevelopment())
            {
                options.FileSets.ReplaceEmbeddedByPhysical<AbpSolution1DomainSharedModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}AbpSolution1.Domain.Shared", Path.DirectorySeparatorChar)));
                options.FileSets.ReplaceEmbeddedByPhysical<AbpSolution1ApplicationContractsModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}AbpSolution1.Application.Contracts", Path.DirectorySeparatorChar)));
                options.FileSets.ReplaceEmbeddedByPhysical<AbpSolution1WebPublicModule>(hostingEnvironment.ContentRootPath);
            }
        });
    }

    private void ConfigureNavigationServices(IConfiguration configuration)
    {
        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new AbpSolution1PublicMenuContributor(configuration));
        });

        Configure<AbpToolbarOptions>(options =>
        {
            options.Contributors.Add(new AbpSolution1ToolbarContributor());
        });
    }

    private void ConfigureDataProtection(
        ServiceConfigurationContext context,
        IConfiguration configuration,
        IWebHostEnvironment hostingEnvironment)
    {
        if (AbpStudioAnalyzeHelper.IsInAnalyzeMode)
        {
            return;
        }

        var dataProtectionBuilder = context.Services.AddDataProtection().SetApplicationName("AbpSolution1");
        if (!hostingEnvironment.IsDevelopment())
        {
            var redis = ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]!);
            dataProtectionBuilder.PersistKeysToStackExchangeRedis(redis, "AbpSolution1-Protection-Keys");
        }
    }
    
    private void ConfigureDistributedLocking(
        ServiceConfigurationContext context,
        IConfiguration configuration)
    {
        if (AbpStudioAnalyzeHelper.IsInAnalyzeMode)
        {
            return;
        }

        context.Services.AddSingleton<IDistributedLockProvider>(sp =>
        {
            var connection = ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]!);
            return new RedisDistributedSynchronizationProvider(connection.GetDatabase());
        });
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();

        if (GlobalFeatureManager.Instance.IsEnabled<UrlShortingFeature>())
        {
            app.UseMiddleware<UrlShortingMiddleware>();
        }
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseAbpRequestLocalization();

        if (!env.IsDevelopment())
        {
            app.UseErrorPage();
        }

        app.UseRouting();
        app.MapAbpStaticAssets();
        app.UseAbpStudioLink();
        app.UseAbpSecurityHeaders();
        app.UseAuthentication();

        if (MultiTenancyConsts.IsEnabled)
        {
            app.UseMultiTenancy();
        }

        app.UseDynamicClaims();
        app.UseAuthorization();
        app.UseAbpSerilogEnrichers();
        app.UseConfiguredEndpoints();
    }
}
