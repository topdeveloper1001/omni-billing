// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FacultyTimeslotsRepository.cs" company="Spadez">
//   OmniHealthcare
// </copyright>
// <summary>
//   The faculty timeslots repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Repository.GenericRepository
{
    using BillingSystem.Model;

    /// <summary>
    ///     The faculty timeslots repository.
    /// </summary>
    public class FacultyTimeslotsRepository : GenericRepository<FacultyTimeslots>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FacultyTimeslotsRepository"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public FacultyTimeslotsRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
        }
    }
}