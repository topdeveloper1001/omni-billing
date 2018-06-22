using System;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;


namespace BillingSystem.Bal.Mapper
{
    public class ClinicianRosterMapper : Mapper<ClinicianRoster, ClinicianRosterCustomModel>
    {
        public override ClinicianRosterCustomModel MapModelToViewModel(ClinicianRoster model)
        {
            var vm = base.MapModelToViewModel(model);
            return vm;
        }
    }
}
