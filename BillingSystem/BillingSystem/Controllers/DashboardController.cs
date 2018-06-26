using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Common;
using BillingSystem.Common.Common;
using BillingSystem.Model.CustomModel;
using BillingSystem.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.Ajax.Utilities;
using BillingSystem.Bal.Interfaces;
namespace BillingSystem.Controllers
{
    public class DashboardController : BaseController
    {
        private readonly IEncounterService _eService;
        private readonly IDashboardBudgetService _dbService;
        private readonly IDashboardService _dService;

        public DashboardController(IEncounterService eService, IDashboardBudgetService dbService, IDashboardService dService)
        {
            _eService = eService;
            _dbService = dbService;
            _dService = dService;
        }


        /// <summary>
        ///     Charges dashboard.
        /// </summary>
        /// <returns></returns>
        public ActionResult BillingDashboard()
        {
            var facilityId = Helpers.GetDefaultFacilityId();
            var corporateId = Helpers.GetSysAdminCorporateID();
            var currentDateTime = _eService.GetInvariantCultureDateTime(facilityId);
            var currentYear = Convert.ToString(currentDateTime.Year);

            //var chargesapplied = bal.SetDashBoardChargesActuals(facilityId, corporateId, currentYear);
            var dashboardview = new BillingDashboardView
            {
                CurrentDate = _eService.GetInvariantCultureDateTime(facilityId),
                NumberofCurrentDay = currentDateTime.DayOfYear,
                ClaimSubmmitedNumber = _dbService.GetDBChargesDashBoard(facilityId, corporateId, Convert.ToString((int)ChargesDashBoard.NumberofTotalClaimsSubmitted), currentYear),
                ClaimSubmmitedDollorAmount = _dbService.GetDBChargesDashBoard(facilityId, corporateId, Convert.ToString(Convert.ToInt32(ChargesDashBoard.DollarAmountofTotalClaimsSubmitted)), currentYear),
                ClaimSubmmitedInpatientNumber = _dbService.GetDBChargesDashBoard(facilityId, corporateId, Convert.ToString(Convert.ToInt32(ChargesDashBoard.NumberofTotalInpatientClaimsSubmitted)), currentYear),
                ClaimSubmmitedDollorInpatientAmount = _dbService.GetDBChargesDashBoard(facilityId, corporateId, Convert.ToString(Convert.ToInt32(ChargesDashBoard.DollarAmountofTotalInpatientClaimsSubmitted)), currentYear),
                ClaimSubmmitedOutpatientNumber = _dbService.GetDBChargesDashBoard(facilityId, corporateId, Convert.ToString(Convert.ToInt32(ChargesDashBoard.NumberofTotalOutpatientClaimsSubmitted)), currentYear),
                ClaimSubmmitedDollorOutpatientAmount = _dbService.GetDBChargesDashBoard(facilityId, corporateId, Convert.ToString(Convert.ToInt32(ChargesDashBoard.DollarAmountoftotalOutpatientClaimsSubmitted)), currentYear),
                ClaimSubmmitedERpatientNumber = _dbService.GetDBChargesDashBoard(facilityId, corporateId, Convert.ToString(Convert.ToInt32(ChargesDashBoard.NumberofTotalEmergencyRoomClaimsSubmitted)), currentYear),
                ClaimSubmmitedEROutpatientAmount = _dbService.GetDBChargesDashBoard(facilityId, corporateId, Convert.ToString(Convert.ToInt32(ChargesDashBoard.DollarAmountoftotalEmergencyRoomClaimsSubmitted)), currentYear),
                ClaimSubmmitedAvgDollorAmount = _dbService.GetDBChargesDashBoard(facilityId, corporateId, Convert.ToString(Convert.ToInt32(ChargesDashBoard.AverageDollarAmountofTotalClaims)), currentYear),
                ClaimSubmmitedAvgDollorAmountInPatinet =
                                            _dbService.GetDBChargesDashBoard(
                                                facilityId,
                                                corporateId,
                                                Convert.ToString(
                                                    Convert.ToInt32(
                                                        ChargesDashBoard
                                            .AverageDollarAmountofInpatientClaim)),
                                                currentYear),
                ClaimSubmmitedAvgDollorAmountOPPatinet =
                                            _dbService.GetDBChargesDashBoard(
                                                facilityId,
                                                corporateId,
                                                Convert.ToString(
                                                    Convert.ToInt32(
                                                        ChargesDashBoard
                                            .AverageDollarAmountofOutpatientClaim)),
                                                currentYear),
                ClaimSubmmitedAvgDollorAmountERPatinet =
                                            _dbService.GetDBChargesDashBoard(
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

        /// <summary>
        ///     Exports to excel.
        /// </summary>
        /// <param name="dashBoardType">Type of the dash board.</param>
        /// <returns></returns>
        public ActionResult ExportToExcel(string dashBoardType)
        {
            Response.AddHeader("Content-Type", "application/vnd.ms-excel");
            if (dashBoardType == "Charges")
            {
                var facilityId = Helpers.GetDefaultFacilityId();
                var corporateId = Helpers.GetSysAdminCorporateID();
                var currentDateTime = _eService.GetInvariantCultureDateTime(facilityId);
                var currentYear = Convert.ToString(currentDateTime.Year);

                // bal.SetDashBoardChargesActuals(facilityId, corporateId, currentYear);
                // bal.SetDashBoardCounterActuals(facilityId, corporateId, currentYear);
                var dashboardview = new ChargesDashboardView
                {
                    CurrentDate = _eService.GetInvariantCultureDateTime(facilityId),
                    NumberofCurrentDay =
                                                _eService.GetInvariantCultureDateTime(facilityId).DayOfYear,
                    RoomChargesCustomModel =
                                                _dbService.GetDBChargesDashBoard(
                                                    facilityId,
                                                    corporateId,
                                                    Convert.ToString(
                                                        Convert.ToInt32(
                                                            ChargesDashBoard.RoomGrossCharges)),
                                                    currentYear),
                    IPChargesCustomModel =
                                                _dbService.GetDBChargesDashBoard(
                                                    facilityId,
                                                    corporateId,
                                                    Convert.ToString(
                                                        Convert.ToInt32(
                                                            ChargesDashBoard
                                                .AncillaryGrossCharges)),
                                                    currentYear),
                    OPChargesCustomModel =
                                                _dbService.GetDBChargesDashBoard(
                                                    facilityId,
                                                    corporateId,
                                                    Convert.ToString(
                                                        Convert.ToInt32(
                                                            ChargesDashBoard
                                                .OutpatientGrossCharges)),
                                                    currentYear),
                    ERChargesCustomModel =
                                                _dbService.GetDBChargesDashBoard(
                                                    facilityId,
                                                    corporateId,
                                                    Convert.ToString(
                                                        Convert.ToInt32(
                                                            ChargesDashBoard
                                                .EmergencyRoomGrossCharges)),
                                                    currentYear),
                    IPRevenueCustomModel =
                                                _dbService.GetDBChargesDashBoard(
                                                    facilityId,
                                                    corporateId,
                                                    Convert.ToString(
                                                        Convert.ToInt32(
                                                            ChargesDashBoard
                                                .InpatientGrossRevenuePerPatientDay)),
                                                    currentYear),
                    OPRevenueCustomModel =
                                                _dbService.GetDBChargesDashBoard(
                                                    facilityId,
                                                    corporateId,
                                                    Convert.ToString(
                                                        Convert.ToInt32(
                                                            ChargesDashBoard
                                                .OutpatientRevenuePerEncounter)),
                                                    currentYear),
                    ERRevenueCustomModel =
                                                _dbService.GetDBChargesDashBoard(
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
                var currentDateTime = _eService.GetInvariantCultureDateTime(facilityId);
                var currentYear = Convert.ToString(currentDateTime.Year);

                //var chargesapplied = bal.SetDashBoardCounterActuals(facilityId, corporateId,
                //    currentYear);
                var dashboardview = new PatientVolumeDashboardView
                {
                    CurrentDate = _eService.GetInvariantCultureDateTime(facilityId),
                    NumberofCurrentDay = _eService.GetInvariantCultureDateTime(facilityId).DayOfYear,
                    IPEncountersCustomModel =
                                                _dbService.GetDBChargesDashBoard(
                                                    facilityId,
                                                    corporateId,
                                                    Convert.ToString(
                                                        Convert.ToInt32(
                                                            ChargesDashBoard.PatientDays)),
                                                    currentYear),
                    OPEncountersCustomModel =
                                                _dbService.GetDBChargesDashBoard(
                                                    facilityId,
                                                    corporateId,
                                                    Convert.ToString(
                                                        Convert.ToInt32(
                                                            ChargesDashBoard
                                                .OutpatientEncounters)),
                                                    currentYear),
                    EREncountersCustomModel =
                                                _dbService.GetDBChargesDashBoard(
                                                    facilityId,
                                                    corporateId,
                                                    Convert.ToString(
                                                        Convert.ToInt32(
                                                            ChargesDashBoard.EREncounters)),
                                                    currentYear),
                    DisChargesCustomModel =
                                                _dbService.GetDBChargesDashBoard(
                                                    facilityId,
                                                    corporateId,
                                                    Convert.ToString(
                                                        Convert.ToInt32(
                                                            ChargesDashBoard.Discharges)),
                                                    currentYear),
                    ALOSCustomModel =
                                                _dbService.GetDBChargesDashBoard(
                                                    facilityId,
                                                    corporateId,
                                                    Convert.ToString(
                                                        Convert.ToInt32(
                                                            ChargesDashBoard.ALOS)),
                                                    currentYear),
                    IPADCCustomModel =
                                                _dbService.GetDBChargesDashBoard(
                                                    facilityId,
                                                    corporateId,
                                                    Convert.ToString(
                                                        Convert.ToInt32(
                                                            ChargesDashBoard
                                                .InpatientsADC)),
                                                    currentYear),
                    PatientDaysCustomModel =
                                                _dbService.GetDBChargesDashBoard(
                                                    facilityId,
                                                    corporateId,
                                                    Convert.ToString(
                                                        Convert.ToInt32(
                                                            ChargesDashBoard.Admissions)),
                                                    currentYear)
                };
                return PartialView(PartialViews.PatientVolumeStats, dashboardview);
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
            var fId = Helpers.GetDefaultFacilityId();
            var currentMonth = _eService.GetInvariantCultureDateTime(fId).Month;
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
            var facilityId = Helpers.GetDefaultFacilityId();
            var corporateId = Helpers.GetSysAdminCorporateID();
            var cmoDashboardData = _eService.GetCMODashboardData(facilityId, corporateId);

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


        #endregion

        #region Bed Occupancy Dashboard View

        /// <summary>
        ///     Beds the occupancy.
        /// </summary>
        /// <returns></returns>
        public ActionResult BedOccupancy()
        {
            var facilityId = Helpers.GetDefaultFacilityId();
            var corporateId = Helpers.GetSysAdminCorporateID();
            var objData = _dService.GetBedOccupencyByFloorData(corporateId, facilityId);
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
            var currentBedOccupencyRate = _dService.GetDashboardChartData(corporateId, facilityId);

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
            List<BedOccupancyCustomModel> objData = _dService.GetDashboardChartData(corporateId, facilityId);
            return objData;
        }

        public JsonResult RebindBedOccupancyData(int facilityId, int corporateId)
        {
            var objData = _dService.GetBedOccupencyByFloorData(corporateId, facilityId);
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

            var currentBedOccupencyRate = _dService.GetDashboardChartData(corporateId, facilityId);
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
            int facilityId = Helpers.GetDefaultFacilityId();
            int corporateId = Helpers.GetSysAdminCorporateID();
            var objDataSource = _dService.GetEncounterTypeData(
                corporateId,
                facilityId,
                displayType,
                new DateTime(DateTime.Today.Year, 1, 1),
                DateTime.Today);
            var jsonResult = Json(objDataSource, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

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
            var dt = _eService.GetInvariantCultureDateTime(facilityId);
            //var currentDay = DateTime.Now.Day;
            //dt = dt.AddDays(-currentDay);

            var objData = _dService.GetRegistrationProductivityData(facilityId, displayTypeId, dt);
            return objData;

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

            var currentDateTime = _eService.GetInvariantCultureDateTime(facilityId);
            var currentYear = Convert.ToString(currentDateTime.Year);

            var jsonResult = _dService.GetHighChartsRegistrationProductivityData(
                facilityId,
                corporateId,
                "2",
                currentYear,
                budgetFor);
            return Json(jsonResult, JsonRequestBehavior.AllowGet);

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
            var currentDateTime = _eService.GetInvariantCultureDateTime(facilityId);
            var currentYear = Convert.ToString(currentDateTime.Year);

            var jsonResult = _dService.GetHighChartsRegistrationProductivityData(
                facilityId,
                corporateId,
                budgetType,
                currentYear,
                budgetFor);
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
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
            var objData = _dService.GetOutPatientVisits(Convert.ToString(displayTypeId));
            return objData;

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
            var objData = _dService.GetInPatientDischarges(Convert.ToString(displayTypeId));
            return objData;
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
            var currentDateTime = _eService.GetInvariantCultureDateTime(facilityId);
            var currentYear = Convert.ToString(currentDateTime.Year);

            var chartData =
                _dbService.GetDBBudgetActual(
                    facilityId,
                    corporateId,
                    Convert.ToString(displayType),
                    currentYear).ToDataSourceResult(request);
            var jsonResult = Json(chartData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
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
            var currentDateTime = _eService.GetInvariantCultureDateTime(facilityId);
            var currentYear = Convert.ToString(currentDateTime.Year);

            var jsonResult = _dService.GetHighChartsBillingTrendData(
                facilityId,
                corporateId,
                diaplayFor,
                currentYear);
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Bill Scrubber

        public ActionResult BillScrubber()
        {
            var facilityId = Helpers.GetDefaultFacilityId();
            var corporateId = Helpers.GetSysAdminCorporateID();
            var currentDateTime = _eService.GetInvariantCultureDateTime(facilityId);
            var currentYear = Convert.ToString(currentDateTime.Year);

            var claimsList = _dbService.GetDBChargesDashBoard(facilityId, corporateId, "38", currentYear);

            var dashboardview = new BillScrubberDashboardView
            {
                ClaimsAcceptancePercentageFirstSubmission =
                                            _dbService.GetDBChargesDashBoard(
                                                facilityId,
                                                corporateId,
                                                Convert.ToString(
                                                    Convert.ToInt32(
                                                        ChargesDashBoard
                                            .NumberofTotalClaimsPaidonRemittance)),
                                                currentYear),
                NumberofTotalClaimsPaidonRemittance =
                                            _dbService.GetDBChargesDashBoard(
                                                facilityId,
                                                corporateId,
                                                Convert.ToString(
                                                    Convert.ToInt32(
                                                        ChargesDashBoard
                                            .NumberofTotalClaimsDeniedonRemittance)),
                                                currentYear),
                NumberofTotalClaimsDeniedonRemittance =
                                            _dbService.GetDBChargesDashBoard(
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

        public ActionResult GetDenialsCodedByPhysicians3D()
        {
            var facilityId = Helpers.GetDefaultFacilityId();

            var dataSource = _dService.GetDenialsCodedByPhysicians(facilityId);
            var jsonResult = Json(dataSource, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
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
            var currentDateTime = _eService.GetInvariantCultureDateTime(facilityId);
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
            var dataSource = _dService.GetClaimDenialPercent();
            var jsonResult = Json(dataSource, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
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
            var facilityId = Helpers.GetDefaultFacilityId();
            var corporateId = Helpers.GetSysAdminCorporateID();
            var currentDateTime = _eService.GetInvariantCultureDateTime(facilityId);
            var currentYear = Convert.ToString(currentDateTime.Year);

            _dbService.SetDashBoardChargesActuals(facilityId, corporateId, currentYear);
            //bal.SetDashBoardCounterActuals(facilityId, corporateId, currentYear);
            var dashboardview = new ChargesDashboardView
            {
                CurrentDate = _eService.GetInvariantCultureDateTime(facilityId),
                NumberofCurrentDay =
                                            _eService.GetInvariantCultureDateTime(facilityId).DayOfYear,
                CurrentFacilityId = facilityId,
                RoomChargesCustomModel = _dbService.GetDBChargesDashBoard(
                                                facilityId,
                                                corporateId,
                                                Convert.ToString(
                                                    Convert.ToInt32(
                                                        ChargesDashBoard.RoomGrossCharges)),
                                                currentYear),
                IPChargesCustomModel = _dbService.GetDBChargesDashBoard(
                                                facilityId,
                                                corporateId,
                                                Convert.ToString(
                                                    Convert.ToInt32(
                                                        ChargesDashBoard.AncillaryGrossCharges)),
                                                currentYear),
                OPChargesCustomModel = _dbService.GetDBChargesDashBoard(
                                                facilityId,
                                                corporateId,
                                                Convert.ToString(
                                                    Convert.ToInt32(
                                                        ChargesDashBoard.OutpatientGrossCharges)),
                                                currentYear),
                ERChargesCustomModel = _dbService.GetDBChargesDashBoard(
                                                facilityId,
                                                corporateId,
                                                Convert.ToString(
                                                    Convert.ToInt32(
                                                        ChargesDashBoard
                                            .EmergencyRoomGrossCharges)),
                                                currentYear),
                IPRevenueCustomModel = _dbService.GetDBChargesDashBoard(
                                                facilityId,
                                                corporateId,
                                                Convert.ToString(
                                                    Convert.ToInt32(
                                                        ChargesDashBoard
                                            .InpatientGrossRevenuePerPatientDay)),
                                                currentYear),
                OPRevenueCustomModel = _dbService.GetDBChargesDashBoard(
                                                facilityId,
                                                corporateId,
                                                Convert.ToString(
                                                    Convert.ToInt32(
                                                        ChargesDashBoard
                                            .OutpatientRevenuePerEncounter)),
                                                currentYear),
                ERRevenueCustomModel = _dbService.GetDBChargesDashBoard(
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

        /// <summary>
        ///     Gets the facility dashboard data.
        /// </summary>
        /// <param name="facilityID">The facility identifier.</param>
        /// <param name="fiscalyear">The fiscalyear.</param>
        /// <returns></returns>
        public ActionResult GetFacilityDashboardData(int? facilityID, string fiscalyear)
        {
            var facilityId = facilityID != 0 ? facilityID ?? Helpers.GetDefaultFacilityId() : -1;
            var corporateId = Helpers.GetSysAdminCorporateID();
            var chargesapplied = facilityId == -1
                                 || _dbService.SetDashBoardChargesActuals(facilityId, corporateId, fiscalyear);
            chargesapplied = facilityId == -1 || _dbService.SetDashBoardCounterActuals(facilityId, corporateId, fiscalyear);
            var dashboardview = new ChargesDashboardView
            {
                CurrentDate = _eService.GetInvariantCultureDateTime(facilityId),
                NumberofCurrentDay = _eService.GetInvariantCultureDateTime(facilityId).DayOfYear,
                CurrentFacilityId = facilityId,
                RoomChargesCustomModel = _dbService.GetDBChargesDashBoard(facilityId, corporateId, Convert.ToString(Convert.ToInt32(ChargesDashBoard.RoomGrossCharges)), fiscalyear),
                IPChargesCustomModel = _dbService.GetDBChargesDashBoard(
                                                facilityId,
                                                corporateId,
                                                Convert.ToString(
                                                    Convert.ToInt32(
                                                        ChargesDashBoard.AncillaryGrossCharges)),
                                                fiscalyear),
                OPChargesCustomModel = _dbService.GetDBChargesDashBoard(
                                                facilityId,
                                                corporateId,
                                                Convert.ToString(
                                                    Convert.ToInt32(
                                                        ChargesDashBoard.OutpatientGrossCharges)),
                                                fiscalyear),
                ERChargesCustomModel = _dbService.GetDBChargesDashBoard(
                                                facilityId,
                                                corporateId,
                                                Convert.ToString(
                                                    Convert.ToInt32(
                                                        ChargesDashBoard
                                            .EmergencyRoomGrossCharges)),
                                                fiscalyear),
                IPRevenueCustomModel = _dbService.GetDBChargesDashBoard(
                                                facilityId,
                                                corporateId,
                                                Convert.ToString(
                                                    Convert.ToInt32(
                                                        ChargesDashBoard
                                            .InpatientGrossRevenuePerPatientDay)),
                                                fiscalyear),
                OPRevenueCustomModel = _dbService.GetDBChargesDashBoard(
                                                facilityId,
                                                corporateId,
                                                Convert.ToString(
                                                    Convert.ToInt32(
                                                        ChargesDashBoard
                                            .OutpatientRevenuePerEncounter)),
                                                fiscalyear),
                ERRevenueCustomModel = _dbService.GetDBChargesDashBoard(
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

        /// <summary>
        ///     Gets the charges dashboard data.
        /// </summary>
        /// <param name="facilityID">The facility identifier.</param>
        /// <param name="fiscalyear">The fiscalyear.</param>
        /// <param name="budgetFor">The budget for.</param>
        /// <returns></returns>
        public JsonResult GetChargesDashboardData(int? facilityID, string fiscalyear, string budgetFor)
        {
            var facilityId = facilityID ?? Helpers.GetDefaultFacilityId();
            var corporateId = Helpers.GetSysAdminCorporateID();
            var list = _dbService.GetDBChargesChartDashBoard(
                facilityId,
                corporateId,
                Convert.ToString(budgetFor),
                fiscalyear);
            return Json(list.Any() ? list : null, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        ///     Patients the volume dashboard.
        /// </summary>
        /// <returns></returns>
        public ActionResult PatientVolumeDashboard()
        {
            var facilityId = Helpers.GetDefaultFacilityId();
            var corporateId = Helpers.GetSysAdminCorporateID();

            var currentDateTime = _eService.GetInvariantCultureDateTime(facilityId);
            var currentYear = Convert.ToString(currentDateTime.Year);

            //var chargesapplied = bal.SetDashBoardCounterActuals(facilityId, corporateId, currentYear);
            var dashboardview = new PatientVolumeDashboardView
            {
                CurrentFacilityId = facilityId,
                CurrentDate = _eService.GetInvariantCultureDateTime(facilityId),
                NumberofCurrentDay = _eService.GetInvariantCultureDateTime(facilityId).DayOfYear,
                IPEncountersCustomModel = _dbService.GetDBChargesDashBoard(
                                                facilityId,
                                                corporateId,
                                                Convert.ToString(
                                                    Convert.ToInt32(
                                                        ChargesDashBoard.PatientDays)),
                                                currentYear),
                OPEncountersCustomModel = _dbService.GetDBChargesDashBoard(
                                                facilityId,
                                                corporateId,
                                                Convert.ToString(
                                                    Convert.ToInt32(
                                                        ChargesDashBoard
                                            .OutpatientEncounters)),
                                                currentYear),
                EREncountersCustomModel = _dbService.GetDBChargesDashBoard(
                                                facilityId,
                                                corporateId,
                                                Convert.ToString(
                                                    Convert.ToInt32(
                                                        ChargesDashBoard.EREncounters)),
                                                currentYear),
                DisChargesCustomModel = _dbService.GetDBChargesDashBoard(
                                                facilityId,
                                                corporateId,
                                                Convert.ToString(
                                                    Convert.ToInt32(
                                                        ChargesDashBoard.Discharges)),
                                                currentYear),
                ALOSCustomModel = _dbService.GetDBChargesDashBoard(
                                                facilityId,
                                                corporateId,
                                                Convert.ToString(
                                                    Convert.ToInt32(ChargesDashBoard.ALOS)),
                                                currentYear),
                IPADCCustomModel = _dbService.GetDBChargesDashBoard(
                                                facilityId,
                                                corporateId,
                                                Convert.ToString(
                                                    Convert.ToInt32(
                                                        ChargesDashBoard.InpatientsADC)),
                                                currentYear),
                PatientDaysCustomModel = _dbService.GetDBChargesDashBoard(
                                                facilityId,
                                                corporateId,
                                                Convert.ToString(
                                                    Convert.ToInt32(
                                                        ChargesDashBoard.Admissions)),
                                                currentYear)
            };
            return View(dashboardview);
        }


        /// <summary>
        ///     Gets the facility dashboard data.
        /// </summary>
        /// <param name="facilityID">The facility identifier.</param>
        /// <param name="fiscalyear">The fiscalyear.</param>
        /// <returns></returns>
        public ActionResult GetPatientVolumeDashboardData(int? facilityID, string fiscalyear)
        {
            //var facilityId = facilityID ?? Helpers.GetDefaultFacilityId();
            var facilityId = facilityID != 0 ? facilityID ?? Helpers.GetDefaultFacilityId() : -1;
            var corporateId = Helpers.GetSysAdminCorporateID();
            var chargesapplied = facilityId == -1 || _dbService.SetDashBoardCounterActuals(facilityId, corporateId, fiscalyear);
            var dashboardview = new PatientVolumeDashboardView
            {
                CurrentFacilityId = facilityId,
                CurrentDate = _eService.GetInvariantCultureDateTime(facilityId),
                NumberofCurrentDay = _eService.GetInvariantCultureDateTime(facilityId).DayOfYear,
                IPEncountersCustomModel = _dbService.GetDBChargesDashBoard(
                                                facilityId,
                                                corporateId,
                                                Convert.ToString(
                                                    Convert.ToInt32(
                                                        ChargesDashBoard.PatientDays)),
                                                fiscalyear),
                OPEncountersCustomModel = _dbService.GetDBChargesDashBoard(
                                                facilityId,
                                                corporateId,
                                                Convert.ToString(
                                                    Convert.ToInt32(
                                                        ChargesDashBoard
                                            .OutpatientEncounters)),
                                                fiscalyear),
                EREncountersCustomModel = _dbService.GetDBChargesDashBoard(
                                                facilityId,
                                                corporateId,
                                                Convert.ToString(
                                                    Convert.ToInt32(
                                                        ChargesDashBoard.EREncounters)),
                                                fiscalyear),
                DisChargesCustomModel = _dbService.GetDBChargesDashBoard(
                                                facilityId,
                                                corporateId,
                                                Convert.ToString(
                                                    Convert.ToInt32(
                                                        ChargesDashBoard.Discharges)),
                                                fiscalyear),
                ALOSCustomModel = _dbService.GetDBChargesDashBoard(
                                                facilityId,
                                                corporateId,
                                                Convert.ToString(
                                                    Convert.ToInt32(ChargesDashBoard.ALOS)),
                                                fiscalyear),
                IPADCCustomModel = _dbService.GetDBChargesDashBoard(
                                                facilityId,
                                                corporateId,
                                                Convert.ToString(
                                                    Convert.ToInt32(
                                                        ChargesDashBoard.InpatientsADC)),
                                                fiscalyear),
                PatientDaysCustomModel = _dbService.GetDBChargesDashBoard(
                                                facilityId,
                                                corporateId,
                                                Convert.ToString(
                                                    Convert.ToInt32(
                                                        ChargesDashBoard.Admissions)),
                                                fiscalyear)
            };
            return PartialView(PartialViews.PatientVolumeStats, dashboardview);

        }

        #endregion
    }
}