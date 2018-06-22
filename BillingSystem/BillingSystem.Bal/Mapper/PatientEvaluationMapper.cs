using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Mapper
{
    public class PatientEvaluationMapper : Mapper<PatientEvaluation, PatientEvaluationCustomModel>
    {
        public override PatientEvaluationCustomModel MapModelToViewModel(PatientEvaluation model)
        {
            var vm = base.MapModelToViewModel(model);
            if (vm != null)
            {
                using (var bal = new PatientEvaluationBal())
                {
                    vm.GlobalCodeName = bal.GetNameByGlobalCodeValue(model.CodeValue, model.CategoryValue);
                    vm.SubSection = !string.IsNullOrEmpty(vm.ParentCodeValue)
                        ? bal.GetNameByGlobalCodeValue(vm.ParentCodeValue, model.CategoryValue)
                        : string.Empty;

                    vm.GlobalCodeCategoryName = bal.GetGlobalCategoryNameById(model.CategoryValue);
                    vm.Value = model.Value.Equals("1") ? "Yes" : model.Value;
                    vm.EnteredBy = bal.GetUserNameByUserId(vm.CreatedBy);
                }
            }
            return vm;
        }
    }
}
