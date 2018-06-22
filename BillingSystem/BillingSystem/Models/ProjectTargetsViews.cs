using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Models
{
    public class ProjectTargetsView
    {
     
        public ProjectTargets CurrentProjectTargets { get; set; }
        public List<ProjectTargetsCustomModel> ProjectTargetsList { get; set; }

    }
}
