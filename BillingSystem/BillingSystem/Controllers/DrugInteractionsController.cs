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
    public class DrugInteractionsController : BaseController
    {
        /// <summary>
        /// Get the details of the DrugInteractions View in the Model DrugInteractions such as DrugInteractionsList, list of countries etc.
        /// </summary>
        /// <returns>
        /// returns the actionresult in the form of current object of the Model DrugInteractions to be passed to View DrugInteractions
        /// </returns>
        public ActionResult Index()
        {
            //Initialize the DrugInteractions BAL object
            using (var bal = new DrugInteractionsBal())
            {
                //Get the Entity list
                var list = bal.GetDrugInteractionsList();

                //Intialize the View Model i.e. DrugInteractionsView which is binded to Main View Index.cshtml under DrugInteractions
                var viewModel = new DrugInteractionsView
                {
                    DrugInteractionsList = list,
                    CurrentDrugInteractions = new DrugInteractions()
                };

                //Pass the View Model in ActionResult to View DrugInteractions
                return View(viewModel);
            }
        }

        /// <summary>
        /// Add New or Update the DrugInteractions based on if we pass the DrugInteractions ID in the DrugInteractionsViewModel object.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// returns the newly added or updated ID of DrugInteractions row
        /// </returns>
        public ActionResult SaveDrugInteractions(DrugInteractions model)
        {
            //Initialize the newId variable 
            var userId = Helpers.GetLoggedInUserId();
            var currentDate = Helpers.GetInvariantCultureDateTime();
            var list = new List<DrugInteractionsCustomModel>();

            //Check if Model is not null 
            if (model != null)
            {
                using (var bal = new DrugInteractionsBal())
                {
                    if (model.Id > 0)
                    {
                        model.ModifiedBy = userId;
                        model.Modifieddate = currentDate;
                    }
                    else
                    {
                        model.CreatedBy = userId;
                        model.CreatedDate = currentDate;
                    }

                    //Call the AddDrugInteractions Method to Add / Update current DrugInteractions
                    list = bal.SaveDrugInteractions(model);
                    //list = bal.GetDrugInteractionsList();
                }
            }

            //Pass the ActionResult with List of DrugInteractionsViewModel object to Partial View DrugInteractionsList
            return PartialView(PartialViews.DrugInteractionsList, list);
        }

        /// <summary>
        /// Get the details of the current DrugInteractions in the view model by ID
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public JsonResult GetDrugInteractionsDetails(int id)
        {
            using (var bal = new DrugInteractionsBal())
            {
                //Call the AddDrugInteractions Method to Add / Update current DrugInteractions
                var current = bal.GetDrugInteractionsById(id);

                //Pass the ActionResult with the current DrugInteractionsViewModel object as model to PartialView DrugInteractionsAddEdit
                return Json(current);
            }
        }

        /// <summary>
        /// Delete the current DrugInteractions based on the DrugInteractions ID passed in the DrugInteractionsModel
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult DeleteDrugInteractions(int id)
        {
            using (var bal = new DrugInteractionsBal())
            {
                //Get DrugInteractions model object by current DrugInteractions ID
                var model = bal.GetDrugInteractionsById(id);
                var userId = Helpers.GetLoggedInUserId();
                var list = new List<DrugInteractionsCustomModel>();
                var currentDate = Helpers.GetInvariantCultureDateTime();

                //Check If DrugInteractions model is not null
                if (model != null)
                {
                    model.IsDeleted = true;
                    model.ModifiedBy = userId;
                    model.Modifieddate = currentDate;

                    //Update Operation of current DrugInteractions
                    var result = bal.SaveDrugInteractions(model);
                    list = bal.GetDrugInteractionsList();
                    //return deleted ID of current DrugInteractions as Json Result to the Ajax Call.
                }
                //Pass the ActionResult with List of DrugInteractionsViewModel object to Partial View DrugInteractionsList
                return PartialView(PartialViews.DrugInteractionsList, list);
            }
        }


        public ActionResult SortDrugInstructionlist()
        {
            using (var bal = new DrugInteractionsBal())
            {
                var list = bal.GetDrugInteractionsList();
                return PartialView(PartialViews.DrugInteractionsList, list);
            }
        }
    }
}
