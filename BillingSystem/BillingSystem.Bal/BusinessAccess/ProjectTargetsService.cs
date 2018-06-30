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
    public class ProjectTargetsService : IProjectTargetsService
    {
        private readonly IRepository<ProjectTargets> _repository;
        private readonly IRepository<GlobalCodes> _gRepository;
        private readonly IMapper _mapper;
        private readonly BillingEntities _context;

        public ProjectTargetsService(IRepository<ProjectTargets> repository, IRepository<GlobalCodes> gRepository, IMapper mapper, BillingEntities context)
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
        public List<ProjectTargetsCustomModel> GetProjectTargetsList(int corporateid, int facilityid)
        {
            var list = new List<ProjectTargetsCustomModel>();
            var lstProjectTargets = corporateid > 0
                ? _repository.Where(
                    a => a.CorporateId == corporateid && a.FacilityId == facilityid).ToList()
                : _repository.GetAll().ToList();
            list = MapValues(lstProjectTargets);
            return list;
        }
        private List<ProjectTargetsCustomModel> MapValues(List<ProjectTargets> m)
        {
            var lst = new List<ProjectTargetsCustomModel>();
            foreach (var model in m)
            {
                var vm = _mapper.Map<ProjectTargetsCustomModel>(model);
                if (vm != null)
                {
                    vm.TargetPercentageValueStr = GetNameByGlobalCodeValue(Convert.ToString(model.TargetedCompletionValue),
                   Convert.ToString((int)GlobalCodeCategoryValue.ProjectsPercentageValue));
                }
                lst.Add(vm);
            }
            return lst;
        }
        private string GetNameByGlobalCodeValue(string codeValue, string categoryValue, string fId = "")
        {
            if (!string.IsNullOrEmpty(codeValue))
            {
                var gl = _gRepository.Where(
                        g => g.GlobalCodeValue.Equals(codeValue) && !g.IsDeleted.Value && g.GlobalCodeCategoryValue.Equals(categoryValue) && (string.IsNullOrEmpty(fId) || g.FacilityNumber.Equals(fId)))
                        .FirstOrDefault();
                return gl != null ? gl.GlobalCodeName : string.Empty;
            }
            return string.Empty;
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public List<ProjectTargetsCustomModel> SaveProjectTargets(ProjectTargets model)
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

            var list = GetProjectTargetsList(model.CorporateId, model.FacilityId);
            return list;
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ProjectTargets GetProjectTargetsById(int id)
        {
            var model = _repository.Where(x => x.Id == id).FirstOrDefault();
            return model;
        }


        public List<ProjectTargetsCustomModel> DeleteProjectTarget(int id, int corporateId, int facilityId)
        {
            _repository.Delete(id);
            var list = GetProjectTargetsList(corporateId, facilityId);
            return list;
        }

        public Int32 SaveMonthWiseValuesInProjectDashboard(string ProjectNumber, string Month, string CorporateId,
            string FacilityId, string TaskNumber)
        {
            Int32 retValue = 0;
            var spName = string.Format("EXEC {0} @ProjectNumber,@Month,@CorporateId,@FacilityId,@TaskNumber", StoredProcedures.SPROC_AddMonthWisevaluesInProjectDashboard.ToString());
            var sqlParameters = new SqlParameter[5];
            sqlParameters[0] = new SqlParameter("ProjectNumber ", ProjectNumber);
            sqlParameters[1] = new SqlParameter("Month ", Month);
            sqlParameters[2] = new SqlParameter("CorporateId ", CorporateId);
            sqlParameters[3] = new SqlParameter("FacilityId ", FacilityId);
            sqlParameters[4] = new SqlParameter("TaskNumber ", TaskNumber);
            retValue = _context.Database.SqlQuery<Int32>(spName, sqlParameters).FirstOrDefault();
            return retValue;
        }
    }
}
