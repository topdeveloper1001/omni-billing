﻿namespace BillingSystem.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using BillingSystem.Bal.Interfaces;
    using BillingSystem.Common;
    using BillingSystem.Model;
    using BillingSystem.Model.CustomModel;
    using BillingSystem.Models;

    /// <summary>
    /// The indicator data check list controller.
    /// </summary>
    public class IndicatorDataCheckListController : BaseController
    {
        private readonly IIndicatorDataCheckListService _service;
        private readonly IFacilityService _fService;
        private readonly IGlobalCodeService _gService;

        public IndicatorDataCheckListController(IIndicatorDataCheckListService service, IFacilityService fService, IGlobalCodeService gService)
        {
            _service = service;
            _fService = fService;
            _gService = gService;
        }


        #region Public Methods and Operators

        /// <summary>
        /// Delete the current IndicatorDataCheckList based on the IndicatorDataCheckList ID passed in the
        ///     IndicatorDataCheckListModel
        /// </summary>
        /// <param name="id">
        /// The identifier.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult DeleteIndicatorDataCheckList(int id)
        {
            IndicatorDataCheckList model = _service.GetIndicatorDataCheckListById(id);
            int userId = Helpers.GetLoggedInUserId();
            var list = new List<IndicatorDataCheckListCustomModel>();
            DateTime currentDate = Helpers.GetInvariantCultureDateTime();

            // Check If IndicatorDataCheckList model is not null
            if (model != null)
            {
                model.ModifiedBy = userId;
                model.ModifiedDate = currentDate;

                // Update Operation of current IndicatorDataCheckList
                list = _service.SaveIndicatorDataCheckList(model);

                // return deleted ID of current IndicatorDataCheckList as Json Result to the Ajax Call.
            }

            // Pass the ActionResult with List of IndicatorDataCheckListViewModel object to Partial View IndicatorDataCheckListList
            return PartialView(PartialViews.IndicatorDataCheckListList, list);
        }

        /// <summary>
        /// The get data from indicator data check list.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult GetDataFromIndicatorDataCheckList(IndicatorDataCheckListView model)
        {
            var list = new List<IndicatorDataCheckListCustomModel>();
            // Get the Entity list
            list = _service.GetDataFromIndicatorDataCheckList(
                Helpers.GetSysAdminCorporateID(),
                Helpers.GetDefaultFacilityId(),
                Convert.ToInt32(model.BudgetType),
                Convert.ToInt32(model.Year),
                model.Month);

            /* if (list.Count == 0)
            {*/
            int cId = Helpers.GetDefaultCorporateId();
            var facilities = _fService.GetFacilities(cId);
            if (facilities.Any())
            {
                var merged = new List<IndicatorDataCheckListCustomModel>(list);
                var list1 = new List<IndicatorDataCheckListCustomModel>();
                list1.AddRange(
                    facilities.Select(
                        item =>
                        new IndicatorDataCheckListCustomModel
                        {
                            FacilityName = item.FacilityName,
                            FacilityId = item.FacilityId,
                        }));
                merged.AddRange(list1.Where(p2 => list.All(p1 => p1.FacilityId != p2.FacilityId)));

                var yearDD = _gService.GetGlobalCodesByCategoryValue("4602").OrderBy(x => x.GlobalCodeID).ToList();
                List<GlobalCodes> monthDD = _gService.GetGlobalCodesByCategoryValue("903").OrderBy(x => x.GlobalCodeID).ToList();
                var obj = new IndicatorDataCheckListView
                {
                    IndicatorDataCheckListList = merged,
                    DdYearList = yearDD,
                    DdMonthList = monthDD
                };
                return PartialView(PartialViews.IndicatorDataCheckListList, obj);
            }
            return PartialView(PartialViews.IndicatorDataCheckListList, list);
        }

        /// <summary>
        /// Get Facilities list
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult GetFacilitiesList()
        {
            int cId = Helpers.GetDefaultCorporateId();
            List<Facility> facilities = _fService.GetFacilities(cId);
            if (facilities.Any())
            {
                var list = new List<IndicatorDataCheckListCustomModel>();
                list.AddRange(
                    facilities.Select(
                        item =>
                        new IndicatorDataCheckListCustomModel
                        {
                            FacilityName = item.FacilityName,
                            FacilityId = item.FacilityId,
                        }));

                var obj = new IndicatorDataCheckListView { IndicatorDataCheckListList = list };
                return PartialView(PartialViews.IndicatorDataCheckListList, obj);
            }

            return Json(null);
        }

        /// <summary>
        /// Get the details of the current IndicatorDataCheckList in the view model by ID
        /// </summary>
        /// <param name="id">
        /// The identifier.
        /// </param>
        /// <returns>
        /// The <see cref="JsonResult"/>.
        /// </returns>
        public JsonResult GetIndicatorDataCheckListDetails(int id)
        {
            var current = _service.GetIndicatorDataCheckListById(id);

            // Pass the ActionResult with the current IndicatorDataCheckListViewModel object as model to PartialView IndicatorDataCheckListAddEdit
            return Json(current);
        }

        /// <summary>
        ///     Get the details of the IndicatorDataCheckList View in the Model IndicatorDataCheckList such as
        ///     IndicatorDataCheckListList, list of countries etc.
        /// </summary>
        /// <returns>
        ///     returns the actionresult in the form of current object of the Model IndicatorDataCheckList to be passed to View
        ///     IndicatorDataCheckList
        /// </returns>
        public ActionResult Index()
        {
            var list = new List<IndicatorDataCheckListCustomModel>();

            // Intialize the View Model i.e. IndicatorDataCheckListView which is binded to Main View Index.cshtml under IndicatorDataCheckList
            var vm = new IndicatorDataCheckListView
            {
                IndicatorDataCheckListList = list,
                CurrentIndicatorDataCheckList = new IndicatorDataCheckList()
            };

            // Pass the View Model in ActionResult to View IndicatorDataCheckList
            return View(vm);
        }

        /// <summary>
        /// Add New or Update the IndicatorDataCheckList based on if we pass the IndicatorDataCheckList ID in the
        ///     IndicatorDataCheckListViewModel object.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// returns the newly added or updated ID of IndicatorDataCheckList row
        /// </returns>
        public ActionResult SaveIndicatorDataCheckList(IndicatorDataCheckList model)
        {
            // Initialize the newId variable 
            int userId = Helpers.GetLoggedInUserId();
            DateTime currentDate = Helpers.GetInvariantCultureDateTime();
            var list = new List<IndicatorDataCheckListCustomModel>();

            model.CorporateId = Helpers.GetSysAdminCorporateID();
            model.FacilityId = Helpers.GetDefaultFacilityId();

            // Check if Model is not null 
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

                // Call the AddIndicatorDataCheckList Method to Add / Update current IndicatorDataCheckList
                list = _service.SaveIndicatorDataCheckList(model);
            }

            // Pass the ActionResult with List of IndicatorDataCheckListViewModel object to Partial View IndicatorDataCheckListList
            return PartialView(PartialViews.IndicatorDataCheckListList, list);
        }

        /// <summary>
        /// The save indicator data check list in db.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult SaveIndicatorDataCheckListInDB(IndicatorDataCheckListView model)
        {
            // Initialize the newId variable 
            int userId = Helpers.GetLoggedInUserId();
            DateTime currentDate = Helpers.GetInvariantCultureDateTime();
            var list = new List<IndicatorDataCheckListCustomModel>();

            int corporateId = Helpers.GetSysAdminCorporateID();
            int facilityId = Helpers.GetDefaultFacilityId();

            // Check if Model is not null 
            if (model != null)
            {
                var budgetType = model.IndicatorDataCheckListList[0].BudgetType;
                var year = model.IndicatorDataCheckListList[0].Year;
                var month = model.IndicatorDataCheckListList[0].Month;
                var isDeleted = _service.DeleteIndicatorDataCheckList(Convert.ToString(corporateId),
                    Convert.ToString(facilityId), Convert.ToInt32(budgetType), year.GetValueOrDefault(), month);
                if (isDeleted)
                {
                    // Call the AddIndicatorDataCheckList Method to Add / Update current IndicatorDataCheckList
                    foreach (IndicatorDataCheckListCustomModel item in model.IndicatorDataCheckListList)
                    {
                        var oIndicatorDataCheckList = new IndicatorDataCheckList
                        {
                            CorporateId = corporateId,
                            FacilityId = item.FacilityId,
                            M1 = Convert.ToString(item.CusM1).ToLower() == "true" ? "1" : "0",
                            M2 = Convert.ToString(item.CusM2).ToLower() == "true" ? "1" : "0",
                            M3 = Convert.ToString(item.CusM3).ToLower() == "true" ? "1" : "0",
                            M4 = Convert.ToString(item.CusM4).ToLower() == "true" ? "1" : "0",
                            M5 = Convert.ToString(item.CusM5)
                                                                      .ToLower() == "true"
                                                                      ? "1"
                                                                      : "0",
                            M6 =
                                                                  Convert.ToString(item.CusM6)
                                                                      .ToLower() == "true"
                                                                      ? "1"
                                                                      : "0",
                            M7 =
                                                                  Convert.ToString(item.CusM7)
                                                                      .ToLower() == "true"
                                                                      ? "1"
                                                                      : "0",
                            M8 =
                                                                  Convert.ToString(item.CusM8)
                                                                      .ToLower() == "true"
                                                                      ? "1"
                                                                      : "0",
                            M9 =
                                                                  Convert.ToString(item.CusM9)
                                                                      .ToLower() == "true"
                                                                      ? "1"
                                                                      : "0",
                            M10 =
                                                                  Convert.ToString(item.CusM10)
                                                                      .ToLower() == "true"
                                                                      ? "1"
                                                                      : "0",
                            M11 =
                                                                  Convert.ToString(item.CusM11)
                                                                      .ToLower() == "true"
                                                                      ? "1"
                                                                      : "0",
                            M12 =
                                                                  Convert.ToString(item.CusM12)
                                                                      .ToLower() == "true"
                                                                      ? "1"
                                                                      : "0",
                            CreatedBy = userId,
                            CreatedDate = currentDate,
                            IsActive = true,
                            BudgetType = budgetType,
                            Year = year,
                            ExternalValue1 = Convert.ToString(item.ExternalValue1),
                            ExternalValue2 = Convert.ToString(month)
                        };
                        list = _service.SaveIndicatorDataCheckList(oIndicatorDataCheckList);
                    }
                }
            }
            return Json(list);
        }

        #endregion

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetMonthsData(string categoryId, int facilityId)
        {
            var currentDateTime = Helpers.GetInvariantCultureDateTime();
            var cId = Helpers.GetDefaultCorporateId();
            facilityId = facilityId > 0 ? facilityId : Helpers.GetDefaultFacilityId();
            var defaultYear = currentDateTime.Year;
            var defaultMonth = currentDateTime.Month - 1;

            var list = new List<SelectListItem>();
            var glist = _gService.GetGlobalCodesByCategoryValue(categoryId).OrderBy(x => int.Parse(x.GlobalCodeValue)).ToList();
            if (glist.Any())
            {
                list.AddRange(glist.Select(item => new SelectListItem
                {
                    Text = item.GlobalCodeName,
                    Value = item.GlobalCodeValue
                }));
            }

            var defaults = _service.GetDefaultMonthAndYearByFacilityId(facilityId, cId);
            if (defaults.Count > 0)
            {
                defaultYear = defaults[0] > 0 ? defaults[0] : defaultYear;
                defaultMonth = defaults[1] > 0 ? defaults[1] : defaultMonth;
            }

            var jsonData = new
            {
                list,
                defaultYear,
                defaultMonth
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

    }
}