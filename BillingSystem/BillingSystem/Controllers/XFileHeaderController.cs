using System.Linq;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Common;
using BillingSystem.Model;
using BillingSystem.Models;
using System;
using System.Web.Mvc;

namespace BillingSystem.Controllers
{
    public class XFileHeaderController : BaseController
    {
        /// <summary>
        /// Get the details of the XFileHeader View in the Model XFileHeader such as XFileHeaderList, list of countries etc.
        /// </summary>
        /// <param name="fileid">The fileid.</param>
        /// <returns>
        /// returns the actionresult in the form of current object of the Model XFileHeader to be passed to View XFileHeader
        /// </returns>
        public ActionResult XFileHeaderMain(int fileid)
        {
            //Initialize the XFileHeader BAL object
            var xFileHeaderBal = new XFileHeaderService();

            //Get the Entity list
            var xFileHeaderList = xFileHeaderBal.GetXFileHeader().Where(x => x.FileType.Equals("IN")).ToList();

            //Intialize the View Model i.e. XFileHeaderView which is binded to Main View Index.cshtml under XFileHeader
            var xFileHeaderView = new XFileHeaderView
            {
               XFileHeaderList = xFileHeaderList,
               CurrentXFileHeader = new Model.XFileHeader()
            };

            //Pass the View Model in ActionResult to View XFileHeader
            return View(xFileHeaderView);
        }

        /// <summary>
        /// Bind all the XFileHeader list 
        /// </summary>
        /// <returns>action result with the partial view containing the XFileHeader list object</returns>
        [HttpPost]
        public ActionResult BindXFileHeaderList()
        {
            //Initialize the XFileHeader BAL object
            using (var XFileHeaderBal = new XFileHeaderService())
            {
                //Get the facilities list
                var XFileHeaderList = XFileHeaderBal.GetXFileHeader();

                //Pass the ActionResult with List of XFileHeaderViewModel object to Partial View XFileHeaderList
                return PartialView(PartialViews.XFileHeaderList, XFileHeaderList);
            }
        }

        /// <summary>
        /// Add New or Update the XFileHeader based on if we pass the XFileHeader ID in the XFileHeaderViewModel object.
        /// </summary>
        /// <param name="XFileHeaderModel">pass the details of XFileHeader in the view model</param>
        /// <returns>returns the newly added or updated ID of XFileHeader row</returns>
        public ActionResult SaveXFileHeader(XFileHeader model)
        {
            //Initialize the newId variable 
            var newId = -1;
			var userId = Helpers.GetLoggedInUserId();

            //Check if Model is not null 
            if (model != null)
            {
                using (var bal = new XFileHeaderService())
                {
                    if (model.FileID > 0)
                    {
                        model.ModifiedBy = userId.ToString();
                        model.ModifiedDate = Helpers.GetInvariantCultureDateTime();
                    }
                    //Call the AddXFileHeader Method to Add / Update current XFileHeader
                    newId = bal.SaveXFileHeader(model);
                }
            }
            return Json(newId);
        }

        /// <summary>
        /// Get the details of the current XFileHeader in the view model by ID 
        /// </summary>
        /// <param name="shared">pass the input parameters such as ID</param>
        /// <returns></returns>
        public ActionResult GetXFileHeader(int id)
        {
            using (var bal = new XFileHeaderService())
            {
                //Call the AddXFileHeader Method to Add / Update current XFileHeader
                var currentXFileHeader = bal.GetXFileHeaderByID(id);

                //Pass the ActionResult with the current XFileHeaderViewModel object as model to PartialView XFileHeaderAddEdit
                return PartialView(PartialViews.XFileHeaderAddEdit, currentXFileHeader);
            }
        }

        /// <summary>
        /// Delete the current XFileHeader based on the XFileHeader ID passed in the XFileHeaderModel
        /// </summary>
        /// <param name="shared"></param>
        /// <returns></returns>
        public ActionResult DeleteXFileHeader(int id)
        {
            using (var bal = new XFileHeaderService())
            {
                //Get XFileHeader model object by current XFileHeader ID
                var currentXFileHeader = bal.GetXFileHeaderByID(id);
				var userId = Helpers.GetLoggedInUserId();

                //Check If XFileHeader model is not null
                if (currentXFileHeader != null)
                {
                    currentXFileHeader.ModifiedBy = userId.ToString();
                    currentXFileHeader.ModifiedDate = CurrentDateTime;

                    //Update Operation of current XFileHeader
                    var result = bal.SaveXFileHeader(currentXFileHeader);

                    //return deleted ID of current XFileHeader as Json Result to the Ajax Call.
                    return Json(result);
                }
            }

            //Return the Json result as Action Result back JSON Call Success
            return Json(null);
        }

        /// <summary>
        /// Reset the XFileHeader View Model and pass it to XFileHeaderAddEdit Partial View. 
        /// </summary>
        /// <param name="shared">pass the input parameters such as ID</param>
        /// <returns></returns>
        public ActionResult ResetXFileHeaderForm()
        {
            //Intialize the new object of XFileHeader ViewModel
            var XFileHeaderViewModel = new Model.XFileHeader();

            //Pass the View Model as XFileHeaderViewModel to PartialView XFileHeaderAddEdit just to update the AddEdit partial view.
            return PartialView(PartialViews.XFileHeaderAddEdit, XFileHeaderViewModel);
        }
    }
}
