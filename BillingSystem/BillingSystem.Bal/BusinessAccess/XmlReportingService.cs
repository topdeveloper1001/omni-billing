using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using System.Linq;
using BillingSystem.Model.CustomModel;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{

    /// <summary>
    /// The xml reporting bal.
    /// </summary>
    public class XmlReportingService : IXmlReportingService
    {
        private readonly BillingEntities _context;

        public XmlReportingService(BillingEntities context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets the batch reort.
        /// </summary>
        /// <param name="corporateid">The corporateid.</param>
        /// <param name="facilityid">The facilityid.</param>
        /// <returns></returns>
        public List<XmlReportingBatchReport> GetBatchReort(int corporateid, int facilityid)
        {
            string spName = string.Format(
                          "EXEC {0} @pCId, @pFId",
                          StoredProcedures.SPROC_GetXMLReport_BatchReport);
            var sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("pCId", corporateid);
            sqlParameters[1] = new SqlParameter("pFId", facilityid);
            IEnumerable<XmlReportingBatchReport> result = _context.Database.SqlQuery<XmlReportingBatchReport>(spName, sqlParameters);
            return result.ToList();
        }


        /// <summary>
        /// Gets the initial claim error report.
        /// </summary>
        /// <param name="corporateid">The corporateid.</param>
        /// <param name="facilityid">The facilityid.</param>
        /// <param name="startdate">The startdate.</param>
        /// <param name="enddate">The enddate.</param>
        /// <param name="encType">Type of the enc.</param>
        /// <param name="clinicalId">The clinical identifier.</param>
        /// <returns></returns>
        public List<XmlReportingInitialClaimErrorReport> GetInitialClaimErrorReport(int corporateid, int facilityid, DateTime startdate, DateTime enddate, string encType, string clinicalId)
        {
            string spName = string.Format(
                         "EXEC {0} @pStartDate, @pEndDate,@pEncounterType,@pClinicalId,@pCId,@pFId",
                         StoredProcedures.SPROC_GetXMLReport_InitialClaimErrorReport);
            var sqlParameters = new SqlParameter[6];
            sqlParameters[0] = new SqlParameter("pStartDate", startdate);
            sqlParameters[1] = new SqlParameter("pEndDate", enddate);
            sqlParameters[2] = new SqlParameter("pEncounterType", encType);
            sqlParameters[3] = new SqlParameter("pClinicalId", clinicalId);
            sqlParameters[4] = new SqlParameter("pCId", corporateid);
            sqlParameters[5] = new SqlParameter("pFId", facilityid);
            IEnumerable<XmlReportingInitialClaimErrorReport> result =
                this._context.Database.SqlQuery<XmlReportingInitialClaimErrorReport>(spName, sqlParameters);
            return result.ToList();
        }
    }
}