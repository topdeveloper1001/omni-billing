using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Repository.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class DrugInteractionsService : IDrugInteractionsService
    {
        private readonly IRepository<DrugInteractions> _repository;
        private readonly IMapper _mapper;

        public DrugInteractionsService(IRepository<DrugInteractions> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>
        /// Return the Entity List
        /// </returns>
        public List<DrugInteractionsCustomModel> GetDrugInteractionsList()
        {
            var lstDrugInteractions = _repository.Where(a => !a.IsDeleted).ToList();
            return lstDrugInteractions.Select(item => _mapper.Map<DrugInteractionsCustomModel>(item)).ToList();
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public List<DrugInteractionsCustomModel> SaveDrugInteractions(DrugInteractions model)
        {
            if (model.Id > 0)
            {
                var current = _repository.GetSingle(model.Id);
                model.CreatedBy = current.CreatedBy;
                model.CreatedDate = current.CreatedDate;
                _repository.UpdateEntity(model, model.Id);
            }
            else
                _repository.Create(model);

            var currentId = model.Id;
            var list = GetDrugInteractionsList();
            return list;
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public DrugInteractions GetDrugInteractionsById(int? id)
        {
            var model = _repository.Where(x => x.Id == id).FirstOrDefault();
            return model;
        }
    }
}
