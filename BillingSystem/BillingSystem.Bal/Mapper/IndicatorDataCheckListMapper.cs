using System;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Mapper
{
    public class IndicatorDataCheckListMapper : Mapper<IndicatorDataCheckList, IndicatorDataCheckListCustomModel>
    {
        public override IndicatorDataCheckListCustomModel MapModelToViewModel(IndicatorDataCheckList model)
        {
            var vm = base.MapModelToViewModel(model);
            if (vm != null)
            {
                using (var bal = new BaseBal())
                {
                    vm.FacilityName = bal.GetFacilityNameByFacilityId(Convert.ToInt32(model.FacilityId));
                    vm.CusM1 = model.M1 == "1";
                    vm.CusM2 = model.M2 == "1";
                    vm.CusM3 = model.M3 == "1";
                    vm.CusM4 = model.M4 == "1";
                    vm.CusM5 = model.M5 == "1";
                    vm.CusM6 = model.M6 == "1";
                    vm.CusM7 = model.M7 == "1";
                    vm.CusM8 = model.M8 == "1";
                    vm.CusM9 = model.M9 == "1";
                    vm.CusM10 = model.M10 == "1";
                    vm.CusM11 = model.M11 == "1";
                    vm.CusM12 = model.M12 == "1";
                    if (model.ExternalValue2 != null)
                        vm.CusMonth = Convert.ToInt32(model.ExternalValue2 == "" ? "0" : model.ExternalValue2) > 0 ? true : false;
                }
            }
            return vm;
        }
    }
}
