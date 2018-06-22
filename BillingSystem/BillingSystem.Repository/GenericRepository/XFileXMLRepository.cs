using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI.WebControls;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Repository.GenericRepository
{
    public class XFileXMLRepository : GenericRepository<XFileXML>
    {
        private readonly DbContext _context;

        public XFileXMLRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
            _context = context;
        }


        /// <summary>
        /// XMLs the remittance advice parser.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <param name="fullPath">The full path.</param>
        /// <param name="successFlag">if set to <c>true</c> [success flag].</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public string XMLRemittanceAdviceParser(string xml, string fullPath, bool successFlag, int corporateId, int facilityId)
        {
            try
            {
                if (_context != null)
                {
                    var spToExecute = string.Format("EXEC {0} @XMLIN, @FullPath, @CorporateID, @FacilityID, @SuccessFlag", StoredProcedures.XMLRemittanceAdviceParser.ToString());
                    var sqlParameters = new SqlParameter[5];
                    sqlParameters[0] = new SqlParameter
                    {
                        SqlDbType = SqlDbType.Xml,
                        ParameterName = "XMLIN",
                        Value = xml
                    };
                    sqlParameters[1] = new SqlParameter("FullPath", fullPath);
                    sqlParameters[2] = new SqlParameter("CorporateID", corporateId);
                    sqlParameters[3] = new SqlParameter("FacilityID", facilityId);
                    sqlParameters[4] = new SqlParameter
                    {
                        ParameterName = "SuccessFlag",
                        Value = successFlag,
                        Direction = ParameterDirection.Output
                    };
                    //ExecuteCommand(spToExecute, sqlParameters);
                    IEnumerable<XMLBillingFileStatus> result = _context.Database.SqlQuery<XMLBillingFileStatus>(spToExecute, sqlParameters);
                    var filestatus = result.ToList();
                    if (filestatus.Any())
                    {
                        return (filestatus.FirstOrDefault().SuccessFlag);
                    }
                    //return true;
                }
            }
            catch (Exception)
            {
                //throw ex;
            }
            return string.Empty;
        }

        /// <summary>
        /// xes the advice XML parsed data.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <param name="fullPath">The full path.</param>
        /// <param name="successFlag">if set to <c>true</c> [success flag].</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public bool XAdviceXMLParsedData(string xml, string fullPath, bool successFlag, int corporateId, int facilityId)
        {
            try
            {
                if (_context != null)
                {
                    var spToExecute = string.Format("EXEC {0} @XMLIN, @FullPath, @CorporateID, @FacilityID, @SuccessFlag", StoredProcedures.XMLRemittanceAdviceParser.ToString());
                    var sqlParameters = new SqlParameter[5];
                    sqlParameters[0] = new SqlParameter
                    {
                        SqlDbType = SqlDbType.Xml,
                        ParameterName = "XMLIN",
                        Value = xml
                    };
                    sqlParameters[1] = new SqlParameter("FullPath", fullPath);
                    sqlParameters[2] = new SqlParameter("CorporateID", corporateId);
                    sqlParameters[3] = new SqlParameter("FacilityID", facilityId);
                    sqlParameters[4] = new SqlParameter
                    {
                        ParameterName = "SuccessFlag",
                        Value = successFlag,
                        Direction = ParameterDirection.Output
                    };
                    ExecuteCommand(spToExecute, sqlParameters);
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
        /// XMLs the bill file parser.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <param name="fullPath">The full path.</param>
        /// <param name="successFlag">if set to <c>true</c> [success flag].</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="batchNumber">The batch number.</param>
        /// <returns></returns>
        public string XMLBillFileParser(string xml, string fullPath, bool successFlag, int corporateId, int facilityId, string batchNumber, bool? withDetails = false, long? loggedinUserId = 0)
        {
            try
            {
                if (_context != null)
                {
                    var spToExecute = string.Format("EXEC {0} @pCID, @XMLIN, @FullPath,@pFID, @SuccessFlag,@UBatchNumber,@pExecuteDetails,@pLoggedInUserId", StoredProcedures.XMLParser.ToString());
                    var sqlParameters = new SqlParameter[8];
                    sqlParameters[0] = new SqlParameter("pCID", corporateId);
                    sqlParameters[1] = new SqlParameter
                    {
                        SqlDbType = SqlDbType.Xml,
                        ParameterName = "XMLIN",
                        Value = xml
                    };
                    sqlParameters[2] = new SqlParameter("FullPath", fullPath);
                    sqlParameters[3] = new SqlParameter("pFID", facilityId);
                    sqlParameters[4] = new SqlParameter
                    {
                        ParameterName = "SuccessFlag",
                        Value = successFlag,
                        Direction = ParameterDirection.Output
                    };
                    sqlParameters[5] = new SqlParameter("UBatchNumber", batchNumber);
                    sqlParameters[6] = new SqlParameter("pExecuteDetails", withDetails);
                    sqlParameters[7] = new SqlParameter("pLoggedInUserId", loggedinUserId);
                    IEnumerable<XMLBillingFileStatus> result = _context.Database.SqlQuery<XMLBillingFileStatus>(spToExecute, sqlParameters);
                    var filestatus = result.ToList();
                    if (filestatus.Any())
                    {
                        return (filestatus.FirstOrDefault().SuccessFlag);
                    }
                }
            }
            catch (Exception)
            {
                //throw ex;
            }
            return string.Empty;
        }
    }
}
