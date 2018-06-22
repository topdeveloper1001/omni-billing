using System;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Mapper
{
    public class DrugIntractionsMapper : Mapper<DrugInteractions, DrugInteractionsCustomModel>
    {
        /// <summary>
        /// Maps the model to view model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public override DrugInteractionsCustomModel MapModelToViewModel(DrugInteractions model)
        {
            var vm = base.MapModelToViewModel(model);
            using (var bal = new BaseBal())
            {
                vm.ReactionCategoryStr = bal.GetNameByGlobalCodeValue(model.ReactionCategory,
                               Convert.ToString((int)GlobalCodeCategoryValue.ReactionCategory));
                vm.WarningStr =vm.Warning==null?String.Empty : vm.Warning.Length > 200 ? vm.Warning.Substring(0, 200) + "...." : vm.Warning;
                vm.OrderTypeName = bal.GetNameByGlobalCodeValue(model.OrderCodeType,
                    Convert.ToString((int)GlobalCodeCategoryValue.CodeTypes));
            }
            return vm;
        }
    }
}
