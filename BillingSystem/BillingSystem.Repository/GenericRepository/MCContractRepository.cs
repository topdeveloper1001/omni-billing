using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Repository.Common;

namespace BillingSystem.Repository.GenericRepository
{
    public class MCContractRepository : GenericRepository<MCContract>
    {
        private readonly DbContext _context;
        public MCContractRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
            _context = context;
        }



        /// <summary>
        /// Gets the mc overview.
        /// </summary>
        /// <param name="MCCode">The mc code.</param>
        /// <returns></returns>
        public List<McContractOverViewCustomModel> GetMCOverview(int MCCode)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @pMCCode", StoredProcedures.SPROC_GetMCOverview.ToString());
                    var sqlParameters = new SqlParameter[1];
                    sqlParameters[0] = new SqlParameter("pMCCode", MCCode);
                    IEnumerable<McContractOverViewCustomModel> result = _context.Database.SqlQuery<McContractOverViewCustomModel>(spName, sqlParameters);
                    return result.ToList();
                }
            }
            catch (Exception)
            {
                //throw ex;
            }
            return new List<McContractOverViewCustomModel>();
        }

        /// <summary>
        /// Gets the mc overview.
        /// </summary>
        /// <param name="MCCode">The mc code.</param>
        /// <returns></returns>
        public IEnumerable<McContractCustomModel> GetManagedCareDataByFacility(long facilityId, long corporateId, bool activeStatus, long loggedinUserId)
        {
            var sqlParameters = new SqlParameter[4];
            sqlParameters[0] = new SqlParameter("pFId", facilityId);
            sqlParameters[1] = new SqlParameter("pCId", corporateId);
            sqlParameters[2] = new SqlParameter("pIsActive", activeStatus);
            sqlParameters[3] = new SqlParameter("pUserId", loggedinUserId);

            using (var ms = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetManagedCareDataByFacility.ToString(), parameters: sqlParameters, isCompiled: false))
            {
                var mList = ms.GetResultWithJson<McContractCustomModel>(JsonResultsArray.ManagedCareResult.ToString());
                return mList;
            }
            //return Enumerable.Empty<McContractCustomModel>();
        }
    }
}
