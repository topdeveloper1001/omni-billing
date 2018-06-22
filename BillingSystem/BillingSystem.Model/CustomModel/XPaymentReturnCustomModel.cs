
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class XPaymentReturnCustomModel : XPaymentReturn
    {
        public string FacilityNumber { get; set; }
        public string ActivityTypeName { get; set; }
    }
}
