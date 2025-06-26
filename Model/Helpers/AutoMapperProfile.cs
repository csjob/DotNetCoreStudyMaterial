using AutoMapper;
using DotNetCoreWebAPI.Model.Dto;

namespace DotNetCoreWebAPI.Model.Helpers
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Product, ProductDto>();       // For output
            CreateMap<ProductDto, Product>();       // For input (optional)
        }
    }
}
