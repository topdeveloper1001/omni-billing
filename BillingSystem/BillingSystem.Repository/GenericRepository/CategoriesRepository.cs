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
    public class CategoriesRepository : GenericRepository<Categories>
    {
        private readonly DbContext _context;

        public CategoriesRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
            _context = context;
        }

        public List<CategoriesCustomModel> GetCategoriesData()
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0}", StoredProcedures.SPROC_GetCategories);
                    
                    IEnumerable<CategoriesCustomModel> result = _context.Database.SqlQuery<CategoriesCustomModel>(spName);
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
