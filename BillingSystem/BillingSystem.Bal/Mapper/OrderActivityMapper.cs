// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrderActivityMapper.cs" company="Spadez">
//   Omnihealthcare
// </copyright>
// <summary>
//   The order activity mapper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Bal.Mapper
{
    using System;
    using BillingSystem.Bal.BusinessAccess;
    using BillingSystem.Common.Common;
    using BillingSystem.Model;
    using BillingSystem.Model.CustomModel;

    /// <summary>
    /// The order activity mapper.
    /// </summary>
    public class OrderActivityMapper : Mapper<OrderActivity, OrderActivityCustomModel>
    {
        /// <summary>
        /// Maps the custom model to view model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public OrderActivityCustomModel MapCustomModelToViewModel(OrderActivityCustomModel model)
        {
            var vm = MapModelToViewModel(model);
            if (vm != null)
            {
                using (var bal = new OrderActivityBal())
                {
                    vm.ShowEditAction =
                        DateTime.Compare(
                            bal.GetInvariantCultureDateTime(Convert.ToInt32(model.FacilityID)),
                            Convert.ToDateTime(model.OrderScheduleDate)) > 0;
                    vm.ResultUOMStr = model.ResultUOM != null
                                          ? bal.GetNameByGlobalCodeValue(
                                              Convert.ToString(model.ResultUOM),
                                              Convert.ToInt32(GlobalCodeCategoryValue.LabMeasurementValue).ToString())
                                          : "";
                    vm.LabResultTypeStr = (model.ResultValueMin != null
                                           && Convert.ToInt32(model.OrderCategoryID)
                                           == Convert.ToInt32(OrderTypeCategory.PathologyandLaboratory))
                                              ? bal.CalculateLabResultType(
                                                  model.OrderCode,
                                                  model.ResultValueMin,
                                                  model.PatientID)
                                              : "";
                    vm.SpecimenTypeStr = bal.CalculateLabResultSpecimanType(
                        model.OrderCode,model.ResultValueMin,model.PatientID);
                    vm.ShowSpecimanEditAction = Convert.ToString(model.OrderActivityStatus) == "0"
                                                || Convert.ToString(model.OrderActivityStatus) == "1"
                                                || Convert.ToString(model.OrderActivityStatus) == "20";
                }
            }
            return vm;
        }

        /// <summary>
        /// Maps the custom model to model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public OrderActivity MapCustomModelToModel(OrderActivityCustomModel model)
        {
            var vm = MapModelToViewModel(model);
            return vm;
        }
    }
}