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
    
    public partial class HCPCSCodes
    {
        [Key]
        public int HCPCSCodesId { get; set; }
        public string CodeTableNumber { get; set; }
        public string CodeTableDescription { get; set; }
        public string CodeNumbering { get; set; }
        public string CodeDescription { get; set; }
        public string CodePrice { get; set; }
        public Nullable<System.DateTime> CodeEffectiveDate { get; set; }
        public Nullable<System.DateTime> CodeExpiryDate { get; set; }
        public string CodeBasicProductApplicationRule { get; set; }
        public string CodeOtherProductsApplicationRule { get; set; }
        public Nullable<int> CodeServiceMainCategory { get; set; }
        public string CodeServiceCodeSubCategory { get; set; }
        public string CodeCPTMUEValues { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<int> DeletedBy { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> DeletedDate { get; set; }
    }
}
