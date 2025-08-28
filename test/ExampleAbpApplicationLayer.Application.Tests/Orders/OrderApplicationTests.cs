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
            result.Items.Any(x => x.Order.Id == Guid.Parse("0d508788-38cc-4743-804d-65adda7f6a6d")).ShouldBe(true);
            result.Items.Any(x => x.Order.Id == Guid.Parse("d5545aa7-9121-4baa-9bc5-d232e20f6482")).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Act
            var result = await _ordersAppService.GetAsync(Guid.Parse("0d508788-38cc-4743-804d-65adda7f6a6d"));

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(Guid.Parse("0d508788-38cc-4743-804d-65adda7f6a6d"));
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new OrderCreateDto
            {
                OrderDate = new DateTime(2018, 5, 16),
                TotalAmount = 1586680255,
                Status = default
            };

            // Act
            var serviceResult = await _ordersAppService.CreateAsync(input);

            // Assert
            var result = await _orderRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.OrderDate.ShouldBe(new DateTime(2018, 5, 16));
            result.TotalAmount.ShouldBe(1586680255);
            result.Status.ShouldBe(default);
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var input = new OrderUpdateDto()
            {
                OrderDate = new DateTime(2019, 5, 9),
                TotalAmount = 698533817,
                Status = default
            };

            // Act
            var serviceResult = await _ordersAppService.UpdateAsync(Guid.Parse("0d508788-38cc-4743-804d-65adda7f6a6d"), input);

            // Assert
            var result = await _orderRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.OrderDate.ShouldBe(new DateTime(2019, 5, 9));
            result.TotalAmount.ShouldBe(698533817);
            result.Status.ShouldBe(default);
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Act
            await _ordersAppService.DeleteAsync(Guid.Parse("0d508788-38cc-4743-804d-65adda7f6a6d"));

            // Assert
            var result = await _orderRepository.FindAsync(c => c.Id == Guid.Parse("0d508788-38cc-4743-804d-65adda7f6a6d"));

            result.ShouldBeNull();
        }
    }
}