using System.Web.Mvc;
using BillingSystem.Common;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Model;
using System;


namespace BillingSystem.Controllers
{
    public class OpenOrderActivityScheduleController : BaseController
    {

        ///// <returns>returns the actionresult in the form of current object of the Model OpenOrderActivitySchedule to be passed to View OpenOrderActivitySchedule</returns>
        //public ActionResult OpenOrderActivitySchedule()
        //{
        //    var OpenOrderActivityScheduleBal = new OpenOrderActivityScheduleBal();


        //    var OpenOrderActivityScheduleList = OpenOrderActivityScheduleBal.GetAllOpenOrderActivitySchedule();


        //    var OpenOrderActivityScheduleView = new OpenOrderActivityScheduleView
        //    {
        //        OpenOrderActivityScheduleList = OpenOrderActivityScheduleList,
        //        CurrentOpenOrderActivitySchedule = new OpenOrderActivitySchedule()
        //    };

        //    //Pass the View Model in ActionResult to View OpenOrderActivitySchedule
        //    return View(OpenOrderActivityScheduleView);

        //}

        //[HttpPost]
        //public ActionResult BindOpenOrderActivityScheduleList()
        //{

        //    using (var OpenOrderActivityScheduleBal = new OpenOrderActivityScheduleBal())
        //    {
        //        //Get the facilities list
        //        var OpenOrderActivityScheduleList = OpenOrderActivityScheduleBal.GetAllOpenOrderActivitySchedule();

        //        //Pass the ActionResult with List of OpenOrderActivityScheduleViewModel object to Partial View OpenOrderActivityScheduleList
        //        return PartialView(PartialViews.OpenOrderActivityScheduleList, OpenOrderActivityScheduleList);
        //    }
        //}


        /// <summary>
        /// Saves the open order activity schedule.
        /// </summary>
        /// <param name="OpenOrderActivityScheduleModel">The open order activity schedule model.</param>
        /// <returns></returns>
        public ActionResult SaveOpenOrderActivitySchedule(OpenOrderActivitySchedule OpenOrderActivityScheduleModel)
        {
            //Initialize the newId variable 
            var newId = -1;

            //Check if OpenOrderActivityScheduleViewModel 
            if (OpenOrderActivityScheduleModel != null)
            {
                using (var openOrderActivityScheduleBal = new OpenOrderActivityScheduleBal())
                {
                    if (OpenOrderActivityScheduleModel.OpenOrderActivityScheduleID > 0)
                    {
                        OpenOrderActivityScheduleModel.ModifiedBy = Helpers.GetLoggedInUserId();
                        OpenOrderActivityScheduleModel.ModifiedDate = Helpers.GetInvariantCultureDateTime();
                    }
                    //Call the AddOpenOrderActivitySchedule Method to Add / Update current OpenOrderActivitySchedule
                    newId = openOrderActivityScheduleBal.AddUpdateOpenOrderActivitySchedule(OpenOrderActivityScheduleModel);
                }
            }
            return Json(newId);
        }

        /// <summary>
        /// Get the details of the current OpenOrderActivitySchedule in the view model by ID
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public ActionResult GetOpenOrderActivitySchedule(OpenOrderActivitySchedule model)
        {
            using (var OpenOrderActivityScheduleBal = new OpenOrderActivityScheduleBal())
            {
                //Call the AddOpenOrderActivitySchedule Method to Add / Update current OpenOrderActivitySchedule
                var currentOpenOrderActivitySchedule = OpenOrderActivityScheduleBal.GetOpenOrderActivityScheduleById(Convert.ToInt32(model));
                return PartialView("", currentOpenOrderActivitySchedule);
            }
        }

        /// <summary>
        /// Delete the current OpenOrderActivitySchedule based on the OpenOrderActivitySchedule ID passed in the SharedViewModel
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public ActionResult DeleteOpenOrderActivitySchedule(OpenOrderActivitySchedule model)
        {
            using (var openOrderActivityScheduleBal = new OpenOrderActivityScheduleBal())
            {
                //Get OpenOrderActivitySchedule model object by current OpenOrderActivitySchedule ID
                var currentOpenOrderActivitySchedule = openOrderActivityScheduleBal.GetOpenOrderActivityScheduleById(Convert.ToInt32(model));

                //Check If OpenOrderActivitySchedule model is not null
                if (currentOpenOrderActivitySchedule != null)
                {
                    currentOpenOrderActivitySchedule.IsDeleted = true;
                    currentOpenOrderActivitySchedule.DeletedBy = Helpers.GetLoggedInUserId();
                    currentOpenOrderActivitySchedule.DeletedDate = Helpers.GetInvariantCultureDateTime();

                    //Update Operation of current OpenOrderActivitySchedule
                    var result = openOrderActivityScheduleBal.AddUpdateOpenOrderActivitySchedule(currentOpenOrderActivitySchedule);

                    //return deleted ID of current OpenOrderActivitySchedule as Json Result to the Ajax Call.
                    return Json(result);
                }
            }

            //Return the Json result as Action Result back JSON Call Success
            return Json(null);
        }

        
        /// <summary>
        /// Resets the open order activity schedule form.
        /// </summary>
        /// <returns></returns>
        public ActionResult ResetOpenOrderActivityScheduleForm()
        {
            //Intialize the new object of OpenOrderActivitySchedule ViewModel
            var openOrderActivityScheduleViewModel = new OpenOrderActivitySchedule();

            //Pass the View Model as OpenOrderActivityScheduleViewModel to PartialView OpenOrderActivityScheduleAddEdit just to update the AddEdit partial view.
            return PartialView(PartialViews.OpenOrderActivityScheduleAddEdit, openOrderActivityScheduleViewModel);
        }
    }
}
