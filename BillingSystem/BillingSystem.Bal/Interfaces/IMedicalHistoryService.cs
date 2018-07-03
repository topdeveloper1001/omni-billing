using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IMedicalHistoryService
    {
        int AddUpdateMedicalHistory(MedicalHistory model);
        AlergyView GetAlergyAndMedicalHistorDataOnLoad(long patientId, long userId, long encounterId = 0);
        List<MedicalHistoryCustomModel> GetMedicalHistory(int patientId, int encounterId);
        MedicalHistoryCustomModel GetMedicalHistoryById(int id);
    }
}