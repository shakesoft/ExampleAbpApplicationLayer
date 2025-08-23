using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace ExampleAbpApplicationLayer.OrderItems
{
    public partial interface IOrderItemRepository : IRepository<OrderItem, Guid>
    {
        Task<List<OrderItem>> GetListByOrderIdAsync(
    Guid orderId,
    string? sorting = null,
    int maxResultCount = int.MaxValue,
    int skipCount = 0,
    CancellationToken cancellationToken = default
);

        Task<long> GetCountByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);

        Task<List<OrderItemWithNavigationProperties>> GetListWithNavigationPropertiesByOrderIdAsync(
            Guid orderId,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<OrderItemWithNavigationProperties> GetWithNavigationPropertiesAsync(
            Guid id,
            CancellationToken cancellationToken = default
        );

        Task<List<OrderItemWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            int? qtyMin = null,
            int? qtyMax = null,
            float? priceMin = null,
            float? priceMax = null,
            float? totalPriceMin = null,
            float? totalPriceMax = null,
            Guid? productId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<List<OrderItem>> GetListAsync(
                    string? filterText = null,
                    int? qtyMin = null,
                    int? qtyMax = null,
                    float? priceMin = null,
                    float? priceMax = null,
                    float? totalPriceMin = null,
                    float? totalPriceMax = null,
                    string? sorting = null,
                    int maxResultCount = int.MaxValue,
                    int skipCount = 0,
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
            string? filterText = null,
            int? qtyMin = null,
            int? qtyMax = null,
            float? priceMin = null,
            float? priceMax = null,
            float? totalPriceMin = null,
            float? totalPriceMax = null,
            Guid? productId = null,
            CancellationToken cancellationToken = default);
    }
}