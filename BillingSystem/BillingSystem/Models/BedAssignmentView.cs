using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Models
{
    public class BedAssignmentView
    {
        public IEnumerable<FacilityBedStructureCustomModel> AvailableBedList { get; set; }
        public IEnumerable<FacilityBedStructureCustomModel> OccupiedBedList { get; set; }
        public UBedMaster CurrentBedMaster { get; set; }

    }
}