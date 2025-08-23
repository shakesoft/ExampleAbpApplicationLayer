using Volo.Abp.Application.Dtos;
using System;

namespace ExampleAbpApplicationLayer.OrderItems
{
    public abstract class GetOrderItemsInputBase : PagedAndSortedResultRequestDto
    {
        public string? FilterText { get; set; }

        public int? QtyMin { get; set; }
        public int? QtyMax { get; set; }
        public float? PriceMin { get; set; }
        public float? PriceMax { get; set; }
        public float? TotalPriceMin { get; set; }
        public float? TotalPriceMax { get; set; }
        public Guid? ProductId { get; set; }

        public GetOrderItemsInputBase()
        {

        }
    }
}