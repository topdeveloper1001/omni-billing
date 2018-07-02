using System.Web.Mvc;
using BillingSystem.Common;
using BillingSystem.Model;
using System;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Controllers
{
    public class OpenOrderActivityScheduleController : BaseController
    {
        private readonly IOpenOrderActivityScheduleService _service;

        public OpenOrderActivityScheduleController(IOpenOrderActivityScheduleService service)
        {
            _service = service;
        }

        public ActionResult SaveOpenOrderActivitySchedule(OpenOrderActivitySchedule OpenOrderActivityScheduleModel)
        {
            //Initialize the newId variable 
            var newId = -1;

            //Check if OpenOrderActivityScheduleViewModel 
            if (OpenOrderActivityScheduleModel != null)
            {
                if (OpenOrderActivityScheduleModel.OpenOrderActivityScheduleID > 0)
                {
                    OpenOrderActivityScheduleModel.ModifiedBy = Helpers.GetLoggedInUserId();
                    OpenOrderActivityScheduleModel.ModifiedDate = Helpers.GetInvariantCultureDateTime();
                }
                //Call the AddOpenOrderActivitySchedule Method to Add / Update current OpenOrderActivitySchedule
                newId = _service.AddUpdateOpenOrderActivitySchedule(OpenOrderActivityScheduleModel);
            }
            return Json(newId);
        }

        public ActionResult GetOpenOrderActivitySchedule(OpenOrderActivitySchedule model)
        {
            var currentOpenOrderActivitySchedule = _service.GetOpenOrderActivityScheduleById(Convert.ToInt32(model));
            return PartialView("", currentOpenOrderActivitySchedule);
        }

        public ActionResult DeleteOpenOrderActivitySchedule(OpenOrderActivitySchedule model)
        {
            //Get OpenOrderActivitySchedule model object by current OpenOrderActivitySchedule ID
            var currentOpenOrderActivitySchedule = _service.GetOpenOrderActivityScheduleById(Convert.ToInt32(model));

            //Check If OpenOrderActivitySchedule model is not null
            if (currentOpenOrderActivitySchedule != null)
            {
                currentOpenOrderActivitySchedule.IsDeleted = true;
                currentOpenOrderActivitySchedule.DeletedBy = Helpers.GetLoggedInUserId();
                currentOpenOrderActivitySchedule.DeletedDate = Helpers.GetInvariantCultureDateTime();

                //Update Operation of current OpenOrderActivitySchedule
                var result = _service.AddUpdateOpenOrderActivitySchedule(currentOpenOrderActivitySchedule);

                //return deleted ID of current OpenOrderActivitySchedule as Json Result to the Ajax Call.
                return Json(result);
            }

            //Return the Json result as Action Result back JSON Call Success
            return Json(null);
        }

        public ActionResult ResetOpenOrderActivityScheduleForm()
        {
            //Intialize the new object of OpenOrderActivitySchedule ViewModel
            var openOrderActivityScheduleViewModel = new OpenOrderActivitySchedule();

            //Pass the View Model as OpenOrderActivityScheduleViewModel to PartialView OpenOrderActivityScheduleAddEdit just to update the AddEdit partial view.
            return PartialView(PartialViews.OpenOrderActivityScheduleAddEdit, openOrderActivityScheduleViewModel);
        }
    }
}
