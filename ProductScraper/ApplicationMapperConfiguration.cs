using AutoMapper;
using Model.Models;
using Model.Requests;

namespace ProductScraper
{
    public class ApplicationMapperConfiguration : Profile
    {
        public ApplicationMapperConfiguration()
        {
            CreateMap<ProductStoreRequest, Product>(MemberList.Source);
        }
    }
}
