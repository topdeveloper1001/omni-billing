using System.Collections.Generic;
using BillingSystem.Model;

namespace BillingSystem.Bal.Interfaces
{
    public interface IDiagnosisCodeService
    {
        int AddUptdateDiagnosisCode(DiagnosisCode m, string DiagnosisTableNumber);
        List<DiagnosisCode> GetDiagnosisCode(string DiagnosisTableNumber);
        DiagnosisCode GetDiagnosisCodeByCodeId(string id, string DiagnosisTableNumber);
        DiagnosisCode GetDiagnosisCodeByID(int? diagnosisCodeId);
        List<DiagnosisCode> GetDiagnosisCodeData(bool showInActive, string DiagnosisTableNumber);
        string GetDiagnosisCodeDescById(string codeId, string DiagnosisTableNumber);
        List<DiagnosisCode> GetFilteredDiagnosisCodes(string text, string DiagnosisTableNumber);
        List<DiagnosisCode> GetFilteredDiagnosisCodesData(string text, string tableNumber);
        List<DiagnosisCode> GetListOnDemand(int blockNumber, int blockSize, string DiagnosisTableNumber);
    }
}