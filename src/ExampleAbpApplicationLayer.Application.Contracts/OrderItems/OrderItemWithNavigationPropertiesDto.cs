using ExampleAbpApplicationLayer.Products;

using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace ExampleAbpApplicationLayer.OrderItems
{
    public abstract class OrderItemWithNavigationPropertiesDtoBase
    {
        public OrderItemDto OrderItem { get; set; } = null!;

        public ProductDto Product { get; set; } = null!;

    }
}