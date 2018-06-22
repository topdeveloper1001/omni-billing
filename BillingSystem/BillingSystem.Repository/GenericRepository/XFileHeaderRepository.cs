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
    public class XFileHeaderRepository : GenericRepository<XFileHeader>
    {
        private readonly DbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="XFileHeaderRepository"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public XFileHeaderRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
            _context = context;
        }


        /// <summary>
        /// Imports the XML billing.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <param name="fullPath">The full path.</param>
        /// <param name="successFlag">if set to <c>true</c> [success flag].</param>
        /// <returns></returns>
        public bool ImportXMLBilling(string xml, string fullPath, bool successFlag)
        {
            try
            {
                if (_context != null)
                {
                    var spToExecute = string.Format("EXEC {0} @XMLIN, @FullPath, @SuccessFlag", StoredProcedures.XMLParser.ToString());
                    var sqlParameters = new SqlParameter[3];
                    sqlParameters[0] = new SqlParameter
                    {
                        SqlDbType = SqlDbType.Xml,
                        ParameterName = "XMLIN",
                        Value = xml
                    };
                    sqlParameters[1] = new SqlParameter("FullPath", fullPath);
                    sqlParameters[2] = new SqlParameter
                    {
                        ParameterName = "SuccessFlag",
                        Value = successFlag,
                        Direction = ParameterDirection.Output
                    };
                    ExecuteCommand(spToExecute, sqlParameters);
                    return true;
                }
            }
            catch (Exception )
            {
                //throw ex;
            }
            return false;
        }
    }
}
