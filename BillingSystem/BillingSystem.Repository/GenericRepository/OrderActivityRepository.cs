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
    public class OrderActivityRepository : GenericRepository<OrderActivity>
    {
        private readonly DbContext _context;

        public OrderActivityRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
            _context = context;
        }

        /// <summary>
        /// Applies the order activity to bill.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="reclaimFlag">The reclaim flag.</param>
        /// <returns></returns>
        public bool ApplyOrderActivityToBill(int encounterId, int corporateId, int facilityId, string reclaimFlag, Int64 claimid)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @pCorporateID, @pFacilityID, @pEncounterID, @pReClaimFlag,@pClaimId",
                        StoredProcedures.SPROC_ApplyOrderActivityToBill);
                    var sqlParameters = new SqlParameter[5];
                    sqlParameters[0] = new SqlParameter("pCorporateID", corporateId);
                    sqlParameters[1] = new SqlParameter("pFacilityID", facilityId);
                    sqlParameters[2] = new SqlParameter("pEncounterID", encounterId);
                    sqlParameters[3] = new SqlParameter("pReClaimFlag", reclaimFlag);
                    sqlParameters[4] = new SqlParameter("pClaimId", claimid);
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

        /// <summary>
        /// Gets the lab result status string.
        /// </summary>
        /// <param name="ordercode">The ordercode.</param>
        /// <param name="resultminvalue">The resultminvalue.</param>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public List<LabOrderActivityResultStatus> GetLabResultStatusString(int ordercode, decimal resultminvalue, int patientId)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @pValueRecorded, @pCode, @pPID",
                        StoredProcedures.SPROC_GetLabTestResultStatus);
                    var sqlParameters = new SqlParameter[3];
                    sqlParameters[0] = new SqlParameter("pValueRecorded", resultminvalue);
                    sqlParameters[1] = new SqlParameter("pCode", ordercode);
                    sqlParameters[2] = new SqlParameter("pPID", patientId);
                    IEnumerable<LabOrderActivityResultStatus> result = _context.Database.SqlQuery<LabOrderActivityResultStatus>(spName, sqlParameters);
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
        /// Gets the lab orders list
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="status">The status.</param>
        /// <param name="orderCategoryId">The order category identifier.</param>
        /// <param name="isActiveEncountersOnly">The is active encounters only.</param>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        public List<OrderActivityCustomModel> GetLabOrderActivitiesByPhysician(int userId, int status, string orderCategoryId, int isActiveEncountersOnly, int encounterId)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @UserId, @OrderActivityStatus, @OrderCategory, @IsActiveEncountersOnly, @EncounterId",
                        StoredProcedures.SPROC_GetActiveLabOrdersByPhysicianId);
                    var sqlParameters = new SqlParameter[5];
                    sqlParameters[0] = new SqlParameter("UserId", userId);
                    sqlParameters[1] = new SqlParameter("OrderActivityStatus", status);
                    sqlParameters[2] = new SqlParameter("OrderCategory", orderCategoryId);
                    sqlParameters[3] = new SqlParameter("IsActiveEncountersOnly", isActiveEncountersOnly);
                    sqlParameters[4] = new SqlParameter("EncounterId", encounterId);
                    var result = _context.Database.SqlQuery<OrderActivityCustomModel>(spName, sqlParameters);
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
        /// Gets the order activities with pcare actvities.
        /// </summary>
        /// <param name="pEncounterId">The p encounter identifier.</param>
        /// <param name="pCategoryId">The p category identifier.</param>
        /// <param name="pStatus">The p status.</param>
        /// <param name="pPatientId">The p patient identifier.</param>
        /// <param name="pFlag">The p flag.</param>
        /// <returns></returns>
        public List<OrderActivityCustomModel> GetOrderActivitiesWithPcareActvities(int pEncounterId, int pCategoryId, string pStatus, int pPatientId, int pFlag)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format(
                        "EXEC {0} @pEncounterId, @pCategoryId, @pStatus, @pPatientId, @pFlag",
                        StoredProcedures.SPROC_GetOrderTypeActivity);
                    var sqlParameters = new SqlParameter[5];
                    sqlParameters[0] = new SqlParameter("pEncounterId", pEncounterId);
                    sqlParameters[1] = new SqlParameter("pCategoryId", pCategoryId);
                    sqlParameters[2] = new SqlParameter("pStatus", pStatus);
                    sqlParameters[3] = new SqlParameter("pPatientId", pPatientId);
                    sqlParameters[4] = new SqlParameter("pFlag", pFlag);
                    var result = _context.Database.SqlQuery<OrderActivityCustomModel>(spName, sqlParameters);
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
        /// Creates the partiallyexecuted activity.
        /// </summary>
        /// <param name="activityid">The activityid.</param>
        /// <param name="quantity">The quantity.</param>
        /// <param name="actvityStatus">The actvity status.</param>
        /// <returns></returns>
        public bool CreatePartiallyexecutedActivity(int activityid, decimal quantity, string actvityStatus)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format(
                        "EXEC {0} @pActivityId, @pOrderActivityQuantity, @pActvityStatus",
                        StoredProcedures.SPROC_CreatePartialOrderActvities);
                    var sqlParameters = new SqlParameter[3];
                    sqlParameters[0] = new SqlParameter("pActivityId", activityid);
                    sqlParameters[1] = new SqlParameter("pOrderActivityQuantity", quantity);
                    sqlParameters[2] = new SqlParameter("pActvityStatus", actvityStatus);
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

        /// <summary>
        /// Get MAR View detail
        /// </summary>
        /// <param name="oSchedularOverViewCustomModel"></param>
        /// <returns></returns>
        public List<MarViewCustomModel> GetMARView(MarViewCustomModel oMarViewCustomModel)
        {
            try
            {
                if (this._context != null)
                {
                    string spName =
                        string.Format(
                            "EXEC {0} @PID, @EID, @FromDate, @TillDate, @DisplayFlag",
                            StoredProcedures.SPROC_MARView_V1);
                    var sqlParameters = new SqlParameter[5];
                    sqlParameters[0] = new SqlParameter("PID", oMarViewCustomModel.PatientId);
                    sqlParameters[1] = new SqlParameter("EID", oMarViewCustomModel.EncounterId);
                    sqlParameters[2] = new SqlParameter("FromDate", oMarViewCustomModel.FromDate);
                    sqlParameters[3] = new SqlParameter("TillDate", oMarViewCustomModel.TillDate);
                    sqlParameters[4] = new SqlParameter("DisplayFlag", oMarViewCustomModel.DisplayFlag);
                    IEnumerable<MarViewCustomModel> result = this._context.Database.SqlQuery<MarViewCustomModel>(
                        spName,
                        sqlParameters);
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
        /// 
        /// </summary>
        /// <param name="encounterId"></param>
        /// <returns></returns>
        public List<OrderActivityCustomModel> GetOrderActivitiesByEncounterIdSP(int encounterId)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @pEncounterId",
                        StoredProcedures.SPROC_GetOrderActivitiesByEncounterId);
                    var sqlParameters = new SqlParameter[1];
                    sqlParameters[0] = new SqlParameter("pEncounterId", encounterId);
                    IEnumerable<OrderActivityCustomModel> result = _context.Database.SqlQuery<OrderActivityCustomModel>(spName, sqlParameters);
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
        /// 
        /// </summary>
        /// <param name="orderActivityId"></param>
        /// <returns></returns>
        public GenerateBarCode GetBarCodeDetailsByOrderActivityId(int orderActivityId)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @pOrderActivityId",
                        StoredProcedures.SPROC_GetBarCodeDetailsByOrderActivityID);
                    var sqlParameters = new SqlParameter[1];
                    sqlParameters[0] = new SqlParameter("pOrderActivityId", orderActivityId);
                    IEnumerable<GenerateBarCode> result = _context.Database.SqlQuery<GenerateBarCode>(spName, sqlParameters);
                    return result.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }


        public OrderActivityCustomModel GetOrderActivityById(int orderActivityId)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @pOrderActivityId",
                        StoredProcedures.SPROC_GetOrderActivityDetailsByOrderActivityID);
                    var sqlParameters = new SqlParameter[1];
                    sqlParameters[0] = new SqlParameter("pOrderActivityId", orderActivityId);
                    var result = _context.Database.SqlQuery<OrderActivityCustomModel>(spName, sqlParameters);
                    return result.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }
    }
}
