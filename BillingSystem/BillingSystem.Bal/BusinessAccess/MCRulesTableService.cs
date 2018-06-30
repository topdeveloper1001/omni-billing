using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Repository.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class MCRulesTableService : IMCRulesTableService
    {
        private readonly IRepository<MCRulesTable> _repository;
        private readonly IRepository<GlobalCodes> _gRepository;

        public MCRulesTableService(IRepository<MCRulesTable> repository, IRepository<GlobalCodes> gRepository)
        {
            _repository = repository;
            _gRepository = gRepository;
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
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<MCRulesTableCustomModel> GetMCRulesTableList()
        {
            var list = new List<MCRulesTableCustomModel>();
            var lstMCRulesTable = _repository.Where(a => a.IsActive == null || (bool)a.IsActive).ToList();
            if (lstMCRulesTable.Count > 0)
            {
                list.AddRange(lstMCRulesTable.Select(item => new MCRulesTableCustomModel
                {
                    ManagedCareRuleId = item.ManagedCareRuleId,
                    RuleSetNumber = item.RuleSetNumber,
                    RuleNumber = item.RuleNumber,
                    AppliestoPatientType = item.AppliestoPatientType,
                    ConStart = item.ConStart,
                    Calculation = item.Calculation,
                    ConNextLine = item.ConNextLine,
                    LHSTableName = item.LHSTableName,
                    LHSTableColumn = item.LHSTableColumn,
                    LHSTableKeyColumn = item.LHSTableKeyColumn,
                    DirectValue = item.DirectValue,
                    CalculationMethod = item.CalculationMethod,
                    CalcualtionRate = item.CalcualtionRate,
                    ModifiedBy = item.ModifiedBy,
                    ModifiedDate = item.ModifiedDate,
                    IsActive = item.IsActive,
                    CreatedBy = item.CreatedBy,
                    CreatedDate = item.CreatedDate,
                    ConGroup = item.ConGroup,
                    RHSTableName = item.RHSTableName,
                    RHSTableColumn = item.RHSTableColumn,
                    RHSTableKeyColumn = item.RHSTableKeyColumn,
                    CalculationTypeStr = GetNameByGlobalCodeValue(item.Calculation.ToString(), Convert.ToInt32(GlobalCodeCategoryValue.Calculationtype).ToString()),
                    ConNextLineStr = GetNameByGlobalCodeValue(item.ConNextLine.ToString(), Convert.ToInt32(GlobalCodeCategoryValue.Calculationtype).ToString()),
                    ConStartStr = "ELSE",//GetNameByGlobalCodeValue(item.ConStart.ToString(), Convert.ToInt32(GlobalCodeCategoryValue.ConditionStart).ToString()),
                                         //ConpareTypeStr = GetNameByGlobalCodeValue(item.FieldName.ToString(), Convert.ToInt32(GlobalCodeCategoryValue.DataComparer).ToString()),
                    PatientTypeStr = GetNameByGlobalCodeValue(item.AppliestoPatientType.ToString(), Convert.ToInt32(GlobalCodeCategoryValue.EncounterpatientType).ToString()),
                    CompareTableString = item.LHSTableName + " " + item.LHSTableColumn,
                    ComparorTableString = string.IsNullOrEmpty(item.DirectValue) ? item.RHSTableName + " " + item.RHSTableColumn : item.DirectValue,
                }));
            }
            return list;
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="MCRulesTable"></param>
        /// <returns></returns>
        public List<MCRulesTableCustomModel> SaveMCRulesTable(MCRulesTable model)
        {
            if (model.ManagedCareRuleId > 0)
            {
                var current = _repository.GetSingle(model.ManagedCareRuleId);
                model.CreatedBy = current.CreatedBy;
                model.CreatedDate = current.CreatedDate;
                _repository.UpdateEntity(model, model.ManagedCareRuleId);
            }
            else
                _repository.Create(model);

            var currentId = model.ManagedCareRuleId;
            var list = GetMcRulesListByRuleSetId(Convert.ToInt32(model.RuleSetNumber));
            return list;
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="shared"></param>
        /// <returns></returns>
        public MCRulesTable GetMCRulesTableByID(int? MCRulesTableId)
        {
            var model = _repository.Where(x => x.ManagedCareRuleId == MCRulesTableId).FirstOrDefault();
            return model;
        }

        /// <summary>
        /// Gets the mc rules list by rule set identifier.
        /// </summary>
        /// <param name="ruleSetNumber">The rule set number.</param>
        /// <returns></returns>
        public List<MCRulesTableCustomModel> GetMcRulesListByRuleSetId(int ruleSetNumber)
        {
            var list = new List<MCRulesTableCustomModel>();
            var lstMcRulesTable = _repository.Where(a => (a.IsActive == null || (bool)a.IsActive) && a.RuleSetNumber == ruleSetNumber).ToList();
            if (lstMcRulesTable.Count > 0)
            {
                list.AddRange(lstMcRulesTable.Select(item => new MCRulesTableCustomModel
                {
                    ManagedCareRuleId = item.ManagedCareRuleId,
                    RuleSetNumber = item.RuleSetNumber,
                    RuleNumber = item.RuleNumber,
                    AppliestoPatientType = item.AppliestoPatientType,
                    ConStart = item.ConStart,
                    Calculation = item.Calculation,
                    ConNextLine = item.ConNextLine,
                    LHSTableName = item.LHSTableName,
                    LHSTableColumn = item.LHSTableColumn,
                    LHSTableKeyColumn = item.LHSTableKeyColumn,
                    DirectValue = item.DirectValue,
                    CalculationMethod = item.CalculationMethod,
                    CalcualtionRate = item.CalcualtionRate,
                    ModifiedBy = item.ModifiedBy,
                    ModifiedDate = item.ModifiedDate,
                    IsActive = item.IsActive,
                    CreatedBy = item.CreatedBy,
                    CreatedDate = item.CreatedDate,
                    ConGroup = item.ConGroup,
                    RHSTableName = item.RHSTableName,
                    RHSTableColumn = item.RHSTableColumn,
                    RHSTableKeyColumn = item.RHSTableKeyColumn,
                    CalculationTypeStr = GetNameByGlobalCodeValue(item.Calculation.ToString(), Convert.ToInt32(GlobalCodeCategoryValue.Calculationtype).ToString()),
                    ConNextLineStr = GetNameByGlobalCodeValue(item.ConNextLine.ToString(), Convert.ToInt32(GlobalCodeCategoryValue.Calculationtype).ToString()),
                    ConStartStr = "ELSE",//GetNameByGlobalCodeValue(item.ConStart.ToString(), Convert.ToInt32(GlobalCodeCategoryValue.ConditionStart).ToString()),
                                         //ConpareTypeStr = GetNameByGlobalCodeValue(item.FieldName.ToString(), Convert.ToInt32(GlobalCodeCategoryValue.DataComparer).ToString()),
                    PatientTypeStr = GetNameByGlobalCodeValue(item.AppliestoPatientType.ToString(), Convert.ToInt32(GlobalCodeCategoryValue.EncounterpatientType).ToString()),
                    CompareTableString = item.LHSTableName + " - " + item.LHSTableColumn,
                    ComparorTableString = string.IsNullOrEmpty(item.DirectValue) ? item.RHSTableName + " - " + item.RHSTableColumn : item.DirectValue,
                }));
            }
            return list;
        }

        /// <summary>
        /// Gets the maximum rule step number.
        /// </summary>
        /// <param name="RuleSetNumber">The rule set number.</param>
        /// <returns></returns>
        public int GetMaxRuleStepNumber(int RuleSetNumber)
        {
            var model = _repository.Where(x => x.RuleSetNumber == RuleSetNumber).ToList().LastOrDefault();
            return model != null ? Convert.ToInt32(model.RuleNumber) + 1 : 1;
        }


        public bool DeleteMCRulesTable(int id, int corporateId, int facilityId)
        {
            _repository.Delete(id);
            return true;
        }
    }
}
