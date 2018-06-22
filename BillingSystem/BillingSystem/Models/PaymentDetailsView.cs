using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Models
{
    public class PaymentDetailsView
    {
        public BillHeader BillHeaderDetails { get; set; }
        public List<Payment> PaymentDetails { get; set; }
        public IEnumerable<PatientInfoXReturnPaymentCustomModel> PatientSearchList { get; set; }
        public PatientInfo PatientSearch { get; set; }
        
    }
}