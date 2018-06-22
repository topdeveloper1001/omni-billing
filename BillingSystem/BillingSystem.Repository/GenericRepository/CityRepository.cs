using BillingSystem.Common;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Repository.Common;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BillingSystem.Repository.GenericRepository
{
    public class CityRepository : GenericRepository<City>
    {
        private readonly DbContext _context;

        public CityRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
            _context = context;
        }


        public async Task<IEnumerable<SelectList>> GetCitiesByStateAsync(long stateId)
        {
            var sqlParams = new SqlParameter[1];
            sqlParams[0] = new SqlParameter("pStateId", stateId);

            using (var ms = _context.MultiResultSetSqlQuery(StoredProcsiOS.iSprocGetCitiesByState.ToString(), isCompiled: false, parameters: sqlParams))
            {
                var result = await ms.GetResultWithJsonAsync<SelectList>(JsonResultsArray.Cities.ToString());
                return result;
            }
        }

    }
}
