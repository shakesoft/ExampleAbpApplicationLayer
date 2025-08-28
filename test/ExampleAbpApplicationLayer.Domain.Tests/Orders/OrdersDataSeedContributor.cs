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
                id: Guid.Parse("5474ba67-048e-4654-a33f-6df2eba8e668"),
                orderDate: new DateTime(2012, 5, 5),
                totalAmount: 489686810,
                status: default,
                identityUserId: null
            ));

            await _orderRepository.InsertAsync(new Order
            (
                id: Guid.Parse("bda19146-3ed5-47d6-9750-3844b52808c3"),
                orderDate: new DateTime(2000, 2, 18),
                totalAmount: 2002917785,
                status: default,
                identityUserId: null
            ));

            await _unitOfWorkManager!.Current!.SaveChangesAsync();

            IsSeeded = true;
        }
    }
}