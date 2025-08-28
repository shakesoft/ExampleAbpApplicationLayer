using ExampleAbpApplicationLayer.Enums.Orders;
using System;
using System.Collections.Generic;
using ExampleAbpApplicationLayer.OrderItems;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace ExampleAbpApplicationLayer.Orders
{
    public abstract class OrderDtoBase : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public DateTime OrderDate { get; set; }
        public float TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public Guid? IdentityUserId { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;

        public List<OrderItemWithNavigationPropertiesDto> OrderItems { get; set; } = new();
    }
}