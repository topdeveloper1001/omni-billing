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
    
    public partial class ReferencedIndicators
    {
        [Key]
        public int Id { get; set; }
        public Nullable<int> IndicatorNumberRef { get; set; }
        public Nullable<int> MainIndicatorNumbe { get; set; }
        public Nullable<int> CorporateId { get; set; }
    }
}
