using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;


namespace BillingSystem.Bal.BusinessAccess
{
    public class FacilityDepartmentService : IFacilityDepartmentService
    {

        private readonly IRepository<FacilityDepartment> _repository;
        private readonly IMapper _mapper;

        public FacilityDepartmentService(IRepository<FacilityDepartment> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }


        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <param name="corpoarteId">The corpoarte identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="showInActive">if set to <c>true</c> [show in active].</param>
        /// <returns>
        /// Return the Entity List
        /// </returns>
        public List<FacilityDepartmentCustomModel> GetFacilityDepartmentList(int corpoarteId, int facilityId, bool showInActive)
        {
            var lstFacilityDepartment = _repository.Where(a => a.IsActive == showInActive && a.FacilityId == facilityId && a.CorporateId == corpoarteId).ToList();
            return lstFacilityDepartment.Select(item => _mapper.Map<FacilityDepartmentCustomModel>(item)).ToList();
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public List<FacilityDepartmentCustomModel> SaveFacilityDepartment(FacilityDepartment model)
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
            var list = GetFacilityDepartmentList(model.CorporateId, model.FacilityId, true);
            return list;
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="facilityDepartmentId">The facility department identifier.</param>
        /// <returns></returns>
        public FacilityDepartment GetFacilityDepartmentById(int? facilityDepartmentId)
        {
            var model = _repository.Where(x => x.Id == facilityDepartmentId).FirstOrDefault();
            return model;
        }
    }
}
