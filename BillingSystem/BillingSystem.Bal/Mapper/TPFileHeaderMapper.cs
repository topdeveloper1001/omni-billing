using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Mapper
{
    public class TPFileHeaderMapper : Mapper<TPFileHeader, TPFileHeaderCustomModel>
    {
        /// <summary>
        /// Maps the model to view model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public override TPFileHeaderCustomModel MapModelToViewModel(TPFileHeader model)
        {
            var vm = base.MapModelToViewModel(model);
            if (vm != null)
            {
                vm.Status = vm.Status == "FL2"
                                ? "Success"
                                : vm.Status == "FL0" ? "Failure" : vm.Status == "FL1" ? "Duplicate File" : string.Empty;
            }
            return vm;
        }
    }
}
