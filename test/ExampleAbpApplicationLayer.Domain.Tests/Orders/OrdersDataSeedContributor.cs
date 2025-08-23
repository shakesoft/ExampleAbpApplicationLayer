using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using ExampleAbpApplicationLayer.Orders;

namespace ExampleAbpApplicationLayer.Orders
{
    public class OrdersDataSeedContributor : IDataSeedContributor, ISingletonDependency
    {
        private bool IsSeeded = false;
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public OrdersDataSeedContributor(IOrderRepository orderRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _orderRepository = orderRepository;
            _unitOfWorkManager = unitOfWorkManager;

        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (IsSeeded)
            {
                return;
            }

            await _orderRepository.InsertAsync(new Order
            (
                id: Guid.Parse("dcdc247a-f601-486e-9594-a3c6c9351c1c"),
                orderDate: new DateTime(2020, 10, 26),
                totalAmount: 504230225,
                status: default
            ));

            await _orderRepository.InsertAsync(new Order
            (
                id: Guid.Parse("53a5e56a-5b6f-4f0f-a42e-3562b6baf84c"),
                orderDate: new DateTime(2024, 4, 20),
                totalAmount: 326594224,
                status: default
            ));

            await _unitOfWorkManager!.Current!.SaveChangesAsync();

            IsSeeded = true;
        }
    }
}