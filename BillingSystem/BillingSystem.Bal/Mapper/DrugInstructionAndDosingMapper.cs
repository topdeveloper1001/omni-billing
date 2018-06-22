using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Mapper
{
    public class DrugInstructionAndDosingMapper : Mapper<DrugInstructionAndDosing, DrugInstructionAndDosingCustomModel>
    {
        /// <summary>
        /// Maps the model to view model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public override DrugInstructionAndDosingCustomModel MapModelToViewModel(DrugInstructionAndDosing model)
        {
            var vm = base.MapModelToViewModel(model);
            vm.AdministrationInstructionsStr =  
                !string.IsNullOrEmpty(vm.AdminInstructions) && vm.AdminInstructions.Length > 200 ? vm.AdminInstructions.Substring(0, 100) + "...." : vm.AdminInstructions;
            vm.RecommendedDosingStr = !string.IsNullOrEmpty(vm.RecommendedDosing) && vm.RecommendedDosing.Length > 200 ? vm.RecommendedDosing.Substring(0, 100) + "..." : vm.RecommendedDosing;
            return vm;
        }
    }
}
