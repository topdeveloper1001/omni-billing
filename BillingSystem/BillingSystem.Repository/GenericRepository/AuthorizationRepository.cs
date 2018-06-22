using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using System.Threading.Tasks;
using BillingSystem.Repository.Common;

namespace BillingSystem.Repository.GenericRepository
{
    public class AuthorizationRepository : GenericRepository<Authorization>
    {
        private readonly DbContext _context;

        public AuthorizationRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
            _context = context;
        }


        /// <summary>
        /// Sends the e claims.
        /// </summary>
        /// <param name="facilityId">The sender identifier.</param>
        /// <param name="dispositionFlag">The disposition flag.</param>
        /// <returns></returns>
        public List<BillHeaderXMLModel> GenerateEAuthorizationFile(int facilityId, string dispositionFlag)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @SenderID, @DispositionFlag", StoredProcedures.GenerateEAuthorizationFile);
                    var sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("SenderID", facilityId);
                    sqlParameters[1] = new SqlParameter("DispositionFlag", dispositionFlag);
                    IEnumerable<BillHeaderXMLModel> result = _context.Database.SqlQuery<BillHeaderXMLModel>(spName, sqlParameters);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        public async Task<AuthorizationViewData> SaveAsync(Authorization m, string filesJson, bool withAuthList, bool withDocs, long missedEncounterId = 0)
        {
            try
            {
                var sqlParams = new SqlParameter[23];
                sqlParams[0] = new SqlParameter("@Id", m.AuthorizationID);
                sqlParams[1] = new SqlParameter("@pCId", m.CorporateID);
                sqlParams[2] = new SqlParameter("@pFId", m.FacilityID);
                sqlParams[3] = new SqlParameter("@pPId", m.PatientID);
                sqlParams[4] = new SqlParameter("@pEId", m.EncounterID);
                sqlParams[5] = new SqlParameter("@pDateOrdered", m.AuthorizationDateOrdered == null ? m.CreatedDate : m.AuthorizationDateOrdered);
                sqlParams[6] = new SqlParameter("@pStart", m.AuthorizationStart);
                sqlParams[7] = new SqlParameter("@pEnd", m.AuthorizationEnd);
                sqlParams[8] = new SqlParameter("@pCode", m.AuthorizationCode);
                sqlParams[9] = new SqlParameter("@pType", Convert.ToInt32(m.AuthorizationType));
                sqlParams[10] = new SqlParameter("@pComments", string.IsNullOrEmpty(m.AuthorizationComments) ? string.Empty : m.AuthorizationComments);
                sqlParams[11] = new SqlParameter("@pDenialCode", Convert.ToInt32(m.AuthorizationDenialCode));
                sqlParams[12] = new SqlParameter("@pIDPayer", string.IsNullOrEmpty(m.AuthorizationIDPayer) ? string.Empty : m.AuthorizationIDPayer);
                sqlParams[13] = new SqlParameter("@pLimit", m.AuthorizationLimit);
                sqlParams[14] = new SqlParameter("@pMemberId", string.IsNullOrEmpty(m.AuthorizationMemberID) ? string.Empty : m.AuthorizationMemberID);
                sqlParams[15] = new SqlParameter("@pResult", Convert.ToInt32(m.AuthorizationResult));
                sqlParams[16] = new SqlParameter("@pUserId", m.CreatedBy);
                sqlParams[17] = new SqlParameter("@pCreatedDate", m.CreatedDate);
                sqlParams[18] = new SqlParameter("@pServiceLevel", m.AuthorizedServiceLevel);
                sqlParams[19] = new SqlParameter("@pFilesDataInJson", filesJson);
                sqlParams[20] = new SqlParameter("@pWithAuthList", withAuthList);
                sqlParams[21] = new SqlParameter("@pWithDocs", withDocs);
                sqlParams[22] = new SqlParameter("@pMissedEId", missedEncounterId);

                using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SprocSaveAuthorization.ToString(), false, parameters: sqlParams))
                {
                    if (missedEncounterId > 0)
                    {
                        var statusEn = (await r.ResultSetForAsync<int>()).FirstOrDefault();
                    }

                    var viewData = new AuthorizationViewData
                    {
                        AuthorizationId = (await r.ResultSetForAsync<int>()).FirstOrDefault()
                    };

                    if (withAuthList)
                        viewData.AuthList = (await r.ResultSetForAsync<AuthorizationCustomModel>()).ToList();

                    if (withDocs)
                    {
                        var docs = (await r.ResultSetForAsync<DocumentsTemplates>()).ToList();
                        if (docs.Any())
                            viewData.Docs = docs.Where(a => a.DocumentName.Equals("Authorization File")).ToList();
                    }
                    return viewData;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public AuthorizationViewData SaveAuthAsync(Authorization m, string filesJson, bool withAuthList, bool withDocs, long missedEncounterId = 0)
        {
            try
            {
                var sqlParams = new SqlParameter[23];
                sqlParams[0] = new SqlParameter("@Id", m.AuthorizationID);
                sqlParams[1] = new SqlParameter("@pCId", m.CorporateID);
                sqlParams[2] = new SqlParameter("@pFId", m.FacilityID);
                sqlParams[3] = new SqlParameter("@pPId", m.PatientID);
                sqlParams[4] = new SqlParameter("@pEId", m.EncounterID);
                sqlParams[5] = new SqlParameter("@pDateOrdered", m.AuthorizationDateOrdered == null ? m.CreatedDate : m.AuthorizationDateOrdered);
                sqlParams[6] = new SqlParameter("@pStart", m.AuthorizationStart);
                sqlParams[7] = new SqlParameter("@pEnd", m.AuthorizationEnd);
                sqlParams[8] = new SqlParameter("@pCode", m.AuthorizationCode);
                sqlParams[9] = new SqlParameter("@pType", Convert.ToInt32(m.AuthorizationType));
                sqlParams[10] = new SqlParameter("@pComments", string.IsNullOrEmpty(m.AuthorizationComments) ? string.Empty : m.AuthorizationComments);
                sqlParams[11] = new SqlParameter("@pDenialCode", Convert.ToInt32(m.AuthorizationDenialCode));
                sqlParams[12] = new SqlParameter("@pIDPayer", string.IsNullOrEmpty(m.AuthorizationIDPayer) ? string.Empty : m.AuthorizationIDPayer);
                sqlParams[13] = new SqlParameter("@pLimit", m.AuthorizationLimit);
                sqlParams[14] = new SqlParameter("@pMemberId", string.IsNullOrEmpty(m.AuthorizationMemberID) ? string.Empty : m.AuthorizationMemberID);
                sqlParams[15] = new SqlParameter("@pResult", Convert.ToInt32(m.AuthorizationResult));
                sqlParams[16] = new SqlParameter("@pUserId", m.CreatedBy);
                sqlParams[17] = new SqlParameter("@pCreatedDate", m.CreatedDate);
                sqlParams[18] = new SqlParameter("@pServiceLevel", m.AuthorizedServiceLevel);
                sqlParams[19] = new SqlParameter("@pFilesDataInJson", filesJson);
                sqlParams[20] = new SqlParameter("@pWithAuthList", withAuthList);
                sqlParams[21] = new SqlParameter("@pWithDocs", withDocs);
                sqlParams[22] = new SqlParameter("@pMissedEId", missedEncounterId);

                using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SprocSaveAuthorization.ToString(), false, parameters: sqlParams))
                {
                    if (missedEncounterId > 0)
                    {
                        var statusEn = r.ResultSetFor<int>().FirstOrDefault();
                    }

                    var viewData = new AuthorizationViewData
                    {
                        AuthorizationId = r.ResultSetFor<int>().FirstOrDefault()
                    };

                    if (withAuthList)
                        viewData.AuthList = r.ResultSetFor<AuthorizationCustomModel>().ToList();

                    if (withDocs)
                    {
                        var docs = r.ResultSetFor<DocumentsTemplates>().ToList();
                        if (docs.Any())
                            viewData.Docs = docs.Where(a => a.DocumentName.Equals("Authorization File")).ToList();
                    }
                    return viewData;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
