using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Models
{
    public class ProjectTasksView
    {
     
        public ProjectTasks CurrentProjectTasks { get; set; }
        public List<ProjectTasksCustomModel> ProjectTasksList { get; set; }

    }
}
