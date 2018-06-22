
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class MedicalNotesCustomModel
    {
        public MedicalNotes MedicalNotes { get; set; }
        public string NotesAddedBy { get; set; }
        public string NotesTypeName { get; set; }
        public string NotesUserTypeName { get; set; }
        public int MedicalNotesID2 { get; set; }
    }
}
