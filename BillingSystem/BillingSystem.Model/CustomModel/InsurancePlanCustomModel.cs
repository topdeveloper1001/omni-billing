using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class InsurancePlanCustomModel
    {
        public InsurancePlans InsurancePlan { get; set; }
        public string InsuranceCompanyName { get; set; }
    }
}
