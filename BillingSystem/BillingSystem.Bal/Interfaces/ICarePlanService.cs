using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface ICarePlanService
    {
        bool CheckDuplicateCarePlanNumber(int id, string planNumber, int facilityId, int corporateId);
        int DeleteCarePlan(Careplan model);
        List<CareplanCustomModel> GetActiveCarePlan(int corporateId, int facilityId, bool val);
        List<CareplanCustomModel> GetCarePlan();
        Careplan GetCarePlanById(int? carePlanId);
        int GetTaskNumber(int facilityId, int corporateId);
        int SaveCarePlan(Careplan model, int val);
    }
}