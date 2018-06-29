using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using System.Threading.Tasks;
using BillingSystem.Repository.Common;
using BillingSystem.Model.EntityDto;

namespace BillingSystem.Repository.GenericRepository
{
    public class TechnicalSpecificationsRepository : GenericRepository<TechnicalSpecifications>
    {
        private readonly DbContext _context;

        public TechnicalSpecificationsRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
            _context = context;
        }

        public List<TechnicalSpecificationsCustomModel> GetTechnicalSpecificationsData(int corporateId, int facilityId)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @FacilityId,@CorporateId", StoredProcedures.SprocGetTechnicalSpecifications);
                    var sqlParameters = new SqlParameter[2];

                    sqlParameters[0] = new SqlParameter("FacilityId", facilityId);
                    sqlParameters[1] = new SqlParameter("CorporateId", corporateId);
                    IEnumerable<TechnicalSpecificationsCustomModel> result = _context.Database.SqlQuery<TechnicalSpecificationsCustomModel>(spName, sqlParameters);
                    return result.ToList();
                }
                return null;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }


    }
}
