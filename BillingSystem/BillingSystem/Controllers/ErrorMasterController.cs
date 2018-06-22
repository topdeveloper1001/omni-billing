using BillingSystem.Models;
using BillingSystem.Common;
using System.Web.Mvc;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Model;

namespace BillingSystem.Controllers
{
    public class ErrorMasterController : BaseController
    {
        /// <summary>
        /// Get the details of the ErrorMaster View in the Model ErrorMaster such as ErrorMasterList, list of countries etc.
        /// </summary>
        /// <returns>
        /// returns the actionresult in the form of current object of the Model ErrorMaster to be passed to View ErrorMaster
        /// </returns>
        public ActionResult Index()
        {
            //Initialize the ErrorMaster BAL object
            var errorMasterBal = new ErrorMasterBal();

            var cId = Helpers.GetDefaultCorporateId();
            var fId = Helpers.GetDefaultFacilityId();

            //Intialize the View Model i.e. ErrorMasterView which is binded to Main View Index.cshtml under ErrorMaster
            var errorMasterView = new ErrorMasterView
            {
                ErrorMasterList = errorMasterBal.GetErrorListByCorporateAndFacilityId(cId, fId, false),
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
            //Initialize the ErrorMaster BAL object
            using (var bal = new ErrorMasterBal())
            {
                //Get the facilities list
                var errorMasterList = bal.GetErrorListByCorporateAndFacilityId(Helpers.GetDefaultCorporateId(), Helpers.GetDefaultFacilityId(), showInActive);

                //Pass the ActionResult with List of ErrorMasterViewModel object to Partial View ErrorMasterList
                return PartialView(PartialViews.ErrorMasterList, errorMasterList);
            }
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
                using (var errorMasterBal = new ErrorMasterBal())
                {
                    if (model.ErrorMasterID > 0)
                    {
                        model.ModifiedBy = userId;
                        model.ModifiedDate = Helpers.GetInvariantCultureDateTime();
                        var errorMasterobj = errorMasterBal.GetErrorMasterById(model.ErrorMasterID);
                        model.CreatedBy = errorMasterobj.CreatedBy;
                        model.CreatedDate = errorMasterobj.CreatedDate;
                    }
                    else
                    {
                        model.CreatedBy = userId;
                        model.CreatedDate = Helpers.GetInvariantCultureDateTime();
                    }

                    //Call the AddErrorMaster Method to Add / Update current ErrorMaster
                    newId = errorMasterBal.AddUptdateErrorMaster(model);
                }
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
            using (var errorMasterBal = new ErrorMasterBal())
            {
                //Call the AddErrorMaster Method to Add / Update current ErrorMaster
                var currentErrorMaster = errorMasterBal.GetErrorMasterById(ErrorMasterID);

                //Pass the ActionResult with the current ErrorMasterViewModel object as model to PartialView ErrorMasterAddEdit
                return PartialView(PartialViews.ErrorMasterAddEdit, currentErrorMaster);
            }
        }

        /// <summary>
        /// Delete the current ErrorMaster based on the ErrorMaster ID passed in the ErrorMasterModel
        /// </summary>
        /// <param name="ErrorMasterID">The error master identifier.</param>
        /// <returns></returns>
        public ActionResult DeleteErrorMaster(int ErrorMasterID)
        {
            using (var bal = new ErrorMasterBal())
            {
                //Get ErrorMaster model object by current ErrorMaster ID
                var currentErrorMaster = bal.GetErrorMasterById(ErrorMasterID);

                //Check If ErrorMaster model is not null
                if (currentErrorMaster != null)
                {
                    currentErrorMaster.IsActive = false;
                    currentErrorMaster.ModifiedBy = Helpers.GetLoggedInUserId();
                    currentErrorMaster.ModifiedDate = Helpers.GetInvariantCultureDateTime();

                    //Update Operation of current ErrorMaster
                    var result = bal.AddUptdateErrorMaster(currentErrorMaster);

                    //return deleted ID of current ErrorMaster as Json Result to the Ajax Call.
                    return Json(result);
                }
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
            using (var errorMasterBal = new ErrorMasterBal())
            {
                //Call the AddErrorMaster Method to Add / Update current ErrorMaster
                var currentErrorMaster = errorMasterBal.GetErrorMasterById(ErrorMasterID);

                //Pass the ActionResult with the current ErrorMasterViewModel object as model to PartialView ErrorMasterAddEdit
                return Json(currentErrorMaster, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
