using System;
using System.Collections.Generic;
using System.Linq;

namespace Volo.Abp.Account.ExternalProviders;

[Serializable]
public class ExternalProviderSettings
{
    public string Name { get; set; }

    public bool Enabled { get; set; }

    public bool EnabledForTenantUser { get; set; }

    public bool UseCustomSettings { get; set; }

    public List<ExternalProviderSettingsProperty> Properties { get; set; }

    public List<ExternalProviderSettingsProperty> SecretProperties { get; set; }

    public ExternalProviderSettings()
    {
        Properties = new List<ExternalProviderSettingsProperty>();
        SecretProperties = new List<ExternalProviderSettingsProperty>();
    }

    public ExternalProviderSettings MergeWith(ExternalProviderSettings setting)
    {
        Name = setting.Name;
        Enabled = setting.Enabled;
        EnabledForTenantUser = setting.EnabledForTenantUser;
        UseCustomSettings = setting.UseCustomSettings;
        Properties = setting.Properties;
        SecretProperties = setting.SecretProperties;

        return this;
    }

    public bool HasAnyProperty()
    {
        return Properties.Any(x => !x.Value.IsNullOrWhiteSpace()) ||
               SecretProperties.Any(x => !x.Value.IsNullOrWhiteSpace());
    }

    public void RemovePropertyValue()
    {
        foreach (var property in Properties)
        {
            property.Value = null;
        }
        foreach (var property in SecretProperties)
        {
            property.Value = null;
        }
    }
}

[Serializable]
public class ExternalProviderHostSettings
{
    public string Name { get; set; }

    public bool Enabled { get; set; }

    public bool EnabledForTenantUser { get; set; }

    public List<ExternalProviderSettingsProperty> Properties { get; set; }

    public List<ExternalProviderSettingsProperty> SecretProperties { get; set; }

    public ExternalProviderHostSettings()
    {
        Properties = new List<ExternalProviderSettingsProperty>();
        SecretProperties = new List<ExternalProviderSettingsProperty>();
    }
}

[Serializable]
public class ExternalProviderTenantSettings
{
    public string Name { get; set; }

    public bool Enabled { get; set; }

    public List<ExternalProviderSettingsProperty> Properties { get; set; }

    public List<ExternalProviderSettingsProperty> SecretProperties { get; set; }

    public ExternalProviderTenantSettings()
    {
        Properties = new List<ExternalProviderSettingsProperty>();
        SecretProperties = new List<ExternalProviderSettingsProperty>();
    }
}
