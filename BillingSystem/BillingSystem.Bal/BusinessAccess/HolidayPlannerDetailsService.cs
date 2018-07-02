
using BillingSystem.Bal.Interfaces;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

using System;
using System.Collections.Generic;
using System.Linq;

namespace BillingSystem.Bal.BusinessAccess
{
    public class HolidayPlannerDetailsService : IHolidayPlannerDetailsService
    {
        private readonly IRepository<HolidayPlannerDetails> _repository;

        public HolidayPlannerDetailsService(IRepository<HolidayPlannerDetails> repository)
        {
            _repository = repository;
        }

        #region Public Methods and Operators

        /// <summary>
        ///     Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<HolidayPlannerDetailsCustomModel> GetHolidayPlannerDetails()
        {
            var list = new List<HolidayPlannerDetailsCustomModel>();

            var lst = _repository.GetAll().ToList();
            if (lst.Count > 0)
                list.AddRange(lst.Select(item => new HolidayPlannerDetailsCustomModel()));

            return list;
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="HolidayPlannerDetailsId">The Holiday Planner Details Id.</param>
        /// <returns>
        /// The <see cref="HolidayPlannerDetails" />.
        /// </returns>
        public HolidayPlannerDetails GetHolidayPlannerDetailsByID(int? HolidayPlannerDetailsId)
        {
            HolidayPlannerDetails model = _repository.Where(x => x.Id == HolidayPlannerDetailsId).FirstOrDefault();
            return model;
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
        public int SaveHolidayPlannerDetails(HolidayPlannerDetails model)
        {
            if (model.Id > 0)
                _repository.UpdateEntity(model, model.Id);
            else
                _repository.Create(model);

            return model.Id;
        }



        public int GetExistingHolidayPlannerDetailsId(int holidayId, string eventId)
        {
            var model = _repository.Where(x => x.HolidayPlannerId == holidayId && x.EventId.ToLower().Trim().Equals(eventId)).FirstOrDefault();
            return model != null ? Convert.ToInt32(model.Id) : 0;
        }

        public bool DeleteHolidayEvent(int id)
        {
            try
            {
                var deletedrecord = _repository.Delete(id);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion
    }
}