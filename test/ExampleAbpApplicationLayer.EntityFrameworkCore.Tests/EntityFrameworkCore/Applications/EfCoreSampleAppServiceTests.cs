using ExampleAbpApplicationLayer.Samples;
using Xunit;

namespace ExampleAbpApplicationLayer.EntityFrameworkCore.Applications;

[Collection(ExampleAbpApplicationLayerTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<ExampleAbpApplicationLayerEntityFrameworkCoreTestModule>
{

}
