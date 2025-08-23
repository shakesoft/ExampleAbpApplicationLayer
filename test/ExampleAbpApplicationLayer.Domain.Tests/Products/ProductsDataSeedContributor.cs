using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using ExampleAbpApplicationLayer.Products;

namespace ExampleAbpApplicationLayer.Products
{
    public class ProductsDataSeedContributor : IDataSeedContributor, ISingletonDependency
    {
        private bool IsSeeded = false;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public ProductsDataSeedContributor(IProductRepository productRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _productRepository = productRepository;
            _unitOfWorkManager = unitOfWorkManager;

        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (IsSeeded)
            {
                return;
            }

            await _productRepository.InsertAsync(new Product
            (
                id: Guid.Parse("d6604948-933e-4944-8191-ab78d78f1d30"),
                name: "11c71ef2c24743f9a15561f887f77ff1af1fc02044ad40b08b2cdee1a9f4227728370b25ec",
                desc: "ca38a7fceb674ede95bb5732eb52",
                price: 983272089,
                isActive: true
            ));

            await _productRepository.InsertAsync(new Product
            (
                id: Guid.Parse("1fb2febb-25f1-48b5-83f0-4c7b4c218476"),
                name: "c75153fbae544e9f9877689c8f8b37be6490c5f01e4348ab93f19a95ea61a964bcbe1532bf6c4cc78beb027269229847db",
                desc: "ea807e41d2b84c10a9f58c9c7534dd7d1e4add1437e64df6ab688756ec",
                price: 1794088944,
                isActive: true
            ));

            await _unitOfWorkManager!.Current!.SaveChangesAsync();

            IsSeeded = true;
        }
    }
}