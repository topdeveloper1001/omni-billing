using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class BedMasterCustomModel
    {
        public UBedMaster BedMaster { get; set; }
        //public IEnumerable<MappingBedService> MappingBedService { get; set; }
        public IEnumerable<BedRateCardCustomModel> MedRateCardlist { get; set; }
        public string FacilityName { get; set; }
        public string BedTypeName { get; set; }
        public string BedName { get; set; }
        public string FloorName { get; set; }
        public string RoomName { get; set; }
        public string DeptName { get; set; }
        public int ServiceCodeId { get; set; }
        public DateTime? ExpectedEndDate { get; set; }
        //public IEnumerable<GlobalCodeCustomModel> BedOverrideTypeList { get; set; }
        public IEnumerable<DropdownListData> BedOverrideTypeList { get; set; }
        public bool BedOverride { get; set; }
        public BedMasterStructureModel BedMasterModel { get; set; }
        public bool NonChargeableRoom { get; set; }
    }
    [NotMapped]
    public class BedMasterStructureModel
    {
        public string FloorName { get; set; }
        public string RoomName { get; set; }
        public string DeptName { get; set; }
        public string FloorId { get; set; }
        public string RoomId { get; set; }
        public string DeptId { get; set; }
        public int? SearchSortOrder { get; set; }
    }
}
