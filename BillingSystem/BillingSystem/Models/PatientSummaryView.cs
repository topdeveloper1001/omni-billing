using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Models
{
    public class PatientSummaryView
    {
        public IEnumerable<EncounterCustomModel> EncountersList { get; set; }
        public List<OpenOrderCustomModel> OpenOrdersList { get; set; }
        public List<OpenOrderCustomModel> ClosedOrdersList { get; set; }
        //public OpenOrder EncounterOrder { get; set; }
        public PatientInfoCustomModel PatientInfo { get; set; }
        //public MedicalRecord CurrentMedicalRecord { get; set; }
        public List<MedicalRecord> MedicalRecordList { get; set; }
        //public MedicalNotes nurseCurrentMedicalNotes { get; set; }
        //public List<MedicalNotesCustomModel> nurseMedicalNotesList { get; set; }
        //public MedicalNotes phyCurrentMedicalNotes { get; set; }
        //public List<MedicalNotesCustomModel> phyMedicalNotesList { get; set; }
        //public MedicalVitalCustomModel CurrentMedicalVital { get; set; }
        public List<MedicalVitalCustomModel> MedicalVitalList { get; set; }
        public int CurrentEncounterId { get; set; }
        public int PatientId { get; set; }
        public int DiagnosisId { get; set; }
        //public AlergyView alergyView { get; set; }
        public List<MedicalNotesCustomModel> PatientSummaryNotes { get; set; }
        //public DiagnosisView DiagnosisView { get; set; }
        public List<OpenOrderCustomModel> MostRecentOrders { get; set; }
        public List<OpenOrderCustomModel> FavoriteOrders { get; set; }
        public List<OpenOrderCustomModel> SearchedOrders { get; set; }
        //public MedicalHistoryView MedicalHistoryView { get; set; }
        public List<OpenOrderCustomModel> AllPhysicianOrders { get; set; }
        //public DischargeSummaryView DischargeSummary { get; set; }

        public List<DiagnosisCustomModel> DiagnosisList { get; set; }
        public List<AlergyCustomModel> AlergyList { get; set; }

        public RiskFactorViewModel Riskfactors { get; set; }

        public int ExternalLinkId { get; set; }
    }
    public class Signature
    {
        public string Value { get; set; }
        public string FileName { get; set; }
        public string PatientId { get; set; }
        public string EncounterId { get; set; }
        public string NurseFormId { get; set; }
        public string NurseFormGlobalCodeCategoryValue { get; set; }
        public string NurseFormText { get; set; }
        public string PageNumber { get; set; }

        public string EnmFromId { get; set; }
        public string EnmFormText { get; set; }
        public string EnmFileName { get; set; }    
    }
}
