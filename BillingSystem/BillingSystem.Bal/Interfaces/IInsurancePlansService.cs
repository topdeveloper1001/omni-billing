using System;
using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IInsurancePlansService
    {
        int AddUpdateInsurancePlans(InsurancePlans m);
        int CheckDuplicateInsurancePlan(string planName, string planNumber, int id, long insCompanyId);
        bool GetInsurancePlanByCompanyId(int id);
        InsurancePlans GetInsurancePlanById(int? insurancePlanId);
        IEnumerable<InsurancePlanCustomModel> GetInsurancePlanList(bool showIsActive, int fId, int cId, int userId);
        string GetInsurancePlanNumberById(string number);
        List<InsurancePlans> GetInsurancePlansByCompanyId(int companyId, DateTime curreDateTime);
        int ValidatePlanNamePlanNumber(string planName, string planNumber, int id);
    }
}