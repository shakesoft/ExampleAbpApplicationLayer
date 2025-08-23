using System;

namespace ExampleAbpApplicationLayer.Products
{
    public abstract class ProductExcelDtoBase
    {
        public string Name { get; set; } = null!;
        public float Price { get; set; }
        public bool IsActive { get; set; }
    }
}