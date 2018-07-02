using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IPatientInsuranceService
    {
        PatientInsuranceCustomModel GetPatientInsurance(int patientId);
        PatientInsuranceCustomModel GetPatientInsuranceView(int patientId);
        PatientInsuranceCustomModel GetPrimaryPatientInsurance(int patientId, bool isPrimary);
        bool IsInsuranceComapnyInUse(int InsuranceComapnyid);
        List<int> SavePatientInsurance(PatientInsurance model);
    }
}