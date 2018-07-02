using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;


namespace BillingSystem.Bal.BusinessAccess
{
    public class ProjectDashboardService : IProjectDashboardService
    {
        private readonly IRepository<ProjectDashboard> _repository;

        private readonly BillingEntities _context;

        public ProjectDashboardService(IRepository<ProjectDashboard> repository, BillingEntities context)
        {
            _repository = repository;
            _context = context;
        }

        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<ProjectDashboardCustomModel> GetProjectDashboardList(int corporateid, int facilityid)
        {
            var list = new List<ProjectDashboardCustomModel>();
            var lstProjectDashboard = _repository.Where(a => a.CorporateId == corporateid && a.FacilityId == facilityid).ToList();
            if (lstProjectDashboard.Count > 0)
            {
                list.AddRange(lstProjectDashboard.Select(item => new ProjectDashboardCustomModel
                {

                }));
            }
            return list;
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public List<ProjectDashboardCustomModel> SaveProjectDashboard(ProjectDashboard model)
        {
            if (model.Id > 0)
            {
                var current = _repository.GetSingle(model.Id);
                model.CreatedBy = current.CreatedBy;
                model.CreatedDate = current.CreatedDate;
                _repository.UpdateEntity(model, model.Id);
            }
            else
                _repository.Create(model);

            var list = GetProjectDashboardList(Convert.ToInt32(model.CorporateId), Convert.ToInt32(model.FacilityId));
            return list;
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ProjectDashboard GetProjectDashboardById(int? id)
        {
            var model = _repository.Where(x => x.Id == id).FirstOrDefault();
            return model;
        }

        /// <summary>
        /// Gets the project dashboard data.
        /// </summary>
        /// <param name="corporateid">The corporateid.</param>
        /// <param name="facilityid">The facilityid.</param>
        /// <param name="year">The year.</param>
        /// <returns></returns>
        public List<ProjectDashboardCustomModel> GetProjectDashboardData(int corporateid, int facilityid, string year)
        {
            var spName = string.Format("EXEC {0} @pCID,@pFID,@pYear ", StoredProcedures.SPROC_GetProjectDashbaordData.ToString());
            var sqlParameters = new SqlParameter[3];
            sqlParameters[0] = new SqlParameter("pCID ", corporateid);
            sqlParameters[1] = new SqlParameter("pFID ", facilityid);
            sqlParameters[2] = new SqlParameter("pYear ", year);
            var result = _context.Database.SqlQuery<ProjectDashboardCustomModel>(spName, sqlParameters);
            return result.ToList();
        }


    }
}
