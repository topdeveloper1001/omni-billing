using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Models
{
    public class BedTransactionView
    {
        //public BedTransaction CurrentBedTransaction { get; set; }
        public List<BedTransactionCustomModel> BedTransactionList { get; set; }

        public BedTransaction CurrentBedTransaction { get; set; }
        public string PatientName { get; set; }
        public string EncounterNumber { get; set; }
        public string EncounterType { get; set; }
        public string FacilityName { get; set; }
        public string TotalCharges { get; set; }
        public PatientInfoCustomModel PatientInfo { get; set; }
        public List<BillDetailCustomModel> EncounterTransactionLst { get; set; }

        public int EncounterId { get; set; }
        public int PatientId { get; set; }

    }
}
