using System;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Mapper
{
    public class EncounterMapper : Mapper<Encounter, EncounterCustomModel>
    {
        public override EncounterCustomModel MapModelToViewModel(Encounter model)
        {
            var vm = base.MapModelToViewModel(model);
            if (vm != null)
            {
                var pi = model.PatientInfo;
                vm.FirstName = pi.PersonFirstName;
                vm.LastName = pi.PersonLastName;
                vm.BirthDate = pi.PersonBirthDate;
                vm.PersonEmiratesIDNumber = pi.PersonEmiratesIDNumber;
                vm.PatientIsVIP = !string.IsNullOrEmpty(pi.PersonVIP) ? pi.PersonVIP : string.Empty;


                if (model.EncounterPatientType == (int)EncounterPatientType.InPatient)
                {

                }
            }
            return vm;
        }
    }
}
