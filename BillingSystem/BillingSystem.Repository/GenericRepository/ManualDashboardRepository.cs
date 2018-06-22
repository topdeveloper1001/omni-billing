using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Repository.GenericRepository
{
    public class ManualDashboardRepository : GenericRepository<ManualDashboard>
    {
        private readonly DbContext _context;

        public ManualDashboardRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
            _context = context;
        }

        /// <summary>
        /// Sends the e claims.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="tokenExpiryDate">The token expiry date.</param>
        /// <returns></returns>
        public List<UserTokenCustomModel> GenerateUserToken(string username, DateTime tokenExpiryDate)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @pUserName, @pExpiryDate", StoredProcedures.SPROC_GenerateUpdateToken);
                    var sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("pUserName", username);
                    sqlParameters[1] = new SqlParameter("pExpiryDate", tokenExpiryDate);
                    IEnumerable<UserTokenCustomModel> result = _context.Database.SqlQuery<UserTokenCustomModel>(spName, sqlParameters);
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
        /// Updates the indicator v1.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public bool UpdateIndicatorV1(ManualDashboard model)
        {
            try
            {
                if (_context != null)
                {
                    var spName =
                        string.Format(
                            "EXEC {0} @CID, @FID, @IndicatorNumber, @BudgetType,@CurrentYear,@SubCategory1,@SubCategory2,@INVal1,@INVal2,@INVal3,@INVal4,@INVal5,@INVal6,@INVal7,@INVal8,@INVal9,@INVal10,@INVal11,@INVal12",
                            StoredProcedures.SPROC_DBDMain_IndicatorUpdate);
                    var sqlParameters = new SqlParameter[19];
                    sqlParameters[0] = new SqlParameter("CID", model.CorporateId);
                    sqlParameters[1] = new SqlParameter("FID", model.FacilityId);
                    sqlParameters[2] = new SqlParameter("IndicatorNumber", model.Indicators);
                    sqlParameters[3] = new SqlParameter("BudgetType", model.BudgetType);
                    sqlParameters[4] = new SqlParameter("CurrentYear", model.Year);
                    sqlParameters[5] = new SqlParameter("SubCategory1", model.SubCategory1);
                    sqlParameters[6] = new SqlParameter("SubCategory2", model.SubCategory2);
                    sqlParameters[7] = new SqlParameter("INVal1", model.M1 ?? "0.0000");
                    sqlParameters[8] = new SqlParameter("INVal2", model.M2 ?? "0.0000");
                    sqlParameters[9] = new SqlParameter("INVal3", model.M3 ?? "0.0000");
                    sqlParameters[10] = new SqlParameter("INVal4", model.M4 ?? "0.0000");
                    sqlParameters[11] = new SqlParameter("INVal5", model.M5 ?? "0.0000");
                    sqlParameters[12] = new SqlParameter("INVal6", model.M6 ?? "0.0000");
                    sqlParameters[13] = new SqlParameter("INVal7", model.M7 ?? "0.0000");
                    sqlParameters[14] = new SqlParameter("INVal8", model.M8 ?? "0.0000");
                    sqlParameters[15] = new SqlParameter("INVal9", model.M9 ?? "0.0000");
                    sqlParameters[16] = new SqlParameter("INVal10", model.M10 ?? "0.0000");
                    sqlParameters[17] = new SqlParameter("INVal11", model.M11 ?? "0.0000");
                    sqlParameters[18] = new SqlParameter("INVal12", model.M12 ?? "0.0000");

                    ExecuteCommand(spName, sqlParameters);
                    return true;
                }
            }
            catch (Exception )
            {
                return false;
            }
            return true;
        }
    }
}
