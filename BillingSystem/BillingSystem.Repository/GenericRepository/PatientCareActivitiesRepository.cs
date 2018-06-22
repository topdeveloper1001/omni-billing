// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PatientCareActivitiesRepository.cs" company="Spadez">
//   OmniHealthcare
// </copyright>
// <summary>
//   The patient care activities repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Repository.GenericRepository
{
    using System.Data.Entity;

    using BillingSystem.Model;

    /// <summary>
    /// The patient care activities repository.
    /// </summary>
    public class PatientCareActivitiesRepository : GenericRepository<PatientCareActivities>
    {
        #region Fields

        /// <summary>
        /// The _context.
        /// </summary>
        private readonly DbContext _context;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PatientCareActivitiesRepository"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public PatientCareActivitiesRepository(BillingEntities context)
            : base(context)
        {
            this.AutoSave = true;
            this._context = context;
        }

        #endregion
    }
}