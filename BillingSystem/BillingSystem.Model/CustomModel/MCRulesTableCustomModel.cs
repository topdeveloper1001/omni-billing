using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class MCRulesTableCustomModel : MCRulesTable
    {
        public string PatientTypeStr { get; set; }
        public string ConStartStr { get; set; }
        public string ConNextLineStr { get; set; }
        public string ConpareTypeStr { get; set; }
        public string CalculationTypeStr { get; set; }

        public string CompareTableString { get; set; }
        public string ComparorTableString { get; set; }

    }
}
