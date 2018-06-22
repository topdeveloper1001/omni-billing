// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TPFileHeaderRepository.cs" company="SPadez">
//   Omnihealthcare
// </copyright>
// <summary>
//   The tp file header repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Repository.GenericRepository
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.SqlClient;
    using System.Linq;

    using BillingSystem.Common.Common;
    using BillingSystem.Model;
    using BillingSystem.Model.CustomModel;

    /// <summary>
    /// The tp file header repository.
    /// </summary>
    public class TPFileHeaderRepository : GenericRepository<TPFileHeader>
    {
        #region Fields

        /// <summary>
        /// The _context.
        /// </summary>
        private readonly DbContext _context;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TPFileHeaderRepository"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public TPFileHeaderRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
            _context = context;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get batch reort.
        /// </summary>
        /// <param name="corperateId">The corperate id.</param>
        /// <param name="facilityid">The facilityid.</param>
        /// <returns>
        /// The <see cref="List" />.
        /// </returns>
        /// <exception cref="Exception"></exception>
        public List<XmlReportingBatchReport> GetBatchReort(int corperateId, int facilityid)
        {
            try
            {
                if (_context != null)
                {
                    string spName = string.Format(
                        "EXEC {0} @pCId, @pFId",
                        StoredProcedures.SPROC_GetXMLReport_BatchReport);
                    var sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("pCId", corperateId);
                    sqlParameters[1] = new SqlParameter("pFId", facilityid);
                    IEnumerable<XmlReportingBatchReport> result = _context.Database.SqlQuery<XmlReportingBatchReport>(spName, sqlParameters);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return null;
        }


        /// <summary>
        /// Gets the initial claim error report.
        /// </summary>
        /// <param name="corperateId">The corperate identifier.</param>
        /// <param name="facilityid">The facilityid.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="tillDate">The till date.</param>
        /// <param name="encounterType">Type of the encounter.</param>
        /// <param name="clinicalId">The clinical identifier.</param>
        /// <returns></returns>
        public List<XmlReportingInitialClaimErrorReport> GetInitialClaimErrorReport(
            int corperateId,
            int facilityid,
            DateTime fromDate,
            DateTime tillDate,
            string encounterType,
            string clinicalId)
        {
            try
            {
                if (this._context != null)
                {
                    string spName = string.Format(
                        "EXEC {0} @pStartDate, @pEndDate,@pEncounterType,@pClinicalId,@pCId,@pFId",
                        StoredProcedures.SPROC_GetXMLReport_InitialClaimErrorReport);
                    var sqlParameters = new SqlParameter[6];
                    sqlParameters[0] = new SqlParameter("pStartDate", fromDate);
                    sqlParameters[1] = new SqlParameter("pEndDate", tillDate);
                    sqlParameters[2] = new SqlParameter("pEncounterType", encounterType);
                    sqlParameters[3] = new SqlParameter("pClinicalId", clinicalId);
                    sqlParameters[4] = new SqlParameter("pCId", corperateId);
                    sqlParameters[5] = new SqlParameter("pFId", facilityid);
                    IEnumerable<XmlReportingInitialClaimErrorReport> result =
                        this._context.Database.SqlQuery<XmlReportingInitialClaimErrorReport>(spName, sqlParameters);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return null;
        }


        /// <summary>
        /// Deletes the XML parsed data.
        /// Only used in the Clean up of the unit test cases data
        /// </summary>
        /// <param name="corperateId">The corperate identifier.</param>
        /// <param name="facilityid">The facilityid.</param>
        /// <returns></returns>
        public bool DeleteXMLParsedData(int corperateId, int facilityid)
        {
            try
            {
                if (this._context != null)
                {
                    string spName = string.Format(
                        "EXEC {0} @pCID, @pFID",
                        StoredProcedures.Delete_XMlParsedData_SA);
                    var sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("pCID", corperateId);
                    sqlParameters[1] = new SqlParameter("pFID", facilityid);
                    ExecuteCommand(spName, sqlParameters);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }



        public List<TPFileHeaderCustomModel> GetHeaderListByFacilityId(int corporateId, int facilityId)
        {
            try
            {
                if (_context != null)
                {
                    string spName = $"EXEC {StoredProcedures.SprocGetTpFileHeaderListByFacilityId} @CId,@FId";
                    var sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("CId", corporateId);
                    sqlParameters[1] = new SqlParameter("@FId", facilityId);
                    var result = _context.Database.SqlQuery<TPFileHeaderCustomModel>(spName, sqlParameters);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }


        public List<TPFileHeaderCustomModel> DeleteXmlDataByFileId(int corporateId, int facilityId, long fileId, bool? withDetails = false)
        {
            try
            {
                string spName = $"EXEC {StoredProcedures.SprocDeleteXmlParsedDataByFileId} @pCId,@pFId,@pFileId,@pWithDetails";
                var sqlParameters = new SqlParameter[4];
                sqlParameters[0] = new SqlParameter("pCId", corporateId);
                sqlParameters[1] = new SqlParameter("pFId", facilityId);
                sqlParameters[2] = new SqlParameter("pFileId", fileId);
                sqlParameters[3] = new SqlParameter("pWithDetails", withDetails);
                var result = _context.Database.SqlQuery<TPFileHeaderCustomModel>(spName, sqlParameters);
                return result.ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }


        public bool ExecuteXmlFileDetails(int corporateId, int facilityid, long fileId)
        {
            try
            {
                string spName = $"EXEC {StoredProcedures.SprocXmlParseDetails} @pCID, @pFID, @pFileHeaderId";
                var sqlParameters = new SqlParameter[3];
                sqlParameters[0] = new SqlParameter("pCID", corporateId);
                sqlParameters[1] = new SqlParameter("pFID", facilityid);
                sqlParameters[2] = new SqlParameter("pFileHeaderId", fileId);
                ExecuteCommand(spName, sqlParameters);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
    }
}