using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace ExampleAbpApplicationLayer.OrderItems
{
    public abstract class OrderItemManagerBase : DomainService
    {
        protected IOrderItemRepository _orderItemRepository;

        public OrderItemManagerBase(IOrderItemRepository orderItemRepository)
        {
            _orderItemRepository = orderItemRepository;
        }

        public virtual async Task<OrderItem> CreateAsync(
        Guid orderId, Guid productId, int qty, float price, float totalPrice, string? productName = null)
        {
            Check.NotNull(productId, nameof(productId));

            var orderItem = new OrderItem(
             GuidGenerator.Create(),
             orderId, productId, qty, price, totalPrice, productName
             );

            return await _orderItemRepository.InsertAsync(orderItem);
        }

        public virtual async Task<OrderItem> UpdateAsync(
            Guid id,
            Guid orderId, Guid productId, int qty, float price, float totalPrice, string? productName = null
        )
        {
            Check.NotNull(productId, nameof(productId));

            var orderItem = await _orderItemRepository.GetAsync(id);

            orderItem.OrderId = orderId;
            orderItem.ProductId = productId;
            orderItem.Qty = qty;
            orderItem.Price = price;
            orderItem.TotalPrice = totalPrice;
            orderItem.ProductName = productName;

            return await _orderItemRepository.UpdateAsync(orderItem);
        }

    }
}