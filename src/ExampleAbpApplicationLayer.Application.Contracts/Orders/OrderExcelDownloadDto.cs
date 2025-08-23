using ExampleAbpApplicationLayer.Enums.Orders;
using Volo.Abp.Application.Dtos;
using System;

namespace ExampleAbpApplicationLayer.Orders
{
    public abstract class OrderExcelDownloadDtoBase
    {
        public string DownloadToken { get; set; } = null!;

        public string? FilterText { get; set; }

        public DateTime? OrderDateMin { get; set; }
        public DateTime? OrderDateMax { get; set; }
        public float? TotalAmountMin { get; set; }
        public float? TotalAmountMax { get; set; }
        public OrderStatus? Status { get; set; }

        public OrderExcelDownloadDtoBase()
        {

        }
    }
}