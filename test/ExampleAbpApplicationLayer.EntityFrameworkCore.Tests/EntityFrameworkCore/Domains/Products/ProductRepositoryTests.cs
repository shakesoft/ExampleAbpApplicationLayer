using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using ExampleAbpApplicationLayer.Products;
using ExampleAbpApplicationLayer.EntityFrameworkCore;
using Xunit;

namespace ExampleAbpApplicationLayer.EntityFrameworkCore.Domains.Products
{
    public class ProductRepositoryTests : ExampleAbpApplicationLayerEntityFrameworkCoreTestBase
    {
        private readonly IProductRepository _productRepository;

        public ProductRepositoryTests()
        {
            _productRepository = GetRequiredService<IProductRepository>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _productRepository.GetListAsync(
                    name: "e6bf143753e3420d857e9a424a965dc5e8441c7fdec24bf",
                    isActive: true
                );

                // Assert
                result.Count.ShouldBe(1);
                result.FirstOrDefault().ShouldNotBe(null);
                result.First().Id.ShouldBe(Guid.Parse("141338a2-38b2-4506-add6-2adc4afb993a"));
            });
        }

        [Fact]
        public async Task GetCountAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _productRepository.GetCountAsync(
                    name: "9f913530214946",
                    isActive: true
                );

                // Assert
                result.ShouldBe(1);
            });
        }
    }
}