using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Models
{
    public class InsurancePlansView
    {
        public IEnumerable<InsurancePlanCustomModel> InsurancePlansList { get; set; }
        public InsurancePlans CurrentInsurancePlans { get; set; }
        public List<InsuranceCompany> InsuranceCompanyList { get; set; }
    }
}