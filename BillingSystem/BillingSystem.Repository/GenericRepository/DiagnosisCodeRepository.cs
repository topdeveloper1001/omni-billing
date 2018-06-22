using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Repository.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace BillingSystem.Repository.GenericRepository
{
    public class DiagnosisCodeRepository : GenericRepository<DiagnosisCode>
    {
        private readonly DbContext _context;


        public DiagnosisCodeRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
            _context = context;
        }

        public IEnumerable<DiagnosisCode> GetDiagnosisCodes(string keyword, string tableNumber, long userId, long facilityId)
        {
            var sqlParams = new SqlParameter[4];
            sqlParams[0] = new SqlParameter("@pUserId", userId);
            sqlParams[1] = new SqlParameter("@pKeyword", keyword);
            sqlParams[2] = new SqlParameter("@pTableNumber", tableNumber);
            sqlParams[3] = new SqlParameter("@pFId", facilityId);

            using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetDiagnosisCodes.ToString(), isCompiled: false, parameters: sqlParams))
            {
                try
                {
                    var list = r.GetResultWithJson<DiagnosisCode>(JsonResultsArray.DiagnosisCodes.ToString());
                    return list;
                }
                catch (Exception ex)
                {
                    //throw;
                }
            }
            return Enumerable.Empty<DiagnosisCode>().ToList();
        }
    }
}
