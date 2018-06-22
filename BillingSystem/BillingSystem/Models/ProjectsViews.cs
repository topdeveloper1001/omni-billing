using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Models
{
    public class ProjectsView
    {
        public Projects CurrentProjects { get; set; }
        public List<ProjectsCustomModel> ProjectsList { get; set; }
        public ProjectTargets CurrentProjectTargets { get; set; }
        public ProjectTasks CurrentProjectTasks { get; set; }
        public List<ProjectTargetsCustomModel> ProjectTargetList { get; set; }
        public List<ProjectTasksCustomModel> ProjectTaskList { get; set; }
        public ProjectTaskTargets CurrentProjectTaskTarget { get; set; }
        public List<ProjectTaskTargetsCustomModel> ProjectTaskTargetsList { get; set; }
        public Int32 FacilityId { get; set; }
    }
}
