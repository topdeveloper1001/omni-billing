using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Mapper
{
    public class PlaceOfServiceMapper : Mapper<PlaceOfService, PlaceOfServiceCustomModel>
    {
        public override PlaceOfServiceCustomModel MapModelToViewModel(PlaceOfService model)
        {
            var vm = base.MapModelToViewModel(model);
            return vm;
        }
    }
}
