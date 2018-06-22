using System.Collections.Generic;
using System.Linq;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using System;
using System.Data.Entity;
using System.Data.SqlClient;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Repository.GenericRepository
{
    public class ScrubReportRepository : GenericRepository<ScrubReport>
    {
        private readonly DbContext _context;

        public ScrubReportRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
            _context = context;
        }

        /// <summary>
        /// Scrubs the report detail corrections.
        /// </summary>
        /// <param name="scrubReportId">The scrub report identifier.</param>
        /// <param name="scrubHeaderId">The scrub header identifier.</param>
        /// <param name="lhsValue">The LHS value.</param>
        /// <param name="rhsValue">The RHS value.</param>
        /// <param name="loggedinUserId">The loggedin user identifier.</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityid">The facilityid.</param>
        /// <returns></returns>
        public bool ScrubReportDetailCorrections(int scrubReportId, int scrubHeaderId, string lhsValue, string rhsValue,
            int loggedinUserId, int corporateId, int facilityid, string correctionCodeId)
        {
            try
            {
                if (_context != null)
                {
                    var spName =
                        string.Format(
                            "EXEC {0} @pScrubHeaderID, @pScrubReportID, @pLHSV, @pRHSV, @pExecutedBy, @pCorporateID, @pFacilityID ,@pcorrectionCodeId",
                            StoredProcedures.SPROC_ScrubReportCorrections.ToString());
                    var sqlParameters = new SqlParameter[8];
                    sqlParameters[0] = new SqlParameter("pScrubHeaderID", scrubHeaderId);
                    sqlParameters[1] = new SqlParameter("pScrubReportID", scrubReportId);
                    sqlParameters[2] = new SqlParameter("pLHSV", lhsValue);
                    sqlParameters[3] = new SqlParameter("pRHSV", rhsValue);
                    sqlParameters[4] = new SqlParameter("pExecutedBy", loggedinUserId);
                    sqlParameters[5] = new SqlParameter("pCorporateID", corporateId);
                    sqlParameters[6] = new SqlParameter("pFacilityID", facilityid);
                    sqlParameters[7] = new SqlParameter("pcorrectionCodeId", correctionCodeId);
                    ExecuteCommand(spName, sqlParameters);
                    return true;
                }
            }
            catch (Exception )
            {
                //throw ex;
            }
            return false;
        }

        /// <summary>
        /// Inserts the diagnosis for order.
        /// </summary>
        /// <param name="corporateid">The corporateid.</param>
        /// <param name="facilityid">The facilityid.</param>
        /// <param name="patientid">The patientid.</param>
        /// <param name="encounterid">The encounterid.</param>
        /// <param name="loggedinUserId">The loggedin user identifier.</param>
        /// <param name="diagnosisCode">The diagnosis code.</param>
        /// <returns></returns>
        public IEnumerable<ScrubCorrectionModel> SetCorrectedDiagnosis(int corporateid, int facilityid, int patientid, int encounterid, int loggedinUserId, string diagnosisCode)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @pCorporateID, @FacilityID, @PatientID, @EncounterID, @CreatedBy, @pDiagnosisCode ", StoredProcedures.SPROC_SetCorrectedDiagnosis.ToString());
                    var sqlParameters = new SqlParameter[6];
                    sqlParameters[0] = new SqlParameter("pCorporateID", corporateid);
                    sqlParameters[1] = new SqlParameter("FacilityID", facilityid);
                    sqlParameters[2] = new SqlParameter("PatientID", patientid);
                    sqlParameters[3] = new SqlParameter("EncounterID", encounterid);
                    sqlParameters[4] = new SqlParameter("CreatedBy", loggedinUserId);
                    sqlParameters[5] = new SqlParameter("pDiagnosisCode", diagnosisCode);
                    IEnumerable<ScrubCorrectionModel> result = _context.Database.SqlQuery<ScrubCorrectionModel>(spName, sqlParameters);
                    return result.ToList();
                }
            }
            catch (Exception )
            {
                //throw ex;
            }
            return null;
        }
    }
}