using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IPatientPreSchedulingService
    {
        List<PatientPreSchedulingCustomModel> GetPatientPreScheduling();
        PatientPreScheduling GetPatientPreSchedulingById(int? patientPreSchedulingId);
        int SavePatientPreScheduling(PatientPreScheduling model);
    }
}