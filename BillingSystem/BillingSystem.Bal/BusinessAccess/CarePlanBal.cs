// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CarePlanBal.cs" company="Spadez">
//   OmniHealthcare
// </copyright>
// <summary>
//   The holiday planner bal.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace BillingSystem.Bal.BusinessAccess
{
    using System.Collections.Generic;
    using System.Linq;

    using BillingSystem.Bal.Mapper;
    using BillingSystem.Model;
    using BillingSystem.Model.CustomModel;
    using BillingSystem.Repository.GenericRepository;

    /// <summary>
    /// The holiday planner bal.
    /// </summary>
    public class CarePlanBal : BaseBal
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CarePlanBal"/> class.
        /// </summary>
        public CarePlanBal()
        {
            this.CarePlanMapper = new CarePlanMapper();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the holiday planner mapper.
        /// </summary>
        private CarePlanMapper CarePlanMapper { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<CareplanCustomModel> GetCarePlan()
        {
            var list = new List<CareplanCustomModel>();
            using (var carePlanRep = this.UnitOfWork.CarePlanRepository)
            {
                var lstCarePlan = carePlanRep.GetAll().ToList();
                if (lstCarePlan.Count > 0)
                {
                    list.AddRange(lstCarePlan.Select(item => CarePlanMapper.MapModelToViewModel(item)));
                }
            }

            return list;
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="carePlanId">The Holiday Planner Id.</param>
        /// <returns>
        /// The <see cref="CarePlan" />.
        /// </returns>
        public Careplan GetCarePlanById(int? carePlanId)
        {
            using (var rep = this.UnitOfWork.CarePlanRepository)
            {
                var model = rep.Where(x => x.Id == carePlanId).FirstOrDefault();
                return model;
            }
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="val"></param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int SaveCarePlan(Careplan model, int val)
        {
            using (var rep = UnitOfWork.CarePlanRepository)
            {
                if (model.Id > 0)
                {
                    var obj = new CarePlanTaskBal().GetCarePlanTaskByCarePlanId(Convert.ToInt32(model.Id));
                    var carePlanId =
                        obj != null ? Convert.ToInt32(obj.CarePlanId) : 0;
                    if (carePlanId > 0 && val != 2) //val == 2 means it is saving/updating
                    {
                        return -5; //it means passing task id is assign to patient
                    }
                    else
                    {
                        rep.UpdateEntity(model, model.Id);
                    }
                }
                else
                {
                    rep.Create(model);
                }

                return model.Id;
            }
        }


        public List<CareplanCustomModel> GetActiveCarePlan(int corporateId, int facilityId, bool val)
        {
            var list = new List<CareplanCustomModel>();
            using (var carePlanTaskRep = UnitOfWork.CarePlanRepository)
            {
                var lstCarePlanTask = carePlanTaskRep.Where(x => x.IsActive == val && x.CorporateId == corporateId && x.FacilityId == facilityId).ToList();
                if (lstCarePlanTask.Count > 0)
                    list.AddRange(lstCarePlanTask.Select(item => CarePlanMapper.MapModelToViewModel(item)));
            }

            return list;

        }

        public int GetTaskNumber(int facilityId, int corporateId)
        {
            using (var rep = UnitOfWork.CarePlanRepository)
            {

                var result =
                    rep.Where(x => x.FacilityId == facilityId && x.CorporateId == corporateId).Max(m => m.PlanNumber);
                return result != null ? Convert.ToInt32(result) + 1 : 1;
            }
        }


        public bool CheckDuplicateCarePlanNumber(int id, string planNumber, int facilityId, int corporateId)
        {
            using (var cRep=UnitOfWork.CarePlanRepository)
            {
                var check =
                    cRep.Where(
                        x =>
                            x.Id != id && x.PlanNumber.Trim().ToLower().Equals(planNumber) && x.FacilityId == facilityId &&
                            x.CorporateId == corporateId).FirstOrDefault();
                return check != null ? true : false;
            }
        }
        /// <summary>
        /// Delete the care plan when IsActive is 0
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int DeleteCarePlan(Careplan model)
        {
            using (var rep = UnitOfWork.CarePlanRepository)
            {
                if (model.Id > 0)
                {
                    rep.Delete(model.Id);
                }
                return model.Id;
            }
        }
        #endregion
    }
}
