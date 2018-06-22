using System;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Mapper
{
    public class PatientLoginDetailMapper : Mapper<PatientLoginDetail, PatientLoginDetailCustomModel>
    {
        public override PatientLoginDetailCustomModel MapModelToViewModel(PatientLoginDetail model)
        {
            var vm = base.MapModelToViewModel(model);
            if (vm != null)
            {
                using (var bal = new FacilityBal())
                {
                    var facility = bal.GetFacilityDetailByPatientId(Convert.ToInt32(vm.PatientId));
                    vm.Facility = facility.FacilityName;
                    vm.FacilityId = facility.FacilityId;
                    vm.FacilityNumber = facility.FacilityNumber;
                    vm.PatientName = bal.GetPatientNameById(Convert.ToInt32(vm.PatientId));
                    vm.FirstTimeUser = string.IsNullOrEmpty(model.Password);
                    vm.CorporateId = facility.CorporateID.HasValue ? facility.CorporateID.Value : 0;
                    using (var pinfobal = new PatientInfoBal())
                    {
                        var patientinfoObj = pinfobal.GetPatientInfoById(Convert.ToInt32(vm.PatientId));
                        vm.BirthDate = patientinfoObj.PersonBirthDate;
                        vm.EmriateId = patientinfoObj.PersonEmiratesIDNumber;
                    }
                }
            }
            return vm;
        }
    }
}
