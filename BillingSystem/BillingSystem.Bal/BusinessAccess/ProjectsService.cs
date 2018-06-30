using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Repository.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class ProjectsService : IProjectsService
    {
        private readonly IRepository<Projects> _repository;
        private readonly IRepository<ProjectDashboard> _pRepository;
        private readonly IRepository<ProjectTasks> _ptRepository;
        private readonly IRepository<Facility> _fRepository;
        private readonly IRepository<Users> _uRepository;
        private readonly IRepository<GlobalCodes> _gRepository;
        private readonly IMapper _mapper;

        public ProjectsService(IRepository<Projects> repository, IRepository<ProjectDashboard> pRepository, IRepository<ProjectTasks> ptRepository, IRepository<Facility> fRepository, IRepository<Users> uRepository, IRepository<GlobalCodes> gRepository, IMapper mapper)
        {
            _repository = repository;
            _pRepository = pRepository;
            _ptRepository = ptRepository;
            _fRepository = fRepository;
            _uRepository = uRepository;
            _gRepository = gRepository;
            _mapper = mapper;
        }


        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<ProjectsCustomModel> GetProjectsList(int corporateid, int facilityid)
        {
            var list = new List<ProjectsCustomModel>();
            var lstProjects = _repository.Where(a => a.CorporateId == corporateid && a.FacilityId == facilityid).ToList();
            list = MapValues(lstProjects);
            return list;
        }
        private List<ProjectsCustomModel> MapValues(List<Projects> m)
        {
            var lst = new List<ProjectsCustomModel>();
            foreach (var model in m)
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
                lst.Add(vm);
            }
            return lst;
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
        public List<ProjectsCustomModel> SaveProjects(Projects model)
        {
            if (model.ProjectId > 0)
            {
                var current = _repository.GetSingle(model.ProjectId);
                model.CreatedBy = current.CreatedBy;
                model.CreatedDate = current.CreatedDate;

                //If the project number is changed, then make the changed in the ProjectTasks table as well. 
                if (!current.ProjectNumber.ToLower().Trim().Equals(model.ProjectNumber.ToLower().Trim()))
                {
                    var tasks = _ptRepository.Where(t => t.ProjectNumber.Trim().Equals(current.ProjectNumber)).ToList();
                    if (tasks.Any())
                    {
                        foreach (var tt in tasks)
                        {
                            tt.ProjectNumber = model.ProjectNumber;
                            _ptRepository.UpdateEntity(tt, tt.ProjectTaskId);
                        }
                    }
                }
                _repository.UpdateEntity(model, model.ProjectId);
            }
            else
            {
                _repository.Create(model);
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
                _pRepository.Create(oProjectDashboard);
                #endregion
            }
            var list = GetProjectsList(model.CorporateId, model.FacilityId);
            return list;
        }
        /// <summary>
        /// Method to Delete the Entity in the database.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public List<ProjectsCustomModel> DeleteProjects(int id, int corporateid, int facilityid)
        {
            if (id > 0)
            {
                _repository.Delete(id);
            }

            var list = GetProjectsList(corporateid, facilityid);
            return list;
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public Projects GetProjectsById(int? id)
        {
            var model = _repository.Where(x => x.ProjectId == id).FirstOrDefault();
            return model;
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="corporateId"></param>
        /// <param name="facilityId"></param>
        /// <returns></returns>
        public List<Projects> GetProjectNumbers(int corporateId, int facilityId)
        {
            var model = corporateId > 0
                ? _repository.Where(c => c.CorporateId == corporateId && c.FacilityId == facilityId && c.IsActive).ToList()
                : _repository.Where(c => c.IsActive).ToList();
            return model.ToList();
        }

        public Projects GetProjectDetailsByNumber(string projectNumber)
        {
            projectNumber = projectNumber.ToLower().Trim();
            var current = _repository.Where(p => p.ProjectNumber.ToLower().Trim().Equals(projectNumber)).FirstOrDefault();
            return current;
        }

        public bool CheckDuplicateProjectNumber(string projectNumber, int projectId)
        {
            var isExists =
                _repository.Where(
                    p =>
                        p.ProjectNumber.Trim().Equals(projectNumber) && (projectId == 0 || p.ProjectId != projectId))
                    .Any();
            return isExists;
        }

    }
}
