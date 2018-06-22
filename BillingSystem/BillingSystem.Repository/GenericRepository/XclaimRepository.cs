using System;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using BillingSystem.Common.Common;
using BillingSystem.Model;

namespace BillingSystem.Repository.GenericRepository
{
    public class XclaimRepository : GenericRepository<XClaim>
    {

        private readonly DbContext _context;
        public XclaimRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
            _context = context;
        }



        /// <summary>
        /// Applies the advice payment.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public bool ApplyAdvicePayment(int corporateId, int facilityId)
        {
            try
            {
                if (_context != null)
                {
                    var spToExecute = string.Format("EXEC {0} @pCorporateID, @pFacilityID", StoredProcedures.SPROC_ApplyAdvicePayments.ToString());
                    var sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("pCorporateID", corporateId);
                    sqlParameters[1] = new SqlParameter("pFacilityID", facilityId);
                    
                    ExecuteCommand(spToExecute, sqlParameters);
                    return true;
                }
            }
            catch (Exception )
            {
                //throw ex;
            }
            return false;
        }

        public bool ApplyAdvicePaymentInRemittanceAdvice(int corporateId, int facilityId, int fileId)
        {
            try
            {
                if (_context != null)
                {
                    var spToExecute = string.Format("EXEC {0} @pCorporateID, @pFacilityID, @pFileId", StoredProcedures.SPROC_ApplyAdvicePaymentsByFileID);
                    var sqlParameters = new SqlParameter[3];
                    sqlParameters[0] = new SqlParameter("pCorporateID", corporateId);
                    sqlParameters[1] = new SqlParameter("pFacilityID", facilityId);
                    sqlParameters[2] = new SqlParameter("pFileId", fileId);

                    ExecuteCommand(spToExecute, sqlParameters);
                    return true;
                }
            }
            catch (Exception )
            {
                //throw ex;
            }
            return false;
        }
    }
}
