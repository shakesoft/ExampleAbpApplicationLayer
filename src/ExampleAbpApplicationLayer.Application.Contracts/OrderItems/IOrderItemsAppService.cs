using ExampleAbpApplicationLayer.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace ExampleAbpApplicationLayer.OrderItems
{
    public partial interface IOrderItemsAppService : IApplicationService
    {

        Task<PagedResultDto<OrderItemDto>> GetListByOrderIdAsync(GetOrderItemListInput input);
        Task<PagedResultDto<OrderItemWithNavigationPropertiesDto>> GetListWithNavigationPropertiesByOrderIdAsync(GetOrderItemListInput input);

        Task<PagedResultDto<OrderItemWithNavigationPropertiesDto>> GetListAsync(GetOrderItemsInput input);

        Task<OrderItemWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<OrderItemDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid>>> GetProductLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<OrderItemDto> CreateAsync(OrderItemCreateDto input);

        Task<OrderItemDto> UpdateAsync(Guid id, OrderItemUpdateDto input);
    }
}