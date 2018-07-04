using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface ILabTestResultService
    {
        int DeleteLabTestResult(LabTestResult model);
        List<LabTestResultCustomModel> GetLabTestResult();
        List<LabTestResultCustomModel> GetLabTestResultByCorporateFacility(int corporateId, int facilityId);
        LabTestResult GetLabTestResultByID(int? LabTestResultId);
        int SaveLabTestResult(LabTestResult model);
    }
}