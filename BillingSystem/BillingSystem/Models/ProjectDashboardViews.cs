using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Models
{
    public class ProjectDashboardView
    {
     
        public ProjectDashboard CurrentProjectDashboard { get; set; }
        public List<ProjectDashboardCustomModel> ProjectDashboardList { get; set; }

    }
}
