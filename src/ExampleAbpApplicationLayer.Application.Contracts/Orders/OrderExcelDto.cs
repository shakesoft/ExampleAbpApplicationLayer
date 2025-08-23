using ExampleAbpApplicationLayer.Enums.Orders;
using System;

namespace ExampleAbpApplicationLayer.Orders
{
    public abstract class OrderExcelDtoBase
    {
        public DateTime OrderDate { get; set; }
        public float TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
    }
}