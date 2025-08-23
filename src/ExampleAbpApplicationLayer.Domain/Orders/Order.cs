using ExampleAbpApplicationLayer.Enums.Orders;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace ExampleAbpApplicationLayer.Orders
{
    public abstract class OrderBase : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        public virtual DateTime OrderDate { get; set; }

        public virtual float TotalAmount { get; set; }

        public virtual OrderStatus Status { get; set; }

        protected OrderBase()
        {

        }

        public OrderBase(Guid id, DateTime orderDate, float totalAmount, OrderStatus status)
        {

            Id = id;
            OrderDate = orderDate;
            TotalAmount = totalAmount;
            Status = status;
        }

    }
}