using System.Collections.Generic;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Models
{
    public class PatientDischargeSummaryView
    {
        public PatientDischargeSummary CurrentPatientDischargeSummary { get; set; }
        public List<DiagnosisCustomModel> DiagnosisList { get; set; }
        public List<OpenOrderCustomModel> MedicationsInHouseList { get; set; }
        public List<OpenOrderCustomModel> ProceduresList { get; set; }
        public List<MedicalNotesCustomModel> ComplicationsList { get; set; }
        public List<MedicalVitalCustomModel> LabTestsList { get; set; }
        public List<OpenOrderCustomModel> DischargeMedicationsList { get; set; }
        public List<DropdownListData> PatientInstructionsList { get; set; }

        public List<DischargeSummaryDetailCustomModel> ActiveMedicalProblemsList { get; set; }
        public List<DischargeSummaryDetailCustomModel> TypeOfFollowupsList { get; set; }
        public List<DischargeSummaryDetailCustomModel> PatientInstructions { get; set; }
    }
}
