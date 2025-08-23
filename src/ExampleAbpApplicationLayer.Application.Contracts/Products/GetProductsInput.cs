using Volo.Abp.Application.Dtos;
using System;

namespace ExampleAbpApplicationLayer.Products
{
    public abstract class GetProductsInputBase : PagedAndSortedResultRequestDto
    {
        public string? FilterText { get; set; }

        public string? Name { get; set; }
        public float? PriceMin { get; set; }
        public float? PriceMax { get; set; }
        public bool? IsActive { get; set; }

        public GetProductsInputBase()
        {

        }
    }
}