using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;
using BillingSystem.Common.Common;

namespace BillingSystem.Bal.BusinessAccess
{
    public class MedicalNotesBal : BaseBal
    {
        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="medicalNotes"></param>
        /// <returns></returns>
        public int AddUptdateMedicalNotes(MedicalNotes medicalNotes)
        {
            using (var medicalNotesRep = UnitOfWork.MedicalNotesRepository)
            {
                if (medicalNotes.MedicalNotesID > 0)
                    medicalNotesRep.UpdateEntity(medicalNotes, medicalNotes.MedicalNotesID);
                else
                    medicalNotesRep.Create(medicalNotes);
                return medicalNotes.MedicalNotesID;
            }
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="medicalNotesId"></param>
        /// <returns></returns>
        public MedicalNotes GetMedicalNotesById(int? medicalNotesId)
        {
            using (var medicalNotesRep = UnitOfWork.MedicalNotesRepository)
            {
                var medicalNotes = medicalNotesRep.Where(x => x.MedicalNotesID == medicalNotesId).FirstOrDefault();
                return medicalNotes;
            }
        }

        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<MedicalNotesCustomModel> GetCustomMedicalNotes(int patientId, int notesUserTypeId)
        {
            var medicalNotesList = new List<MedicalNotesCustomModel>();
            using (var medicalNotesRep = UnitOfWork.MedicalNotesRepository)
            {
                var lstMedicalNotes = medicalNotesRep.Where(a => (a.IsDeleted == null || !(bool)a.IsDeleted)
                                                                 && a.PatientID == patientId
                                                                 && a.NotesUserType == notesUserTypeId)
                    .OrderByDescending(x => x.MedicalNotesID)
                    .ToList();

                medicalNotesList.AddRange(lstMedicalNotes.Select(item => new MedicalNotesCustomModel
                {
                    MedicalNotes = item,
                    NotesAddedBy = GetNameByUserId(item.NotesBy),
                    NotesUserTypeName = item.NotesUserType == Convert.ToInt32(NotesUserType.Physician)
                        ? (NotesUserType.Physician).ToString() : (NotesUserType.Nurse).ToString(),
                    NotesTypeName = GetNameByGlobalCodeValue(Convert.ToString(item.MedicalNotesType), Convert.ToString(Convert.ToInt32(GlobalCodeCategoryValue.NoteTypes)))
                }));

                //foreach (var item in lstMedicalNotes)
                //{
                //    using (var userbal = new UsersBal())
                //    {
                //        var medicalnotesCustomModel = new MedicalNotesCustomModel
                //        {

                //        };
                //        medicalNotesList.Add(medicalnotesCustomModel);
                //    }
                //}
            }
            return medicalNotesList;
        }

        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<MedicalNotesCustomModel> GetMedicalNotesByPatientId(int patientId)
        {
            var medicalNotesList = new List<MedicalNotesCustomModel>();
            using (var medicalNotesRep = UnitOfWork.MedicalNotesRepository)
            {
                var lstMedicalNotes = medicalNotesRep.Where(a => (a.IsDeleted == null || !(bool)a.IsDeleted)
                                                                 && a.PatientID == patientId).OrderByDescending(x => x.MedicalNotesID).ToList();
                medicalNotesList.AddRange(lstMedicalNotes.Select(item => new MedicalNotesCustomModel
                {
                    MedicalNotes = item,
                    NotesAddedBy = GetNameByUserId(item.NotesBy),
                    NotesUserTypeName = item.NotesUserType == Convert.ToInt32(NotesUserType.Physician)
                        ? (NotesUserType.Physician).ToString() : (NotesUserType.Nurse).ToString(),
                    NotesTypeName = GetNameByGlobalCodeValue(Convert.ToString(item.MedicalNotesType), Convert.ToString(Convert.ToInt32(GlobalCodeCategoryValue.NoteTypes)))
                }));
            }
            return medicalNotesList;
        }

        /// <summary>
        /// Gets the medical notes by patient identifier encounter identifier.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="currentEncounterId">The current encounter identifier.</param>
        /// <returns></returns>
        public List<MedicalNotesCustomModel> GetMedicalNotesByPatientIdEncounterId(int patientId, int currentEncounterId)
        {
            var medicalNotesList = new List<MedicalNotesCustomModel>();
            using (var medicalNotesRep = UnitOfWork.MedicalNotesRepository)
            {
                var lstMedicalNotes = medicalNotesRep.Where(a => (a.IsDeleted == null || !(bool)a.IsDeleted)
                                                                 && a.PatientID == patientId && a.EncounterID == currentEncounterId).OrderByDescending(x => x.MedicalNotesID).ToList();
                medicalNotesList.AddRange(lstMedicalNotes.Select(item => new MedicalNotesCustomModel
                {
                    MedicalNotes = item,
                    NotesAddedBy = GetNameByUserId(item.NotesBy),
                    NotesUserTypeName = item.NotesUserType == Convert.ToInt32(NotesUserType.Physician)
                        ? (NotesUserType.Physician).ToString() : (NotesUserType.Nurse).ToString(),
                    NotesTypeName = GetNameByGlobalCodeValue(Convert.ToString(item.MedicalNotesType), Convert.ToString(Convert.ToInt32(GlobalCodeCategoryValue.NoteTypes)))
                }));
            }
            return medicalNotesList;
        }
    }
}

