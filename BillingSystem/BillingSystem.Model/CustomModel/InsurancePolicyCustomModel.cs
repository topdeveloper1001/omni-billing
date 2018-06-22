using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class InsurancePolicyCustomModel
    {
        public InsurancePolices InsurancePolices { get; set; }
        public string InsuranceCompanyName { get; set; }
        public string PlanName { get; set; }
        public string PlanNumber { get; set; }
        public string ManagedCareCode { get; set; }
    }
}
