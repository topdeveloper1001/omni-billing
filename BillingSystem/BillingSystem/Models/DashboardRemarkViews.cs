using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Models
{
    public class DashboardRemarkView
    {
     
        public DashboardRemark CurrentDashboardRemark { get; set; }
        public List<DashboardRemarkCustomModel> DashboardRemarkList { get; set; }

    }
}
