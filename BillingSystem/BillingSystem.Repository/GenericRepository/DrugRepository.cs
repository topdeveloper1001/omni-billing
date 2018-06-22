using System;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using Microsoft.SqlServer.Server;

namespace BillingSystem.Repository.GenericRepository
{
    public class DrugRepository : GenericRepository<Drug>
    {
        #region Fields

        /// <summary>
        /// The _context.
        /// </summary>
        private readonly DbContext _context;

        #endregion
        public DrugRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
            _context = context;
        }

        public string ImportDrugCodesToDB(DataTable dt, Int32 loggedInUser, string tableNumber, string type)
        {
            var returnStr = "";
            try
            {
                if (_context != null)
                {
                    string spName =
                        string.Format(
                            "EXEC {0} @insertDrugExcelToDB, @LoggedInUser, @TableNumber, @Type",
                            StoredProcedures.SPROC_InsertDrugExcelToDB);
                    var sqlParameters = new SqlParameter[4];
                    sqlParameters[0] = new SqlParameter
                    {
                        ParameterName = "insertDrugExcelToDB",
                        SqlDbType = SqlDbType.Structured,
                        Value = dt,
                        TypeName = "DrugTT"
                    };
                    //sqlParameters[0] = new SqlParameter("insertDrugExcelToDB", dt);
                    sqlParameters[1] = new SqlParameter("LoggedInUser", loggedInUser);
                    sqlParameters[2] = new SqlParameter("TableNumber", tableNumber);
                    sqlParameters[3] = new SqlParameter("Type", type);
                    ExecuteCommand(spName, sqlParameters);
                    returnStr = "Successfully imported!";
                }
            }
            catch (Exception ex)
            {
                //throw ex;
                returnStr = ex.Message + "`0";
            }
            return returnStr;
        }
    }
}
