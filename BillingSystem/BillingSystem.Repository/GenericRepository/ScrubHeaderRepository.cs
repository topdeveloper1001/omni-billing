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
    public class ScrubHeaderRepository : GenericRepository<ScrubHeader>
    {
        private readonly DbContext _context;

        public ScrubHeaderRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
            _context = context;
        }

        //public bool ApplyScrubBill(int billHeaderId, int loggedUserId)
        //{
        //    try
        //    {
        //        if (_context != null)
        //        {
        //            var spName = string.Format("EXEC {0} @pBillHeaderID, @pExecutedBy", StoredProcedures.SPROC_ScrubBill_Batch.ToString());
        //            var sqlParameters = new SqlParameter[2];
        //            sqlParameters[0] = new SqlParameter("pBillHeaderID", billHeaderId);
        //            sqlParameters[1] = new SqlParameter("pExecutedBy", loggedUserId);
        //            ExecuteCommand(spName, sqlParameters);
        //            return true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //throw ex;
        //    }
        //    return false;
        //}

        /// <summary>
        /// Generate Scrub for all bill headers
        /// </summary>
        /// <param name="corporateId"></param>
        /// <param name="facilityId"></param>
        /// <param name="loggedUserId"></param>
        /// <returns></returns>
        public bool ApplyScrubBill(int corporateId, int facilityId, int loggedUserId)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @pCorporateID, @pFacilityID, @pExecutedBy", StoredProcedures.SPROC_ScrubBill_Batch.ToString());
                    var sqlParameters = new SqlParameter[3];
                    sqlParameters[0] = new SqlParameter("pCorporateID", corporateId);
                    sqlParameters[1] = new SqlParameter("pFacilityID", facilityId);
                    sqlParameters[2] = new SqlParameter("pExecutedBy", loggedUserId);
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
        /// Applies the scrub bill to specific bill header identifier.
        /// </summary>
        /// <param name="billHeaderId">The bill header identifier.</param>
        /// <param name="loggedUserId">The logged user identifier.</param>
        /// <returns></returns>
        public bool ApplyScrubBillToSpecificBillHeaderId(int billHeaderId, int loggedUserId)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @pBillHeaderID, @pExecutedBy, @pRETStatus", StoredProcedures.SPROC_ScrubBill.ToString());
                    var sqlParameters = new SqlParameter[3];
                    sqlParameters[0] = new SqlParameter("pBillHeaderID", billHeaderId);
                    sqlParameters[1] = new SqlParameter("pExecutedBy", loggedUserId);
                    sqlParameters[2] = new SqlParameter
                    {
                        Direction = System.Data.ParameterDirection.Output,
                        ParameterName = "pRETStatus",
                        Value = 0,
                        SqlDbType = System.Data.SqlDbType.Int
                    };
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
        /// Gets the srub header detail.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="loggedUserId">The logged user identifier.</param>
        /// <returns></returns>
        public List<ScrubHeaderCustomModel> GetSrubHeaderDetail(int corporateId, int facilityId, int loggedUserId)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @pCorporateID, @pFacilityID", StoredProcedures.SPROC_GetSrubHeaderDetail);
                    var sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("pCorporateID", corporateId);
                    sqlParameters[1] = new SqlParameter("pFacilityID", facilityId);
                    IEnumerable<ScrubHeaderCustomModel> result = _context.Database.SqlQuery<ScrubHeaderCustomModel>(spName, sqlParameters);
                    return result.ToList();
                }
            }
            catch (Exception )
            {
                //throw ex;
            }
            return new List<ScrubHeaderCustomModel>();
        }


        public List<ScrubHeaderCustomModel> GetScrubSummaryDetail(int corporateId, int facilityId, DateTime? fromDate, DateTime? tillDate)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @pCorporateID, @pFacilityID, @pdtFrom,@pdtTill", StoredProcedures.SPROC_GetScrubberSummaryReport);
                    var sqlParameters = new SqlParameter[4];
                    sqlParameters[0] = new SqlParameter("pCorporateID", corporateId);
                    sqlParameters[1] = new SqlParameter("pFacilityID", facilityId);
                    sqlParameters[2] = new SqlParameter("pdtFrom", fromDate);
                    sqlParameters[3] = new SqlParameter("pdtTill", tillDate);
                    IEnumerable<ScrubHeaderCustomModel> result = _context.Database.SqlQuery<ScrubHeaderCustomModel>(spName, sqlParameters);
                    return result.ToList();
                }
            }
            catch (Exception )
            {
                //throw ex;
            }
            return new List<ScrubHeaderCustomModel>();
        }



        public List<ScrubHeaderCustomModel> GetErrorDetailByRuleCode(int corporateId, int facilityId, DateTime? fromDate, DateTime? tillDate)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @pCorporateID, @pFacilityID, @pdtFrom, @pdtTill", StoredProcedures.SPROC_GetErrorDetailReportByRuleCode);
                    var sqlParameters = new SqlParameter[4];
                    sqlParameters[0] = new SqlParameter("pCorporateID", corporateId);
                    sqlParameters[1] = new SqlParameter("pFacilityID", facilityId);
                    sqlParameters[2] = new SqlParameter("pdtFrom", fromDate);
                    sqlParameters[3] = new SqlParameter("pdtTill", tillDate);
                    IEnumerable<ScrubHeaderCustomModel> result = _context.Database.SqlQuery<ScrubHeaderCustomModel>(spName, sqlParameters);
                    return result.ToList();
                }
            }
            catch (Exception )
            {
                //throw ex;
            }
            return new List<ScrubHeaderCustomModel>();
        }

        public List<ScrubHeaderCustomModel> GetErrorSummaryByRuleCode(int corporateId, int facilityId, DateTime? fromDate, DateTime? tillDate)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @pCorporateID, @pFacilityID, @pdtFrom, @pdtTill", StoredProcedures.SPROC_GetErrorSummaryReportByRuleCode);
                    var sqlParameters = new SqlParameter[4];
                    sqlParameters[0] = new SqlParameter("pCorporateID", corporateId);
                    sqlParameters[1] = new SqlParameter("pFacilityID", facilityId);
                    sqlParameters[2] = new SqlParameter("pdtFrom", fromDate);
                    sqlParameters[3] = new SqlParameter("pdtTill", tillDate);
                    IEnumerable<ScrubHeaderCustomModel> result = _context.Database.SqlQuery<ScrubHeaderCustomModel>(spName, sqlParameters);
                    return result.ToList();
                }
            }
            catch (Exception )
            {
                //throw ex;
            }
            return new List<ScrubHeaderCustomModel>();
        }


        public List<PhysicianActivityCustomModel> GetPhysicianActivityReport(int corporateId, int facilityId, DateTime? fromDate, DateTime? tillDate, int physicianId)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @pCorporateId, @pFacilityId, @pFromDate, @pTillDate,@pPhysicianId", StoredProcedures.SPROC_GET_REP_PhysicianChargeReportDetails);
                    var sqlParameters = new SqlParameter[5];
                    sqlParameters[0] = new SqlParameter("pCorporateId", corporateId);
                    sqlParameters[1] = new SqlParameter("pFacilityId", facilityId);
                    sqlParameters[2] = new SqlParameter("pFromDate", fromDate);
                    sqlParameters[3] = new SqlParameter("pTillDate", tillDate);
                    sqlParameters[4] = new SqlParameter("pPhysicianId", physicianId);
                    IEnumerable<PhysicianActivityCustomModel> result = _context.Database.SqlQuery<PhysicianActivityCustomModel>(spName, sqlParameters);
                    return result.ToList();
                }
            }
            catch (Exception )
            {
                //throw ex;
            }
            return new List<PhysicianActivityCustomModel>();
        }

      

        public List<PhysicianDepartmentUtilizationCustomModel> GetUtilizationReport(int corporateId, DateTime? fromDate,
            DateTime? tillDate, int displayflag, int facilityId, int physicianId, int departmentId)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @pCorporateID,@pFromDate, @pTillDate,@pDisplayFlag,@pFacilityID,@pPhysicianID,@pDepartmentID", StoredProcedures.SPROC_GetUtilization);
                    var sqlParameters = new SqlParameter[7];
                    sqlParameters[0] = new SqlParameter("pCorporateID", corporateId);

                    sqlParameters[1] = new SqlParameter("pFromDate", fromDate);
                    sqlParameters[2] = new SqlParameter("pTillDate", tillDate);
                    sqlParameters[3] = new SqlParameter("pDisplayFlag", displayflag);
                    sqlParameters[4] = new SqlParameter("pFacilityID", facilityId);
                    sqlParameters[5] = new SqlParameter("pPhysicianID", physicianId);
                    sqlParameters[6] = new SqlParameter("pDepartmentID", departmentId);
                    IEnumerable<PhysicianDepartmentUtilizationCustomModel> result = _context.Database.SqlQuery<PhysicianDepartmentUtilizationCustomModel>(spName, sqlParameters);
                    return result.ToList();
                }
            }
            catch (Exception )
            {
                //throw ex;
            }
            return new List<PhysicianDepartmentUtilizationCustomModel>();
        }



        /// <summary>
        /// Gets the future charge report.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="tillDate">The till date.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public List<FutureOpenOrderCustomModel> GetFutureChargeReport(int corporateId, DateTime? fromDate, DateTime? tillDate, int facilityId)
        {
            try
            {
                if (_context != null)
                {
                    var spName =
                        string.Format(
                            "EXEC {0} @pFromDate, @pTillDate,@pCID,@pFID",
                            StoredProcedures.SPROC_GetFutureChargeReport);
                    var sqlParameters = new SqlParameter[4];
                    //sqlParameters[0] = new SqlParameter("pPatientId", corporateId);
                    sqlParameters[0] = new SqlParameter("pFromDate", fromDate);
                    sqlParameters[1] = new SqlParameter("pTillDate", tillDate);
                    sqlParameters[2] = new SqlParameter("pCID", corporateId);
                    sqlParameters[3] = new SqlParameter("pFID", facilityId);
                    IEnumerable<FutureOpenOrderCustomModel> result = _context.Database.SqlQuery<FutureOpenOrderCustomModel>(spName, sqlParameters);
                    return result.ToList();
                }
            }
            catch (Exception )
            {
                //throw ex;
            }
            return new List<FutureOpenOrderCustomModel>();
        }
    }
}
