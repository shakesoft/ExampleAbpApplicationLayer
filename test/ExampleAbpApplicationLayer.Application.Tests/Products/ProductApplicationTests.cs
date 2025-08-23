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
            result.Items.Any(x => x.Id == Guid.Parse("d6604948-933e-4944-8191-ab78d78f1d30")).ShouldBe(true);
            result.Items.Any(x => x.Id == Guid.Parse("1fb2febb-25f1-48b5-83f0-4c7b4c218476")).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Act
            var result = await _productsAppService.GetAsync(Guid.Parse("d6604948-933e-4944-8191-ab78d78f1d30"));

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(Guid.Parse("d6604948-933e-4944-8191-ab78d78f1d30"));
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new ProductCreateDto
            {
                Name = "bac02df9832f493cada933bc16daec5d7537d5e15a954fc18b3d",
                Desc = "893e6f582",
                Price = 483816289,
                IsActive = true
            };

            // Act
            var serviceResult = await _productsAppService.CreateAsync(input);

            // Assert
            var result = await _productRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe("bac02df9832f493cada933bc16daec5d7537d5e15a954fc18b3d");
            result.Desc.ShouldBe("893e6f582");
            result.Price.ShouldBe(483816289);
            result.IsActive.ShouldBe(true);
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var input = new ProductUpdateDto()
            {
                Name = "6f2999bf4a71400d98bd5217e63611477d14",
                Desc = "af562c7a791346e0b6fc4163368bb75deef92116279d4acfa945aac0ed10e57bd5732d565c7a4d6a9088",
                Price = 1002811477,
                IsActive = true
            };

            // Act
            var serviceResult = await _productsAppService.UpdateAsync(Guid.Parse("d6604948-933e-4944-8191-ab78d78f1d30"), input);

            // Assert
            var result = await _productRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe("6f2999bf4a71400d98bd5217e63611477d14");
            result.Desc.ShouldBe("af562c7a791346e0b6fc4163368bb75deef92116279d4acfa945aac0ed10e57bd5732d565c7a4d6a9088");
            result.Price.ShouldBe(1002811477);
            result.IsActive.ShouldBe(true);
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Act
            await _productsAppService.DeleteAsync(Guid.Parse("d6604948-933e-4944-8191-ab78d78f1d30"));

            // Assert
            var result = await _productRepository.FindAsync(c => c.Id == Guid.Parse("d6604948-933e-4944-8191-ab78d78f1d30"));

            result.ShouldBeNull();
        }
    }
}