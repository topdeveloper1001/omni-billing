using AutoMapper;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace BillingSystem.Bal.BusinessAccess
{

    /// <summary>
    /// The holiday planner bal.
    /// </summary>
    public class AppointmentTypesService : IAppointmentTypesService
    {

        private readonly IRepository<AppointmentTypes> _repository;
        private readonly BillingEntities _context;
        private readonly IMapper _mapper;

        public AppointmentTypesService(IRepository<AppointmentTypes> repository, BillingEntities context, IMapper mapper)
        {
            _repository = repository;
            _context = context;
            _mapper = mapper;
        }

        #region Public Methods and Operators

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// The <see cref="AppointmentTypes" />.
        /// </returns>
        public AppointmentTypes GetAppointmentTypesById(int id)
        {
            var model = _repository.Where(x => x.Id == id).FirstOrDefault();
            return model;

        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// The <see cref="int" />.
        /// </returns>
        public int SaveAppointmentTypes(AppointmentTypes model)
        {
            if (model.Id > 0)
            {
                var current = _repository.GetSingle(model.Id);
                model.CreatedBy = current.CreatedBy;
                model.CraetedDate = current.CraetedDate;
                _repository.Update(model, model.Id);
            }
            else
            {
                _repository.Create(model);
            }

            return model.Id;

        }

        public int DeleteAppointmentTyepsData(AppointmentTypes model)
        {
            if (model.Id > 0)
            {
                _repository.Delete(model);
            }
            return model.Id;

        }

        /// <summary>
        /// Checks the type of the duplicate appointment.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="name">The name.</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public bool CheckDuplicateAppointmentType(int id, string name, int? corporateId, int? facilityId)
        { 
                var isExists = _repository.Where(model => model.Id != id && model.Name.Trim().ToLower().Equals(name) 
                && model.CorporateId == corporateId && model.FacilityId == facilityId)
                        .Any();
                return isExists;
             
        }

        /// <summary>
        /// Gets the maximum category number.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <returns></returns>
        public int GetMaxCategoryNumber(int facilityId, int corporateId)
        {
            var result = _repository.Where(x => x.FacilityId == facilityId && x.CorporateId == corporateId).ToList();

            return result.Any() ? Convert.ToInt32(result.Max(x => Convert.ToInt32(x.CategoryNumber))) : 0;

        }

        /// <summary>
        /// Checks the duplicate category number.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="categoryNumber">The category number.</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public bool CheckDuplicateCategoryNumber(int id, string categoryNumber, int? corporateId, int? facilityId)
        {
            var isExists = _repository.Where(model => model.Id != id && model.CategoryNumber.Trim().ToLower().Equals(categoryNumber)
                                && model.CorporateId == corporateId && model.FacilityId == facilityId).Any();
            return isExists;

        }

        /// <summary>
        /// Gets the appointmne types by facility identifier.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public List<AppointmentTypes> GetAppointmneTypesByFacilityId(int facilityId)
        {
            var list = _repository.Where(x => x.FacilityId == facilityId && x.IsActive).ToList();
            return list;

        }

        /// <summary>
        /// Gets the appointment types data.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="showInActive">if set to <c>true</c> [show in active].</param>
        /// <returns></returns>
        public List<AppointmentTypesCustomModel> GetAppointmentTypesData(int corporateId, int facilityId, bool showInActive)
        {
            var spName = string.Format("EXEC {0} @FacilityId,@CorporateId, @ShowInActive", StoredProcedures.SPORC_GetAppointmentTypes);
            var sqlParameters = new SqlParameter[3];
            sqlParameters[0] = new SqlParameter("FacilityId", facilityId);
            sqlParameters[1] = new SqlParameter("CorporateId", corporateId);
            sqlParameters[2] = new SqlParameter("ShowInActive", showInActive);
            IEnumerable<AppointmentTypesCustomModel> result = _context.Database.SqlQuery<AppointmentTypesCustomModel>(spName, sqlParameters);
            return result.ToList();
        }

        public List<AppointmentTypes> GetAppointmentTypesByFacilityId(int facilityId, List<int> ids)
        {
            var list = _repository.Where(x => x.FacilityId == facilityId && x.IsActive && ids.Contains(x.Id)).ToList();
            return list;

        }

        public bool IsEquipmentRequiredWithProcedure(int id)
        {
            var isEquipmentRequired = _repository.Where(x => x.Id == id).Select(e => e.ExtValue1).FirstOrDefault();
            return !string.IsNullOrEmpty(isEquipmentRequired) && Convert.ToBoolean(isEquipmentRequired);

        }
        #endregion
    }
}
