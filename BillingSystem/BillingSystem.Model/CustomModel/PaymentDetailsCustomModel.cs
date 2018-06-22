using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class PaymentDetailsCustomModel
    {
        
        public decimal PatientSharePayable { get; set; }
        public decimal InsSharePayable { get; set; }
        public decimal GrossSharePayable { get; set; }

        public decimal PatientSharePaid { get; set; }
        public decimal InsSharePaid { get; set; }
        public decimal GrossSharePaid { get; set; }

        public decimal PatientShareBalance { get; set; }
        public decimal InsShareBalance { get; set; }
        public decimal GrossShareBalance { get; set; }

        public decimal InsPayment { get; set; }
        public decimal InsTotalPaid { get; set; }
        public decimal InsApplied { get; set; }
        public decimal InsUnapplied { get; set; }

        public decimal PatientPayment { get; set; }
        public decimal PatientTotalPaid { get; set; }
        public decimal PatientApplied { get; set; }
        public decimal PatientUnApplied { get; set; }

        public List<PaymentCustomModel> PaymentDetails { get; set; }
        
    }
}
