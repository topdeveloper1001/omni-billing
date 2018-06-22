using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Models
{
    public class DashboardParametersView
    {
     
        public DashboardParameters CurrentDashboardParameters { get; set; }
        public List<DashboardParametersCustomModel> DashboardParametersList { get; set; }

    }
}
