using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IPatientInfoChangesQueueService
    {
        PatientInfoChangesQueue GetPatientInfoChangesQueueByID(int? patientInfoChangesQueueId);
        List<PatientInfoChangesQueueCustomModel> GetPatientInfoChangesQueueList();
        List<PatientInfoChangesQueueCustomModel> SavePatientInfoChangesQueue(PatientInfoChangesQueue model);
    }
}