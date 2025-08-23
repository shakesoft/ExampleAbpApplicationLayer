using Xunit;

namespace ExampleAbpApplicationLayer.EntityFrameworkCore;

[CollectionDefinition(ExampleAbpApplicationLayerTestConsts.CollectionDefinitionName)]
public class ExampleAbpApplicationLayerEntityFrameworkCoreCollection : ICollectionFixture<ExampleAbpApplicationLayerEntityFrameworkCoreFixture>
{

}
