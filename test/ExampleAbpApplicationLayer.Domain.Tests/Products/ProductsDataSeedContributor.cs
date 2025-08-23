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
                id: Guid.Parse("141338a2-38b2-4506-add6-2adc4afb993a"),
                name: "e6bf143753e3420d857e9a424a965dc5e8441c7fdec24bf",
                desc: "8d7d45bc29e4496cb71ecef8f212fdf3323602efb12844eda4b9532a",
                price: 1156311753,
                isActive: true
            ));

            await _productRepository.InsertAsync(new Product
            (
                id: Guid.Parse("04e3ee65-95e5-404d-93c1-0cdbc590e4d5"),
                name: "9f913530214946",
                desc: "f6c75408264944328b9abe6f304e9d21bd8b87bc0353496888f88b47c6f7",
                price: 1504960767,
                isActive: true
            ));

            await _unitOfWorkManager!.Current!.SaveChangesAsync();

            IsSeeded = true;
        }
    }
}