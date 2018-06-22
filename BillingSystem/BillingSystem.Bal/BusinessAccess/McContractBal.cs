using System.Collections.Generic;
using System.Linq;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using System;
using BillingSystem.Common.Common;

namespace BillingSystem.Bal.BusinessAccess
{
    public class McContractBal : BaseBal
    {
        public McContractBal()
        {

        }

        public McContractBal(string cptTableNumber, string serviceCodeTableNumber, string drgTableNumber, string drugTableNumber, string hcpcsTableNumber, string diagnosisTableNumber)
        {
            CptTableNumber = cptTableNumber;
            ServiceCodeTableNumber = serviceCodeTableNumber;
            DrgTableNumber = drgTableNumber;
            DrugTableNumber = drugTableNumber;
            HcpcsTableNumber = hcpcsTableNumber;
            DiagnosisTableNumber = diagnosisTableNumber;
        }

        /// <summary>
        /// Gets the contract list.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public IEnumerable<McContractCustomModel> GetManagedCareByFacility(int corporateId, int facilityId, long userId)
        {
            //var list = new List<McContractCustomModel>();
            using (var rep = UnitOfWork.MCContractRepository)
            {
                //var result = rep.Where(a => a.CorporateId == corporateId && a.FacilityId == facilityId)
                //    .OrderBy(m => m.MCOrderCode == "0").ToList();
                //if (result.Any())
                //{
                //    list.AddRange(result.Select(item => new McContractCustomModel
                //    {
                //        MCContractID = item.MCContractID,
                //        BCCreatedBy = item.BCCreatedBy,
                //        BCCreatedDate = item.BCCreatedDate,
                //        BCIsActive = item.BCIsActive,
                //        BCModifiedBy = item.BCModifiedBy,
                //        BCModifiedDate = item.BCModifiedDate,
                //        MCApplyWeightAge = item.MCApplyWeightAge,
                //        MCCode = item.MCCode,
                //        MCCodeRangeFrom = item.MCCodeRangeFrom,
                //        MCCodeRangeTill = item.MCCodeRangeTill,
                //        MCEncounterType =
                //            GetNameByGlobalCodeValue(item.MCEncounterType,
                //                Convert.ToString(Convert.ToInt32(GlobalCodeCategoryValue.EncounterPatientType))),
                //        MCExpireAfterDays = item.MCExpireAfterDays,
                //        MCLevel =
                //            GetNameByGlobalCodeValue(item.MCLevel,
                //                Convert.ToString(Convert.ToInt32(GlobalCodeCategoryValue.MCContractLevel))),
                //        MCMultiplier = item.MCMultiplier,
                //        MCOrderCode = item.MCOrderCode,
                //        MCPatientCapping = item.MCPatientCapping,
                //        MCPatientFixed = item.MCPatientFixed,
                //        MCPatientPercent = item.MCPatientPercent,
                //        MCWaitingDays = item.MCWaitingDays,
                //        MCOrderType = item.MCOrderType,
                //        OrderTypeText = GetOrderTypeText(item.MCOrderType),
                //        MCAnnualOutOfPocket = item.MCAnnualOutOfPocket,
                //        MCInPatientBaseRate = item.MCInPatientBaseRate,
                //        MCCarveoutsApplicable = item.MCCarveoutsApplicable,
                //        MCCPTTableNumber = item.MCCPTTableNumber,
                //        MCDRGTableNumber = item.MCDRGTableNumber,
                //        MCPerDiemsApplicable = item.MCPerDiemsApplicable,
                //        ModelName = item.ModelName,
                //        PenaltyLateSubmission = item.PenaltyLateSubmission,
                //        ResubmitDays1 = item.ResubmitDays1,
                //        ResubmitDays2 = item.ResubmitDays2,
                //        ExpectedPaymentDays = item.ExpectedPaymentDays,
                //        InitialSubmitDay = item.InitialSubmitDay,
                //        BillScrubberRule = item.BillScrubberRule,

                //        MCRuleSetNumber = item.MCRuleSetNumber,
                //        MCPenaltyRateResubmission = item.MCPenaltyRateResubmission,
                //        MCOutpatientDeduct = item.MCOutpatientDeduct,
                //        MCMultiplierOutpatient = item.MCMultiplierOutpatient,
                //        MCMultiplierOther = item.MCMultiplierOther,
                //        MCEMCertified = item.MCEMCertified,
                //        MCMultiplierEmergencyRoom = item.MCMultiplierEmergencyRoom,
                //        MCInpatientDeduct = item.MCInpatientDeduct,
                //        MCAddon = item.MCAddon,
                //        MCExpectedFixedrate = item.MCExpectedFixedrate,
                //        MCExpectedPercentage = item.MCExpectedPercentage,
                //        MCInPatientType = item.MCInPatientType,
                //        MCOPPatientType = item.MCOPPatientType,
                //        MCERPatientType = item.MCERPatientType,

                //        ARGeneralLedgerAccount = item.ARGeneralLedgerAccount,
                //        MCGeneralLedgerAccount = item.MCGeneralLedgerAccount,
                //    }));
                //}
                var list = rep.GetManagedCareDataByFacility(facilityId, corporateId, true, userId);
                return list;
            }
        }

        /// <summary>
        /// Saves the contract.
        /// </summary>
        /// <param name="m">The model.</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public IEnumerable<McContractCustomModel> SaveContract(MCContract m)
        {
            using (var rep = UnitOfWork.MCContractRepository)
            {
                if (m.MCContractID > 0)
                {
                    var current = rep.GetSingle(m.MCContractID);
                    m.BCCreatedBy = current.BCCreatedBy;
                    m.BCCreatedDate = current.BCCreatedDate;
                    rep.UpdateEntity(m, m.MCContractID);
                }
                else
                    rep.Create(m);

                var list = rep.GetManagedCareDataByFacility(m.FacilityId, m.CorporateId, true, m.BCCreatedBy);
                return list;
            }
        }

        /// <summary>
        /// Gets the mc contract detail.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public McContractCustomModel GetMcContractDetail(int id)
        {
            using (var rep = UnitOfWork.MCContractRepository)
            {
                var item = rep.Where(m => m.MCContractID == id).FirstOrDefault();
                if (item != null)
                {
                    var customModel = new McContractCustomModel
                    {
                        MCContractID = item.MCContractID,
                        BCCreatedBy = item.BCCreatedBy,
                        BCCreatedDate = item.BCCreatedDate,
                        BCIsActive = item.BCIsActive,
                        BCModifiedBy = item.BCModifiedBy,
                        BCModifiedDate = item.BCModifiedDate,
                        MCApplyWeightAge = item.MCApplyWeightAge,
                        MCCode = item.MCCode,// string.IsNullOrEmpty(item.ModelName) ? GetNameByGlobalCodeValue(item.MCCode, Convert.ToString(Convert.ToInt32(GlobalCodeCategoryValue.McManagedCareCode))) : item.ModelName,
                        MCCodeRangeFrom = item.MCCodeRangeFrom,
                        MCCodeRangeTill = item.MCCodeRangeTill,
                        MCEncounterType = item.MCEncounterType,// GetNameByGlobalCodeValue(item.MCEncounterType, Convert.ToString(Convert.ToInt32(GlobalCodeCategoryValue.EncounterPatientType))),
                        MCExpireAfterDays = item.MCExpireAfterDays,
                        MCLevel = GetNameByGlobalCodeValue(item.MCLevel, Convert.ToString(Convert.ToInt32(GlobalCodeCategoryValue.MCContractLevel))),
                        MCMultiplier = item.MCMultiplier,
                        MCOrderCode = item.MCOrderCode,
                        MCPatientCapping = item.MCPatientCapping,
                        MCPatientFixed = item.MCPatientFixed,
                        MCPatientPercent = item.MCPatientPercent,
                        MCWaitingDays = item.MCWaitingDays,
                        MCOrderType = item.MCOrderType,
                        OrderTypeText = GetOrderTypeText(item.MCOrderType),
                        MCAnnualOutOfPocket = item.MCAnnualOutOfPocket,
                        MCInPatientBaseRate = item.MCInPatientBaseRate,
                        MCCarveoutsApplicable = item.MCCarveoutsApplicable,
                        MCCPTTableNumber = item.MCCPTTableNumber,
                        MCDRGTableNumber = item.MCDRGTableNumber,
                        MCPerDiemsApplicable = item.MCPerDiemsApplicable,
                        ModelName = item.ModelName,
                        PenaltyLateSubmission = item.PenaltyLateSubmission,
                        ResubmitDays1 = item.ResubmitDays1,
                        ResubmitDays2 = item.ResubmitDays2,
                        ExpectedPaymentDays = item.ExpectedPaymentDays,
                        InitialSubmitDay = item.InitialSubmitDay,
                        BillScrubberRule = item.BillScrubberRule,
                        MCRuleSetNumber = item.MCRuleSetNumber,
                        MCPenaltyRateResubmission = item.MCPenaltyRateResubmission,
                        MCOutpatientDeduct = item.MCOutpatientDeduct,
                        MCMultiplierOutpatient = item.MCMultiplierOutpatient,
                        MCMultiplierOther = item.MCMultiplierOther,
                        MCEMCertified = item.MCEMCertified,
                        MCMultiplierEmergencyRoom = item.MCMultiplierEmergencyRoom,
                        MCInpatientDeduct = item.MCInpatientDeduct,
                        MCAddon = item.MCAddon,
                        MCExpectedFixedrate = item.MCExpectedFixedrate,
                        MCExpectedPercentage = item.MCExpectedPercentage,
                        MCInPatientType = item.MCInPatientType,
                        MCOPPatientType = item.MCOPPatientType,
                        MCERPatientType = item.MCERPatientType,
                        ARGeneralLedgerAccount = item.ARGeneralLedgerAccount,
                        MCGeneralLedgerAccount = item.MCGeneralLedgerAccount,
                    };
                    if (customModel.MCLevel == "3")
                    {
                        //var orderCodeObj = GetSelectedCodeParent(customModel.MCOrderCode, customModel.MCOrderType);
                        var orderCodeObj = GetSelectedCodeParent1(customModel.MCOrderCode, customModel.MCOrderType, 0, "4010");

                        customModel.OrderCategoryId = orderCodeObj.GlobalCodeCategoryId;
                        customModel.OrderSubCategoryId = orderCodeObj.GlobalCodeId.ToString();
                    }
                    return customModel;
                }
                return null;
            }
        }

        /// <summary>
        /// Deletes the contract.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public IEnumerable<McContractCustomModel> DeleteContract(int id, int corporateId, int facilityId, long userId)
        {
            using (var rep = UnitOfWork.MCContractRepository)
            {
                rep.Delete(id);
                var list = rep.GetManagedCareDataByFacility(facilityId, corporateId, true, userId);
                return list;
            }
        }

        /// <summary>
        /// Gets the order type text.
        /// </summary>
        /// <param name="globalCodeCategoryValue">The global code category value.</param>
        /// <returns></returns>
        private string GetOrderTypeText(string globalCodeCategoryValue)
        {
            var result = string.Empty;
            if (!string.IsNullOrEmpty(globalCodeCategoryValue))
            {
                if (Convert.ToInt32(globalCodeCategoryValue) < 100)
                {
                    result = GetNameByGlobalCodeValue(globalCodeCategoryValue, Convert.ToString(Convert.ToInt32(GlobalCodeCategoryValue.CodeTypes)));
                }
                else
                {
                    var value = Convert.ToInt32(globalCodeCategoryValue);
                    var enumString = ((MCOrderType)value);
                    result = Convert.ToString(enumString);
                }
            }
            return result;
        }

        /// <summary>
        /// Gets the mc overview.
        /// </summary>
        /// <param name="MCCode">The mc code.</param>
        /// <returns></returns>
        public string GetMCOverview(int MCCode)
        {
            using (var rep = UnitOfWork.MCContractRepository)
            {
                var mcOverviewObj = rep.GetMCOverview(MCCode);
                var firstFileString = mcOverviewObj.FirstOrDefault();
                return firstFileString != null
                    ? !string.IsNullOrEmpty(firstFileString.MCOverview)
                        ? firstFileString.MCOverview.Replace("/n", "<br/><br/>")
                        : ""
                    : "";
            }
        }

        /// <summary>
        /// Gets the name of the manage care.
        /// </summary>
        /// <param name="MCCode">The mc code.</param>
        /// <returns></returns>
        public string GetManageCareName(string MCCode)
        {
            using (var rep = UnitOfWork.MCContractRepository)
            {
                var mcOverviewObj = rep.Where(x => x.MCCode == MCCode);
                var firstFileString = mcOverviewObj.FirstOrDefault();
                return firstFileString != null
                    ? firstFileString.ModelName
                    : "";
            }
        }

    }
}
