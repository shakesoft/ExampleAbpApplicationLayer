using ExampleAbpApplicationLayer.Samples;
using Xunit;

namespace ExampleAbpApplicationLayer.EntityFrameworkCore.Domains;

[Collection(ExampleAbpApplicationLayerTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<ExampleAbpApplicationLayerEntityFrameworkCoreTestModule>
{

}
