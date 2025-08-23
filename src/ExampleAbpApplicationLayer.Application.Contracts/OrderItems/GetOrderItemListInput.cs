using Volo.Abp.Application.Dtos;
using System;

namespace ExampleAbpApplicationLayer.OrderItems
{
    public class GetOrderItemListInput : PagedAndSortedResultRequestDto
    {
        public Guid OrderId { get; set; }
    }
}