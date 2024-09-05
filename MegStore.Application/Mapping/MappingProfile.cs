using AutoMapper;
using MegStore.Application.DTOs;
using MegStore.Core.Entities.ProductFolder;
using MegStore.Core.Entities.Users;
using Org.BouncyCastle.Bcpg;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MegStore.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<ProductDto, Product>()
                .ForMember(dest => dest.productId, opt => opt.Ignore());

          
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<CategoryDto, Category>()
                .ForMember(dest => dest.categoryId, opt => opt.Ignore());

          
            CreateMap<Order, OrderDto>().ReverseMap();
            CreateMap<OrderDto, Order>()
                .ForMember(dest => dest.orderId, opt => opt.Ignore());

            CreateMap<Cart, CartDto>().ReverseMap();
            CreateMap<Cart, CartDto>()
                .ForMember(dest => dest.CartId, opt => opt.Ignore());


            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}
