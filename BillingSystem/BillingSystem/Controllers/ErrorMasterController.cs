using BillingSystem.Models;
using BillingSystem.Common;
using System.Web.Mvc;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Model;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Controllers
{
    public class ErrorMasterController : BaseController
    {
        private readonly IErrorMasterService _service;

        public ErrorMasterController(IErrorMasterService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get the details of the ErrorMaster View in the Model ErrorMaster such as ErrorMasterList, list of countries etc.
        /// </summary>
        /// <returns>
        /// returns the actionresult in the form of current object of the Model ErrorMaster to be passed to View ErrorMaster
        /// </returns>
        public ActionResult Index()
        {

            var cId = Helpers.GetDefaultCorporateId();
            var fId = Helpers.GetDefaultFacilityId();

            //Intialize the View Model i.e. ErrorMasterView which is binded to Main View Index.cshtml under ErrorMaster
            var errorMasterView = new ErrorMasterView
            {
                ErrorMasterList = _service.GetErrorListByCorporateAndFacilityId(cId, fId, false),
                CurrentErrorMaster = new ErrorMaster { IsActive = true }
            };

            //Pass the View Model in ActionResult to View ErrorMaster
            return View(errorMasterView);
        }

        /// <summary>
        /// Bind all the ErrorMaster list
        /// </summary>
        /// <returns>
        /// action result with the partial view containing the ErrorMaster list object
        /// </returns>
        [HttpPost]
        public ActionResult BindErrorMasterList(bool? showInActive)
        {
            //Get the facilities list
            var errorMasterList = _service.GetErrorListByCorporateAndFacilityId(Helpers.GetDefaultCorporateId(), Helpers.GetDefaultFacilityId(), showInActive);

            //Pass the ActionResult with List of ErrorMasterViewModel object to Partial View ErrorMasterList
            return PartialView(PartialViews.ErrorMasterList, errorMasterList);
        }

        /// <summary>
        /// Add New or Update the ErrorMaster based on if we pass the ErrorMaster ID in the ErrorMasterViewModel object.
        /// </summary>
        /// <param name="model">pass the details of ErrorMaster in the view model</param>
        /// <returns>
        /// returns the newly added or updated ID of ErrorMaster row
        /// </returns>
        public ActionResult SaveErrorMaster(ErrorMaster model)
        {
            //Initialize the newId variable 
            var newId = -1;
            var userId = Helpers.GetLoggedInUserId();

            //Check if ErrorMasterViewModel 
            if (model != null)
            {
                if (model.ErrorMasterID > 0)
                {
                    model.ModifiedBy = userId;
                    model.ModifiedDate = Helpers.GetInvariantCultureDateTime();
                    var errorMasterobj = _service.GetErrorMasterById(model.ErrorMasterID);
                    model.CreatedBy = errorMasterobj.CreatedBy;
                    model.CreatedDate = errorMasterobj.CreatedDate;
                }
                else
                {
                    model.CreatedBy = userId;
                    model.CreatedDate = Helpers.GetInvariantCultureDateTime();
                }

                //Call the AddErrorMaster Method to Add / Update current ErrorMaster
                newId = _service.AddUptdateErrorMaster(model);
            }
            return Json(newId);
        }

        /// <summary>
        /// Get the details of the current ErrorMaster in the view model by ID
        /// </summary>
        /// <param name="ErrorMasterID">The error master identifier.</param>
        /// <returns></returns>
        public ActionResult GetErrorMaster(int ErrorMasterID)
        {
            //Call the AddErrorMaster Method to Add / Update current ErrorMaster
            var currentErrorMaster = _service.GetErrorMasterById(ErrorMasterID);

            //Pass the ActionResult with the current ErrorMasterViewModel object as model to PartialView ErrorMasterAddEdit
            return PartialView(PartialViews.ErrorMasterAddEdit, currentErrorMaster);
        }

        /// <summary>
        /// Delete the current ErrorMaster based on the ErrorMaster ID passed in the ErrorMasterModel
        /// </summary>
        /// <param name="ErrorMasterID">The error master identifier.</param>
        /// <returns></returns>
        public ActionResult DeleteErrorMaster(int ErrorMasterID)
        {
            var currentErrorMaster = _service.GetErrorMasterById(ErrorMasterID);

            //Check If ErrorMaster model is not null
            if (currentErrorMaster != null)
            {
                currentErrorMaster.IsActive = false;
                currentErrorMaster.ModifiedBy = Helpers.GetLoggedInUserId();
                currentErrorMaster.ModifiedDate = Helpers.GetInvariantCultureDateTime();

                //Update Operation of current ErrorMaster
                var result = _service.AddUptdateErrorMaster(currentErrorMaster);

                //return deleted ID of current ErrorMaster as Json Result to the Ajax Call.
                return Json(result);
            }

            //Return the Json result as Action Result back JSON Call Success
            return Json(null);
        }

        /// <summary>
        /// Reset the ErrorMaster View Model and pass it to ErrorMasterAddEdit Partial View.
        /// </summary>
        /// <returns></returns>
        public ActionResult ResetErrorMasterForm()
        {
            //Intialize the new object of ErrorMaster ViewModel
            var errorMasterViewModel = new Model.ErrorMaster();

            //Pass the View Model as ErrorMasterViewModel to PartialView ErrorMasterAddEdit just to update the AddEdit partial view.
            return PartialView(PartialViews.ErrorMasterAddEdit, errorMasterViewModel);
        }

        /// <summary>
        /// Gets the denial code by identifier.
        /// </summary>
        /// <param name="ErrorMasterID">The error master identifier.</param>
        /// <returns></returns>
        public ActionResult GetDenialCodeById(int ErrorMasterID)
        {
            var currentErrorMaster = _service.GetErrorMasterById(ErrorMasterID);
            return Json(currentErrorMaster, JsonRequestBehavior.AllowGet);
        }
    }
}
