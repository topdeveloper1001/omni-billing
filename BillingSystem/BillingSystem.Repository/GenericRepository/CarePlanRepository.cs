// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CarePlanRepository.cs" company="SPadez">
//   Omnihealthcare
// </copyright>
// <summary>
//   The care plan repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Repository.GenericRepository
{
    using BillingSystem.Model;

    /// <summary>
    /// The care plan repository.
    /// </summary>
    public class CarePlanRepository : GenericRepository<Careplan>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CarePlanRepository"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public CarePlanRepository(BillingEntities context)
            : base(context)
        {
            this.AutoSave = true;
        }

        #endregion
    }
}