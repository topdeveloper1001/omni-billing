using System;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Mapper
{

    public class DashboardParametersMapper : Mapper<DashboardParameters, DashboardParametersCustomModel>
    {
        public override DashboardParametersCustomModel MapModelToViewModel(DashboardParameters model)
        {
            var vm = base.MapModelToViewModel(model);
            if (vm != null)
            {
                using (var bal = new BaseBal())
                {
                    vm.DashboardTypeStr = bal.GetNameByGlobalCodeValue(Convert.ToString(model.DashboardType),
                    Convert.ToString((int)GlobalCodeCategoryValue.DashboardTypes));
                    vm.IndicatorCategoryStr = bal.GetNameByGlobalCodeValue(Convert.ToString(model.IndicatorCategory),
                    Convert.ToString((int)GlobalCodeCategoryValue.TypeofData));
                    vm.DataFieldStr = bal.GetNameByGlobalCodeValue(Convert.ToString(model.DataField),
                   Convert.ToString((int)GlobalCodeCategoryValue.ExternalDashboardDataFields));
                    vm.ValueTypeStr = bal.GetNameByGlobalCodeValue(Convert.ToString(model.ValueType),
                   Convert.ToString((int)GlobalCodeCategoryValue.DashboardFormatType));
                    vm.ArgumentStr = bal.GetNameByGlobalCodeValue(Convert.ToString(model.Argument),
                   Convert.ToString((int)GlobalCodeCategoryValue.ExternalDashboardArguments));
                    vm.ColorCodeStr = bal.GetNameByGlobalCodeValue(Convert.ToString(model.ColorCode),
                   Convert.ToString((int)GlobalCodeCategoryValue.ExternalDashboardColors));
                    vm.ExternalDashboardTypeStr = bal.GetNameByGlobalCodeValue(Convert.ToString(model.ExternalValue1),
                   Convert.ToString((int)GlobalCodeCategoryValue.ExternalDashboardType));
                }
            }
            return vm;
        }
    }
}
