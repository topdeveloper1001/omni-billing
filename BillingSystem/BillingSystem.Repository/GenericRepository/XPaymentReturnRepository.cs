using System;
using System.Data.Entity;
using System.Data.SqlClient;
using BillingSystem.Common.Common;
using BillingSystem.Model;

namespace BillingSystem.Repository.GenericRepository
{
    public class XPaymentReturnRepository : GenericRepository<XPaymentReturn>
    {
        private readonly DbContext _context;

        public XPaymentReturnRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
            _context = context;
        }

        /// <summary>
        /// Generates the remittance information.
        /// </summary>
        /// <param name="claimId">The claim identifier.</param>
        /// <param name="facilityid">The facilityid.</param>
        /// <param name="coporateId">The coporate identifier.</param>
        /// <returns></returns>
        public bool GenerateRemittanceInfo(int claimId, int coporateId, int facilityid)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @PassedInClaim , @CorporateID ,@FacilityID", StoredProcedures.GenerateRemittanceInfo.ToString());
                    var sqlParameters = new SqlParameter[3];
                    sqlParameters[0] = new SqlParameter("PassedInClaim ", claimId);
                    sqlParameters[1] = new SqlParameter("CorporateID", coporateId);
                    sqlParameters[2] = new SqlParameter("FacilityID",facilityid);
                    ExecuteCommand(spName, sqlParameters);
                    return true;
                }
            }
            catch (Exception )
            {
                return false;
            }
            return false;
        }

        /// <summary>
        /// Generates the remittance XML file.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public bool GenerateRemittanceXmlFile(int corporateId, int facilityId)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0}  @CorporateID ,@FacilityID", StoredProcedures.GenerateRemittanceXMLFile.ToString());
                    var sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("CorporateID", corporateId);
                    sqlParameters[1] = new SqlParameter("FacilityID", facilityId);
                    ExecuteCommand(spName, sqlParameters);
                    return true;
                }
            }
            catch (Exception )
            {
                return false;
            }
            return false;
        }
    }
}
