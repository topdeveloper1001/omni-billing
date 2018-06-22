using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Repository.Common;

namespace BillingSystem.Repository.GenericRepository
{
    public class BillHeaderRepository : GenericRepository<BillHeader>
    {
        private readonly DbContext _context;

        public BillHeaderRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
            _context = context;
        }


        ///// <summary>
        ///// Sends the e claims.
        ///// </summary>
        ///// <param name="senderId">The sender identifier.</param>
        ///// <param name="dispositionFlag">The disposition flag.</param>
        ///// <returns></returns>
        //public List<BillHeaderXMLModel> SendEClaims(int senderId, string dispositionFlag)
        //{
        //    try
        //    {
        //        if (_context != null)
        //        {
        //            var spName = string.Format("EXEC {0} @SenderID, @DispositionFlag", StoredProcedures.SendEClaim);
        //            var sqlParameters = new SqlParameter[2];
        //            sqlParameters[0] = new SqlParameter("SenderID", senderId);
        //            sqlParameters[1] = new SqlParameter("DispositionFlag", dispositionFlag);
        //            IEnumerable<BillHeaderXMLModel> result = _context.Database.SqlQuery<BillHeaderXMLModel>(spName, sqlParameters);
        //            return result.ToList();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return null;
        //}

        /// <summary>
        /// Applies the bed charges.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        public bool ApplyBedCharges(int encounterId)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @pEncounuterID", StoredProcedures.SPROC_ApplyBedChargesToBill);
                    var sqlParameters = new SqlParameter[1];
                    sqlParameters[0] = new SqlParameter("pEncounuterID", encounterId);
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
        /// Applies the order bill.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        public bool ApplyOrderBill(int encounterId)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @pEncounuterID", StoredProcedures.SPROC_ApplyOrderToBill);
                    var sqlParameters = new SqlParameter[1];
                    sqlParameters[0] = new SqlParameter("pEncounuterID", encounterId);
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
        /// Gets the revenue forecast facility.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="tillDate">The till date.</param>
        /// <returns></returns>
        public List<RevenueForecast> GetRevenueForecastFacility(int corporateId, int facilityId, DateTime? fromDate, DateTime? tillDate)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @pCorporateID , @pFacilityID, @pFromDate, @pTillDate", StoredProcedures.SPROC_GetRevenueForecastFacility);
                    var sqlParameters = new SqlParameter[4];
                    sqlParameters[0] = new SqlParameter("pCorporateID", corporateId);
                    sqlParameters[1] = new SqlParameter("pFacilityID", facilityId);
                    sqlParameters[2] = new SqlParameter("pFromDate", fromDate);
                    sqlParameters[3] = new SqlParameter("pTillDate", tillDate);
                    IEnumerable<RevenueForecast> result = _context.Database.SqlQuery<RevenueForecast>(spName, sqlParameters);
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
        /// Gets the revenue forecast facility by patient identifier.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public List<RevenueForecast> GetRevenueForecastFacilityByPatientId(int patientId)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @pPatientID", StoredProcedures.SPROC_GetRevenueForecastbyPatient);
                    var sqlParameters = new SqlParameter[1];
                    sqlParameters[0] = new SqlParameter("pPatientID", patientId);
                    IEnumerable<RevenueForecast> result = _context.Database.SqlQuery<RevenueForecast>(spName, sqlParameters);
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
        /// Gets the bill detail view.
        /// </summary>
        /// <param name="billheaderId">The billheader identifier.</param>
        /// <returns></returns>
        public List<BillDetailCustomModel> GetBillDetailView(int billheaderId)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @pBillHeaderID ", StoredProcedures.SPROC_BillDetailView);
                    var sqlParameters = new SqlParameter[1];
                    sqlParameters[0] = new SqlParameter("pBillHeaderID ", billheaderId);
                    IEnumerable<BillDetailCustomModel> result = _context.Database.SqlQuery<BillDetailCustomModel>(spName, sqlParameters);
                    return result.ToList();
                }
            }
            catch (Exception)
            {
                //throw ex;
            }
            return new List<BillDetailCustomModel>();
        }

        /// <summary>
        /// Updates the bill headers by encounter identifier.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <param name="billheaderid">The billheaderid.</param>
        /// <returns></returns>
        public bool UpdateBillHeadersByBillHeaderIdEncounterId(int encounterId, int billheaderid)
        {
            try
            {
                var spName =
                    string.Format(
                        "EXEC {0} @pEncounterID, @BillHeaderID, @BillDetailLineNumber, @BillNumber, @AuthID, @AuthType, @AuthCode, @SelfPayFlag, @pReClaimFlag, @pClaimId",
                        StoredProcedures.SPROC_ApplyOrderToBillSetHeader);
                var sqlParameters = new SqlParameter[10];
                sqlParameters[0] = new SqlParameter("pEncounterID", encounterId);
                sqlParameters[1] = new SqlParameter
                {
                    ParameterName = "BillHeaderID",
                    Value = billheaderid
                    //Direction = ParameterDirection.Output,
                };
                sqlParameters[2] = new SqlParameter
                {
                    ParameterName = "BillDetailLineNumber",
                    Value = DBNull.Value
                    //Direction = ParameterDirection.Output,
                };
                sqlParameters[3] = new SqlParameter
                {
                    ParameterName = "BillNumber",
                    Value = DBNull.Value
                    //Direction = ParameterDirection.Output,
                    //Size = 50
                };
                sqlParameters[4] = new SqlParameter
                {
                    ParameterName = "AuthID",
                    Value = DBNull.Value
                    //Direction = ParameterDirection.Output,
                };
                sqlParameters[5] = new SqlParameter
                {
                    ParameterName = "AuthType",
                    Value = DBNull.Value
                    //Direction = ParameterDirection.Output,
                };
                sqlParameters[6] = new SqlParameter
                {
                    ParameterName = "AuthCode",
                    Value = DBNull.Value
                    // Direction = ParameterDirection.Output,
                    // Size = 50
                };
                sqlParameters[7] = new SqlParameter
                {
                    ParameterName = "SelfPayFlag",
                    Value = DBNull.Value
                    //Direction = ParameterDirection.Output
                };
                sqlParameters[8] = new SqlParameter("pReClaimFlag", DBNull.Value);
                sqlParameters[9] = new SqlParameter("pClaimId", DBNull.Value);
                ExecuteCommand(spName, sqlParameters);
                return true;
            }
            catch (Exception)
            {
                //throw ex;
            }
            return false;
        }

        /// <summary>
        /// Gets the encounter bill detail view.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        public List<BillDetailCustomModel> GetEncounterBillDetailView(int encounterId)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @pEncounterID ", StoredProcedures.SPROC_EncounterTransactionView);
                    var sqlParameters = new SqlParameter[1];
                    sqlParameters[0] = new SqlParameter("pEncounterID ", encounterId);
                    IEnumerable<BillDetailCustomModel> result = _context.Database.SqlQuery<BillDetailCustomModel>(spName, sqlParameters);
                    return result.ToList();
                }
            }
            catch (Exception)
            {
                //throw ex;
            }
            return new List<BillDetailCustomModel>();
        }

        /// <summary>
        /// Gets the claim trans details.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="tillDate">The till date.</param>
        /// <param name="displayBy">The display by.</param>
        /// <returns></returns>
        public List<BillTransmissionReportCustomModel> GetClaimTransDetails(int corporateId, int facilityId, DateTime? fromDate, DateTime? tillDate, int? displayBy)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @pCorporateID , @pFacilityID, @pFromDate, @pTillDate ,@pDisplayBy ", StoredProcedures.SPROC_Get_REP_ClaimTransDetails);
                    var sqlParameters = new SqlParameter[5];
                    sqlParameters[0] = new SqlParameter("pCorporateID", corporateId);
                    sqlParameters[1] = new SqlParameter("pFacilityID", facilityId);
                    sqlParameters[2] = new SqlParameter("pFromDate", fromDate);
                    sqlParameters[3] = new SqlParameter("pTillDate", tillDate);
                    sqlParameters[4] = new SqlParameter("pDisplayBy", displayBy);
                    IEnumerable<BillTransmissionReportCustomModel> result = _context.Database.SqlQuery<BillTransmissionReportCustomModel>(spName, sqlParameters);
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
        /// Gets the bill details by bill header identifier.
        /// </summary>
        /// <param name="billheaderId">The billheader identifier.</param>
        /// <returns></returns>
        public List<BillDetailCustomModel> GetBillDetailsByBillHeaderId(int billheaderId)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @pBillHeaderID ", StoredProcedures.SPROC_BillHeaderDetailsView);
                    var sqlParameters = new SqlParameter[1];
                    sqlParameters[0] = new SqlParameter("pBillHeaderID ", billheaderId);
                    IEnumerable<BillDetailCustomModel> result = _context.Database.SqlQuery<BillDetailCustomModel>(spName, sqlParameters);
                    return result.ToList();
                }
            }
            catch (Exception)
            {
                //throw ex;
            }
            return new List<BillDetailCustomModel>();
        }

        /// <summary>
        /// Adds the update manual charges.
        /// </summary>
        /// <param name="billActivityId">The bill activity identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public bool DeleteBillActivity(int billActivityId, int userId)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @pBillActivityID, @pCreatedBy", StoredProcedures.SPROC_DeleteBillActivites);
                    var sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("pBillActivityID ", billActivityId);
                    sqlParameters[1] = new SqlParameter("pCreatedBy ", userId);
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
        /// Recaluclates the bill.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <param name="reclaimFlag">The reclaim flag.</param>
        /// <param name="calimId">The calim identifier.</param>
        /// <returns></returns>
        public bool RecaluclateBill(int corporateId, int facilityId, int encounterId, string reclaimFlag, long calimId, int userId)
        {
            try
            {
                if (_context != null)
                {
                    var spName =
                        string.Format("EXEC {0} @pCorporateID,@pFacilityID,@pEncounterID,@pReClaimFlag,@pClaimId, @pLoggedInUserId",
                            StoredProcedures.SPROC_ReValuateCurrentBill);
                    var sqlParameters = new SqlParameter[6];
                    sqlParameters[0] = new SqlParameter("pCorporateID", corporateId);
                    sqlParameters[1] = new SqlParameter("pFacilityID", facilityId);
                    sqlParameters[2] = new SqlParameter("pEncounterID", encounterId);
                    sqlParameters[3] = new SqlParameter("pReClaimFlag", reclaimFlag);
                    sqlParameters[4] = new SqlParameter("pClaimId", calimId);
                    sqlParameters[5] = new SqlParameter("pLoggedInUserId", userId);

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
        /// Applies the bed charges.
        /// </summary>
        /// <param name="billHeaderId">The bill header identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public List<BillHeaderPreXMLModel> GetPreXMLFile(int billHeaderId, int facilityId)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @pFacilityId,@pBillHeaderId", StoredProcedures.SPROC_GetPreliminaryXmlFile);
                    var sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("pFacilityId", facilityId);
                    sqlParameters[1] = new SqlParameter("pBillHeaderId", billHeaderId);
                    IEnumerable<BillHeaderPreXMLModel> result = _context.Database.SqlQuery<BillHeaderPreXMLModel>(spName, sqlParameters);
                    return result.ToList();
                }
            }
            catch (Exception)
            {
                //throw ex;
            }
            return null;
        }

        /// <summary>
        /// Scrubs the XML bill.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <param name="claimId">The claim identifier.</param>
        /// <returns></returns>
        public bool ScrubXMLBill(int claimId, int userid)
        {
            try
            {
                if (_context != null)
                {
                    var spName =
                        string.Format("EXEC {0} @pBillHeaderID,@pExecutedBy,@pRETStatus",
                            StoredProcedures.SPROC_ScrubBill);
                    var sqlParameters = new SqlParameter[3];
                    sqlParameters[0] = new SqlParameter("pBillHeaderID", claimId);
                    sqlParameters[1] = new SqlParameter("pExecutedBy", userid);
                    sqlParameters[2] = new SqlParameter
                    {
                        ParameterName = "pRETStatus",
                        Value = DBNull.Value,
                        Size = Int32.MaxValue,
                        Direction = ParameterDirection.Output
                    };
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
        /// Gets the final bill payer headers list.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public List<BillHeaderCustomModel> GetFinalBillPayerHeadersList(int corporateId, int facilityId)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @pFID,@pCID", StoredProcedures.SPROC_GetPayerWiseFinalBills);
                    var sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("pFID ", facilityId);
                    sqlParameters[1] = new SqlParameter("pCID ", corporateId);
                    IEnumerable<BillHeaderCustomModel> result = _context.Database.SqlQuery<BillHeaderCustomModel>(spName, sqlParameters);
                    return result.ToList();
                }
            }
            catch (Exception)
            {
                //throw ex;
            }
            return new List<BillHeaderCustomModel>();
        }

        /// <summary>
        /// Gets the final bill by payer headers list.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="payerid">The payerid.</param>
        /// <returns></returns>
        public List<BillHeaderCustomModel> GetFinalBillByPayerHeadersList(int corporateId, int facilityId, string payerid)
        {
            //var spName = string.Format("EXEC {0} @pFID,@pCID,@pPayerId", StoredProcedures.SPROC_GetFinalBillsByPayer);
            var sqlParameters = new SqlParameter[3];
            sqlParameters[0] = new SqlParameter("pFID ", facilityId);
            sqlParameters[1] = new SqlParameter("pCID ", corporateId);
            sqlParameters[2] = new SqlParameter("pPayerId ", payerid);
            using (var ms = _context.MultiResultSetSqlQuery(StoredProcedures.SPROC_GetFinalBillsByPayer.ToString(), isCompiled: false, parameters: sqlParameters))
            {
                var result = ms.GetResultWithJson<BillHeaderCustomModel>(JsonResultsArray.Claims.ToString());
                return result;
            }
        }

        /// <summary>
        /// Finds the e claims.
        /// </summary>
        /// <param name="serachstring">The serachstring.</param>
        /// <param name="claimstatus">The claimstatus.</param>
        /// <param name="datefrom">The datefrom.</param>
        /// <param name="datetill">The datetill.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <returns></returns>
        public List<BillHeaderCustomModel> FindEClaims(string serachstring, string claimstatus, DateTime? datefrom, DateTime? datetill, int facilityId,
            int corporateId, int? fileid)
        {
            try
            {
                if (_context != null)
                {
                    var spName =
                        string.Format(
                            "EXEC {0} @pSearchString, @pClaimStatus, @pDateFrom, @pDateTill, @pFID, @pCID, @pFileId",
                            StoredProcedures.SPROC_FindEClaim);
                    var sqlParameters = new SqlParameter[7];
                    sqlParameters[0] = new SqlParameter("pSearchString", serachstring);
                    sqlParameters[1] = new SqlParameter("pClaimStatus", claimstatus);
                    sqlParameters[2] = new SqlParameter("pDateFrom", datefrom);
                    sqlParameters[3] = new SqlParameter("pDateTill", datetill);
                    sqlParameters[4] = new SqlParameter("pFID", facilityId);
                    sqlParameters[5] = new SqlParameter("pCID", corporateId);
                    sqlParameters[6] = new SqlParameter("pFileId", fileid);
                    IEnumerable<BillHeaderCustomModel> result = _context.Database.SqlQuery<BillHeaderCustomModel>(
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
        /// Sends the e claims by payer ids.
        /// </summary>
        /// <param name="senderId">The sender identifier.</param>
        /// <param name="dispositionFlag">The disposition flag.</param>
        /// <param name="payerIds">The payer ids.</param>
        /// <param name="billHeaderId">The bill header identifier.</param>
        /// <param name="payerId"></param>
        /// <param name="billHeaderIds"></param>
        /// <returns></returns>
        public List<BillHeaderXMLModel> SendEClaimsByPayerIds(int senderId, string dispositionFlag, string payerId, string billHeaderIds)
        {
            try
            {
                var spName = string.Format("EXEC {0} @SenderID, @DispositionFlag, @PayerID, @BillHeaderIds", StoredProcedures.SendEClaimByPayerIDs);
                var sqlParameters = new SqlParameter[4];
                sqlParameters[0] = new SqlParameter("SenderID", senderId);
                sqlParameters[1] = new SqlParameter("DispositionFlag", dispositionFlag);
                sqlParameters[2] = new SqlParameter("PayerID", payerId);
                sqlParameters[3] = new SqlParameter("BillHeaderIds", billHeaderIds);
                IEnumerable<BillHeaderXMLModel> result = _context.Database.SqlQuery<BillHeaderXMLModel>(spName, sqlParameters);
                return result.ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the bill format data.
        /// </summary>
        /// <param name="billheaderId">The billheader identifier.</param>
        /// <returns></returns>
        public List<BillPdfFormatCustomModel> GetBillFormatData(int billheaderId)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @pBillHeaderID ", StoredProcedures.SPROC_GetBillFormatData);
                    var sqlParameters = new SqlParameter[1];
                    sqlParameters[0] = new SqlParameter("pBillHeaderID ", billheaderId);
                    IEnumerable<BillPdfFormatCustomModel> result = _context.Database.SqlQuery<BillPdfFormatCustomModel>(spName, sqlParameters);
                    return result.ToList();
                }
            }
            catch (Exception)
            {
                //throw ex;
            }
            return new List<BillPdfFormatCustomModel>();
        }

        /// <summary>
        /// Gets the bill detail view.
        /// </summary>
        /// <param name="faiclityId">The faiclity identifier.</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <returns></returns>
        public List<BillHeaderCustomModel> GetBillHeaderList(int faiclityId, int corporateId)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @lFID,@lCID ", StoredProcedures.SPROC_BillHeaderList);
                    var sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("lFID ", faiclityId);
                    sqlParameters[1] = new SqlParameter("lCID ", corporateId);
                    IEnumerable<BillHeaderCustomModel> result = _context.Database.SqlQuery<BillHeaderCustomModel>(spName, sqlParameters);
                    return result.ToList();
                }
            }
            catch (Exception)
            {
                //throw ex;
            }
            return new List<BillHeaderCustomModel>();
        }



        /// <summary>
        /// Gets the final bill headers list.
        /// </summary>
        /// <param name="isEncounterselected">if set to <c>true</c> [is encounterselected].</param>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="faiclityId">The faiclity identifier.</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <returns></returns>
        public List<BillHeaderCustomModel> GetFinalBillHeadersList(
            bool isEncounterselected,
            int encounterId,
            int patientId,
            int faiclityId,
            int corporateId)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format(
                        "EXEC {0} @pIsEnc,@pEncId,@pPid,@pFID,@pCID ",
                        StoredProcedures.SPROC_GetFinalBillHeadersList);
                    var sqlParameters = new SqlParameter[5];
                    sqlParameters[0] = new SqlParameter("pIsEnc ", Convert.ToInt32(isEncounterselected));
                    sqlParameters[1] = new SqlParameter("pEncId ", encounterId);
                    sqlParameters[2] = new SqlParameter("pPid ", patientId);
                    sqlParameters[3] = new SqlParameter("pFID ", faiclityId);
                    sqlParameters[4] = new SqlParameter("pCID ", corporateId);
                    IEnumerable<BillHeaderCustomModel> result = _context.Database.SqlQuery<BillHeaderCustomModel>(
                        spName,
                        sqlParameters);
                    return result.ToList();
                }
            }
            catch
            {
                //throw ex;
            }
            return new List<BillHeaderCustomModel>();
        }

    }
}