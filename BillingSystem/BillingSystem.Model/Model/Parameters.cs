//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace BillingSystem.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Parameters
    {
        [Key]
        public int ParametersID { get; set; }
        public Nullable<int> CorporateID { get; set; }
        public Nullable<int> FacilityID { get; set; }
        public string FacilityNumber { get; set; }
        public Nullable<int> ParamLevel { get; set; }
        public string ParamName { get; set; }
        public string ParamDescription { get; set; }
        public Nullable<bool> ParamType { get; set; }
        public Nullable<int> ParamDataType { get; set; }
        public Nullable<int> IntValue1 { get; set; }
        public Nullable<int> IntValue2 { get; set; }
        public Nullable<decimal> NumValue1 { get; set; }
        public Nullable<decimal> NumValue2 { get; set; }
        public Nullable<System.DateTime> DatValue1 { get; set; }
        public Nullable<System.DateTime> DatValue2 { get; set; }
        public string StrValue1 { get; set; }
        public string StrValue2 { get; set; }
        public Nullable<bool> BitValue { get; set; }
        public string ExtValue1 { get; set; }
        public string ExtValue2 { get; set; }
        public string ExtValue3 { get; set; }
        public string ExtValue4 { get; set; }
        public Nullable<System.DateTime> EffectiveStartDate { get; set; }
        public Nullable<System.DateTime> EffectiveEndDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string SystemCode { get; set; }
    }
}
