using System;
using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IInsurancePolicesService
    {
        IEnumerable<InsurancePolicyCustomModel> AddUpdateInsurancePolices(InsurancePolices m);
        bool CheckInsurancePolicyExist(int planId);
        List<InsurancePlans> GetInsurancePlanByCompanyId(int? insuranceCompanyId);
        string GetInsurancePlanNameById(int? id);
        IEnumerable<InsurancePolices> GetInsurancePolicesByPlanId(int planId, DateTime cuDateTime);
        InsurancePolices GetInsurancePolicyById(int insurancePolicyId);
        List<InsurancePolicyCustomModel> GetInsurancePolicyList();
        IEnumerable<InsurancePolicyCustomModel> GetInsurancePolicyListByFacility(int fId, int cId, int userId);
        int ValidatePolicyNamePolicyNumber(string policyName, string policyNumber, int id);
    }
}