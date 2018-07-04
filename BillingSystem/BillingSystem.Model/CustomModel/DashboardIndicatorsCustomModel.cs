using System;

namespace BillingSystem.Model.CustomModel
{
    public class DashboardIndicatorsCustomModel
    {
        public int IndicatorID { get; set; }
        public string IndicatorNumber { get; set; }
        public string Dashboard { get; set; }
        public string Description { get; set; }
        public string Defination { get; set; }
        public string SubCategory1 { get; set; }
        public string SubCategory2 { get; set; }
        public int? FormatType { get; set; }
        public string DecimalNumbers { get; set; }
        public int? FerquencyType { get; set; }
        public string OwnerShip { get; set; }
        public int? FacilityId { get; set; }
        public int? CorporateId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? IsActive { get; set; }
        public string ExternalValue1 { get; set; }
        public string ExternalValue2 { get; set; }
        public string ExternalValue3 { get; set; }
        public string ReferencedIndicators { get; set; }
        public string ExternalValue4 { get; set; }
        public string ExternalValue5 { get; set; }
        public string ExternalValue6 { get; set; }
        public int? SortOrder { get; set; }
        public string ExpressionText { get; set; }
        public string ExpressionValue { get; set; }
        public int SpecialCase { get; set; }

        public string FacilityNameStr { get; set; }
        public string UsernameStr { get; set; }
        public string DashboardTypeStr { get; set; }
        public string FormatTypeStr { get; set; }
        public string FerquencyTypeStr { get; set; }
        public string SubCategoryFirst { get; set; }
        public string SubCategorySecond { get; set; }
        public string TypeOfData { get; set; }
    }
}
