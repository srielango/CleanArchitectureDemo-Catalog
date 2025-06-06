using AutoMapper;
using Catalog.Application.Products;
using Catalog.Domain.Entities;

namespace Catalog.API
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Product, ProductDto>().ReverseMap();
        }
    }
}
