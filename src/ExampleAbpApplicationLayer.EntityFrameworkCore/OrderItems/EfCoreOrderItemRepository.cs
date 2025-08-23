using ExampleAbpApplicationLayer.Products;
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

namespace ExampleAbpApplicationLayer.OrderItems
{
    public abstract class EfCoreOrderItemRepositoryBase : EfCoreRepository<ExampleAbpApplicationLayerDbContext, OrderItem, Guid>
    {
        public EfCoreOrderItemRepositoryBase(IDbContextProvider<ExampleAbpApplicationLayerDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public virtual async Task<List<OrderItem>> GetListByOrderIdAsync(
           Guid orderId,
           string? sorting = null,
           int maxResultCount = int.MaxValue,
           int skipCount = 0,
           CancellationToken cancellationToken = default)
        {
            var query = (await GetQueryableAsync()).Where(x => x.OrderId == orderId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? OrderItemConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<long> GetCountByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            return await (await GetQueryableAsync()).Where(x => x.OrderId == orderId).CountAsync(cancellationToken);
        }

        public virtual async Task<List<OrderItemWithNavigationProperties>> GetListWithNavigationPropertiesByOrderIdAsync(
    Guid orderId,
    string? sorting = null,
    int maxResultCount = int.MaxValue,
    int skipCount = 0,
    CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = query.Where(x => x.OrderItem.OrderId == orderId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? OrderItemConsts.GetDefaultSorting(true) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<OrderItemWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            return (await GetDbSetAsync()).Where(b => b.Id == id)
                .Select(orderItem => new OrderItemWithNavigationProperties
                {
                    OrderItem = orderItem,
                    Product = dbContext.Set<Product>().FirstOrDefault(c => c.Id == orderItem.ProductId)
                }).FirstOrDefault();
        }

        public virtual async Task<List<OrderItemWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
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
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, qtyMin, qtyMax, priceMin, priceMax, totalPriceMin, totalPriceMax, productId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? OrderItemConsts.GetDefaultSorting(true) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        protected virtual async Task<IQueryable<OrderItemWithNavigationProperties>> GetQueryForNavigationPropertiesAsync()
        {
            return from orderItem in (await GetDbSetAsync())
                   join product in (await GetDbContextAsync()).Set<Product>() on orderItem.ProductId equals product.Id into products
                   from product in products.DefaultIfEmpty()
                   select new OrderItemWithNavigationProperties
                   {
                       OrderItem = orderItem,
                       Product = product
                   };
        }

        protected virtual IQueryable<OrderItemWithNavigationProperties> ApplyFilter(
            IQueryable<OrderItemWithNavigationProperties> query,
            string? filterText,
            int? qtyMin = null,
            int? qtyMax = null,
            float? priceMin = null,
            float? priceMax = null,
            float? totalPriceMin = null,
            float? totalPriceMax = null,
            Guid? productId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.OrderItem.ProductName!.Contains(filterText!))
                    .WhereIf(qtyMin.HasValue, e => e.OrderItem.Qty >= qtyMin!.Value)
                    .WhereIf(qtyMax.HasValue, e => e.OrderItem.Qty <= qtyMax!.Value)
                    .WhereIf(priceMin.HasValue, e => e.OrderItem.Price >= priceMin!.Value)
                    .WhereIf(priceMax.HasValue, e => e.OrderItem.Price <= priceMax!.Value)
                    .WhereIf(totalPriceMin.HasValue, e => e.OrderItem.TotalPrice >= totalPriceMin!.Value)
                    .WhereIf(totalPriceMax.HasValue, e => e.OrderItem.TotalPrice <= totalPriceMax!.Value)
                    .WhereIf(productId != null && productId != Guid.Empty, e => e.Product != null && e.Product.Id == productId);
        }

        public virtual async Task<List<OrderItem>> GetListAsync(
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
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, qtyMin, qtyMax, priceMin, priceMax, totalPriceMin, totalPriceMax);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? OrderItemConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            int? qtyMin = null,
            int? qtyMax = null,
            float? priceMin = null,
            float? priceMax = null,
            float? totalPriceMin = null,
            float? totalPriceMax = null,
            Guid? productId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, qtyMin, qtyMax, priceMin, priceMax, totalPriceMin, totalPriceMax, productId);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<OrderItem> ApplyFilter(
            IQueryable<OrderItem> query,
            string? filterText = null,
            int? qtyMin = null,
            int? qtyMax = null,
            float? priceMin = null,
            float? priceMax = null,
            float? totalPriceMin = null,
            float? totalPriceMax = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.ProductName!.Contains(filterText!))
                    .WhereIf(qtyMin.HasValue, e => e.Qty >= qtyMin!.Value)
                    .WhereIf(qtyMax.HasValue, e => e.Qty <= qtyMax!.Value)
                    .WhereIf(priceMin.HasValue, e => e.Price >= priceMin!.Value)
                    .WhereIf(priceMax.HasValue, e => e.Price <= priceMax!.Value)
                    .WhereIf(totalPriceMin.HasValue, e => e.TotalPrice >= totalPriceMin!.Value)
                    .WhereIf(totalPriceMax.HasValue, e => e.TotalPrice <= totalPriceMax!.Value);
        }
    }
}