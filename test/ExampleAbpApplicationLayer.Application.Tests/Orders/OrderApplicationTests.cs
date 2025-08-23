using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace ExampleAbpApplicationLayer.Orders
{
    public abstract class OrdersAppServiceTests<TStartupModule> : ExampleAbpApplicationLayerApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly IOrdersAppService _ordersAppService;
        private readonly IRepository<Order, Guid> _orderRepository;

        public OrdersAppServiceTests()
        {
            _ordersAppService = GetRequiredService<IOrdersAppService>();
            _orderRepository = GetRequiredService<IRepository<Order, Guid>>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Act
            var result = await _ordersAppService.GetListAsync(new GetOrdersInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Id == Guid.Parse("dcdc247a-f601-486e-9594-a3c6c9351c1c")).ShouldBe(true);
            result.Items.Any(x => x.Id == Guid.Parse("53a5e56a-5b6f-4f0f-a42e-3562b6baf84c")).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Act
            var result = await _ordersAppService.GetAsync(Guid.Parse("dcdc247a-f601-486e-9594-a3c6c9351c1c"));

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(Guid.Parse("dcdc247a-f601-486e-9594-a3c6c9351c1c"));
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new OrderCreateDto
            {
                OrderDate = new DateTime(2011, 6, 5),
                TotalAmount = 704622860,
                Status = default
            };

            // Act
            var serviceResult = await _ordersAppService.CreateAsync(input);

            // Assert
            var result = await _orderRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.OrderDate.ShouldBe(new DateTime(2011, 6, 5));
            result.TotalAmount.ShouldBe(704622860);
            result.Status.ShouldBe(default);
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var input = new OrderUpdateDto()
            {
                OrderDate = new DateTime(2006, 11, 2),
                TotalAmount = 151144323,
                Status = default
            };

            // Act
            var serviceResult = await _ordersAppService.UpdateAsync(Guid.Parse("dcdc247a-f601-486e-9594-a3c6c9351c1c"), input);

            // Assert
            var result = await _orderRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.OrderDate.ShouldBe(new DateTime(2006, 11, 2));
            result.TotalAmount.ShouldBe(151144323);
            result.Status.ShouldBe(default);
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Act
            await _ordersAppService.DeleteAsync(Guid.Parse("dcdc247a-f601-486e-9594-a3c6c9351c1c"));

            // Assert
            var result = await _orderRepository.FindAsync(c => c.Id == Guid.Parse("dcdc247a-f601-486e-9594-a3c6c9351c1c"));

            result.ShouldBeNull();
        }
    }
}