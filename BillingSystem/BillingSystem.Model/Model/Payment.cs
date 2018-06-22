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
    
    public partial class Payment
    {
        [Key]
        public long PaymentID { get; set; }
        public Nullable<int> PayType { get; set; }
        public Nullable<System.DateTime> PayDate { get; set; }
        public Nullable<decimal> PayAmount { get; set; }
        public string PayReference { get; set; }
        public string PayBillNumber { get; set; }
        public Nullable<int> PayFor { get; set; }
        public Nullable<int> PayBy { get; set; }
        public Nullable<int> PayBillID { get; set; }
        public Nullable<int> PayActivityID { get; set; }
        public Nullable<int> PayEncounterID { get; set; }
        public Nullable<int> PayFacilityID { get; set; }
        public Nullable<int> PayCorporateID { get; set; }
        public Nullable<long> PayXAFileHeaderID { get; set; }
        public Nullable<long> PayXAAdviceID { get; set; }
        public Nullable<decimal> PayNETAmount { get; set; }
        public string PayXADenialCode { get; set; }
        public Nullable<int> PayStatus { get; set; }
        public Nullable<decimal> PayAppliedAmount { get; set; }
        public Nullable<decimal> PayUnAppliedAmount { get; set; }
        public Nullable<int> PayAppliedStatus { get; set; }
        public Nullable<int> PayCreatedBy { get; set; }
        public Nullable<System.DateTime> PayCreatedDate { get; set; }
        public Nullable<int> PayModifiedBy { get; set; }
        public Nullable<System.DateTime> PayModifiedDate { get; set; }
        public Nullable<int> PayIsActive { get; set; }
        public int? PaymentTypeId { get; set; }
    }
}
