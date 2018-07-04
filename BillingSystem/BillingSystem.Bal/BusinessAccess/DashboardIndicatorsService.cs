using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

using AutoMapper;
using System.Data.SqlClient;
using BillingSystem.Common.Common;


namespace BillingSystem.Bal.BusinessAccess
{
    public class DashboardIndicatorsService : IDashboardIndicatorsService
    {
        private readonly IRepository<DashboardIndicators> _repository;
        private readonly IRepository<GlobalCodes> _gRepository;
        private readonly BillingEntities _context;
        private readonly IMapper _mapper;

        public DashboardIndicatorsService(IRepository<GlobalCodes> gRepository, IRepository<DashboardIndicators> repository
            , IMapper mapper, BillingEntities context)
        {
            _repository = repository;
            _gRepository = gRepository;
            _mapper = mapper;
            _context = context;
        }

        /// <summary>
        /// Gets the dashboard indicators list active in active.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="showinactive">The showinactive.</param>
        /// <returns></returns>
        public List<DashboardIndicatorsCustomModel> GetDashboardIndicators(long corporateId, string sort, string sortdir, bool status, long facilityId = 0)
        {
            var sqlParameters = new SqlParameter[5];
            sqlParameters[0] = new SqlParameter(InputParams.pSortBy.ToString(), !string.IsNullOrEmpty(sort) ? sort : "IndicatorNumber");
            sqlParameters[1] = new SqlParameter(InputParams.pSortDirection.ToString(), !string.IsNullOrEmpty(sortdir) ? sortdir : "ASC");
            sqlParameters[2] = new SqlParameter(InputParams.pCID.ToString(), corporateId);
            sqlParameters[3] = new SqlParameter(InputParams.pFID.ToString(), facilityId);
            sqlParameters[4] = new SqlParameter(InputParams.pStatus.ToString(), status);

            using (var ms = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetDashboardIndicators.ToString(), isCompiled: false
                , parameters: sqlParameters))
            {
                var result = ms.GetResultWithJson<DashboardIndicatorsCustomModel>(JsonResultsArray.DashboardResult.ToString());
                return result;
            }
        }

        /// <summary>
        /// Gets the dashboard indicators list by corporate.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId"></param>
        /// <returns></returns>
        public List<DashboardIndicatorsCustomModel> GetDashboardIndicatorsListByCorporate(int corporateId, int facilityId)
        {
            var sort = string.Empty;
            var sortdir = string.Empty;
            var sqlParameters = new SqlParameter[5];
            sqlParameters[0] = new SqlParameter(InputParams.pSortBy.ToString(), !string.IsNullOrEmpty(sort) ? sort : "IndicatorNumber");
            sqlParameters[1] = new SqlParameter(InputParams.pSortDirection.ToString(), !string.IsNullOrEmpty(sortdir) ? sortdir : "ASC");
            sqlParameters[2] = new SqlParameter(InputParams.pCID.ToString(), corporateId);
            sqlParameters[3] = new SqlParameter(InputParams.pFID.ToString(), facilityId);
            sqlParameters[4] = new SqlParameter(InputParams.pStatus.ToString(), true);

            using (var ms = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetDashboardIndicators.ToString(), isCompiled: false
                , parameters: sqlParameters))
            {
                var result = ms.GetResultWithJson<DashboardIndicatorsCustomModel>(JsonResultsArray.DashboardResult.ToString());
                return result;
            }
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public List<DashboardIndicatorsCustomModel> SaveDashboardIndicators(DashboardIndicatorsCustomModel vm)
        {
            var model = _mapper.Map<DashboardIndicators>(vm);
            var executedStatus = false;
            if (model.IndicatorID > 0)
            {
                var current = _repository.GetSingle(model.IndicatorID);
                model.CreatedBy = current.CreatedBy;
                model.CreatedDate = current.CreatedDate;
                model.CorporateId = current.CorporateId;
                _repository.UpdateEntity(model, model.IndicatorID);
                executedStatus = true;
            }
            else
            {
                _repository.Create(model);
                executedStatus = model.IndicatorID > 0;
                if (executedStatus)
                {
                    var sqlParameters = new SqlParameter[5];

                    sqlParameters[0] = new SqlParameter("CID", model.CorporateId.GetValueOrDefault());
                    sqlParameters[1] = new SqlParameter("IndicatorNumber", model.IndicatorNumber);
                    sqlParameters[2] = new SqlParameter("CurrentYear", DateTime.Now.Year);
                    sqlParameters[3] = new SqlParameter("SubCategory1", model.SubCategory1);
                    sqlParameters[4] = new SqlParameter("SubCategory2", model.SubCategory2);
                    _repository.ExecuteCommand(StoredProcedures.SPROC_SaveIndicatorDataAfterNewIndicator.ToString(), sqlParameters);

                    sqlParameters = new SqlParameter[5];

                    sqlParameters[0] = new SqlParameter("CID", model.CorporateId.GetValueOrDefault());
                    sqlParameters[1] = new SqlParameter("IndicatorNumber", model.IndicatorNumber);
                    sqlParameters[2] = new SqlParameter("CurrentYear", DateTime.Now.Year - 1);
                    sqlParameters[3] = new SqlParameter("SubCategory1", model.SubCategory1);
                    sqlParameters[4] = new SqlParameter("SubCategory2", model.SubCategory2);
                    _repository.ExecuteCommand(StoredProcedures.SPROC_SaveIndicatorDataAfterNewIndicator.ToString(), sqlParameters);
                }
            }
            var list = new List<DashboardIndicatorsCustomModel>();// GetDashboardIndicatorsListByCorporate(Convert.ToInt32(model.CorporateId), Convert.ToInt32(model.FacilityId));
            return list;
        }

        public bool DeleteIndicator(DashboardIndicatorsCustomModel vm)
        {
            var m = _mapper.Map<DashboardIndicators>(vm);
            _repository.UpdateEntity(m, m.IndicatorID);
            return true;
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="dashboardIndicatorsId">The dashboard indicators identifier.</param>
        /// <returns></returns>
        public DashboardIndicatorsCustomModel GetDashboardIndicatorsById(int id, long corporateId = 0)
        {
            var number = string.Empty;
            var sqlParameters = new SqlParameter[3];
            sqlParameters[0] = new SqlParameter(InputParams.pCID.ToString(), corporateId);
            sqlParameters[1] = new SqlParameter(InputParams.pValue.ToString(), number);
            sqlParameters[2] = new SqlParameter(InputParams.pId.ToString(), id);

            using (var ms = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetDashboardIndicatorsByIndicatorNumber.ToString(), isCompiled: false
                , parameters: sqlParameters))
            {
                var result = ms.GetResultWithJson<DashboardIndicatorsCustomModel>(JsonResultsArray.DashboardResult.ToString());
                return result.Any() ? result.FirstOrDefault() : new DashboardIndicatorsCustomModel();
            }
        }

        /// <summary>
        /// Gets the dashboard indicators by number.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns></returns>
        public DashboardIndicatorsCustomModel GetDashboardIndicatorsByNumber(string number, long corporateId)
        {
            var id = 0;
            var sqlParameters = new SqlParameter[3];
            sqlParameters[0] = new SqlParameter(InputParams.pCID.ToString(), corporateId);
            sqlParameters[1] = new SqlParameter(InputParams.pValue.ToString(), number);
            sqlParameters[2] = new SqlParameter(InputParams.pId.ToString(), id);

            using (var ms = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetDashboardIndicatorsByIndicatorNumber.ToString(), isCompiled: false
                , parameters: sqlParameters))
            {
                var result = ms.GetResultWithJson<DashboardIndicatorsCustomModel>(JsonResultsArray.DashboardResult.ToString());
                return result.Any() ? result.FirstOrDefault() : new DashboardIndicatorsCustomModel();
            }
        }

        /// <summary>
        /// Gets the indicator next number.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <returns></returns>
        public string GetIndicatorNextNumber(int corporateId)
        {
            var currentlist = _repository.FindBy(i => i.CorporateId == corporateId && i.IsActive == 1);
            var maxIndicatorNumber = currentlist.Any() ? currentlist.Select(a => Convert.ToInt32(a.IndicatorNumber)).Max() : 1;
            maxIndicatorNumber = maxIndicatorNumber == 0 ? 1 : (maxIndicatorNumber + 1);
            return Convert.ToString(maxIndicatorNumber);
        }

        /// <summary>
        /// Determines whether [is indicator exist] [the specified indicator number].
        /// </summary>
        /// <param name="indicatorNumber">The indicator number.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="subCategory1">The sub category1.</param>
        /// <param name="subCategory2">The sub category2.</param>
        /// <returns></returns>
        public bool IsIndicatorExist(string indicatorNumber, int id, int corporateId, string subCategory1, string subCategory2)
        {
            indicatorNumber = indicatorNumber.Trim();
            subCategory1 = subCategory1.Trim();
            subCategory2 = subCategory2.Trim();
            var isExist = _repository.Where(
                    i => i.IsActive == 1 && ((id > 0 && i.IndicatorID != id) || id == 0) &&
                        i.IndicatorNumber.Trim().Equals(indicatorNumber) && i.CorporateId == corporateId &&
                        i.SubCategory1.Trim().Equals(subCategory1) && i.SubCategory2.Trim().Equals(subCategory2))
                    .Any();
            return isExist;
        }


        public bool CheckDuplicateSortOrder(int sortOrder, int indicatorId)
        {
            var isExists = _repository.Where(d => d.SortOrder == sortOrder && (d.IndicatorID == indicatorId || indicatorId == 0)).Any();
            return isExists;
        }

        public bool UpdateIndicatorsOtherDetail(DashboardIndicatorsCustomModel vm)
        {
            var sqlParameters = new SqlParameter[6];
            sqlParameters[0] = new SqlParameter("pIndicatorNumber", vm.IndicatorNumber);
            sqlParameters[1] = new SqlParameter("pSubCategory1", vm.SubCategory1);
            sqlParameters[2] = new SqlParameter("pSubCategory2", vm.SubCategory2);
            sqlParameters[3] = new SqlParameter("pCId", vm.CorporateId.GetValueOrDefault());
            sqlParameters[4] = new SqlParameter("pFId", vm.FacilityId.GetValueOrDefault());
            sqlParameters[5] = new SqlParameter("pIsActive", vm.IsActive);

            _repository.ExecuteCommand(StoredProcedures.SPROC_MakeIndicatorInActive.ToString(), sqlParameters);
            return true;
        }
    }
}
