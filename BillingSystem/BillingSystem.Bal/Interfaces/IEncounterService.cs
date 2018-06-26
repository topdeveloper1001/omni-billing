using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces

{
    public interface IEncounterService
    {
        DateTime GetInvariantCultureDateTime(int facilityid);
        string GetPhysicianName(int physicianId);
        bool PatientEncounterOpenOrders(int patientId);
        bool EncounterOpenOrders(int encounterId);
        bool AddBedChargesForTransferPatient(int encounterId, DateTime currentdatetime);
        int AddUpdateEncounter(Encounter e);
        bool AddVirtualDischargeLog(int encounterId, int facilityId, int corporateId);
        string CalulateAverageLengthOfStay(DateTime endcounterstartTime, int facilityId);
        bool CheckEncounterNumberExist(string uniqueNumber);
        Encounter GetActiveEncounterByPateintId(int patientId);
        List<ClaimDenialPercentage> GetActiveEncounterChartData(int facilityId, int displayType, DateTime fromDate, DateTime tillDate);
        List<EncounterCustomModel> GetActiveEncounters(CommonModel common, string DiagnosisTableNumber, string DrgTableNumber);
        List<Encounter> GetActiveMotherDropdownData(int corporateId);
        List<EncounterCustomModel> GetAllActiveEncounters(string facilityNumber, List<int> patientTypes);
        Task<EncounterCustomModel> GetAuthorizationViewDataAsync(int encounterid);
        List<CMODashboardModel> GetCMODashboardData(int facilityId, int corporateid);
        Encounter GetEncounterByEncounterId(int encounterId);
        Encounter GetEncounterByPatientIdAndActive(int patientId);
        List<Encounter> GetEncounterByUserId(int userId);
        IEnumerable<EncounterExtension> GetEncounterChartData(int facilityId, int displayType, DateTime fromDate, DateTime tillDate);
        EncounterCustomModel GetEncounterDetailByEncounterID(int encounterid);
        Encounter GetEncounterDetailById(int encounterId);
        EncounterCustomModel GetEncounterDetailByPatientId(int patientId);
        int GetEncounterEndCheck(int encounterId, int userId);
        int GetEncounterEndCheckVirtualDischarge(int encounterId, int loggedInUserId);
        IEnumerable<EncounterCustomModel> GetEncounterListByPatientId(int patientId);
        string GetEncounterNumberByEncounterId(int encounterId);
        bool GetEncounterOpenStatus(int patientId);
        List<EncounterCustomModel> GetEncountersByPatientId(int patientId);
        List<Encounter> GetEncountersListByPatientId(int p);
        Encounter GetEncounterStateByPatientId(int patientId);
        int GetEncounterStatusBeforeUpdate(int encounterId);
        string GetInsuranceMemberIdByPatientId(int patientId);
        List<PatientEvaluationSetCustomModel> GetNurseAssessmentData(int encounterId, int patientId);
        EncounterCustomModel GetPatientBedInformationByBedId(int patientId, int bedId, string serviceCodeValue);
        EncounterCustomModel GetPatientBedInformationByPatientId(int patientId);
        string GetPatientStateData(int encounterId);
        List<PatientEvaluationSetCustomModel> GetPreActiveEncounters(int encounterId, int patientId);
        string GetTriageData(int encounterId);
        List<EncounterCustomModel> GetUnclosedEncounters(int facilityId, int corporateId);
        Encounter GetXMLActiveEncounterByPateintId(int PatientId);
        bool IsPatientExist(int patientId);
        string UpdatePatitentStageInEncounter(int encounterId, string patientStage);
        string UpdateTriageLevelInEncounter(int encounterId, string triageLevel);
        List<int> GetActiveEncounterIdsByPatientId(int patientId);
    }
}