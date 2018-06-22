using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Mapper
{
    public class DrugAllergyLogMapper : Mapper<DrugAllergyLog, DrugAllergyLogCustomModel>
    {
        /// <summary>
        /// Maps the model to view model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public override DrugAllergyLogCustomModel MapModelToViewModel(DrugAllergyLog model)
        {
            var vm = base.MapModelToViewModel(model);
            return vm;
        }
    }
}
