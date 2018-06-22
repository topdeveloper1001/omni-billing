using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;
using BillingSystem.Model.Model;

namespace BillingSystem.Models
{
    public class PaymentTypeDetailView
    {
     
        public PaymentTypeDetail CurrentPaymentTypeDetail { get; set; }
        public List<PaymentTypeDetailCustomModel> PaymentTypeDetailList { get; set; }

    }
}
