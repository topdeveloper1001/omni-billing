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
    
    public partial class OperatingRoom
    {
        [Key]
        public int Id { get; set; }
        public Nullable<int> PatientId { get; set; }
        public Nullable<int> EncounterId { get; set; }
        public Nullable<int> OperatingType { get; set; }
        public Nullable<System.DateTime> StartDay { get; set; }
        public Nullable<System.DateTime> EndDay { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public Nullable<decimal> CalculatedHours { get; set; }
        public string CodeValue { get; set; }
        public string CodeValueType { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> Modifieddate { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<int> DeletedBy { get; set; }
        public Nullable<System.DateTime> DeletedDate { get; set; }
        public string Status { get; set; }
        public Nullable<int> CorporateID { get; set; }
        public Nullable<int> FacilityId { get; set; }
    }
}
