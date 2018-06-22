using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Models
{
    public class DashboardIndicatorsView
    {
        public DashboardIndicators CurrentDashboardIndicators { get; set; }
        public List<DashboardIndicatorsCustomModel> DashboardIndicatorsList { get; set; }
    }
}
