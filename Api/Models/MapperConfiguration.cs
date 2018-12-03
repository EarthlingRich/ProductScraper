using System;
using AutoMapper;
using Model.Models;

namespace Api.Models
{
    public class MapperConfiguration : Profile
    {
        public MapperConfiguration()
        {
            CreateMap<IngredientViewModel, Ingredient>()
                .ForMember(_ => _.KeyWords, opt => opt.Ignore());
        }
    }
}
