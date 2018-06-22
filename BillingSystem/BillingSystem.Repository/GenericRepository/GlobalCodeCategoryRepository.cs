using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Repository.Common;
using BillingSystem.Repository.GenericRepository;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;

namespace BillingSystem.Repository.GenericRepository
{
    public class GlobalCodeCategoryRepository : GenericRepository<GlobalCodeCategory>
    {
        private readonly DbContext _context;

        public GlobalCodeCategoryRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
            _context = context;
        }

        public List<GlobalCodeCategory> GetOrderTypeCategories(long facilityId, long userId, bool status = true)
        {
            var sqlParameters = new SqlParameter[3];
            sqlParameters[0] = new SqlParameter("pUserId", userId);
            sqlParameters[1] = new SqlParameter("pFId", facilityId);
            sqlParameters[2] = new SqlParameter("pStatusId", status);

            using (var ms = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetOrderTypeCategoriesByFacility.ToString(), isCompiled: false, parameters: sqlParameters))
            {
                var result = ms.GetResultWithJson<GlobalCodeCategory>(JsonResultsArray.GlobalCategory.ToString());
                return result;
            }
        }
    }
}
