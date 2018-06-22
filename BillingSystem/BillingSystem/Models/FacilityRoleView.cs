using System.Collections.Generic;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Models
{
    public class FacilityRoleView
    {
        public FacilityRoleCustomModel CurrentFacilityRole { get; set; }
        public List<FacilityRoleCustomModel> FacilityRolesList { get; set; }
    }
}
