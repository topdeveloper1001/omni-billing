using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class ProjectsCustomModel : Projects
    {
        public string ProjectType { get; set; }
        public string ProjectStatus { get; set; }
        public string TaskName { get; set; }
        public string TaskNumber { get; set; }
        public string KpiTargetDate { get; set; }
        public string Responsible { get; set; }
        public string FacilityName { get; set; }
        public List<ProjectTasksCustomModel> Milestones { get; set; }
    }
}
