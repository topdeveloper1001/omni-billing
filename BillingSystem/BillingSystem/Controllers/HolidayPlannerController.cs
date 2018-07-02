using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Models;

namespace BillingSystem.Controllers
{

    /// <summary>
    /// The holiday planner controller.
    /// </summary>
    public class HolidayPlannerController : BaseController
    {
        private readonly IFacilityStructureService _fsService;
        private readonly IHolidayPlannerService _service;
        private readonly IFacilityService _fService;
        private readonly IRoleService _rService;

        public HolidayPlannerController(IFacilityStructureService fsService, IHolidayPlannerService service, IFacilityService fService, IRoleService rService)
        {
            _fsService = fsService;
            _service = service;
            _fService = fService;
            _rService = rService;
        }

        #region Public Methods and Operators

        /// <summary>
        ///     Bind all the HolidayPlanner list
        /// </summary>
        /// <returns>action result with the partial view containing the HolidayPlanner list object</returns>
        [HttpPost]
        public ActionResult BindHolidayPlannerList()
        {
            var coprporateId = Helpers.GetDefaultCorporateId();
            var holidayPlannerList = _service.GetHolidayPlanner(coprporateId);
            return PartialView(PartialViews.HolidayPlannerList, holidayPlannerList);
        }



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
            HolidayPlanner currentHolidayPlanner = _service.GetHolidayPlannerById(id);

            // Pass the ActionResult with the current HolidayPlannerViewModel object as model to PartialView HolidayPlannerAddEdit
            return PartialView(PartialViews.HolidayPlannerAddEdit, currentHolidayPlanner);
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
            var corporateId = Helpers.GetDefaultCorporateId();
            // Get the Entity list
            var holidayPlannerList = _service.GetHolidayPlanner(corporateId);

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
                if (model.HolidayPlannerId == 0)
                {
                    model.IsActive = true;
                    model.CreatedBy = Helpers.GetLoggedInUserId();
                    model.CreatedDate = Helpers.GetInvariantCultureDateTime();
                }

                // Call the AddHolidayPlanner Method to Add / Update current HolidayPlanner
                var obj = _service.SaveHolidayPlanner(model);

                return Json(obj, JsonRequestBehavior.AllowGet);
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

        #endregion

        public ActionResult GetHolidayPlannerList(int facilityId, int corporateId, int year, string itemTypeId,
                    string itemId)
        {
            var holidayList = _service.GetHolidayPlannerByCurrentSelection(facilityId, corporateId, year, itemTypeId, itemId);
            return Json(holidayList, JsonRequestBehavior.AllowGet);
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
                        var name = _fService.GetFacilityNameById(fId);
                        list.Add(new DropdownListData
                        {
                            Text = name,
                            Value = Convert.ToString(fId)
                        });
                        break;
                    //Departments
                    case 2:
                        var facilityDepartments = _fsService.GetFacilityDepartments(cId, Convert.ToString(fId));
                        if (facilityDepartments.Any())
                        {
                            list.AddRange(facilityDepartments.Select(item => new DropdownListData
                            {
                                Text = string.Format(" {0} ", item.FacilityStructureName),
                                Value = Convert.ToString(item.FacilityStructureId)
                            }));
                        }

                        break;
                    //Physicians
                    case 3:
                        var userTypeId = _rService.GetRoleIdByFacilityAndName("physicians", cId, fId);
                        list = Helpers.GetPhysiciansByUserRole(userTypeId);
                        break;

                }
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
    }
}