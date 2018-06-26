// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ATCCodesController.cs" company="Spadez Solutions PVT. LTD.">
//    ServicesDotCom
// </copyright>
// <summary>
//   The atc codes controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Controllers
{
    using System.Web.Mvc;

    using Bal.BusinessAccess;
    using BillingSystem.Bal.Interfaces;
    using Common;
    using Model;
    using Models;

    /// <summary>
    /// The atc codes controller.
    /// </summary>
    public class ATCCodesController : BaseController
    {

        private readonly IATCCodesService _Service;

        public ATCCodesController(IATCCodesService Service)
        {
            _Service = Service;
        }

        /// <summary>
        /// Get the details of the ATCCodes View in the Model ATCCodes such as ATCCodesList, list of countries etc.
        /// </summary>
        /// <returns>
        /// returns the ActionResult in the form of current object of the Model ATCCodes to be passed to View ATCCodes
        /// </returns>
        public ActionResult ATCCodesMain()
        {
            // Get the Entity list
            var atcCodesList = _Service.GetATCCodes();

            // Intialize the View Model i.e. ATCCodesView which is binded to Main View Index.cshtml under ATCCodes
            var atcCodesView = new ATCCodesView
            {
                ATCCodesList = atcCodesList,
                CurrentATCCodes = new Model.ATCCodes()
            };

            // Pass the View Model in ActionResult to View ATCCodes
            return View(atcCodesView);
        }

        /// <summary>
        /// Bind all the ATCCodes list
        /// </summary>
        /// <returns>
        /// action result with the partial view containing the ATCCodes list object
        /// </returns>
        [HttpPost]
        public ActionResult BindATCCodesList()
        {
            // Get the facilities list
            var atcCodesList = _Service.GetATCCodes();

            // Pass the ActionResult with List of ATCCodesViewModel object to Partial View ATCCodesList
            return PartialView(PartialViews.ATCCodesList, atcCodesList);
        }

        /// <summary>
        /// Add New or Update the ATCCodes based on if we pass the ATCCodes ID in the ATCCodesViewModel object.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// returns the newly added or updated ID of ATCCodes row
        /// </returns>
        public ActionResult SaveATCCodes(ATCCodes model)
        {
            // Initialize the newId variable 
            var newId = -1;

            // Check if Model is not null 
            if (model != null)
            {
                model.CodeTableNumber = Helpers.DefaultCptTableNumber;
                model.IsActive = true;
                if (model.ATCCodeID > 0)
                {
                    model.ModifiedBy = Helpers.GetLoggedInUserId();
                    model.ModifiedDate = Helpers.GetInvariantCultureDateTime();
                }
                else
                {
                    model.CreatedBy = Helpers.GetLoggedInUserId();
                    model.CreatedDate = Helpers.GetInvariantCultureDateTime();
                }


                // Call the AddATCCodes Method to Add / Update current ATCCodes
                newId = _Service.SaveATCCodes(model);

            }

            return Json(newId);
        }

        /// <summary>
        /// Get the details of the current ATCCodes in the view model by ID
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult GetATCCodes(int id)
        {
            // Call the AddATCCodes Method to Add / Update current ATCCodes
            var currentAtcCodes = _Service.GetATCCodesByID(id);

            // Pass the ActionResult with the current ATCCodesViewModel object as model to PartialView ATCCodesAddEdit
            return PartialView(PartialViews.ATCCodesAddEdit, currentAtcCodes);

        }

        /// <summary>
        /// Delete the current ATCCodes based on the ATCCodes ID passed in the ATCCodesModel
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult DeleteATCCodes(int id)
        {
            // Get ATCCodes model object by current ATCCodes ID
            var currentAtcCodes = _Service.GetATCCodesByID(id);

            // Check If ATCCodes model is not null
            if (currentAtcCodes != null)
            {
                // Update Operation of current ATCCodes
                var deleteAtc = _Service.DeleteATCCode(currentAtcCodes);

                // var result = bal.SaveATCCodes(currentATCCodes);

                // return deleted ID of current ATCCodes as Json Result to the Ajax Call.
                return Json(deleteAtc);
            }


            // Return the Json result as Action Result back JSON Call Success
            return Json(null);
        }

        /// <summary>
        /// Reset the ATCCodes View Model and pass it to ATCCodesAddEdit Partial View.
        /// </summary>
        /// <returns></returns>
        public ActionResult ResetATCCodesForm()
        {
            // Intialize the new object of ATCCodes ViewModel
            var atcCodesViewModel = new ATCCodes();

            // Pass the View Model as ATCCodesViewModel to PartialView ATCCodesAddEdit just to update the AddEdit partial view.
            return PartialView(PartialViews.ATCCodesAddEdit, atcCodesViewModel);
        }
    }
}
