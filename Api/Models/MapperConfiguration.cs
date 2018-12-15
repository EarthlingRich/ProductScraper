using AutoMapper;
using Model.Models;

namespace Api.Models
{
    public class MapperConfiguration : Profile
    {
        public MapperConfiguration()
        {
            CreateMap<IngredientViewModel, Ingredient>()
                .ForMember(_ => _.KeyWords, opt => opt.Ignore())
                .ForMember(_ => _.AllergyKeywords, opt => opt.Ignore());
            CreateMap<Product, ProductUpdateViewModel>()
                .ForMember(_ => _.Request, opt => opt.Ignore());
            CreateMap<ProductUpdateRequest, Product>(MemberList.Source);
        }
    }
}
