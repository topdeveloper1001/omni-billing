using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class BedOccupancyCustomModel
    {
        public bool IsOccupied { get; set; }
        public string BedStatus { get; set; }
        public int Beds { get; set; }
        public int TotalBeds { get; set; }
        public string Room { get; set; }
        public string Department { get; set; }
        public string Floor { get; set; }
        public string Bed { get; set; }

        public string Title { get; set; }
        public int Code { get; set; }
        public string XAxis { get; set; }
        public int YAxis { get; set; }

        public int VacantBeds { get; set; }
        public int OccupiedBeds { get; set; }
        public decimal OccupiedRate { get; set; }

        public string TypeName { get; set; }
        public int Budget { get; set; }
        public int Current { get; set; }
        public int Previous { get; set; }

        public int SortOrder { get; set; }
    }
    [NotMapped]
    public class BedOccupancyFloorDashboard
    {
        public string ParentFloorName { get; set; }
        public string ParentDepartmentName { get; set; }
        public IEnumerable<BedOccupancyCustomModel> Collection { get; set; }
        public BedOccupancyFloorDashboard(string FName, string DName, IEnumerable<BedOccupancyCustomModel> collection)
        {
            this.ParentFloorName = FName;
            this.ParentDepartmentName = DName;
            this.Collection = collection.Where(x => x.Department == DName);
        }
    }
    [NotMapped]
    public class FacilityBedStructureCustomModel
    {
        public int BedId { get; set; } 
        public string BedTypeName { get; set; }
        public int BedType { get; set; }
        public string FacilityName { get; set; }
        public bool IsOccupied { get; set; }
        public string BedName { get; set; }
        public int RoomID { get; set; }
        public string Room { get; set; }
        public bool RoomNonChargeAble { get; set; }
        public int DepartmentID { get; set; }
        public string Department { get; set; }
        public int FloorID { get; set; }
        public string Floor { get; set; }
        public int SortOrder { get; set; }
        public bool CanOverRide { get; set; }
        public string OverRideWith { get; set; }
        public IEnumerable<DropdownListData> BedOverrideTypeList { get; set; }
        public IEnumerable<BedRateCardCustomModel> MedRateCardlist { get; set; }
    }
}
