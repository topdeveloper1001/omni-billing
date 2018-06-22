using System;
using System.Data.Entity;
using System.Data.SqlClient;
using BillingSystem.Common.Common;
using BillingSystem.Model;

namespace BillingSystem.Repository.GenericRepository
{
    public class ProjectTaskTargetsRepository : GenericRepository<ProjectTaskTargets>
    {
        private readonly DbContext _context;
        public ProjectTaskTargetsRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
            _context = context;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldTaskNumber"></param>
        /// <param name="newTaskNumber"></param>
        /// <param name="projectTaskId"></param>
        /// <returns></returns>
        public bool UpdateProjectTaskTargetTaskNumber(string oldTaskNumber, string newTaskNumber, string projectTaskId)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @OldTaskNumber,@NewTaskNumber,@ProjectTaskId ", StoredProcedures.SPROC_UpdateProjectTaskTargetTaskNumber);
                    var sqlParameters = new SqlParameter[3];
                    sqlParameters[0] = new SqlParameter("OldTaskNumber", oldTaskNumber);
                    sqlParameters[1] = new SqlParameter("NewTaskNumber", newTaskNumber);
                    sqlParameters[2] = new SqlParameter("ProjectTaskId", projectTaskId);
                    ExecuteCommand(spName, sqlParameters);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return false;
        }
    }
}
