// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScrubReportMapper.cs" company="Spadez ">
//   Omni healthCare
// </copyright>
// <summary>
//   Defines the ScrubReportMapper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Bal.Mapper
{
    using System;

    using BillingSystem.Bal.BusinessAccess;
    using BillingSystem.Model;
    using BillingSystem.Model.CustomModel;

    /// <summary>
    /// The scrub report mapper.
    /// </summary>
    public class ScrubHeaderMapper : Mapper<ScrubHeader, ScrubHeaderCustomModel>
    {
        /// <summary>
        /// The map model to view model.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ScrubReportCustomModel"/>.
        /// </returns>
        public override ScrubHeaderCustomModel MapModelToViewModel(ScrubHeader model)
        {
            var vm = base.MapModelToViewModel(model);
            if (vm != null)
            {
                using (var bal = new BaseBal())
                {
                    vm.AssignedByUser = bal.GetNameByUserId(model.AssignedBy);
                    vm.AssignedToUser = bal.GetNameByUserId(model.AssignedTo);
                    vm.ExecutedByUser = bal.GetNameByUserId(model.ExecutedBy);
                    vm.PatientName = bal.GetPatientNameById(Convert.ToInt32(model.PatientID));
                    vm.BillHeaderStatus = bal.GetBillHeaderStatusIDByBillHeaderId(Convert.ToInt32(model.BillHeaderID));
                    vm.Section = bal.GetSectionValueByBillHeaderId(Convert.ToInt32(model.BillHeaderID));
                    vm.Failed = model.Failed ?? 0;
                    vm.NotApplicable = model.NotApplicable ?? 0;
                    vm.Passed = model.Passed ?? 0;
                    vm.Performed = model.Performed ?? 0;
                }
                
            } 
            return vm;

        }
    }
}