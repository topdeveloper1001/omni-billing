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
    
    public partial class BillHeader
    {
        [Key]
        public int BillHeaderID { get; set; }
        public Nullable<int> EncounterID { get; set; }
        public string BillNumber { get; set; }
        public Nullable<int> PatientID { get; set; }
        public Nullable<int> FacilityID { get; set; }
        public string PayerID { get; set; }
        public string MemberID { get; set; }
        public Nullable<decimal> Gross { get; set; }
        public Nullable<decimal> PatientShare { get; set; }
        public Nullable<decimal> PayerShareNet { get; set; }
        public Nullable<System.DateTime> BillDate { get; set; }
        public string Status { get; set; }
        public string DenialCode { get; set; }
        public string PaymentReference { get; set; }
        public Nullable<System.DateTime> DateSettlement { get; set; }
        public Nullable<decimal> PaymentAmount { get; set; }
        public string PatientPayReference { get; set; }
        public Nullable<System.DateTime> PatientDateSettlement { get; set; }
        public Nullable<decimal> PatientPayAmount { get; set; }
        public Nullable<long> ClaimID { get; set; }
        public Nullable<long> FileID { get; set; }
        public Nullable<long> ARFileID { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<int> DeletedBy { get; set; }
        public Nullable<System.DateTime> DeletedDate { get; set; }
        public Nullable<int> CorporateID { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
        public Nullable<int> AuthID { get; set; }
        public string AuthCode { get; set; }
        public Nullable<int> MCID { get; set; }
        public Nullable<decimal> MCMultiplier { get; set; }
        public Nullable<decimal> MCPatientShare { get; set; }
        public Nullable<bool> EncounterSelfPayFlag { get; set; }

        public Nullable<decimal> MCDiscount { get; set; }

        public Nullable<decimal> ActivityCost { get; set; }
    }
}
