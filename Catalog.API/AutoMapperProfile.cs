using AutoMapper;
using Catalog.Application;
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
