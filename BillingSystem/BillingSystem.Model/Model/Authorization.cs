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
    
    public partial class Authorization
    {
        [Key]
        public int AuthorizationID { get; set; }
        public Nullable<int> CorporateID { get; set; }
        public Nullable<int> FacilityID { get; set; }
        public Nullable<int> PatientID { get; set; }
        public string EncounterID { get; set; }
        public Nullable<System.DateTime> AuthorizationDateOrdered { get; set; }
        public Nullable<System.DateTime> AuthorizationStart { get; set; }
        public Nullable<System.DateTime> AuthorizationEnd { get; set; }
        public string AuthorizationCode { get; set; }
        public Nullable<int> AuthorizationType { get; set; }
        public string AuthorizationComments { get; set; }
        public Nullable<int> AuthorizationDenialCode { get; set; }
        public string AuthorizationIDPayer { get; set; }
        public Nullable<decimal> AuthorizationLimit { get; set; }
        public string AuthorizationMemberID { get; set; }
        public Nullable<int> AuthorizationResult { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<int> DeletedBy { get; set; }
        public Nullable<System.DateTime> DeletedDate { get; set; }
        public string AuthorizedServiceLevel { get; set; }
    }
}
