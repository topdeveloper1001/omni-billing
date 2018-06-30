using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using AutoMapper;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Repository.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{ 
    public class PatientCarePlanService : IPatientCarePlanService
    {
        private readonly IRepository<PatientCarePlan> _repository;
        private readonly BillingEntities _context;
        private readonly IMapper _mapper;
        private readonly IRepository<Careplan> _cpRepository;
        private readonly IRepository<CarePlanTask> _cptrepository;

        public PatientCarePlanService(IRepository<PatientCarePlan> repository, BillingEntities context, IMapper mapper, IRepository<Careplan> cpRepository, IRepository<CarePlanTask> cptrepository)
        {
            _repository = repository;
            _context = context;
            _mapper = mapper;
            _cpRepository = cpRepository;
            _cptrepository = cptrepository;
        }

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
            string spName = string.Format("EXEC {0} @pFId, @pCId", StoredProcedures.SPROC_CreatePatientCarePlanActivites);
            var sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("pFId", facilityId);
            sqlParameters[1] = new SqlParameter("pCId", corporateId);
            IEnumerable<PatientCarePlanCustomModel> result = _context.Database.SqlQuery<PatientCarePlanCustomModel>(spName, sqlParameters);
            return result.ToList();
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
            string spName = string.Format("EXEC {0} @pPId, @pEId", StoredProcedures.SPROC_CreatePatientCarePlanActivites_V1);
            var sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("pPId", patientID);
            sqlParameters[1] = new SqlParameter("pEId", encounterID);
            IEnumerable<PatientCarePlanCustomModel> result = _context.Database.SqlQuery<PatientCarePlanCustomModel>(spName, sqlParameters);
            return result.ToList();
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
            var lstCarePlanTask =
                _repository.Where(
                    x =>
                    x.CarePlanId.Trim().ToLower().Equals(careId) && x.PatientId.Trim().ToLower().Equals(patientId)
                    && x.EncounterId == encounterId && x.IsActive != false).ToList();
            if (lstCarePlanTask.Count > 0)
            {
                list.AddRange(MapValues(lstCarePlanTask));
            }

            return list;
        }
        private List<PatientCarePlanCustomModel> MapValues(List<PatientCarePlan> m)
        {
            var lst = new List<PatientCarePlanCustomModel>();
            foreach (var model in m)
            {
                var vm = _mapper.Map<PatientCarePlanCustomModel>(model);
                vm.CarePlanName = !string.IsNullOrEmpty((model.CarePlanId))
                                      ? Convert.ToInt32(model.CarePlanId) != 9999
                                            ? GetCarePlanById(Convert.ToInt32(model.CarePlanId)).Name
                                            : "Single Task"
                                      : "NA";
                vm.CarePlanNumber = !string.IsNullOrEmpty((model.CarePlanId))
                                      ? Convert.ToInt32(model.CarePlanId) != 9999
                                            ? GetCarePlanById(Convert.ToInt32(model.CarePlanId)).PlanNumber
                                            : "-"
                                      : "NA";
                vm.CarePlanTaskName = GetCarePlanTaskById(Convert.ToInt32(model.TaskId)).TaskName;
                vm.CarePlanTaskNumber = GetCarePlanTaskById(Convert.ToInt32(model.TaskId)).TaskNumber;
                var patientCareList = GetPatientCarePlanPlanId(
                    Convert.ToString(model.CarePlanId),
                    Convert.ToString(model.PatientId),
                    Convert.ToInt32(model.EncounterId));
                vm.PatientCarePlanList = new List<PatientCarePlanTaskCustomModel>();
                foreach (var item in patientCareList)
                {
                    vm.PatientCarePlanList.Add(MapCustomModelModelToViewModel(item));
                }
                lst.Add(vm);
            }
            return lst;
        }
        public PatientCarePlanTaskCustomModel MapCustomModelModelToViewModel(PatientCarePlan model)
        {
            var vm = _mapper.Map<PatientCarePlanTaskCustomModel>(model);
            vm.CareTaskName = GetCarePlanTaskById(Convert.ToInt32(model.TaskId)).TaskName;
            vm.CareTaskNumber = GetCarePlanTaskById(Convert.ToInt32(model.TaskId)).TaskNumber;
            vm.StartDate = model.FromDate.ToString();
            vm.EndDate = model.TillDate.ToString();
            return vm;
        }
        private CarePlanTask GetCarePlanTaskById(int taskId)
        {
            var list = _cptrepository.Where(x => x.IsActive && x.Id == taskId).FirstOrDefault();
            return list != null ? list : new CarePlanTask();

        }
        private Careplan GetCarePlanById(int id)
        {
            var list = _cpRepository.Where(x => x.IsActive && x.Id == id).FirstOrDefault();
            return list != null ? list : new Careplan();
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
        public bool CheckDuplicateTaskName(int id, int encounterId, string patientId, string taskId, DateTime startDate, DateTime endDate, int facilityId, int corporateId)
        {
            PatientCarePlan list = _repository.Where(x => x.EncounterId == encounterId && x.PatientId.Trim().ToLower().Equals(patientId) && x.TaskId.Trim().ToLower().Equals(taskId) && x.FromDate == startDate && x.TillDate == endDate && x.Id != id && x.FacilityId == facilityId && x.CorporateId == corporateId).FirstOrDefault();
            return list != null ? true : false;
        }

        /// <summary>
        /// Deletes the patient care plan.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        public void DeletePatientCarePlan(PatientCarePlan model)
        {
            var sqlParameters = new SqlParameter[4];
            sqlParameters[0] = new SqlParameter("pPId", model.PatientId);
            sqlParameters[1] = new SqlParameter("pEId", model.EncounterId);
            sqlParameters[2] = new SqlParameter("pId", model.Id);
            sqlParameters[3] = new SqlParameter("pTaskNumber", model.TaskId);
            _repository.ExecuteCommand(StoredProcedures.SPROC_DeletePatientCarePlanActivites.ToString(), sqlParameters);
        }

        /// <summary>
        ///     Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<PatientCarePlanCustomModel> GetPatientCarePlan()
        {
            var list = new List<PatientCarePlanCustomModel>();
            List<PatientCarePlan> lstPatientCarePlan = _repository.GetAll().ToList();
            if (lstPatientCarePlan.Count > 0)
                list.AddRange(lstPatientCarePlan.Select(item => new PatientCarePlanCustomModel()));

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
            PatientCarePlan model = _repository.Where(x => x.Id == PatientCarePlanId).FirstOrDefault();
            return model;
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
            List<PatientCarePlan> lstPatientCarePlan = _repository.Where(x => x.PatientId.Trim().Equals(patientId) && x.EncounterId == encounterId && x.IsActive != false).ToList();
            if (lstPatientCarePlan.Count > 0)
            {
                lstPatientCarePlan = lstPatientCarePlan.GroupBy(x => x.CarePlanId).Select(x => x.First()).ToList();
                list.AddRange(MapValues(lstPatientCarePlan));
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
            PatientCarePlan selectedObj = _repository.Where(x => x.CarePlanId == planid && x.TaskId == taskid && x.PatientId == patientid)
                    .FirstOrDefault();
            return selectedObj;
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
            List<PatientCarePlan> planlist = _repository.Where(
                    x =>
                    x.CarePlanId.Trim().ToLower().Equals(planId) && x.PatientId.Trim().ToLower().Equals(patientId)
                    && x.EncounterId == encounterId && x.IsActive != false).ToList();
            return planlist;
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
            List<PatientCarePlan> lstPatientCarePlan = _repository.Where(
                    x => x.PatientId.Trim().Equals(patientId) && x.EncounterId == encounterId && x.IsActive != false)
                    .ToList();
            return lstPatientCarePlan;
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
            if (model.Id > 0)
                _repository.UpdateEntity(model, model.Id);
            else
                _repository.Create(model);

            return model.Id;
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
            if (model[0].Id > 0)
            {
                foreach (PatientCarePlan patientCarePlan in model)
                {
                    if (patientCarePlan.Id > 0)
                    {
                        PatientCarePlan objToUpdate = GetPatientCarePlanByPlanIdTaskId(patientCarePlan.CarePlanId, patientCarePlan.TaskId, patientCarePlan.PatientId);
                        if (objToUpdate != null)
                        {
                            _repository.UpdateEntity(patientCarePlan, objToUpdate.Id);
                        }
                        else
                        {
                            _repository.Create(patientCarePlan);
                        }
                    }
                    else
                    {
                        _repository.Create(patientCarePlan);
                    }
                }
            }
            else
            {
                _repository.Create(model);
            }

            AddUpdatePatientCarePlan_1(
                    Convert.ToInt32(model[0].PatientId),
                    Convert.ToInt32(model[0].EncounterId));
            return 1;
        }
        /// <summary>
        /// Get the patient care plan data by passing task id
        /// </summary>
        /// <param name="taskid"></param>
        /// <returns></returns>
        public PatientCarePlan GetPatientCarePlanByTaskId(string taskid)
        {
            var m = _repository.Where(x => x.TaskId == taskid && x.IsActive == true)
                    .FirstOrDefault();
            return m;
        }
        #endregion
    }
}