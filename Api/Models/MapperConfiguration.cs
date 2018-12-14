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
            CreateMap<ProductViewModel, Product>()
                .ForMember(_ => _.Name, opt => opt.Ignore())
                .ForMember(_ => _.MatchedIngredients, opt => opt.Ignore())
                .ForMember(_ => _.ProductIngredients, opt => opt.Ignore())
                .ForMember(_ => _.Ingredients, opt => opt.Ignore())
                .ForMember(_ => _.VeganType, opt => opt.Ignore());
        }
    }
}
