using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Models
{
    public class DashboardIndicatorDataView
    {
     
        public DashboardIndicatorData CurrentDashboardIndicatorData { get; set; }
        public List<DashboardIndicatorDataCustomModel> DashboardIndicatorDataList { get; set; }

    }
}
