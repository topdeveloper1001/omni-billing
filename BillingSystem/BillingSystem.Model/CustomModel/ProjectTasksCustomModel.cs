using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class ProjectTasksCustomModel : ProjectTasks
    {
        public string StatusColor { get; set; }
        public string ProjectName { get; set; }
        public string KpiTargetDate { get; set; }
        public int ProjectTypeId { get; set; }
        public string ColorImage { get; set; }
        public string Responsible { get; set; }
        public string FacilityName { get; set; }
    }
}
