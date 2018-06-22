// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HolidayPlannerDetailsBal.cs" company="SPadez">
//   OmniHealthcare
// </copyright>
// <summary>
//   The holiday planner details bal.
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
    using Repository.GenericRepository;

    /// <summary>
    /// The holiday planner details bal.
    /// </summary>
    public class HolidayPlannerDetailsBal : BaseBal
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HolidayPlannerDetailsBal"/> class. 
        ///     Initializes a new instance of the <see cref="HolidayPlannerBal"/> class.
        /// </summary>
        public HolidayPlannerDetailsBal()
        {
            HolidayPlannerDetailsMapper = new HolidayPlannerDetailsMapper();
        }

        #endregion

        private HolidayPlannerDetailsMapper HolidayPlannerDetailsMapper { get; set; }

        #region Properties

        /// <summary>
        ///     Gets or sets the holiday planner mapper.
        /// </summary>
     

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<HolidayPlannerDetailsCustomModel> GetHolidayPlannerDetails()
        {
            var list = new List<HolidayPlannerDetailsCustomModel>();
            using (
                var holidayPlannerDetailsRep =
                    UnitOfWork.HolidayPlannerDetailsRepository)
            {
                List<HolidayPlannerDetails> lstHolidayPlannerDetails = holidayPlannerDetailsRep.GetAll().ToList();
                if (lstHolidayPlannerDetails.Count > 0)
                {
                    list.AddRange(lstHolidayPlannerDetails.Select(item => new HolidayPlannerDetailsCustomModel()));
                }
            }

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
            using (HolidayPlannerDetailsRepository rep = UnitOfWork.HolidayPlannerDetailsRepository)
            {
                HolidayPlannerDetails model = rep.Where(x => x.Id == HolidayPlannerDetailsId).FirstOrDefault();
                return model;
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
        public int SaveHolidayPlannerDetails(HolidayPlannerDetails model)
        {
            using (HolidayPlannerDetailsRepository rep = UnitOfWork.HolidayPlannerDetailsRepository)
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



        public int GetExistingHolidayPlannerDetailsId(int holidayId, string eventId)
        {
            using (var rep = UnitOfWork.HolidayPlannerDetailsRepository)
            {
                var model =
                    rep.Where(x => x.HolidayPlannerId == holidayId && x.EventId.ToLower().Trim().Equals(eventId)).FirstOrDefault();
                return model!=null? Convert.ToInt32(model.Id):0;
            }
        }


        public bool DeleteHolidayEvent(int id)
        {
            try
            {
                using (var rep = UnitOfWork.HolidayPlannerDetailsRepository)
                {
                    var valueToDelete = rep.Where(x => x.Id==id);
                    var deletedrecord = rep.Delete(valueToDelete);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion
    }
}