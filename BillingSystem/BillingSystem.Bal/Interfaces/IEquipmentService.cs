using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IEquipmentService
    {
        bool AddRoomIdToEquipments(int facilityStructureId, List<int> equipmentIds);
        int AddUpdateEquipment(EquipmentMaster equipment);
        bool CheckEquipmentInScheduling(int equipmentId);
        List<EquipmentCustomModel> GetDeletedEquipmentList(bool showIsDeleted, string facilityId);
        EquipmentMaster GetEquipmentByMasterId(int id);
        List<EquipmentMaster> GetEquipmentDataByMasterId(int id);
        List<EquipmentCustomModel> GetEquipmentList(bool showIsDisabled, string facilityId);
        List<EquipmentMaster> GetEquipmentListByFacilityId(string facilityId, int facilityStructureId);
        int GetFacilityStructureIdByEquipmentMasterId(int equipmentMasterId);
        int UpdateEuipmentCustomModel(EquipmentCustomModel vm);
    }
}