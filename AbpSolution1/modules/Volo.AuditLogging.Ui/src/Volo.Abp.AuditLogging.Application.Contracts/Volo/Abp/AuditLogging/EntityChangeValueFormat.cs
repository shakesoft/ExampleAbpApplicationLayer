using System;
using System.Globalization;
using System.Text.Json;

namespace Volo.Abp.AuditLogging;

// Todo: Move this class to Domain Shared
public static class EntityChangeValueFormatter
{
    public static string Format(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        if(DateTime.TryParse(value.Trim('"'), out var formattedValue))
        {
            return formattedValue.ToString(CultureInfo.CurrentCulture);
        }

        try
        {
            var json = JsonDocument.Parse(value);
            if (json.RootElement.ValueKind == JsonValueKind.Object || json.RootElement.ValueKind == JsonValueKind.Array)
            {
                return JsonSerializer.Serialize(json.RootElement, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
            }
        }
        catch (Exception)
        {
            // ignored
        }

        return value;
    }
}
