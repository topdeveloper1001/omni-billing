using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Models
{
    public class KPISummaryDashboardView
    {
        public List<PatientFallStats> PatinetFallList { get; set; }
        //public List<ExternalDashboardModel> Section5List { get; set; }
        //public List<ExternalDashboardModel> Section10List { get; set; }
        public int FacilityId { get; set; }
        public string Title { get; set; }
        public int DashboardType { get; set; }
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
    }
}