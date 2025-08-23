using ExampleAbpApplicationLayer.Enums.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using ExampleAbpApplicationLayer.EntityFrameworkCore;

namespace ExampleAbpApplicationLayer.Orders
{
    public abstract class EfCoreOrderRepositoryBase : EfCoreRepository<ExampleAbpApplicationLayerDbContext, Order, Guid>
    {
        public EfCoreOrderRepositoryBase(IDbContextProvider<ExampleAbpApplicationLayerDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public virtual async Task<List<Order>> GetListAsync(
            string? filterText = null,
            DateTime? orderDateMin = null,
            DateTime? orderDateMax = null,
            float? totalAmountMin = null,
            float? totalAmountMax = null,
            OrderStatus? status = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, orderDateMin, orderDateMax, totalAmountMin, totalAmountMax, status);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? OrderConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            DateTime? orderDateMin = null,
            DateTime? orderDateMax = null,
            float? totalAmountMin = null,
            float? totalAmountMax = null,
            OrderStatus? status = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetDbSetAsync()), filterText, orderDateMin, orderDateMax, totalAmountMin, totalAmountMax, status);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<Order> ApplyFilter(
            IQueryable<Order> query,
            string? filterText = null,
            DateTime? orderDateMin = null,
            DateTime? orderDateMax = null,
            float? totalAmountMin = null,
            float? totalAmountMax = null,
            OrderStatus? status = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => true)
                    .WhereIf(orderDateMin.HasValue, e => e.OrderDate >= orderDateMin!.Value)
                    .WhereIf(orderDateMax.HasValue, e => e.OrderDate <= orderDateMax!.Value)
                    .WhereIf(totalAmountMin.HasValue, e => e.TotalAmount >= totalAmountMin!.Value)
                    .WhereIf(totalAmountMax.HasValue, e => e.TotalAmount <= totalAmountMax!.Value)
                    .WhereIf(status.HasValue, e => e.Status == status);
        }
    }
}