using System.Collections.Generic;
using BillingSystem.Model;

namespace BillingSystem.Bal.Interfaces
{
    public interface IInsuranceCompanyService
    {
        string GetCompanyNameById(int? insuranceCompanyId);
        List<InsuranceCompany> GetInsuranceCompanies(bool showInActive, int facilityId = 0, int cId = 0);
        InsuranceCompany GetInsuranceCompanyById(int? insuranceCompanyId);
        InsuranceCompany GetInsuranceDetailsByPayorId(string payorId);
        string GetPayerId(int insuranceCompanyId);
        string GetPayerId(int insuranceCompanyId, int patientId);
        int SaveInsuranceCompany(InsuranceCompany m);
        int ValidateInsuranceCompanyNameInsuranceCompanyLicenseNumber(string insuranceCompanyName, string licenseNumber, int id);
    }
}