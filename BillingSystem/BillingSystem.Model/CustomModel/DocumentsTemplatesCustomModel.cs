using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class DocumentsTemplatesCustomModel
    {
        public int DocumentsTemplatesID { get; set; }

        public int DocumentTypeID { get; set; }

        public string DocumentName { get; set; }

        public string DocumentNotes { get; set; }

        public int? AssociatedID { get; set; }

        public int AssociatedType { get; set; }

        public string FileName { get; set; }

        public string FilePath { get; set; }
        public bool IsTemplate { get; set; }
        public bool IsRequired { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool? IsDeleted { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? CorporateID { get; set; }
        public int? FacilityID { get; set; }
        public int? PatientID { get; set; }
        public int? EncounterID { get; set; }
        public string ExternalValue1 { get; set; }
        public string ExternalValue2 { get; set; }
        public string ExternalValue3 { get; set; }

        public string DocumentType { get; set; }
        public string EncounterNumber { get; set; }

        public string OldMedicalRecordSoruce { get; set; }
        public string ReferenceNumber { get; set; }

        public string GridType { get; set; }
        public string FileNameCustom { get; set; }

        [NotMapped]
        public HttpPostedFileBase File { get; set; }
    }
}
