using ExampleAbpApplicationLayer.Shared;
using ExampleAbpApplicationLayer.Products;
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
using ExampleAbpApplicationLayer.OrderItems;

namespace ExampleAbpApplicationLayer.OrderItems
{
    [RemoteService(IsEnabled = false)]
    [Authorize(ExampleAbpApplicationLayerPermissions.OrderItems.Default)]
    public abstract class OrderItemsAppServiceBase : ExampleAbpApplicationLayerAppService
    {

        protected IOrderItemRepository _orderItemRepository;
        protected OrderItemManager _orderItemManager;

        protected IRepository<ExampleAbpApplicationLayer.Products.Product, Guid> _productRepository;

        public OrderItemsAppServiceBase(IOrderItemRepository orderItemRepository, OrderItemManager orderItemManager, IRepository<ExampleAbpApplicationLayer.Products.Product, Guid> productRepository)
        {

            _orderItemRepository = orderItemRepository;
            _orderItemManager = orderItemManager; _productRepository = productRepository;

        }

        public virtual async Task<PagedResultDto<OrderItemDto>> GetListByOrderIdAsync(GetOrderItemListInput input)
        {
            var orderItems = await _orderItemRepository.GetListByOrderIdAsync(
                input.OrderId,
                input.Sorting,
                input.MaxResultCount,
                input.SkipCount);

            return new PagedResultDto<OrderItemDto>
            {
                TotalCount = await _orderItemRepository.GetCountByOrderIdAsync(input.OrderId),
                Items = ObjectMapper.Map<List<OrderItem>, List<OrderItemDto>>(orderItems)
            };
        }
        public virtual async Task<PagedResultDto<OrderItemWithNavigationPropertiesDto>> GetListWithNavigationPropertiesByOrderIdAsync(GetOrderItemListInput input)
        {
            var orderItems = await _orderItemRepository.GetListWithNavigationPropertiesByOrderIdAsync(
                input.OrderId,
                input.Sorting,
                input.MaxResultCount,
                input.SkipCount);

            return new PagedResultDto<OrderItemWithNavigationPropertiesDto>
            {
                TotalCount = await _orderItemRepository.GetCountByOrderIdAsync(input.OrderId),
                Items = ObjectMapper.Map<List<OrderItemWithNavigationProperties>, List<OrderItemWithNavigationPropertiesDto>>(orderItems)
            };
        }

        public virtual async Task<PagedResultDto<OrderItemWithNavigationPropertiesDto>> GetListAsync(GetOrderItemsInput input)
        {
            var totalCount = await _orderItemRepository.GetCountAsync(input.FilterText, input.QtyMin, input.QtyMax, input.PriceMin, input.PriceMax, input.TotalPriceMin, input.TotalPriceMax, input.ProductId);
            var items = await _orderItemRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.QtyMin, input.QtyMax, input.PriceMin, input.PriceMax, input.TotalPriceMin, input.TotalPriceMax, input.ProductId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<OrderItemWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<OrderItemWithNavigationProperties>, List<OrderItemWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<OrderItemWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<OrderItemWithNavigationProperties, OrderItemWithNavigationPropertiesDto>
                (await _orderItemRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<OrderItemDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<OrderItem, OrderItemDto>(await _orderItemRepository.GetAsync(id));
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetProductLookupAsync(LookupRequestDto input)
        {
            var query = (await _productRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.Name != null &&
                         x.Name.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<ExampleAbpApplicationLayer.Products.Product>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<ExampleAbpApplicationLayer.Products.Product>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        [Authorize(ExampleAbpApplicationLayerPermissions.OrderItems.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _orderItemRepository.DeleteAsync(id);
        }

        [Authorize(ExampleAbpApplicationLayerPermissions.OrderItems.Create)]
        public virtual async Task<OrderItemDto> CreateAsync(OrderItemCreateDto input)
        {
            if (input.ProductId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["Product"]]);
            }

            var orderItem = await _orderItemManager.CreateAsync(input.OrderId
            , input.ProductId, input.Qty, input.Price, input.TotalPrice, input.ProductName
            );

            return ObjectMapper.Map<OrderItem, OrderItemDto>(orderItem);
        }

        [Authorize(ExampleAbpApplicationLayerPermissions.OrderItems.Edit)]
        public virtual async Task<OrderItemDto> UpdateAsync(Guid id, OrderItemUpdateDto input)
        {
            if (input.ProductId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["Product"]]);
            }

            var orderItem = await _orderItemManager.UpdateAsync(
            id, input.OrderId
            , input.ProductId, input.Qty, input.Price, input.TotalPrice, input.ProductName
            );

            return ObjectMapper.Map<OrderItem, OrderItemDto>(orderItem);
        }
    }
}