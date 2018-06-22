using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class DischargeSummaryDetailCustomModel : DischargeSummaryDetail
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
