using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using AutoMapper;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;


namespace BillingSystem.Bal.BusinessAccess
{
    public class RuleMasterService : IRuleMasterService
    {
        private readonly IRepository<RuleMaster> _repository;
        private readonly IRepository<Role> _rRepository;
        private readonly IRepository<GlobalCodes> _gRepository;
        private readonly IRepository<Corporate> _cRepository;
        private readonly IRepository<Facility> _fRepository;
        private readonly IRepository<RuleStep> _rsRepository;
        private readonly IRepository<ScrubReport> _srRepository;


        private readonly IMapper _mapper;
        private readonly BillingEntities _context;

        public RuleMasterService(IRepository<RuleMaster> repository, IRepository<Role> rRepository, IRepository<GlobalCodes> gRepository, IRepository<Corporate> cRepository, IRepository<Facility> fRepository, IRepository<RuleStep> rsRepository, IRepository<ScrubReport> srRepository, IMapper mapper, BillingEntities context)
        {
            _repository = repository;
            _rRepository = rRepository;
            _gRepository = gRepository;
            _cRepository = cRepository;
            _fRepository = fRepository;
            _rsRepository = rsRepository;
            _srRepository = srRepository;
            _mapper = mapper;
            _context = context;
        }

        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        //public List<RuleMasterCustomModel> GetRuleMasterList(string BillEditRuleTableNumber, bool notActive = false)
        //{
        //    var list = new List<RuleMasterCustomModel>();
        //    var spName = string.Format("EXEC {0} @pCodeTableNumber,@pIsNotActive", StoredProcedures.SPROC_GetRuleMasterByTableNumber);
        //    var sqlParameters = new SqlParameter[2];
        //    sqlParameters[0] = new SqlParameter("pCodeTableNumber", BillEditRuleTableNumber);
        //    sqlParameters[1] = new SqlParameter("pIsNotActive", notActive);
        //    var result = _context.Database.SqlQuery<RuleMasterCustomModel>(spName, sqlParameters);
        //    if (list.Count > 0)
        //        list = list.OrderBy(g => g.RuleCode1).ToList();


        //    return list;
        //}

        public List<RuleMasterCustomModel> GetRuleMasterList(string BillEditRuleTableNumber, bool isActive = true)
        {
            var list = new List<RuleMasterCustomModel>();
            var spName = string.Format("EXEC {0} @pCodeTableNumber,@pIsNotActive", StoredProcedures.SPROC_GetRuleMasterByTableNumber);
            var sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("pCodeTableNumber", BillEditRuleTableNumber);
            sqlParameters[1] = new SqlParameter("pIsNotActive", isActive);
            using (var ms = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetRuleMasterData.ToString(), isCompiled: false, parameters: sqlParameters))
            {
                var result = ms.GetResultWithJson<RuleMasterCustomModel>(JsonResultsArray.RuleMaster.ToString());
                return result;
            }
        }

        private List<RuleMasterCustomModel> MapValues(List<RuleMaster> m)
        {
            var lst = new List<RuleMasterCustomModel>();
            foreach (var model in m)
            {

                var vm = _mapper.Map<RuleMasterCustomModel>(model);
                if (vm != null)
                {
                    vm.RuleSpecifiedForString = GetNameByGlobalCodeValue(model.RuleSpecifiedFor,
                        Convert.ToString((int)GlobalCodeCategoryValue.RulesSpecifiedfor));
                    vm.RoleIdString = GetRoleNameByRoleId(Convert.ToInt32(model.RoleId));
                    vm.CorporateString = GetCorporateNameFromId(Convert.ToInt32(model.CorporateID));
                    vm.FacilityString = GetFacilityNameByFacilityId(Convert.ToInt32(model.FacilityID));
                    vm.RuleTypeString = Convert.ToInt32(model.RuleType) == 1 ? "Normal" : "Other";
                }
                lst.Add(vm);
            }
            return lst;
        }
        private RuleMasterCustomModel MapValues(RuleMaster model)
        {

            var vm = _mapper.Map<RuleMasterCustomModel>(model);
            if (vm != null)
            {
                vm.RuleSpecifiedForString = GetNameByGlobalCodeValue(model.RuleSpecifiedFor,
                    Convert.ToString((int)GlobalCodeCategoryValue.RulesSpecifiedfor));
                vm.RoleIdString = GetRoleNameByRoleId(Convert.ToInt32(model.RoleId));
                vm.CorporateString = GetCorporateNameFromId(Convert.ToInt32(model.CorporateID));
                vm.FacilityString = GetFacilityNameByFacilityId(Convert.ToInt32(model.FacilityID));
                vm.RuleTypeString = Convert.ToInt32(model.RuleType) == 1 ? "Normal" : "Other";
            }
            return vm;
        }
        private string GetFacilityNameByFacilityId(int facilityId)
        {
            var m = _fRepository.GetSingle(facilityId);
            return m != null ? m.FacilityName : string.Empty;
        }
        private string GetCorporateNameFromId(int corpId)
        {
            var corpName = "";
            var obj = _cRepository.Where(f => f.CorporateID == corpId).FirstOrDefault();
            if (obj != null) corpName = obj.CorporateName;
            return corpName;
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
        private string GetRoleNameByRoleId(int roleId)
        {
            var roleName = string.Empty;
            if (roleId > 0)
            {
                roleName = _rRepository.Where(r => r.RoleID == roleId).Select(role => role.RoleName).FirstOrDefault();
            }
            return roleName;
        }
        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="ruleMaster">The rule master.</param>
        /// <returns></returns>
        public int AddUptdateRuleMaster(RuleMaster ruleMaster, string BillEditRuleTableNumber)
        {
            if (ruleMaster.RuleMasterID > 0)
            {
                var current = _repository.GetSingle(ruleMaster.RuleMasterID);
                ruleMaster.ExtValue9 = current.ExtValue9;
                ruleMaster.CreatedBy = current.CreatedBy;
                ruleMaster.CreatedDate = current.CreatedDate;
                _repository.UpdateEntity(ruleMaster, ruleMaster.RuleMasterID);
            }
            else
            {
                ruleMaster.ExtValue9 = BillEditRuleTableNumber;
                _repository.Create(ruleMaster);
            }
            return ruleMaster.RuleMasterID;
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="ruleMasterId">The rule master identifier.</param>
        /// <returns></returns>
        public RuleMaster GetRuleMasterById(int? ruleMasterId, string BillEditRuleTableNumber)
        {
            var ruleMaster = _repository.Where(a => a.RuleMasterID == ruleMasterId && !string.IsNullOrEmpty(a.ExtValue9) && a.ExtValue9.Trim().Equals(BillEditRuleTableNumber)).FirstOrDefault();
            return ruleMaster;
        }

        /// <summary>
        /// Gets the rule master custom model by identifier.
        /// </summary>
        /// <param name="ruleMasterId">The rule master identifier.</param>
        /// <returns></returns>
        public RuleMasterCustomModel GetRuleMasterCustomModelById(int? ruleMasterId)
        {
            var ruleMaster = _repository.Where(a => a.RuleMasterID == ruleMasterId).FirstOrDefault();
            var vm = MapValues(ruleMaster);
            return vm ?? new RuleMasterCustomModel();
        }

        /// <summary>
        /// Deletes the rule master.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public int DeleteRuleMaster(RuleMaster model)
        {
            int result;
            _repository.Delete(model);
            DeleteRuleMasterFromReferenceTables(model.RuleMasterID);
            result = model.RuleMasterID;
            return result;
        }

        private void DeleteRuleMasterFromReferenceTables(int ruleMasterId)
        {
            var sList = _rsRepository.Where(s => s.RuleMasterID == ruleMasterId).ToList();
            if (sList.Count > 0)
                _rsRepository.Delete(sList);

            var sList1 = _srRepository.Where(s => s.RuleMasterID == ruleMasterId).ToList();
            if (sList1.Count > 0)
                _srRepository.Delete(sList1);
        }

        public bool DeleteMultipleRules(List<int> ids)
        {
            var list = _repository.Where(r => ids.Contains(r.RuleMasterID)).ToList();
            if (list.Count > 0)
            {
                try
                {

                    var stepList =
                        _rsRepository.Where(s => s.RuleMasterID != null && ids.Contains(s.RuleMasterID.Value)).ToList();

                    var scrubReportList = _srRepository.Where(s => s.RuleMasterID != null && ids.Contains(s.RuleMasterID.Value)).ToList();
                    if (scrubReportList.Count > 0)
                        _srRepository.Delete(scrubReportList);

                    if (stepList.Count > 0)
                        _rsRepository.Delete(stepList);

                    if (list.Count > 0)
                        _repository.Delete(list);

                }
                catch
                {
                    return false;
                }
                return true;
            }
            return true;
        }

    }
}

