using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BillingSystem.Model.Model
{
    public class PaymentTypeDetail
    {
        [Key]
        public int Id { get; set; }

        public string PaymentType { get; set; }

        public string CardNumber { get; set; }

        public string ExpiryMonth { get; set; }

        public string ExpiryYear { get; set; }

        public string CardHolderName { get; set; }

        public string ExtValue1 { get; set; }

        public string ExtValue2 { get; set; }

        public string ExtValue3 { get; set; }

        public string ExtValue4 { get; set; }

        public string ExtValue6 { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? PaymentId { get; set; }

    }

}
