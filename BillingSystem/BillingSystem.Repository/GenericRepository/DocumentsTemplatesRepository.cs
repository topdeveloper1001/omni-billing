using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Repository.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BillingSystem.Repository.GenericRepository
{
    public class DocumentsTemplatesRepository : GenericRepository<DocumentsTemplates>
    {
        private readonly DbContext _context;

        public DocumentsTemplatesRepository(BillingEntities context)
           : base(context)
        {
            AutoSave = true;
            _context = context;
        }


        public async Task<IEnumerable<DocumentsTemplatesCustomModel>> SaveDocumentsAsync(DataTable dt, bool showDocsList, string exclusions)
        {
            try
            {
                var sqlParams = new SqlParameter[3];
                sqlParams[0] = new SqlParameter
                {
                    ParameterName = "@pDocs",
                    SqlDbType = SqlDbType.Structured,
                    Value = dt,
                    TypeName = "TypeDocumentTemplate"
                };
                sqlParams[1] = new SqlParameter("@pWithDocs", showDocsList);
                sqlParams[2] = new SqlParameter("@pExclusions", exclusions);

                using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SprocUploadFiles.ToString(), false, parameters: sqlParams))
                {
                    IEnumerable<DocumentsTemplatesCustomModel> docs = null;

                    if (showDocsList)
                        docs = (await r.ResultSetForAsync<DocumentsTemplatesCustomModel>()).ToList();
                    return docs;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<List<DocumentsTemplates>> GetDocumentsAsync(long facilityId = 0, long corporateId = 0, long userId = 0, long patientId = 0, string exclusions = "")
        {
            try
            {
                var sqlParams = new SqlParameter[5];
                sqlParams[0] = new SqlParameter("@pFId", facilityId);
                sqlParams[1] = new SqlParameter("@pCId", corporateId);
                sqlParams[2] = new SqlParameter("@pUserId", userId);
                sqlParams[3] = new SqlParameter("@pPId", patientId);
                sqlParams[4] = new SqlParameter("@pExclusions", exclusions);

                using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetDocumentsByPatient.ToString(), false, parameters: sqlParams))
                {
                    var docs = (await r.ResultSetForAsync<DocumentsTemplates>()).ToList();
                    return docs;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
