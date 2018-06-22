using System.Collections.Generic;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Models
{
    public class InsurancePolicesView
    {
       // public InsurancePolicesViewModel InsurancePolicesViewModel { get; set; }
        public IEnumerable<InsurancePolicyCustomModel> InsurancePolicesList { get; set; }
        public List<InsurancePlans> InsurancePlansList { get; set; }
        public List<InsuranceCompany> InsuranceCompanyList { get; set; }
        public InsurancePolices CurrentInsurancePolices { get; set; }
    }
}