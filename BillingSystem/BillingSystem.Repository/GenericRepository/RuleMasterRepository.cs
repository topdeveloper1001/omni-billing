using System.Linq;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System;

namespace BillingSystem.Repository.GenericRepository
{
    public class RuleMasterRepository : GenericRepository<RuleMaster>
    {
        private readonly DbContext _context;
        public RuleMasterRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
            _context = context;
        }


        public List<RuleMasterCustomModel> GetRuleMasterList(string tableNumber, bool isNotActive)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @pCodeTableNumber,@pIsNotActive", StoredProcedures.SPROC_GetRuleMasterByTableNumber);
                    var sqlParameters = new SqlParameter[2];
                    //sqlParameters[0] = new SqlParameter("SortBy", sortBy);
                    //sqlParameters[1] = new SqlParameter("SortDir", sortDir);
                    sqlParameters[0] = new SqlParameter("pCodeTableNumber", tableNumber);
                    sqlParameters[1] = new SqlParameter("pIsNotActive", isNotActive);
                    var result = _context.Database.SqlQuery<RuleMasterCustomModel>(spName, sqlParameters);
                    return result.ToList();
                }
            }
            catch (Exception )
            {
                return null;
            }
            return null;
        }
    }
}
