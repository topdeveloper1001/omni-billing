using System.Web.Mvc;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common;
using BillingSystem.Model;
using BillingSystem.Models;
namespace BillingSystem.Controllers
{
    using System.Web.Mvc;
    using Bal.BusinessAccess;
    using Common;
    using Model;
    using Models;

    /// <summary>
    /// TechnicalSpecifications controller.
    /// </summary>
    public class TechnicalSpecificationsController : BaseController
    {

        private readonly ITechnicalSpecificationsService _service;

        public TechnicalSpecificationsController(ITechnicalSpecificationsService service)
        {
            _service = service;
        }
        #region Public Methods and Operators

        /// <summary>
        ///     Bind all the TechnicalSpecifications list
        /// </summary>
        /// <returns>action result with the partial view containing the TechnicalSpecifications list object</returns>
        [HttpPost]
        public ActionResult BindTechnicalSpecificationsList()
        {

            var corporateId = Helpers.GetSysAdminCorporateID();
            var facilityId = Helpers.GetDefaultFacilityId();
            // Get TechnicalSpecifications list
            var cateogiresList = _service.GetTechnicalSpecificationsData(corporateId, facilityId);


            // Pass the ActionResult with List of TechnicalSpecificationsViewModel object to Partial View TechnicalSpecificationsList
            return PartialView(PartialViews.TechnicalSpecificationsList, cateogiresList);
            
        }


        
        /// <summary>
        /// Delete the current TechnicalSpecification based on the TechnicalSpecification ID passed in the TechnicalSpecificationsModel
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult DeleteTechnicalSpecification(int id)
        {
            
            // Get currentTechnicalSpecification model object by current currentTechnicalSpecification ID
            var currentTechnicalSpecification = _service.GetTechnicalSpecificationById(id);

            // Check If currentTechnicalSpecification model is not null
            if (currentTechnicalSpecification != null)
            {

                // Update Operation of current currentTechnicalSpecification
                int result = _service.DeleteTechnicalSpecificationsData(currentTechnicalSpecification);


                // return deleted ID of current TechnicalSpecification as Json Result to the Ajax Call.
                return Json(result);
            }
            

            // Return the Json result as Action Result back JSON Call Success
            return Json(null);
        }

        /// <summary>
        /// Get the details of the current TechnicalSpecification in the view model by ID
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult GetTechnicalSpecification(int id)
        {
            
            // Call the AddTechnicalSpecifications Method to Add / Update current TechnicalSpecifications
            TechnicalSpecifications currentTechnicalSpecification = _service.GetTechnicalSpecificationById(id);

            // Pass the ActionResult with the current TechnicalSpecificationsViewModel object as model to PartialView TechnicalSpecificationsAddEdit
            return PartialView(PartialViews.TechnicalSpecificationsAddEdit, currentTechnicalSpecification);
            
        }

        /// <summary>
        /// Get the details of the TechnicalSpecifications View in the Model TechnicalSpecifications such as TechnicalSpecificationsList, list of
        ///     countries etc.
        /// </summary>
        /// <returns>
        /// returns the actionresult in the form of current object of the Model TechnicalSpecifications to be passed to View
        ///     TechnicalSpecifications
        /// </returns>
        public ActionResult TechnicalSpecificationsMain()
        {
            var corporateId = Helpers.GetSysAdminCorporateID();
            var facilityId = Helpers.GetDefaultFacilityId();
            // Get the Entity list
            var technicalSpecificationsList = _service.GetTechnicalSpecificationsData(corporateId, facilityId);

            // Intialize the View Model i.e. TechnicalSpecificationsView which is binded to Main View Index.cshtml under TechnicalSpecifications
            var technicalSpecificationsView = new TechnicalSpecificationsView
            {
                TechnicalSpecificationsList = technicalSpecificationsList,
                CurrentTechnicalSpecification = new TechnicalSpecifications()
            };

            // Pass the View Model in ActionResult to View TechnicalSpecifications
            return View(technicalSpecificationsView);
        }

        /// <summary>
        /// Reset the TechnicalSpecifications View Model and pass it to TechnicalSpecificationsAddEdit Partial View.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult ResetTechnicalSpecificationsForm()
        {
            // Intialize the new object of TechnicalSpecifications ViewModel
            var technicalSpecificationsViewModel = new TechnicalSpecifications();

            // Pass the View Model as TechnicalSpecificationsViewModel to PartialView TechnicalSpecificationsAddEdit just to update the AddEdit partial view.
            return PartialView(PartialViews.TechnicalSpecificationsAddEdit, technicalSpecificationsViewModel);
        }

        /// <summary>
        /// Add New or Update the TechnicalSpecifications based on if we pass the TechnicalSpecifications ID in the TechnicalSpecificationsViewModel
        ///     object.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// returns the newly added or updated ID of TechnicalSpecifications row
        /// </returns>
        public ActionResult SaveTechnicalSpecifications(TechnicalSpecifications model)
        {
            // Initialize the newId variable 
            int newId = -1;
            int userId = Helpers.GetLoggedInUserId();

            // Check if Model is not null 
            if (model != null)
            {
                var isExist = _service.CheckDuplicateTechnicalSpecification(model.Id, model.ItemID, model.CorporateId, model.FacilityId);

                if (isExist)
                    return Json("-1");

                if (model.Id > 0)
                {
                    model.ModifiedBy = userId;
                    model.ModifiedDate = Helpers.GetInvariantCultureDateTime();
                }
                else
                {
                    model.CreatedDate = Helpers.GetInvariantCultureDateTime();
                    model.CreatedBy = userId;
                }

                // Call the AddTechnicalSpecifications Method to Add / Update current TechnicalSpecifications
                newId = _service.SaveTechnicalSpecifications(model);
                
            }

            return Json(newId);
        }


        public ActionResult GetTechnicalSpecificationsData(int id)
        {

            var currentTechnicalSpecification = _service.GetTechnicalSpecificationById(id);
            var jsonResult = new
            {
                currentTechnicalSpecification.Id,
                currentTechnicalSpecification.FacilityId,
                currentTechnicalSpecification.CorporateId,
                currentTechnicalSpecification.ItemID,
                currentTechnicalSpecification.TechSpec
                
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);


        }

        /// <summary>
        /// Method is used to bind the technical specifications dropdown by passing facility id
        /// </summary>
        /// <param name="facilityId"></param>
        /// <returns></returns>
        public ActionResult GetTechnicalSpecificationsList(int facilityId)
        {
            var corporateId = Helpers.GetSysAdminCorporateID();
            // Initialize the AppointmentTypes BAL object
            var technicalSpecificationsList = _service.GetTechnicalSpecificationsData(corporateId, facilityId);

            return Json(technicalSpecificationsList, JsonRequestBehavior.AllowGet);

        }

        #endregion
    }
}
