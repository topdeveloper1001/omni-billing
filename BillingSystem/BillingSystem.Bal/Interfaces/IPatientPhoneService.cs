using System;
using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IPatientPhoneService
    {
        IEnumerable<PatientPhoneCustomModel> DeletePatientPhone(PatientPhone model);
        PatientPhone GetPatientPersonalPhoneByPateintId(int patientId);
        PatientPhone GetPatientPhoneById(int patientphoneId);
        IEnumerable<PatientPhoneCustomModel> GetPatientPhoneList(int patientId);
        int GetPhoneId(int patientId);
        int SavePatientPhone(PatientPhone model);
        int SavePatientPhoneInfo(int patientId, int phoneType, string phone, int userId, DateTime currentDateTime);
    }
}