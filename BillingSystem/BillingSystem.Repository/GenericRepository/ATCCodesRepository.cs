// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ATCCodesRepository.cs" company="SPadez">
//   OmniHelathcare
// </copyright>
// <summary>
//   The atc codes repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Repository.GenericRepository
{
    using BillingSystem.Model;

    /// <summary>
    /// The atc codes repository.
    /// </summary>
    public class ATCCodesRepository : GenericRepository<ATCCodes>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ATCCodesRepository"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public ATCCodesRepository(BillingEntities context)
            : base(context)
        {
            this.AutoSave = true;
        }

        #endregion
    }
}