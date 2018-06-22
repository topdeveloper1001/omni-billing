// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HolidayPlannerController.cs" company="SPadez">
//   OmniHealthcare
// </copyright>
// <summary>
//   The holiday planner controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Controllers
{
    using System.Web.Mvc;

    using BillingSystem.Bal.BusinessAccess;
    using BillingSystem.Common;
    using BillingSystem.Model;
    using BillingSystem.Models;

    /// <summary>
    /// PreSchedulingLink controller.
    /// </summary>
    public class PreSchedulingLinkController : BaseController
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Bind all the PreSchedulingLink list
        /// </summary>
        /// <returns>action result with the partial view containing the PreSchedulingLink list object</returns>
        public ActionResult BindPreSchedulingLinkList()
        {
            // Initialize the PreSchedulingLink BAL object
            using (var preSchedulingLinkBal = new PreSchedulingLinkBal())
            {
                var cId = Helpers.GetDefaultCorporateId();
                var fId = Helpers.GetDefaultFacilityId();

                // Get the facilities list
                var preSchedulingLinkList = preSchedulingLinkBal.GetPreSchedulingLink(cId, fId);

                // Pass the ActionResult with List of PreSchedulingLinkViewModel object to Partial View PreSchedulingLinkList
                return this.PartialView(PartialViews.PreSchedulingLinkList, preSchedulingLinkList);
            }
        }

        /// <summary>
        /// Delete the current PreSchedulingLink based on the PreSchedulingLink ID passed in the PreSchedulingLinkModel
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult DeletePreSchedulingLink(int id)
        {
            using (var bal = new PreSchedulingLinkBal())
            {
                // Get PreSchedulingLink model object by current PreSchedulingLink ID
                var currentPreSchedulingLink = bal.GetPreSchedulingLinkById(id);
                var userId = Helpers.GetLoggedInUserId();
                bal.DeletePreSchedulingLink(currentPreSchedulingLink);
            }

            // Return the Json result as Action Result back JSON Call Success
            return this.Json(true);
        }

        /// <summary>
        /// Get the details of the current PreSchedulingLink in the view model by ID
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult GetPreSchedulingLink(int id)
        {
            using (var bal = new PreSchedulingLinkBal())
            {
                // Call the AddPreSchedulingLink Method to Add / Update current PreSchedulingLink
                var currentPreSchedulingLink = bal.GetPreSchedulingLinkById(id);

                // Pass the ActionResult with the current PreSchedulingLinkViewModel object as model to PartialView PreSchedulingLinkAddEdit
                return Json(currentPreSchedulingLink, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Get the details of the PreSchedulingLink View in the Model PreSchedulingLink such as PreSchedulingLinkList, list of
        ///     countries etc.
        /// </summary>
        /// <returns>
        /// returns the actionresult in the form of current object of the Model PreSchedulingLink to be passed to View
        ///     PreSchedulingLink
        /// </returns>
        public ActionResult Index()
        {
            // Initialize the PreSchedulingLink BAL object
            var preSchedulingLinkBal = new PreSchedulingLinkBal();

            var cId = Helpers.GetDefaultCorporateId();
            var fId = Helpers.GetDefaultFacilityId();

            // Get the Entity list
            var preSchedulingLinkList = preSchedulingLinkBal.GetPreSchedulingLink(cId, fId);

            // Intialize the View Model i.e. PreSchedulingLinkView which is binded to Main View Index.cshtml under PreSchedulingLink
            var preSchedulingLinkView = new PreSchedulingLinkView
                                         {
                                             PreSchedulingLinkList = preSchedulingLinkList, 
                                             CurrentPreSchedulingLink = new PreSchedulingLink()
                                         };

            // Pass the View Model in ActionResult to View PreSchedulingLink
            return View(preSchedulingLinkView);
        }

        /// <summary>
        /// Reset the PreSchedulingLink View Model and pass it to PreSchedulingLinkAddEdit Partial View.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult ResetPreSchedulingLinkForm()
        {
            // Intialize the new object of PreSchedulingLink ViewModel
            var preSchedulingLinkViewModel = new PreSchedulingLink();

            // Pass the View Model as PreSchedulingLinkViewModel to PartialView PreSchedulingLinkAddEdit just to update the AddEdit partial view.
            return this.PartialView(PartialViews.PreSchedulingLinkAddEdit, preSchedulingLinkViewModel);
        }

        /// <summary>
        /// Add New or Update the PreSchedulingLink based on if we pass the PreSchedulingLink ID in the PreSchedulingLinkViewModel
        ///     object.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// returns the newly added or updated ID of PreSchedulingLink row
        /// </returns>
        public ActionResult SavePreSchedulingLink(PreSchedulingLink model)
        {
            // Initialize the newId variable 
            var newId = -1;
            var userId = Helpers.GetLoggedInUserId();
            var datetime = Helpers.GetInvariantCultureDateTime();

            // Check if Model is not null 
            if (model != null)
            {
                using (var bal = new PreSchedulingLinkBal())
                {
                    if (model.Id > 0)
                    {
                        model.Modifiedby = userId;
                        model.ModifiedDate = datetime;
                    }
                    else
                    {
                        model.CreatedBy = userId;
                        model.CreatedDate = datetime;
                        var objpreviousRecordId = bal.CheckforPreviousData(model.CorporateId, model.FacilityId);
                        if (objpreviousRecordId != null && objpreviousRecordId.Id != 0)
                        {
                            model.Modifiedby = userId;
                            model.ModifiedDate = datetime;
                            model.CreatedBy = objpreviousRecordId.CreatedBy;
                            model.CreatedDate = objpreviousRecordId.CreatedDate;
                            model.Id = objpreviousRecordId.Id; 
                        }
                    }

                    // Call the AddPreSchedulingLink Method to Add / Update current PreSchedulingLink
                    newId = bal.SavePreSchedulingLink(model);
                }
            }

            return this.Json(newId);
        }

        #endregion
    }
}
