using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace ExampleAbpApplicationLayer.Products
{
    public partial interface IProductRepository : IRepository<Product, Guid>
    {

        Task DeleteAllAsync(
            string? filterText = null,
            string? name = null,
            float? priceMin = null,
            float? priceMax = null,
            bool? isActive = null,
            CancellationToken cancellationToken = default);
        Task<List<Product>> GetListAsync(
                    string? filterText = null,
                    string? name = null,
                    float? priceMin = null,
                    float? priceMax = null,
                    bool? isActive = null,
                    string? sorting = null,
                    int maxResultCount = int.MaxValue,
                    int skipCount = 0,
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
            string? filterText = null,
            string? name = null,
            float? priceMin = null,
            float? priceMax = null,
            bool? isActive = null,
            CancellationToken cancellationToken = default);
    }
}