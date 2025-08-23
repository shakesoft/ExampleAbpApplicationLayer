using Volo.Abp.Application.Dtos;
using System;

namespace ExampleAbpApplicationLayer.Products
{
    public abstract class ProductExcelDownloadDtoBase
    {
        public string DownloadToken { get; set; } = null!;

        public string? FilterText { get; set; }

        public string? Name { get; set; }
        public float? PriceMin { get; set; }
        public float? PriceMax { get; set; }
        public bool? IsActive { get; set; }

        public ProductExcelDownloadDtoBase()
        {

        }
    }
}