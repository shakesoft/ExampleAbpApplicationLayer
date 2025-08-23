using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace ExampleAbpApplicationLayer.OrderItems
{
    public abstract class OrderItemUpdateDtoBase
    {
        public Guid OrderId { get; set; }
        public int Qty { get; set; }
        public float Price { get; set; }
        public float TotalPrice { get; set; }
        public string? ProductName { get; set; }
        public Guid ProductId { get; set; }

    }
}