// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PatientPreSchedulingMapper.cs" company="Spadez">
//   Omnihealthcare
// </copyright>
// <summary>
//   The patient pre scheduling mapper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Bal.Mapper
{
    using BillingSystem.Model;
    using BillingSystem.Model.CustomModel;

    /// <summary>
    /// The patient pre scheduling mapper.
    /// </summary>
    public class PatientPreSchedulingMapper : Mapper<PatientPreScheduling, PatientPreSchedulingCustomModel>
    {
        #region Public Methods and Operators

        /// <summary>
        /// The map model to view model.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="PatientPreSchedulingCustomModel"/>.
        /// </returns>
        public override PatientPreSchedulingCustomModel MapModelToViewModel(PatientPreScheduling model)
        {
            var vm = base.MapModelToViewModel(model);

            return vm;
        }

        #endregion
    }
}