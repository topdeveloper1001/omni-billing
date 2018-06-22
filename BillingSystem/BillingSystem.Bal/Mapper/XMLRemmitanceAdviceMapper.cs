using System;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Mapper
{
    public class XMLRemmitanceAdviceMapper : Mapper<XAdviceXMLParsedData,XAdviceXMLParsedDataCustomModel>
    {
        public override XAdviceXMLParsedDataCustomModel MapModelToViewModel(XAdviceXMLParsedData model)
        {
            var vm = base.MapModelToViewModel(model);
            if (vm != null)
            {
                using (var bal = new BaseBal())
                {
                    //vm.Clinician = vm.OrderingClinician = bal.GetClinicianLicNumber(model.XAAOrderingClinician);
                    vm.SenderIdStr = bal.GetFacilitySenderIdByFacilityId(Convert.ToInt32(model.FacilityID));
                    vm.FacilityLicType = bal.GetFacilityLicNumberByFacilityId(Convert.ToInt32(model.FacilityID));
                    vm.XACDateSettlement = !string.IsNullOrEmpty(model.XACDateSettlement)
                        ? model.XACDateSettlement.Split('T')[0]
                        : "";
                    vm.XAAStart = !string.IsNullOrEmpty(model.XAAStart)
                        ? model.XAAStart.Split('T')[0]
                        : "";
                }
            }
            return vm;
        }
    }
}
