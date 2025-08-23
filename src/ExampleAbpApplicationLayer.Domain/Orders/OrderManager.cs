using ExampleAbpApplicationLayer.Enums.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace ExampleAbpApplicationLayer.Orders
{
    public abstract class OrderManagerBase : DomainService
    {
        protected IOrderRepository _orderRepository;

        public OrderManagerBase(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public virtual async Task<Order> CreateAsync(
        DateTime orderDate, float totalAmount, OrderStatus status)
        {
            Check.NotNull(orderDate, nameof(orderDate));
            Check.NotNull(status, nameof(status));

            var order = new Order(
             GuidGenerator.Create(),
             orderDate, totalAmount, status
             );

            return await _orderRepository.InsertAsync(order);
        }

        public virtual async Task<Order> UpdateAsync(
            Guid id,
            DateTime orderDate, float totalAmount, OrderStatus status, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNull(orderDate, nameof(orderDate));
            Check.NotNull(status, nameof(status));

            var order = await _orderRepository.GetAsync(id);

            order.OrderDate = orderDate;
            order.TotalAmount = totalAmount;
            order.Status = status;

            order.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _orderRepository.UpdateAsync(order);
        }

    }
}