using System;
using System.Collections.Generic;
using System.Linq;
using Elmah.ContentSyndication;

namespace BillingSystem.Controllers
{
    using System.Web.Mvc;

    using Bal.BusinessAccess;
    using Common;
    using Model;
    using Model.CustomModel;
    using Models;

    /// <summary>
    /// The holiday planner controller.
    /// </summary>
    public class HolidayPlannerController : BaseController
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Bind all the HolidayPlanner list
        /// </summary>
        /// <returns>action result with the partial view containing the HolidayPlanner list object</returns>
        [HttpPost]
        public ActionResult BindHolidayPlannerList()
        {

            using (var holidayPlannerBal = new HolidayPlannerBal())
            {
                var coprporateId = Helpers.GetDefaultCorporateId();
                var holidayPlannerList = holidayPlannerBal.GetHolidayPlanner(coprporateId);

                // Pass the ActionResult with List of HolidayPlannerViewModel object to Partial View HolidayPlannerList
                return PartialView(PartialViews.HolidayPlannerList, holidayPlannerList);
            }
        }

        /// <summary>
        /// Delete the current HolidayPlanner based on the HolidayPlanner ID passed in the HolidayPlannerModel
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        //public ActionResult DeleteHolidayPlanner(int id)
        //{
        //    using (var bal = new HolidayPlannerBal())
        //    {
        //        // Get HolidayPlanner model object by current HolidayPlanner ID
        //        var currentHolidayPlanner = bal.GetHolidayPlannerById(id);
        //        //var userId = Helpers.GetLoggedInUserId();

        //        // Check If HolidayPlanner model is not null
        //        if (currentHolidayPlanner != null)
        //        {
        //            currentHolidayPlanner.IsActive = false;

        //            // currentHolidayPlanner.ModifiedBy = userId;
        //            // currentHolidayPlanner.ModifiedDate = DateTime.Now;

        //            // Update Operation of current HolidayPlanner
        //            int result = bal.SaveHolidayPlanner(currentHolidayPlanner);

        //            // return deleted ID of current HolidayPlanner as Json Result to the Ajax Call.
        //            return Json(result);
        //        }
        //    }

        //    // Return the Json result as Action Result back JSON Call Success
        //    return Json(null);
        //}

        /// <summary>
        /// Get the details of the current HolidayPlanner in the view model by ID
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult GetHolidayPlanner(int id)
        {
            using (var bal = new HolidayPlannerBal())
            {
                // Call the AddHolidayPlanner Method to Add / Update current HolidayPlanner
                HolidayPlanner currentHolidayPlanner = bal.GetHolidayPlannerById(id);

                // Pass the ActionResult with the current HolidayPlannerViewModel object as model to PartialView HolidayPlannerAddEdit
                return PartialView(PartialViews.HolidayPlannerAddEdit, currentHolidayPlanner);
            }
        }

        /// <summary>
        /// Get the details of the HolidayPlanner View in the Model HolidayPlanner such as HolidayPlannerList, list of
        ///     countries etc.
        /// </summary>
        /// <returns>
        /// returns the actionresult in the form of current object of the Model HolidayPlanner to be passed to View
        ///     HolidayPlanner
        /// </returns>
        public ActionResult HolidayPlannerMain()
        {
            // Initialize the HolidayPlanner BAL object
            var holidayPlannerBal = new HolidayPlannerBal();
            var corporateId = Helpers.GetDefaultCorporateId();
            // Get the Entity list
            var holidayPlannerList = holidayPlannerBal.GetHolidayPlanner(corporateId);

            // Intialize the View Model i.e. HolidayPlannerView which is binded to Main View Index.cshtml under HolidayPlanner
            var holidayPlannerView = new HolidayPlannerView
            {
                HolidayPlannerList = holidayPlannerList,
                CurrentHolidayPlanner = new HolidayPlanner()
            };

            // Pass the View Model in ActionResult to View HolidayPlanner
            return View(holidayPlannerView);
        }

        /// <summary>
        /// Reset the HolidayPlanner View Model and pass it to HolidayPlannerAddEdit Partial View.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult ResetHolidayPlannerForm()
        {
            // Intialize the new object of HolidayPlanner ViewModel
            var holidayPlannerViewModel = new HolidayPlanner();

            // Pass the View Model as HolidayPlannerViewModel to PartialView HolidayPlannerAddEdit just to update the AddEdit partial view.
            return PartialView(PartialViews.HolidayPlannerAddEdit, holidayPlannerViewModel);
        }

        /// <summary>
        /// Add New or Update the HolidayPlanner based on if we pass the HolidayPlanner ID in the HolidayPlannerViewModel
        ///     object.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// returns the newly added or updated ID of HolidayPlanner row
        /// </returns>


        public JsonResult SaveHolidayPlanner(HolidayPlannerCustomModel model)
        {
            // Check if Model is not null 
            if (model != null)
            {
                using (var bal = new HolidayPlannerBal())
                {
                    if (model.HolidayPlannerId == 0)
                    {
                        model.IsActive = true;
                        model.CreatedBy = Helpers.GetLoggedInUserId();
                        model.CreatedDate = Helpers.GetInvariantCultureDateTime();
                    }

                    // Call the AddHolidayPlanner Method to Add / Update current HolidayPlanner
                    var obj = bal.SaveHolidayPlanner(model);

                    return Json(obj, JsonRequestBehavior.AllowGet);
                }
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

        #endregion

        public ActionResult GetHolidayPlannerList(int facilityId, int corporateId, int year, string itemTypeId,
                    string itemId)
        {
            using (var hBal = new HolidayPlannerBal())
            {
                var holidayList = hBal.GetHolidayPlannerByCurrentSelection(facilityId, corporateId, year, itemTypeId, itemId);
                return Json(holidayList, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult GetItemsDropdownData(int cId, int fId, int itemTypeId)
        {
            var list = new List<DropdownListData>();
            if (itemTypeId > 0)
            {
                switch (itemTypeId)
                {
                    //Facility Case
                    case 1:
                        using (var bal = new FacilityBal())
                        {
                            var name = bal.GetFacilityNameById(fId);
                            list.Add(new DropdownListData
                            {
                                Text = name,
                                Value = Convert.ToString(fId)
                            });
                        }
                        break;
                    //Departments
                    case 2:
                        using (var bal = new FacilityStructureBal())
                        {
                            var facilityDepartments = bal.GetFacilityDepartments(cId, Convert.ToString(fId));
                            if (facilityDepartments.Any())
                            {
                                list.AddRange(facilityDepartments.Select(item => new DropdownListData
                                {
                                    Text = string.Format(" {0} ", item.FacilityStructureName),
                                    Value = Convert.ToString(item.FacilityStructureId)
                                }));
                            }
                        }
                        break;
                    //Physicians
                    case 3:
                        using (var rBal = new RoleBal())
                        {
                            var userTypeId = rBal.GetRoleIdByFacilityAndName("physicians", cId, fId);
                            list = Helpers.GetPhysiciansByUserRole(userTypeId);
                        }
                        break;

                }
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
    }
}