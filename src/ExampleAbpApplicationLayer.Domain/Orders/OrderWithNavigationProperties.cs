using Volo.Abp.Identity;

using System;
using System.Collections.Generic;

namespace ExampleAbpApplicationLayer.Orders
{
    public abstract class OrderWithNavigationPropertiesBase
    {
        public Order Order { get; set; } = null!;

        public IdentityUser IdentityUser { get; set; } = null!;
        

        
    }
}