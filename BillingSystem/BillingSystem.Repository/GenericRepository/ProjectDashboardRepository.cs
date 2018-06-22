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
    public class ProjectDashboardRepository : GenericRepository<ProjectDashboard>
    {
        private readonly DbContext _context;

        public ProjectDashboardRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
            _context = context;
        }


        /// <summary>
        /// Gets the project dashboard data.
        /// </summary>
        /// <param name="corpoarteid">The corpoarteid.</param>
        /// <param name="facilityid">The facilityid.</param>
        /// <param name="year">The year.</param>
        /// <returns></returns>
        public List<ProjectDashboardCustomModel> GetProjectDashboardData(int corpoarteid, int facilityid, string year)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @pCID,@pFID,@pYear ", StoredProcedures.SPROC_GetProjectDashbaordData.ToString());
                    var sqlParameters = new SqlParameter[3];
                    sqlParameters[0] = new SqlParameter("pCID ", corpoarteid);
                    sqlParameters[1] = new SqlParameter("pFID ", facilityid);
                    sqlParameters[2] = new SqlParameter("pYear ", year);
                    IEnumerable<ProjectDashboardCustomModel> result = _context.Database.SqlQuery<ProjectDashboardCustomModel>(spName, sqlParameters);
                    return result.ToList();
                }
            }
            catch (Exception )
            {
                //throw ex;
            }
            return new List<ProjectDashboardCustomModel>();
        }
        public Int32 SaveMonthWiseValuesInProjectDashboard(string ProjectNumber, string Month, string CorporateId,
            string FacilityId, string TaskNumber)
        {
            Int32 result = 0;
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @ProjectNumber,@Month,@CorporateId,@FacilityId,@TaskNumber", StoredProcedures.SPROC_AddMonthWisevaluesInProjectDashboard.ToString());
                    var sqlParameters = new SqlParameter[5];
                    sqlParameters[0] = new SqlParameter("ProjectNumber ", ProjectNumber);
                    sqlParameters[1] = new SqlParameter("Month ", Month);
                    sqlParameters[2] = new SqlParameter("CorporateId ", CorporateId);
                    sqlParameters[3] = new SqlParameter("FacilityId ", FacilityId);
                    sqlParameters[4] = new SqlParameter("TaskNumber ", TaskNumber);
                    result = _context.Database.SqlQuery<Int32>(spName, sqlParameters).FirstOrDefault();
                }
            }
            catch (Exception )
            {
                //throw ex;
            }
            return result;
        }
    }
}
