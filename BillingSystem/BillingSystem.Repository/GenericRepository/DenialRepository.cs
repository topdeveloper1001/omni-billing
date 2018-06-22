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
    public class DenialRepository : GenericRepository<Denial>
    {
        private readonly DbContext _context;
        public DenialRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
            _context = context;
        }

        /// <summary>
        /// Gets the denial codes report.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="tillDate">The till date.</param>
        /// <param name="displayBy">The display by.</param>
        /// <returns></returns>
        public List<DenialReportCustomModel> GetDenialCodesReport(int corporateId, int facilityId, DateTime? fromDate, DateTime? tillDate, int? displayBy)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @pCorporateID , @pFacilityID, @pFromDate, @pTillDate, @pDisplayBy", StoredProcedures.SPROC_Get_REP_DenialCode.ToString());
                    var sqlParameters = new SqlParameter[5];
                    sqlParameters[0] = new SqlParameter("pCorporateID", corporateId);
                    sqlParameters[1] = new SqlParameter("pFacilityID", facilityId);
                    sqlParameters[2] = new SqlParameter("pFromDate", SqlDbType.DateTime);
                    sqlParameters[2].Value = (fromDate != null)
                        ? fromDate.Value.ToString("MM-dd-yyyy")
                        : sqlParameters[2].Value = null;
                    sqlParameters[3] = new SqlParameter("pTillDate", SqlDbType.DateTime);
                    sqlParameters[3].Value = (tillDate != null)
                        ? tillDate.Value.ToString("MM-dd-yyyy")
                        : sqlParameters[3].Value = null;
                    sqlParameters[4] = new SqlParameter("pDisplayBy", displayBy);
                    IEnumerable<DenialReportCustomModel> result = _context.Database.SqlQuery<DenialReportCustomModel>(spName, sqlParameters);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }


        public List<ClaimDenialPercentage> GetDenialCodesReport()
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} ", StoredProcedures.SPROC_GetClaimDenialPercent.ToString());
                    var sqlParameters = new SqlParameter[0];
                    IEnumerable<ClaimDenialPercentage> result = _context.Database.SqlQuery<ClaimDenialPercentage>(spName, sqlParameters);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        } 
        //SPROC_Get_REP_DenialCode
    }
}
