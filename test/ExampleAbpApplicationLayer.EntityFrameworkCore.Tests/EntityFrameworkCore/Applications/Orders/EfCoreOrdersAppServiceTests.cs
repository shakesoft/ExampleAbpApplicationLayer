using ExampleAbpApplicationLayer.Orders;
using Xunit;
using ExampleAbpApplicationLayer.EntityFrameworkCore;

namespace ExampleAbpApplicationLayer.Orders;

public class EfCoreOrdersAppServiceTests : OrdersAppServiceTests<ExampleAbpApplicationLayerEntityFrameworkCoreTestModule>
{
}