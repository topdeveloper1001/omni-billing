using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
namespace BillingSystem.Repository.GenericRepository
{
    public class JournalEntrySupportRepository : GenericRepository<JournalEntrySupportReportCustomModel>
    {
        private readonly DbContext _context;
        public JournalEntrySupportRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
            _context = context;
        }

        public List<JournalEntrySupportReportCustomModel> GetJournalEntrySupport(int corporateId, int facilityId, DateTime? fromDate, DateTime? tillDate, int? displayBy)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @pCorporateID , @pFacilityID, @pFromDate, @pTillDate, @pDisplayBy", StoredProcedures.SPROC_Get_REP_JEByDepartment.ToString());
                    var sqlParameters = new SqlParameter[5];
                    sqlParameters[0] = new SqlParameter("pCorporateID", corporateId);
                    sqlParameters[1] = new SqlParameter("pFacilityID", facilityId);
                    sqlParameters[2] = new SqlParameter("pFromDate", SqlDbType.DateTime);
                    sqlParameters[2].Value = (fromDate != null)
                        ? fromDate.Value.ToString("MM-dd-yyyy")
                        : sqlParameters[2].Value = DateTime.Today;
                    sqlParameters[3] = new SqlParameter("pTillDate", SqlDbType.DateTime);
                    sqlParameters[3].Value = (tillDate != null)
                        ? tillDate.Value.ToString("MM-dd-yyyy")
                        : sqlParameters[3].Value = DateTime.Today.AddDays(1);
                    sqlParameters[4] = new SqlParameter("pDisplayBy", displayBy);
                    IEnumerable<JournalEntrySupportReportCustomModel> result = _context.Database.SqlQuery<JournalEntrySupportReportCustomModel>(spName, sqlParameters);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }
    }
}
