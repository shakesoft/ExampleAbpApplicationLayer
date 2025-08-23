using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace ExampleAbpApplicationLayer.OrderItems
{
    public abstract class OrderItemCreateDtoBase
    {
        public Guid OrderId { get; set; }
        public int Qty { get; set; } = 1;
        public float Price { get; set; } = 0f;
        public float TotalPrice { get; set; }
        public string? ProductName { get; set; }
        public Guid ProductId { get; set; }
    }
}