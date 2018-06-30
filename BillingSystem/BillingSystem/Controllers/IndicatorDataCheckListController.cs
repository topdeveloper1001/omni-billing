// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IndicatorDataCheckListController.cs" company="Spadez">
//   Omnihelathcare
// </copyright>
// <summary>
//   The indicator data check list controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    using BillingSystem.Bal.BusinessAccess;
    using BillingSystem.Common;
    using BillingSystem.Model;
    using BillingSystem.Model.CustomModel;
    using BillingSystem.Models;

    /// <summary>
    /// The indicator data check list controller.
    /// </summary>
    public class IndicatorDataCheckListController : BaseController
    {
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
            using (var bal = new IndicatorDataCheckListService())
            {
                // Get IndicatorDataCheckList model object by current IndicatorDataCheckList ID
                IndicatorDataCheckList model = bal.GetIndicatorDataCheckListById(id);
                int userId = Helpers.GetLoggedInUserId();
                var list = new List<IndicatorDataCheckListCustomModel>();
                DateTime currentDate = Helpers.GetInvariantCultureDateTime();

                // Check If IndicatorDataCheckList model is not null
                if (model != null)
                {
                    model.ModifiedBy = userId;
                    model.ModifiedDate = currentDate;

                    // Update Operation of current IndicatorDataCheckList
                    list = bal.SaveIndicatorDataCheckList(model);

                    // return deleted ID of current IndicatorDataCheckList as Json Result to the Ajax Call.
                }

                // Pass the ActionResult with List of IndicatorDataCheckListViewModel object to Partial View IndicatorDataCheckListList
                return this.PartialView(PartialViews.IndicatorDataCheckListList, list);
            }
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
            using (var bal = new IndicatorDataCheckListService())
            {
                // Get the Entity list
                list = bal.GetDataFromIndicatorDataCheckList(
                    Helpers.GetSysAdminCorporateID(), 
                    Helpers.GetDefaultFacilityId(), 
                    Convert.ToInt32(model.BudgetType), 
                    Convert.ToInt32(model.Year), 
                    model.Month);

                /* if (list.Count == 0)
                {*/
                int cId = Helpers.GetDefaultCorporateId();
                using (var facBal = new FacilityService())
                {
                    List<Facility> facilities = facBal.GetFacilities(cId);
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

                        var oGlobalCodeBal = new GlobalCodeService();
                        List<GlobalCodes> yearDD =
                            oGlobalCodeBal.GetGlobalCodesByCategoryValue("4602").OrderBy(x => x.GlobalCodeID).ToList();
                        List<GlobalCodes> monthDD =
                            oGlobalCodeBal.GetGlobalCodesByCategoryValue("903").OrderBy(x => x.GlobalCodeID).ToList();
                        var obj = new IndicatorDataCheckListView
                                      {
                                          IndicatorDataCheckListList = merged, 
                                          DdYearList = yearDD, 
                                          DdMonthList = monthDD
                                      };
                        return this.PartialView(PartialViews.IndicatorDataCheckListList, obj);
                    }
                }

                /*}
                else
                {
                    var obj = new IndicatorDataCheckListView
                    {
                        IndicatorDataCheckListList = list
                    };
                    return PartialView(PartialViews.IndicatorDataCheckListList, obj);
                }*/
            }

            return this.PartialView(PartialViews.IndicatorDataCheckListList, list);
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
            using (var facBal = new FacilityService())
            {
                List<Facility> facilities = facBal.GetFacilities(cId);
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
                    return this.PartialView(PartialViews.IndicatorDataCheckListList, obj);
                }
            }

            return this.Json(null);
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
            using (var bal = new IndicatorDataCheckListService())
            {
                // Call the AddIndicatorDataCheckList Method to Add / Update current IndicatorDataCheckList
                IndicatorDataCheckList current = bal.GetIndicatorDataCheckListById(id);

                // Pass the ActionResult with the current IndicatorDataCheckListViewModel object as model to PartialView IndicatorDataCheckListAddEdit
                return this.Json(current);
            }
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
            // Initialize the IndicatorDataCheckList BAL object
            using (var bal = new IndicatorDataCheckListService())
            {
                // Get the Entity list
                var list = new List<IndicatorDataCheckListCustomModel>();

                // bal.GetIndicatorDataCheckListList(Helpers.GetSysAdminCorporateID(), Helpers.GetDefaultFacilityId());

                // Intialize the View Model i.e. IndicatorDataCheckListView which is binded to Main View Index.cshtml under IndicatorDataCheckList
                var viewModel = new IndicatorDataCheckListView
                                    {
                                        IndicatorDataCheckListList = list, 
                                        CurrentIndicatorDataCheckList =
                                            new IndicatorDataCheckList()
                                    };

                // Pass the View Model in ActionResult to View IndicatorDataCheckList
                return View(viewModel);
            }
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
                using (var bal = new IndicatorDataCheckListService())
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
                    list = bal.SaveIndicatorDataCheckList(model);
                }
            }

            // Pass the ActionResult with List of IndicatorDataCheckListViewModel object to Partial View IndicatorDataCheckListList
            return this.PartialView(PartialViews.IndicatorDataCheckListList, list);
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
                using (var bal = new IndicatorDataCheckListService())
                {
                    // if (model.Id > 0)
                    // {
                    // model.ModifiedBy = userId;
                    // model.ModifiedDate = currentDate;
                    // }
                    // else
                    // {
                    // model.CreatedBy = userId;
                    // model.CreatedDate = currentDate;
                    // }
                    int? budgetType = model.IndicatorDataCheckListList[0].BudgetType;
                    int? year = model.IndicatorDataCheckListList[0].Year;
                    string month = Convert.ToString(model.IndicatorDataCheckListList[0].Month);
                    bool isDeleted = bal.DeleteIndicatorDataCheckList(
                        Convert.ToString(corporateId), 
                        Convert.ToString(facilityId), 
                        Convert.ToInt32(budgetType), 
                        Convert.ToInt32(year), 
                        Convert.ToInt32(month));
                    if (isDeleted)
                    {
                        // Call the AddIndicatorDataCheckList Method to Add / Update current IndicatorDataCheckList
                        foreach (IndicatorDataCheckListCustomModel item in model.IndicatorDataCheckListList)
                        {
                            var oIndicatorDataCheckList = new IndicatorDataCheckList
                                                              {
                                                                  CorporateId = corporateId, 
                                                                  FacilityId = item.FacilityId, 
                                                                  M1 =
                                                                      Convert.ToString(item.CusM1)
                                                                          .ToLower() == "true"
                                                                          ? "1"
                                                                          : "0", 
                                                                  M2 =
                                                                      Convert.ToString(item.CusM2)
                                                                          .ToLower() == "true"
                                                                          ? "1"
                                                                          : "0", 
                                                                  M3 =
                                                                      Convert.ToString(item.CusM3)
                                                                          .ToLower() == "true"
                                                                          ? "1"
                                                                          : "0", 
                                                                  M4 =
                                                                      Convert.ToString(item.CusM4)
                                                                          .ToLower() == "true"
                                                                          ? "1"
                                                                          : "0", 
                                                                  M5 =
                                                                      Convert.ToString(item.CusM5)
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
                                                                  ExternalValue1 =
                                                                      Convert.ToString(
                                                                          item.ExternalValue1), 
                                                                  ExternalValue2 = month
                                                              };
                            list = bal.SaveIndicatorDataCheckList(oIndicatorDataCheckList);
                        }
                    }
                }
            }

            // Pass the ActionResult with List of IndicatorDataCheckListViewModel object to Partial View IndicatorDataCheckListList
            // return PartialView(PartialViews.IndicatorDataCheckListList, list);
            return this.Json(list);
        }

        #endregion
    }
}