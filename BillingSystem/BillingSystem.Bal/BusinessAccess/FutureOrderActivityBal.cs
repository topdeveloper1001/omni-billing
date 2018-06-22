// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FutureOrderActivityBal.cs" company="Spadez">
//   OmniHealthcare
// </copyright>
// <summary>
//   The holiday planner bal.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Bal.BusinessAccess
{
    using System.Collections.Generic;
    using System.Linq;

    using BillingSystem.Bal.Mapper;
    using BillingSystem.Model;
    using BillingSystem.Model.CustomModel;
    using BillingSystem.Model.Model;
    using BillingSystem.Repository.GenericRepository;

    /// <summary>
    /// The holiday planner bal.
    /// </summary>
    public class FutureOrderActivityBal : BaseBal
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FutureOrderActivityBal"/> class.
        /// </summary>
        public FutureOrderActivityBal()
        {
            this.FutureOrderActivityMapper = new FutureOrderActivityMapper();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the holiday planner mapper.
        /// </summary>
        private FutureOrderActivityMapper FutureOrderActivityMapper { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<FutureOrderActivityCustomModel> GetFutureOrderActivity()
        {
            var list = new List<FutureOrderActivityCustomModel>();
            using (var FutureOrderActivityRep = this.UnitOfWork.FutureOrderActivityRepository)
            {
                var lstFutureOrderActivity = FutureOrderActivityRep.GetAll().ToList();
                if (lstFutureOrderActivity.Count > 0)
                {
                    list.AddRange(lstFutureOrderActivity.Select(item => FutureOrderActivityMapper.MapModelToViewModel(item)));
                }
            }

            return list;
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="FutureOrderActivityId">The Holiday Planner Id.</param>
        /// <returns>
        /// The <see cref="FutureOrderActivity" />.
        /// </returns>
        public FutureOrderActivity GetFutureOrderActivityById(int? FutureOrderActivityId)
        {
            using (FutureOrderActivityRepository rep = this.UnitOfWork.FutureOrderActivityRepository)
            {
                FutureOrderActivity model = rep.Where(x => x.FutureOrderActivityID == FutureOrderActivityId).FirstOrDefault();
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
        public int SaveFutureOrderActivity(FutureOrderActivity model)
        {
            using (FutureOrderActivityRepository rep = this.UnitOfWork.FutureOrderActivityRepository)
            {
                if (model.FutureOrderActivityID > 0)
                {
                    rep.UpdateEntity(model, model.FutureOrderActivityID);
                }
                else
                {
                    rep.Create(model);
                }

                return model.FutureOrderActivityID;
            }
        }

        #endregion
    }
}
