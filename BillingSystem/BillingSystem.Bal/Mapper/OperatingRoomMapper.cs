using System;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Mapper
{
    public class OperatingRoomMapper : Mapper<OperatingRoom, OperatingRoomCustomModel>
    {
        public string CptTableNumber { get; set; }
        public string ServiceCodeTableNumber { get; set; }
        public string DrgTableNumber { get; set; }
        public string DrugTableNumber { get; set; }
        public string HcpcsTableNumber { get; set; }
        public string DiagnosisTableNumber { get; set; }

        public OperatingRoomMapper()
        {
            
        }

        public OperatingRoomMapper(string cptTableNumber, string serviceCodeTableNumber, string drgTableNumber, string drugTableNumber, string hcpcsTableNumber, string diagnosisTableNumber)
        {
            if (!string.IsNullOrEmpty(cptTableNumber))
                CptTableNumber = cptTableNumber;

            if (!string.IsNullOrEmpty(serviceCodeTableNumber))
                ServiceCodeTableNumber = serviceCodeTableNumber;

            if (!string.IsNullOrEmpty(drgTableNumber))
                DrgTableNumber = drgTableNumber;

            if (!string.IsNullOrEmpty(drugTableNumber))
                DrugTableNumber = drugTableNumber;

            if (!string.IsNullOrEmpty(hcpcsTableNumber))
                HcpcsTableNumber = hcpcsTableNumber;

            if (!string.IsNullOrEmpty(diagnosisTableNumber))
                DiagnosisTableNumber = diagnosisTableNumber;
        }

        public override OperatingRoomCustomModel MapModelToViewModel(OperatingRoom model)
        {
            var vm = base.MapModelToViewModel(model);
            using (var bal = new BaseBal(
                CptTableNumber,
                ServiceCodeTableNumber,
                DrgTableNumber,
                DrugTableNumber,
                HcpcsTableNumber,
                DiagnosisTableNumber))
            {
                var type = bal.GetNameByGlobalCodeValue(Convert.ToString(model.OperatingType),
                    Convert.ToString((int)GlobalCodeCategoryValue.OperatingRoomType));
                var statusdesc = bal.GetNameByGlobalCodeValue(Convert.ToString(model.Status),
                    Convert.ToString((int)GlobalCodeCategoryValue.OrderStatus));
                var encounterNumber = bal.GetEncounterNumberById(Convert.ToInt32(model.EncounterId));
                vm.OperatingTypeText = type;
                vm.EncounterNumber = encounterNumber;
                vm.Patient = bal.GetPatientNameById(Convert.ToInt32(model.PatientId));
                vm.StatusDescription = statusdesc;
                var codeType =
                    (OperatingRoomTypes)Enum.Parse(typeof(OperatingRoomTypes), Convert.ToString(model.OperatingType));
                switch (codeType)
                {
                    case OperatingRoomTypes.Surgery:
                        vm.CodeDescription = bal.GetCodeDescription(model.CodeValue,
                            Convert.ToString((int)OrderType.BedCharges));
                        break;
                    case OperatingRoomTypes.Anesthesia:
                        vm.CodeDescription = bal.GetCodeDescription(model.CodeValue,
                            Convert.ToString((int)OrderType.CPT));
                        break;
                }
            }
            return vm;
        }
    }
}
