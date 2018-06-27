using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface ICarePlanTaskService
    {
        List<Careplan> BindCarePlan(int corporateId, int facilityId);
        List<CarePlanTaskCustomModel> BindCarePlanTask(int careId);
        int CarePlanId(int corporateId, int facilityId, int taskId);
        bool CheckDuplicateTaskNumber(int id, string taskNumber, int facilityId, int corporateId);
        int DeleteCarePlanTask(CarePlanTask model);
        List<CarePlanTaskCustomModel> GetActiveCarePlanTask(int corporateId, int facilityId, bool val);
        string GetCarePlanNameById(int id);
        string GetCarePlanNumberById(int id);
        List<CarePlanTaskCustomModel> GetCarePlanTask();
        CarePlanTask GetCarePlanTaskByCarePlanId(int carePlanId);
        CarePlanTask GetCarePlanTaskById(int? CarePlanTaskId);
        string GetCarePlanTaskNameById(int taskId);
        string GetCarePlanTaskNumberById(int taskId);
        int GetMaxTaskNumber(int corporateId, int facilityId);
        int SaveCarePlanTask(CarePlanTask model, int val);
        int SaveCarePlanTaskCustomModel(CarePlanTaskCustomModel vm);
    }
}