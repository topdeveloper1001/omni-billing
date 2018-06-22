using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class XActivityCustomModel : XActivity
    {
        public string ActivityType { get; set; }
        public decimal? UnAppliedAmount { get; set; }
        public decimal? AppliedAmount { get; set; }
    }
}
