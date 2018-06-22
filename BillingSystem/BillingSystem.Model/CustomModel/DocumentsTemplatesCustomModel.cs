using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class DocumentsTemplatesCustomModel : DocumentsTemplates
    {
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
