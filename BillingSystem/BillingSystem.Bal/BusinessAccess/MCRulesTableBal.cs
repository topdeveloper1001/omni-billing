using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Common.Common;
using BillingSystem.Repository.UOW;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.BusinessAccess
{
    public class MCRulesTableBal : BaseBal
    {
        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<MCRulesTableCustomModel> GetMCRulesTableList()
        {
            var list = new List<MCRulesTableCustomModel>();
            using (var MCRulesTableRep = UnitOfWork.MCRulesTableRepository)
            {
                var lstMCRulesTable = MCRulesTableRep.Where(a => a.IsActive == null || (bool)a.IsActive).ToList();
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
            using (var rep = UnitOfWork.MCRulesTableRepository)
            {
                if (model.ManagedCareRuleId > 0)
                {
                    var current = rep.GetSingle(model.ManagedCareRuleId);
                    model.CreatedBy = current.CreatedBy;
                    model.CreatedDate = current.CreatedDate;
                    rep.UpdateEntity(model, model.ManagedCareRuleId);
                }
                else
                    rep.Create(model);

                var currentId = model.ManagedCareRuleId;
                var list = GetMcRulesListByRuleSetId(Convert.ToInt32(model.RuleSetNumber));
                return list;
            }
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="shared"></param>
        /// <returns></returns>
        public MCRulesTable GetMCRulesTableByID(int? MCRulesTableId)
        {
            using (var rep = UnitOfWork.MCRulesTableRepository)
            {
                var model = rep.Where(x => x.ManagedCareRuleId == MCRulesTableId).FirstOrDefault();
                return model;
            }
        }

        /// <summary>
        /// Gets the mc rules list by rule set identifier.
        /// </summary>
        /// <param name="ruleSetNumber">The rule set number.</param>
        /// <returns></returns>
        public List<MCRulesTableCustomModel> GetMcRulesListByRuleSetId(int ruleSetNumber)
        {
            var list = new List<MCRulesTableCustomModel>();
            using (var mcRulesTableRep = UnitOfWork.MCRulesTableRepository)
            {
                var lstMcRulesTable = mcRulesTableRep.Where(a => (a.IsActive == null || (bool)a.IsActive) && a.RuleSetNumber == ruleSetNumber).ToList();
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
            using (var rep = UnitOfWork.MCRulesTableRepository)
            {
                var model = rep.Where(x => x.RuleSetNumber == RuleSetNumber).ToList().LastOrDefault();
                return model != null ? Convert.ToInt32(model.RuleNumber) + 1 : 1;
            }
        }


        public bool DeleteMCRulesTable(int id, int corporateId, int facilityId)
        {
            using (var rep = UnitOfWork.MCRulesTableRepository)
            {
                rep.Delete(id);
                return true;
            }
        }
    }
}
