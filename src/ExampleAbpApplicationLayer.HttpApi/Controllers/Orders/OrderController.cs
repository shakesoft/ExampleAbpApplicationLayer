using Asp.Versioning;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using ExampleAbpApplicationLayer.Orders;
using Volo.Abp.Content;
using ExampleAbpApplicationLayer.Shared;

namespace ExampleAbpApplicationLayer.Controllers.Orders
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Order")]
    [Route("api/app/orders")]

    public abstract class OrderControllerBase : AbpController
    {
        protected IOrdersAppService _ordersAppService;

        public OrderControllerBase(IOrdersAppService ordersAppService)
        {
            _ordersAppService = ordersAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<OrderDto>> GetListAsync(GetOrdersInput input)
        {
            return _ordersAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<OrderDto> GetAsync(Guid id)
        {
            return _ordersAppService.GetAsync(id);
        }

        [HttpPost]
        public virtual Task<OrderDto> CreateAsync(OrderCreateDto input)
        {
            return _ordersAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<OrderDto> UpdateAsync(Guid id, OrderUpdateDto input)
        {
            return _ordersAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _ordersAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("as-excel-file")]
        public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(OrderExcelDownloadDto input)
        {
            return _ordersAppService.GetListAsExcelFileAsync(input);
        }

        [HttpGet]
        [Route("download-token")]
        public virtual Task<ExampleAbpApplicationLayer.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            return _ordersAppService.GetDownloadTokenAsync();
        }

    }
}