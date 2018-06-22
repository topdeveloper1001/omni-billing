using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Models
{
    public class MedicalNotesView
    {
        public MedicalNotes CurrentMedicalNotes { get; set; }
        public List<MedicalNotesCustomModel> MedicalNotesList { get; set; }
        public List<OpenOrderCustomModel> ClosedOrdersList { get; set; }
        public List<OpenOrderCustomModel> OpenOrdersList { get; set; }
        public List<OrderActivityCustomModel> OpenActvitiesList { get; set; }
        public List<OrderActivityCustomModel> ClosedActvitiesList { get; set; }
        public List<OrderActivityCustomModel> LabOpenOrdersActivitesList { get; set; }
        public List<OrderActivityCustomModel> ClosedLabOrdersActivitesList { get; set; }
        public OpenOrder EncounterOrder { get; set; }
        public bool IsLabTest { get; set; }

        public MedicalVitalCustomModel CurrentMedicalVital { get; set; }
        public List<MedicalVitalCustomModel> MedicalVitalList { get; set; }

        public List<PatientEvaluationCustomModel> EvaluationList { get; set; }

        public int PatientInfoId { get; set; }
        public int PatientEncounterId { get; set; }
        public List<PatientEvaluationSetCustomModel> EncounterList { get; set; }

        public List<DocumentsTemplates> NurseEnteredFormList { get; set; }

        public List<PatientEvaluationSetCustomModel> NurseDocList { get; set; }

        public List<OpenOrderCustomModel> LabOrders { get; set; }
    }
}
