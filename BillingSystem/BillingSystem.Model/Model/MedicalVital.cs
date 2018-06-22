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
    
    public partial class MedicalVital
    {
        [Key]
        public int MedicalVitalID { get; set; }
        public int MedicalVitalType { get; set; }
        public Nullable<int> CorporateID { get; set; }
        public Nullable<int> FacilityID { get; set; }
        public Nullable<int> PatientID { get; set; }
        public Nullable<int> EncounterID { get; set; }
        public string MedicalRecordNumber { get; set; }
        public Nullable<int> GlobalCodeCategoryID { get; set; }
        public Nullable<int> GlobalCode { get; set; }
        public Nullable<decimal> AnswerValueMin { get; set; }
        public Nullable<decimal> AnswerValueMax { get; set; }
        public Nullable<int> AnswerUOM { get; set; }
        public string Comments { get; set; }
        public Nullable<int> CommentBy { get; set; }
        public Nullable<System.DateTime> CommentDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<int> DeletedBy { get; set; }
        public Nullable<System.DateTime> DeletedDate { get; set; }
    }
}
