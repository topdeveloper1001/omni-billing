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
    
    public partial class LabTestResult
    {
        [Key]
        public int LabTestResultID { get; set; }
        public string LabTestResultTableNumber { get; set; }
        public string LabTestResultTableName { get; set; }
        public Nullable<int> LabTestResultCPTCode { get; set; }
        public string LabTestResultTestName { get; set; }
        public string LabTestResultSpecimen { get; set; }
        public string LabTestResultGender { get; set; }
        public string LabTestResultAgeFrom { get; set; }
        public string LabTestResultAgeTo { get; set; }
        public string LabTestResultMeasurementValue { get; set; }
        public Nullable<decimal> LabTestResultLowRangeResult { get; set; }
        public Nullable<decimal> LabTestResultHighRangeResult { get; set; }
        public Nullable<decimal> LabTestResultGoodFrom { get; set; }
        public Nullable<decimal> LabTestResultGoodTo { get; set; }
        public Nullable<decimal> LabTestResultCautionFrom { get; set; }
        public Nullable<decimal> LabTestResultCautionTo { get; set; }
        public Nullable<decimal> LabTestResultBadFrom { get; set; }
        public Nullable<decimal> LabTestResultBadTo { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> Modifieddate { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<int> DeletedBy { get; set; }
        public Nullable<System.DateTime> DeletedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CorporateId { get; set; }
        public Nullable<int> FacilityId { get; set; }
    }
}
