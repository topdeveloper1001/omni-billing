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
    
    public partial class XEncounter
    {
        [Key]
        public long XEncounterID { get; set; }
        public long EncounterID { get; set; }
        public string IDPayer { get; set; }
        public string MemberID { get; set; }
        public string PayerID { get; set; }
        public string ProviderID { get; set; }
        public string EmiratesIDNumber { get; set; }
        public Nullable<decimal> Gross { get; set; }
        public Nullable<decimal> PatientShare { get; set; }
        public Nullable<decimal> Net { get; set; }
        public string FacilityID { get; set; }
        public string FType { get; set; }
        public string PatientID { get; set; }
        public string EligibilityIDPayer { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public string StartType { get; set; }
        public string EndType { get; set; }
        public string TransferSource { get; set; }
        public string TransferDestination { get; set; }
        public string DenialCode { get; set; }
        public string PaymentReference { get; set; }
        public Nullable<System.DateTime> DateSettlement { get; set; }
        public Nullable<decimal> PaymentAmount { get; set; }
        public string PatientPayReference { get; set; }
        public Nullable<System.DateTime> PatientDateSettlement { get; set; }
        public Nullable<decimal> PatientPayAmount { get; set; }
        public string Status { get; set; }
        public Nullable<long> FileID { get; set; }
        public Nullable<long> ARFileID { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<long> ClaimID { get; set; }
    }
}
