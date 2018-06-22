using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Models
{
    public class ManualDashboardView
    {

        public ManualDashboardCustomModel CurrentManualDashboard { get; set; }
        public List<ManualDashboardCustomModel> ManualDashboardList { get; set; }
        public bool IsAdmin { get; set; }
        public int CFacilityId { get; set; }
        public int CYear { get; set; }
        public int COwner { get; set; }
        public int CIndicator { get; set; }
    }
}
