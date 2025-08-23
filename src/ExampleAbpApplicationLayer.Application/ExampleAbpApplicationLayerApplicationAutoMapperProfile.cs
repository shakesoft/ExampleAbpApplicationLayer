using ExampleAbpApplicationLayer.OrderItems;
using ExampleAbpApplicationLayer.Orders;
using System;
using ExampleAbpApplicationLayer.Shared;
using ExampleAbpApplicationLayer.Products;
using Volo.Abp.AutoMapper;
using AutoMapper;

namespace ExampleAbpApplicationLayer;

public class ExampleAbpApplicationLayerApplicationAutoMapperProfile : Profile
{
    public ExampleAbpApplicationLayerApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */

        CreateMap<Product, ProductDto>();
        CreateMap<Product, ProductExcelDto>();

        CreateMap<Order, OrderDto>();
        CreateMap<Order, OrderExcelDto>();

        CreateMap<OrderItem, OrderItemDto>();
        CreateMap<OrderItemWithNavigationProperties, OrderItemWithNavigationPropertiesDto>();
        CreateMap<Product, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name));
    }
}