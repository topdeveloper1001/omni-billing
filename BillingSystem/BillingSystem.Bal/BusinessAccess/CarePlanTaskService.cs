using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;


namespace BillingSystem.Bal.BusinessAccess
{
    /// <summary>
    ///     The holiday planner bal.
    /// </summary>
    public class CarePlanTaskService : ICarePlanTaskService
    {
        private readonly IRepository<Careplan> _cpRepository;
        private readonly IRepository<CarePlanTask> _repository;
        private readonly IMapper _mapper;
        private readonly IRepository<PatientCarePlan> _pcprepository;

        public CarePlanTaskService(IRepository<Careplan> cpRepository, IRepository<CarePlanTask> repository, IMapper mapper, IRepository<PatientCarePlan> pcprepository)
        {
            _cpRepository = cpRepository;
            _repository = repository;
            _mapper = mapper;
            _pcprepository = pcprepository;
        }



        #region Public Methods and Operators

        private PatientCarePlan GetPatientCarePlanByTaskId(string taskid)
        {
            var m = _pcprepository.Where(x => x.TaskId == taskid && x.IsActive == true)
                    .FirstOrDefault();
            return m;
        }
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
            List<Careplan> list = _cpRepository.Where(x => x.IsActive && x.FacilityId == facilityId && x.CorporateId == corporateId && x.ExtValue1 != "9999").ToList();
            return list;
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
            List<CarePlanTask> lstCarePlanTask = _repository.Where(x => x.CarePlanId == careId && x.IsActive).ToList();
            if (lstCarePlanTask.Count > 0)
                list = lstCarePlanTask.Select(x => _mapper.Map<CarePlanTaskCustomModel>(x)).ToList();
            return list;
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
            int? result = _repository.Where(x => x.FacilityId == facilityId && x.CorporateId == corporateId && x.Id == taskId)
                    .Max(m => m.CarePlanId);
            return result != null ? Convert.ToInt32(result) : 0;

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
            var check = _repository.Where(x => x.Id != id && x.TaskNumber.Trim().ToLower().Equals(taskNumber) && x.FacilityId == facilityId && x.CorporateId == corporateId).FirstOrDefault();
            return check != null ? true : false;

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
            List<CarePlanTask> lstCarePlanTask = _repository.Where(x => x.IsActive == val && x.CorporateId == corporateId && x.FacilityId == facilityId && x.CarePlanId != null && x.CarePlanId != 9999).ToList();
            if (lstCarePlanTask.Count > 0)
                list = lstCarePlanTask.Select(x => _mapper.Map<CarePlanTaskCustomModel>(x)).ToList();
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
            var list = _cpRepository.Where(x => x.IsActive && x.Id == id).FirstOrDefault();
            return list != null && list.Name != null ? list.Name : string.Empty;
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
            var list = _cpRepository.Where(x => x.IsActive && x.Id == id).FirstOrDefault();
            return list != null && list.PlanNumber != null ? list.PlanNumber : string.Empty;
        }

        /// <summary>
        ///     Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<CarePlanTaskCustomModel> GetCarePlanTask()
        {
            var list = new List<CarePlanTaskCustomModel>();
            List<CarePlanTask> lstCarePlanTask = _repository.GetAll().ToList();
            if (lstCarePlanTask.Count > 0)
                list = lstCarePlanTask.Select(x => _mapper.Map<CarePlanTaskCustomModel>(x)).ToList();

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
            var m = _repository.Where(x => x.Id == CarePlanTaskId).FirstOrDefault();
            return m;

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
            var list = _repository.Where(x => x.IsActive && x.Id == taskId).FirstOrDefault();

            return list != null && list.TaskName != null ? list.TaskName : string.Empty;
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
            var list = _repository.Where(x => x.IsActive && x.Id == taskId).FirstOrDefault();
            return list != null && list.TaskNumber != null ? list.TaskNumber : string.Empty;

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
            var q = _repository.Where(x => x.FacilityId == facilityId && x.CorporateId == corporateId);
            var maxValue = q.Any() ? q.Max(a => Convert.ToInt32(a.TaskNumber) + 1) : 1;
            return maxValue;

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
            if (model.Id > 0)
            {
                var obj = GetPatientCarePlanByTaskId(Convert.ToString(model.Id));
                var carePlanId =
                    obj != null ? Convert.ToInt32(obj.CarePlanId) : 0;
                if (carePlanId > 0 && val != 2)//val == 2 means it is saving/updating
                {
                    return -5; //it means passing task id is assign to patient
                }
                else
                {
                    _repository.UpdateEntity(model, model.Id);
                }
            }
            else
            {
                _repository.Create(model);
            }

            return model.Id;

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
            var m = _mapper.Map<CarePlanTask>(vm);
            if (m.Id > 0)
            {
                _repository.UpdateEntity(m, m.Id);
            }
            else
            {
                _repository.Create(m);
            }

            return m.Id;

        }
        /// <summary>
        /// Delete the care plan task when IsActive is 0
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int DeleteCarePlanTask(CarePlanTask model)
        {
            if (model.Id > 0)
                _repository.Delete(model.Id);

            return model.Id;
        }


        /// <summary>
        /// Get the care plan task by passing care plan Id
        /// </summary>
        /// <param name="carePlanId"></param>
        /// <returns></returns>
        public CarePlanTask GetCarePlanTaskByCarePlanId(Int32 carePlanId)
        {
            var selectedObj = _repository.Where(x => x.CarePlanId == carePlanId && x.IsActive == true).FirstOrDefault();
            return selectedObj;
        }

        #endregion
    }
}