using System;
using System.Collections.Generic;
using System.Data;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IFacilityService
    {
        string GetFacilityNameByNumber(string number);
        List<FacilityCustomModel> AddUpdateFacility(FacilityCustomModel facility, DataTable dt, out int facilityId);
        int CheckDuplicateFacilityNoAndLicenseNo(string facilityNumber, string lic, int id, int corporateId);
        void CreateDefaultFacilityItems(int fId, string fName, int userId);
        bool DeleteFacilityData(string facilityId);
        List<Facility> GetFacilities(int corporateId);
        List<Facility> GetFacilities(int corporateId, int facilityId);
        IEnumerable<Facility> GetFacilitiesByCorpoarteId(int corporateId);
        List<Facility> GetFacilitiesByCorporateId(int corpId);
        List<Facility> GetFacilitiesByCorporateIdWithoutCountries(int corpId);
        List<Facility> GetFacilitiesByRoles(int facilityId, int corporateId);
        List<DropdownListData> GetFacilitiesForDashboards(int facilityId, int corporateId, bool userIsAdmin);
        List<Facility> GetFacilitiesWithoutCorporateFacility(int corporateId);
        IEnumerable<Facility> GetFacilitiesWithoutCorporateFacility(int corporateId, int facilityId);
        FacilityCustomModel GetFacilityById(int id);
        Facility GetFacilityDetailByPatientId(int patientId);
        List<DropdownListData> GetFacilityDropdownData(int corporateId, int facilityId);
        List<FacilityCustomModel> GetFacilityList(int corporateId);
        string GetFacilityNameById(int id);
        string GetFacilityNumberById(int id);
        string GetFacilityTimeZoneById(int id);
        DateTime GetInvariantCultureDateTime(int facilityid);
    }
}