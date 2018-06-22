using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Mapper
{
    using System;

    public class TPXMLParsedDataMapper : Mapper<TPXMLParsedData, TPXMLParsedDataCustomModel>
    {
        /// <summary>
        /// Maps the model to view model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public override TPXMLParsedDataCustomModel MapModelToViewModel(TPXMLParsedData model)
        {
            var vm = base.MapModelToViewModel(model);
            if (vm != null)
            {
                using (var bal = new BaseBal())
                {
                    vm.BillNumber = bal.GetBillNumberByBillHeaderId(Convert.ToInt32(vm.OMBillID));
                    vm.CorporateName = bal.GetCorporateNameFromId(Convert.ToInt32(vm.OMCorporateID));
                    vm.FacilityName = bal.GetFacilityNameByFacilityId(Convert.ToInt32(vm.OMFacilityID));
                    vm.EncounterType = bal.GetNameByGlobalCodeValue(vm.EType, "1107");
                    vm.EncounterStartType = bal.GetNameByGlobalCodeValue(vm.EStartType, "1116");
                    vm.EncounterEndType = bal.GetNameByGlobalCodeValue(vm.EEndType, "1114");
                    vm.InsuranceCompany = !string.IsNullOrEmpty(vm.CPayerID) ? bal.GetInsuranceCompanyNameByPayerId(vm.CPayerID) : vm.CPayerID;
                    vm.PatientName = bal.GetPatientNameById(Convert.ToInt32(vm.OMPatientID));
                }
            }
            return vm;
        }
    }
}
