using System.Collections.Generic;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Models
{
    public class ExecutiveDashboardView
    {
        public List<ExternalDashboardModel> Section1List { get; set; }
        public List<ExternalDashboardModel> Section5List { get; set; }
        public List<ExternalDashboardModel> Section10List { get; set; }
        public List<ExternalDashboardModel> BalanceSheetList { get; set; }
        public List<DashboardRemarkCustomModel> Section1RemarksList { get; set; }
        public List<DashboardRemarkCustomModel> Section2RemarksList { get; set; }
        public List<DashboardRemarkCustomModel> Section3RemarksList { get; set; }
        public List<DashboardRemarkCustomModel> Section4RemarksList { get; set; }
        public List<DashboardRemarkCustomModel> Section5RemarksList { get; set; }
        public List<DashboardRemarkCustomModel> Section6RemarksList { get; set; }
        public List<DashboardRemarkCustomModel> Section7RemarksList { get; set; }
        public List<DashboardRemarkCustomModel> Section8RemarksList { get; set; }
        public List<DashboardRemarkCustomModel> Section9RemarksList { get; set; }
        public List<DashboardRemarkCustomModel> Section10RemarksList { get; set; }
        public List<DashboardRemarkCustomModel> Section11RemarksList { get; set; }
        public List<DashboardRemarkCustomModel> Section12RemarksList { get; set; }
        public List<DashboardRemarkCustomModel> Section13RemarksList { get; set; }
        public List<DashboardRemarkCustomModel> Section14RemarksList { get; set; }
        public List<DashboardRemarkCustomModel> Section15RemarksList { get; set; }
        public List<DashboardRemarkCustomModel> Section16RemarksList { get; set; }
        public List<DashboardRemarkCustomModel> Section17RemarksList { get; set; }
        public List<DashboardRemarkCustomModel> Section18RemarksList { get; set; }
        public int FacilityId { get; set; }
        public string Title { get; set; }
        public int DashboardType { get; set; }
    }
}