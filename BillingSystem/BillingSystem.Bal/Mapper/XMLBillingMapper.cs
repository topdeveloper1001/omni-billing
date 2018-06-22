using System;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Mapper
{
    public class XMLBillingMapper : Mapper<XFileHeader, XFileHeaderCustomModel>
    {
        public override XFileHeaderCustomModel MapModelToViewModel(XFileHeader model)
        {
            var vm = base.MapModelToViewModel(model);
            if (vm != null)
            {
                using (var basebal = new BaseBal())
                {
                    vm.StatusBit = string.IsNullOrEmpty(vm.Status) && Convert.ToBoolean(vm.Status);
                    vm.StatusStr = string.IsNullOrEmpty(vm.Status) ? Convert.ToString(vm.Status) : string.Empty;
                }
            }
            return vm;
        }
    }
}
