using System.Collections.Generic;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Models
{
    public class ProjectsDashboardView
    {
        //public int FacilityId { get; set; }
        //public string Title { get; set; }
        //public int DashboardType { get; set; }
        //public List<ProjectDashboardCustomModel> ProjectsDashbaord { get; set; }
        //public List<ProjectDashboardCustomModel> TaskDashbaord { get; set; }
        //public List<DashboardRemarkCustomModel> Section1RemarksList { get; set; }
        //public List<DashboardRemarkCustomModel> Section2RemarksList { get; set; }
        //public List<DashboardRemarkCustomModel> Section3RemarksList { get; set; }
        //public List<DashboardRemarkCustomModel> Section4RemarksList { get; set; }

        public List<DashboardRemarkCustomModel> Section1RemarksList { get; set; }
        public List<DashboardRemarkCustomModel> Section2RemarksList { get; set; }
        public List<DashboardRemarkCustomModel> Section3RemarksList { get; set; }
        public List<DashboardRemarkCustomModel> Section4RemarksList { get; set; }
        public List<ProjectsCustomModel> StrategicKpiList { get; set; }
        public List<ProjectsCustomModel> FinancialKpiList { get; set; }
        public List<ProjectsCustomModel> IndividualKpiList { get; set; }
        public List<ProjectsCustomModel> OperationalKpiList { get; set; }

        public int FacilityId { get; set; }
        public string Title { get; set; }
        public int DashboardType { get; set; }
        public int ResponsibleUserId { get; set; }
    }
}