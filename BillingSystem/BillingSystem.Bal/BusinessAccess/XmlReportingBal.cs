// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlReportingBal.cs" company="SPadez">
//   Omnihealthcare
// </copyright>
// <summary>
//   The xml reporting bal.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Bal.BusinessAccess
{
    using System;
    using System.Collections.Generic;

    using BillingSystem.Model.CustomModel;
    using BillingSystem.Repository.UOW;

    /// <summary>
    /// The xml reporting bal.
    /// </summary>
    public class XmlReportingBal : BaseBal
    {
        /// <summary>
        /// Gets the batch reort.
        /// </summary>
        /// <param name="corporateid">The corporateid.</param>
        /// <param name="facilityid">The facilityid.</param>
        /// <returns></returns>
        public List<XmlReportingBatchReport> GetBatchReort(int corporateid, int facilityid)
        {
            var result = new List<XmlReportingBatchReport>();
            using (var rep = UnitOfWork.TPFileHeaderRepository)
            {
                result = rep.GetBatchReort(corporateid, facilityid);
            }
            return result;
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
            var result = new List<XmlReportingInitialClaimErrorReport>();
            using (var rep = UnitOfWork.TPFileHeaderRepository)
            {
                result = rep.GetInitialClaimErrorReport(corporateid, facilityid, startdate, enddate, encType, clinicalId);
            }
            return result;
        }
    }
}