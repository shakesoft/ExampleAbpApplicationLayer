using ExampleAbpApplicationLayer.Shared;
using Volo.Abp.Identity;
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

        protected IRepository<Volo.Abp.Identity.IdentityUser, Guid> _identityUserRepository;

        public OrdersAppServiceBase(IOrderRepository orderRepository, OrderManager orderManager, IDistributedCache<OrderDownloadTokenCacheItem, string> downloadTokenCache, IRepository<Volo.Abp.Identity.IdentityUser, Guid> identityUserRepository)
        {
            _downloadTokenCache = downloadTokenCache;
            _orderRepository = orderRepository;
            _orderManager = orderManager; _identityUserRepository = identityUserRepository;

        }

        public virtual async Task<PagedResultDto<OrderWithNavigationPropertiesDto>> GetListAsync(GetOrdersInput input)
        {
            var totalCount = await _orderRepository.GetCountAsync(input.FilterText, input.OrderDateMin, input.OrderDateMax, input.TotalAmountMin, input.TotalAmountMax, input.Status, input.IdentityUserId);
            var items = await _orderRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.OrderDateMin, input.OrderDateMax, input.TotalAmountMin, input.TotalAmountMax, input.Status, input.IdentityUserId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<OrderWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<OrderWithNavigationProperties>, List<OrderWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<OrderWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<OrderWithNavigationProperties, OrderWithNavigationPropertiesDto>
                (await _orderRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<OrderDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Order, OrderDto>(await _orderRepository.GetAsync(id));
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetIdentityUserLookupAsync(LookupRequestDto input)
        {
            var query = (await _identityUserRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.UserName != null &&
                         x.UserName.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Volo.Abp.Identity.IdentityUser>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Volo.Abp.Identity.IdentityUser>, List<LookupDto<Guid>>>(lookupData)
            };
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
            input.IdentityUserId, input.OrderDate, input.TotalAmount, input.Status
            );

            return ObjectMapper.Map<Order, OrderDto>(order);
        }

        [Authorize(ExampleAbpApplicationLayerPermissions.Orders.Edit)]
        public virtual async Task<OrderDto> UpdateAsync(Guid id, OrderUpdateDto input)
        {

            var order = await _orderManager.UpdateAsync(
            id,
            input.IdentityUserId, input.OrderDate, input.TotalAmount, input.Status, input.ConcurrencyStamp
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

            var orders = await _orderRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.OrderDateMin, input.OrderDateMax, input.TotalAmountMin, input.TotalAmountMax, input.Status, input.IdentityUserId);
            var items = orders.Select(item => new
            {
                OrderDate = item.Order.OrderDate,
                TotalAmount = item.Order.TotalAmount,
                Status = item.Order.Status,

                IdentityUser = item.IdentityUser?.UserName,

            });

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(items);
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