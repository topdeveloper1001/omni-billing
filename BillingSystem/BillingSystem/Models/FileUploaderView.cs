using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Models
{
    public class FileUploaderView
    {
        public IEnumerable<DocumentsTemplates> Attachments { get; set; }
        public DocumentsTemplates CurrentAttachment { get; set; }
        public List<OpenOrderCustomModel> OpenOrdersList { get; set; }
        public List<OpenOrderCustomModel> ClosedOrdersList { get; set; }
        public MedicalNotes CurrentMedicalNotes { get; set; }
        public List<MedicalNotesCustomModel> MedicalNotesList { get; set; }
        public List<OrderActivityCustomModel> OpenActvitiesList { get; set; }
        public List<OrderActivityCustomModel> ClosedActvitiesList { get; set; }
        public OrderActivity CurrentOrderActivity { get; set; }
        public OpenOrder EncounterOrder { get; set; }
        public string ViewType { get; set; }
        public DocumentsTemplatesCustomModel Model { get; set; }
    }
}