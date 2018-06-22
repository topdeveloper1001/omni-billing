using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Bal.Mapper;
using BillingSystem.Repository.UOW;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.BusinessAccess
{
    public class ProjectsBal : BaseBal
    {
        private ProjectsMapper ProjectsMapper { get; set; }

        public ProjectsBal()
        {
            ProjectsMapper = new ProjectsMapper();
        }
        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<ProjectsCustomModel> GetProjectsList(int corporateid, int facilityid)
        {
            var list = new List<ProjectsCustomModel>();
            using (var ProjectsRep = UnitOfWork.ProjectsRepository)
            {
                var lstProjects = ProjectsRep.Where(a => a.CorporateId == corporateid && a.FacilityId == facilityid).ToList();
                list.AddRange(lstProjects.Select(item => ProjectsMapper.MapModelToViewModel(item)));
            }
            return list;
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public List<ProjectsCustomModel> SaveProjects(Projects model)
        {
            using (var rep = UnitOfWork.ProjectsRepository)
            {
                if (model.ProjectId > 0)
                {
                    var current = rep.GetSingle(model.ProjectId);
                    model.CreatedBy = current.CreatedBy;
                    model.CreatedDate = current.CreatedDate;

                    //If the project number is changed, then make the changed in the ProjectTasks table as well. 
                    if (!current.ProjectNumber.ToLower().Trim().Equals(model.ProjectNumber.ToLower().Trim()))
                    {
                        using (var tRep = UnitOfWork.ProjectTasksRepository)
                        {
                            var tasks = tRep.Where(t => t.ProjectNumber.Trim().Equals(current.ProjectNumber)).ToList();
                            if (tasks.Any())
                            {
                                foreach (var tt in tasks)
                                {
                                    tt.ProjectNumber = model.ProjectNumber;
                                    tRep.UpdateEntity(tt, tt.ProjectTaskId);
                                }
                            }
                        }
                    }
                    rep.UpdateEntity(model, model.ProjectId);
                }
                else
                {
                    rep.Create(model);
                    #region Add project id with start date year in project dashboard table
                    var pId = model.ProjectId;
                    var startDateYear = Convert.ToDateTime(model.StartDate).Year;
                    var oProjectDashboard = new ProjectDashboard
                    {
                        ProjectID = pId,
                        ExternalValue1 = Convert.ToString(startDateYear),
                        CorporateId = model.CorporateId,
                        FacilityId = model.FacilityId
                    };
                    UnitOfWork.ProjectDashboardRepository.Create(oProjectDashboard);
                    #endregion
                }
                var list = GetProjectsList(model.CorporateId, model.FacilityId);
                return list;
            }
        }
        /// <summary>
        /// Method to Delete the Entity in the database.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public List<ProjectsCustomModel> DeleteProjects(int id, int corporateid, int facilityid)
        {
            using (var rep = UnitOfWork.ProjectsRepository)
            {
                if (id > 0)
                {
                    rep.Delete(id);
                }

                var list = GetProjectsList(corporateid, facilityid);
                return list;
            }
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public Projects GetProjectsById(int? id)
        {
            using (var rep = UnitOfWork.ProjectsRepository)
            {
                var model = rep.Where(x => x.ProjectId == id).FirstOrDefault();
                return model;
            }
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="corporateId"></param>
        /// <param name="facilityId"></param>
        /// <returns></returns>
        public List<Projects> GetProjectNumbers(int corporateId, int facilityId)
        {
            using (var rep = UnitOfWork.ProjectsRepository)
            {
                var model = corporateId > 0
                    ? rep.Where(c => c.CorporateId == corporateId && c.FacilityId == facilityId && c.IsActive).ToList()
                    : rep.Where(c => c.IsActive).ToList();
                return model.ToList();
            }
        }

        public Projects GetProjectDetailsByNumber(string projectNumber)
        {
            projectNumber = projectNumber.ToLower().Trim();
            using (var rep = UnitOfWork.ProjectsRepository)
            {
                var current = rep.Where(p => p.ProjectNumber.ToLower().Trim().Equals(projectNumber)).FirstOrDefault();
                return current;
            }
        }

        public bool CheckDuplicateProjectNumber(string projectNumber, int projectId)
        {
            using (var rep = UnitOfWork.ProjectsRepository)
            {
                var isExists =
                    rep.Where(
                        p =>
                            p.ProjectNumber.Trim().Equals(projectNumber) && (projectId == 0 || p.ProjectId != projectId))
                        .Any();
                return isExists;
            }
        }

    }
}
