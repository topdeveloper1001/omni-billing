using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using AutoMapper;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Repository.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class ProjectTasksService : IProjectTasksService
    {
        private readonly IRepository<ProjectTaskTargets> _pttRepository;
        private readonly IRepository<ProjectTasks> _repository;
        private readonly IRepository<GlobalCodes> _gRepository;
        private readonly IRepository<Facility> _fRepository;
        private readonly IRepository<Users> _uRepository;
        private readonly IRepository<Projects> _pRepository;
        private readonly IRepository<ProjectDashboard> _pdRepository;

        private readonly IMapper _mapper;
        private readonly BillingEntities _context;

        public ProjectTasksService(IRepository<ProjectTasks> repository, IRepository<GlobalCodes> gRepository, IRepository<Facility> fRepository, IRepository<Users> uRepository, IRepository<Projects> pRepository, IRepository<ProjectDashboard> pdRepository, IMapper mapper, BillingEntities context)
        {
            _repository = repository;
            _gRepository = gRepository;
            _fRepository = fRepository;
            _uRepository = uRepository;
            _pRepository = pRepository;
            _pdRepository = pdRepository;
            _mapper = mapper;
            _context = context;
        }

        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<ProjectTasksCustomModel> GetProjectTasksList(int corporateid, int facilityid, string userId)
        {
            var list = new List<ProjectTasksCustomModel>();
            var lstProjectTasks = _repository.Where(
                a =>
                    (a.CorporateId == corporateid || corporateid == 0) &&
                    (a.FacilityId == facilityid || facilityid == 0) &&
                    (userId == string.Empty || a.UserResponsible.Trim().Equals(userId))).ToList();

            list = MapValues(lstProjectTasks);
            list = list.OrderBy(o => o.ProjectTaskId).ToList();
            return list;
        }
        private Projects GetProjectDetailsByNumber(string projectNumber)
        {
            projectNumber = projectNumber.ToLower().Trim();
            var current = _pRepository.Where(p => p.ProjectNumber.ToLower().Trim().Equals(projectNumber)).FirstOrDefault();
            return current;
        }

        private List<ProjectTasksCustomModel> MapValues(List<ProjectTasks> m)
        {
            var lst = new List<ProjectTasksCustomModel>();
            foreach (var model in m)
            {
                var vm = _mapper.Map<ProjectTasksCustomModel>(model);
                if (vm != null)
                {
                    vm.StatusColor = !string.IsNullOrEmpty(vm.ExternalValue2)
                        ? GetNameByGlobalCodeValue(vm.ExternalValue2,
                            Convert.ToString((int)GlobalCodeCategoryValue.ExternalDashboardColors))
                        : string.Empty;
                    var pr = GetProjectDetailsByNumber(vm.ProjectNumber);
                    if (pr != null)
                    {
                        vm.ProjectName = pr.Name;
                        vm.KpiTargetDate = pr.EstCompletionDate.HasValue ? pr.EstCompletionDate.Value.ToString("dd/MM/yyyy") : string.Empty;
                        if (pr.ExternalValue2 != null)
                            vm.ProjectTypeId = int.Parse(pr.ExternalValue2);
                        int ur;
                        if (int.TryParse(vm.UserResponsible, out ur))
                            vm.Responsible = GetNameByUserId(ur);
                    }
                    vm.FacilityName = GetFacilityNameByFacilityId(Convert.ToInt32(vm.FacilityId));

                    if (!string.IsNullOrEmpty(vm.ExternalValue2))
                    {
                        var colors = (ExternalDashboardColor)Enum.Parse(typeof(ExternalDashboardColor), vm.ExternalValue2);
                        switch (colors)
                        {
                            case ExternalDashboardColor.Green:
                                vm.ColorImage = "/images/circleGreen19x19.png";
                                break;
                            case ExternalDashboardColor.Yellow:
                                vm.ColorImage = "~/images/circleYellow19x19.png";
                                break;
                            case ExternalDashboardColor.Red:
                                vm.ColorImage = "~/images/circleRed19x19.png";
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }
                lst.Add(vm);
            }
            return lst;
        }
        private ProjectsCustomModel MapProjectValues(Projects model)
        {

            var vm = _mapper.Map<ProjectsCustomModel>(model);
            if (vm != null)
            {
                vm.ProjectType = !string.IsNullOrEmpty(vm.ExternalValue2) ? GetNameByGlobalCodeValue(vm.ExternalValue2,
                    Convert.ToString((int)GlobalCodeCategoryValue.DashboardProjectsType)) : string.Empty;
                vm.ProjectStatus = !string.IsNullOrEmpty(vm.ExternalValue3) ? GetNameByGlobalCodeValue(vm.ExternalValue3,
                   Convert.ToString((int)GlobalCodeCategoryValue.ExternalDashboardColors)) : string.Empty;

                vm.FacilityName = GetFacilityNameByFacilityId(Convert.ToInt32(vm.FacilityId));

                int ur;
                if (int.TryParse(vm.UserResponsible, out ur))
                    vm.Responsible = GetNameByUserId(ur);
            }
            return vm;
        }
        private string GetNameByUserId(int? UserID)
        {
            var usersModel = _uRepository.Where(x => x.UserID == UserID && x.IsDeleted == false).FirstOrDefault();
            return usersModel != null ? usersModel.FirstName + " " + usersModel.LastName : string.Empty;
        }
        private string GetFacilityNameByFacilityId(int facilityId)
        {
            var m = _fRepository.GetSingle(facilityId);
            return m != null ? m.FacilityName : string.Empty;
        }
        private string GetNameByGlobalCodeValue(string codeValue, string categoryValue, string fId = "")
        {
            if (!string.IsNullOrEmpty(codeValue))
            {
                var gl = _gRepository.Where(g => g.GlobalCodeValue.Equals(codeValue) && !g.IsDeleted.Value && g.GlobalCodeCategoryValue.Equals(categoryValue) && (string.IsNullOrEmpty(fId) || g.FacilityNumber.Equals(fId))).FirstOrDefault();
                return gl != null ? gl.GlobalCodeName : string.Empty;
            }
            return string.Empty;
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

            if (model.ProjectTaskId > 0)
            {
                var current = _repository.GetSingle(model.ProjectTaskId);
                model.CreatedBy = current.CreatedBy;
                model.CreatedDate = current.CreatedDate;
                UpdateProjectTaskTargetTaskNumber(current.TaskNumber, model.TaskNumber,
                                    Convert.ToString(model.ProjectTaskId));
                _repository.UpdateEntity(model, model.ProjectTaskId);
            }
            else
            {
                _repository.Create(model);

                #region Add project id with start date year in project dashboard table

                var project = _pRepository.Where(p => p.ProjectNumber.Trim().Equals(model.ProjectNumber)).FirstOrDefault();

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
                    _pdRepository.Create(oProjectDashboard);

                }
                #endregion
            }
            var list = GetProjectTasksList(model.CorporateId, model.FacilityId, userSelected);
            return list;
        }
        private bool UpdateProjectTaskTargetTaskNumber(string oldTaskNumber, string newTaskNumber, string projectTaskId)
        {
            var sqlParameters = new SqlParameter[3];
            sqlParameters[0] = new SqlParameter("OldTaskNumber", oldTaskNumber);
            sqlParameters[1] = new SqlParameter("NewTaskNumber", newTaskNumber);
            sqlParameters[2] = new SqlParameter("ProjectTaskId", projectTaskId);
            _pttRepository.ExecuteCommand(StoredProcedures.SPROC_UpdateProjectTaskTargetTaskNumber.ToString(), sqlParameters);
            return true;
        }
        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="projectTasksId">The project tasks identifier.</param>
        /// <returns></returns>
        public ProjectTasks GetProjectTasksById(int? projectTasksId)
        {
            var model = _repository.Where(x => x.ProjectTaskId == projectTasksId).FirstOrDefault();
            return model;
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
            _repository.Delete(id);
            var list = GetProjectTasksList(corporateId, facilityId, userId);
            return list;
        }

        /// <summary>
        /// Gets the task numbers.
        /// </summary>
        /// <param name="corporateid">The corporateid.</param>
        /// <param name="facilityid">The facilityid.</param>
        /// <returns></returns>
        public List<String> GetTaskNumbers(int corporateid, int facilityid)
        {
            var list = corporateid > 0
                ? _repository.Where(
                    a => a.CorporateId == corporateid && a.FacilityId == facilityid)
                    .Select(a1 => a1.TaskNumber)
                    .ToList()
                : _repository.GetAll().Select(a1 => a1.TaskNumber).ToList();
            return list;
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
            var projects = _pRepository.Where(p => (p.FacilityId == facilityId || facilityId == 0) && p.CorporateId == corporateId && p.IsActive).ToList();
            if (projects.Any())
            {
                foreach (var pItem in projects)
                {
                    var taskList = new List<ProjectTasksCustomModel>();

                    //ExternalValue1 is Milestone. 1 means true for Milestone
                    var tasks = _repository.Where(p => p.ProjectNumber == pItem.ProjectNumber && p.ExternalValue1.ToLower().Trim().Equals("1") && p.IsActive).ToList();
                    var projectsVm = MapProjectValues(pItem);

                    if (tasks.Any())
                    {
                        taskList = MapValues(tasks);
                        projectsVm.Milestones = taskList;
                    }

                    list.Add(projectsVm);
                }
            }
            return list;
        }

        public bool CheckDuplicateTaskNumber(string projectNumber, string taskNumber, int projectTaskId)
        {
            var isExists =
                _repository.Where(
                    t =>
                        t.ProjectNumber.Trim().Equals(projectNumber) &&
                        (projectTaskId == 0 || t.ProjectTaskId != projectTaskId) &&
                        t.TaskNumber.Trim().Equals(taskNumber)).Any();

            return isExists;
        }


        public List<ProjectsCustomModel> GetProjectsDashboardData(int corporateId, int facilityId, string responsibleUserId)
        {
            var list = new List<ProjectsCustomModel>();
            var projects = _pRepository.Where(p => (p.FacilityId == facilityId || facilityId == 0) && p.CorporateId == corporateId && (p.UserResponsible.Trim().Equals(responsibleUserId) || string.IsNullOrEmpty(responsibleUserId))).ToList();
            if (projects.Any())
            {
                foreach (var pItem in projects)
                {
                    var taskList = new List<ProjectTasksCustomModel>();

                    var tasks = _repository.Where(p => p.ProjectNumber == pItem.ProjectNumber).ToList();
                    var projectsVm = MapProjectValues(pItem);

                    if (tasks.Any())
                    {
                        taskList = MapValues(tasks);
                        projectsVm.Milestones = taskList;
                    }

                    list.Add(projectsVm);
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
            var comment = _repository.Where(x => x.ProjectTaskId == projectTasksId).Select(p => p.Comments).FirstOrDefault();
            return comment ?? string.Empty;
        }
    }
}
