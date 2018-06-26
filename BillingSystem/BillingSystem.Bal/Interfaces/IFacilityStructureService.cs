using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IFacilityStructureService
    {
        int AddUptdateFacilityStructure(FacilityStructure facilityStructure);
        bool CheckForChildrens(int facilityStructureId);
        bool CheckStructureExist(FacilityStructure facilityStructure);
        bool DeleteFacilityStructureById(int facilityStructureId);
        List<FacilityStructureCustomModel> GetAppointRoomAssignmentsList(int facilityId, string txtSearch);
        List<FacilityStructureCustomModel> GetAppointRoomAssignmentsList(string facilityId);
        List<AppointmentTypesCustomModel> GetDepartmentAppointmentTypes(int deptId, int facilityId, int cId, bool active);
        List<AppointmentTypes> GetDepartmentAppointmentTypes(int deptId, string facilityId);
        List<FacilityStructure> GetDepartmentRooms(int deptId, string facilityId);
        List<FacilityStructure> GetDepartmentRooms(List<SchedularFiltersCustomModel> deptIds, string facilityId);
        List<FacilityStructure> GetFacilityBeds(string facilityId);
        List<FacilityStructure> GetFacilityDepartments(int corporateid, string facilityid);
        List<FacilityStructure> GetFacilityRooms(int corporateid, string facilityid);
        List<FacilityStructureRoomsCustomModel> GetFacilityRoomsByDepartments(int corporateid, string facilityid, string depIds, string roomIds);
        List<FacilityStructureRoomsCustomModel> GetFacilityRoomsCustomModel(int corporateid, string facilityid);
        List<FacilityStructureCustomModel> GetFacilityStructure(string facilityId);
        string GetFacilityStructureBreadCrumbs(int facilityStructureId, string facilityid, string ParentId);
        FacilityStructure GetFacilityStructureById(int? facilityStructureId);
        List<FacilityStructureCustomModel> GetFacilityStructureCustom(string facilityId, int structureId);
        List<FacilityStructureCustomModel> GetfacilityStructureData(int facilityId, int structureId, bool showIsActive);
        List<FacilityStructure> GetFacilityStructureForDDL(string facilityId);
        List<DropdownListData> GetFacilityStructureListByParentId(int parentId);
        string GetFacilityStructureNameById(int? facilityStructureId);
        List<FacilityStructureCustomModel> GetFacilityStructureParent(int facilityStructureId, string facilityid);
        List<FacilityStructureCustomModel> GetInActiveFacilityStructureCustomList(string facilityId, int structureId);
        int GetMaxSortOrder(string facilityId, string structureType);
        string GetParentNameByFacilityStructureId(int facilityStructureId);
        string GetParentNameById(int parentId);
        List<DropdownListData> GetRevenueDepartments(int corporateid, string facilityid);
        List<FacilityStructure> GetRoomsByFacilityId(string facilityid);
    }
}