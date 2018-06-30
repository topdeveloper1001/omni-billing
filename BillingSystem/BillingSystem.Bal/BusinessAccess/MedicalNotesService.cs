using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;
using BillingSystem.Common.Common;
using BillingSystem.Repository.Interfaces;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class MedicalNotesService : IMedicalNotesService
    {
        private readonly IRepository<MedicalNotes> _repository;
        private readonly IRepository<GlobalCodes> _gRepository;
        private readonly IRepository<Users> _uRepository;

        public MedicalNotesService(IRepository<MedicalNotes> repository, IRepository<GlobalCodes> gRepository, IRepository<Users> uRepository)
        {
            _repository = repository;
            _gRepository = gRepository;
            _uRepository = uRepository;
        }


        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="medicalNotes"></param>
        /// <returns></returns>
        public int AddUptdateMedicalNotes(MedicalNotes medicalNotes)
        {
            if (medicalNotes.MedicalNotesID > 0)
                _repository.UpdateEntity(medicalNotes, medicalNotes.MedicalNotesID);
            else
                _repository.Create(medicalNotes);
            return medicalNotes.MedicalNotesID;
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="medicalNotesId"></param>
        /// <returns></returns>
        public MedicalNotes GetMedicalNotesById(int? medicalNotesId)
        {
            var medicalNotes = _repository.Where(x => x.MedicalNotesID == medicalNotesId).FirstOrDefault();
            return medicalNotes;
        }

        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<MedicalNotesCustomModel> GetCustomMedicalNotes(int patientId, int notesUserTypeId)
        {
            var medicalNotesList = new List<MedicalNotesCustomModel>();
            var lstMedicalNotes = _repository.Where(a => (a.IsDeleted == null || !(bool)a.IsDeleted)
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

            return medicalNotesList;
        }
        private string GetNameByUserId(int? UserID)
        {
            var usersModel = _uRepository.Where(x => x.UserID == UserID && x.IsDeleted == false).FirstOrDefault();
            return usersModel != null ? usersModel.FirstName + " " + usersModel.LastName : string.Empty;
        }

        private string GetNameByGlobalCodeValue(string codeValue, string categoryValue, string fId = "")
        {
            if (!string.IsNullOrEmpty(codeValue))
            {
                var gl = _gRepository.Where(g => g.GlobalCodeValue.Equals(codeValue) && !g.IsDeleted.Value && g.GlobalCodeCategoryValue.Equals(categoryValue) && (string.IsNullOrEmpty(fId) || g.FacilityNumber.Equals(fId))).FirstOrDefault();
                return gl != null ? gl.GlobalCodeName : string.Empty;
            }
            return string.Empty;
        }
        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<MedicalNotesCustomModel> GetMedicalNotesByPatientId(int patientId)
        {
            var medicalNotesList = new List<MedicalNotesCustomModel>();
            var lstMedicalNotes = _repository.Where(a => (a.IsDeleted == null || !(bool)a.IsDeleted)
                                                             && a.PatientID == patientId).OrderByDescending(x => x.MedicalNotesID).ToList();
            medicalNotesList.AddRange(lstMedicalNotes.Select(item => new MedicalNotesCustomModel
            {
                MedicalNotes = item,
                NotesAddedBy = GetNameByUserId(item.NotesBy),
                NotesUserTypeName = item.NotesUserType == Convert.ToInt32(NotesUserType.Physician)
                    ? (NotesUserType.Physician).ToString() : (NotesUserType.Nurse).ToString(),
                NotesTypeName = GetNameByGlobalCodeValue(Convert.ToString(item.MedicalNotesType), Convert.ToString(Convert.ToInt32(GlobalCodeCategoryValue.NoteTypes)))
            }));
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
            var lstMedicalNotes = _repository.Where(a => (a.IsDeleted == null || !(bool)a.IsDeleted)
                                                             && a.PatientID == patientId && a.EncounterID == currentEncounterId).OrderByDescending(x => x.MedicalNotesID).ToList();
            medicalNotesList.AddRange(lstMedicalNotes.Select(item => new MedicalNotesCustomModel
            {
                MedicalNotes = item,
                NotesAddedBy = GetNameByUserId(item.NotesBy),
                NotesUserTypeName = item.NotesUserType == Convert.ToInt32(NotesUserType.Physician)
                    ? (NotesUserType.Physician).ToString() : (NotesUserType.Nurse).ToString(),
                NotesTypeName = GetNameByGlobalCodeValue(Convert.ToString(item.MedicalNotesType), Convert.ToString(Convert.ToInt32(GlobalCodeCategoryValue.NoteTypes)))
            }));
            return medicalNotesList;
        }
    }
}

