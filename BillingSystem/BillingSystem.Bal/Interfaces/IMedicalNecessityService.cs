using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IMedicalNecessityService
    {
        List<MedicalNecessityCustomModel> GetMedicalNecessity(string DiagnosisTableNumber);
        MedicalNecessity GetMedicalNecessityById(int? medicalNecessityId);
        int SaveMedicalNecessity(MedicalNecessity model);
    }
}