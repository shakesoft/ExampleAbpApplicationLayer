using System;
using System.Collections.Generic;
using System.Text;

namespace ExampleAbpApplicationLayer.Enums.Orders
{
    public enum OrderStatus
    {
        Initialized = 0,
        Paid = 1,
        Processing = 2,
        Ordered = 3,
        Shipped = 4,
        Arrived = 5,
        Delivered = 6,
        Cancelled = 7,
        NotPaid = 8
    }
}
