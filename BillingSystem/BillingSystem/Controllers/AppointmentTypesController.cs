﻿using System.Web.Mvc;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common;
using BillingSystem.Model;
using BillingSystem.Models;


namespace BillingSystem.Controllers
{
  

    /// <summary>
    /// AppointmentTypes controller.
    /// </summary>
    public class AppointmentTypesController : BaseController
    {
        private readonly IAppointmentTypesService _service;

        public AppointmentTypesController(IAppointmentTypesService service)
        {
            _service = service;
        }
        #region Public Methods and Operators

        /// <summary>
        ///     Bind all the AppointmentTypes list
        /// </summary>
        /// <returns>action result with the partial view containing the AppointmentTypes list object</returns>
        [HttpPost]
        public ActionResult BindAppointmentTypesList(bool showInActive)
        {
            var corporateId = Helpers.GetSysAdminCorporateID();
            var facilityId = Helpers.GetDefaultFacilityId();
            // Initialize the AppointmentTypes BAL object
            var appointmentTypesList = _service.GetAppointmentTypesData(corporateId, facilityId, showInActive);
            return PartialView(PartialViews.AppointmentTypesList, appointmentTypesList);

        }


        public ActionResult BindAppointmentGridActiveInActive(int corporateId, int facilityId, bool showInActive)
        {
            var list = _service.GetAppointmentTypesData(corporateId, facilityId, showInActive);
            return PartialView(PartialViews.AppointmentTypesList, list);

        }
        /// <summary>
        /// Delete the current AppointmentTypes based on the AppointmentTypes ID passed in the AppointmentTypesModel
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult DeleteAppointmentTypes(int id)
        {
            // Get AppointmentTypes model object by current AppointmentTypes ID
            var currentAppointmentTypes = _service.GetAppointmentTypesById(id);

            // Check If AppointmentTypes model is not null
            if (currentAppointmentTypes != null)
            {
                currentAppointmentTypes.IsActive = false;

                // Update Operation of current AppointmentTypes
                int result = _service.DeleteAppointmentTyepsData(currentAppointmentTypes);


                // return deleted ID of current AppointmentTypes as Json Result to the Ajax Call.
                return Json(result);
            }


            // Return the Json result as Action Result back JSON Call Success
            return Json(null);
        }

        /// <summary>
        /// Get the details of the current AppointmentTypes in the view model by ID
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult GetAppointmentTypes(int id)
        {
            AppointmentTypes currentAppointmentTypes = _service.GetAppointmentTypesById(id);

            // Pass the ActionResult with the current AppointmentTypesViewModel object as model to PartialView AppointmentTypesAddEdit
            return PartialView(PartialViews.AppointmentTypesAddEdit, currentAppointmentTypes);

        }

        /// <summary>
        /// Get the details of the AppointmentTypes View in the Model AppointmentTypes such as AppointmentTypesList, list of
        ///     countries etc.
        /// </summary>
        /// <returns>
        /// returns the actionresult in the form of current object of the Model AppointmentTypes to be passed to View
        ///     AppointmentTypes
        /// </returns>
        public ActionResult AppointmentTypesMain()
        {
            var corporateId = Helpers.GetSysAdminCorporateID();
            var facilityId = Helpers.GetDefaultFacilityId();

            // Get the Entity list
            var appointmentTypesList = _service.GetAppointmentTypesData(corporateId, facilityId, true);

            // Intialize the View Model i.e. AppointmentTypesView which is binded to Main View Index.cshtml under AppointmentTypes
            var appointmentTypesView = new AppointmentTypesView
            {
                AppointmentTypesList = appointmentTypesList,
                CurrentAppointmentTypes = new AppointmentTypes()
            };

            // Pass the View Model in ActionResult to View AppointmentTypes
            return View(appointmentTypesView);
        }

        /// <summary>
        /// Reset the AppointmentTypes View Model and pass it to AppointmentTypesAddEdit Partial View.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult ResetAppointmentTypesForm()
        {
            // Intialize the new object of AppointmentTypes ViewModel
            var appointmentTypesViewModel = new AppointmentTypes();

            // Pass the View Model as AppointmentTypesViewModel to PartialView AppointmentTypesAddEdit just to update the AddEdit partial view.
            return PartialView(PartialViews.AppointmentTypesAddEdit, appointmentTypesViewModel);
        }

        /// <summary>
        /// Add New or Update the AppointmentTypes based on if we pass the AppointmentTypes ID in the AppointmentTypesViewModel
        ///     object.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// returns the newly added or updated ID of AppointmentTypes row
        /// </returns>
        public ActionResult SaveAppointmentTypes(AppointmentTypes model)
        {
            // Initialize the newId variable 
            int newId = -1;
            int userId = Helpers.GetLoggedInUserId();

            // Check if Model is not null 
            if (model != null)
            {
                var isExist = _service.CheckDuplicateAppointmentType(model.Id, model.Name, model.CorporateId, model.FacilityId);

                if (isExist)
                    return Json("-1");
                var categoryIsExist = _service.CheckDuplicateCategoryNumber(model.Id, model.CategoryNumber, model.CorporateId, model.FacilityId);
                if (categoryIsExist)
                    return Json("-2");

                if (model.Id > 0)
                {
                    model.ModifiedBy = userId;
                    model.ModifiedDate = Helpers.GetInvariantCultureDateTime();
                }
                else
                {
                    model.CraetedDate = Helpers.GetInvariantCultureDateTime();
                    model.CreatedBy = userId;
                }

                // Call the AddAppointmentTypes Method to Add / Update current AppointmentTypes
                newId = _service.SaveAppointmentTypes(model);

            }

            return Json(newId);
        }


        public ActionResult GetAppointmentData(int id)
        {
            var currentappointmnet = _service.GetAppointmentTypesById(id);
            var jsonResult = new
            {
                currentappointmnet.Id,
                currentappointmnet.FacilityId,
                currentappointmnet.CorporateId,
                currentappointmnet.CptRangeFrom,
                currentappointmnet.CptRangeTo,
                currentappointmnet.CategoryNumber,
                currentappointmnet.DefaultTime,
                currentappointmnet.Description,
                currentappointmnet.IsActive,
                currentappointmnet.Name,
                currentappointmnet.ExtValue1,
                currentappointmnet.ExtValue2

            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Method is used to bind the appointment type dropdown by passing facility id
        /// </summary>
        /// <param name="facilityId"></param>
        /// <returns></returns>
        public ActionResult GetAppointmentTypesList(int facilityId)
        {
            var corporateId = Helpers.GetSysAdminCorporateID();
            // Initialize the AppointmentTypes BAL object
            var appointmentTypesList = _service.GetAppointmentTypesData(corporateId, facilityId, true);

            return Json(appointmentTypesList, JsonRequestBehavior.AllowGet);

        }


        public ActionResult GetMaxCategoryNumber(int corporateId, int facilityId)
        {
            int categoryNumber = _service.GetMaxCategoryNumber(facilityId, corporateId);
            return Json(categoryNumber);

        }

        #endregion
    }
}
