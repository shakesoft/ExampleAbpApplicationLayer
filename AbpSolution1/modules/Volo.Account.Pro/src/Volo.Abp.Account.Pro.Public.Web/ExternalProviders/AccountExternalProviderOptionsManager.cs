using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Volo.Abp.Account.ExternalProviders;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Options;
using Volo.Abp.Security.Encryption;

namespace Volo.Abp.Account.Public.Web.ExternalProviders;

public class AccountExternalProviderOptionsManager<TOptions> : AbpDynamicOptionsManager<TOptions>, IOptionsMonitor<TOptions>
    where TOptions : class, new()
{
    protected IAccountExternalProviderAppService AccountExternalProviderAppService { get; }
    protected IStringEncryptionService StringEncryptionService { get; }
    protected ITenantConfigurationProvider TenantConfigurationProvider { get; }
    protected IEnumerable<IPostConfigureAccountExternalProviderOptions<TOptions>> PostConfigures { get; }
    protected ICurrentTenant CurrentTenant { get; }

    public ILogger<AccountExternalProviderOptionsManager<TOptions>> Logger { get; set; }

    public AccountExternalProviderOptionsManager(IOptionsFactory<TOptions> factory,
        IAccountExternalProviderAppService accountExternalProviderAppService,
        IStringEncryptionService stringEncryptionService,
        ITenantConfigurationProvider tenantConfigurationProvider,
        IEnumerable<IPostConfigureAccountExternalProviderOptions<TOptions>> postConfigures,
        ICurrentTenant currentTenant)
        : base(factory)
    {
        AccountExternalProviderAppService = accountExternalProviderAppService;
        StringEncryptionService = stringEncryptionService;
        TenantConfigurationProvider = tenantConfigurationProvider;
        PostConfigures = postConfigures;
        CurrentTenant = currentTenant;

        Logger = NullLogger<AccountExternalProviderOptionsManager<TOptions>>.Instance;
    }

    protected async override Task OverrideOptionsAsync(string name, TOptions options)
    {
        var tenantId = CurrentTenant.Id;
        if (tenantId == null)
        {
            try
            {
                var tenant = await TenantConfigurationProvider.GetAsync(saveResolveResult: false);
                tenantId = tenant?.Id;
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return;
            }
        }

        var externalProviderItemDto = await AccountExternalProviderAppService.GetByNameAsync(new GetByNameInput
        {
            TenantId = tenantId,
            Name = name
        });

        var enabled = externalProviderItemDto.Success &&
                      externalProviderItemDto.Enabled &&
                      externalProviderItemDto.Properties.All(x => !x.Value.IsNullOrWhiteSpace()) &&
                      externalProviderItemDto.SecretProperties.All(x => !x.Value.IsNullOrWhiteSpace());
        if (tenantId != null)
        {
            enabled = enabled && externalProviderItemDto.EnabledForTenantUser;
        }

        if (enabled)
        {
            externalProviderItemDto.SecretProperties = externalProviderItemDto.SecretProperties
                .Select(secretValue => new ExternalProviderSettingsProperty(secretValue.Name, StringEncryptionService.Decrypt(secretValue.Value)))
                .ToList();

            var properties = externalProviderItemDto.Properties.Concat(externalProviderItemDto.SecretProperties).ToList();
            foreach (var property in properties)
            {
                var optionsProp = typeof(TOptions).GetProperty(property.Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (optionsProp != null && optionsProp.CanWrite && !property.Value.IsNullOrWhiteSpace())
                {
                    optionsProp.SetValue(options, property.Value);
                }
            }
        }

        foreach (var postConfigure in PostConfigures)
        {
            await postConfigure.PostConfigureAsync(name, options);
        }
    }

    public IDisposable OnChange(Action<TOptions, string> listener)
    {
        return NullDisposable.Instance;
    }

    public TOptions CurrentValue => Get(Microsoft.Extensions.Options.Options.DefaultName);
}
