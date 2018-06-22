using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class CPTCodesCustomModel : CPTCodes
    {
        public string GlobalCodeCategoryName { get; set; }
        public Int32 GlobalCodeCategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryId { get; set; }
        public string CreatedByName { get; set; }
    }

    public class ExportCodesData
    {
        public string TableNumber { get; set; }
        public string Code { get; set; }
        public string CodeDescription { get; set; }
        public string Price { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTill { get; set; }
        public string CodeGroup { get; set; }
        public string OtherValue1 { get; set; }
        public string OtherValue2 { get; set; }
        public string OtherValue3 { get; set; }
        public string OtherValue4 { get; set; }
        public string OtherValue5 { get; set; }
        public string OtherValue6 { get; set; }
    }
}
