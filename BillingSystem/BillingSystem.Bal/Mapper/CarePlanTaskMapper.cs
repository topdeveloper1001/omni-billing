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
    /// The care plan task mapper.
    /// </summary>
    public class CarePlanTaskMapper : Mapper<CarePlanTask, CarePlanTaskCustomModel>
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
        public override CarePlanTaskCustomModel MapModelToViewModel(CarePlanTask model)
        {
            CarePlanTaskCustomModel vm = base.MapModelToViewModel(model);
            var cBal = new CarePlanTaskBal();
            var rBal = new RoleBal();
            var gBal = new GlobalCodeBal();
            vm.Activity = gBal.GetNameByGlobalCodeValueAndCategoryValue("1201", Convert.ToString(model.ActivityType));
            vm.Occurrence = gBal.GetNameByGlobalCodeValueAndCategoryValue(
                "4906",
                Convert.ToString(model.RecurranceType));
            vm.RecurranceTimeIntervalType = gBal.GetNameByGlobalCodeValueAndCategoryValue(
                "4907",
                Convert.ToString(model.RecTImeIntervalType));
            vm.ResponsibleUser = rBal.GetRoleNameById(Convert.ToInt32(model.ResponsibleUserType));
            vm.CarePlan = (model.CarePlanId != null) && (model.CarePlanId != 9999)
                              ? cBal.GetCarePlanNameById(Convert.ToInt32(model.CarePlanId))
                              : "Single Task";
            return vm;
        }

        #endregion
    }
}