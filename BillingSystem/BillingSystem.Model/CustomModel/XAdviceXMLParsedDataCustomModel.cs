using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class XAdviceXMLParsedDataCustomModel : XAdviceXMLParsedData
    {
        public string OrderingClinician { get; set; }
        public string Clinician { get; set; }
        public string FacilityLicType { get; set; }
        public string SenderIdStr { get; set; }

        public string ReceiverId { get; set; }
        public string BillNumber { get; set; }
        public string PatientName { get; set; }
        public string EncounterNumber { get; set; }
        public int? IsFacilityEncounter { get; set; }
    }
}
