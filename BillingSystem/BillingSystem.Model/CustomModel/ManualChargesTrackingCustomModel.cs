
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class ManualChargesTrackingCustomModel : ManualChargesTracking
    {
        public string TrackingTypeStr { get; set; }
        public string TrackingTypeNameValStr { get; set; }
        public string TrackingBillStatusStr { get; set; }
        public string BillNumber { get; set; }
        public string EncounterNumber { get; set; }
        public string PatientName { get; set; }
        public string CorporateName { get; set; }
        public string FacilityName { get; set; }
        public string UpdatedBy { get; set; }
    }
}
