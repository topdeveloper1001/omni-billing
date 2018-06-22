using System;

namespace BillingSystem.Bal.BusinessAccess
{
    using System.Collections.Generic;
    using System.Linq;

    using Mapper;
    using Model;
    using Model.CustomModel;

    /// <summary>
    /// The holiday planner bal.
    /// </summary>
    public class AppointmentTypesBal : BaseBal
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AppointmentTypesBal"/> class.
        /// </summary>
        public AppointmentTypesBal()
        {
            AppointmentTypesMapper = new AppointmentTypesMapper();
        }

        #endregion

        #region Properties


        /// <summary>
        /// Gets or sets the appointment types mapper.
        /// </summary>
        /// <value>
        /// The appointment types mapper.
        /// </value>
        private AppointmentTypesMapper AppointmentTypesMapper { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// The <see cref="AppointmentTypes" />.
        /// </returns>
        public AppointmentTypes GetAppointmentTypesById(int? id)
        {
            using (var rep = UnitOfWork.AppointmentTypesRepository)
            {
                var model = rep.Where(x => x.Id == id).FirstOrDefault();
                return model;
            }
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
            using (var rep = UnitOfWork.AppointmentTypesRepository)
            {
                if (model.Id > 0)
                {
                    var current = rep.GetSingle(model.Id);
                    model.CreatedBy = current.CreatedBy;
                    model.CraetedDate = current.CraetedDate;
                    rep.UpdateEntity(model, model.Id);
                }
                else
                {
                    rep.Create(model);
                }

                return model.Id;
            }
        }

        public int DeleteAppointmentTyepsData(AppointmentTypes model)
        {
            using (var rep = UnitOfWork.AppointmentTypesRepository)
            {
                if (model.Id > 0)
                {
                    rep.Delete(model);
                }
                return model.Id;

            }
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
            using (var rep = UnitOfWork.AppointmentTypesRepository)
            {
                var isExists =
                    rep.Where(model => model.Id != id && model.Name.Trim().ToLower().Equals(name) && model.CorporateId == corporateId && model.FacilityId == facilityId)
                        .Any();
                return isExists;
            }
        }

        /// <summary>
        /// Gets the maximum category number.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <returns></returns>
        public int GetMaxCategoryNumber(int facilityId, int corporateId)
        {
            using (var rep = UnitOfWork.AppointmentTypesRepository)
            {

                var result =
                    rep.Where(x => x.FacilityId == facilityId && x.CorporateId == corporateId).ToList();

                return result.Any() ? Convert.ToInt32(result.Max(x => Convert.ToInt32(x.CategoryNumber))) : 0;
            }
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
            using (var rep = UnitOfWork.AppointmentTypesRepository)
            {
                var isExists =
                    rep.Where(model => model.Id != id && model.CategoryNumber.Trim().ToLower().Equals(categoryNumber) && model.CorporateId == corporateId && model.FacilityId == facilityId)
                        .Any();
                return isExists;
            }
        }

        /// <summary>
        /// Gets the appointmne types by facility identifier.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public List<AppointmentTypes> GetAppointmneTypesByFacilityId(int facilityId)
        {
            using (var rep = UnitOfWork.AppointmentTypesRepository)
            {
                var list = rep.Where(x => x.FacilityId == facilityId && x.IsActive).ToList();
                return list;
            }
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
            using (var rep = UnitOfWork.AppointmentTypesRepository)
            {
                var list = rep.GetAppointmentTypesData(corporateId, facilityId, showInActive).ToList();
                return list;
            }
        }

        public List<AppointmentTypes> GetAppointmentTypesByFacilityId(int facilityId, List<int> ids)
        {
            using (var rep = UnitOfWork.AppointmentTypesRepository)
            {
                var list = rep.Where(x => x.FacilityId == facilityId && x.IsActive && ids.Contains(x.Id)).ToList();
                return list;
            }
        }

        public bool IsEquipmentRequiredWithProcedure(int id)
        {
            using (var rep = UnitOfWork.AppointmentTypesRepository)
            {
                var isEquipmentRequired = rep.Where(x => x.Id == id).Select(e => e.ExtValue1).FirstOrDefault();
                return !string.IsNullOrEmpty(isEquipmentRequired) && Convert.ToBoolean(isEquipmentRequired);
            }
        }
        #endregion
    }
}
