using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using Microsoft.Ajax.Utilities;
using BillingSystem.Repository.Interfaces;
using AutoMapper;
using BillingSystem.Bal.Interfaces;
using System.Data.SqlClient;
using BillingSystem.Common.Common;
using BillingSystem.Repository.Common;

namespace BillingSystem.Bal.BusinessAccess
{
    public class ManualDashboardService : IManualDashboardService
    {
        private readonly IRepository<ManualDashboard> _repository;
        private readonly IRepository<DashboardIndicatorData> _diRepository;
        private readonly IRepository<DashboardIndicators> _dRepository;
        private readonly IRepository<GlobalCodes> _gRepository;
        private readonly BillingEntities _context;
        private readonly IMapper _mapper;

        public ManualDashboardService(IRepository<GlobalCodes> gRepository, IRepository<ManualDashboard> repository
            , IRepository<DashboardIndicatorData> diRepository, IRepository<DashboardIndicators> dRepository, IMapper mapper, BillingEntities context)
        {
            _repository = repository;
            _gRepository = gRepository;
            _diRepository = diRepository;
            _dRepository = dRepository;
            _mapper = mapper;
            _context = context;
        }

        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<ManualDashboardCustomModel> GetManualDashboardList(int corporateId, int facilityId = 0)
        {
            var sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter(InputParams.pCID.ToString(), corporateId);
            sqlParameters[1] = new SqlParameter(InputParams.pFID.ToString(), facilityId);
            using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetManualDashboardData.ToString()
                , isCompiled: false, parameters: sqlParameters))
            {
                var result = r.GetResultWithJson<ManualDashboardCustomModel>(JsonResultsArray.DashboardResult.ToString());
                return result;
            }
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public List<ManualDashboardCustomModel> SaveManualDashboard(ManualDashboard model)
        {
            if (model.ID > 0)
            {
                var current = _repository.GetSingle(model.ID);
                model.CreatedBy = current.CreatedBy;
                model.CreatedDate = current.CreatedDate;
                _repository.UpdateEntity(model, model.ID);
            }
            else
                _repository.Create(model);

            var currentId = model.ID;
            var list = GetManualDashboardList(model.CorporateId.GetValueOrDefault());
            return list;
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="id">The manual dashboard identifier.</param>
        /// <returns></returns>
        public ManualDashboard GetManualDashboardById(int? id)
        {
            var model = _repository.Where(x => x.ID == id).FirstOrDefault();
            return model;
        }

        /// <summary>
        /// Gets the indicators data for indicator number.
        /// </summary>
        /// <param name="corporateId"></param>
        /// <param name="facilityid"></param>
        /// <param name="year"></param>
        /// <param name="indicatorNumber"></param>
        /// <param name="budgetType"></param>
        /// <returns></returns>
        public List<DashboardIndicatorData> GetIndicatorsDataForIndicatorNumber(int corporateId, int facilityid, string year, string indicatorNumber, string budgetType, string subCategory1, string subCategory2)
        {
            var list = _diRepository.Where(x => x.IndicatorNumber.Equals(indicatorNumber) &&
                    x.ExternalValue1.Equals(budgetType) && x.FacilityId == facilityid && x.CorporateId == corporateId && x.Year.Equals(year)
                    && x.ExternalValue3 == "1" && x.SubCategory1.Equals(subCategory1) && x.SubCategory2.Equals(subCategory2)).ToList();
            return list;
        }

        /// <summary>
        /// Gets the manual indicator dashboard data list.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityid">The facilityid.</param>
        /// <param name="year">The year.</param>
        /// <returns></returns>
        public List<ManualDashboardCustomModel> GetManualIndicatorDashboardDataList(int corporateId, int facilityid, string year)
        {
            var sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter(InputParams.pCID.ToString(), corporateId);
            sqlParameters[1] = new SqlParameter(InputParams.pFID.ToString(), facilityid);
            sqlParameters[2] = new SqlParameter(InputParams.pYear.ToString(), year);
            using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetManualDashboardData.ToString()
                , isCompiled: false, parameters: sqlParameters))
            {
                var result = r.GetResultWithJson<ManualDashboardCustomModel>(JsonResultsArray.DashboardResult.ToString());

                if (result.Any())
                    result = result.OrderBy(a => a.Indicators).ToList();
                return result;
            }
        }

        /// <summary>
        /// Gets the manual dashboard data by indicator number.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityid">The facilityid.</param>
        /// <param name="year">The year.</param>
        /// <param name="indicatorNumber">The indicator number.</param>
        /// <param name="budgetType">Type of the budget.</param>
        /// <returns></returns>
        public ManualDashboardCustomModel GetManualDashboardDataByIndicatorNumber(int corporateId, int facilityid, string year, string indicatorNumber, string budgetType, string subCategory1, string subCategory2)
        {
            //var vm = new ManualDashboardCustomModel();
            //var mapper = new ManualDashboardCustomMapper();
            //using (var rep = UnitOfWork.DashboardBudgetRepository)
            //{
            //    var charegsLists = _repository.GetManualDashBoardDataByIndicatorNumber(facilityid, corporateId, year, indicatorNumber, budgetType, subCategory1, subCategory2);
            //    if (charegsLists.Any())
            //        vm = charegsLists.Select(mapper.MapModelToViewModel).FirstOrDefault();
            //}
            //return vm;

            var sqlParameters = new SqlParameter[7];
            sqlParameters[0] = new SqlParameter(InputParams.pCID.ToString(), corporateId);
            sqlParameters[1] = new SqlParameter(InputParams.pFID.ToString(), facilityid);
            sqlParameters[2] = new SqlParameter(InputParams.pYear.ToString(), year);
            sqlParameters[3] = new SqlParameter(InputParams.pValue.ToString(), indicatorNumber);
            sqlParameters[4] = new SqlParameter(InputParams.pBudgetType.ToString(), budgetType);
            sqlParameters[5] = new SqlParameter(InputParams.pCategory1.ToString(), subCategory1);
            sqlParameters[6] = new SqlParameter(InputParams.pCategory2.ToString(), subCategory2);

            using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetManualDashboardDataByIndicator.ToString()
                , isCompiled: false, parameters: sqlParameters))
            {
                var result = r.GetResultWithJson<ManualDashboardCustomModel>(JsonResultsArray.DashboardResult.ToString());

                if (result.Any())
                    return result.FirstOrDefault();

                return Enumerable.Empty<ManualDashboardCustomModel>().FirstOrDefault();
            }
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="expiryDate">The expiray date.</param>
        /// <returns></returns>
        public UserTokenCustomModel GetUserToken(string username, DateTime expiryDate)
        {
            var spName = string.Format("EXEC {0} @pUserName, @pExpiryDate", StoredProcedures.SPROC_GenerateUpdateToken);
            var sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("pUserName", username);
            sqlParameters[1] = new SqlParameter("pExpiryDate", expiryDate);
            var result = _context.Database.SqlQuery<UserTokenCustomModel>(spName, sqlParameters);

            return result.FirstOrDefault();
        }

        /// <summary>
        /// Gets the manual indicator dashboard data list.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityid">The facilityid.</param>
        /// <param name="year">The year.</param>
        /// <param name="indicator"></param>
        /// <param name="ownership"></param>
        /// <returns></returns>
        public List<ManualDashboardCustomModel> RebindManualIndicatorDashboardDataList(int corporateId, int facilityid, int year, int indicator, string ownership)
        {
            //var list = new List<ManualDashboardCustomModel>();
            //var manualDashboardMapper = new ManualDashboardCustomMapper1();
            //using (var rep = UnitOfWork.DashboardBudgetRepository)
            //{
            //    if (!string.IsNullOrEmpty(ownership))
            //        ownership = ownership.ToLower().Trim();

            //    var charegsLists = _repository.GetManualDashBoardDataListRebind(facilityid, corporateId, year, ownership, Convert.ToString(indicator));
            //    if (charegsLists.Any())
            //        list.AddRange(charegsLists.Select(manualDashboardMapper.MapModelToViewModel));
            //    //return list.OrderBy(x => x.Indicators).ToList();
            //    return list.OrderBy(x => x.IndicatorTypeStr).ToList(); /*Updated By Krishna on 29072015*/
            //}

            var spName = string.Format(
                            "EXEC {0} @pFacilityId,@pCorporateId,@pFiscalYear,@Ownership,@IndicatorNumber",
                            StoredProcedures.SPROC_GetManualDashboardDataListRebind);
            var sqlParameters = new SqlParameter[5];
            sqlParameters[0] = new SqlParameter("pFacilityId", facilityid);
            sqlParameters[1] = new SqlParameter("pCorporateId", corporateId);
            sqlParameters[2] = new SqlParameter("pFiscalYear", year);
            sqlParameters[3] = new SqlParameter("Ownership", ownership);
            sqlParameters[4] = new SqlParameter("IndicatorNumber", indicator);
            var result = _context.Database.SqlQuery<ManualDashboardCustomModel>(spName, sqlParameters);
            return result.ToList();
        }

        /// <summary>
        /// Gets the ownership list.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <returns></returns>
        public List<string> GetOwnershipList(int corporateId)
        {
            var list = _repository.Where(c => c.CorporateId == corporateId && c.IsActive != false).Select(o => o.OwnerShip).Distinct().ToList();
            return list;
        }

        /// <summary>
        /// Gets the indicators list.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="ownership">The ownership.</param>
        /// <returns></returns>
        public List<DashboardIndicators> GetIndicatorsList(int corporateId, string ownership = "")
        {
            if (!string.IsNullOrEmpty(ownership))
                ownership = ownership.ToLower().Trim();

            var list = _dRepository.Where(c => c.CorporateId == corporateId && c.IsActive == 1
                    && (string.IsNullOrEmpty(ownership) || c.OwnerShip.ToLower().Trim().Equals(ownership)))
                .DistinctBy(m => m.IndicatorNumber).OrderBy(o => o.Dashboard).ToList();
            return list;
        }

        public string SaveIndicatorsPlusData(ManualDashboard model)
        {
            var indicatorNumber = string.Empty;

            //using (var rep = UnitOfWork.DashboardIndicatorDataRepository)
            //{
            //    var result = _repository.SaveIndicatorsPlusData(model);
            //    return result != null ? result.IndicatorNumber : string.Empty;
            //}
            var spName = string.Format(
                            "EXEC {0} @IndicatorNumber,@SubCat1,@SubCat2,@M1,@M2,@M3,@M4,@M5,@M6,@M7,@M8,@M9,@M10,@M11,@M12,@FacilityId,@CId,@BugdetType,@Year,@DashDesc,@Ownership,@Definition",
                            StoredProcedures.SPROC_SaveAllDashboardIndicatorData);

            var sqlParameters = new SqlParameter[22];
            sqlParameters[0] = new SqlParameter("IndicatorNumber", model.Indicators);
            sqlParameters[1] = new SqlParameter("SubCat1", model.SubCategory1);
            sqlParameters[2] = new SqlParameter("SubCat2", model.SubCategory2);
            sqlParameters[3] = new SqlParameter("M1", model.M1);
            sqlParameters[4] = new SqlParameter("M2", model.M2);
            sqlParameters[5] = new SqlParameter("M3", model.M3);
            sqlParameters[6] = new SqlParameter("M4", model.M4);
            sqlParameters[7] = new SqlParameter("M5", model.M5);
            sqlParameters[8] = new SqlParameter("M6", model.M6);
            sqlParameters[9] = new SqlParameter("M7", model.M7);
            sqlParameters[10] = new SqlParameter("M8", model.M8);
            sqlParameters[11] = new SqlParameter("M9", model.M9);
            sqlParameters[12] = new SqlParameter("M10", model.M10);
            sqlParameters[13] = new SqlParameter("M11", model.M11);
            sqlParameters[14] = new SqlParameter("M12", model.M12);
            sqlParameters[15] = new SqlParameter("FacilityId", model.FacilityId);
            sqlParameters[16] = new SqlParameter("CId", model.CorporateId);
            sqlParameters[17] = new SqlParameter("BugdetType", model.BudgetType);
            sqlParameters[18] = new SqlParameter("Year", model.Year);
            sqlParameters[19] = new SqlParameter("DashDesc", model.OtherDescription);
            sqlParameters[20] = new SqlParameter("Ownership", model.OwnerShip);
            sqlParameters[21] = new SqlParameter("Definition", model.Defination);
            var result = _context.Database.SqlQuery<DashboardIndicatorDataCustomModel>(spName, sqlParameters);

            indicatorNumber = result != null && result.Any() ? result.FirstOrDefault().IndicatorNumber : string.Empty;

            return indicatorNumber;
        }
    }
}
