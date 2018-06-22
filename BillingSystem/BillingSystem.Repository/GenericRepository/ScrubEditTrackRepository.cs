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
    public class ScrubEditTrackRepository : GenericRepository<ScrubEditTrack>
    {
        private readonly DbContext _context;
        public ScrubEditTrackRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
            _context = context;
        }



        public List<ScrubEditTrackCustomModel> GetScruberTrackData(int corporateId, int facilityId)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @CorporateId, @FacilityId", StoredProcedures.SPROC_GetScrubberEditTrack.ToString());
                    var sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("CorporateId", corporateId);
                    sqlParameters[1] = new SqlParameter("FacilityId", facilityId);
                    IEnumerable<ScrubEditTrackCustomModel> result = _context.Database.SqlQuery<ScrubEditTrackCustomModel>(spName, sqlParameters);
                    return result.ToList();
                }
            }
            catch (Exception )
            {
                return null;
            }
            return null;
        }

        /// <summary>
        /// Gets the correction log data.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="datefrom">The datefrom.</param>
        /// <param name="datetill">The datetill.</param>
        /// <returns></returns>
        public List<ScrubEditTrackCustomModel> GetCorrectionLogData(int corporateId, int facilityId ,DateTime? datefrom, DateTime? datetill)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @pFromDate,@pTillDate,@pFID, @pCID", StoredProcedures.SPROC_Get_Rep_CorrectionLog.ToString());
                    var sqlParameters = new SqlParameter[4];
                    sqlParameters[0] = new SqlParameter("pFromDate", datefrom);
                    sqlParameters[1] = new SqlParameter("pTillDate", datetill);
                    sqlParameters[2] = new SqlParameter("pFID", facilityId);
                    sqlParameters[3] = new SqlParameter("pCID", corporateId);
                    IEnumerable<ScrubEditTrackCustomModel> result = _context.Database.SqlQuery<ScrubEditTrackCustomModel>(spName, sqlParameters);
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
