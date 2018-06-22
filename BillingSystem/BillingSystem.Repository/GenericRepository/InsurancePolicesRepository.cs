using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Repository.Common;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;

namespace BillingSystem.Repository.GenericRepository
{
    public class InsurancePolicesRepository : GenericRepository<InsurancePolices>
    {
        private readonly DbContext _context;

        public InsurancePolicesRepository(BillingEntities context)
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
        public IEnumerable<InsurancePolicyCustomModel> GetInsurancePolicyListByFacility(long facilityId, long corporateId, bool activeStatus, long loggedinUserId)
        {
            var sqlParameters = new SqlParameter[4];
            sqlParameters[0] = new SqlParameter("pFId", facilityId);
            sqlParameters[1] = new SqlParameter("pCId", corporateId);
            sqlParameters[2] = new SqlParameter("pIsActive", activeStatus);
            sqlParameters[3] = new SqlParameter("pUserId", loggedinUserId);

            using (var ms = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetInsurancePolicyListByFacility.ToString(), parameters: sqlParameters, isCompiled: false))
            {
                var mList = ms.GetResultWithJson<InsurancePolicyCustomModel>(JsonResultsArray.PolicyResult.ToString());
                return mList;
            }
        }
    }
}
