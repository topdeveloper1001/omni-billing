using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Repository.UOW;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.BusinessAccess
{
    public class ProjectDashboardBal : BaseBal
    {
        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<ProjectDashboardCustomModel> GetProjectDashboardList(int corporateid, int facilityid)
        {
            var list = new List<ProjectDashboardCustomModel>();
            using (var projectDashboardRep = UnitOfWork.ProjectDashboardRepository)
            {
                var lstProjectDashboard = projectDashboardRep.Where(a => a.CorporateId == corporateid && a.FacilityId == facilityid).ToList();
                if (lstProjectDashboard.Count > 0)
                {
                    list.AddRange(lstProjectDashboard.Select(item => new ProjectDashboardCustomModel
                    {

                    }));
                }
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
            using (var rep = UnitOfWork.ProjectDashboardRepository)
            {
                if (model.Id > 0)
                {
                    var current = rep.GetSingle(model.Id);
                    model.CreatedBy = current.CreatedBy;
                    model.CreatedDate = current.CreatedDate;
                    rep.UpdateEntity(model, model.Id);
                }
                else
                    rep.Create(model);

                var list = GetProjectDashboardList(Convert.ToInt32(model.CorporateId), Convert.ToInt32(model.FacilityId));
                return list;
            }
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ProjectDashboard GetProjectDashboardById(int? id)
        {
            using (var rep = UnitOfWork.ProjectDashboardRepository)
            {
                var model = rep.Where(x => x.Id == id).FirstOrDefault();
                return model;
            }
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
            var list = new List<ProjectDashboardCustomModel>();
            using (var projectDashboardRep = UnitOfWork.ProjectDashboardRepository)
            {
                list = projectDashboardRep.GetProjectDashboardData(corporateid, facilityid, year);
            }
            return list;
        }


    }
}
