using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Models
{
    public class FacilityDepartmentView
    {
     
        public FacilityDepartment CurrentFacilityDepartment { get; set; }
        public List<FacilityDepartmentCustomModel> FacilityDepartmentList { get; set; }

    }
}
