using System;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Mapper
{
    public class MedicalNecessityMapper : Mapper<MedicalNecessity, MedicalNecessityCustomModel>
    {
        public override MedicalNecessityCustomModel MapModelToViewModel(MedicalNecessity model)
        {
            var vm = base.MapModelToViewModel(model);
            if (vm != null)
            {
               }
            return vm;
        }
    }
}
