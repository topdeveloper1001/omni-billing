using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IBedMasterService
    {
        int AddUpdateBedMaster(UBedMaster m);
        bool CheckIdBedOccupied(int bedid);
        bool DeleteBedMasterById(int facilityStructureId);
        string GetBedByInPatientEncounterId(string encounterId);
        UBedMaster GetBedMasterById(int id);
        UBedMaster GetBedMasterByStructureId(int id);
        UBedMaster GetBedMasterIdByStructureId(int id);
        IEnumerable<BedMasterCustomModel> GetBedMasterListByRole(int facilityId, int corporateid);
        string GetBedNameByInPatientEncounterId(string encounterId);
        string GetBedNameFromBedId(int bedId);
        List<FacilityBedStructureCustomModel> GetBedStrutureForFacility(int facilityId, int corporateid, string ServiceCodeTableNumber);
        int? GetBedTypeByServiceCode(string serviceCode);
        IEnumerable<DropdownListData> GetOverrideBedsListInEncounters(string serviceCodeValues, string ServiceCodeTableNumber);
        string GetOverRideBedTypeByInPatientEncounterId(string encounterId);
    }
}