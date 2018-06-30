using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Repository.Interfaces;
using AutoMapper;
using BillingSystem.Bal.Interfaces;


namespace BillingSystem.Bal.BusinessAccess
{
    public class DrugAllergyLogService : IDrugAllergyLogService
    {
        private readonly IRepository<DrugAllergyLog> _repository;
        private readonly BillingEntities _context;
        private readonly IMapper _mapper;

        public DrugAllergyLogService(IRepository<DrugAllergyLog> repository, BillingEntities context, IMapper mapper)
        {
            _repository = repository;
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<DrugAllergyLogCustomModel> GetDrugAllergyLogList(int corporateId, int facilityid)
        {
            var list = new List<DrugAllergyLogCustomModel>();
            var lstDrugAllergyLog = _repository.Where(a => a.CorporateId == corporateId && a.FacilityId == facilityid).ToList();
            if (lstDrugAllergyLog.Count > 0)
            {
                if (lstDrugAllergyLog.Any())
                {
                    list.AddRange(
                        lstDrugAllergyLog.Select(x => _mapper.Map<DrugAllergyLogCustomModel>(x)));
                }
            }
            return list;
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public List<DrugAllergyLogCustomModel> SaveDrugAllergyLog(DrugAllergyLog model)
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

            var list = GetDrugAllergyLogList(Convert.ToInt32(model.CorporateId), Convert.ToInt32(model.FacilityId));
            return list;
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public DrugAllergyLog GetDrugAllergyLogById(int? id)
        {
            var model = _repository.Where(x => x.Id == id).FirstOrDefault();
            return model;
        }

        /// <summary>
        /// Saves the drug allergy log custom.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public bool SaveDrugAllergyLogCustom(DrugAllergyLog model)
        {
            try
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


                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
