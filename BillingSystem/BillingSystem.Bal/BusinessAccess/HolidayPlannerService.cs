 
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Repository.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class HolidayPlannerService : IHolidayPlannerService
    {
        private readonly IRepository<HolidayPlanner> _repository;
        private readonly IRepository<HolidayPlannerDetails> _hdRepository;

        private readonly IMapper _mapper;

        public HolidayPlannerService(IRepository<HolidayPlanner> repository, IRepository<HolidayPlannerDetails> hdRepository, IMapper mapper)
        {
            _repository = repository;
            _hdRepository = hdRepository;
            _mapper = mapper;
        }

        /// <summary>
        ///     Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<HolidayPlannerCustomModel> GetHolidayPlanner(int corporateId)
        {
            var lst = _repository.Where(x => x.IsActive && x.CorporateId == corporateId).ToList();
            return lst.Select(x => _mapper.Map<HolidayPlannerCustomModel>(x)).ToList();
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="HolidayPlannerId">The Holiday Planner Id.</param>
        /// <returns>
        /// The <see cref="HolidayPlanner" />.
        /// </returns>
        public HolidayPlanner GetHolidayPlannerById(int? HolidayPlannerId)
        {
            var model = _repository.Where(x => x.HolidayPlannerId == HolidayPlannerId).FirstOrDefault();
            return model;

        }

        public HolidayPlannerCustomModel SaveHolidayPlanner(HolidayPlannerCustomModel vm)
        {
            HolidayPlannerCustomModel obj = null;
            int? status = -1;
            var model = _mapper.Map<HolidayPlanner>(vm);

            //Get the existing holiday planner ID
            model.HolidayPlannerId = GetExistingPlannerId(Convert.ToInt32(model.FacilityId), Convert.ToInt32(model.CorporateId),
                  Convert.ToInt32(model.Year), model.ItemtypeId, model.ItemId);

            status = model.HolidayPlannerId > 0 ? _repository.Updatei(model, model.HolidayPlannerId) : _repository.Create(model);
            if (model.HolidayPlannerId > 0)
            {
                if (vm.TimeSlots.Count > 0)
                {
                    var timeSlots = vm.TimeSlots.Select(c =>
                    {
                        c.HolidayPlannerId = model.HolidayPlannerId;
                        c.IsActive = true;
                        c.CreatedBy = model.CreatedBy;
                        c.CreatedDate = model.CreatedDate;
                        c.IsFullDay = true;

                        return c;
                    }).ToList();

                    var list = timeSlots.Select(y => _mapper.Map<HolidayPlannerDetails>(y)).ToList();

                    if (list.All(x => x.Id > 0))
                    {
                        status = _hdRepository.Update(vm.TimeSlots);
                    }
                    else
                    {
                        status = _hdRepository.Create(list);
                    }


                    if (status >= 0)
                    {
                        obj = GetHolidayPlannerByCurrentSelection(Convert.ToInt32(model.FacilityId),
                            Convert.ToInt32(model.CorporateId),
                            Convert.ToInt32(model.Year), model.ItemtypeId, model.ItemId);
                    }

                }
            }
            return obj;
        }

        private int GetExistingPlannerId(int facilityId, int corporateId, int year, string itemTypeId, string itemId)
        {
            var model = _repository.Where(m => m.IsActive && m.CorporateId == corporateId && m.FacilityId == facilityId && m.ItemId == itemId && m.ItemtypeId == itemTypeId && m.Year == year).FirstOrDefault();
            return model != null ? model.HolidayPlannerId : 0;
        }


        /// <summary>
        /// Gets the holiday planner list.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="year">The year.</param>
        /// <param name="itemTypeId">The item type identifier.</param>
        /// <param name="itemId">The item identifier.</param>
        /// <returns></returns>
        public HolidayPlannerCustomModel GetHolidayPlannerByCurrentSelection(int facilityId, int corporateId, int year, string itemTypeId, string itemId)
        {
            var holidayPlanner = new HolidayPlannerCustomModel { TimeSlots = new List<HolidayPlannerDetailsCustomModel>() };
            var model = _repository.Where(
                p =>
                    p.Year == year && p.FacilityId == facilityId && p.CorporateId == corporateId &&
                    p.ItemId.Trim().Equals(itemId) && p.ItemtypeId.Trim().Equals(itemTypeId) &&
                    p.IsActive).FirstOrDefault();

            if (model == null) return holidayPlanner;
            holidayPlanner = _mapper.Map<HolidayPlannerCustomModel>(model);
            var list = _hdRepository.Where(x => x.HolidayPlannerId == model.HolidayPlannerId).ToList();
            if (list.Count > 0)
            {
                holidayPlanner.TimeSlots = new List<HolidayPlannerDetailsCustomModel>();
                holidayPlanner.TimeSlots.AddRange(
                    list.Select(item => _mapper.Map<HolidayPlannerDetailsCustomModel>(item)));
            }
            return holidayPlanner;
        }
    }
}