// To Do: DashboardController.cs
// FileName :DashboardController.cs
// CreatedDate: 2015-09-25 10:09 AM
// ModifiedDate: 2016-05-11 7:07 PM
// CreatedBy: Shashank Awasthy

namespace BillingSystem.Controllers
{
    #region

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web.Mvc;

    using Bal.BusinessAccess;
    using Common;
    using Common.Common;
    using Model.CustomModel;
    using Models;

    using Kendo.Mvc.Extensions;
    using Kendo.Mvc.UI;

    using Microsoft.Ajax.Utilities;

    #endregion

    public class DashboardController : BaseController
    {
        /// <summary>
        ///     Charges dashboard.
        /// </summary>
        /// <returns></returns>
        public ActionResult BillingDashboard()
        {
            using (var bal = new DashboardBudgetBal())
            {
                var facilityId = Helpers.GetDefaultFacilityId();
                var corporateId = Helpers.GetSysAdminCorporateID();
                var currentDateTime = Helpers.GetInvariantCultureDateTime();
                var currentYear = Convert.ToString(currentDateTime.Year);

                //var chargesapplied = bal.SetDashBoardChargesActuals(facilityId, corporateId, currentYear);
                var dashboardview = new BillingDashboardView
                                        {
                                            CurrentDate = Helpers.GetInvariantCultureDateTime(),
                                            NumberofCurrentDay = currentDateTime.DayOfYear,
                                            ClaimSubmmitedNumber =
                                                bal.GetDBChargesDashBoard(
                                                    facilityId, corporateId, Convert.ToString((int)ChargesDashBoard.NumberofTotalClaimsSubmitted), currentYear),
                                            ClaimSubmmitedDollorAmount =
                                                bal.GetDBChargesDashBoard(
                                                    facilityId,
                                                    corporateId,
                                                    Convert.ToString(
                                                        Convert.ToInt32(
                                                            ChargesDashBoard
                                                .DollarAmountofTotalClaimsSubmitted)),
                                                    currentYear),
                                            ClaimSubmmitedInpatientNumber =
                                                bal.GetDBChargesDashBoard(
                                                    facilityId,
                                                    corporateId,
                                                    Convert.ToString(
                                                        Convert.ToInt32(
                                                            ChargesDashBoard
                                                .NumberofTotalInpatientClaimsSubmitted)),
                                                    currentYear),
                                            ClaimSubmmitedDollorInpatientAmount =
                                                bal.GetDBChargesDashBoard(
                                                    facilityId,
                                                    corporateId,
                                                    Convert.ToString(
                                                        Convert.ToInt32(
                                                            ChargesDashBoard
                                                .DollarAmountofTotalInpatientClaimsSubmitted)),
                                                    currentYear),
                                            ClaimSubmmitedOutpatientNumber =
                                                bal.GetDBChargesDashBoard(
                                                    facilityId,
                                                    corporateId,
                                                    Convert.ToString(
                                                        Convert.ToInt32(
                                                            ChargesDashBoard
                                                .NumberofTotalOutpatientClaimsSubmitted)),
                                                    currentYear),
                                            ClaimSubmmitedDollorOutpatientAmount =
                                                bal.GetDBChargesDashBoard(
                                                    facilityId,
                                                    corporateId,
                                                    Convert.ToString(
                                                        Convert.ToInt32(
                                                            ChargesDashBoard
                                                .DollarAmountoftotalOutpatientClaimsSubmitted)),
                                                    currentYear),
                                            ClaimSubmmitedERpatientNumber =
                                                bal.GetDBChargesDashBoard(
                                                    facilityId,
                                                    corporateId,
                                                    Convert.ToString(
                                                        Convert.ToInt32(
                                                            ChargesDashBoard
                                                .NumberofTotalEmergencyRoomClaimsSubmitted)),
                                                    currentYear),
                                            ClaimSubmmitedEROutpatientAmount =
                                                bal.GetDBChargesDashBoard(
                                                    facilityId,
                                                    corporateId,
                                                    Convert.ToString(
                                                        Convert.ToInt32(
                                                            ChargesDashBoard
                                                .DollarAmountoftotalEmergencyRoomClaimsSubmitted)),
                                                    currentYear),
                                            ClaimSubmmitedAvgDollorAmount =
                                                bal.GetDBChargesDashBoard(
                                                    facilityId,
                                                    corporateId,
                                                    Convert.ToString(
                                                        Convert.ToInt32(
                                                            ChargesDashBoard
                                                .AverageDollarAmountofTotalClaims)),
                                                    currentYear),
                                            ClaimSubmmitedAvgDollorAmountInPatinet =
                                                bal.GetDBChargesDashBoard(
                                                    facilityId,
                                                    corporateId,
                                                    Convert.ToString(
                                                        Convert.ToInt32(
                                                            ChargesDashBoard
                                                .AverageDollarAmountofInpatientClaim)),
                                                    currentYear),
                                            ClaimSubmmitedAvgDollorAmountOPPatinet =
                                                bal.GetDBChargesDashBoard(
                                                    facilityId,
                                                    corporateId,
                                                    Convert.ToString(
                                                        Convert.ToInt32(
                                                            ChargesDashBoard
                                                .AverageDollarAmountofOutpatientClaim)),
                                                    currentYear),
                                            ClaimSubmmitedAvgDollorAmountERPatinet =
                                                bal.GetDBChargesDashBoard(
                                                    facilityId,
                                                    corporateId,
                                                    Convert.ToString(
                                                        Convert.ToInt32(
                                                            ChargesDashBoard
                                                .AverageDollarAmountofEmergencyRoomClaim)),
                                                    currentYear)
                                        };
                return View(dashboardview);
            }
        }

        /// <summary>
        ///     Exports to excel.
        /// </summary>
        /// <param name="dashBoardType">Type of the dash board.</param>
        /// <returns></returns>
        public ActionResult ExportToExcel(string dashBoardType)
        {
            Response.AddHeader("Content-Type", "application/vnd.ms-excel");
            using (var bal = new DashboardBudgetBal())
            {
                if (dashBoardType == "Charges")
                {
                    var facilityId = Helpers.GetDefaultFacilityId();
                    var corporateId = Helpers.GetSysAdminCorporateID();
                    var currentDateTime = Helpers.GetInvariantCultureDateTime(facilityId);
                    var currentYear = Convert.ToString(currentDateTime.Year);

                    // bal.SetDashBoardChargesActuals(facilityId, corporateId, currentYear);
                    // bal.SetDashBoardCounterActuals(facilityId, corporateId, currentYear);
                    var dashboardview = new ChargesDashboardView
                                            {
                                                CurrentDate = Helpers.GetInvariantCultureDateTime(),
                                                NumberofCurrentDay =
                                                    Helpers.GetInvariantCultureDateTime().DayOfYear,
                                                RoomChargesCustomModel =
                                                    bal.GetDBChargesDashBoard(
                                                        facilityId,
                                                        corporateId,
                                                        Convert.ToString(
                                                            Convert.ToInt32(
                                                                ChargesDashBoard.RoomGrossCharges)),
                                                        currentYear),
                                                IPChargesCustomModel =
                                                    bal.GetDBChargesDashBoard(
                                                        facilityId,
                                                        corporateId,
                                                        Convert.ToString(
                                                            Convert.ToInt32(
                                                                ChargesDashBoard
                                                    .AncillaryGrossCharges)),
                                                        currentYear),
                                                OPChargesCustomModel =
                                                    bal.GetDBChargesDashBoard(
                                                        facilityId,
                                                        corporateId,
                                                        Convert.ToString(
                                                            Convert.ToInt32(
                                                                ChargesDashBoard
                                                    .OutpatientGrossCharges)),
                                                        currentYear),
                                                ERChargesCustomModel =
                                                    bal.GetDBChargesDashBoard(
                                                        facilityId,
                                                        corporateId,
                                                        Convert.ToString(
                                                            Convert.ToInt32(
                                                                ChargesDashBoard
                                                    .EmergencyRoomGrossCharges)),
                                                        currentYear),
                                                IPRevenueCustomModel =
                                                    bal.GetDBChargesDashBoard(
                                                        facilityId,
                                                        corporateId,
                                                        Convert.ToString(
                                                            Convert.ToInt32(
                                                                ChargesDashBoard
                                                    .InpatientGrossRevenuePerPatientDay)),
                                                        currentYear),
                                                OPRevenueCustomModel =
                                                    bal.GetDBChargesDashBoard(
                                                        facilityId,
                                                        corporateId,
                                                        Convert.ToString(
                                                            Convert.ToInt32(
                                                                ChargesDashBoard
                                                    .OutpatientRevenuePerEncounter)),
                                                        currentYear),
                                                ERRevenueCustomModel =
                                                    bal.GetDBChargesDashBoard(
                                                        facilityId,
                                                        corporateId,
                                                        Convert.ToString(
                                                            Convert.ToInt32(
                                                                ChargesDashBoard
                                                    .ERpatientRevenuePerEncounter)),
                                                        currentYear)
                                            };
                    return PartialView(PartialViews.ChargesStats, dashboardview);
                }
                if (dashBoardType == "Volume")
                {
                    var facilityId = Helpers.GetDefaultFacilityId();
                    var corporateId = Helpers.GetSysAdminCorporateID();
                    var currentDateTime = Helpers.GetInvariantCultureDateTime(facilityId);
                    var currentYear = Convert.ToString(currentDateTime.Year);

                    //var chargesapplied = bal.SetDashBoardCounterActuals(facilityId, corporateId,
                    //    currentYear);
                    var dashboardview = new PatientVolumeDashboardView
                                            {
                                                CurrentDate =
                                                    Helpers.GetInvariantCultureDateTime(),
                                                NumberofCurrentDay =
                                                    Helpers.GetInvariantCultureDateTime()
                                                    .DayOfYear,
                                                IPEncountersCustomModel =
                                                    bal.GetDBChargesDashBoard(
                                                        facilityId,
                                                        corporateId,
                                                        Convert.ToString(
                                                            Convert.ToInt32(
                                                                ChargesDashBoard.PatientDays)),
                                                        currentYear),
                                                OPEncountersCustomModel =
                                                    bal.GetDBChargesDashBoard(
                                                        facilityId,
                                                        corporateId,
                                                        Convert.ToString(
                                                            Convert.ToInt32(
                                                                ChargesDashBoard
                                                    .OutpatientEncounters)),
                                                        currentYear),
                                                EREncountersCustomModel =
                                                    bal.GetDBChargesDashBoard(
                                                        facilityId,
                                                        corporateId,
                                                        Convert.ToString(
                                                            Convert.ToInt32(
                                                                ChargesDashBoard.EREncounters)),
                                                        currentYear),
                                                DisChargesCustomModel =
                                                    bal.GetDBChargesDashBoard(
                                                        facilityId,
                                                        corporateId,
                                                        Convert.ToString(
                                                            Convert.ToInt32(
                                                                ChargesDashBoard.Discharges)),
                                                        currentYear),
                                                ALOSCustomModel =
                                                    bal.GetDBChargesDashBoard(
                                                        facilityId,
                                                        corporateId,
                                                        Convert.ToString(
                                                            Convert.ToInt32(
                                                                ChargesDashBoard.ALOS)),
                                                        currentYear),
                                                IPADCCustomModel =
                                                    bal.GetDBChargesDashBoard(
                                                        facilityId,
                                                        corporateId,
                                                        Convert.ToString(
                                                            Convert.ToInt32(
                                                                ChargesDashBoard
                                                    .InpatientsADC)),
                                                        currentYear),
                                                PatientDaysCustomModel =
                                                    bal.GetDBChargesDashBoard(
                                                        facilityId,
                                                        corporateId,
                                                        Convert.ToString(
                                                            Convert.ToInt32(
                                                                ChargesDashBoard.Admissions)),
                                                        currentYear)
                                            };
                    return PartialView(PartialViews.PatientVolumeStats, dashboardview);
                }
            }
            return null;
        }

        /// <summary>
        ///     Gets the claim edits value.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        private double GetClaimEditsValue(DashboardChargesCustomModel item)
        {
            var currentMonth = Helpers.GetInvariantCultureDateTime().Month;
            decimal? value = null;
            switch (currentMonth)
            {
                case 1:
                    value = item.M1;
                    break;
                case 2:
                    value = item.M2;
                    break;
                case 3:
                    value = item.M3;
                    break;
                case 4:
                    value = item.M4;
                    break;
                case 5:
                    value = item.M5;
                    break;
                case 6:
                    value = item.M6;
                    break;
                case 7:
                    value = item.M7;
                    break;
                case 8:
                    value = item.M8;
                    break;
                case 9:
                    value = item.M9;
                    break;
                case 10:
                    value = item.M10;
                    break;
                case 11:
                    value = item.M11;
                    break;
                case 12:
                    value = item.M12;
                    break;
            }
            return value == null ? 0.0 : Convert.ToDouble(value);
        }

        #region CMO Dashboard

        public ActionResult Cmo()
        {
            using (var encounterBal = new EncounterBal())
            {
                var facilityId = Helpers.GetDefaultFacilityId();
                var corporateId = Helpers.GetSysAdminCorporateID();
                var cmoDashboardData = encounterBal.GetCMODashboardData(facilityId, corporateId);

                var activeEncounter = new ActiveEncounterCustomView
                                          {
                                              //In Patients List Where EncounterPatientType is In Patient
                                              ActiveInPatientEncounterList =
                                                  cmoDashboardData.Where(
                                                      x =>
                                                      x.EncounterPatientType
                                                      == Convert.ToInt32(
                                                          EncounterPatientType.InPatient))
                                                  .ToList(),
                                              ActiveOutPatientEncounterList =
                                                  cmoDashboardData.Where(
                                                      x =>
                                                      x.EncounterPatientType
                                                      == Convert.ToInt32(
                                                          EncounterPatientType.OutPatient))
                                                  .ToList(),
                                              ActiveEmergencyEncounterList =
                                                  cmoDashboardData.Where(
                                                      x =>
                                                      x.EncounterPatientType
                                                      == Convert.ToInt32(
                                                          EncounterPatientType.ERPatient))
                                                  .ToList()
                                          };

                using (var bal = new RoleTabsBal())
                {
                    var roleId = Helpers.GetDefaultRoleId();
                    activeEncounter.EncounterViewAccessible = (bal.CheckIfTabNameAccessibleToGivenRole(
                        "Admit Patient",
                        ControllerAccess.PatientSearch.ToString(),
                        ActionNameAccess.PatientSearch.ToString(),
                        Convert.ToInt32(roleId))
                                                               && bal.CheckIfTabNameAccessibleToGivenRole(
                                                                   "Start Outpatient visit",
                                                                   ControllerAccess.PatientSearch.ToString(),
                                                                   ActionNameAccess.PatientSearch.ToString(),
                                                                   Convert.ToInt32(roleId)));
                    activeEncounter.EndEncounterViewAccessible =
                        (bal.CheckIfTabNameAccessibleToGivenRole(
                            "Discharge patient",
                            ControllerAccess.PatientSearch.ToString(),
                            ActionNameAccess.PatientSearch.ToString(),
                            Convert.ToInt32(roleId))
                         && bal.CheckIfTabNameAccessibleToGivenRole(
                             "Close Outpatient visit",
                             ControllerAccess.PatientSearch.ToString(),
                             ActionNameAccess.PatientSearch.ToString(),
                             Convert.ToInt32(roleId)));

                    activeEncounter.DiagnosisViewAccessible =
                        bal.CheckIfTabNameAccessibleToGivenRole(
                            "Additional Diagnosis",
                            ControllerAccess.ActiveEncounter.ToString(),
                            ActionNameAccess.ActiveEncounter.ToString(),
                            Convert.ToInt32(roleId));
                }
                return View(activeEncounter);
            }
        }

        #endregion

        #region Bed Occupancy Dashboard View

        /// <summary>
        ///     Beds the occupancy.
        /// </summary>
        /// <returns></returns>
        public ActionResult BedOccupancy()
        {
            using (var dashboardBal = new DashboardBal())
            {
                var facilityId = Helpers.GetDefaultFacilityId();
                var corporateId = Helpers.GetSysAdminCorporateID();
                var objData = dashboardBal.GetBedOccupencyByFloorData(corporateId, facilityId);
                var bedOccupencyList = new List<BedOccupancyFloorDashboard>();
                if (objData.Any())
                {
                    bedOccupencyList =
                        (objData.GroupJoin(
                            objData,
                            c => c.Floor,
                            o => o.Floor,
                            (c, result) => new BedOccupancyFloorDashboard(c.Floor, c.Department, result))
                            .DistinctBy(x => x.ParentDepartmentName)).ToList();
                }
                var currentBedOccupencyRate = dashboardBal.GetDashboardChartData(corporateId, facilityId);

                var dashboardview = new DashboardView
                                        {
                                            BedOccupencyList = bedOccupencyList.ToList(),
                                            CurrentBedOccupency = new BedOccupancyCustomModel()
                                        };
                if (currentBedOccupencyRate.Any())
                {
                    dashboardview.CurrentBedOccupency.VacantBeds = currentBedOccupencyRate[0].Beds;
                    dashboardview.CurrentBedOccupency.OccupiedBeds = currentBedOccupencyRate.Count > 1
                                                                         ? currentBedOccupencyRate[1].Beds
                                                                         : 0;
                    dashboardview.CurrentBedOccupency.OccupiedRate = currentBedOccupencyRate.Count > 1
                                                                         ? currentBedOccupencyRate[0].TotalBeds > 0
                                                                               ? (Convert.ToDecimal(
                                                                                   currentBedOccupencyRate[1].Beds)
                                                                                  / Convert.ToDecimal(
                                                                                      currentBedOccupencyRate[0]
                                                                                        .TotalBeds))
                                                                               : 0
                                                                         : 0;
                    dashboardview.CurrentBedOccupency.TotalBeds = currentBedOccupencyRate.Count > 1
                                                                      ? currentBedOccupencyRate[1].TotalBeds
                                                                      : currentBedOccupencyRate[0].TotalBeds;
                }
                return View(dashboardview);
            }
        }

        /// <summary>
        ///     Gets the chart data collection.
        /// </summary>
        /// <param name="facilityId"></param>
        /// <param name="corporateId"></param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public ActionResult GetBedOccupancyChartDataCollection(
            int facilityId,
            int corporateId,
            [DataSourceRequest] DataSourceRequest request)
        {
            if (facilityId == 0) facilityId = Helpers.GetDefaultFacilityId();

            if (corporateId == 0) corporateId = Helpers.GetSysAdminCorporateID();

            var jsonResult = Json(
                GetChartData(corporateId, facilityId).ToDataSourceResult(request),
                JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        /// <summary>
        ///     Gets the chart data.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        private IEnumerable<BedOccupancyCustomModel> GetChartData(int corporateId, int facilityId)
        {
            using (var dashboardBal = new DashboardBal())
            {
                List<BedOccupancyCustomModel> objData = dashboardBal.GetDashboardChartData(corporateId, facilityId);
                return objData;
            }
        }

        public JsonResult RebindBedOccupancyData(int facilityId, int corporateId)
        {
            using (var dashboardBal = new DashboardBal())
            {
                var objData = dashboardBal.GetBedOccupencyByFloorData(corporateId, facilityId);
                var bedOccupencyList = new List<BedOccupancyFloorDashboard>();
                if (objData.Any())
                {
                    bedOccupencyList =
                        (objData.GroupJoin(
                            objData,
                            c => c.Floor,
                            o => o.Floor,
                            (c, result) => new BedOccupancyFloorDashboard(c.Floor, c.Department, result))
                            .DistinctBy(x => x.ParentDepartmentName)).ToList();
                }

                var currentBedOccupencyRate = dashboardBal.GetDashboardChartData(corporateId, facilityId);
                var currentBedsStats = new BedOccupancyCustomModel();

                if (currentBedOccupencyRate.Any())
                {
                    currentBedsStats.VacantBeds = currentBedOccupencyRate[0].Beds;
                    currentBedsStats.OccupiedBeds = currentBedOccupencyRate.Count > 1
                                                        ? currentBedOccupencyRate[1].Beds
                                                        : 0;
                    currentBedsStats.OccupiedRate = currentBedOccupencyRate.Count > 1
                                                        ? currentBedOccupencyRate[0].TotalBeds > 0
                                                              ? (Convert.ToDecimal(currentBedOccupencyRate[1].Beds)
                                                                 / Convert.ToDecimal(
                                                                     currentBedOccupencyRate[0].TotalBeds))
                                                              : 0
                                                        : 0;
                    currentBedsStats.TotalBeds = currentBedOccupencyRate.Count > 1
                                                     ? currentBedOccupencyRate[1].TotalBeds
                                                     : currentBedOccupencyRate[0].TotalBeds;
                }

                var partialViewResult = RenderPartialViewToString(
                    PartialViews.TotalBedsListPartialView,
                    bedOccupencyList);
                var jsonResult = new { partialViewResult, currentBedsStats };
                return Json(jsonResult, JsonRequestBehavior.AllowGet);
            }
        }

        private string RenderPartialViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName)) viewName = ControllerContext.RouteData.GetRequiredString("action");

            ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

        #endregion

        #region Encounter Type Dashboard View

        public ActionResult EncounterType()
        {
            return View();
        }

        /// <summary>
        ///     Gets the chart data collection.
        /// </summary>
        /// <param name="displayType">The display type.</param>
        /// <returns></returns>
        //public JsonResult GetEncounterTypeChart(string displayType, [DataSourceRequest] DataSourceRequest request)
        //{
        //    using (var dashboardBal = new DashboardBal())
        //    {
        //        int facilityId = Helpers.GetDefaultFacilityId();
        //        int corporateId = Helpers.GetSysAdminCorporateID();
        //        var objDataSource = dashboardBal.GetEncounterTypeData(corporateId, facilityId, displayType, new DateTime(DateTime.Today.Year, 1, 1), DateTime.Today).ToDataSourceResult(request);
        //        var jsonResult = Json(objDataSource, JsonRequestBehavior.AllowGet);
        //        jsonResult.MaxJsonLength = int.MaxValue;
        //        return jsonResult;
        //    }
        //}
        public JsonResult GetEncounterTypeChartData(string displayType)
        {
            using (var dashboardBal = new DashboardBal())
            {
                int facilityId = Helpers.GetDefaultFacilityId();
                int corporateId = Helpers.GetSysAdminCorporateID();
                var objDataSource = dashboardBal.GetEncounterTypeData(
                    corporateId,
                    facilityId,
                    displayType,
                    new DateTime(DateTime.Today.Year, 1, 1),
                    DateTime.Today);
                var jsonResult = Json(objDataSource, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
        }

        #endregion

        #region Registration Productivity Dashboard View

        public ActionResult RegistrationProductivity()
        {
            return View();
        }

        /// <summary>
        ///     Gets the chart data collection.
        /// </summary>
        /// <param name="displayType">The display type.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public JsonResult GetRegistrationProductivityChart(
            int displayType,
            [DataSourceRequest] DataSourceRequest request)
        {
            int facilityId = Helpers.GetDefaultFacilityId();
            //int corporateId = Helpers.GetSysAdminCorporateID();
            var jsonResult = Json(
                GetRegistrationProductivityData(facilityId, displayType).ToDataSourceResult(request),
                JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        /// <summary>
        ///     Gets the chart data.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="displayTypeId">The display type identifier.</param>
        /// <returns></returns>
        public IEnumerable<RegistrationProductivity> GetRegistrationProductivityData(int facilityId, int displayTypeId)
        {
            using (var bal = new DashboardBal())
            {
                var dt = Helpers.GetInvariantCultureDateTime();
                //var currentDay = DateTime.Now.Day;
                //dt = dt.AddDays(-currentDay);

                var objData = bal.GetRegistrationProductivityData(facilityId, displayTypeId, dt);
                return objData;
            }
        }

        /// <summary>
        ///     Gets the highchart registraion dat a.
        /// </summary>
        /// <param name="budgetFor">The budget for.</param>
        /// <returns></returns>
        public ActionResult GetHighchartRegistraionData(string budgetFor)
        {
            var facilityId = Helpers.GetDefaultFacilityId();
            var corporateId = Helpers.GetDefaultCorporateId();

            var currentDateTime = Helpers.GetInvariantCultureDateTime(facilityId);
            var currentYear = Convert.ToString(currentDateTime.Year);
            using (var bal = new DashboardBal())
            {
                var jsonResult = bal.GetHighChartsRegistrationProductivityData(
                    facilityId,
                    corporateId,
                    "2",
                    currentYear,
                    budgetFor);
                return Json(jsonResult, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        ///     Gets the highchart registraion dat a.
        /// </summary>
        /// <param name="budgetFor">The budget for.</param>
        /// <param name="budgetType">Type of the budget.</param>
        /// <returns></returns>
        public ActionResult GetHighchartProductivityData(string budgetFor, string budgetType)
        {
            var facilityId = Helpers.GetDefaultFacilityId();
            var corporateId = Helpers.GetDefaultCorporateId();
            var currentDateTime = Helpers.GetInvariantCultureDateTime();
            var currentYear = Convert.ToString(currentDateTime.Year);

            using (var bal = new DashboardBal())
            {
                var jsonResult = bal.GetHighChartsRegistrationProductivityData(
                    facilityId,
                    corporateId,
                    budgetType,
                    currentYear,
                    budgetFor);
                return Json(jsonResult, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region WorkFlow Indicator Analysis

        public ActionResult WorkflowIndicator()
        {
            return View();
        }

        /// <summary>
        ///     Gets the chart data collection.
        /// </summary>
        /// <param name="displayType">The display type.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public ActionResult GetOutPatientVisitsChart(int displayType, [DataSourceRequest] DataSourceRequest request)
        {
            var facilityId = Helpers.GetDefaultFacilityId();
            var jsonResult = Json(
                GetOutPatientVisitsChartData(facilityId, displayType).ToDataSourceResult(request),
                JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        /// <summary>
        ///     Gets the chart data.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="displayTypeId">The display type identifier.</param>
        /// <returns></returns>
        public IEnumerable<PatientBillingTrend> GetOutPatientVisitsChartData(int facilityId, int displayTypeId)
        {
            using (var bal = new DashboardBal())
            {
                //var dt = DateTime.Today;
                var objData = bal.GetOutPatientVisits(Convert.ToString(displayTypeId));
                return objData;
            }
        }

        /// <summary>
        ///     Gets the chart data collection.
        /// </summary>
        /// <param name="displayType">The display type.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public ActionResult GetInPatientDischargesChart(int displayType, [DataSourceRequest] DataSourceRequest request)
        {
            var facilityId = Helpers.GetDefaultFacilityId();
            var jsonResult = Json(
                GetInPatientDischargesChartData(facilityId, displayType).ToDataSourceResult(request),
                JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        /// <summary>
        ///     Gets the chart data.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="displayTypeId">The display type identifier.</param>
        /// <returns></returns>
        public IEnumerable<PatientBillingTrend> GetInPatientDischargesChartData(int facilityId, int displayTypeId)
        {
            using (var bal = new DashboardBal())
            {
                //var dt = DateTime.Today;
                var objData = bal.GetInPatientDischarges(Convert.ToString(displayTypeId));
                return objData;
            }
        }

        /// <summary>
        ///     Gets the database budget actual chart.
        /// </summary>
        /// <param name="displayType">The display type.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public ActionResult GetDBBudgetActualChart(int displayType, [DataSourceRequest] DataSourceRequest request)
        {
            var facilityId = Helpers.GetDefaultFacilityId();
            var corporateId = Helpers.GetSysAdminCorporateID();
            var currentDateTime = Helpers.GetInvariantCultureDateTime(facilityId);
            var currentYear = Convert.ToString(currentDateTime.Year);

            using (var bal = new DashboardBudgetBal())
            {
                var chartData =
                    bal.GetDBBudgetActual(
                        facilityId,
                        corporateId,
                        Convert.ToString(displayType),
                        currentYear).ToDataSourceResult(request);
                var jsonResult = Json(chartData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
        }

        /// <summary>
        ///     Gets the billing trend data.
        /// </summary>
        /// <param name="diaplayFor">The diaplay for.</param>
        /// <returns></returns>
        public ActionResult GetBillingTrendData(string diaplayFor)
        {
            var facilityId = Helpers.GetDefaultFacilityId();
            var corporateId = Helpers.GetDefaultCorporateId();
            var currentDateTime = Helpers.GetInvariantCultureDateTime();
            var currentYear = Convert.ToString(currentDateTime.Year);

            using (var bal = new DashboardBal())
            {
                var jsonResult = bal.GetHighChartsBillingTrendData(
                    facilityId,
                    corporateId,
                    diaplayFor,
                    currentYear);
                return Json(jsonResult, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Bill Scrubber

        ///// <summary>
        ///// Gets the denials coded by physicians.
        ///// </summary>
        ///// <param name="displayType">The display type.</param>
        ///// <param name="request">The request.</param>
        ///// <returns></returns>
        //public ActionResult GetDenialsCodedByPhysicians(int displayType, [DataSourceRequest] DataSourceRequest request)
        //{
        //    var facilityId = Helpers.GetDefaultFacilityId();
        //    using (var bal = new DashboardBal())
        //    {
        //        var dataSource = bal.GetDenialsCodedByPhysicians(facilityId).ToDataSourceResult(request);
        //        var jsonResult = Json(dataSource, JsonRequestBehavior.AllowGet);
        //        jsonResult.MaxJsonLength = int.MaxValue;
        //        return jsonResult;
        //    }
        //}

        ///// <summary>
        ///// Gets the denialsby denial code.
        ///// </summary>
        ///// <param name="displayType">The display type.</param>
        ///// <param name="request">The request.</param>
        ///// <returns></returns>
        //public ActionResult GetDenialsbyDenialCode(int displayType, [DataSourceRequest] DataSourceRequest request)
        //{
        //    var facilityId = Helpers.GetDefaultFacilityId();
        //    var corporateId = Helpers.GetSysAdminCorporateID();
        //    using (var bal = new ReportingBal())
        //    {
        //        var currentMonthdate = DateTime.Now;
        //        var firstDayOfMonth = new DateTime(currentMonthdate.Year, 1, 1);
        //        var dataSource = bal.GetDenialCodesReport(corporateId, facilityId, firstDayOfMonth, DateTime.Now, displayType).ToDataSourceResult(request);
        //        var jsonResult = Json(dataSource, JsonRequestBehavior.AllowGet);
        //        jsonResult.MaxJsonLength = int.MaxValue;
        //        return jsonResult;
        //    }
        //}

        //public ActionResult GetDenialsForFirstSubmmision(int displayType, [DataSourceRequest] DataSourceRequest request)
        //{
        //    using (var bal = new DashboardBal())
        //    {
        //        var dataSource = bal.GetClaimDenialPercent().ToDataSourceResult(request);
        //        var jsonResult = Json(dataSource, JsonRequestBehavior.AllowGet);
        //        jsonResult.MaxJsonLength = int.MaxValue;
        //        return jsonResult;
        //    }
        //}

        public ActionResult BillScrubber()
        {
            using (var bal = new DashboardBudgetBal())
            {
                var facilityId = Helpers.GetDefaultFacilityId();
                var corporateId = Helpers.GetSysAdminCorporateID();
                //var chargesapplied = bal.SetDashBoardChargesActuals(facilityId, corporateId, currentYear);
                var currentDateTime = Helpers.GetInvariantCultureDateTime(facilityId);
                var currentYear = Convert.ToString(currentDateTime.Year);

                var claimsList = bal.GetDBChargesDashBoard(
                    facilityId,
                    corporateId,
                    "38",
                    currentYear);

                var dashboardview = new BillScrubberDashboardView
                                        {
                                            ClaimsAcceptancePercentageFirstSubmission =
                                                bal.GetDBChargesDashBoard(
                                                    facilityId,
                                                    corporateId,
                                                    Convert.ToString(
                                                        Convert.ToInt32(
                                                            ChargesDashBoard
                                                .NumberofTotalClaimsPaidonRemittance)),
                                                    currentYear),
                                            NumberofTotalClaimsPaidonRemittance =
                                                bal.GetDBChargesDashBoard(
                                                    facilityId,
                                                    corporateId,
                                                    Convert.ToString(
                                                        Convert.ToInt32(
                                                            ChargesDashBoard
                                                .NumberofTotalClaimsDeniedonRemittance)),
                                                    currentYear),
                                            NumberofTotalClaimsDeniedonRemittance =
                                                bal.GetDBChargesDashBoard(
                                                    facilityId,
                                                    corporateId,
                                                    Convert.ToString(
                                                        Convert.ToInt32(
                                                            ChargesDashBoard
                                                .ClaimsAcceptancePercentageFirstSubmission)),
                                                    currentYear),
                                            ClaimsValue =
                                                claimsList.Count > 0
                                                    ? GetClaimEditsValue(claimsList[0])
                                                    : 0
                                        };
                return View(dashboardview);
            }
        }

        public ActionResult GetDenialsCodedByPhysicians3D()
        {
            var facilityId = Helpers.GetDefaultFacilityId();
            using (var bal = new DashboardBal())
            {
                var dataSource = bal.GetDenialsCodedByPhysicians(facilityId);
                var jsonResult = Json(dataSource, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
        }

        /// <summary>
        ///     Gets the denialsby denial code.
        /// </summary>
        /// <param name="displayType">The display type.</param>
        /// <returns></returns>
        public JsonResult GetDenialsbyDenialCode3D(int displayType)
        {
            var facilityId = Helpers.GetDefaultFacilityId();
            var corporateId = Helpers.GetSysAdminCorporateID();
            var currentDateTime = Helpers.GetInvariantCultureDateTime(facilityId);
            using (var bal = new ReportingBal())
            {
                var currentMonthdate = currentDateTime;
                var firstDayOfMonth = new DateTime(currentMonthdate.Year, 1, 1);
                var dataSource = bal.GetDenialCodesReport(corporateId, facilityId, firstDayOfMonth, currentDateTime,
                    displayType);
                var jsonResult = Json(dataSource, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
        }

        /// <summary>
        ///     Gets the denials for first submmision3 d.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetDenialsForFirstSubmmision3D()
        {
            using (var bal = new DashboardBal())
            {
                var dataSource = bal.GetClaimDenialPercent();
                var jsonResult = Json(dataSource, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
        }

        #endregion

        #region Charges Dashboard

        public ActionResult ChargesDashboardView()
        {
            return View();
        }

        /// <summary>
        ///     Charges dashboard.
        /// </summary>
        /// <returns></returns>
        public ActionResult ChargesDashboard()
        {
            using (var bal = new DashboardBudgetBal())
            {
                var facilityId = Helpers.GetDefaultFacilityId();
                var corporateId = Helpers.GetSysAdminCorporateID();
                var currentDateTime = Helpers.GetInvariantCultureDateTime(facilityId);
                var currentYear = Convert.ToString(currentDateTime.Year);

                bal.SetDashBoardChargesActuals(facilityId, corporateId, currentYear);
                //bal.SetDashBoardCounterActuals(facilityId, corporateId, currentYear);
                var dashboardview = new ChargesDashboardView
                                        {
                                            CurrentDate = Helpers.GetInvariantCultureDateTime(),
                                            NumberofCurrentDay =
                                                Helpers.GetInvariantCultureDateTime().DayOfYear,
                                            CurrentFacilityId = facilityId,
                                            RoomChargesCustomModel =
                                                bal.GetDBChargesDashBoard(
                                                    facilityId,
                                                    corporateId,
                                                    Convert.ToString(
                                                        Convert.ToInt32(
                                                            ChargesDashBoard.RoomGrossCharges)),
                                                    currentYear),
                                            IPChargesCustomModel =
                                                bal.GetDBChargesDashBoard(
                                                    facilityId,
                                                    corporateId,
                                                    Convert.ToString(
                                                        Convert.ToInt32(
                                                            ChargesDashBoard.AncillaryGrossCharges)),
                                                    currentYear),
                                            OPChargesCustomModel =
                                                bal.GetDBChargesDashBoard(
                                                    facilityId,
                                                    corporateId,
                                                    Convert.ToString(
                                                        Convert.ToInt32(
                                                            ChargesDashBoard.OutpatientGrossCharges)),
                                                    currentYear),
                                            ERChargesCustomModel =
                                                bal.GetDBChargesDashBoard(
                                                    facilityId,
                                                    corporateId,
                                                    Convert.ToString(
                                                        Convert.ToInt32(
                                                            ChargesDashBoard
                                                .EmergencyRoomGrossCharges)),
                                                    currentYear),
                                            IPRevenueCustomModel =
                                                bal.GetDBChargesDashBoard(
                                                    facilityId,
                                                    corporateId,
                                                    Convert.ToString(
                                                        Convert.ToInt32(
                                                            ChargesDashBoard
                                                .InpatientGrossRevenuePerPatientDay)),
                                                    currentYear),
                                            OPRevenueCustomModel =
                                                bal.GetDBChargesDashBoard(
                                                    facilityId,
                                                    corporateId,
                                                    Convert.ToString(
                                                        Convert.ToInt32(
                                                            ChargesDashBoard
                                                .OutpatientRevenuePerEncounter)),
                                                    currentYear),
                                            ERRevenueCustomModel =
                                                bal.GetDBChargesDashBoard(
                                                    facilityId,
                                                    corporateId,
                                                    Convert.ToString(
                                                        Convert.ToInt32(
                                                            ChargesDashBoard
                                                .ERpatientRevenuePerEncounter)),
                                                    currentYear)
                                        };
                return View(dashboardview);
            }
        }

        /// <summary>
        ///     Gets the facility dashboard data.
        /// </summary>
        /// <param name="facilityID">The facility identifier.</param>
        /// <param name="fiscalyear">The fiscalyear.</param>
        /// <returns></returns>
        public ActionResult GetFacilityDashboardData(int? facilityID, string fiscalyear)
        {
            using (var bal = new DashboardBudgetBal())
            {
                var facilityId = facilityID != 0 ? facilityID ?? Helpers.GetDefaultFacilityId() : -1;
                var corporateId = Helpers.GetSysAdminCorporateID();
                var chargesapplied = facilityId == -1
                                     || bal.SetDashBoardChargesActuals(facilityId, corporateId, fiscalyear);
                chargesapplied = facilityId == -1 || bal.SetDashBoardCounterActuals(facilityId, corporateId, fiscalyear);
                var dashboardview = new ChargesDashboardView
                                        {
                                            CurrentDate = Helpers.GetInvariantCultureDateTime(),
                                            NumberofCurrentDay =
                                                Helpers.GetInvariantCultureDateTime().DayOfYear,
                                            CurrentFacilityId = facilityId,
                                            RoomChargesCustomModel =
                                                bal.GetDBChargesDashBoard(
                                                    facilityId,
                                                    corporateId,
                                                    Convert.ToString(
                                                        Convert.ToInt32(
                                                            ChargesDashBoard.RoomGrossCharges)),
                                                    fiscalyear),
                                            IPChargesCustomModel =
                                                bal.GetDBChargesDashBoard(
                                                    facilityId,
                                                    corporateId,
                                                    Convert.ToString(
                                                        Convert.ToInt32(
                                                            ChargesDashBoard.AncillaryGrossCharges)),
                                                    fiscalyear),
                                            OPChargesCustomModel =
                                                bal.GetDBChargesDashBoard(
                                                    facilityId,
                                                    corporateId,
                                                    Convert.ToString(
                                                        Convert.ToInt32(
                                                            ChargesDashBoard.OutpatientGrossCharges)),
                                                    fiscalyear),
                                            ERChargesCustomModel =
                                                bal.GetDBChargesDashBoard(
                                                    facilityId,
                                                    corporateId,
                                                    Convert.ToString(
                                                        Convert.ToInt32(
                                                            ChargesDashBoard
                                                .EmergencyRoomGrossCharges)),
                                                    fiscalyear),
                                            IPRevenueCustomModel =
                                                bal.GetDBChargesDashBoard(
                                                    facilityId,
                                                    corporateId,
                                                    Convert.ToString(
                                                        Convert.ToInt32(
                                                            ChargesDashBoard
                                                .InpatientGrossRevenuePerPatientDay)),
                                                    fiscalyear),
                                            OPRevenueCustomModel =
                                                bal.GetDBChargesDashBoard(
                                                    facilityId,
                                                    corporateId,
                                                    Convert.ToString(
                                                        Convert.ToInt32(
                                                            ChargesDashBoard
                                                .OutpatientRevenuePerEncounter)),
                                                    fiscalyear),
                                            ERRevenueCustomModel =
                                                bal.GetDBChargesDashBoard(
                                                    facilityId,
                                                    corporateId,
                                                    Convert.ToString(
                                                        Convert.ToInt32(
                                                            ChargesDashBoard
                                                .ERpatientRevenuePerEncounter)),
                                                    fiscalyear)
                                        };
                return PartialView(PartialViews.ChargesStats, dashboardview);
            }
        }

        /// <summary>
        ///     Gets the charges dashboard data.
        /// </summary>
        /// <param name="facilityID">The facility identifier.</param>
        /// <param name="fiscalyear">The fiscalyear.</param>
        /// <param name="budgetFor">The budget for.</param>
        /// <returns></returns>
        public JsonResult GetChargesDashboardData(int? facilityID, string fiscalyear, string budgetFor)
        {
            using (var bal = new DashboardBudgetBal())
            {
                var facilityId = facilityID ?? Helpers.GetDefaultFacilityId();
                var corporateId = Helpers.GetSysAdminCorporateID();
                var list = bal.GetDBChargesChartDashBoard(
                    facilityId,
                    corporateId,
                    Convert.ToString(budgetFor),
                    fiscalyear);
                return Json(list.Any() ? list : null, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        ///     Patients the volume dashboard.
        /// </summary>
        /// <returns></returns>
        public ActionResult PatientVolumeDashboard()
        {
            using (var bal = new DashboardBudgetBal())
            {
                var facilityId = Helpers.GetDefaultFacilityId();
                var corporateId = Helpers.GetSysAdminCorporateID();

                var currentDateTime = Helpers.GetInvariantCultureDateTime(facilityId);
                var currentYear = Convert.ToString(currentDateTime.Year);

                //var chargesapplied = bal.SetDashBoardCounterActuals(facilityId, corporateId, currentYear);
                var dashboardview = new PatientVolumeDashboardView
                                        {
                                            CurrentFacilityId = facilityId,
                                            CurrentDate = Helpers.GetInvariantCultureDateTime(),
                                            NumberofCurrentDay =
                                                Helpers.GetInvariantCultureDateTime().DayOfYear,
                                            IPEncountersCustomModel =
                                                bal.GetDBChargesDashBoard(
                                                    facilityId,
                                                    corporateId,
                                                    Convert.ToString(
                                                        Convert.ToInt32(
                                                            ChargesDashBoard.PatientDays)),
                                                    currentYear),
                                            OPEncountersCustomModel =
                                                bal.GetDBChargesDashBoard(
                                                    facilityId,
                                                    corporateId,
                                                    Convert.ToString(
                                                        Convert.ToInt32(
                                                            ChargesDashBoard
                                                .OutpatientEncounters)),
                                                    currentYear),
                                            EREncountersCustomModel =
                                                bal.GetDBChargesDashBoard(
                                                    facilityId,
                                                    corporateId,
                                                    Convert.ToString(
                                                        Convert.ToInt32(
                                                            ChargesDashBoard.EREncounters)),
                                                    currentYear),
                                            DisChargesCustomModel =
                                                bal.GetDBChargesDashBoard(
                                                    facilityId,
                                                    corporateId,
                                                    Convert.ToString(
                                                        Convert.ToInt32(
                                                            ChargesDashBoard.Discharges)),
                                                    currentYear),
                                            ALOSCustomModel =
                                                bal.GetDBChargesDashBoard(
                                                    facilityId,
                                                    corporateId,
                                                    Convert.ToString(
                                                        Convert.ToInt32(ChargesDashBoard.ALOS)),
                                                    currentYear),
                                            IPADCCustomModel =
                                                bal.GetDBChargesDashBoard(
                                                    facilityId,
                                                    corporateId,
                                                    Convert.ToString(
                                                        Convert.ToInt32(
                                                            ChargesDashBoard.InpatientsADC)),
                                                    currentYear),
                                            PatientDaysCustomModel =
                                                bal.GetDBChargesDashBoard(
                                                    facilityId,
                                                    corporateId,
                                                    Convert.ToString(
                                                        Convert.ToInt32(
                                                            ChargesDashBoard.Admissions)),
                                                    currentYear)
                                        };
                return View(dashboardview);
            }
        }

        /// <summary>
        ///     Gets the facility dashboard data.
        /// </summary>
        /// <param name="facilityID">The facility identifier.</param>
        /// <param name="fiscalyear">The fiscalyear.</param>
        /// <returns></returns>
        public ActionResult GetPatientVolumeDashboardData(int? facilityID, string fiscalyear)
        {
            using (var bal = new DashboardBudgetBal())
            {
                //var facilityId = facilityID ?? Helpers.GetDefaultFacilityId();
                var facilityId = facilityID != 0 ? facilityID ?? Helpers.GetDefaultFacilityId() : -1;
                var corporateId = Helpers.GetSysAdminCorporateID();
                var chargesapplied = facilityId == -1
                                     || bal.SetDashBoardCounterActuals(facilityId, corporateId, fiscalyear);
                var dashboardview = new PatientVolumeDashboardView
                                        {
                                            CurrentFacilityId = facilityId,
                                            CurrentDate = Helpers.GetInvariantCultureDateTime(),
                                            NumberofCurrentDay =
                                                Helpers.GetInvariantCultureDateTime().DayOfYear,
                                            IPEncountersCustomModel =
                                                bal.GetDBChargesDashBoard(
                                                    facilityId,
                                                    corporateId,
                                                    Convert.ToString(
                                                        Convert.ToInt32(
                                                            ChargesDashBoard.PatientDays)),
                                                    fiscalyear),
                                            OPEncountersCustomModel =
                                                bal.GetDBChargesDashBoard(
                                                    facilityId,
                                                    corporateId,
                                                    Convert.ToString(
                                                        Convert.ToInt32(
                                                            ChargesDashBoard
                                                .OutpatientEncounters)),
                                                    fiscalyear),
                                            EREncountersCustomModel =
                                                bal.GetDBChargesDashBoard(
                                                    facilityId,
                                                    corporateId,
                                                    Convert.ToString(
                                                        Convert.ToInt32(
                                                            ChargesDashBoard.EREncounters)),
                                                    fiscalyear),
                                            DisChargesCustomModel =
                                                bal.GetDBChargesDashBoard(
                                                    facilityId,
                                                    corporateId,
                                                    Convert.ToString(
                                                        Convert.ToInt32(
                                                            ChargesDashBoard.Discharges)),
                                                    fiscalyear),
                                            ALOSCustomModel =
                                                bal.GetDBChargesDashBoard(
                                                    facilityId,
                                                    corporateId,
                                                    Convert.ToString(
                                                        Convert.ToInt32(ChargesDashBoard.ALOS)),
                                                    fiscalyear),
                                            IPADCCustomModel =
                                                bal.GetDBChargesDashBoard(
                                                    facilityId,
                                                    corporateId,
                                                    Convert.ToString(
                                                        Convert.ToInt32(
                                                            ChargesDashBoard.InpatientsADC)),
                                                    fiscalyear),
                                            PatientDaysCustomModel =
                                                bal.GetDBChargesDashBoard(
                                                    facilityId,
                                                    corporateId,
                                                    Convert.ToString(
                                                        Convert.ToInt32(
                                                            ChargesDashBoard.Admissions)),
                                                    fiscalyear)
                                        };
                return PartialView(PartialViews.PatientVolumeStats, dashboardview);
            }
        }

        #endregion
    }
}