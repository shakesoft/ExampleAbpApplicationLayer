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
                id: Guid.Parse("0d508788-38cc-4743-804d-65adda7f6a6d"),
                orderDate: new DateTime(2014, 8, 10),
                totalAmount: 442297264,
                status: default,
                identityUserId: null
            ));

            await _orderRepository.InsertAsync(new Order
            (
                id: Guid.Parse("d5545aa7-9121-4baa-9bc5-d232e20f6482"),
                orderDate: new DateTime(2017, 4, 11),
                totalAmount: 1109970867,
                status: default,
                identityUserId: null
            ));

            await _unitOfWorkManager!.Current!.SaveChangesAsync();

            IsSeeded = true;
        }
    }
}