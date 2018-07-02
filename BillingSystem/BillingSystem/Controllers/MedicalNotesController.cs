using BillingSystem.Model;
using BillingSystem.Models;
using BillingSystem.Common;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using BillingSystem.Model.CustomModel;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Controllers
{
    public class MedicalNotesController : BaseController
    {
        private readonly IMedicalNotesService _service;

        public MedicalNotesController(IMedicalNotesService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get the details of the MedicalNotes View in the Model MedicalNotes such as MedicalNotesList, list of countries etc.
        /// </summary>
        /// <returns>
        /// returns the actionresult in the form of current object of the Model MedicalNotes to be passed to View MedicalNotes
        /// </returns>
        public ActionResult MedicalNotesMain()
        {
            //Initialize the MedicalNotes BAL object
            //var _service = new MedicalNotesBal();

            ////Get the Entity list
            //var medicalNotesList = _service.GetMedicalNotes();

            //Intialize the View Model i.e. MedicalNotesView which is binded to Main View Index.cshtml under MedicalNotes
            var medicalNotesView = new MedicalNotesView
            {
                MedicalNotesList = new List<MedicalNotesCustomModel>(),
                CurrentMedicalNotes = new MedicalNotes()
            };

            //Pass the View Model in ActionResult to View MedicalNotes
            return View(medicalNotesView);
        }

        /// <summary>
        /// Bind all the MedicalNotes list
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="notesUserTypeId">Type of the medical notes.</param>
        /// <returns>
        /// action result with the partial view containing the MedicalNotes list object
        /// </returns>
        public ActionResult BindMedicalNotesList(int patientId, int notesUserTypeId)
        {
            var medicalNotesList = _service.GetCustomMedicalNotes(patientId, notesUserTypeId);
            return PartialView(PartialViews.MedicalNotesList, medicalNotesList);
        }

        /// <summary>
        /// Add New or Update the MedicalNotes based on if we pass the MedicalNotes ID in the MedicalNotesViewModel object.
        /// </summary>
        /// <param name="medicalNotesModel">pass the details of MedicalNotes in the view model</param>
        /// <returns>
        /// returns the newly added or updated ID of MedicalNotes row
        /// </returns>
        public ActionResult SaveMedicalNotes(MedicalNotes medicalNotesModel)
        {
            //Initialize the newId variable 
            var newId = -1;
            //corporate Id
            var corporateId = Helpers.GetSysAdminCorporateID();
            //Facility Id
            var facilityId = Helpers.GetDefaultFacilityId();
            //User Id
            var userId = Helpers.GetLoggedInUserId();
            var currentdateTime = Helpers.GetInvariantCultureDateTime();
            if (medicalNotesModel != null)
            {
                medicalNotesModel.FacilityID = facilityId;
                medicalNotesModel.CorporateID = corporateId;
                medicalNotesModel.NotesBy = userId;
                medicalNotesModel.NotesDate = currentdateTime;
                if (medicalNotesModel.MedicalNotesID > 0)
                {
                    medicalNotesModel.ModifiedBy = userId;
                    medicalNotesModel.ModifiedDate = currentdateTime;
                }
                else
                {
                    medicalNotesModel.NotesBy = userId;
                    medicalNotesModel.NotesDate = currentdateTime;
                }
                //Call the AddMedicalNotes Method to Add / Update current MedicalNotes
                newId = _service.AddUptdateMedicalNotes(medicalNotesModel);
            }
            return Json(newId);
        }

        /// <summary>
        /// Get the details of the current MedicalNotes in the view model by ID
        /// </summary>
        /// <param name="medicalNotesId">The medical notes identifier.</param>
        /// <returns></returns>
        public ActionResult GetMedicalNotes(int medicalNotesId)
        {
            var currentMedicalNotes = _service.GetMedicalNotesById(Convert.ToInt32(medicalNotesId));
            return PartialView(PartialViews.MedicalNotesAddEdit, currentMedicalNotes);
        }

        /// <summary>
        /// Delete the current MedicalNotes based on the MedicalNotes ID passed in the MedicalNotesModel
        /// </summary>
        /// <param name="medicalNotesId">The medical notes identifier.</param>
        /// <returns></returns>
        public ActionResult DeleteMedicalNotes(int medicalNotesId)
        {
            var currentMedicalNotes = _service.GetMedicalNotesById(Convert.ToInt32(medicalNotesId));
            if (currentMedicalNotes != null)
            {
                currentMedicalNotes.IsDeleted = true;
                currentMedicalNotes.DeletedBy = Helpers.GetLoggedInUserId();
                currentMedicalNotes.DeletedDate = Helpers.GetInvariantCultureDateTime();

                var result = _service.AddUptdateMedicalNotes(currentMedicalNotes);
                return Json(result);
            }

            //Return the Json result as Action Result back JSON Call Success
            return Json(null);
        }

        /// <summary>
        /// Reset the MedicalNotes View Model and pass it to MedicalNotesAddEdit Partial View.
        /// </summary>
        /// <returns></returns>
        public ActionResult ResetMedicalNotesForm()
        {
            //Intialize the new object of MedicalNotes ViewModel
            var medicalNotesViewModel = new MedicalNotes();

            //Pass the View Model as MedicalNotesViewModel to PartialView MedicalNotesAddEdit just to update the AddEdit partial view.
            return PartialView(PartialViews.MedicalNotesAddEdit, medicalNotesViewModel);
        }
    }
}
