using System.Collections.Generic;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Models
{
    public class CaseMgtDashboardView
    {
        public int FacilityId { get; set; }
        public string Title { get; set; }
        public int DashboardType { get; set; }
        public List<DashboardRemarkCustomModel> Section1RemarksList { get; set; }
        public List<DashboardRemarkCustomModel> Section2RemarksList { get; set; }
        public List<DashboardRemarkCustomModel> Section3RemarksList { get; set; }
        public List<DashboardRemarkCustomModel> Section4RemarksList { get; set; }
        public List<DashboardRemarkCustomModel> Section5RemarksList { get; set; }
    }
}