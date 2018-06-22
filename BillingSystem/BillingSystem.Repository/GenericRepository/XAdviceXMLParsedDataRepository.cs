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
    public class XAdviceXMLParsedDataRepository : GenericRepository<XAdviceXMLParsedData>
    {
        private readonly DbContext _context;

        public XAdviceXMLParsedDataRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
            _context = context;
        }

        /// <summary>
        /// Sends the e claims.
        /// </summary>
        /// <param name="senderId">The sender identifier.</param>
        /// <param name="dispositionFlag">The disposition flag.</param>
        /// <returns></returns>
        public List<XAdviceXMLParsedDataCustomModel> GetXAdviceXMLParsedDataCustom(int corporateId, int facilityId)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @CId, @FId", StoredProcedures.SPROC_GetXMLParsedDataRemittanceAdvice);
                    var sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("CId", corporateId);
                    sqlParameters[1] = new SqlParameter("FId", facilityId);
                    IEnumerable<XAdviceXMLParsedDataCustomModel> result = _context.Database.SqlQuery<XAdviceXMLParsedDataCustomModel>(spName, sqlParameters);
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
        /// Gets the x advice XML parsed data by identifier.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="fileid">The fileid.</param>
        /// <returns></returns>
        public List<XAdviceXMLParsedDataCustomModel> GetXAdviceXmlParsedDataById(int corporateId, int facilityId, int fileid)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @pCId, @pFId, @pFileId", StoredProcedures.SPROC_GetXMLParsedDataRemittanceAdviceById);
                    var sqlParameters = new SqlParameter[3];
                    sqlParameters[0] = new SqlParameter("pCId", corporateId);
                    sqlParameters[1] = new SqlParameter("pFId", facilityId);
                    sqlParameters[2] = new SqlParameter("pFileId", fileid);
                    IEnumerable<XAdviceXMLParsedDataCustomModel> result = _context.Database.SqlQuery<XAdviceXMLParsedDataCustomModel>(spName, sqlParameters);
                    return result.ToList();
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
