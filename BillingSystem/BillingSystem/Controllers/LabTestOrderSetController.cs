using BillingSystem.Models;
using BillingSystem.Common;
using System.Collections.Generic;
using System.Web.Mvc;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Controllers
{
    public class LabTestOrderSetController : BaseController
    {
        private readonly ILabTestOrderSetService _service;

        public LabTestOrderSetController(ILabTestOrderSetService service)
        {
            _service = service;
        }


        /// <summary>
        /// Get the details of the LabTestOrderSet View in the Model LabTestOrderSet such as LabTestOrderSetList, list of countries etc.
        /// </summary>
        /// <returns>returns the actionresult in the form of current object of the Model LabTestOrderSet to be passed to View LabTestOrderSet</returns>
        public ActionResult Index()
        {
            var list = _service.GetLabOrderSetList();
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

        /// <summary>
        /// Bind all the LabTestOrderSet list 
        /// </summary>
        /// <returns>action result with the partial view containing the LabTestOrderSet list object</returns>
        [HttpPost]
        public ActionResult BindLabTestOrderSetList()
        {
            var list = _service.GetLabOrderSetList();
            return PartialView(PartialViews.LabTestOrderSetList, list);
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
                list = _service.SaveLabTestOrderSet(model);
            }
            return PartialView(PartialViews.LabTestOrderSetListView, list);
        }

        /// <summary>
        /// Get the details of the current LabTestOrderSet in the view model by ID 
        /// </summary>
        /// <returns></returns>
        public JsonResult GetLabTestOrderSet(int id)
        {
            var current = _service.GetDetailById(id);
            return Json(current);
        }

        /// <summary>
        /// Delete the current LabTestOrderSet based on the LabTestOrderSet ID passed in the LabTestOrderSetModel
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteLabTestOrderSet(int id)
        {
            var currentLabTestOrderSet = _service.GetDetailById(id);
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
                var list = _service.SaveLabTestOrderSet(currentLabTestOrderSet);

                //return deleted ID of current LabTestOrderSet as Json Result to the Ajax Call.
                return PartialView(PartialViews.LabTestOrderSetListView, list);
            }

            //Return the Json result as Action Result back JSON Call Success
            return Json(null);
        }


        public ActionResult SortLabTestOrderList()
        {
            var list = _service.GetLabOrderSetList();
            return PartialView(PartialViews.LabTestOrderSetListView, list);
        }


    }
}
