using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using ExampleAbpApplicationLayer.Orders;
using ExampleAbpApplicationLayer.EntityFrameworkCore;
using Xunit;

namespace ExampleAbpApplicationLayer.EntityFrameworkCore.Domains.Orders
{
    public class OrderRepositoryTests : ExampleAbpApplicationLayerEntityFrameworkCoreTestBase
    {
        private readonly IOrderRepository _orderRepository;

        public OrderRepositoryTests()
        {
            _orderRepository = GetRequiredService<IOrderRepository>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _orderRepository.GetListAsync(
                    status: default
                );

                // Assert
                result.Count.ShouldBe(1);
                result.FirstOrDefault().ShouldNotBe(null);
                result.First().Id.ShouldBe(Guid.Parse("5474ba67-048e-4654-a33f-6df2eba8e668"));
            });
        }

        [Fact]
        public async Task GetCountAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _orderRepository.GetCountAsync(
                    status: default
                );

                // Assert
                result.ShouldBe(1);
            });
        }
    }
}