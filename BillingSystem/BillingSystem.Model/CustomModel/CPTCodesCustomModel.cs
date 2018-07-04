using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class CPTCodesCustomModel
    {
        public int CPTCodesId { get; set; }
        public string CodeTableNumber { get; set; }
        public string CodeTableDescription { get; set; }
        public string CodeNumbering { get; set; }
        public string CodeDescription { get; set; }
        public string CodePrice { get; set; }
        public string CodeAnesthesiaBaseUnit { get; set; }
        public DateTime? CodeEffectiveDate { get; set; }
        public DateTime? CodeExpiryDate { get; set; }
        public string CodeBasicProductApplicationRule { get; set; }
        public string CodeOtherProductsApplicationRule { get; set; }
        public int? CodeServiceMainCategory { get; set; }
        public string CodeServiceCodeSubCategory { get; set; }
        public string CodeUSCLSChapter { get; set; }
        public string CodeCPTMUEValues { get; set; }
        public string CodeGroup { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsDeleted { get; set; }
        public int? DeletedBy { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? DeletedDate { get; set; }
        public long? CTPCodeRangeValue { get; set; }
        public string ExternalValue1 { get; set; }
        public string ExternalValue2 { get; set; }
        public string ExternalValue3 { get; set; }

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
