using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

using System.Data.SqlClient;
using AutoMapper;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class DashboardBudgetService : IDashboardBudgetService
    {
        private readonly IRepository<DashboardBudget> _dbRepository;
        private readonly IRepository<GlobalCodes> _gRepository;
        private readonly BillingEntities _context;
        private readonly IMapper _mapper;

        public DashboardBudgetService(IRepository<DashboardBudget> dbRepository, IRepository<GlobalCodes> gRepository, BillingEntities context, IMapper mapper)
        {
            _dbRepository = dbRepository;
            _gRepository = gRepository;
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets the dashboard budget.
        /// </summary>
        /// <returns></returns>
        public List<DashboardBudgetCustomModel> GetDashboardBudget(int corporateid, int facilityid)
        {
            var list = new List<DashboardBudgetCustomModel>();
            var lstDashboardBudget = _dbRepository.Where(a => a.CorporateId == corporateid && a.FacilityId == facilityid && (a.IsActive == null || (bool)a.IsActive)).ToList();
            if (lstDashboardBudget.Count > 0)
            {
                list.AddRange(lstDashboardBudget.Select(item => new DashboardBudgetCustomModel
                {
                    BudgetId = item.BudgetId,
                    BudgetType = item.BudgetType,
                    BudgetDescription = item.BudgetDescription,
                    DepartmentNumber = item.DepartmentNumber,
                    FiscalYear = item.FiscalYear,
                    JanuaryBudget = item.JanuaryBudget,
                    FebruaryBudget = item.FebruaryBudget,
                    MarchBudget = item.MarchBudget,
                    AprilBudget = item.AprilBudget,
                    MayBudget = item.MayBudget,
                    JuneBudget = item.JuneBudget,
                    JulyBudget = item.JulyBudget,
                    AugustBudget = item.AugustBudget,
                    SeptemberBudget = item.SeptemberBudget,
                    OctoberBudget = item.OctoberBudget,
                    NovemberBudget = item.NovemberBudget,
                    DecemberBudget = item.DecemberBudget,
                    CorporateId = item.CorporateId,
                    FacilityId = item.FacilityId,
                    CreatedBy = item.CreatedBy,
                    CreatedDate = item.CreatedDate,
                    ModifiedBy = item.ModifiedBy,
                    ModifiedDate = item.ModifiedDate,
                    IsActive = item.IsActive,
                    BudgetTypeString = GetNameByGlobalCodeValue(item.BudgetType.ToString(), Convert.ToInt32(GlobalCodeCategoryValue.DashBoardBudgetType).ToString()),
                    BudgetForStr = GetNameByGlobalCodeValue(item.BudgetFor.ToString(), Convert.ToInt32(GlobalCodeCategoryValue.DashboardBudgetFor).ToString()),
                }));
            }
            return list;
        }

        private string GetNameByGlobalCodeValue(string codeValue, string categoryValue, string fId = "")
        {
            if (!string.IsNullOrEmpty(codeValue))
            {
                var gl = _gRepository.Where(g => g.GlobalCodeValue.Equals(codeValue) && !g.IsDeleted.Value && g.GlobalCodeCategoryValue.Equals(categoryValue) && (string.IsNullOrEmpty(fId) || g.FacilityNumber.Equals(fId))).FirstOrDefault();
                return gl != null ? gl.GlobalCodeName : string.Empty;
            }
            return string.Empty;
        }
        /// <summary>
        /// Saves the dashboard budget.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public int SaveDashboardBudget(DashboardBudget model)
        {
            if (model.BudgetId > 0)
                _dbRepository.UpdateEntity(model, model.BudgetId);
            else
                _dbRepository.Create(model);
            return model.BudgetId;
        }

        /// <summary>
        /// Gets the dashboard budget by identifier.
        /// </summary>
        /// <param name="dashboardBudgetId">The dashboard budget identifier.</param>
        /// <returns></returns>
        public DashboardBudget GetDashboardBudgetById(int? dashboardBudgetId)
        {
            var model = _dbRepository.Where(x => x.BudgetId == dashboardBudgetId).FirstOrDefault();
            return model;
        }

        /// <summary>
        /// Deletes the dash borad budget.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public int DeleteDashBoradBudget(DashboardBudget model)
        {
            return _dbRepository.Delete(model);
        }

        /// <summary>
        /// Gets the database budget actual.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="bugetFor">The buget for.</param>
        /// <param name="fiscalyear">The fiscalyear.</param>
        /// <returns></returns>
        public List<DashboardBudgetReportCustomModel> GetDBBudgetActual(int facilityId, int corporateId, string bugetFor, string fiscalyear)
        {
            var spName = string.Format("EXEC {0} @CID,@FID, @BudgetFor,@FiscalYear", StoredProcedures.SPROC_GetDBBudgetActual.ToString());
            var sqlParameters = new SqlParameter[4];
            sqlParameters[0] = new SqlParameter("CID", corporateId);
            sqlParameters[1] = new SqlParameter("FID", facilityId);
            sqlParameters[2] = new SqlParameter("BudgetFor", bugetFor);
            sqlParameters[3] = new SqlParameter("FiscalYear", fiscalyear);
            IEnumerable<DashboardBudgetReportCustomModel> result = _context.Database.SqlQuery<DashboardBudgetReportCustomModel>(spName, sqlParameters);
            return result.ToList();
        }
        private List<DashboardChargesCustomModel> GetDBChargesDashBoardRep(int facilityId, int corporateId, string bugetFor, string fiscalyear)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @CID,@FID, @BudgetFor,@FiscalYear", StoredProcedures.SPROC_GetDBCharges);
                    if (facilityId == -1)
                        spName = string.Format("EXEC {0} @CID,@FID, @BudgetFor,@FiscalYear", StoredProcedures.SPROC_GetDBChargesByCorporate);
                    var sqlParameters = new SqlParameter[4];
                    sqlParameters[0] = new SqlParameter("CID", corporateId);
                    sqlParameters[1] = new SqlParameter("FID", facilityId);
                    sqlParameters[2] = new SqlParameter("BudgetFor", bugetFor);
                    sqlParameters[3] = new SqlParameter("FiscalYear", fiscalyear);
                    IEnumerable<DashboardChargesCustomModel> result = _context.Database.SqlQuery<DashboardChargesCustomModel>(spName, sqlParameters);
                    return result.ToList();
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Gets the database charges dash board.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="bugetFor">The buget for.</param>
        /// <param name="fiscalyear">The fiscalyear.</param>
        /// <returns></returns>
        public List<DashboardChargesCustomModel> GetDBChargesDashBoard(int facilityId, int corporateId, string bugetFor, string fiscalyear)
        {
            var charegsLists = GetDBChargesDashBoardRep(facilityId, corporateId, bugetFor, fiscalyear);
            charegsLists = charegsLists.Where(x => x.BudgetType != 3).ToList();
            switch (bugetFor)
            {
                case "14":
                case "15":
                case "16":
                    charegsLists = charegsLists.Take(3).ToList();
                    break;
                default:
                    var ytDvalues = charegsLists.Where(x => x.BudgetType > 99).ToList();
                    var yeartodateBudgetval = ytDvalues.Any(x => x.BudgetType == 100) ? ytDvalues.FirstOrDefault(x => x.BudgetType == 100).M12 : Convert.ToDecimal(0.00);
                    var yeartodateActualval = ytDvalues.Any(x => x.BudgetType == 101) ? ytDvalues.FirstOrDefault(x => x.BudgetType == 101).M12 : Convert.ToDecimal(0.00);
                    var budgetforyear = ytDvalues.Any(x => x.BudgetType == 102) ? ytDvalues.FirstOrDefault(x => x.BudgetType == 102).M12 : Convert.ToDecimal(0.00);
                    var projectesYea = yeartodateBudgetval != null &&
                                       yeartodateBudgetval != Convert.ToDecimal(0.00)
                        ? (yeartodateActualval / yeartodateBudgetval) * budgetforyear
                        : Convert.ToDecimal(0.00);
                    charegsLists = charegsLists.Where(x => x.BudgetType < 100).ToList();
                    var actualbudgets = charegsLists.FirstOrDefault(x => x.BudgetType == 2);
                    var tragetbudgets = charegsLists.FirstOrDefault(x => x.BudgetType == 1);
                    var sumofactual = Convert.ToDecimal(0.00);
                    var sumoftarget = Convert.ToDecimal(0.00);
                    if (tragetbudgets != null)
                    {
                        //sumoftarget =
                        //    Convert.ToDecimal(tragetbudgets.M1 + tragetbudgets.M2 + tragetbudgets.M3 +
                        //                      tragetbudgets.M4 +
                        //                      tragetbudgets.M5 + tragetbudgets.M6 + tragetbudgets.M7 +
                        //                      +tragetbudgets.M8 + tragetbudgets.M9 + tragetbudgets.M10 +
                        //                      tragetbudgets.M11 +
                        //                      tragetbudgets.M12);
                        var yearBudget = new DashboardChargesCustomModel()
                        {
                            BudgetDescription = "Budget for the Year",
                            M1 = null,
                            M2 = null,
                            M3 = null,
                            M4 = null,
                            M5 = null,
                            M6 = null,
                            M7 = null,
                            M8 = null,
                            M9 = null,
                            M10 = null,
                            M11 = null,
                            M12 = budgetforyear,
                        };
                        charegsLists.Add(yearBudget);
                    }
                    if (actualbudgets != null)
                    {

                        //sumofactual =
                        //    Convert.ToDecimal(actualbudgets.M1 + actualbudgets.M2 + actualbudgets.M3 +
                        //                      actualbudgets.M4 +
                        //                      actualbudgets.M5 + actualbudgets.M6 + actualbudgets.M7 +
                        //                      +actualbudgets.M8 + actualbudgets.M9 + actualbudgets.M10 +
                        //                      actualbudgets.M11 +
                        //                      actualbudgets.M12);

                        var yearEndactual = new DashboardChargesCustomModel()
                        {
                            BudgetDescription = "Projected Year End Actual",
                            M1 = null,
                            M2 = null,
                            M3 = null,
                            M4 = null,
                            M5 = null,
                            M6 = null,
                            M7 = null,
                            M8 = null,
                            M9 = null,
                            M10 = null,
                            M11 = null,
                            M12 = projectesYea,
                            //M12 = sumofactual / DateTime.Now.DayOfYear * 365,
                        };
                        charegsLists.Add(yearEndactual);
                    }
                    if (tragetbudgets != null && actualbudgets != null)
                    {
                        var varianceBudget = new DashboardChargesCustomModel()
                        {
                            BudgetDescription = "Variance",
                            M1 = null,
                            M2 = null,
                            M3 = null,
                            M4 = null,
                            M5 = null,
                            M6 = null,
                            M7 = null,
                            M8 = null,
                            M9 = null,
                            M10 = null,
                            M11 = null,
                            M12 = projectesYea - budgetforyear,
                            //M12 = sumofactual / DateTime.Now.DayOfYear * 365 - sumoftarget,
                        };
                        charegsLists.Add(varianceBudget);
                    }
                    break;
            }

            return charegsLists;
        }

        /// <summary>
        /// Gets the database charges dash board.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="bugetFor">The buget for.</param>
        /// <param name="fiscalyear">The fiscalyear.</param>
        /// <returns></returns>
        public List<DashboardChargesCustomModel> GetDBChargesChartDashBoard(int facilityId, int corporateId, string bugetFor, string fiscalyear)
        {
            var charegsLists = GetDBChargesDashBoardRep(facilityId, corporateId, bugetFor, fiscalyear);
            if (bugetFor ==
                Convert.ToString(Convert.ToInt32(ChargesDashBoard.ClaimsAcceptancePercentageFirstSubmission)))
            {
                charegsLists = charegsLists.Any()
                    ? charegsLists.SingleOrDefault(x => x.BudgetType == 1) != null
                    ? charegsLists.Where(x => x.BudgetType == 2).ToList() : new List<DashboardChargesCustomModel>() : new List<DashboardChargesCustomModel>();
            }
            return charegsLists;
        }

        /// <summary>
        /// Sets the database charges actuals.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="fiscalyear">The fiscalyear.</param>
        /// <returns></returns>
        public bool SetDashBoardChargesActuals(int facilityId, int corporateId, string fiscalyear)
        {
            var sqlParameters = new SqlParameter[4];
            sqlParameters[0] = new SqlParameter("CID", corporateId);
            sqlParameters[1] = new SqlParameter("FID", facilityId);
            sqlParameters[2] = new SqlParameter("BudgetFor", "0");
            sqlParameters[3] = new SqlParameter("FiscalYear", fiscalyear);
            _dbRepository.ExecuteCommand(StoredProcedures.SPROC_SetDBChargesActuals.ToString(), sqlParameters);
            return true;
        }

        /// <summary>
        /// Sets the dash board counter actuals.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="fiscalyear">The fiscalyear.</param>
        /// <returns></returns>
        public bool SetDashBoardCounterActuals(int facilityId, int corporateId, string fiscalyear)
        {
            for (var i = 1001; i < 1031; i++)
            {
                if (i == 1015 || i == 1014 || i == 1002 || i == 1004 || i == 1011 || i == 1009 || i == 1026 || i == 1007 ||
                    i == 1025 || i == 1024 || i == 1001 || i == 1021 || i == 1016 || i == 1023 || i == 1030)
                {
                    var sqlParameters = new SqlParameter[4];
                    sqlParameters[0] = new SqlParameter("CID", corporateId);
                    sqlParameters[1] = new SqlParameter("FID", facilityId);
                    sqlParameters[2] = new SqlParameter("BudgetFor", Convert.ToString(i));
                    sqlParameters[3] = new SqlParameter("FiscalYear", fiscalyear);
                    _dbRepository.ExecuteCommand(StoredProcedures.SPROC_SetDBCountersActuals.ToString(), sqlParameters);

                }
            }
            return true;
        }

        /// <summary>
        /// Gets the manual dash board.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="bugetFor">The buget for.</param>
        /// <param name="fiscalyear">The fiscalyear.</param>
        /// <returns></returns>
        public List<ManualDashboardCustomModel> GetManualDashBoard(int facilityId, int corporateId, string bugetFor, Int32 fiscalyear, int facilityType, int segment, int Department)
        {
            var list = new List<ManualDashboardCustomModel>();
            var spName =
                       string.Format(
                           "EXEC {0} @pFacilityId,@pCorporateId, @pIndicatorNumber,@pFiscalYear,@pfacilityType,@psegment,@pDepartment",
                           StoredProcedures.SPROC_GetManualDashboardDataSA);
            var sqlParameters = new SqlParameter[7];
            sqlParameters[0] = new SqlParameter("pFacilityId", facilityId);
            sqlParameters[1] = new SqlParameter("pCorporateId", corporateId);
            sqlParameters[2] = new SqlParameter("pIndicatorNumber", bugetFor);
            sqlParameters[3] = new SqlParameter("pFiscalYear", fiscalyear);
            sqlParameters[4] = new SqlParameter("pfacilityType", facilityType);
            sqlParameters[5] = new SqlParameter("psegment", segment);
            sqlParameters[6] = new SqlParameter("pDepartment", Department);
            IEnumerable<ManualDashboardModel> result = _context.Database.SqlQuery<ManualDashboardModel>(spName, sqlParameters);

            list = result.Select(x => _mapper.Map<ManualDashboardCustomModel>(x)).ToList();

            if (list.Any())
                list = list.OrderBy(m => m.Indicators).ThenBy(m1 => m1.Year).ThenByDescending(m2 => m2.BudgetType).ToList();

            return list;
        }

        ///<Changedon> 12 August 2015</Changedon>
        /// <summary>
        /// Gets the manual dash board stat data. GetSubCategoryCharts
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="bugetFor">The buget for.</param>
        /// <param name="fiscalyear">The fiscalyear.</param>
        /// <param name="facilityType">Type of the facility.</param>
        /// <param name="segment">The segment.</param>
        /// <param name="Department">The department.</param>
        /// <returns></returns>
        public List<ExternalDashboardModel> GetManualDashBoardStatData(int facilityId, int corporateId, string bugetFor, string fiscalyear, int facilityType, int segment, int Department)
        {
            var list = new List<ExternalDashboardModel>();
            var spName = string.Format("EXEC {0} @CID,@FID,@TillDate,@FacilityRelated,@FacilityRegion,@IndiatorNumber", StoredProcedures.SPROC_GetManualDashboardDataStatList_SA);
            var sqlParameters = new SqlParameter[6];
            sqlParameters[0] = new SqlParameter("CID", corporateId);
            sqlParameters[1] = new SqlParameter("FID", facilityId);
            //sqlParameters[2] = new SqlParameter("pIndicatorNumber", type);
            sqlParameters[2] = new SqlParameter("TillDate", fiscalyear);
            sqlParameters[3] = new SqlParameter("FacilityRelated", facilityType == 0 ? DBNull.Value : (object)facilityType.ToString());
            sqlParameters[4] = new SqlParameter("FacilityRegion", segment == 0 ? DBNull.Value : (object)segment.ToString());
            sqlParameters[5] = new SqlParameter("IndiatorNumber", bugetFor);
            IEnumerable<ExternalDashboardModel> result = _context.Database.SqlQuery<ExternalDashboardModel>(spName, sqlParameters);

            var charegsLists = result.ToList();

            /*
             * Who: Amit Jain
             * Why: To Calculate the values based on Pie Chart
             * What: To get each indicator's value as CYTA and calulcate based on Pie Chart. 
             * Currently, Done for Year-To-Date Actuals column Only
             * When: 11 March, 2016
             */
            const string revenueByCategoryIndicator = "222";
            if (bugetFor.Contains(revenueByCategoryIndicator))
            {
                var revenueByCategory = charegsLists.Where(r => r.IndicatorNumber.Equals(revenueByCategoryIndicator)).ToList();
                if (revenueByCategory.Count > 0)
                {
                    var totalSumYtdA = Convert.ToDecimal(revenueByCategory.Sum(m => m.CYTA));
                    var totalSumYtdB = Convert.ToDecimal(revenueByCategory.Sum(m => m.CYTB));
                    foreach (var item in revenueByCategory)
                    {
                        item.CYTA = totalSumYtdA > 0 ? (item.CYTA * 100) / totalSumYtdA : 0;
                        item.CYTB = totalSumYtdB > 0 ? (item.CYTB * 100) / totalSumYtdB : 0;
                    }
                }
            }


            /*
             * Who: Amit Jain
             * Why: To Calculate the values based on Pie Chart
             * What: To get each indicator's value as CYTA and calulcate based on Pie Chart. 
             * Currently, Done for Year-To-Date Actuals column Only
             * When: 13 March, 2016
             */
            const string revenueByServiceCodeIndicator = "229";
            if (bugetFor.Contains(revenueByServiceCodeIndicator))
            {
                var revenueByServiceCode = charegsLists.Where(r => r.IndicatorNumber.Equals(revenueByServiceCodeIndicator)).ToList();
                if (revenueByServiceCode.Count > 0)
                {
                    var totalSumYtdServiceCodeA = Convert.ToDecimal(revenueByServiceCode.Sum(m => m.CYTA));
                    var totalSumYtdServiceCodeB = Convert.ToDecimal(revenueByServiceCode.Sum(m => m.CYTB));
                    foreach (var item in revenueByServiceCode)
                    {
                        item.CYTA = totalSumYtdServiceCodeA > 0 ? (item.CYTA * 100) / totalSumYtdServiceCodeA : 0;
                        item.CYTB = totalSumYtdServiceCodeB > 0 ? (item.CYTB * 100) / totalSumYtdServiceCodeB : 0;
                    }
                }
            }


            return charegsLists;
        }

        /// <summary>
        /// Gets the executive dashboard balance sheet.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="bugetFor">The buget for.</param>
        /// <param name="fiscalyear">The fiscalyear.</param>
        /// <param name="facilityType">Type of the facility.</param>
        /// <param name="segment">The segment.</param>
        /// <param name="Department">The department.</param>
        /// <returns></returns>
        public List<ExternalDashboardModel> GetExecutiveDashboardBalanceSheet(int facilityId, int corporateId, string bugetFor, string fiscalyear, int facilityType, int segment, int Department)
        {
            var list = new List<ExternalDashboardModel>();

            var facilityidCustom = facilityId == 0 ? 9999 : facilityId;

            var spName = string.Format("EXEC {0} @CID,@FID,@TillDate,@FacilityRelated,@FacilityRegion", StoredProcedures.SPROC_GetDBBudgetManualBalanceSheet);

            var sqlParameters = new SqlParameter[5];
            sqlParameters[0] = new SqlParameter("CID", corporateId);
            sqlParameters[1] = new SqlParameter("FID", facilityId);
            sqlParameters[2] = new SqlParameter("TillDate", fiscalyear);
            sqlParameters[3] = new SqlParameter("FacilityRelated", facilityType == 0 ? DBNull.Value : (object)facilityType.ToString());
            sqlParameters[4] = new SqlParameter("FacilityRegion", segment == 0 ? DBNull.Value : (object)segment.ToString());
            IEnumerable<ExternalDashboardModel> result = _context.Database.SqlQuery<ExternalDashboardModel>(spName, sqlParameters);
            var charegsLists = result.ToList();
            return charegsLists;

        }


        /// <summary>
        /// Gets the manual dash board stat data. GetSubCategoryCharts
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="bugetFor">The buget for.</param>
        /// <param name="fiscalyear">The fiscalyear.</param>
        /// <param name="facilityType">Type of the facility.</param>
        /// <param name="segment">The segment.</param>
        /// <param name="Department">The department.</param>
        /// <returns></returns>
        public List<ExternalDashboardModel> GetManualDashBoardStatSingleData(int facilityId, int corporateId, string bugetFor, string fiscalyear, int facilityType, int segment, int Department)
        {
            var spName = string.Format("EXEC {0} @CID,@FID,@TillDate,@FacilityRelated,@FacilityRegion", StoredProcedures.SPROC_GetDBBudgetActualManualIndicators);
            //var spName = string.Format("EXEC {0} @CID,@FID,@TillDate,@FacilityRelated,@FacilityRegion", StoredProcedures.SPROC_GetDBBudgetActualManual_Modified);

            var sqlParameters = new SqlParameter[6];
            sqlParameters[0] = new SqlParameter("CID", corporateId);
            sqlParameters[1] = new SqlParameter("FID", facilityId);
            sqlParameters[2] = new SqlParameter("TillDate", fiscalyear);
            sqlParameters[3] = new SqlParameter("FacilityRelated", facilityType == 0 ? DBNull.Value : (object)facilityType.ToString());
            sqlParameters[4] = new SqlParameter("FacilityRegion", segment == 0 ? DBNull.Value : (object)segment.ToString());
            sqlParameters[5] = new SqlParameter("pIndicatorNumber", bugetFor);
            //sqlParameters[6] = new SqlParameter("pDepartment", Department);
            IEnumerable<ExternalDashboardModel> result = _context.Database.SqlQuery<ExternalDashboardModel>(spName, sqlParameters);
            return result.ToList();
        }


        /// <summary>
        /// Gets the manual dash board stat data acute.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="bugetFor">The buget for.</param>
        /// <param name="fiscalyear">The fiscalyear.</param>
        /// <param name="facilityType">Type of the facility.</param>
        /// <param name="segment">The segment.</param>
        /// <param name="Department">The department.</param>
        /// <returns></returns>
        public List<ExternalDashboardModel> GetManualDashBoardStatDataAcute(int facilityId, int corporateId, string bugetFor, string fiscalyear, int facilityType, int segment, int Department)
        {
            var spName = string.Format("EXEC {0} @CID,@FID,@TillDate,@FacilityRelated,@FacilityRegion",
                        StoredProcedures.SPROC_GetDBBudgetActualManual_Acute);
            var sqlParameters = new SqlParameter[5];
            sqlParameters[0] = new SqlParameter("CID", corporateId);
            sqlParameters[1] = new SqlParameter("FID", facilityId);
            sqlParameters[2] = new SqlParameter("TillDate", fiscalyear);
            sqlParameters[3] = new SqlParameter("FacilityRelated", facilityType == 0 ? DBNull.Value : (object)facilityType.ToString());
            sqlParameters[4] = new SqlParameter("FacilityRegion", segment == 0 ? DBNull.Value : (object)segment.ToString());
            IEnumerable<ExternalDashboardModel> result = _context.Database.SqlQuery<ExternalDashboardModel>(spName, sqlParameters);
            return result.ToList();
        }

        /// <summary>
        /// Gets the patient fall rate.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="bugetFor">The buget for.</param>
        /// <param name="fiscalyear">The fiscalyear.</param>
        /// <param name="facilityType">Type of the facility.</param>
        /// <param name="segment">The segment.</param>
        /// <param name="Department">The department.</param>
        /// <returns></returns>
        public List<PatientFallStats> GetPatientFallRate(int facilityId, int corporateId, string bugetFor, int fiscalyear, int facilityType, int segment, int Department)
        {
            var spName = string.Format("EXEC {0} @FID,@CID,@pFiscalYear ", StoredProcedures.SPROC_GetPatientFallRate_ManualDashboard);
            var sqlParameters = new SqlParameter[3];
            sqlParameters[0] = new SqlParameter("FID", facilityId);
            sqlParameters[1] = new SqlParameter("CID", corporateId);
            sqlParameters[2] = new SqlParameter("pFiscalYear", fiscalyear);
            IEnumerable<PatientFallStats> result = _context.Database.SqlQuery<PatientFallStats>(spName, sqlParameters);

            return result.ToList();
        }
        public List<PatientFallStats> GetPatientFallRateV1(int facilityId, int corporateId, string bugetFor, int fiscalyear, int facilityType, int segment, int Department)
        {
            var spName = string.Format("EXEC {0} @FID,@CID,@pFiscalYear ", StoredProcedures.SPROC_GetPatientFallRate_DashboardData);
            var sqlParameters = new SqlParameter[3];
            sqlParameters[0] = new SqlParameter("FID", facilityId);
            sqlParameters[1] = new SqlParameter("CID", corporateId);
            sqlParameters[2] = new SqlParameter("pFiscalYear", fiscalyear);
            IEnumerable<PatientFallStats> result = _context.Database.SqlQuery<PatientFallStats>(spName, sqlParameters);

            return result.ToList();
        }
        /// <summary>
        /// Gets the sub category charts.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="bugetFor">The buget for.</param>
        /// <param name="fiscalyear">The fiscalyear.</param>
        /// <param name="facilityType">Type of the facility.</param>
        /// <param name="segment">The segment.</param>
        /// <param name="Department">The department.</param>
        /// <returns></returns>
        public List<ManualDashboardCustomModel> GetSubCategoryCharts(int facilityId, int corporateId, string bugetFor, Int32 fiscalyear, int facilityType, int segment, int Department)
        {
            var list = new List<ManualDashboardCustomModel>();
            var spName =
                         string.Format(
                             "EXEC {0} @pFacilityId,@pCorporateId, @pIndicatorNumber,@pFiscalYear,@pfacilityType,@psegment,@pDepartment",
                             StoredProcedures.SPROC_GetSubCategoryCharts_Manual);
            var sqlParameters = new SqlParameter[7];
            sqlParameters[0] = new SqlParameter("pFacilityId", facilityId);
            sqlParameters[1] = new SqlParameter("pCorporateId", corporateId);
            sqlParameters[2] = new SqlParameter("pIndicatorNumber", bugetFor);
            sqlParameters[3] = new SqlParameter("pFiscalYear", fiscalyear);
            sqlParameters[4] = new SqlParameter("pfacilityType", facilityType);
            sqlParameters[5] = new SqlParameter("psegment", segment);
            sqlParameters[6] = new SqlParameter("pDepartment", Department);
            IEnumerable<ManualDashboardModel> result = _context.Database.SqlQuery<ManualDashboardModel>(spName, sqlParameters);
            list = result.Select(x => _mapper.Map<ManualDashboardCustomModel>(x)).ToList();
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="facilityId"></param>
        /// <param name="corporateId"></param>
        /// <param name="bugetFor"></param>
        /// <param name="fiscalyear"></param>
        /// <param name="facilityType"></param>
        /// <param name="segment"></param>
        /// <param name="Department"></param>
        /// <returns></returns>
        public List<ManualDashboardCustomModel> GetSubCategoryChartsPayorMix(int facilityId, int corporateId, string bugetFor, Int32 fiscalyear, int facilityType, int segment, int Department)
        {
            var list = new List<ManualDashboardCustomModel>();
            var spName = string.Format("EXEC {0} @pFacilityId,@pCorporateId, @pIndicatorNumber,@pFiscalYear,@pfacilityType,@psegment,@pDepartment", StoredProcedures.SPROC_GetSubCategoryCharts_PixorMix);
            var sqlParameters = new SqlParameter[7];
            sqlParameters[0] = new SqlParameter("pFacilityId", facilityId);
            sqlParameters[1] = new SqlParameter("pCorporateId", corporateId);
            sqlParameters[2] = new SqlParameter("pIndicatorNumber", bugetFor);
            sqlParameters[3] = new SqlParameter("pFiscalYear", fiscalyear);
            sqlParameters[4] = new SqlParameter("pfacilityType", facilityType);
            sqlParameters[5] = new SqlParameter("psegment", segment);
            sqlParameters[6] = new SqlParameter("pDepartment", Department);
            IEnumerable<ManualDashboardModel> result = _context.Database.SqlQuery<ManualDashboardModel>(spName, sqlParameters);
            list = result.Select(x => _mapper.Map<ManualDashboardCustomModel>(x)).ToList();
            return list;
        }


        /// <summary>
        /// Gets the dash board data stat list.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="bugetFor">The buget for.</param>
        /// <param name="fiscalyear">The fiscalyear.</param>
        /// <param name="facilityType">Type of the facility.</param>
        /// <param name="segment">The segment.</param>
        /// <param name="Department">The department.</param>
        /// <returns></returns>
        public List<ExternalDashboardModel> GetDashBoardDataStatList(int facilityId, int corporateId, string bugetFor, string fiscalyear, int facilityType, int segment, int Department)
        {
            var spName = string.Format("EXEC {0} @CID,@FID,@TillDate,@FacilityRelated,@FacilityRegion,@IndiatorNumber", StoredProcedures.SPROC_GetDashboardDataStatList_SA);
            var sqlParameters = new SqlParameter[6];
            sqlParameters[0] = new SqlParameter("CID", corporateId);
            sqlParameters[1] = new SqlParameter("FID", facilityId);
            sqlParameters[2] = new SqlParameter("TillDate", fiscalyear);
            sqlParameters[3] = new SqlParameter("FacilityRelated", facilityType == 0 ? DBNull.Value : (object)facilityType.ToString());
            sqlParameters[4] = new SqlParameter("FacilityRegion", segment == 0 ? DBNull.Value : (object)segment.ToString());
            sqlParameters[5] = new SqlParameter("IndiatorNumber", bugetFor);
            IEnumerable<ExternalDashboardModel> result = _context.Database.SqlQuery<ExternalDashboardModel>(spName, sqlParameters);
            return result.ToList();
        }


        /// <summary>
        /// Gets the executive dashboard balance sheet.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="bugetFor">The buget for.</param>
        /// <param name="fiscalyear">The fiscalyear.</param>
        /// <param name="facilityType">Type of the facility.</param>
        /// <param name="segment">The segment.</param>
        /// <param name="Department">The department.</param>
        /// <returns></returns>
        public List<ExternalDashboardModel> GetDashboardDataBalanceSheetList(int facilityId, int corporateId, string bugetFor, string fiscalyear, int facilityType, int segment, int Department)
        {
            var list = new List<ExternalDashboardModel>();

            var facilityidCustom = facilityId == 0 ? 9999 : facilityId;
            var spName = string.Format("EXEC {0} @CID,@FID,@TillDate,@FacilityRelated,@FacilityRegion", StoredProcedures.SPROC_GetDashboardDataBalanceSheetDataList_SA);
            var sqlParameters = new SqlParameter[5];
            sqlParameters[0] = new SqlParameter("CID", corporateId);
            sqlParameters[1] = new SqlParameter("FID", facilityId);
            sqlParameters[2] = new SqlParameter("TillDate", fiscalyear);
            sqlParameters[3] = new SqlParameter("FacilityRelated", facilityType == 0 ? DBNull.Value : (object)facilityType.ToString());
            sqlParameters[4] = new SqlParameter("FacilityRegion", segment == 0 ? DBNull.Value : (object)segment.ToString());
            IEnumerable<ExternalDashboardModel> result = _context.Database.SqlQuery<ExternalDashboardModel>(spName, sqlParameters);
            return result.ToList();
        }

        /// <summary>
        /// Gets the manual dash board v1.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="bugetFor">The buget for.</param>
        /// <param name="fiscalyear">The fiscalyear.</param>
        /// <param name="facilityType">Type of the facility.</param>
        /// <param name="segment">The segment.</param>
        /// <param name="Department">The department.</param>
        /// <returns></returns>
        public List<ManualDashboardCustomModel> GetManualDashBoardV1(int facilityId, int corporateId, string bugetFor, Int32 fiscalyear, int facilityType, int segment, int Department)
        {
            var list = new List<ManualDashboardCustomModel>();

            var spName =
                     string.Format(
                         "EXEC {0} @pFacilityId,@pCorporateId, @pIndicatorNumber,@pFiscalYear,@pfacilityType,@psegment,@pDepartment",
                         StoredProcedures.SPROC_GetDashboardDataGraph_SA);
            var sqlParameters = new SqlParameter[7];
            sqlParameters[0] = new SqlParameter("pFacilityId", facilityId);
            sqlParameters[1] = new SqlParameter("pCorporateId", corporateId);
            sqlParameters[2] = new SqlParameter("pIndicatorNumber", bugetFor);
            sqlParameters[3] = new SqlParameter("pFiscalYear", fiscalyear);
            sqlParameters[4] = new SqlParameter("pfacilityType", facilityType);
            sqlParameters[5] = new SqlParameter("psegment", segment);
            sqlParameters[6] = new SqlParameter("pDepartment", Department);
            IEnumerable<ManualDashboardModel> result = _context.Database.SqlQuery<ManualDashboardModel>(spName, sqlParameters);

            list = result.Select(x => _mapper.Map<ManualDashboardCustomModel>(x)).ToList();
            if (list.Any())
                list = list.OrderBy(m => m.Indicators).ThenBy(m1 => m1.Year).ThenByDescending(m2 => m2.BudgetType).ToList();
            return list;

        }
    }
}
