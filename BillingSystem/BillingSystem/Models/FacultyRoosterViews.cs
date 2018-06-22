using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Models
{
    public class FacultyRoosterView
    {
     
        public FacultyRooster CurrentFacultyRooster { get; set; }
        public List<FacultyRoosterCustomModel> FacultyRoosterList { get; set; }

    }
}
