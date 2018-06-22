// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CarePlanMapper.cs" company="SPadez">
//   OmniHealtchare
// </copyright>
// <summary>
//   The care plan mapper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using BillingSystem.Bal.BusinessAccess;

namespace BillingSystem.Bal.Mapper
{
    using BillingSystem.Model;
    using BillingSystem.Model.CustomModel;

    /// <summary>
    /// The care plan mapper.
    /// </summary>
    public class CarePlanMapper : Mapper<Careplan, CareplanCustomModel>
    {
        #region Public Methods and Operators

        /// <summary>
        /// The map model to view model.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="CareplanCustomModel"/>.
        /// </returns>
        public override CareplanCustomModel MapModelToViewModel(Careplan model)
        {
            CareplanCustomModel vm = base.MapModelToViewModel(model);
            var enclunterType = new GlobalCodeBal();
            vm.EncounterType =enclunterType.GetNameByGlobalCodeValueAndCategoryValue("1107", model.EncounterPatientType);
            vm.PlanLengthType = enclunterType.GetNameByGlobalCodeValueAndCategoryValue("4908", Convert.ToString(model.PlanLengthType));
            return vm;
        }

        #endregion
    }
}