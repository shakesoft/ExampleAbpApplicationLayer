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
                    name: "11c71ef2c24743f9a15561f887f77ff1af1fc02044ad40b08b2cdee1a9f4227728370b25ec",
                    isActive: true
                );

                // Assert
                result.Count.ShouldBe(1);
                result.FirstOrDefault().ShouldNotBe(null);
                result.First().Id.ShouldBe(Guid.Parse("d6604948-933e-4944-8191-ab78d78f1d30"));
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
                    name: "c75153fbae544e9f9877689c8f8b37be6490c5f01e4348ab93f19a95ea61a964bcbe1532bf6c4cc78beb027269229847db",
                    isActive: true
                );

                // Assert
                result.ShouldBe(1);
            });
        }
    }
}