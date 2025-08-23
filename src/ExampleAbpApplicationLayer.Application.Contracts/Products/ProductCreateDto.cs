using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace ExampleAbpApplicationLayer.Products
{
    public abstract class ProductCreateDtoBase
    {
        [Required]
        public string Name { get; set; } = null!;
        public string? Desc { get; set; }
        public float Price { get; set; } = 0f;
        public bool IsActive { get; set; } = true;
    }
}