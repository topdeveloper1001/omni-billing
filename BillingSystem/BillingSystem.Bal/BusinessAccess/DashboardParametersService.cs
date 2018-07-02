using System.Collections.Generic;
using System.Linq;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

using AutoMapper;
using BillingSystem.Common.Common;
using System.Data.SqlClient;

using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class DashboardParametersService : IDashboardParametersService
    {
        private readonly IRepository<DashboardParameters> _repository;
        private readonly BillingEntities _context;
        private readonly IMapper _mapper;

        public DashboardParametersService(IRepository<DashboardParameters> repository
            , IMapper mapper, BillingEntities context)
        {
            _repository = repository;
            _mapper = mapper;
            _context = context;
        }

        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<DashboardParametersCustomModel> GetDashboardParameters(int corpoarteId, int facilityId)
        {
            var sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter(InputParams.pCID.ToString(), corpoarteId);
            sqlParameters[1] = new SqlParameter(InputParams.pFID.ToString(), facilityId);
            using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetDashboardParameters.ToString(), isCompiled: false
                , parameters: sqlParameters))
            {
                var result = r.GetResultWithJson<DashboardParametersCustomModel>(JsonResultsArray.DashboardResult.ToString());

                if (result.Any())
                    result = result.OrderBy(d => d.IndicatorCategory).ThenBy(d1 => d1.ValueType).ToList();

                return result;
            }
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public List<DashboardParametersCustomModel> SaveDashboardParameters(DashboardParameters model)
        {
            var exStatus = false;
            if (model.ParameterId > 0)
            {
                var current = _repository.GetSingle(model.ParameterId);
                model.CreatedBy = current.CreatedBy;
                model.CreatedDate = current.CreatedDate;
                _repository.UpdateEntity(model, model.ParameterId);
                exStatus = true;
            }
            else
            {
                _repository.Create(model);
                exStatus = model.ParameterId > 0;
            }

            var list = GetDashboardParameters(model.CorporateId, model.FacilityId);
            return list;
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="Id">The identifier.</param>
        /// <returns></returns>
        public DashboardParameters GetDashboardParametersById(int? Id)
        {
            var m = _repository.Where(x => x.ParameterId == Id).FirstOrDefault();
            return m;
        }

        /// <summary>
        /// Method to Delete the Entity in the database.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public List<DashboardParametersCustomModel> DeleteDashboardParameters(DashboardParameters model)
        {
            if (model.ParameterId > 0)
            {
                var current = _repository.GetSingle(model.ParameterId);
                _repository.Delete(model.ParameterId);
            }

            var list = GetDashboardParameters(model.CorporateId, model.FacilityId);
            return list;
        }

        /// <summary>
        /// Gets the parameters list by dashboard.
        /// </summary>
        /// <param name="corpoarteId">The corpoarte identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="dashboardtype">The dashboardtype.</param>
        /// <returns></returns>
        public List<DashboardParametersRangeCustomModel> GetParametersListByDashboard(int corpoarteId, int facilityId, string dashboardtype)
        {
            //var spName = string.Format("EXEC {0} @pCID, @pFID, @pDashboardType", StoredProcedures.SprocGetDashboardParameters.ToString());
            //var sqlParameters = new SqlParameter[3];
            //sqlParameters[0] = new SqlParameter("pCID", corpoarteId);
            //sqlParameters[1] = new SqlParameter("pFID", facilityId);
            //sqlParameters[2] = new SqlParameter("pDashboardType", dashboardtype);
            //var result = _context.Database.SqlQuery<DashboardParametersRangeCustomModel>(spName, sqlParameters);
            //return result.ToList();

            var sqlParameters = new SqlParameter[3];
            sqlParameters[0] = new SqlParameter(InputParams.pCID.ToString(), corpoarteId);
            sqlParameters[1] = new SqlParameter(InputParams.pFID.ToString(), facilityId);
            sqlParameters[2] = new SqlParameter(InputParams.pTypeId.ToString(), dashboardtype);
            using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetDashboardParameters.ToString(), isCompiled: false
                , parameters: sqlParameters))
            {
                var result = r.GetResultWithJson<DashboardParametersRangeCustomModel>(JsonResultsArray.DashboardResult.ToString());

                if (result.Any())
                    result = result.OrderBy(d => d.IndicatorCategory).ThenBy(d1 => d1.ValueType).ToList();

                return result;
            }
        }
    }
}
