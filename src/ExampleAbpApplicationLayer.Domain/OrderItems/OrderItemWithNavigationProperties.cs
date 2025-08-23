using ExampleAbpApplicationLayer.Products;

using System;
using System.Collections.Generic;

namespace ExampleAbpApplicationLayer.OrderItems
{
    public abstract class OrderItemWithNavigationPropertiesBase
    {
        public OrderItem OrderItem { get; set; } = null!;

        public Product Product { get; set; } = null!;
        

        
    }
}