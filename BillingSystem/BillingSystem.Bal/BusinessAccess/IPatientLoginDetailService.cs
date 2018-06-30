using System;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IPatientLoginDetailService
    {
        string GetPatientEmail(int patientId);
        PatientLoginDetailCustomModel GetPatientLoginDetailByPatientId(int patientId);
        PatientLoginDetailCustomModel GetPatientLoginDetailsByEmail(string email);
        int SavePatientLoginDetails(PatientLoginDetailCustomModel vm);
        void UpdatePatientEmailId(int patientId, string emailId);
        void UpdatePatientLoginFailedLog(int patientId, int failedLoginAttempts, DateTime lastInvalidLogin);
    }
}