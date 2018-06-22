using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class ProjectDashboardCustomModel : ProjectDashboard
    {
        public string ProjectNumber { get; set; }
        public string TaskNumber { get; set; }
        public string ProjectDescription { get; set; }
        public string RepsonsibleParty { get; set; }
        public DateTime? ProjectStartDate { get; set; }
        public DateTime? ProjectEstimatedCompletionDate { get; set; }
        public DateTime? ProjectRevisedCompletionDate { get; set; }
        public string Type { get; set; }
    }
}
