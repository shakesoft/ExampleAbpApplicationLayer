using ExampleAbpApplicationLayer.Enums.Orders;
using Volo.Abp.Identity;
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

        public virtual async Task<OrderWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            return (await GetDbSetAsync()).Where(b => b.Id == id)
                .Select(order => new OrderWithNavigationProperties
                {
                    Order = order,
                    IdentityUser = dbContext.Set<IdentityUser>().FirstOrDefault(c => c.Id == order.IdentityUserId)
                }).FirstOrDefault();
        }

        public virtual async Task<List<OrderWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            DateTime? orderDateMin = null,
            DateTime? orderDateMax = null,
            float? totalAmountMin = null,
            float? totalAmountMax = null,
            OrderStatus? status = null,
            Guid? identityUserId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, orderDateMin, orderDateMax, totalAmountMin, totalAmountMax, status, identityUserId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? OrderConsts.GetDefaultSorting(true) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        protected virtual async Task<IQueryable<OrderWithNavigationProperties>> GetQueryForNavigationPropertiesAsync()
        {
            return from order in (await GetDbSetAsync())
                   join identityUser in (await GetDbContextAsync()).Set<IdentityUser>() on order.IdentityUserId equals identityUser.Id into identityUsers
                   from identityUser in identityUsers.DefaultIfEmpty()
                   select new OrderWithNavigationProperties
                   {
                       Order = order,
                       IdentityUser = identityUser
                   };
        }

        protected virtual IQueryable<OrderWithNavigationProperties> ApplyFilter(
            IQueryable<OrderWithNavigationProperties> query,
            string? filterText,
            DateTime? orderDateMin = null,
            DateTime? orderDateMax = null,
            float? totalAmountMin = null,
            float? totalAmountMax = null,
            OrderStatus? status = null,
            Guid? identityUserId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => true)
                    .WhereIf(orderDateMin.HasValue, e => e.Order.OrderDate >= orderDateMin!.Value)
                    .WhereIf(orderDateMax.HasValue, e => e.Order.OrderDate <= orderDateMax!.Value)
                    .WhereIf(totalAmountMin.HasValue, e => e.Order.TotalAmount >= totalAmountMin!.Value)
                    .WhereIf(totalAmountMax.HasValue, e => e.Order.TotalAmount <= totalAmountMax!.Value)
                    .WhereIf(status.HasValue, e => e.Order.Status == status)
                    .WhereIf(identityUserId != null && identityUserId != Guid.Empty, e => e.IdentityUser != null && e.IdentityUser.Id == identityUserId);
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
            Guid? identityUserId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, orderDateMin, orderDateMax, totalAmountMin, totalAmountMax, status, identityUserId);
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