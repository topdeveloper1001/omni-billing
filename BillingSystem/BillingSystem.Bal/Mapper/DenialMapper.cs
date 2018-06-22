using System;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Mapper
{
    public class DenialMapper : Mapper<Denial, DenialCodeCustomModel>
    {
        public override DenialCodeCustomModel MapModelToViewModel(Denial model)
        {
            var vm = base.MapModelToViewModel(model);
            if (vm != null)
            {
                 var bal = new GlobalCodeBal();
                vm.DenialTypeStr = bal.GetNameByGlobalCodeValueAndCategoryValue("5203", model.DenialType);
                vm.DenialStatusStr = bal.GetNameByGlobalCodeValueAndCategoryValue("5202", model.DenialStatus);
            }
            return vm;
        }
    }
}
