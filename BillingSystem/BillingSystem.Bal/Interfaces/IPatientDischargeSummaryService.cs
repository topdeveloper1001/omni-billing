using System.Collections.Generic;
using BillingSystem.Model;

namespace BillingSystem.Bal.Interfaces
{
    public interface IPatientDischargeSummaryService
    {
        PatientDischargeSummary GetPatientDischargeSummaryByEncounterId(int? encounterId);
        List<PatientEvaluation> GetPatientEvaluationData(int patientId, int encounterId, IEnumerable<string> categories, string setId);
        List<PatientEvaluation> ListPatientEvaluation(int patientId, int encounterId, string globalCodeCategory, string setId);
        int SavePatientDischargeSummary(PatientDischargeSummary model);
    }
}