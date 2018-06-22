using System.Collections.Generic;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Models
{
    public class DashboardTargetsView
    {
        public DashboardTargets CurrentDashboardTargets { get; set; }
        public List<DashboardTargetsCustomModel> DashboardTargetsList { get; set; }
    }
}
