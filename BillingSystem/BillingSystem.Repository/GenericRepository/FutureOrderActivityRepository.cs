// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FutureOrderActivityRepository.cs" company="Spadez">
//   OmnihealthCare
// </copyright>
// <summary>
//   The future order activity repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------




namespace BillingSystem.Repository.GenericRepository
{
    using BillingSystem.Model.Model;
    using BillingSystem.Model;

    /// <summary>
    /// The future order activity repository.
    /// </summary>
    public class FutureOrderActivityRepository : GenericRepository<FutureOrderActivity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FutureOrderActivityRepository"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public FutureOrderActivityRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
        }
    }
}