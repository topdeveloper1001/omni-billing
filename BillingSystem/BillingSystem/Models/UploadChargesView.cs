using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Models
{
    public class UploadChargesView
    {
        public List<EncounterCustomModel> EncounterList { get; set; }
        public OpenOrder EncounterOrder { get; set; }
        public DiagnosisCustomModel CurrentDiagnosis { get; set; }
        public List<OpenOrderCustomModel> OpenOrdersList { get; set; }

        public DiagnosisView DiagnosisViewCustom { get; set; }
        public RoomChargesView RoomChargesViewCustom { get; set; }
        public int PatientId { get; set; }
        public int EncounterId { get; set; }
        public int BillHeaderId { get; set; }
    }
}