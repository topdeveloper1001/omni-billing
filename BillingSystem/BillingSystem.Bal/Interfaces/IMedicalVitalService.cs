using System;
using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IMedicalVitalService
    {
        int AddUpdateModuleAccess(List<MedicalVital> moduleVitalList);
        int AddUptdateMedicalVital(MedicalVital MedicalVital);
        List<MedicalVitalCustomModel> GetCustomLabTest(int patientId, int encounterid, int type);
        List<MedicalVitalCustomModel> GetCustomMedicalVitals(int patientId, int type);
        List<MedicalVitalCustomModel> GetCustomMedicalVitals(int patientId, int type, int encounterid);
        List<MedicalVitalCustomModel> GetCustomMedicalVitalsByPidEncounterId(int patientId, int type, int currentEncounterId);
        List<MedicalVital> GetMedicalVital();
        MedicalVital GetMedicalVitalByID(int? MedicalVitalId);
        List<MedicalVitalCustomModel> GetMedicalVitalChart2(int vitalCode, int patientId, DateTime? fromDate, DateTime? tillDate);
        List<MedicalVitalCustomModel> GetMedicalVitalsChartData(int patientId, int type, DateTime tillDate);
        string GetNameByUserId1(int? UserID);
        RiskFactorViewModel GetRiskFactors(int patientId);
    }
}