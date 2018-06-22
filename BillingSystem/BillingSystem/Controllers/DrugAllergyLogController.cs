using BillingSystem.Models;
using BillingSystem.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Controllers
{
    public class DrugAllergyLogController : BaseController
    {
        /// <summary>
        /// Get the details of the DrugAllergyLog View in the Model DrugAllergyLog such as DrugAllergyLogList, list of countries etc.
        /// </summary>
        /// <returns>
        /// returns the actionresult in the form of current object of the Model DrugAllergyLog to be passed to View DrugAllergyLog
        /// </returns>
        public ActionResult Index()
        {
            //Initialize the DrugAllergyLog BAL object
            using (var bal = new DrugAllergyLogBal())
            {

                //Get the Entity list
                var list = bal.GetDrugAllergyLogList(Helpers.GetSysAdminCorporateID(),Helpers.GetDefaultFacilityId());

                //Intialize the View Model i.e. DrugAllergyLogView which is binded to Main View Index.cshtml under DrugAllergyLog
                var viewModel = new DrugAllergyLogView
                {
                    DrugAllergyLogList = list,
                    CurrentDrugAllergyLog = new DrugAllergyLog()
                };

                //Pass the View Model in ActionResult to View DrugAllergyLog
                return View(viewModel);
            }
        }

        /// <summary>
        /// Add New or Update the DrugAllergyLog based on if we pass the DrugAllergyLog ID in the DrugAllergyLogViewModel object.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// returns the newly added or updated ID of DrugAllergyLog row
        /// </returns>
        public ActionResult SaveDrugAllergyLog(DrugAllergyLog model)
        {
            //Initialize the newId variable 
            var userId = Helpers.GetLoggedInUserId();
            var currentDate = Helpers.GetInvariantCultureDateTime();
            var list = new List<DrugAllergyLogCustomModel>();

            //Check if Model is not null 
            if (model != null)
            {
                using (var bal = new DrugAllergyLogBal())
                {
                    if (model.Id > 0)
                    {
                        model.OrderBy = userId;
                        model.OrderedDate = currentDate;
                    }
                    else
                    {
                        model.CreatedBy = userId;
                        model.CreatedDate = currentDate;
                    }

                    //Call the AddDrugAllergyLog Method to Add / Update current DrugAllergyLog
                    list = bal.SaveDrugAllergyLog(model);
                }
            }
            return PartialView(PartialViews.DrugAllergyLogList, list);
            //Pass the ActionResult with List of DrugAllergyLogViewModel object to Partial View DrugAllergyLogList
        }

        /// <summary>
        /// Saves the drug allergy log custom.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public ActionResult SaveDrugAllergyLogCustom(DrugAllergyLog model)
        {
            //Initialize the newId variable 
            var issaved = false;
            var userId = Helpers.GetLoggedInUserId();
            var currentDate = Helpers.GetInvariantCultureDateTime();
            var facilityid = Helpers.GetDefaultFacilityId();
            var corporateid = Helpers.GetSysAdminCorporateID();
            //Check if Model is not null 
            if (model != null)
            {
                using (var bal = new DrugAllergyLogBal())
                {
                    model.OrderBy = userId;
                    model.OrderedDate = currentDate;
                    model.CreatedBy = userId;
                    model.CreatedDate = currentDate;
                    model.FacilityId = facilityid;
                    model.CorporateId = corporateid;
                    //Call the AddDrugAllergyLog Method to Add / Update current DrugAllergyLog
                    issaved = bal.SaveDrugAllergyLogCustom(model);
                }
            }
            return Json(issaved, JsonRequestBehavior.AllowGet);
            //Pass the ActionResult with List of DrugAllergyLogViewModel object to Partial View DrugAllergyLogList
        }

        /// <summary>
        /// Get the details of the current DrugAllergyLog in the view model by ID
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public JsonResult GetDrugAllergyLogDetails(int id)
        {
            using (var bal = new DrugAllergyLogBal())
            {
                //Call the AddDrugAllergyLog Method to Add / Update current DrugAllergyLog
                var current = bal.GetDrugAllergyLogById(id);
                //Pass the ActionResult with the current DrugAllergyLogViewModel object as model to PartialView DrugAllergyLogAddEdit
                return Json(current);
            }
        }

        /// <summary>
        /// Delete the current DrugAllergyLog based on the DrugAllergyLog ID passed in the DrugAllergyLogModel
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult DeleteDrugAllergyLog(int id)
        {
            using (var bal = new DrugAllergyLogBal())
            {
                //Get DrugAllergyLog model object by current DrugAllergyLog ID
                var model = bal.GetDrugAllergyLogById(id);
                var userId = Helpers.GetLoggedInUserId();
                var list = new List<DrugAllergyLogCustomModel>();
                var currentDate = Helpers.GetInvariantCultureDateTime();

                //Check If DrugAllergyLog model is not null
                if (model != null)
                {
                    //Update Operation of current DrugAllergyLog
                    list = bal.SaveDrugAllergyLog(model);
                    //return deleted ID of current DrugAllergyLog as Json Result to the Ajax Call.
                }
                return PartialView(PartialViews.DrugAllergyLogList, list);
            }
            //Pass the ActionResult with List of DrugAllergyLogViewModel object to Partial View DrugAllergyLogList
        }
    }
}
