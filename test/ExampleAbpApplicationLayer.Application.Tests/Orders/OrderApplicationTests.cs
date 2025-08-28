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
            result.Items.Any(x => x.Order.Id == Guid.Parse("5474ba67-048e-4654-a33f-6df2eba8e668")).ShouldBe(true);
            result.Items.Any(x => x.Order.Id == Guid.Parse("bda19146-3ed5-47d6-9750-3844b52808c3")).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Act
            var result = await _ordersAppService.GetAsync(Guid.Parse("5474ba67-048e-4654-a33f-6df2eba8e668"));

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(Guid.Parse("5474ba67-048e-4654-a33f-6df2eba8e668"));
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new OrderCreateDto
            {
                OrderDate = new DateTime(2021, 5, 8),
                TotalAmount = 1975980167,
                Status = default
            };

            // Act
            var serviceResult = await _ordersAppService.CreateAsync(input);

            // Assert
            var result = await _orderRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.OrderDate.ShouldBe(new DateTime(2021, 5, 8));
            result.TotalAmount.ShouldBe(1975980167);
            result.Status.ShouldBe(default);
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var input = new OrderUpdateDto()
            {
                OrderDate = new DateTime(2018, 5, 8),
                TotalAmount = 1385158195,
                Status = default
            };

            // Act
            var serviceResult = await _ordersAppService.UpdateAsync(Guid.Parse("5474ba67-048e-4654-a33f-6df2eba8e668"), input);

            // Assert
            var result = await _orderRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.OrderDate.ShouldBe(new DateTime(2018, 5, 8));
            result.TotalAmount.ShouldBe(1385158195);
            result.Status.ShouldBe(default);
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Act
            await _ordersAppService.DeleteAsync(Guid.Parse("5474ba67-048e-4654-a33f-6df2eba8e668"));

            // Assert
            var result = await _orderRepository.FindAsync(c => c.Id == Guid.Parse("5474ba67-048e-4654-a33f-6df2eba8e668"));

            result.ShouldBeNull();
        }
    }
}