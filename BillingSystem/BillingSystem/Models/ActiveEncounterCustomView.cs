using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Models
{
    public class ActiveEncounterCustomView
    {
        public List<CMODashboardModel> ActiveInPatientEncounterList { get; set; }
        public List<CMODashboardModel> ActiveOutPatientEncounterList { get; set; }
        public List<CMODashboardModel> ActiveEmergencyEncounterList { get; set; }
        public List<CMODashboardModel> UnClosedEncounters { get; set; }
        public List<CMODashboardModel> ForcedClosedEncounters { get; set; }
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