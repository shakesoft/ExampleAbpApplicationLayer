using Volo.Abp.Identity;

using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace ExampleAbpApplicationLayer.Orders
{
    public abstract class OrderWithNavigationPropertiesDtoBase
    {
        public OrderDto Order { get; set; } = null!;

        public IdentityUserDto IdentityUser { get; set; } = null!;

    }
}