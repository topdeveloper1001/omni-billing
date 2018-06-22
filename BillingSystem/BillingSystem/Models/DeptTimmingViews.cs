using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Models
{
    public class DeptTimmingView
    {
     
        public DeptTimming CurrentDeptTimming { get; set; }
        public List<DeptTimmingCustomModel> DeptTimmingList { get; set; }

    }
}
