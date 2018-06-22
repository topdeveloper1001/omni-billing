using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Models
{
    public class PatientPortalView
    {
        public int PatientId { get; set; }
        public PatientInfoCustomModel PatientInfo { get; set; }
        public IEnumerable<EncounterCustomModel> EncountersList { get; set; }
        public List<OpenOrderCustomModel> OpenOrdersList { get; set; }
        public List<OpenOrderCustomModel> ClosedOrdersList { get; set; }
        public List<DiagnosisCustomModel> DiagnosisList { get; set; }
        public List<MedicalVitalCustomModel> MedicalVitalList { get; set; }
        public List<AlergyCustomModel> AlergyList { get; set; }
        public List<MedicalNotesCustomModel> PatientSummaryNotes { get; set; }
    }
}