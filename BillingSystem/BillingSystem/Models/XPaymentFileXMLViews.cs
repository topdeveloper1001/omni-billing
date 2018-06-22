using System.Collections.Generic;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Models
{
    public class XPaymentFileXMLView
    {
     
        public XPaymentFileXML CurrentXPaymentFileXML { get; set; }
        public List<XPaymentFileXMLCustomModel> XPaymentFileXMLList { get; set; }

    }
}
