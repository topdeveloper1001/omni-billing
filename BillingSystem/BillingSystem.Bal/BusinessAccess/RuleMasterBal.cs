using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using BillingSystem.Bal.Mapper;
using BillingSystem.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.BusinessAccess
{
    public class RuleMasterBal : BaseBal
    {
        private RuleMasterMapper RuleMasterMapper { get; set; }
        public RuleMasterBal(string billEditRuleTableNumber)
        {
            BillEditRuleTableNumber = billEditRuleTableNumber;

            RuleMasterMapper = new RuleMasterMapper();
        }

        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<RuleMasterCustomModel> GetRuleMasterList(bool notActive = false)
        {
            var list = new List<RuleMasterCustomModel>();
            using (var rep = UnitOfWork.RuleMasterRepository)
            {
                //var lstRuleMaster =
                //    notActive == false
                //        ? rep.Where(a =>
                //            a.IsActive != false && !string.IsNullOrEmpty(a.ExtValue9) &&
                //            a.ExtValue9.Trim().Equals(BillEditRuleTableNumber))
                //            .ToList()
                //            .OrderByDescending(x => x.RuleMasterID)
                //            .ToList()
                //        : rep.Where(a =>
                //            a.IsActive == false && !string.IsNullOrEmpty(a.ExtValue9) &&
                //            a.ExtValue9.Trim().Equals(BillEditRuleTableNumber))
                //            .ToList()
                //            .OrderByDescending(x => x.RuleMasterID)
                //            .ToList();

                //if (lstRuleMaster.Count > 0)
                //{
                //    list.AddRange(lstRuleMaster.Select(item => RuleMasterMapper.MapModelToViewModel(item)));
                //    list = list.OrderByDescending(f => f.CreatedDate).ToList();
                //}


                list = rep.GetRuleMasterList(BillEditRuleTableNumber, notActive);
                if (list.Count > 0)
                    //list = list.OrderBy(g => g.RuleCode, new NumeralAlphaCompare()).ToList();
                    list = list.OrderBy(g => g.RuleCode1).ToList();


            }
            return list;
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="ruleMaster">The rule master.</param>
        /// <returns></returns>
        public int AddUptdateRuleMaster(RuleMaster ruleMaster)
        {
            using (var ruleMasterRep = UnitOfWork.RuleMasterRepository)
            {

                if (ruleMaster.RuleMasterID > 0)
                {
                    var current = ruleMasterRep.GetSingle(ruleMaster.RuleMasterID);
                    ruleMaster.ExtValue9 = current.ExtValue9;
                    ruleMaster.CreatedBy = current.CreatedBy;
                    ruleMaster.CreatedDate = current.CreatedDate;
                    ruleMasterRep.UpdateEntity(ruleMaster, ruleMaster.RuleMasterID);
                }
                else
                {
                    ruleMaster.ExtValue9 = BillEditRuleTableNumber;
                    ruleMasterRep.Create(ruleMaster);
                }
                return ruleMaster.RuleMasterID;
            }
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="ruleMasterId">The rule master identifier.</param>
        /// <returns></returns>
        public RuleMaster GetRuleMasterById(int? ruleMasterId)
        {
            using (var ruleMasterRep = UnitOfWork.RuleMasterRepository)
            {
                var ruleMaster = ruleMasterRep.Where(a => a.RuleMasterID == ruleMasterId && !string.IsNullOrEmpty(a.ExtValue9) && a.ExtValue9.Trim().Equals(BillEditRuleTableNumber)).FirstOrDefault();
                return ruleMaster;
            }
        }

        /// <summary>
        /// Gets the rule master custom model by identifier.
        /// </summary>
        /// <param name="ruleMasterId">The rule master identifier.</param>
        /// <returns></returns>
        public RuleMasterCustomModel GetRuleMasterCustomModelById(int? ruleMasterId)
        {
            using (var ruleMasterRep = UnitOfWork.RuleMasterRepository)
            {
                var ruleMaster = ruleMasterRep.Where(a => a.RuleMasterID == ruleMasterId).FirstOrDefault();
                var vm = RuleMasterMapper.MapModelToViewModel(ruleMaster);
                return vm ?? new RuleMasterCustomModel();
            }
        }

        /// <summary>
        /// Deletes the rule master.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public int DeleteRuleMaster(RuleMaster model)
        {
            int result;
            using (var rep = UnitOfWork.RuleMasterRepository)
            {
                rep.Delete(model);
                DeleteRuleMasterFromReferenceTables(model.RuleMasterID);
                result = model.RuleMasterID;
            }
            return result;
        }

        private void DeleteRuleMasterFromReferenceTables(int ruleMasterId)
        {
            using (var rep = UnitOfWork.RuleStepRepository)
            {
                var sList = rep.Where(s => s.RuleMasterID == ruleMasterId).ToList();
                if (sList.Count > 0)
                    rep.Delete(sList);
            }

            using (var rep = UnitOfWork.ScrubReportRepository)
            {
                var sList = rep.Where(s => s.RuleMasterID == ruleMasterId).ToList();
                if (sList.Count > 0)
                    rep.Delete(sList);
            }
        }

        public bool DeleteMultipleRules(List<int> ids)
        {
            using (var rep = UnitOfWork.RuleMasterRepository)
            {
                var list = rep.Where(r => ids.Contains(r.RuleMasterID)).ToList();
                if (list.Count > 0)
                {
                    using (var trans = new TransactionScope())
                    {
                        try
                        {
                            using (var sRep = UnitOfWork.RuleStepRepository)
                            {

                                var stepList =
                                    sRep.Where(s => s.RuleMasterID != null && ids.Contains(s.RuleMasterID.Value)).ToList();

                                using (var rRep = UnitOfWork.ScrubReportRepository)
                                {
                                    var scrubReportList =
                                        rRep.Where(s => s.RuleMasterID != null && ids.Contains(s.RuleMasterID.Value)).ToList();
                                    if (scrubReportList.Count > 0)
                                        rRep.Delete(scrubReportList);
                                }

                                if (stepList.Count > 0)
                                    sRep.Delete(stepList);
                            }

                            if (list.Count > 0)
                                rep.Delete(list);

                            trans.Complete();
                        }
                        catch
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
            return true;
        }

    }
}

