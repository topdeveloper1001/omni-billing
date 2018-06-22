using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Bal.Mapper;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.BusinessAccess
{
    public class ProjectTasksBal : BaseBal
    {
        private ProjectTasksMapper ProjectTasksMapper { get; set; }
        private ProjectsMapper ProjectsMapper { get; set; }

        public ProjectTasksBal()
        {
            ProjectTasksMapper = new ProjectTasksMapper();
            ProjectsMapper = new ProjectsMapper();
        }
        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<ProjectTasksCustomModel> GetProjectTasksList(int corporateid, int facilityid, string userId)
        {
            var list = new List<ProjectTasksCustomModel>();
            using (var projectTasksRep = UnitOfWork.ProjectTasksRepository)
            {
                var lstProjectTasks = projectTasksRep.Where(
                    a =>
                        (a.CorporateId == corporateid || corporateid == 0) &&
                        (a.FacilityId == facilityid || facilityid == 0) &&
                        (userId == string.Empty || a.UserResponsible.Trim().Equals(userId))).ToList();

                list.AddRange(lstProjectTasks.Select(item => ProjectTasksMapper.MapModelToViewModel(item)));
                list = list.OrderBy(o => o.ProjectTaskId).ToList();
            }
            return list;
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public List<ProjectTasksCustomModel> SaveProjectTasks(ProjectTasks model)
        {
            //Here, ExternalValue3 is taken temporarily to store the User / Owner selected in the Users dropdownlist used for filtering the Project Tasks List.
            var userSelected = model.ExternalValue3 ?? string.Empty;
            model.ExternalValue3 = string.Empty;

            using (var rep = UnitOfWork.ProjectTasksRepository)
            {
                if (model.ProjectTaskId > 0)
                {
                    var current = rep.GetSingle(model.ProjectTaskId);
                    model.CreatedBy = current.CreatedBy;
                    model.CreatedDate = current.CreatedDate;
                    var oProjectTaskTargetsBal = new ProjectTaskTargetsBal();
                    oProjectTaskTargetsBal.UpdateProjectTaskTargetTaskNumber(current.TaskNumber, model.TaskNumber,
                        Convert.ToString(model.ProjectTaskId));
                    rep.UpdateEntity(model, model.ProjectTaskId);
                }
                else
                {
                    rep.Create(model);

                    #region Add project id with start date year in project dashboard table

                    using (var pRep = UnitOfWork.ProjectsRepository)
                    {
                        var project =
                            pRep.Where(p => p.ProjectNumber.Trim().Equals(model.ProjectNumber)).FirstOrDefault();

                        if (project != null)
                        {
                            var oProjectDashboard = new ProjectDashboard
                            {
                                ProjectID = project.ProjectId,
                                TaskID = Convert.ToInt32(model.ProjectTaskId),
                                ExternalValue1 = Convert.ToString(Convert.ToDateTime(model.StartDate).Year),
                                CorporateId = model.CorporateId,
                                FacilityId = model.FacilityId,
                                CreatedBy = model.CreatedBy,
                                CreatedDate = model.CreatedDate,
                            };
                            using (var pdRep = UnitOfWork.ProjectDashboardRepository)
                                pdRep.Create(oProjectDashboard);

                        }
                    }
                    #endregion
                }
                var list = GetProjectTasksList(model.CorporateId, model.FacilityId, userSelected);
                return list;
            }
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="projectTasksId">The project tasks identifier.</param>
        /// <returns></returns>
        public ProjectTasks GetProjectTasksById(int? projectTasksId)
        {
            using (var rep = UnitOfWork.ProjectTasksRepository)
            {
                var model = rep.Where(x => x.ProjectTaskId == projectTasksId).FirstOrDefault();
                return model;
            }
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="corporateId"></param>
        /// <param name="facilityId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<ProjectTasksCustomModel> DeleteProjectTask(int id, int corporateId, int facilityId, string userId)
        {
            using (var rep = UnitOfWork.ProjectTasksRepository)
            {
                rep.Delete(id);
                var list = GetProjectTasksList(corporateId, facilityId, userId);
                return list;
            }
        }

        /// <summary>
        /// Gets the task numbers.
        /// </summary>
        /// <param name="corporateid">The corporateid.</param>
        /// <param name="facilityid">The facilityid.</param>
        /// <returns></returns>
        public List<String> GetTaskNumbers(int corporateid, int facilityid)
        {
            using (var projectTasksRep = UnitOfWork.ProjectTasksRepository)
            {
                var list = corporateid > 0
                    ? projectTasksRep.Where(
                        a => a.CorporateId == corporateid && a.FacilityId == facilityid)
                        .Select(a1 => a1.TaskNumber)
                        .ToList()
                    : projectTasksRep.GetAll().Select(a1 => a1.TaskNumber).ToList();
                return list;
            }
        }

        /// <summary>
        /// Gets the projects for execute kpi dashboard.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <returns></returns>
        public List<ProjectsCustomModel> GetProjectsForExecKpiDashboard(int corporateId, int facilityId)
        {
            var list = new List<ProjectsCustomModel>();
            using (var rep = UnitOfWork.ProjectsRepository)
            {
                var projects = rep.Where(p => (p.FacilityId == facilityId || facilityId == 0) && p.CorporateId == corporateId && p.IsActive).ToList();
                if (projects.Any())
                {
                    using (var tRep = UnitOfWork.ProjectTasksRepository)
                    {
                        foreach (var pItem in projects)
                        {
                            var taskList = new List<ProjectTasksCustomModel>();

                            //ExternalValue1 is Milestone. 1 means true for Milestone
                            var tasks = tRep.Where(p => p.ProjectNumber == pItem.ProjectNumber && p.ExternalValue1.ToLower().Trim().Equals("1") && p.IsActive).ToList();
                            var projectsVm = ProjectsMapper.MapModelToViewModel(pItem);

                            if (tasks.Any())
                            {
                                taskList.AddRange(tasks.Select(item => ProjectTasksMapper.MapModelToViewModel(item)));
                                projectsVm.Milestones = taskList;
                            }

                            list.Add(projectsVm);
                        }
                    }
                }
            }
            return list;
        }

        public bool CheckDuplicateTaskNumber(string projectNumber, string taskNumber, int projectTaskId)
        {
            using (var rep = UnitOfWork.ProjectTasksRepository)
            {
                var isExists =
                    rep.Where(
                        t =>
                            t.ProjectNumber.Trim().Equals(projectNumber) &&
                            (projectTaskId == 0 || t.ProjectTaskId != projectTaskId) &&
                            t.TaskNumber.Trim().Equals(taskNumber)).Any();

                return isExists;
            }
        }


        public List<ProjectsCustomModel> GetProjectsDashboardData(int corporateId, int facilityId, string responsibleUserId)
        {
            var list = new List<ProjectsCustomModel>();
            using (var rep = UnitOfWork.ProjectsRepository)
            {
                var projects = rep.Where(p => (p.FacilityId == facilityId || facilityId == 0) && p.CorporateId == corporateId && (p.UserResponsible.Trim().Equals(responsibleUserId) || string.IsNullOrEmpty(responsibleUserId))).ToList();
                if (projects.Any())
                {
                    using (var tRep = UnitOfWork.ProjectTasksRepository)
                    {
                        foreach (var pItem in projects)
                        {
                            var taskList = new List<ProjectTasksCustomModel>();

                            var tasks = tRep.Where(p => p.ProjectNumber == pItem.ProjectNumber).ToList();
                            var projectsVm = ProjectsMapper.MapModelToViewModel(pItem);

                            if (tasks.Any())
                            {
                                taskList.AddRange(tasks.Select(item => ProjectTasksMapper.MapModelToViewModel(item)));
                                projectsVm.Milestones = taskList;
                            }

                            list.Add(projectsVm);
                        }
                    }
                }
            }
            return list;
        }


        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="projectTasksId">The project tasks identifier.</param>
        /// <returns></returns>
        public string GetProjectTaskCommentById(int projectTasksId)
        {
            using (var rep = UnitOfWork.ProjectTasksRepository)
            {
                var comment = rep.Where(x => x.ProjectTaskId == projectTasksId).Select(p => p.Comments).FirstOrDefault();
                return comment ?? string.Empty;
            }
        }
    }
}
