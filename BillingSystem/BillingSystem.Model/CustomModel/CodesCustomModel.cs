using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class CodesCustomModel
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string CodeType { get; set; }
        public string ID { get; set; }
        public string ExternalCode { get; set; }
    }
}
