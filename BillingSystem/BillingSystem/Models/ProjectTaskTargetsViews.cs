using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Models
{
    public class ProjectTaskTargetsView
    {
     
        public ProjectTaskTargets CurrentProjectTaskTargets { get; set; }
        public List<ProjectTaskTargetsCustomModel> ProjectTaskTargetsList { get; set; }

    }
}
