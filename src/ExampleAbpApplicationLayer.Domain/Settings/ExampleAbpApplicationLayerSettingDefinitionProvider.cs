using Volo.Abp.Settings;

namespace ExampleAbpApplicationLayer.Settings;

public class ExampleAbpApplicationLayerSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(ExampleAbpApplicationLayerSettings.MySetting1));
    }
}
