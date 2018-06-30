using BillingSystem.Models;
using BillingSystem.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Controllers
{
    public class LabTestOrderSetController : BaseController
    {
        /// <summary>
        /// Get the details of the LabTestOrderSet View in the Model LabTestOrderSet such as LabTestOrderSetList, list of countries etc.
        /// </summary>
        /// <returns>returns the actionresult in the form of current object of the Model LabTestOrderSet to be passed to View LabTestOrderSet</returns>
        public ActionResult Index()
        {
            //Initialize the LabTestOrderSet BAL object
            using (var bal = new LabTestOrderSetService())
            {
                //Get the Entity list
                var list = bal.GetLabOrderSetList();

                //Intialize the View Model i.e. LabTestOrderSetView which is binded to Main View Index.cshtml under LabTestOrderSet
                var view = new LabTestOrderSetViews
                {
                    LabTestOrderSetList = list,
                    CurrentLabTestOrderSet = new LabTestOrderSet
                    {
                        IsDeleted = false
                    }
                };

                //Pass the View Model in ActionResult to View LabTestOrderSet
                return View(view);
            }
        }

        /// <summary>
        /// Bind all the LabTestOrderSet list 
        /// </summary>
        /// <returns>action result with the partial view containing the LabTestOrderSet list object</returns>
        [HttpPost]
        public ActionResult BindLabTestOrderSetList()
        {
            //Initialize the LabTestOrderSet BAL object
            using (var bal = new LabTestOrderSetService())
            {
                //Get the facilities list
                var list = bal.GetLabOrderSetList();

                //Pass the ActionResult with List of LabTestOrderSetViewModel object to Partial View LabTestOrderSetList
                return PartialView(PartialViews.LabTestOrderSetList, list);
            }
        }

        /// <summary>
        /// Add New or Update the LabTestOrderSet based on if we pass the LabTestOrderSet ID in the LabTestOrderSetViewModel object.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>returns the newly added or updated ID of LabTestOrderSet row</returns>
        public ActionResult SaveLabTestOrderSet(LabTestOrderSet model)
        {
            var list = new List<LabTestOrderSetCustomModel>();

            //Initialize the newId variable 
            var userId = Helpers.GetLoggedInUserId();
            var currenDate = Helpers.GetInvariantCultureDateTime();
            //Check if Model is not null 
            if (model != null)
            {
                using (var bal = new LabTestOrderSetService())
                {
                    if (model.Id > 0)
                    {
                        model.ModifiedBy = userId;
                        model.ModifiedDate = currenDate;
                    }
                    else
                    {
                        model.CreatedBy = userId;
                        model.CreatedDate = currenDate;
                    }
                    //Call the AddLabTestOrderSet Method to Add / Update current LabTestOrderSet
                    list = bal.SaveLabTestOrderSet(model);
                }
            }
            return PartialView(PartialViews.LabTestOrderSetListView, list);
        }

        /// <summary>
        /// Get the details of the current LabTestOrderSet in the view model by ID 
        /// </summary>
        /// <returns></returns>
        public JsonResult GetLabTestOrderSet(int id)
        {
            using (var bal = new LabTestOrderSetService())
            {
                //Call the AddLabTestOrderSet Method to Add / Update current LabTestOrderSet
                var current = bal.GetDetailById(id);

                //Pass the ActionResult with the current LabTestOrderSetViewModel object as model to PartialView LabTestOrderSetAddEdit
                return Json(current);
            }
        }

        /// <summary>
        /// Delete the current LabTestOrderSet based on the LabTestOrderSet ID passed in the LabTestOrderSetModel
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteLabTestOrderSet(int id)
        {
            using (var bal = new LabTestOrderSetService())
            {
                //Get LabTestOrderSet model object by current LabTestOrderSet ID
                var currentLabTestOrderSet = bal.GetDetailById(id);
                var userId = Helpers.GetLoggedInUserId();
                var currentDate = Helpers.GetInvariantCultureDateTime();
                //Check If LabTestOrderSet model is not null
                if (currentLabTestOrderSet != null)
                {
                    currentLabTestOrderSet.ModifiedBy = userId;
                    currentLabTestOrderSet.ModifiedDate = currentDate;
                    currentLabTestOrderSet.IsDeleted = true;
                    currentLabTestOrderSet.DeletedBy = userId;
                    currentLabTestOrderSet.DeletedDate = currentDate;

                    //Update Operation of current LabTestOrderSet
                    var list = bal.SaveLabTestOrderSet(currentLabTestOrderSet);

                    //return deleted ID of current LabTestOrderSet as Json Result to the Ajax Call.
                    return PartialView(PartialViews.LabTestOrderSetListView, list);
                }
            }

            //Return the Json result as Action Result back JSON Call Success
            return Json(null);
        }


        public ActionResult SortLabTestOrderList()
        {
            using (var bal = new LabTestOrderSetService())
            {
               var list = bal.GetLabOrderSetList();
                //return deleted ID of current LabTestOrderSet as Json Result to the Ajax Call.
                    return PartialView(PartialViews.LabTestOrderSetListView, list);
                }
        }


    }
}
