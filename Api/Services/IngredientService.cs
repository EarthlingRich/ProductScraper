using System;
using Api.Models;
using Api.Persistance.Interfaces.Repositories;
using Model.Models;

namespace Api.Services
{
    public class IngredientService
    {
        readonly IUnitOfWork _unitOfWork;

        public IngredientService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Create(IngredientViewModel viewModel)
        {
            var ingredient = new Ingredient {
                Name = viewModel.Name
            };
            _unitOfWork.Ingredients.Add(ingredient);
            _unitOfWork.Complete();
        }
    }
}
