using BillingSystem.Model;
using BillingSystem.Models;
using BillingSystem.Common;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Controllers
{
    public class MedicalNotesController : BaseController
    {
        /// <summary>
        /// Get the details of the MedicalNotes View in the Model MedicalNotes such as MedicalNotesList, list of countries etc.
        /// </summary>
        /// <returns>
        /// returns the actionresult in the form of current object of the Model MedicalNotes to be passed to View MedicalNotes
        /// </returns>
        public ActionResult MedicalNotesMain()
        {
            //Initialize the MedicalNotes BAL object
            //var medicalNotesBal = new MedicalNotesBal();

            ////Get the Entity list
            //var medicalNotesList = medicalNotesBal.GetMedicalNotes();

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
            //Initialize the MedicalNotes BAL object
            using (var medicalNotesBal = new MedicalNotesBal())
            {
                //Get the facilities list
                //var medicalNotesList = medicalNotesBal.GetMedicalNotes();
                var medicalNotesList = medicalNotesBal.GetCustomMedicalNotes(patientId, notesUserTypeId);

                //Pass the ActionResult with List of MedicalNotesViewModel object to Partial View MedicalNotesList
                return PartialView(PartialViews.MedicalNotesList, medicalNotesList);
            }
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
                using (var medicalNotesBal = new MedicalNotesBal())
                {
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
                    newId = medicalNotesBal.AddUptdateMedicalNotes(medicalNotesModel);
                }
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
            using (var medicalNotesBal = new MedicalNotesBal())
            {
                //Call the AddMedicalNotes Method to Add / Update current MedicalNotes
                var currentMedicalNotes = medicalNotesBal.GetMedicalNotesById(Convert.ToInt32(medicalNotesId));

                //If the view is shown in ViewMode only, then ViewBag.ViewOnly is set to true otherwise false.
                //ViewBag.ViewOnly = !string.IsNullOrEmpty(model.ViewOnly);

                //Pass the ActionResult with the current MedicalNotesViewModel object as model to PartialView MedicalNotesAddEdit
                return PartialView(PartialViews.MedicalNotesAddEdit, currentMedicalNotes);
            }
        }

        /// <summary>
        /// Delete the current MedicalNotes based on the MedicalNotes ID passed in the MedicalNotesModel
        /// </summary>
        /// <param name="medicalNotesId">The medical notes identifier.</param>
        /// <returns></returns>
        public ActionResult DeleteMedicalNotes(int medicalNotesId)
        {
            using (var medicalNotesBal = new MedicalNotesBal())
            {
                //Get MedicalNotes model object by current MedicalNotes ID
                var currentMedicalNotes = medicalNotesBal.GetMedicalNotesById(Convert.ToInt32(medicalNotesId));

                //Check If MedicalNotes model is not null
                if (currentMedicalNotes != null)
                {
                    currentMedicalNotes.IsDeleted = true;
                    currentMedicalNotes.DeletedBy = Helpers.GetLoggedInUserId();
                    currentMedicalNotes.DeletedDate = Helpers.GetInvariantCultureDateTime();

                    //Update Operation of current MedicalNotes
                    var result = medicalNotesBal.AddUptdateMedicalNotes(currentMedicalNotes);

                    //return deleted ID of current MedicalNotes as Json Result to the Ajax Call.
                    return Json(result);
                }
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
