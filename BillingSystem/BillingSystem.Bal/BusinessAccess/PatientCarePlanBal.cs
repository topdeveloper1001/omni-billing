// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PatientCarePlanBal.cs" company="SPadez">
//   Omnihealthcare
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
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using BillingSystem.Bal.Mapper;
    using BillingSystem.Model;
    using BillingSystem.Model.CustomModel;
    using BillingSystem.Repository.GenericRepository;

    /// <summary>
    ///     The holiday planner bal.
    /// </summary>
    public class PatientCarePlanBal : BaseBal
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="PatientCarePlanBal" /> class.
        /// </summary>
        public PatientCarePlanBal()
        {
            this.PatientCarePlanMapper = new PatientCarePlanMapper();
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the holiday planner mapper.
        /// </summary>
        private PatientCarePlanMapper PatientCarePlanMapper { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Adds the update patient care plan.
        /// </summary>
        /// <param name="corporateId">
        /// The corporate identifier.
        /// </param>
        /// <param name="facilityId">
        /// The facility identifier.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<PatientCarePlanCustomModel> AddUpdatePatientCarePlan(int corporateId, int facilityId)
        {
            var list = new List<PatientCarePlanCustomModel>();
            using (PatientCarePlanRepository rep = this.UnitOfWork.PatientCarePlanRepository)
            {
                list = rep.AddUpdatePatientCarePlan(corporateId, facilityId);
            }

            return list;
        }

        /// <summary>
        /// Adds the update patient care plan_1.
        /// </summary>
        /// <param name="patientID">
        /// The patient identifier.
        /// </param>
        /// <param name="encounterID">
        /// The encounter identifier.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<PatientCarePlanCustomModel> AddUpdatePatientCarePlan_1(int patientID, int encounterID)
        {
            var list = new List<PatientCarePlanCustomModel>();
            using (PatientCarePlanRepository rep = this.UnitOfWork.PatientCarePlanRepository)
            {
                list = rep.AddUpdatePatientCarePlan_1(patientID, encounterID);
            }

            return list;
        }

        /// <summary>
        /// Binds the care plan task data.
        /// </summary>
        /// <param name="careId">
        /// The care identifier.
        /// </param>
        /// <param name="patientId">
        /// The patient identifier.
        /// </param>
        /// <param name="encounterId">
        /// The encounter identifier.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<PatientCarePlanCustomModel> BindCarePlanTaskData(string careId, string patientId, int encounterId)
        {
            var list = new List<PatientCarePlanCustomModel>();
            using (PatientCarePlanRepository cBal = this.UnitOfWork.PatientCarePlanRepository)
            {
                List<PatientCarePlan> lstCarePlanTask =
                    cBal.Where(
                        x =>
                        x.CarePlanId.Trim().ToLower().Equals(careId) && x.PatientId.Trim().ToLower().Equals(patientId)
                        && x.EncounterId == encounterId && x.IsActive != false).ToList();
                if (lstCarePlanTask.Count > 0)
                {
                    list.AddRange(lstCarePlanTask.Select(item => this.PatientCarePlanMapper.MapModelToViewModel(item)));
                }

                return list;
            }
        }

        /// <summary>
        /// The check duplicate task name.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="encounterId">
        /// The encounter id.
        /// </param>
        /// <param name="patientId">
        /// The patient id.
        /// </param>
        /// <param name="taskId">
        /// The task id.
        /// </param>
        /// <param name="startDate">
        /// The start date.
        /// </param>
        /// <param name="endDate">
        /// The end date.
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
        public bool CheckDuplicateTaskName(
            int id, 
            int encounterId, 
            string patientId, 
            string taskId, 
            DateTime startDate, 
            DateTime endDate, 
            int facilityId, 
            int corporateId)
        {
            using (PatientCarePlanRepository pRep = this.UnitOfWork.PatientCarePlanRepository)
            {
                PatientCarePlan list =
                    pRep.Where(
                        x =>
                        x.EncounterId == encounterId && x.PatientId.Trim().ToLower().Equals(patientId)
                        && x.TaskId.Trim().ToLower().Equals(taskId) && x.FromDate == startDate && x.TillDate == endDate
                        && x.Id != id && x.FacilityId == facilityId && x.CorporateId == corporateId).FirstOrDefault();
                return list != null ? true : false;
            }
        }

        /// <summary>
        /// Deletes the patient care plan.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        public void DeletePatientCarePlan(PatientCarePlan model)
        {
            using (PatientCarePlanRepository pRep = this.UnitOfWork.PatientCarePlanRepository)
            {
                bool list = pRep.DeletePatientCarePlan(
                    Convert.ToInt32(model.PatientId), 
                    Convert.ToInt32(model.EncounterId), 
                    model.Id, 
                    model.TaskId);
            }
        }

        /// <summary>
        ///     Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<PatientCarePlanCustomModel> GetPatientCarePlan()
        {
            var list = new List<PatientCarePlanCustomModel>();
            using (PatientCarePlanRepository PatientCarePlanRep = this.UnitOfWork.PatientCarePlanRepository)
            {
                List<PatientCarePlan> lstPatientCarePlan = PatientCarePlanRep.GetAll().ToList();
                if (lstPatientCarePlan.Count > 0)
                {
                    list.AddRange(lstPatientCarePlan.Select(item => new PatientCarePlanCustomModel()));
                }
            }

            return list;
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="PatientCarePlanId">
        /// The Patient Care Plan Id.
        /// </param>
        /// <returns>
        /// The <see cref="PatientCarePlan"/>.
        /// </returns>
        public PatientCarePlan GetPatientCarePlanById(int? PatientCarePlanId)
        {
            using (PatientCarePlanRepository rep = this.UnitOfWork.PatientCarePlanRepository)
            {
                PatientCarePlan model = rep.Where(x => x.Id == PatientCarePlanId).FirstOrDefault();
                return model;
            }
        }

        /// <summary>
        /// Gets the patient care plan by patient identifier and encounter identifier.
        /// </summary>
        /// <param name="patientId">
        /// The patient identifier.
        /// </param>
        /// <param name="encounterId">
        /// The encounter identifier.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<PatientCarePlanCustomModel> GetPatientCarePlanByPatientIdAndEncounterId(
            string patientId, 
            int encounterId)
        {
            var list = new List<PatientCarePlanCustomModel>();
            using (PatientCarePlanRepository patientCarePlanRep = this.UnitOfWork.PatientCarePlanRepository)
            {
                List<PatientCarePlan> lstPatientCarePlan =
                    patientCarePlanRep.Where(
                        x => x.PatientId.Trim().Equals(patientId) && x.EncounterId == encounterId && x.IsActive != false)
                        .ToList();
                if (lstPatientCarePlan.Count > 0)
                {
                    lstPatientCarePlan = lstPatientCarePlan.GroupBy(x => x.CarePlanId).Select(x => x.First()).ToList();
                    list.AddRange(
                        lstPatientCarePlan.Select(item => this.PatientCarePlanMapper.MapModelToViewModel(item)));
                }
            }

            return list;
        }

        /// <summary>
        /// Gets the patient care plan by plan identifier task identifier.
        /// </summary>
        /// <param name="planid">
        /// The planid.
        /// </param>
        /// <param name="taskid">
        /// The taskid.
        /// </param>
        /// <param name="patientid">
        /// The patientid.
        /// </param>
        /// <returns>
        /// The <see cref="PatientCarePlan"/>.
        /// </returns>
        public PatientCarePlan GetPatientCarePlanByPlanIdTaskId(string planid, string taskid, string patientid)
        {
            using (PatientCarePlanRepository rep = this.UnitOfWork.PatientCarePlanRepository)
            {
                PatientCarePlan selectedObj =
                    rep.Where(x => x.CarePlanId == planid && x.TaskId == taskid && x.PatientId == patientid)
                        .FirstOrDefault();
                return selectedObj;
            }
        }

        /// <summary>
        /// Gets the patient care plan plan identifier.
        /// </summary>
        /// <param name="planId">
        /// The plan identifier.
        /// </param>
        /// <param name="patientId">
        /// The patient identifier.
        /// </param>
        /// <param name="encounterId">
        /// The encounter identifier.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<PatientCarePlan> GetPatientCarePlanPlanId(string planId, string patientId, int encounterId)
        {
            using (PatientCarePlanRepository rep = this.UnitOfWork.PatientCarePlanRepository)
            {
                List<PatientCarePlan> planlist =
                    rep.Where(
                        x =>
                        x.CarePlanId.Trim().ToLower().Equals(planId) && x.PatientId.Trim().ToLower().Equals(patientId)
                        && x.EncounterId == encounterId && x.IsActive != false).ToList();
                return planlist;
            }
        }

        /// <summary>
        /// Gets the task list.
        /// </summary>
        /// <param name="patientId">
        /// The patient identifier.
        /// </param>
        /// <param name="encounterId">
        /// The encounter identifier.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<PatientCarePlan> GetTaskList(string patientId, int encounterId)
        {
            using (PatientCarePlanRepository patientCarePlanRep = this.UnitOfWork.PatientCarePlanRepository)
            {
                List<PatientCarePlan> lstPatientCarePlan =
                    patientCarePlanRep.Where(
                        x => x.PatientId.Trim().Equals(patientId) && x.EncounterId == encounterId && x.IsActive != false)
                        .ToList();
                return lstPatientCarePlan;
            }
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int SavePatientCarePlan(PatientCarePlan model)
        {
            using (PatientCarePlanRepository rep = this.UnitOfWork.PatientCarePlanRepository)
            {
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
        /// Saves the patient care plan data.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="isDelete">
        /// if set to <c>true</c> [is delete].
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int SavePatientCarePlanData(List<PatientCarePlan> model, bool isDelete)
        {
            using (PatientCarePlanRepository rep = this.UnitOfWork.PatientCarePlanRepository)
            {
                // var model = PatientCarePlanMapper.MapViewModelToModel(vm[0]);
                if (model[0].Id > 0)
                {
                    foreach (PatientCarePlan patientCarePlan in model)
                    {
                        if (patientCarePlan.Id > 0)
                        {
                            PatientCarePlan objToUpdate =
                                this.GetPatientCarePlanByPlanIdTaskId(
                                    patientCarePlan.CarePlanId, 
                                    patientCarePlan.TaskId, 
                                    patientCarePlan.PatientId);
                            if (objToUpdate != null)
                            {
                                rep.UpdateEntity(patientCarePlan, objToUpdate.Id);
                            }
                            else
                            {
                                rep.Create(patientCarePlan);
                            }
                        }
                        else
                        {
                            rep.Create(patientCarePlan);
                        }
                    }
                }
                else
                {
                    rep.Create(model);
                }

                // AddUpdatePatientCarePlan(Convert.ToInt32(model[0].CorporateId), Convert.ToInt32(model[0].FacilityId));
                this.AddUpdatePatientCarePlan_1(
                    Convert.ToInt32(model[0].PatientId), 
                    Convert.ToInt32(model[0].EncounterId));
                return 1;
            }
        }
        /// <summary>
        /// Get the patient care plan data by passing task id
        /// </summary>
        /// <param name="taskid"></param>
        /// <returns></returns>
        public PatientCarePlan GetPatientCarePlanByTaskId(string taskid)
        {
            using (PatientCarePlanRepository rep = this.UnitOfWork.PatientCarePlanRepository)
            {
                PatientCarePlan selectedObj =
                    rep.Where(x => x.TaskId == taskid && x.IsActive == true)
                        .FirstOrDefault();
                return selectedObj;
            }
        }
        #endregion
    }
}