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
    public class ProjectTaskTargetsService : IProjectTaskTargetsService
    {

        private readonly IRepository<ProjectTaskTargets> _repository;
        private readonly IRepository<GlobalCodes> _gRepository;

        private readonly IMapper _mapper;
        private readonly BillingEntities _context;

        public ProjectTaskTargetsService(IRepository<ProjectTaskTargets> repository, IRepository<GlobalCodes> gRepository, IMapper mapper, BillingEntities context)
        {
            _repository = repository;
            _gRepository = gRepository;
            _mapper = mapper;
            _context = context;
        }

        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<ProjectTaskTargetsCustomModel> GetProjectTaskTargetsList(int corporateid, int facilityid)
        {
            var lstProjectTaskTargets = corporateid > 0
                ? _repository.Where(
                    a => a.CorporateId == corporateid && a.FacilityId == facilityid).ToList()
                : _repository.GetAll().ToList();
            var list = MapValues(lstProjectTaskTargets);
            return list;
        }
        private List<ProjectTaskTargetsCustomModel> MapValues(List<ProjectTaskTargets> m)
        {
            var lst = new List<ProjectTaskTargetsCustomModel>();
            foreach (var model in m)
            {

                var vm = _mapper.Map<ProjectTaskTargetsCustomModel>(model);
                if (vm != null)
                {
                    vm.TargetPercentageValueStr = GetNameByGlobalCodeValue(Convert.ToString(model.TargetedCompletionValue),
                   Convert.ToString((int)GlobalCodeCategoryValue.ProjectsPercentageValue));
                }

            }

            return lst;
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
        public List<ProjectTaskTargetsCustomModel> SaveProjectTaskTargets(ProjectTaskTargets model)
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

            var list = GetProjectTaskTargetsList(model.CorporateId, model.FacilityId);
            return list;
        }

        public List<ProjectTaskTargetsCustomModel> DeleteProjectTaskTargets(int id, int corporateId, int facilityId)
        {
            _repository.Delete(id);
            var list = GetProjectTaskTargetsList(corporateId, facilityId);
            return list;
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="id">The project task targets identifier.</param>
        /// <returns></returns>
        public ProjectTaskTargets GetProjectTaskTargetsById(int? id)
        {
            var model = _repository.Where(x => x.Id == id).FirstOrDefault();
            return model;
        }
        /// <summary>
        /// Method will update the task number column of Project Task Target table
        /// </summary>
        /// <param name="oldTaskNumber"></param>
        /// <param name="newTaskNumber"></param>
        /// <param name="projectTaskId"></param>
        /// <returns></returns>
        public bool UpdateProjectTaskTargetTaskNumber(string oldTaskNumber, string newTaskNumber, string projectTaskId)
        {
            var sqlParameters = new SqlParameter[3];
            sqlParameters[0] = new SqlParameter("OldTaskNumber", oldTaskNumber);
            sqlParameters[1] = new SqlParameter("NewTaskNumber", newTaskNumber);
            sqlParameters[2] = new SqlParameter("ProjectTaskId", projectTaskId);
            _repository.ExecuteCommand(StoredProcedures.SPROC_UpdateProjectTaskTargetTaskNumber.ToString(), sqlParameters);
            return true;
        }
    }
}
