﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeptTimmingMapper.cs" company="">
//   
// </copyright>
// <summary>
//   The dept timming mapper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BillingSystem.Bal.Mapper
{
    using BillingSystem.Bal.BusinessAccess;
    using BillingSystem.Model;
    using BillingSystem.Model.CustomModel;

    /// <summary>
    /// The dept timming mapper.
    /// </summary>
    public class DeptTimmingMapper : Mapper<DeptTimming, DeptTimmingCustomModel>
    {
        #region Public Methods and Operators

        /// <summary>
        /// The map model to view model.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="DeptTimmingCustomModel"/>.
        /// </returns>
        public override DeptTimmingCustomModel MapModelToViewModel(DeptTimming model)
        {
            var vm = base.MapModelToViewModel(model);
            if (vm != null)
            {
                using (var bal = new BaseBal())
                {
                }
            }

            return vm;
        }

        #endregion
    }
}