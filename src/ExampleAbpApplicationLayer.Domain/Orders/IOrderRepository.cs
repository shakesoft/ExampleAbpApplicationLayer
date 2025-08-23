using ExampleAbpApplicationLayer.Enums.Orders;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace ExampleAbpApplicationLayer.Orders
{
    public partial interface IOrderRepository : IRepository<Order, Guid>
    {
        Task<List<Order>> GetListAsync(
            string? filterText = null,
            DateTime? orderDateMin = null,
            DateTime? orderDateMax = null,
            float? totalAmountMin = null,
            float? totalAmountMax = null,
            OrderStatus? status = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<long> GetCountAsync(
            string? filterText = null,
            DateTime? orderDateMin = null,
            DateTime? orderDateMax = null,
            float? totalAmountMin = null,
            float? totalAmountMax = null,
            OrderStatus? status = null,
            CancellationToken cancellationToken = default);
    }
}