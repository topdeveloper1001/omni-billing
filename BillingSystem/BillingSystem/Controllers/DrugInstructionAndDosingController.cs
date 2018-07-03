using BillingSystem.Models;
using BillingSystem.Common;
using System.Collections.Generic;
using System.Web.Mvc;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Controllers
{
    public class DrugInstructionAndDosingController : BaseController
    {
        private readonly IDrugInstructionAndDosingService _service;

        public DrugInstructionAndDosingController(IDrugInstructionAndDosingService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get the details of the DrugInstructionAndDosing View in the Model DrugInstructionAndDosing such as DrugInstructionAndDosingList, list of countries etc.
        /// </summary>
        /// <returns>
        /// returns the actionresult in the form of current object of the Model DrugInstructionAndDosing to be passed to View DrugInstructionAndDosing
        /// </returns>
        public ActionResult Index()
        {
            //Get the Entity list
            var list = _service.GetDrugInstructionAndDosingList();

            //Intialize the View Model i.e. DrugInstructionAndDosingView which is binded to Main View Index.cshtml under DrugInstructionAndDosing
            var viewModel = new DrugInstructionAndDosingView
            {
                DrugInstructionAndDosingList = list,
                CurrentDrugInstructionAndDosing = new DrugInstructionAndDosing()
            };

            //Pass the View Model in ActionResult to View DrugInstructionAndDosing
            return View(viewModel);
        }

        /// <summary>
        /// Add New or Update the DrugInstructionAndDosing based on if we pass the DrugInstructionAndDosing ID in the DrugInstructionAndDosingViewModel object.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// returns the newly added or updated ID of DrugInstructionAndDosing row
        /// </returns>
        public ActionResult SaveDrugInstructionAndDosing(DrugInstructionAndDosing model)
        {
            //Initialize the newId variable 
            var userId = Helpers.GetLoggedInUserId();
            var currentDate = Helpers.GetInvariantCultureDateTime();
            var list = new List<DrugInstructionAndDosingCustomModel>();

            //Check if Model is not null 
            if (model != null)
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

                //Call the AddDrugInstructionAndDosing Method to Add / Update current DrugInstructionAndDosing
                list = _service.SaveDrugInstructionAndDosing(model);
            }

            //Pass the ActionResult with List of DrugInstructionAndDosingViewModel object to Partial View DrugInstructionAndDosingList
            return PartialView(PartialViews.DrugInstructionAndDosingList, list);
        }


        /// <summary>
        /// Gets the drug instruction and dosing list.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetDrugInstructionAndDosingList()
        {
            var list = _service.GetDrugInstructionAndDosingList();
            return PartialView(PartialViews.DrugInstructionAndDosingList, list);
        }

        /// <summary>
        /// Get the details of the current DrugInstructionAndDosing in the view model by ID
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public JsonResult GetDrugInstructionAndDosingDetails(int id)
        {
            //Call the AddDrugInstructionAndDosing Method to Add / Update current DrugInstructionAndDosing
            var current = _service.GetDrugInstructionAndDosingById(id);

            //Pass the ActionResult with the current DrugInstructionAndDosingViewModel object as model to PartialView DrugInstructionAndDosingAddEdit
            return Json(current);
        }

        /// <summary>
        /// Delete the current DrugInstructionAndDosing based on the DrugInstructionAndDosing ID passed in the DrugInstructionAndDosingModel
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult DeleteDrugInstructionAndDosing(int id)
        {
            //Get DrugInstructionAndDosing model object by current DrugInstructionAndDosing ID
            var model = _service.GetDrugInstructionAndDosingById(id);
            var userId = Helpers.GetLoggedInUserId();
            var list = new List<DrugInstructionAndDosingCustomModel>();
            var currentDate = Helpers.GetInvariantCultureDateTime();

            //Check If DrugInstructionAndDosing model is not null
            if (model != null)
            {
                model.IsDeleted = true;
                model.ModifiedBy = userId;
                model.ModifiedDate = currentDate;
                //Update Operation of current DrugInstructionAndDosing
                var result = _service.SaveDrugInstructionAndDosing(model);
                list = _service.GetDrugInstructionAndDosingList();
                //return deleted ID of current DrugInstructionAndDosing as Json Result to the Ajax Call.
            }
            return PartialView(PartialViews.DrugInstructionAndDosingList, list);
        }
    }
}
