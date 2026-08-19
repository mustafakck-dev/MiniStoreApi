using AutoMapper;
using Entities.DTOs;
using Entities.Models;

namespace Services.Mapping;

public class MappingProfile : Profile  // Hangi türün hangi türe çevrilebileceğini AutoMapper’a öğretmek.
{
    public MappingProfile()
    {
        CreateMap<Product, ProductDto>()
    .ForMember(
        destination => destination.CategoryName,
        options => options.MapFrom(
            source => source.Category != null
                ? source.Category.Name
                : null));

        CreateMap<ProductForCreationDto, Product>();

        CreateMap<ProductForUpdateDto, Product>();

        CreateMap<Category, CategoryDto>();

        CreateMap<CategoryForCreationDto, Category>();

        CreateMap<CategoryForUpdateDto, Category>();

        CreateMap<UserForRegistrationDto, User>();

        CreateMap<OrderItem, OrderItemDto>()
    .ForMember(destination => destination.ProductName, options => options.MapFrom(source => source.Product.Name))
    .ForMember(destination => destination.LineTotal, options => options.MapFrom(source => source.Quantity * source.UnitPrice));

        CreateMap<Order, OrderDto>()
            .ForMember(destination => destination.Items, options => options.MapFrom(source => source.OrderItems));
    }
}