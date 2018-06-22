using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Models
{
    public class DashboardDisplayOrderView
    {
     
        public DashboardDisplayOrder CurrentDashboardDisplayOrder { get; set; }
        public List<DashboardDisplayOrderCustomModel> DashboardDisplayOrderList { get; set; }

    }
}
