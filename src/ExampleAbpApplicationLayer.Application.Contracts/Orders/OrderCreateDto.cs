using ExampleAbpApplicationLayer.Enums.Orders;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace ExampleAbpApplicationLayer.Orders
{
    public abstract class OrderCreateDtoBase
    {
        public DateTime OrderDate { get; set; }
        public float TotalAmount { get; set; } = 0f;
        public OrderStatus Status { get; set; } = ((OrderStatus[])Enum.GetValues(typeof(OrderStatus)))[0];
    }
}