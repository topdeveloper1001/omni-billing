using System.Collections.Generic;
using BillingSystem.Model;

namespace BillingSystem.Models
{
    public class InsuranceCompanyView
    {
        //public List<CountryViewModel> CountryList { get; set; }
        //public InsuranceCompanyViewModel CurrentInsurance { get; set; }
        //public List<InsuranceCompanyViewModel> InsuranceCompaniesList { get; set; }

        public List<Country> CountryList { get; set; }
        public InsuranceCompany CurrentInsurance { get; set; }
        public List<InsuranceCompany> InsuranceCompaniesList { get; set; }
    }
}