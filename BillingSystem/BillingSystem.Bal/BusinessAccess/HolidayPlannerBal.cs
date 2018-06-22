// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HolidayPlannerBal.cs" company="Spadez">
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

    using Mapper;
    using Model;
    using Model.CustomModel;
    using System.Globalization; 

    /// <summary>
    /// The holiday planner bal.
    /// </summary>
    public class HolidayPlannerBal : BaseBal
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HolidayPlannerBal"/> class.
        /// </summary>
        public HolidayPlannerBal()
        {
            HolidayPlannerMapper = new HolidayPlannerMapper();
            HolidayPlannerDetailsMapper = new HolidayPlannerDetailsMapper();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the holiday planner mapper.
        /// </summary>
        private HolidayPlannerMapper HolidayPlannerMapper { get; set; }
        private HolidayPlannerDetailsMapper HolidayPlannerDetailsMapper { get; set; }

        #endregion

        /// <summary>
        ///     Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<HolidayPlannerCustomModel> GetHolidayPlanner(int corporateId)
        {
            var list = new List<HolidayPlannerCustomModel>();
            using (var holidayPlannerRep = UnitOfWork.HolidayPlannerRepository)
            {
                //var lstHolidayPlanner = holidayPlannerRep.GetAll().ToList();
                var lstHolidayPlanner = holidayPlannerRep.Where(x => x.IsActive && x.CorporateId == corporateId).ToList();
                if (lstHolidayPlanner.Any())
                {
                    //list.AddRange(lstHolidayPlanner.Select(item => new HolidayPlannerCustomModel()));
                    list.AddRange(lstHolidayPlanner.Select(item => HolidayPlannerMapper.MapModelToViewModel(item)));
                }
            }

            return list;
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
            using (var rep = UnitOfWork.HolidayPlannerRepository)
            {
                var model = rep.Where(x => x.HolidayPlannerId == HolidayPlannerId).FirstOrDefault();
                return model;
            }
        }

        public HolidayPlannerCustomModel SaveHolidayPlanner(HolidayPlannerCustomModel vm)
        {
            HolidayPlannerCustomModel obj = null;
            int? status = -1;
            using (var rep = UnitOfWork.HolidayPlannerRepository)
            {
                var model = HolidayPlannerMapper.MapViewModelToModel(vm);

                //Get the existing holiday planner ID
                model.HolidayPlannerId = GetExistingPlannerId(Convert.ToInt32(model.FacilityId), Convert.ToInt32(model.CorporateId),
                      Convert.ToInt32(model.Year), model.ItemtypeId, model.ItemId);

                status = model.HolidayPlannerId > 0 ? rep.UpdateEntity(model, model.HolidayPlannerId) : rep.Create(model);
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
                       
                        var list = timeSlots.Select(item => HolidayPlannerDetailsMapper.MapViewModelToModel(item)).ToList();
                      using (var hRep = UnitOfWork.HolidayPlannerDetailsRepository)
                        {
                            if (list.All(x => x.Id > 0))
                            {
                                status = hRep.Update(vm.TimeSlots);
                            }
                            else
                            {
                                status = hRep.Create(list);
                            }
                        

                            if (status >= 0)
                            {
                                obj = GetHolidayPlannerByCurrentSelection(Convert.ToInt32(model.FacilityId),
                                    Convert.ToInt32(model.CorporateId),
                                    Convert.ToInt32(model.Year), model.ItemtypeId, model.ItemId);
                            }
                        }
                    }
                }
                return obj;
            }
        }

        private int GetExistingPlannerId(int facilityId, int corporateId, int year, string itemTypeId, string itemId)
        {
            using (var rep = UnitOfWork.HolidayPlannerRepository)
            {
                var model =
                    rep.Where(
                        m =>
                            m.IsActive && m.CorporateId == corporateId && m.FacilityId == facilityId &&
                            m.ItemId == itemId && m.ItemtypeId == itemTypeId && m.Year == year).FirstOrDefault();
                return model != null ? model.HolidayPlannerId : 0;
            }
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
            using (var rep = UnitOfWork.HolidayPlannerRepository)
            {
                var model = rep.Where(
                    p =>
                        p.Year == year && p.FacilityId == facilityId && p.CorporateId == corporateId &&
                        p.ItemId.Trim().Equals(itemId) && p.ItemtypeId.Trim().Equals(itemTypeId) &&
                        p.IsActive).FirstOrDefault();

                if (model == null) return holidayPlanner;
                holidayPlanner = HolidayPlannerMapper.MapModelToViewModel(model);
                using (var dRep = UnitOfWork.HolidayPlannerDetailsRepository)
                {
                    var list = dRep.Where(x => x.HolidayPlannerId == model.HolidayPlannerId).ToList();
                    if (list.Count > 0)
                    {
                        holidayPlanner.TimeSlots = new List<HolidayPlannerDetailsCustomModel>();
                        holidayPlanner.TimeSlots.AddRange(
                            list.Select(item => HolidayPlannerDetailsMapper.MapModelToViewModel(item)));
                    }
                }
            }
            return holidayPlanner;
        }
    }
}