using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Volo.Abp.Account.Settings;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Json;
using Volo.Abp.MultiTenancy;
using Volo.Abp.SettingManagement;
using Volo.Abp.Settings;

namespace Volo.Abp.Account.ExternalProviders;

public class ExternalProviderSettingsHelper : ITransientDependency
{
    private readonly ICurrentTenant _currentTenant;
    private readonly AbpExternalProviderOptions _externalProviderOptions;
    private readonly ISettingManager _settingManager;
    private readonly IJsonSerializer _jsonSerializer;
    private readonly ISettingEncryptionService _stringEncryptionService;
    private readonly ISettingDefinitionManager _settingDefinitionManager;

    public ExternalProviderSettingsHelper(
        ICurrentTenant currentTenant,
        IOptions<AbpExternalProviderOptions> externalProvidersOptions,
        ISettingManager settingManager,
        IJsonSerializer jsonSerializer,
        ISettingEncryptionService stringEncryptionService,
        ISettingDefinitionManager settingDefinitionManager)
    {
        _currentTenant = currentTenant;
        _externalProviderOptions = externalProvidersOptions.Value;
        _settingManager = settingManager;
        _jsonSerializer = jsonSerializer;
        _stringEncryptionService = stringEncryptionService;
        _settingDefinitionManager = settingDefinitionManager;
    }

    public virtual async Task<List<ExternalProviderSettings>> GetAllAsync()
    {
        return _currentTenant.IsAvailable
            ? await GetAllForTenantAsync(_currentTenant.GetId())
            : await GetAllForHostAsync();
    }

    public virtual async Task<List<ExternalProviderSettings>> GetAllForHostAsync()
    {
        return await GetAllInternalAsync(null);
    }

    public virtual async Task<List<ExternalProviderSettings>> GetAllForTenantAsync(Guid tenantId)
    {
        var hostSettings = await GetAllInternalAsync(null);
        var tenantSettings = await GetAllInternalAsync(tenantId);

        // Override tenant settings with host settings for empty values
        foreach (var tenantSetting in tenantSettings)
        {
            var hostSetting = hostSettings?.FirstOrDefault(x => x.Name == tenantSetting.Name);
            if (hostSetting == null)
            {
                tenantSetting.Enabled = false;
                tenantSetting.EnabledForTenantUser = false;
                tenantSetting.UseCustomSettings = false;
                tenantSetting.RemovePropertyValue();
                continue;
            }

            tenantSetting.UseCustomSettings = tenantSetting.HasAnyProperty();

            foreach (var property in tenantSetting.Properties.Where(x => x.Value.IsNullOrWhiteSpace()))
            {
                property.Value = hostSetting.Properties
                    .FirstOrDefault(x => x.Name.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase))?.Value;
            }

            foreach (var property in tenantSetting.SecretProperties.Where(x => x.Value.IsNullOrWhiteSpace()))
            {
                property.Value = hostSetting.SecretProperties
                    .FirstOrDefault(x => x.Name.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase))?.Value;
            }

            if (!hostSetting.EnabledForTenantUser)
            {
                tenantSetting.Enabled = false;
                tenantSetting.EnabledForTenantUser = false;
            }
            else
            {
                var tenantSettingString = await _settingManager.GetOrNullForTenantAsync(AccountSettingNames.ExternalProviders, tenantId, fallback: false) ?? string.Empty;
                var hasTenantValue = tenantSettingString.Contains(tenantSetting.Name, StringComparison.OrdinalIgnoreCase);
                if (!hasTenantValue)
                {
                    tenantSetting.Enabled = true;
                }
                tenantSetting.EnabledForTenantUser = true;
            }
        }

        return tenantSettings.ToList();
    }

    protected virtual async Task<List<ExternalProviderSettings>> GetAllInternalAsync(Guid? tenantId)
    {
        var allSettings = new List<ExternalProviderSettings>();
        var settings = await GetSettingsOrNullAsync(tenantId);
        foreach (var definition in _externalProviderOptions.Definitions)
        {
            var setting = CreateEmptySetting(definition);

            var existSetting = settings?.FirstOrDefault(x => x.Name == definition.Name);
            if (existSetting != null)
            {
                setting.MergeWith(existSetting);
            }

            var settingDefinition = await _settingDefinitionManager.GetAsync(AccountSettingNames.ExternalProviders);
            foreach (var secretProperty in setting.SecretProperties)
            {
                if (!secretProperty.Value.IsNullOrWhiteSpace())
                {
                    secretProperty.Value = _stringEncryptionService.Decrypt(settingDefinition, secretProperty.Value);
                }
            }

            allSettings.Add(setting);
        }

        return allSettings;
    }

    public virtual async Task<ExternalProviderSettings> GetByNameAsync(string name)
    {
        return (await GetAllAsync()).FirstOrDefault(x => x.Name == name);
    }

    protected virtual ExternalProviderDefinition GetDefinitionsByNameOrNull(string name)
    {
        return _externalProviderOptions.Definitions.FirstOrDefault(x => x.Name == name);
    }

    public virtual async Task SetAsync(List<ExternalProviderSettings> settings)
    {
        var existSettings = (_currentTenant.IsAvailable
            ? await GetSettingsOrNullAsync(_currentTenant.Id)
            : await GetSettingsOrNullAsync(null)) ?? new List<ExternalProviderSettings>();

        foreach (var setting in settings)
        {
            var definition = GetDefinitionsByNameOrNull(setting.Name);
            if (definition == null)
            {
                throw new Exception($"External provider with {setting.Name} not definition!");
            }

            var newSetting = CreateEmptySetting(definition).MergeWith(setting);
            var settingDefinition = await _settingDefinitionManager.GetAsync(AccountSettingNames.ExternalProviders);
            foreach (var secretProperty in newSetting.SecretProperties)
            {
                secretProperty.Value = _stringEncryptionService.Encrypt(settingDefinition, secretProperty.Value);
            }

            if (_currentTenant.IsAvailable)
            {
                if (newSetting.UseCustomSettings)
                {
                    var existSetting = existSettings.FirstOrDefault(x => x.Name == newSetting.Name);
                    if (existSetting != null)
                    {
                        foreach (var property in newSetting.SecretProperties.Where(x => x.Value.IsNullOrWhiteSpace()))
                        {
                            property.Value = existSetting.SecretProperties.FirstOrDefault(x =>
                                x.Name.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase))?.Value;
                        }
                    }
                }
                else
                {
                    newSetting.RemovePropertyValue();
                }
            }

            existSettings.RemoveAll(x => x.Name == newSetting.Name);
            existSettings.Add(newSetting);
        }

        string settingValue = null;
        if ( _currentTenant.IsAvailable)
        {
            existSettings.RemoveAll(x => x.Enabled && !x.HasAnyProperty());
            if (existSettings.Any())
            {
                settingValue = _jsonSerializer.Serialize(existSettings.Select(x => new ExternalProviderTenantSettings
                {
                    Name = x.Name,
                    Enabled = x.Enabled,
                    Properties = x.Properties,
                    SecretProperties = x.SecretProperties
                }));
            }
        }
        else
        {
            existSettings.RemoveAll(x => !x.Enabled && !x.EnabledForTenantUser && !x.HasAnyProperty());
            if (existSettings.Any())
            {
                settingValue = _jsonSerializer.Serialize(existSettings.Select(x => new ExternalProviderHostSettings
                {
                    Name = x.Name,
                    Enabled = x.Enabled,
                    EnabledForTenantUser = x.EnabledForTenantUser,
                    Properties = x.Properties,
                    SecretProperties = x.SecretProperties
                }));
            }
        }

        await _settingManager.SetForTenantOrGlobalAsync(_currentTenant.Id, AccountSettingNames.ExternalProviders, settingValue, true);
    }

    protected virtual async Task<List<ExternalProviderSettings>> GetSettingsOrNullAsync(Guid? tenantId)
    {
        var settingsString = tenantId.HasValue
            ? await _settingManager.GetOrNullForTenantAsync(AccountSettingNames.ExternalProviders, tenantId.Value, fallback: false)
            : await _settingManager.GetOrNullGlobalAsync(AccountSettingNames.ExternalProviders, fallback: false);

        if (settingsString.IsNullOrWhiteSpace())
        {
            return null;
        }

        var settings = new List<ExternalProviderSettings>();
        var jsonArray = JsonDocument.Parse(settingsString).RootElement.EnumerateArray();
        foreach (var obj in jsonArray)
        {
           var setting = _jsonSerializer.Deserialize<ExternalProviderSettings>(obj.GetRawText());
           if (!setting.EnabledForTenantUser)
           {
               // Compatible with old version,
               // The EnabledForTenantUser is new added, if not exists, set EnabledForTenantUser to Enabled.
               var hasEnabledForTenantUser = obj.GetRawText().Contains("EnabledForTenantUser", StringComparison.InvariantCultureIgnoreCase);
               if (!hasEnabledForTenantUser && setting.Enabled)
               {
                   setting.EnabledForTenantUser = true;
               }
           }
           settings.Add(setting);
        }

        return settings;
    }

    protected virtual ExternalProviderSettings CreateEmptySetting(ExternalProviderDefinition definition)
    {
        return new ExternalProviderSettings
        {
            Name = definition.Name,
            Enabled = false,
            EnabledForTenantUser = false,

            Properties = definition.Properties.
                Where(x => !x.IsSecret).
                Select(x => new ExternalProviderSettingsProperty(x.PropertyName, null)).
                ToList(),

            SecretProperties = definition.Properties.
                Where(x => x.IsSecret).
                Select(x => new ExternalProviderSettingsProperty(x.PropertyName, null)).
                ToList()
        };
    }
}
