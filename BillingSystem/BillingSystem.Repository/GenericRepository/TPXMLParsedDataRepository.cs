using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Repository.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BillingSystem.Repository.GenericRepository
{
    public class TPXMLParsedDataRepository : GenericRepository<TPXMLParsedData>
    {
        /// <summary>
        /// The _context.
        /// </summary>
        private readonly DbContext _context;

        public TPXMLParsedDataRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
            _context = context;
        }


        public List<TPXMLParsedDataCustomModel> GetXmlParsedData(long tpFileId)
        {
            try
            {
                var spName = $"EXEC {StoredProcedures.SPROCGetXMLParsedDataByFileId} @FileId";
                var sqlParameters = new SqlParameter[1];
                sqlParameters[0] = new SqlParameter("FileId", tpFileId);

                using (var r = _context.MultiResultSetSqlQuery(spName, parameters: sqlParameters))
                {
                    var result = r.ResultSetFor<TPXMLParsedDataCustomModel>().ToList();
                    return result != null ? result : new List<TPXMLParsedDataCustomModel>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
