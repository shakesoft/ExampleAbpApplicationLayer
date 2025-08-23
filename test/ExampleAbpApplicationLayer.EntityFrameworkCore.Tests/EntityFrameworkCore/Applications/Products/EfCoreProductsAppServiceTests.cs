using ExampleAbpApplicationLayer.Products;
using Xunit;
using ExampleAbpApplicationLayer.EntityFrameworkCore;

namespace ExampleAbpApplicationLayer.Products;

public class EfCoreProductsAppServiceTests : ProductsAppServiceTests<ExampleAbpApplicationLayerEntityFrameworkCoreTestModule>
{
}