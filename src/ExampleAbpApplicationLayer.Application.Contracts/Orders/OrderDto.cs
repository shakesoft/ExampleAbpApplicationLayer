using ExampleAbpApplicationLayer.Enums.Orders;
using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace ExampleAbpApplicationLayer.Orders
{
    public abstract class OrderDtoBase : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public DateTime OrderDate { get; set; }
        public float TotalAmount { get; set; }
        public OrderStatus Status { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;

    }
}