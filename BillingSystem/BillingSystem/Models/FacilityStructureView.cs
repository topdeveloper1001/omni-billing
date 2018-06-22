using System.Collections.Generic;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Models
{
    public class FacilityStructureView
    {
        public FacilityStructureCustomModel CurrentFacilityStructure { get; set; }
        public List<FacilityStructureCustomModel> FacilityStructureList { get; set; }

    }
}
