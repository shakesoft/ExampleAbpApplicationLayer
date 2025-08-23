using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace ExampleAbpApplicationLayer.Products
{
    public abstract class ProductManagerBase : DomainService
    {
        protected IProductRepository _productRepository;

        public ProductManagerBase(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public virtual async Task<Product> CreateAsync(
        string name, float price, bool isActive, string? desc = null)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            var product = new Product(
             GuidGenerator.Create(),
             name, price, isActive, desc
             );

            return await _productRepository.InsertAsync(product);
        }

        public virtual async Task<Product> UpdateAsync(
            Guid id,
            string name, float price, bool isActive, string? desc = null, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            var product = await _productRepository.GetAsync(id);

            product.Name = name;
            product.Price = price;
            product.IsActive = isActive;
            product.Desc = desc;

            product.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _productRepository.UpdateAsync(product);
        }

    }
}