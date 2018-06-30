using System.Collections.Generic;
using System.Threading.Tasks;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IPhysicianService
    {
        int AddUpdatePhysician(PhysicianViewModel vm);
        List<PhysicianCustomModel> BindPhysicianBySpeciality(int facilityId, string specialityId);
        bool CheckDuplicateClinicalId(string clinicalId, int id);
        bool CheckDuplicateEmpNo(int empNo, int physicianId);
        bool CheckIfUserTypeAndUserIdAlreadyExists(int userType, int userId, int physicianId);
        List<PhysicianCustomModel> GetCorporatePhysiciansList(int corporateId, bool isadmin, int userid, int facilityId);
        List<PhysicianCustomModel> GetCorporatePhysiciansPreScheduling(int corporateId, int facilityId);
        List<Users> GetDistinctUsersByUserTypeId(int roleId, int corporateId, int facilityId);
        List<Physician> GetFacilityPhysicians(int facilityId);
        Task<List<PhysicianViewModel>> GetFacultyList(int facilityId, long userId = 0);
        string GetNameByLicenseTypeIdAndUserTypeId(string licenceTypeId, string userTypeId);
        List<Physician> GetPhysicianByCorporateIdandfacilityId(int corporateId, int facilityId);
        Physician GetPhysicianByUserId(int physicianId);
        PhysicianCustomModel GetPhysicianCModelById(int physicianId);
        List<PhysicianCustomModel> GetPhysicianListByPatientId(int patientId);
        List<PhysicianCustomModel> GetPhysicians(int corporateId, bool isadmin, int userid, int facilityId);
        List<Physician> GetPhysiciansByPhysicianTypeId(int physicianTypeId, int facilityid);
        List<PhysicianCustomModel> GetPhysiciansListByFacilityId(int facilityId);
    }
}