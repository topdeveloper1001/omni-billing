using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Repository.GenericRepository
{
    public class RuleStepRepository : GenericRepository<RuleStep>
    {
        private readonly DbContext _context;
        public RuleStepRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
            _context = context;
        }

        /// <summary>
        /// Applies the scrub bill.
        /// </summary>
        /// <param name="ruleMasterId">The rule master identifier.</param>
        /// <returns></returns>
        public List<RuleStepPreview> GetPreviewRuleStepResult(int ruleMasterId)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @RuleMasterID", StoredProcedures.SPROC_ScrubRule_Preview.ToString());
                    var sqlParameters = new SqlParameter[1];
                    sqlParameters[0] = new SqlParameter("RuleMasterID", ruleMasterId);
                    IEnumerable<RuleStepPreview> result = _context.Database.SqlQuery<RuleStepPreview>(spName, sqlParameters);
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
