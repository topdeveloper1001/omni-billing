using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IMedicalRecordService
    {
        int AddOtherDrugAlergy(MedicalRecord model);
        int AddUptdateMedicalRecord(MedicalRecord medicalRecord);
        int DeleteMedicalRecordById(int id);
        List<AlergyCustomModel> GetAlergyRecords(int pateintID, int MedicalRecordType);
        List<MedicalRecord> GetAlergyRecordsByPatientIdEncounterId(int patientId, int encounterID);
        List<MedicalRecord> GetMedicalRecord();
        MedicalRecord GetMedicalRecordById(int? medicalRecordId);
        string GetNameByUserId1(int? UserID);
        List<OtherDrugAlergy> GetOtherDrugAlergyListByEncounter(int encounterId, string DrugTableNumber);
        bool SaveMedicalRecords(List<MedicalRecord> list, long patientId, long encounterId, long userId, long corporateId, long facilityId);
    }
}