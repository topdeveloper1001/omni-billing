using System.Collections.Generic;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IPatientEvaluationService
    {
        int GetCreatedByFromEvaluationSet(int setId, int patientId);
        string GetSignaturePath(int ecounterId, int patinetId, string setId);
        List<PatientEvaluationCustomModel> ListPatientEvaluation(int patientId, int encounterId);
        int SaveEvaluationSet(PatientEvaluationSet model);
        ResponseData SavePatientEvaluationData(List<string> data, long patientId, long eId, long cId, long fId, long userId, long setId, string eStatus, string imagePath);
    }
}