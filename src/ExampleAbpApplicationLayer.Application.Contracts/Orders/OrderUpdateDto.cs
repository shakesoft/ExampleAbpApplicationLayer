using ExampleAbpApplicationLayer.Enums.Orders;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace ExampleAbpApplicationLayer.Orders
{
    public abstract class OrderUpdateDtoBase : IHasConcurrencyStamp
    {
        public DateTime OrderDate { get; set; }
        public float TotalAmount { get; set; }
        public OrderStatus Status { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;
    }
}