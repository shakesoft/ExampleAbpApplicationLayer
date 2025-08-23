using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using ExampleAbpApplicationLayer.OrderItems;

namespace ExampleAbpApplicationLayer.Controllers.OrderItems
{
    [RemoteService]
    [Area("app")]
    [ControllerName("OrderItem")]
    [Route("api/app/order-items")]

    public class OrderItemController : OrderItemControllerBase, IOrderItemsAppService
    {
        public OrderItemController(IOrderItemsAppService orderItemsAppService) : base(orderItemsAppService)
        {
        }
    }
}