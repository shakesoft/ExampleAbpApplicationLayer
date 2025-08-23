using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using ExampleAbpApplicationLayer.Permissions;
using ExampleAbpApplicationLayer.Orders;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using ExampleAbpApplicationLayer.Shared;

namespace ExampleAbpApplicationLayer.Orders
{
    [RemoteService(IsEnabled = false)]
    [Authorize(ExampleAbpApplicationLayerPermissions.Orders.Default)]
    public abstract class OrdersAppServiceBase : ExampleAbpApplicationLayerAppService
    {
        protected IDistributedCache<OrderDownloadTokenCacheItem, string> _downloadTokenCache;
        protected IOrderRepository _orderRepository;
        protected OrderManager _orderManager;

        public OrdersAppServiceBase(IOrderRepository orderRepository, OrderManager orderManager, IDistributedCache<OrderDownloadTokenCacheItem, string> downloadTokenCache)
        {
            _downloadTokenCache = downloadTokenCache;
            _orderRepository = orderRepository;
            _orderManager = orderManager;

        }

        public virtual async Task<PagedResultDto<OrderDto>> GetListAsync(GetOrdersInput input)
        {
            var totalCount = await _orderRepository.GetCountAsync(input.FilterText, input.OrderDateMin, input.OrderDateMax, input.TotalAmountMin, input.TotalAmountMax, input.Status);
            var items = await _orderRepository.GetListAsync(input.FilterText, input.OrderDateMin, input.OrderDateMax, input.TotalAmountMin, input.TotalAmountMax, input.Status, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<OrderDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Order>, List<OrderDto>>(items)
            };
        }

        public virtual async Task<OrderDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Order, OrderDto>(await _orderRepository.GetAsync(id));
        }

        [Authorize(ExampleAbpApplicationLayerPermissions.Orders.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _orderRepository.DeleteAsync(id);
        }

        [Authorize(ExampleAbpApplicationLayerPermissions.Orders.Create)]
        public virtual async Task<OrderDto> CreateAsync(OrderCreateDto input)
        {

            var order = await _orderManager.CreateAsync(
            input.OrderDate, input.TotalAmount, input.Status
            );

            return ObjectMapper.Map<Order, OrderDto>(order);
        }

        [Authorize(ExampleAbpApplicationLayerPermissions.Orders.Edit)]
        public virtual async Task<OrderDto> UpdateAsync(Guid id, OrderUpdateDto input)
        {

            var order = await _orderManager.UpdateAsync(
            id,
            input.OrderDate, input.TotalAmount, input.Status, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<Order, OrderDto>(order);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(OrderExcelDownloadDto input)
        {
            var downloadToken = await _downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var items = await _orderRepository.GetListAsync(input.FilterText, input.OrderDateMin, input.OrderDateMax, input.TotalAmountMin, input.TotalAmountMax, input.Status);

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(ObjectMapper.Map<List<Order>, List<OrderExcelDto>>(items));
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "Orders.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public virtual async Task<ExampleAbpApplicationLayer.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _downloadTokenCache.SetAsync(
                token,
                new OrderDownloadTokenCacheItem { Token = token },
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                });

            return new ExampleAbpApplicationLayer.Shared.DownloadTokenResultDto
            {
                Token = token
            };
        }
    }
}