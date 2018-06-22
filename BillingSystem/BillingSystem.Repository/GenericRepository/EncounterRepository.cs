using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace BillingSystem.Repository.GenericRepository
{
    public class EncounterRepository : GenericRepository<Encounter>
    {
        private readonly DbContext _context;

        public EncounterRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
            _context = context;
        }

        /// <summary>
        /// Gets the encounter chart data.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="displayType">The display type.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="tillDate">The till date.</param>
        /// <returns></returns>
        public List<EncounterExtension> GetEncounterChartData(int facilityId, int displayType, DateTime fromDate, DateTime tillDate)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @pFacilityID, @pDisplayTypeID, @pFromDate, @pTillDate", StoredProcedures.SPROC_GetDBEncounter);
                    var sqlParameters = new SqlParameter[4];
                    sqlParameters[0] = new SqlParameter("pFacilityID", facilityId);
                    sqlParameters[1] = new SqlParameter("pDisplayTypeID", displayType);
                    sqlParameters[2] = new SqlParameter("pFromDate", fromDate);
                    sqlParameters[3] = new SqlParameter("pTillDate", tillDate);
                    IEnumerable<EncounterExtension> result = _context.Database.SqlQuery<EncounterExtension>(spName, sqlParameters);
                    return result.ToList();
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the active encounter chart data.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="displayType">The display type.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="tillDate">The till date.</param>
        /// <returns></returns>
        public List<ClaimDenialPercentage> GetActiveEncounterChartData(int facilityId, int displayType, DateTime fromDate, DateTime tillDate)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @pFacilityID, @pDisplayTypeID, @pFromDate, @pTillDate", StoredProcedures.SPROC_GetActiveEncounterGraphs);
                    var sqlParameters = new SqlParameter[4];
                    sqlParameters[0] = new SqlParameter("pFacilityID", facilityId);
                    sqlParameters[1] = new SqlParameter("pDisplayTypeID", displayType);
                    sqlParameters[2] = new SqlParameter("pFromDate", fromDate);
                    sqlParameters[3] = new SqlParameter("pTillDate", tillDate);
                    IEnumerable<ClaimDenialPercentage> result = _context.Database.SqlQuery<ClaimDenialPercentage>(spName, sqlParameters);
                    return result.ToList();
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get the unclosed encounters of O/p and ER Patients with Status 'O' or 'E'.
        /// 'O' means Patients are still having Open Orders and 'E' means Patient's Encounter Doesn't end yet.
        /// </summary>
        /// <param name="facilityId"></param>
        /// <param name="corporateId"></param>
        /// <returns></returns>
        public List<EncounterCustomModel> GetUnclosedEncounters(int facilityId, int corporateId)
        {
            //SPROC_GetJobClosedEncounterAndOrders
            try
            {
                //if (_context != null)
                //{
                //    var spName = string.Format("EXEC {0} @pCorporateID, @pFacilityID", StoredProcedures.SPROC_GetNotClosedEncounters.ToString());
                //    var sqlParameters = new SqlParameter[2];
                //    sqlParameters[0] = new SqlParameter("pCorporateID", corporateId);
                //    sqlParameters[1] = new SqlParameter("pFacilityID", facilityId);
                //    IEnumerable<EncounterCustomModel> result = _context.Database.SqlQuery<EncounterCustomModel>(spName, sqlParameters);
                //    return result.ToList();
                //}
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @pCorporateID, @pFacilityID", StoredProcedures.SPROC_GetJobClosedEncounterAndOrders);
                    var sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("pCorporateID", corporateId);
                    sqlParameters[1] = new SqlParameter("pFacilityID", facilityId);
                    IEnumerable<EncounterCustomModel> result = _context.Database.SqlQuery<EncounterCustomModel>(spName, sqlParameters);
                    return result.ToList();
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the encounter end check.
        /// </summary>
        /// <param name="encounterid">The encounterid.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public List<EncounterEndCheckReturnStatus> GetEncounterEndCheck(int encounterid, int userId)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @pEncounterID , @pLoggedInUserId", StoredProcedures.SPROC_EncounterEndCheckBillEdit);
                    var sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("pEncounterID", encounterid);
                    sqlParameters[1] = new SqlParameter("pLoggedInUserId", userId);

                    IEnumerable<EncounterEndCheckReturnStatus> result = _context.Database.SqlQuery<EncounterEndCheckReturnStatus>(spName, sqlParameters);
                    return result.ToList();
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// Adds the bed charges for transfer patient.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <param name="computedon">The computedon.</param>
        /// <returns></returns>
        public bool AddBedChargesForTransferPatient(int encounterId, DateTime computedon)
        {
            try
            {
                if (_context != null)
                {
                    //var spName = string.Format("EXEC {0} @pEncounuterID, @pComputedOn", StoredProcedures.SPROC_BedChargesNightlyPerEncounter.ToString());
                    var spName = string.Format("EXEC {0} @ComputedOn, @EncounterID ", StoredProcedures.SPROC_ReValuateBedChargesPerEncounter);
                    var sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("ComputedOn", computedon);
                    sqlParameters[1] = new SqlParameter("EncounterID", encounterId);
                    ExecuteCommand(spName, sqlParameters);
                    return true;
                }
            }
            catch (Exception)
            {
                //throw ex;
            }
            return false;
        }

        /// <summary>
        /// Gets the cmo dashboard data.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="corporateid">The corporateid.</param>
        /// <returns></returns>
        public List<CMODashboardModel> GetCMODashboardData(int facilityId, int corporateid)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @CID,@FID", StoredProcedures.SPROC_GetCMODashboardData);
                    var sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("CID", corporateid);
                    sqlParameters[1] = new SqlParameter("FID", facilityId);
                    IEnumerable<CMODashboardModel> result = _context.Database.SqlQuery<CMODashboardModel>(spName, sqlParameters);
                    return result.ToList();
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Approves the pharmacy order.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="type">The type.</param>
        /// <param name="comment">The comment.</param>
        /// <returns></returns>
        public bool ApprovePharmacyOrder(int id, string type, string comment)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @pOrderId, @pOrderStatus,@pComment ", StoredProcedures.SPROC_ApprovePharmacyOrder);
                    var sqlParameters = new SqlParameter[3];
                    sqlParameters[0] = new SqlParameter("pOrderId", id);
                    sqlParameters[1] = new SqlParameter("pOrderStatus", type);
                    sqlParameters[2] = new SqlParameter("pComment", comment);
                    ExecuteCommand(spName, sqlParameters);
                    return true;
                }
            }
            catch (Exception)
            {
                //throw ex;
            }
            return false;
        }

        /// <summary>
        /// Gets the encounter status before update.
        /// </summary>
        /// <param name="encounterid">The encounterid.</param>
        /// <returns></returns>
        public List<EncounterEndCheckReturnStatus> GetEncounterStatusBeforeUpdate(int encounterid)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @pEncounterID", StoredProcedures.SPROC_EncounterEndChecks_SA);
                    var sqlParameters = new SqlParameter[1];
                    sqlParameters[0] = new SqlParameter("pEncounterID", encounterid);
                    IEnumerable<EncounterEndCheckReturnStatus> result = _context.Database.SqlQuery<EncounterEndCheckReturnStatus>(spName, sqlParameters);
                    return result.ToList();
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public bool UpdateBillDateOnEncounterEnds(int encounterId)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @pEId", StoredProcedures.SPROC_UpdateBillDate);
                    var sqlParameters = new SqlParameter[1];
                    sqlParameters[0] = new SqlParameter("pEId", encounterId);
                    ExecuteCommand(spName, sqlParameters);
                    return true;
                }
            }
            catch (Exception)
            {
                //throw ex;
            }
            return false;
        }


        /// <summary>
        /// Adds the virtual discharge log.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <returns></returns>
        public bool AddVirtualDischargeLog(int encounterId, int facilityId, int corporateId)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @pEncId, @pFid, @pCid", StoredProcedures.SPROC_AddVirtualDischargeLog);
                    var sqlParameters = new SqlParameter[3];
                    sqlParameters[0] = new SqlParameter("pEncId", encounterId);
                    sqlParameters[1] = new SqlParameter("pFid", facilityId);
                    sqlParameters[2] = new SqlParameter("pCid", corporateId);
                    ExecuteCommand(spName, sqlParameters);
                    return true;
                }
            }
            catch (Exception)
            {
                //throw ex;
            }
            return false;
        }


        /// <summary>
        /// Gets the encounter end check virtual discharge.
        /// </summary>
        /// <param name="encounterid">The encounterid.</param>
        /// <param name="loggedInUserId">The logged in user identifier.</param>
        /// <returns></returns>
        public List<EncounterEndCheckReturnStatus> GetEncounterEndCheckVirtualDischarge(int encounterid, int loggedInUserId)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @pEncounterID, @pLoggedInUserId", StoredProcedures.SPROC_EncounterEndCheckBillEdit_VirtualDischarge);
                    var sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("pEncounterID", encounterid);
                    sqlParameters[1] = new SqlParameter("pLoggedInUserId", loggedInUserId);
                    IEnumerable<EncounterEndCheckReturnStatus> result = _context.Database.SqlQuery<EncounterEndCheckReturnStatus>(spName, sqlParameters);
                    //return result.Count() > 0 ? result.ToList() : new List<EncounterEndCheckReturnStatus>();
                    return result.ToList();
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<EncounterCustomModel> GetActiveEncounterData(string facilityId)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0}  @FacilityId", StoredProcedures.SPORC_GetEncounterData);
                    var sqlParameters = new SqlParameter[1];
                    sqlParameters[0] = new SqlParameter("FacilityId", facilityId);
                    IEnumerable<EncounterCustomModel> result = _context.Database.SqlQuery<EncounterCustomModel>(spName, sqlParameters);
                    //return result.Count() > 0 ? result.ToList() : new List<EncounterCustomModel>();
                    return result.ToList();

                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
