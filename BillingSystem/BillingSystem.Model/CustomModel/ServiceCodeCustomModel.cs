using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class ServiceCodeCustomModel : ServiceCode
    {
        public string ServiceCodeValueDesc { get; set; }
        public string ServiceCodeServiceCodeMainText { get; set; }
        public string ServiceServiceCodeSubText { get; set; }
    }
}
