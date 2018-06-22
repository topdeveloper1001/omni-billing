using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Repository.Common;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;

namespace BillingSystem.Repository.GenericRepository
{
    public class InsurancePlansRepository : GenericRepository<InsurancePlans>
    {
        private readonly DbContext _context;
        public InsurancePlansRepository(BillingEntities context)
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
        public IEnumerable<InsurancePlanCustomModel> GetInsurancePlansByFacility(long facilityId, long corporateId, bool activeStatus, long loggedinUserId)
        {
            var sqlParameters = new SqlParameter[4];
            sqlParameters[0] = new SqlParameter("pFId", facilityId);
            sqlParameters[1] = new SqlParameter("pCId", corporateId);
            sqlParameters[2] = new SqlParameter("pIsActive", activeStatus);
            sqlParameters[3] = new SqlParameter("pUserId", loggedinUserId);

            using (var ms = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetInsurancePlansByFacility.ToString(), parameters: sqlParameters, isCompiled: false))
            {
                var mList = ms.GetResultWithJson<InsurancePlanCustomModel>(JsonResultsArray.PlanResult.ToString());
                return mList;
            }
        }
    }
}
