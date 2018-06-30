using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IDiagnosisService
    {
        int CheckIfAnyDiagnosisExists(int encounterId);
        bool CheckIfDuplicateDiagnosisAgainstCurrentEncounter(string diagnosisCode, int encounterId, int diagnosisId);
        DiagnosisTabData DeleteCurrentDiagnosis(long userId, long id, string drgTn);
        IEnumerable<DiagnosisCode> ExportFilteredDiagnosisCodes(string text, string tableNumber);
        List<DiagnosisCode> GetAllDiagnosisCodes(int facilityId, string DiagnosisTableNumber);
        IEnumerable<DiagnosisCustomModel> GetCurrentDiagnosisData(long pId, long eid);
        Diagnosis GetDiagnosisById(string id);
        Diagnosis GetDiagnosisInfoByEncounterId(int encounterId);
        List<DiagnosisCustomModel> GetDiagnosisList(int patientId);
        List<DiagnosisCustomModel> GetDiagnosisList(int patientId, int encounterid);
        List<DiagnosisCustomModel> GetDiagnosisListByEncounterId(int encounterId);
        DiagnosisTabData GetDiagnosisTabData(long pId, long eId = 0, long physicianId = 0, string diagnosisTn = "", string drgTn = "");
        string GetDiagnosisTypeById(int id);
        Diagnosis GetDRGDiagnosisInfoByEncounterId(int encounterId);
        List<DiagnosisCode> GetFilteredDiagnosis(List<string> codeList, string DiagnosisTableNumber);
        IEnumerable<DiagnosisCode> GetFilteredDiagnosisCodes(string keyword, long userId, long facilityId, string DiagnosisTableNumber);
        DiagnosisCustomModel GetNewDiagnosisByEncounterId(int encounterId, int patientId);
        List<DiagnosisCustomModel> GetPreviousDiagnosisList(int patientId, int encounterid);
        int SaveDiagnosis(DiagnosisCustomModel vm, string DiagnosisTableNumber);
    }
}