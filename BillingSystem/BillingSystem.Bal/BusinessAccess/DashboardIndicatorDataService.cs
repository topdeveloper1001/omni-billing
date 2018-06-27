using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using AutoMapper;
using BillingSystem.Repository.Interfaces;
using BillingSystem.Bal.Interfaces;
using System.Data.SqlClient;
using BillingSystem.Common.Common;
using BillingSystem.Repository.Common;

namespace BillingSystem.Bal.BusinessAccess
{
    public class DashboardIndicatorDataService : IDashboardIndicatorDataService
    {
        private readonly IRepository<DashboardIndicatorData> _repository;
        private readonly IRepository<GlobalCodes> _gRepository;
        private readonly BillingEntities _context;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardIndicatorDataService"/> class.
        /// </summary>
        public DashboardIndicatorDataService(IRepository<GlobalCodes> gRepository, IRepository<DashboardIndicatorData> repository
            , IMapper mapper, BillingEntities context)
        {
            _repository = repository;
            _gRepository = gRepository;
            _mapper = mapper;
            _context = context;
        }

        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>
        /// Return the Entity List
        /// </returns>
        public List<DashboardIndicatorDataCustomModel> GetDashboardIndicatorDataList(int corporateid, int facilityId)
        {
            var sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter(InputParams.pFID.ToString(), facilityId);
            sqlParameters[1] = new SqlParameter(InputParams.pCID.ToString(), corporateid);
            using (var ms = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetDashboardIndicatorData.ToString(), isCompiled: false
                , parameters: sqlParameters))
            {
                var result = ms.GetResultWithJson<DashboardIndicatorDataCustomModel>(JsonResultsArray.DashboardResult.ToString());
                return result;
            }
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public List<DashboardIndicatorDataCustomModel> SaveDashboardIndicatorData(DashboardIndicatorData model)
        {
            if (model.ID > 0)
            {
                var current = _repository.GetSingle(model.ID);
                model.CreatedBy = current.CreatedBy;
                model.CreatedDate = current.CreatedDate;
                model.CorporateId = current.CorporateId;
                _repository.UpdateEntity(model, model.ID);
            }
            else
                _repository.Create(model);

            var list = GetDashboardIndicatorDataList(model.CorporateId.GetValueOrDefault(), model.FacilityId.GetValueOrDefault());
            return list;
        }

        /// <summary>
        /// Saves the dashboard indicator data custom.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public int SaveDashboardIndicatorDataCustom(DashboardIndicatorData model)
        {
            var logId = model.CreatedBy;
            if (model.ID > 0)
            {
                var current = _repository.GetSingle(model.ID);
                model.CreatedBy = current.CreatedBy;
                model.CreatedDate = current.CreatedDate;
                model.CorporateId = current.CorporateId;

                if (model.StatisticData == null)
                    model.StatisticData = "0.0000";
                _repository.UpdateEntity(model, model.ID);
            }
            else
                _repository.Create(model);

            return model.ID;
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="dashboardIndicatorDataId">The dashboard indicator data identifier.</param>
        /// <returns></returns>
        public DashboardIndicatorData GetCurrentById(int id)
        {
            var model = _repository.FindBy(x => x.ID == id).FirstOrDefault();
            return model;
        }

        /// <summary>
        /// Bulks the save dashboard indicator data.
        /// </summary>
        /// <param name="vmList">The vm list.</param>
        /// <param name="currentFacilityId">The current facility identifier.</param>
        /// <param name="currentCorporateId">The current corporate identifier.</param>
        /// <returns></returns>
        public List<DashboardIndicatorDataCustomModel> BulkSaveDashboardIndicatorData(List<DashboardIndicatorDataCustomModel> vmList, int currentFacilityId, int currentCorporateId)
        {
            var executedStatus = false;
            var mList = vmList.Select(i =>
            {
                var m = _mapper.Map<DashboardIndicatorData>(i);
                m.FacilityId = currentFacilityId;
                m.CorporateId = currentCorporateId;
                m.ExternalValue1 = !string.IsNullOrEmpty(i.BudgetType) ? _gRepository.FindBy(a => a.GlobalCodeCategoryValue.Equals(Convert.ToString((int)GlobalCodeCategoryValue.DashBoardBudgetType))
                                    && a.GlobalCodeValue.Equals(i.BudgetType)).FirstOrDefault().GlobalCodeName : string.Empty;
                return m;
            });

            if (mList.Any())
            {
                _repository.Create(mList);
                executedStatus = mList.Any(a => a.ID == 0) ? false : true;
            }

            var list = executedStatus
                ? GetDashboardIndicatorDataList(currentCorporateId, currentFacilityId)
                : new List<DashboardIndicatorDataCustomModel>();
            return list;
        }

        /// <summary>
        /// Bulks the delete dashboard indicator data.
        /// </summary>
        /// <param name="indicatorNumber">The indicator number.</param>
        /// <param name="corporateid">The corporateid.</param>
        /// <returns></returns>
        public bool BulkInactiveDashboardIndicatorData(string indicatorNumber, int corporateid)
        {
            return false;
            //var isDeleted = UpdateDashBoardIndicatorDataStatus(corporateid, 0, indicatorNumber, "", "", false, "1");
            //return isDeleted;
        }

        /// <summary>
        /// Bulks the active dashboard indicator data.
        /// </summary>
        /// <param name="indicatorNumber">The indicator number.</param>
        /// <param name="corporateid">The corporateid.</param>
        /// <returns></returns>
        public bool BulkActiveDashboardIndicatorData(string indicatorNumber, int corporateid)
        {
            return false;

            //var isactive = UpdateDashBoardIndicatorDataStatus(corporateid, 0, indicatorNumber, "", "", true, "1");
            //return isactive;
        }

        /// <summary>
        /// Gets the type of the dashboard indicator data list by indicator number.
        /// </summary>
        /// <param name="corporateid">The corporateid.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="indicatorNumber">The indicator number.</param>
        /// <param name="budgetType">Type of the budget.</param>
        /// <param name="year">The year.</param>
        /// <returns></returns>
        public List<DashboardIndicatorData> GetDashboardIndicatorDataListByIndicatorNumberType(int corporateid, int facilityId, string indicatorNumber, string budgetType, string year)
        {
            var list = _repository.Where(x => x.CorporateId == corporateid && x.FacilityId == facilityId &&
                            x.IndicatorNumber == indicatorNumber && x.Year == year && x.ExternalValue1 == budgetType)
                        .ToList();
            return list;
        }

        /// <summary>
        /// Deletes the manual dashboard details.
        /// </summary>
        /// <param name="corporateid">The corporateid.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="indicatorNumber">The indicator number.</param>
        /// <param name="budgetType">Type of the budget.</param>
        /// <param name="year">The year.</param>
        /// <returns></returns>
        public bool DeleteManualDashboardDetails(int corporateid, int facilityId, string indicatorNumber, string budgetType, string year, string subCategory1, string subCategory2)
        {
            var isDeleted = UpdateDashBoardIndicatorDataStatus(corporateid, facilityId, indicatorNumber, budgetType, year, false, "2", subCategory1, subCategory2);
            return isDeleted;
        }

        /// <summary>
        /// Updates the dash board indicator data status.
        /// </summary>
        /// <param name="corporateid">The corporateid.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="indicatorNumber">The indicator number.</param>
        /// <param name="budgetType">Type of the budget.</param>
        /// <param name="year">The year.</param>
        /// <param name="status">if set to <c>true</c> [status].</param>
        /// <returns></returns>
        public bool UpdateDashBoardIndicatorDataStatus(int corporateid, int facilityId, string indicatorNumber, string budgetType, string year, bool status, string type, string subCategory1, string subCategory2)
        {
            var sqlParameters = new SqlParameter[9];
            sqlParameters[0] = new SqlParameter("pIndicatorNumber", indicatorNumber);
            sqlParameters[1] = new SqlParameter("pCID", corporateid);
            sqlParameters[2] = new SqlParameter("pStatus", status);
            sqlParameters[3] = new SqlParameter("pFID", facilityId);
            sqlParameters[4] = new SqlParameter("pbudgetType", budgetType);
            sqlParameters[5] = new SqlParameter("pYear", year);
            sqlParameters[6] = new SqlParameter("pType", type);
            sqlParameters[7] = new SqlParameter("SubCategory1", subCategory1);
            sqlParameters[8] = new SqlParameter("SubCategory2", subCategory2);
            _repository.ExecuteCommand(StoredProcedures.SPROC_UpdateDashboardIndicatorsData.ToString(), sqlParameters);
            return true;
        }

        /// <summary>
        /// Updates the indicators data in manual dashboard.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public bool UpdateIndicatorsDataInManualDashboard(DashboardIndicatorData model)
        {
            var sqlParameters = new SqlParameter[8];
            sqlParameters[0] = new SqlParameter("IndicatorNumbers", model.IndicatorNumber);
            sqlParameters[1] = new SqlParameter("CorporateId", model.CorporateId.GetValueOrDefault());
            sqlParameters[2] = new SqlParameter("FacilityId", model.FacilityId.GetValueOrDefault());
            sqlParameters[3] = new SqlParameter("Year", model.Year);
            sqlParameters[4] = new SqlParameter("BudgetType", model.ExternalValue1);
            sqlParameters[5] = new SqlParameter("LoggedInUserId", model.CreatedBy.GetValueOrDefault());
            sqlParameters[6] = new SqlParameter("SubCategory1", model.SubCategory1);
            sqlParameters[7] = new SqlParameter("SubCategory2", model.SubCategory2);
            _repository.ExecuteCommand(StoredProcedures.SPROC_UpdateCalculativeIndicatorData_SA.ToString(), sqlParameters);
            return true;
        }

        /// <summary>
        /// Sets the static budget target.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public bool SetStaticBudgetTarget(DashboardIndicatorData model)
        {
            var sqlParameters = new SqlParameter[7];
            sqlParameters[0] = new SqlParameter("pIndicatorNumber", model.IndicatorNumber);
            sqlParameters[1] = new SqlParameter("pSubCategory1", model.SubCategory1);
            sqlParameters[2] = new SqlParameter("pSubCategory2", model.SubCategory2);
            sqlParameters[3] = new SqlParameter("pCorporateId", model.CorporateId.GetValueOrDefault());
            sqlParameters[4] = new SqlParameter("pBudgetType", model.ExternalValue1);
            sqlParameters[5] = new SqlParameter("FacID", model.FacilityId.GetValueOrDefault());
            sqlParameters[6] = new SqlParameter("pYear", model.Year);
            _repository.ExecuteCommand(StoredProcedures.SPROC_SetStaticBudgetTarget.ToString(), sqlParameters);
            return true;
        }

        /// <summary>
        /// Sets the static budget target.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public bool SetStaticBudgetTargetIndciators(DashboardIndicators model)
        {
            var sqlParameters = new SqlParameter[7];
            sqlParameters[0] = new SqlParameter("pIndicatorNumber", model.IndicatorNumber);
            sqlParameters[1] = new SqlParameter("pSubCategory1", model.SubCategory1);
            sqlParameters[2] = new SqlParameter("pSubCategory2", model.SubCategory2);
            sqlParameters[3] = new SqlParameter("pCorporateId", model.CorporateId.GetValueOrDefault());
            sqlParameters[4] = new SqlParameter("pBudgetType", "1");
            sqlParameters[5] = new SqlParameter("FacID", model.FacilityId.GetValueOrDefault());
            sqlParameters[6] = new SqlParameter("pYear", DateTime.Now.Year);
            _repository.ExecuteCommand(StoredProcedures.SPROC_SetStaticBudgetTarget.ToString(), sqlParameters);
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateCalculateIndicatorUpdate(DashboardIndicators model)
        {
            for (int i = 1; i <= 2; i++)
            {
                var sqlParameters = new SqlParameter[7];
                sqlParameters[0] = new SqlParameter("CID", model.CorporateId.GetValueOrDefault());
                sqlParameters[1] = new SqlParameter("FID", model.FacilityId.GetValueOrDefault());
                sqlParameters[2] = new SqlParameter("IndicatorNumber", model.IndicatorNumber);
                sqlParameters[3] = new SqlParameter("BudgetType", i);
                sqlParameters[4] = new SqlParameter("CurrentYear", DateTime.Now.Year);
                sqlParameters[5] = new SqlParameter("SubCategory1", model.SubCategory1);
                sqlParameters[6] = new SqlParameter("SubCategory2", model.SubCategory2);
                _repository.ExecuteCommand(StoredProcedures.SPROC_DBDCalc_IndicatorUpdate.ToString(), sqlParameters);
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool GenerateIndicatorEffects(DashboardIndicators model)
        {
            var sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("CID", model.CorporateId.GetValueOrDefault());
            sqlParameters[1] = new SqlParameter("IndicatorNumber", model.IndicatorNumber);
            _repository.ExecuteCommand(StoredProcedures.SPROC_DBDGenerate_IndicatorEffects.ToString(), sqlParameters);
            return true;
        }
    }
}
