using System;
using System.Data.Entity;
using System.Data.SqlClient;
using BillingSystem.Common.Common;
using BillingSystem.Model;

namespace BillingSystem.Repository.GenericRepository
{
    public class OperatingRoomRepository : GenericRepository<OperatingRoom>
    {
        private readonly DbContext _context;
        public OperatingRoomRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
            _context = context;
        }


        /// <summary>
        /// Applies the surgury charges to bill.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="reclaimFlag">The reclaim flag.</param>
        /// <param name="claimId">The claim identifier.</param>
        /// <returns></returns>
        public bool ApplySurguryChargesToBill(int encounterId, int corporateId, int facilityId, string reclaimFlag, long claimId)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @pCorporateID, @pFacilityID, @pEncounterID, @pReClaimFlag,@pClaimId",
                        StoredProcedures.SPROC_ApplySurguryChargesToBill.ToString());
                    var sqlParameters = new SqlParameter[5];
                    sqlParameters[0] = new SqlParameter("pCorporateID", corporateId);
                    sqlParameters[1] = new SqlParameter("pFacilityID", facilityId);
                    sqlParameters[2] = new SqlParameter("pEncounterID", encounterId);
                    sqlParameters[3] = new SqlParameter("pReClaimFlag", reclaimFlag);
                    sqlParameters[4] = new SqlParameter("pClaimId", claimId);
                    ExecuteCommand(spName, sqlParameters);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return false;
        }
    }
}
