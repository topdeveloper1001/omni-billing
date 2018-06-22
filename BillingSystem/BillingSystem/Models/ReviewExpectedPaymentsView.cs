using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Models
{
    public class ReviewExpectedPaymentsView
    {
        public List<EncounterCustomModel> EncounterList { get; set; }
        public OpenOrder EncounterOrder { get; set; }
        public List<OpenOrderCustomModel> OpenOrdersList { get; set; }

        public IEnumerable<ReviewExpectedPaymentReport> ExpectedPaymentInsNotPaidList { get; set; }
        public IEnumerable<ReviewExpectedPaymentReport> ExpectedPaymentPatientVarList { get; set; }
        public IEnumerable<ReviewExpectedPaymentReport> ExpectedPaymentInsVarianceList { get; set; }
    }
}