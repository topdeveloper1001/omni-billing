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

    public class MedicalNecessity
    {
        [Key]
        public int Id { get; set; }

        public string BillingCode { get; set; }

        public string BillingCodeType { get; set; }

        public string ICD9Code { get; set; }

        public bool? IsActive { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string ExtValue1 { get; set; }

        public string ExtValue2 { get; set; }

        public string ExtValue3 { get; set; }

    }

}
