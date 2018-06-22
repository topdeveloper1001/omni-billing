
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class ManagedCareCustomModel : ManagedCare
    {
        public string InsuranceCompany { get; set; }
        public string InsurancePlan { get; set; }
        public string InsuarancePolicy { get; set; }
    }
}
