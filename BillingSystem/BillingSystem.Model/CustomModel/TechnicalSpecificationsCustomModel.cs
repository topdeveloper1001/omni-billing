using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class TechnicalSpecificationsCustomModel : TechnicalSpecifications
    {
        public string EName { get; set; }
    }
}
