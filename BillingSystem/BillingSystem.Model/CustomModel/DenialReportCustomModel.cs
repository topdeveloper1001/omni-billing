using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class DenialReportCustomModel
    {
        public DateTime PaymentDate { get; set; }
        public Int32 DenialCount { get; set; }
        public string PayXADenialCode { get; set; }
        public Int32 PayBy { get; set; }
        public string InsuranceCompanyName { get; set; }
    }
}
