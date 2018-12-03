using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Api.Persistance.Interfaces.Repositories;
using AutoMapper;
using Model.Models;

namespace Api.Services
{
    public class ProductService
    {
        readonly IMapper _mapper;
        readonly IUnitOfWork _unitOfWork;

        public ProductService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public Product ProcessMatchedIngredientsForProduct(Product product)
        {
            var ingredients = _unitOfWork.Ingredients.GetAll();

            foreach(var ingredient in ingredients)
            {
                foreach(var keyWord in ingredient.KeyWords)
                {
                    var match = Regex.Match(product.Ingredients, @"[\s,]" + keyWord + @"[\s,]");
                    if (match.Success)
                    {
                        product.MatchedIngredients.Add(ingredient);
                        break;
                    }
                }
            }

            return product;
        }
    }
}
