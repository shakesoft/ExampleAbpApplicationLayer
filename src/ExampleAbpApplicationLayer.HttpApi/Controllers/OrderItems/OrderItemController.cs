using ExampleAbpApplicationLayer.Shared;
using Asp.Versioning;
using System;
using System.Collections.Generic;
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

    public abstract class OrderItemControllerBase : AbpController
    {
        protected IOrderItemsAppService _orderItemsAppService;

        public OrderItemControllerBase(IOrderItemsAppService orderItemsAppService)
        {
            _orderItemsAppService = orderItemsAppService;
        }

        [HttpGet]
        [Route("by-order")]
        public virtual Task<PagedResultDto<OrderItemDto>> GetListByOrderIdAsync(GetOrderItemListInput input)
        {
            return _orderItemsAppService.GetListByOrderIdAsync(input);
        }
        [HttpGet]
        [Route("detailed/by-order")]
        public virtual Task<PagedResultDto<OrderItemWithNavigationPropertiesDto>> GetListWithNavigationPropertiesByOrderIdAsync(GetOrderItemListInput input)
        {
            return _orderItemsAppService.GetListWithNavigationPropertiesByOrderIdAsync(input);
        }

        [HttpGet]
        public virtual Task<PagedResultDto<OrderItemWithNavigationPropertiesDto>> GetListAsync(GetOrderItemsInput input)
        {
            return _orderItemsAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("with-navigation-properties/{id}")]
        public virtual Task<OrderItemWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return _orderItemsAppService.GetWithNavigationPropertiesAsync(id);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<OrderItemDto> GetAsync(Guid id)
        {
            return _orderItemsAppService.GetAsync(id);
        }

        [HttpGet]
        [Route("product-lookup")]
        public virtual Task<PagedResultDto<LookupDto<Guid>>> GetProductLookupAsync(LookupRequestDto input)
        {
            return _orderItemsAppService.GetProductLookupAsync(input);
        }

        [HttpPost]
        public virtual Task<OrderItemDto> CreateAsync(OrderItemCreateDto input)
        {
            return _orderItemsAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<OrderItemDto> UpdateAsync(Guid id, OrderItemUpdateDto input)
        {
            return _orderItemsAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _orderItemsAppService.DeleteAsync(id);
        }
    }
}