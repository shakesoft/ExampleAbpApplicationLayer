using ExampleAbpApplicationLayer.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace ExampleAbpApplicationLayer.Permissions;

public class ExampleAbpApplicationLayerPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(ExampleAbpApplicationLayerPermissions.GroupName);

        myGroup.AddPermission(ExampleAbpApplicationLayerPermissions.Dashboard.Host, L("Permission:Dashboard"), MultiTenancySides.Host);
        myGroup.AddPermission(ExampleAbpApplicationLayerPermissions.Dashboard.Tenant, L("Permission:Dashboard"), MultiTenancySides.Tenant);

        //Define your own permissions here. Example:
        //myGroup.AddPermission(ExampleAbpApplicationLayerPermissions.MyPermission1, L("Permission:MyPermission1"));

        var productPermission = myGroup.AddPermission(ExampleAbpApplicationLayerPermissions.Products.Default, L("Permission:Products"));
        productPermission.AddChild(ExampleAbpApplicationLayerPermissions.Products.Create, L("Permission:Create"));
        productPermission.AddChild(ExampleAbpApplicationLayerPermissions.Products.Edit, L("Permission:Edit"));
        productPermission.AddChild(ExampleAbpApplicationLayerPermissions.Products.Delete, L("Permission:Delete"));

        var orderPermission = myGroup.AddPermission(ExampleAbpApplicationLayerPermissions.Orders.Default, L("Permission:Orders"));
        orderPermission.AddChild(ExampleAbpApplicationLayerPermissions.Orders.Create, L("Permission:Create"));
        orderPermission.AddChild(ExampleAbpApplicationLayerPermissions.Orders.Edit, L("Permission:Edit"));
        orderPermission.AddChild(ExampleAbpApplicationLayerPermissions.Orders.Delete, L("Permission:Delete"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<ExampleAbpApplicationLayerResource>(name);
    }
}