using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class BillTransmissionReportCustomModel
    {
        public Int64 FileID { get; set; }
        public DateTime TransactionDate  { get; set; }
        public string PersonFirstName { get; set; }
        public string PersonLastName { get; set; }
        public string PersonEmiratesIDNumber { get; set; }
        public string EncounterNumber { get; set; }
        public string InsuranceCompanyName { get; set; }
        public string BillNumber { get; set; }
        public decimal Gross{ get; set; }
        public decimal PatientShare { get; set; }
        public decimal Net { get; set; }
        public Int64 EncounterID { get; set; }
        public string PatientID { get; set; }
        public string PayorID { get; set; }
        public Int64 ClaimID { get; set; }
    }
}
