
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class AccountStatementCustomModel
    {
        public string PatientName { get; set; }
        public Int32 PatientID { get; set; }
        public string EmirateID { get; set; }
        public string MedicalRecord { get; set; }
        public string Encounter { get; set; }
        public DateTime BillDate { get; set; }
        public string BillNumber { get; set; }

        public decimal GrossExpected { get; set; }
        public decimal GrossPaid { get; set; }
        public decimal GrossBalance { get; set; }

        public decimal PatientExpected { get; set; }
        public decimal PatientPaid { get; set; }
        public decimal PatientBalance { get; set; }

        public decimal PayerExpected { get; set; }
        public decimal PayerPaid { get; set; }
        public decimal PayerBalance { get; set; }

        public decimal MCDiscount { get; set; }

        public Int32? EncounterID { get; set; }
        public Int32? BillHeaderID { get; set; }
        public string PayerID { get; set; }
    }
}
