using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Models
{
    public class DashboardBudgetView
    {
     
        public DashboardBudget CurrentDashboardBudget { get; set; }
        public List<DashboardBudgetCustomModel> DashboardBudgetList { get; set; }

    }
}
