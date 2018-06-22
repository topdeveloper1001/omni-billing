using System.Collections.Generic;
using BillingSystem.Model.CustomModel;


namespace BillingSystem.Models
{
    public class ActiveEncounter
    {
        public List<EncounterCustomModel> ActiveInPatientEncounterList { get; set; }
        public List<EncounterCustomModel> ActiveOutPatientEncounterList { get; set; }
        public List<EncounterCustomModel> ActiveEmergencyEncounterList { get; set; }
        public List<EncounterCustomModel> UnClosedEncounters { get; set; }
        public List<EncounterCustomModel> ForcedClosedEncounters { get; set; }
        public bool EncounterViewAccessible { get; set; }
        public bool EndEncounterViewAccessible { get; set; }
        public bool EhrViewAccessible { get; set; }
        public bool TransactionsViewAccessible { get; set; }
        public bool AuthorizationViewAccessible { get; set; }
        public bool BillHeaderViewAccessible { get; set; }
        public bool DiagnosisViewAccessible { get; set; }
        public bool PatientInfoViewAccessible { get; set; }

    }
}