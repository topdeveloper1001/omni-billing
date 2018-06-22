using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model.Model;


namespace BillingSystem.Models
{
    public class PaymentsView
    {
        public List<PaymentCustomModel> PaymentsList { get; set; }
        public PaymentCustomModel CurrentPayment { get; set; }
        public IEnumerable<PatientInfoXReturnPaymentCustomModel> PatientSearchList { get; set; }
        public PatientInfo PatientSearch { get; set; }
        public int PatientId { get; set; }
        public int EncounterId { get; set; }
        public int BillHeaderId { get; set; }
        public PaymentTypeDetail CurrentPaymentTypeDetail{ get; set; }
    }
}