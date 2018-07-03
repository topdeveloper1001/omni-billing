using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;



namespace BillingSystem.Bal.BusinessAccess
{
    public class DashboardTargetsService : IDashboardTargetsService
    {
        private readonly IRepository<DashboardTargets> _repository;
        private readonly BillingEntities _context;

        public DashboardTargetsService(IRepository<DashboardTargets> repository, BillingEntities context)
        {
            _repository = repository;
            _context = context;
        }

        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<DashboardTargetsCustomModel> GetDashboardTargetsList(int cId, int fId)
        {
            var sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter(InputParams.pCID.ToString(), cId);
            sqlParameters[1] = new SqlParameter(InputParams.pFID.ToString(), fId);

            using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SprocDashboardTargets.ToString(), isCompiled: false
                , parameters: sqlParameters))
            {
                var result = r.GetResultWithJson<DashboardTargetsCustomModel>(JsonResultsArray.DashboardResult.ToString());
                return result;
            }
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<DashboardTargetsCustomModel> SaveDashboardTargets(DashboardTargets model)
        {
            List<DashboardTargetsCustomModel> list;
            if (model.TargetId > 0)
            {
                var current = _repository.GetSingle(model.TargetId);
                model.CreatedBy = current.CreatedBy;
                model.CreatedDate = current.CreatedDate;
                _repository.UpdateEntity(model, model.TargetId);
            }
            else
                _repository.Create(model);

            list = GetDashboardTargetsList(model.CorporateId, model.FacilityId);
            return list;
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="dashboardTargetsId"></param>
        /// <returns></returns>
        public DashboardTargets GetDashboardTargetsById(int? dashboardTargetsId)
        {
            var model = _repository.Where(x => x.TargetId == dashboardTargetsId).FirstOrDefault();
            return model;
        }
    }
}
