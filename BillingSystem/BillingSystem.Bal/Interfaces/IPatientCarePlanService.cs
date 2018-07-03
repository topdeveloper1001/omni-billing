using System;
using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IPatientCarePlanService
    {
        List<PatientCarePlanCustomModel> AddUpdatePatientCarePlan(int corporateId, int facilityId);
        List<PatientCarePlanCustomModel> AddUpdatePatientCarePlan_1(int patientID, int encounterID);
        List<PatientCarePlanCustomModel> BindCarePlanTaskData(string careId, string patientId, int encounterId);
        bool CheckDuplicateTaskName(int id, int encounterId, string patientId, string taskId, DateTime startDate, DateTime endDate, int facilityId, int corporateId);
        void DeletePatientCarePlan(PatientCarePlan model);
        List<PatientCarePlanCustomModel> GetPatientCarePlan();
        PatientCarePlan GetPatientCarePlanById(int? PatientCarePlanId);
        List<PatientCarePlanCustomModel> GetPatientCarePlanByPatientIdAndEncounterId(string patientId, int encounterId);
        PatientCarePlan GetPatientCarePlanByPlanIdTaskId(string planid, string taskid, string patientid);
        PatientCarePlan GetPatientCarePlanByTaskId(string taskid);
        List<PatientCarePlan> GetPatientCarePlanPlanId(string planId, string patientId, int encounterId);
        List<PatientCarePlan> GetTaskList(string patientId, int encounterId);
        PatientCarePlanTaskCustomModel MapCustomModelModelToViewModel(PatientCarePlan model);
        int SavePatientCarePlan(PatientCarePlan model);
        int SavePatientCarePlanData(List<PatientCarePlan> model, bool isDelete);
    }
}