using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace ExampleAbpApplicationLayer.Products
{
    public abstract class ProductsAppServiceTests<TStartupModule> : ExampleAbpApplicationLayerApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly IProductsAppService _productsAppService;
        private readonly IRepository<Product, Guid> _productRepository;

        public ProductsAppServiceTests()
        {
            _productsAppService = GetRequiredService<IProductsAppService>();
            _productRepository = GetRequiredService<IRepository<Product, Guid>>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Act
            var result = await _productsAppService.GetListAsync(new GetProductsInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Id == Guid.Parse("141338a2-38b2-4506-add6-2adc4afb993a")).ShouldBe(true);
            result.Items.Any(x => x.Id == Guid.Parse("04e3ee65-95e5-404d-93c1-0cdbc590e4d5")).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Act
            var result = await _productsAppService.GetAsync(Guid.Parse("141338a2-38b2-4506-add6-2adc4afb993a"));

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(Guid.Parse("141338a2-38b2-4506-add6-2adc4afb993a"));
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new ProductCreateDto
            {
                Name = "b2761cdd53fe4a3cb8ab677333066c161bd4c0b77fc54ff",
                Desc = "66c239536b0d4e27831753a9f6d751750c51ae3d662e419daefe",
                Price = 594822901,
                IsActive = true
            };

            // Act
            var serviceResult = await _productsAppService.CreateAsync(input);

            // Assert
            var result = await _productRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe("b2761cdd53fe4a3cb8ab677333066c161bd4c0b77fc54ff");
            result.Desc.ShouldBe("66c239536b0d4e27831753a9f6d751750c51ae3d662e419daefe");
            result.Price.ShouldBe(594822901);
            result.IsActive.ShouldBe(true);
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var input = new ProductUpdateDto()
            {
                Name = "6be7f8a92c764c49b10",
                Desc = "3e8200c65d9f47e29a059b8abf5b8329116ac3cc1200433089d6bdb6496527d834a63df59478447b8f82157",
                Price = 638338967,
                IsActive = true
            };

            // Act
            var serviceResult = await _productsAppService.UpdateAsync(Guid.Parse("141338a2-38b2-4506-add6-2adc4afb993a"), input);

            // Assert
            var result = await _productRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe("6be7f8a92c764c49b10");
            result.Desc.ShouldBe("3e8200c65d9f47e29a059b8abf5b8329116ac3cc1200433089d6bdb6496527d834a63df59478447b8f82157");
            result.Price.ShouldBe(638338967);
            result.IsActive.ShouldBe(true);
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Act
            await _productsAppService.DeleteAsync(Guid.Parse("141338a2-38b2-4506-add6-2adc4afb993a"));

            // Assert
            var result = await _productRepository.FindAsync(c => c.Id == Guid.Parse("141338a2-38b2-4506-add6-2adc4afb993a"));

            result.ShouldBeNull();
        }
    }
}