using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Mapper
{
    public class AppointmentTypesMapper : Mapper<AppointmentTypes, AppointmentTypesCustomModel>
    {
        public override AppointmentTypesCustomModel MapModelToViewModel(AppointmentTypes model)
        {
            var vm = base.MapModelToViewModel(model);
            if (vm != null)
            {
                vm.CPTRange = model.CptRangeFrom + " - " + model.CptRangeTo;
                var bal = new GlobalCodeBal();
                vm.TimeSlot = model.DefaultTime;    //bal.GetNameByGlobalCodeValue(vm.DefaultTime,Convert.ToString((int)GlobalCodeCategoryValue.AppointmentDefaultTime));
                vm.EquipmentRequired = !string.IsNullOrEmpty(model.ExtValue1) && int.Parse(model.ExtValue1) == 1
                    ? "True"
                    : "False";
            }
            return vm;
        }
    }
}
