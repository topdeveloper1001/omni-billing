using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Models
{
    public class XPaymentReturnView
    {
        public XPaymentReturn CurrentXPaymentReturn { get; set; }
        public XPaymentReturnCustomModel CurrentXPaymentReturnCustomModel { get; set; }
        public List<XPaymentReturnCustomModel> XPaymentReturnList { get; set; }
        public int ClaimId { get; set; }
        public int? EncounterId { get; set; }
        public int? PatientId { get; set; }
    }
}
