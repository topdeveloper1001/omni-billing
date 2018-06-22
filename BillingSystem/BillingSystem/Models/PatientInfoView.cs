using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Models
{
    public class PatientInfoView
    {
        //public PatientInfo CurrentPatient { get; set; }
        public PatientInfoCustomModel CurrentPatient { get; set; }
        public PatientLoginDetailCustomModel PatientLoginDetail { get; set; }

        public PatientInsuranceCustomModel Insurance { get; set; }

        //public IEnumerable<DocumentsTemplates> Attachments { get; set; }
        //public DocumentsTemplates CurrentAttachment { get; set; }

        public IEnumerable<EncounterCustomModel> Encounters { get; set; }

        public PatientAddressRelation CurrentPatientAddressRelation { get; set; }
        public IEnumerable<PatientAddressRelationCustomModel> PatientAddressRealtionList { get; set; }

        public PatientPhone CurrentPhone { get; set; }
        //public IEnumerable<PatientPhoneCustomModel> Phonelst { get; set; }

        public int PatientId { get; set; }
        public bool EncounterOpen { get; set; }
        public int EncounterOpenType { get; set; }
        public DocumentsView PatientDocumentsView { get; set; }
        public PhonesView PatientPhoneView { get; set; }
    }

    public class DocumentsView
    {
        public IEnumerable<DocumentsTemplates> Attachments { get; set; }
        public DocumentsTemplates CurrentAttachment { get; set; }
    }

    public class PhonesView
    {
        public PatientPhone CurrentPhone { get; set; }
        public IEnumerable<PatientPhoneCustomModel> Phonelst { get; set; }
    }
}