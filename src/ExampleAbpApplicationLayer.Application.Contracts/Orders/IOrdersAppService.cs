using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using ExampleAbpApplicationLayer.Shared;

namespace ExampleAbpApplicationLayer.Orders
{
    public partial interface IOrdersAppService : IApplicationService
    {

        Task<PagedResultDto<OrderDto>> GetListAsync(GetOrdersInput input);

        Task<OrderDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<OrderDto> CreateAsync(OrderCreateDto input);

        Task<OrderDto> UpdateAsync(Guid id, OrderUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(OrderExcelDownloadDto input);

        Task<ExampleAbpApplicationLayer.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();

    }
}