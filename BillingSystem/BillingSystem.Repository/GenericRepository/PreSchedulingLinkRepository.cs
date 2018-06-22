// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PreSchedulingLinkRepository.cs" company="Spadez">
//   OmniHealthcare
// </copyright>
// <owner>
// Shashank (Created on : 1st of Feb 2016)
// </owner>
// <summary>
//   The pre scheduling link.
// </summary>
// --------------------------------------------------------------------------------------------------------------------




namespace BillingSystem.Repository.GenericRepository
{
    using BillingSystem.Model;

    /// <summary>
    /// The pre scheduling link repository.
    /// </summary>
    public class PreSchedulingLinkRepository : GenericRepository<PreSchedulingLink>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PreSchedulingLinkRepository"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public PreSchedulingLinkRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
        }
    }
}