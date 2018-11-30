using System;
using Api.Models;
using Api.Persistance.Interfaces.Repositories;
using AutoMapper;
using Model.Models;

namespace Api.Services
{
    public class IngredientService
    {
        readonly IMapper _mapper;
        readonly IUnitOfWork _unitOfWork;

        public IngredientService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public void Create(IngredientViewModel viewModel)
        {
            var ingredient = _mapper.Map<Ingredient>(viewModel);
            _unitOfWork.Ingredients.Add(ingredient);
            _unitOfWork.Complete();
        }

        public void Update(IngredientViewModel viewModel)
        {
            var ingredient = _unitOfWork.Ingredients.Get(viewModel.Id);
            _mapper.Map(viewModel, ingredient);
            _unitOfWork.Complete();
        }
    }
}
