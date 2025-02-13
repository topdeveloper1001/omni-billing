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
    
    public partial class MedicalNotes
    {
        [Key]
        public int MedicalNotesID { get; set; }
        public int MedicalNotesType { get; set; }
        public int NotesUserType { get; set; }
        public Nullable<int> CorporateID { get; set; }
        public Nullable<int> FacilityID { get; set; }
        public Nullable<int> PatientID { get; set; }
        public Nullable<int> EncounterID { get; set; }
        public string MedicalRecordNumber { get; set; }
        public string Notes { get; set; }
        public Nullable<int> NotesBy { get; set; }
        public Nullable<System.DateTime> NotesDate { get; set; }
        public bool MarkedComplication { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<int> DeletedBy { get; set; }
        public Nullable<System.DateTime> DeletedDate { get; set; }
    }
}
