using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class JournalEntrySupportReportCustomModel
    {
        public DateTime? ActivityDate { get; set; }
        public string ActivityType { get; set; }
        public string ActivityCode { get; set; }
        public string ActivityDescription { get; set; }
        public string EncounterNumber { get; set; }
        public decimal Gross { get; set; }
        public string DebitAccount { get; set; }
        public string CreditAccount { get; set; }
    }
}
