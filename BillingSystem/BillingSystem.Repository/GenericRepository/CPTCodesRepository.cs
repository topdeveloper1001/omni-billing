using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Repository.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BillingSystem.Repository.GenericRepository
{
    public class CPTCodesRepository : GenericRepository<CPTCodes>
    {
        private readonly DbContext _context;

        public CPTCodesRepository(BillingEntities context)
            : base(context)
        {
            _context = context;
            AutoSave = true;
        }


        public List<ExportCodesData> GetCodesDataToExport(long cId, long fId, long userId, string tableNumber, string codeType, string searchText, out string columns, string tableName = "")
        {
            columns = string.Empty;
            try
            {
                var sqlParams = new SqlParameter[7];
                sqlParams[0] = new SqlParameter("@pSearchText", !string.IsNullOrEmpty(searchText) ? searchText : string.Empty);
                sqlParams[1] = new SqlParameter("@pCodeType", codeType);
                sqlParams[2] = new SqlParameter("@pCId", cId);
                sqlParams[3] = new SqlParameter("@pFId", fId);
                sqlParams[4] = new SqlParameter("@pTableNumber", !string.IsNullOrEmpty(tableNumber) ? tableNumber : string.Empty);
                sqlParams[5] = new SqlParameter("@pTableName", !string.IsNullOrEmpty(tableName) ? tableName : string.Empty);
                sqlParams[6] = new SqlParameter("@pUserId", userId);

                using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetOrderCodesToExport.ToString(), false, parameters: sqlParams))
                {
                    var result = r.ResultSetFor<ExportCodesData>().ToList();
                    columns = r.ResultSetFor<string>().FirstOrDefault();
                    return result;
                }
            }
            catch (System.Exception ex)
            {
                //throw ex;
                return new List<ExportCodesData>();
            }
        }

        public bool ImportBillingCodes(string codeType, long cId, long fId, string tno, string tname, long loggedinUserId, DataTable dt)
        {
            var status = false;
            var sqlParams = new SqlParameter[7];

            sqlParams[0] = new SqlParameter("@pCodeType", codeType);
            sqlParams[1] = new SqlParameter("@pCId", cId);
            sqlParams[2] = new SqlParameter("@pFId", fId);
            sqlParams[3] = new SqlParameter("@pTableNumber", tno);
            sqlParams[4] = new SqlParameter("@pTableName", tname);
            sqlParams[5] = new SqlParameter("@pUserId", loggedinUserId);

            sqlParams[6] = new SqlParameter
            {
                ParameterName = "@TCodes",
                SqlDbType = SqlDbType.Structured,
                Value = dt,
                TypeName = "SqlBillingCodeType"
            };

            try
            {
                ExecuteCommand(StoredProcedures.SprocImportBillingCodes.ToString(), sqlParams, isCompiled: false);
                status = true;
            }
            catch (Exception ex)
            {
                status = false;
            }
            return status;
        }
    }
}
