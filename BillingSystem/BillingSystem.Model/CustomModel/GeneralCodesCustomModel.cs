using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class GeneralCodesCustomModel
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string CodeDescription { get; set; }
        public string CodeType { get; set; }
        public string CodeTypeName { get; set; }
        public string ID { get; set; }
        public string ExternalCode { get; set; }
        public int? GlobalCodeId { get; set; }
        public string GlobalCodeCategoryId { get; set; }
        public string GlobalCodeName { get; set; }
        public string GlobalCodeCategoryName { get; set; }
        public string DrugPackageName { get; set; }
    }
}
