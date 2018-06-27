using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IPatientInfoService
    {
        string GetFacilityNameByFacilityId(int facilityId);
        string GetPatientNameById(int PatientID);
        string GetPersonMotherNameById(int patientId);
        int AddUpdatePatientInfo(PatientInfo patientInfo);
        int CalculatePersonAge(DateTime? birthDate, DateTime now);
        bool CheckForDuplicateEmail(string email, int patientId);
        bool CheckForDuplicateHealthCareNumber(string memberId, int patientId, int insCompanyId, int insPlanId, int insPolicyId);
        bool CheckIfBirthDateExists(DateTime birthDate);
        bool CheckIfEmailExists(string email);
        bool CheckIfEmiratesIdExists(string emiratesId, int patientId, string lastname, DateTime bDate, int facilityId);
        bool CheckIfMedicalRecordNumber(string medicalRecordNumber);
        bool CheckIfPassportExists(string passportNumber, int patientId);
        object CheckPatientActiveEncounter(int patientidint, int encountertypeint);
        int GetMaxMedicalRecordNumber();
        int GetNextPatientId();
        PatientInfo GetPatientDetailByEmailid(string email);
        PatientInfoCustomModel GetPatientDetailsByPatientId(int patientId, int encounterId = 0, bool showEncounters = false);
        PatientInfo GetPatientInfoById(int? patientId);
        IEnumerable<PatientInfo> GetPatientInfoByPatientName(string patientName, int corporateId, long facilityId);
        PatientInfoCustomModel GetPatientInfoCustomModelById(int? patientId);
        PatientInfoViewData GetPatientInfoOnLoad(long patientId, long encounterId);
        List<PatientInfo> GetPatientList(int facilityId);
        List<PatientInfo> GetPatientNames(int fId, int cId);
        List<PatientInfo> GetPatientsByFacilityId(int facilityId);
        List<PatientInfoCustomModel> GetPatientSearchResult(CommonModel vm);
        List<PatientInfoCustomModel> GetPatientSearchResultAndOtherData(CommonModel m);
        List<PatientInfoCustomModel> GetPatientSearchResultByCId(CommonModel patientSearch);
        List<PatientInfoXReturnPaymentCustomModel> GetPatientSearchResultInPayment(CommonModel common);
        PatientInfoCustomModel PatientDetailsByPatientIdForDropdown(int patientId);
        RegisterPatientCustomModel PatientInfoForSchedulingByPatient(int patientId);
        Task<ResponseData> SavePatientInfo(PatientInfo patientInfo, PatientInsuranceCustomModel ins, PatientAddressRelation a, int userId, DateTime currentDate, string tokenId, string codeValue, List<DocumentsTemplates> docs);
        int GetPatientIdByEncounterId(int encounterId);

    }
}