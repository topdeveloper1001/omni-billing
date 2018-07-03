using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;


namespace BillingSystem.Bal.BusinessAccess
{
    public class DrugInstructionAndDosingService : IDrugInstructionAndDosingService
    { 
        private readonly IRepository<DrugInstructionAndDosing> _repository;
        private readonly IMapper _mapper;

        public DrugInstructionAndDosingService(IRepository<DrugInstructionAndDosing> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<DrugInstructionAndDosingCustomModel> GetDrugInstructionAndDosingList()
        {
            var list = new List<DrugInstructionAndDosingCustomModel>();
            var lstDrugInstructionAndDosing = _repository.Where(a => a.IsDeleted == null || !(bool)a.IsDeleted).ToList();
            if (lstDrugInstructionAndDosing.Count > 0)
            {
                if (lstDrugInstructionAndDosing.Any())
                {
                    list.AddRange(lstDrugInstructionAndDosing.Select(item => _mapper.Map<DrugInstructionAndDosingCustomModel>(item)));
                }
            }
            return list;
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public List<DrugInstructionAndDosingCustomModel> SaveDrugInstructionAndDosing(DrugInstructionAndDosing model)
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
            var list = GetDrugInstructionAndDosingList();
            return list;
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public DrugInstructionAndDosing GetDrugInstructionAndDosingById(int? id)
        {
            var model = _repository.Where(x => x.Id == id).FirstOrDefault();
            return model;
        }
    }
}
