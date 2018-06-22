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
    
    public class LoginTracking
    {
        [Key]
        public int LoginTrackingID { get; set; }
        public int ID { get; set; }
        public System.DateTime LoginTime { get; set; }
        public Nullable<System.DateTime> LogoutTime { get; set; }
        public string IPAddress { get; set; }
        public int LoginUserType { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<int> DeletedBy { get; set; }
        public Nullable<System.DateTime> DeletedDate { get; set; }
        public bool IsActive { get; set; }
        public Nullable<int> FacilityId { get; set; }
        public Nullable<int> CorporateId { get; set; }
    }
}
