using System.Collections.Generic;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Models
{
    public class ExternalDashboardView
    {
        public int FacilityId { get; set; }
        public string Title { get; set; }
        public int DashboardType { get; set; }
        public List<ManualDashboardCustomModel> DashboardStatisticsList { get; set; }
    }
}