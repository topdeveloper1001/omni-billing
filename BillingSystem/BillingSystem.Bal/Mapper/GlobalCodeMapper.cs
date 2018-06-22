using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Mapper
{
    public class GlobalCodesMapper : Mapper<GlobalCodes, GlobalCodeCustomModel>
    {
        public override GlobalCodeCustomModel MapModelToViewModel(GlobalCodes model)
        {
            var vm = base.MapModelToViewModel(model);
            if (vm != null)
            {
                using (var bal = new BaseBal())
                {
                    vm.GlobalCodeCustomValue = bal.GetGlobalCategoryNameById(model.GlobalCodeCategoryValue);
                }
            }
            return vm;
        }
    }
}
