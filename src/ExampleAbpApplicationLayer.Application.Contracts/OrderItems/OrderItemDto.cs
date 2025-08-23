using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;

namespace ExampleAbpApplicationLayer.OrderItems
{
    public abstract class OrderItemDtoBase : FullAuditedEntityDto<Guid>
    {
        public Guid OrderId { get; set; }
        public int Qty { get; set; }
        public float Price { get; set; }
        public float TotalPrice { get; set; }
        public string? ProductName { get; set; }
        public Guid ProductId { get; set; }

    }
}