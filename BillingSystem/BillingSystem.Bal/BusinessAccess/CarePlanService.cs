using System.Collections.Generic;
using System.Linq;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using System;

using AutoMapper;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    /// <summary>
    /// The holiday planner bal.
    /// </summary>
    public class CarePlanService : ICarePlanService
    {

        private readonly IRepository<Careplan> _repository;
        private readonly IRepository<CarePlanTask> _cpRepository;
        private readonly BillingEntities _context;
        private readonly IMapper _mapper;

        public CarePlanService(IRepository<Careplan> repository, IRepository<CarePlanTask> cpRepository, BillingEntities context, IMapper mapper)
        {
            _repository = repository;
            _cpRepository = cpRepository;
            _context = context;
            _mapper = mapper;
        }

        #region Public Methods and Operators

        /// <summary>
        ///     Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<CareplanCustomModel> GetCarePlan()
        {
            var list = new List<CareplanCustomModel>();
            var lstCarePlan = _repository.GetAll().ToList();
            if (lstCarePlan.Count > 0)
            {
                list = lstCarePlan.Select(x => _mapper.Map<CareplanCustomModel>(x)).ToList();
                //list.AddRange(lstCarePlan.Select(item => CarePlanMapper.MapModelToViewModel(item)));
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
            var model = _repository.Where(x => x.Id == carePlanId).FirstOrDefault();
            return model;
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
            if (model.Id > 0)
            {
                var obj = GetCarePlanTaskByCarePlanId(Convert.ToInt32(model.Id));
                var carePlanId =
                    obj != null ? Convert.ToInt32(obj.CarePlanId) : 0;
                if (carePlanId > 0 && val != 2) //val == 2 means it is saving/updating
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

        private CarePlanTask GetCarePlanTaskByCarePlanId(Int32 carePlanId)
        {
            CarePlanTask selectedObj = _cpRepository.Where(x => x.CarePlanId == carePlanId && x.IsActive == true)
                    .FirstOrDefault();
            return selectedObj;
        }
        public List<CareplanCustomModel> GetActiveCarePlan(int corporateId, int facilityId, bool val)
        {
            var list = new List<CareplanCustomModel>();
            var lstCarePlanTask = _repository.Where(x => x.IsActive == val && x.CorporateId == corporateId && x.FacilityId == facilityId).ToList();
            if (lstCarePlanTask.Count > 0)
                list = lstCarePlanTask.Select(x => _mapper.Map<CareplanCustomModel>(x)).ToList();

            //list.AddRange(lstCarePlanTask.Select(item => CarePlanMapper.MapModelToViewModel(item)));

            return list;

        }

        public int GetTaskNumber(int facilityId, int corporateId)
        {
            var result = _repository.Where(x => x.FacilityId == facilityId && x.CorporateId == corporateId).Max(m => m.PlanNumber);
            return result != null ? Convert.ToInt32(result) + 1 : 1;

        }


        public bool CheckDuplicateCarePlanNumber(int id, string planNumber, int facilityId, int corporateId)
        {
            var check = _repository.Where(x => x.Id != id && x.PlanNumber.Trim().ToLower().Equals(planNumber) && x.FacilityId == facilityId && x.CorporateId == corporateId).FirstOrDefault();
            return check != null ? true : false;

        }
        /// <summary>
        /// Delete the care plan when IsActive is 0
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int DeleteCarePlan(Careplan model)
        {
            if (model.Id > 0)
                _repository.Delete(model.Id);
            return model.Id;

        }
        #endregion
    }
}
