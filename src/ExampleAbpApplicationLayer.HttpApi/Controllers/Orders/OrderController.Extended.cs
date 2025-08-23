using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using ExampleAbpApplicationLayer.Orders;

namespace ExampleAbpApplicationLayer.Controllers.Orders
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Order")]
    [Route("api/app/orders")]

    public class OrderController : OrderControllerBase, IOrdersAppService
    {
        public OrderController(IOrdersAppService ordersAppService) : base(ordersAppService)
        {
        }
    }
}