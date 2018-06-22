// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CarePlanTaskBal.cs" company="SPadez">
//   Omnihelathcare
// </copyright>
// </Screen Owner>
// Shashank Modified on : Feb 09 2016
// </Screen Owner>
// <summary>
//   The holiday planner bal.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BillingSystem.Bal.BusinessAccess
{
    #region System Referneces
    using System;
    using System.Collections.Generic;
    using System.Linq;
    #endregion

    #region Project Referneces
    using BillingSystem.Bal.Mapper;
    using BillingSystem.Model;
    using BillingSystem.Model.CustomModel;
    using BillingSystem.Repository.GenericRepository;
    #endregion

    /// <summary>
    ///     The holiday planner bal.
    /// </summary>
    public class CarePlanTaskBal : BaseBal
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="CarePlanTaskBal" /> class.
        /// </summary>
        public CarePlanTaskBal()
        {
            this.CarePlanTaskMapper = new CarePlanTaskMapper();
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the holiday planner mapper.
        /// </summary>
        private CarePlanTaskMapper CarePlanTaskMapper { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The bind care plan.
        /// </summary>
        /// <param name="corporateId">
        /// The corporate id.
        /// </param>
        /// <param name="facilityId">
        /// The facility id.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<Careplan> BindCarePlan(int corporateId, int facilityId)
        {
            using (CarePlanRepository cBal = this.UnitOfWork.CarePlanRepository)
            {
                List<Careplan> list =
                    cBal.Where(x => x.IsActive && x.FacilityId == facilityId && x.CorporateId == corporateId && x.ExtValue1 != "9999").ToList();
                return list;
            }
        }

        /// <summary>
        /// The bind care plan task.
        /// </summary>
        /// <param name="careId">
        /// The care id.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<CarePlanTaskCustomModel> BindCarePlanTask(int careId)
        {
            var list = new List<CarePlanTaskCustomModel>();
            using (CarePlanTaskRepository cBal = this.UnitOfWork.CarePlanTaskRepository)
            {
                List<CarePlanTask> lstCarePlanTask = cBal.Where(x => x.CarePlanId == careId && x.IsActive).ToList();
                if (lstCarePlanTask.Count > 0)
                {
                    list.AddRange(lstCarePlanTask.Select(item => this.CarePlanTaskMapper.MapModelToViewModel(item)));
                }

                return list;
            }
        }

        /// <summary>
        /// The care plan id.
        /// </summary>
        /// <param name="corporateId">
        /// The corporate id.
        /// </param>
        /// <param name="facilityId">
        /// The facility id.
        /// </param>
        /// <param name="taskId">
        /// The task id.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int CarePlanId(int corporateId, int facilityId, int taskId)
        {
            using (CarePlanTaskRepository rep = this.UnitOfWork.CarePlanTaskRepository)
            {
                int? result =

                    // rep.Where(x => x.FacilityId == facilityId && x.CorporateId == corporateId && x.TaskNumber==taskId).Max(m => m.CarePlanId);
                    // return result != null ? Convert.ToInt32(result) : 0;
                    rep.Where(x => x.FacilityId == facilityId && x.CorporateId == corporateId && x.Id == taskId)
                        .Max(m => m.CarePlanId);
                return result != null ? Convert.ToInt32(result) : 0;
            }
        }

        /// <summary>
        /// The check duplicate task number.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="taskNumber">
        /// The task number.
        /// </param>
        /// <param name="facilityId">
        /// The facility id.
        /// </param>
        /// <param name="corporateId">
        /// The corporate id.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CheckDuplicateTaskNumber(int id, string taskNumber, int facilityId, int corporateId)
        {
            using (CarePlanTaskRepository cRep = this.UnitOfWork.CarePlanTaskRepository)
            {
                CarePlanTask check =
                    cRep.Where(
                        x =>
                        x.Id != id && x.TaskNumber.Trim().ToLower().Equals(taskNumber) && x.FacilityId == facilityId
                        && x.CorporateId == corporateId).FirstOrDefault();
                return check != null ? true : false;
            }
        }

        /// <summary>
        /// The get active care plan task.
        /// </summary>
        /// <param name="corporateId">
        /// The corporate id.
        /// </param>
        /// <param name="facilityId">
        /// The facility id.
        /// </param>
        /// <param name="val"></param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<CarePlanTaskCustomModel> GetActiveCarePlanTask(int corporateId, int facilityId, bool val)
        {
            var list = new List<CarePlanTaskCustomModel>();
            using (CarePlanTaskRepository carePlanTaskRep = this.UnitOfWork.CarePlanTaskRepository)
            {
                List<CarePlanTask> lstCarePlanTask =
                    carePlanTaskRep.Where(
                        x =>
                        x.IsActive == val && x.CorporateId == corporateId && x.FacilityId == facilityId && x.CarePlanId != null
                        && x.CarePlanId != 9999)
                        .ToList();
                if (lstCarePlanTask.Count > 0)
                {
                    list.AddRange(lstCarePlanTask.Select(item => this.CarePlanTaskMapper.MapModelToViewModel(item)));
                }
            }

            return list;
        }

        /// <summary>
        /// The get care plan name by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetCarePlanNameById(int id)
        {
            using (CarePlanRepository cBal = this.UnitOfWork.CarePlanRepository)
            {
                Careplan list = cBal.Where(x => x.IsActive && x.Id == id).FirstOrDefault();

                return list != null && list.Name != null ? list.Name : string.Empty;
            }
        }

        /// <summary>
        /// Gets the care plan number by identifier.
        /// </summary>
        /// <param name="id">
        /// The identifier.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetCarePlanNumberById(int id)
        {
            using (CarePlanRepository cBal = this.UnitOfWork.CarePlanRepository)
            {
                Careplan list = cBal.Where(x => x.IsActive && x.Id == id).FirstOrDefault();
                return list != null && list.PlanNumber != null ? list.PlanNumber : string.Empty;
            }
        }

        /// <summary>
        ///     Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<CarePlanTaskCustomModel> GetCarePlanTask()
        {
            var list = new List<CarePlanTaskCustomModel>();
            using (CarePlanTaskRepository CarePlanTaskRep = this.UnitOfWork.CarePlanTaskRepository)
            {
                List<CarePlanTask> lstCarePlanTask = CarePlanTaskRep.GetAll().ToList();
                if (lstCarePlanTask.Count > 0)
                {
                    list.AddRange(lstCarePlanTask.Select(item => new CarePlanTaskCustomModel()));
                }
            }

            return list;
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="CarePlanTaskId">
        /// The Holiday Planner Id.
        /// </param>
        /// <returns>
        /// The <see cref="CarePlanTask"/>.
        /// </returns>
        public CarePlanTask GetCarePlanTaskById(int? CarePlanTaskId)
        {
            using (CarePlanTaskRepository rep = this.UnitOfWork.CarePlanTaskRepository)
            {
                CarePlanTask model = rep.Where(x => x.Id == CarePlanTaskId).FirstOrDefault();
                return model;
            }
        }

        /// <summary>
        /// The get care plan task name by id.
        /// </summary>
        /// <param name="taskId">
        /// The task id.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetCarePlanTaskNameById(int taskId)
        {
            using (CarePlanTaskRepository cBal = this.UnitOfWork.CarePlanTaskRepository)
            {
                CarePlanTask list = cBal.Where(x => x.IsActive && x.Id == taskId).FirstOrDefault();

                return list != null && list.TaskName != null ? list.TaskName : string.Empty;
            }
        }

        /// <summary>
        /// Gets the care plan task number by identifier.
        /// </summary>
        /// <param name="taskId">
        /// The task identifier.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetCarePlanTaskNumberById(int taskId)
        {
            using (CarePlanTaskRepository cBal = this.UnitOfWork.CarePlanTaskRepository)
            {
                CarePlanTask list = cBal.Where(x => x.IsActive && x.Id == taskId).FirstOrDefault();
                return list != null && list.TaskNumber != null ? list.TaskNumber : string.Empty;
            }
        }

        /// <summary>
        /// The get max task number.
        /// </summary>
        /// <param name="corporateId">
        /// The corporate id.
        /// </param>
        /// <param name="facilityId">
        /// The facility id.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetMaxTaskNumber(int corporateId, int facilityId)
        {
            using (CarePlanTaskRepository rep = this.UnitOfWork.CarePlanTaskRepository)
            {
                var queryable = rep.Where(x => x.FacilityId == facilityId && x.CorporateId == corporateId);
                var maxValue = queryable.Any() ? queryable.Max(a => Convert.ToInt32(a.TaskNumber) + 1) : 1;
                return maxValue;
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
        public int SaveCarePlanTask(CarePlanTask model, int val)
        {
            using (CarePlanTaskRepository rep = this.UnitOfWork.CarePlanTaskRepository)
            {
                if (model.Id > 0)
                {
                    var obj = new PatientCarePlanBal().GetPatientCarePlanByTaskId(Convert.ToString(model.Id));
                    var carePlanId =
                        obj != null ? Convert.ToInt32(obj.CarePlanId) : 0;
                    if (carePlanId > 0 && val != 2)//val == 2 means it is saving/updating
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

        /// <summary>
        /// The save care plan task custom model.
        /// </summary>
        /// <param name="vm">
        /// The vm.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int SaveCarePlanTaskCustomModel(CarePlanTaskCustomModel vm)
        {
            using (CarePlanTaskRepository rep = this.UnitOfWork.CarePlanTaskRepository)
            {
                CarePlanTask model = this.CarePlanTaskMapper.MapViewModelToModel(vm);
                if (model.Id > 0)
                {
                    rep.UpdateEntity(model, model.Id);
                }
                else
                {
                    rep.Create(model);
                }

                return model.Id;
            }
        }
        /// <summary>
        /// Delete the care plan task when IsActive is 0
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int DeleteCarePlanTask(CarePlanTask model)
        {
            using (var rep = UnitOfWork.CarePlanTaskRepository)
            {
                if (model.Id > 0)
                {
                    rep.Delete(model.Id);
                }
                return model.Id;
            }
        }

        /// <summary>
        /// Get the care plan task by passing care plan Id
        /// </summary>
        /// <param name="carePlanId"></param>
        /// <returns></returns>
        public CarePlanTask GetCarePlanTaskByCarePlanId(Int32 carePlanId)
        {
            using (CarePlanTaskRepository rep = this.UnitOfWork.CarePlanTaskRepository)
            {
                CarePlanTask selectedObj =
                    rep.Where(x => x.CarePlanId == carePlanId && x.IsActive == true)
                        .FirstOrDefault();
                return selectedObj;
            }
        }
        #endregion
    }
}