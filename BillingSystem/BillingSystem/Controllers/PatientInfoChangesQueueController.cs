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
    public class PatientInfoChangesQueueController : BaseController
    {
        /// <summary>
        /// Get the details of the PatientInfoChangesQueue View in the Model PatientInfoChangesQueue such as PatientInfoChangesQueueList, list of countries etc.
        /// </summary>
        /// <returns>
        /// returns the actionresult in the form of current object of the Model PatientInfoChangesQueue to be passed to View PatientInfoChangesQueue
        /// </returns>
        public ActionResult Index()
        {
            //Initialize the PatientInfoChangesQueue BAL object
            using (var bal = new PatientInfoChangesQueueBal())
            {
                //Get the Entity list
                var list = bal.GetPatientInfoChangesQueueList();

                //Intialize the View Model i.e. PatientInfoChangesQueueView which is binded to Main View Index.cshtml under PatientInfoChangesQueue
                var viewModel = new PatientInfoChangesQueueView
                {
                    PatientInfoChangesQueueList = list,
                    CurrentPatientInfoChangesQueue = new PatientInfoChangesQueue()
                };

                //Pass the View Model in ActionResult to View PatientInfoChangesQueue
                return View(viewModel);
            }
        }

        /// <summary>
        /// Add New or Update the PatientInfoChangesQueue based on if we pass the PatientInfoChangesQueue ID in the PatientInfoChangesQueueViewModel object.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// returns the newly added or updated ID of PatientInfoChangesQueue row
        /// </returns>
        public ActionResult SavePatientInfoChangesQueue(PatientInfoChangesQueue model)
        {
            //Initialize the newId variable 
            var userId = Helpers.GetLoggedInUserId();
            var currentDate = Helpers.GetInvariantCultureDateTime();
            var list = new List<PatientInfoChangesQueueCustomModel>();

            //Check if Model is not null 
            if (model != null)
            {
                using (var bal = new PatientInfoChangesQueueBal())
                {
                    if (model.Id > 0)
                    {
                        model.ModifiedBy = userId;
                        model.ModifiedDate = currentDate;
                    }
                    else
                    {
                        model.CreatedBy = userId;
                        model.CreatedDate = currentDate;
                    }
                    //Call the AddPatientInfoChangesQueue Method to Add / Update current PatientInfoChangesQueue
                    list = bal.SavePatientInfoChangesQueue(model);
                }
            }

            //Pass the ActionResult with List of PatientInfoChangesQueueViewModel object to Partial View PatientInfoChangesQueueList
            return PartialView(PartialViews.PatientInfoChangesQueueList, list);
        }

        /// <summary>
        /// Get the details of the current PatientInfoChangesQueue in the view model by ID 
        /// </summary>
        /// <param name="shared">pass the input parameters such as ID</param>
        /// <returns></returns>
        public JsonResult GetPatientInfoChangesQueueDetails(int id)
        {
            using (var bal = new PatientInfoChangesQueueBal())
            {
                //Call the AddPatientInfoChangesQueue Method to Add / Update current PatientInfoChangesQueue
                var current = bal.GetPatientInfoChangesQueueByID(id);

                //Pass the ActionResult with the current PatientInfoChangesQueueViewModel object as model to PartialView PatientInfoChangesQueueAddEdit
                return Json(current);
            }
        }

        /// <summary>
        /// Delete the current PatientInfoChangesQueue based on the PatientInfoChangesQueue ID passed in the PatientInfoChangesQueueModel
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult DeletePatientInfoChangesQueue(int id)
        {
            using (var bal = new PatientInfoChangesQueueBal())
            {
                //Get PatientInfoChangesQueue model object by current PatientInfoChangesQueue ID
                var model = bal.GetPatientInfoChangesQueueByID(id);
                var userId = Helpers.GetLoggedInUserId();
                var list = new List<PatientInfoChangesQueueCustomModel>();
                var currentDate = Helpers.GetInvariantCultureDateTime();

                //Check If PatientInfoChangesQueue model is not null
                if (model != null)
                {
                    model.IsActive = false;
                    model.ModifiedBy = userId;
                    model.ModifiedDate = currentDate;

                    //Update Operation of current PatientInfoChangesQueue
                    var result = bal.SavePatientInfoChangesQueue(model);
                    list = bal.GetPatientInfoChangesQueueList();
                    //return deleted ID of current PatientInfoChangesQueue as Json Result to the Ajax Call.
                }
                return PartialView(PartialViews.PatientInfoChangesQueueList, list);
            }

            //Pass the ActionResult with List of PatientInfoChangesQueueViewModel object to Partial View PatientInfoChangesQueueList
        }
    }
}
