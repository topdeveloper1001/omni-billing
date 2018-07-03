using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IMedicalNotesService
    {
        int AddUptdateMedicalNotes(MedicalNotes medicalNotes);
        List<MedicalNotesCustomModel> GetCustomMedicalNotes(int patientId, int notesUserTypeId);
        MedicalNotes GetMedicalNotesById(int? medicalNotesId);
        List<MedicalNotesCustomModel> GetMedicalNotesByPatientId(int patientId);
        List<MedicalNotesCustomModel> GetMedicalNotesByPatientIdEncounterId(int patientId, int currentEncounterId);
    }
}