using ExampleAbpApplicationLayer.Products;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace ExampleAbpApplicationLayer.OrderItems
{
    public abstract class OrderItemBase : FullAuditedEntity<Guid>, IMultiTenant
    {
        public virtual Guid OrderId { get; set; }

        public virtual Guid? TenantId { get; set; }

        public virtual int Qty { get; set; }

        public virtual float Price { get; set; }

        public virtual float TotalPrice { get; set; }

        [CanBeNull]
        public virtual string? ProductName { get; set; }
        public Guid ProductId { get; set; }

        protected OrderItemBase()
        {

        }

        public OrderItemBase(Guid id, Guid orderId, Guid productId, int qty, float price, float totalPrice, string? productName = null)
        {

            Id = id;
            OrderId = orderId;
            Qty = qty;
            Price = price;
            TotalPrice = totalPrice;
            ProductName = productName;
            ProductId = productId;
        }

    }
}