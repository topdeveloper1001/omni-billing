using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class EquipmentCustomModel
    {
        public int EquipmentMasterId { get; set; }
        public string FacilityId { get; set; }
        public string EquipmentName { get; set; }
        public string EquipmentType { get; set; }
        public string EquipmentModel { get; set; }
        public string EquipmentSerialNumber { get; set; }
        public bool? EquipmentIsInsured { get; set; }
        public DateTime? EquipmentAquistionDate { get; set; }
        public bool EquipmentDisabled { get; set; }
        public DateTime? EquipmentDisabledDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsDeleted { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool? IsEquipmentFixed { get; set; }
        public string TurnAroundTime { get; set; }
        public string CorporateId { get; set; }
        public string BaseLocation { get; set; }
        public int FacilityStructureId { get; set; }
        public DateTime? EquipmentEnableDate { get; set; }
        public string FacilityName { get; set; }
        public string EquipmentTypeName { get; set; }
        public string AssignedRoom { get; set; }
        public string Department { get; set; }
        public string RoomDepartment { get; set; }

        public bool ActiveDeactive { get; set; }
    }
}
