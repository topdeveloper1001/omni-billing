// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PatientPreSchedulingBal.cs" company="Spadez">
//   OmniHealthcare
// </copyright>
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
    /// The holiday planner bal.
    /// </summary>
    public class PatientPreSchedulingBal : BaseBal
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PatientPreSchedulingBal"/> class.
        /// </summary>
        public PatientPreSchedulingBal()
        {
            this.PatientPreSchedulingMapper = new PatientPreSchedulingMapper();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the holiday planner mapper.
        /// </summary>
        private PatientPreSchedulingMapper PatientPreSchedulingMapper { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<PatientPreSchedulingCustomModel> GetPatientPreScheduling()
        {
            var list = new List<PatientPreSchedulingCustomModel>();
            using (var patientPreSchedulingRep = this.UnitOfWork.PatientPreSchedulingRepository)
            {
                var lstPatientPreScheduling = patientPreSchedulingRep.GetAll().ToList();
                if (lstPatientPreScheduling.Count > 0)
                {
                    list.AddRange(lstPatientPreScheduling.Select(item => new PatientPreSchedulingCustomModel()));
                }
            }

            return list;
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="patientPreSchedulingId">The Holiday Planner Id.</param>
        /// <returns>
        /// The <see cref="PatientPreScheduling" />.
        /// </returns>
        public PatientPreScheduling GetPatientPreSchedulingById(int? patientPreSchedulingId)
        {
            using (var rep = this.UnitOfWork.PatientPreSchedulingRepository)
            {
                var model = rep.Where(x => x.PatientPreSchedulingId == patientPreSchedulingId).FirstOrDefault();
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
        public int SavePatientPreScheduling(PatientPreScheduling model)
        {
            using (var rep = this.UnitOfWork.PatientPreSchedulingRepository)
            {
                if (model.PatientPreSchedulingId > 0)
                {
                    rep.UpdateEntity(model, model.PatientPreSchedulingId);
                }
                else
                {
                    rep.Create(model);
                }

                return model.PatientPreSchedulingId;
            }
        }

     
        #endregion
    }
}
