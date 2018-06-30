using System.Collections.Generic;
using System.Linq;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using System;
using BillingSystem.Common.Common;
using BillingSystem.Repository.Interfaces;
using System.Data.SqlClient;
using BillingSystem.Repository.Common;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class McContractService : IMcContractService
    {
        private readonly IRepository<MCContract> _repository;
        private readonly BillingEntities _context;
        private readonly IRepository<GlobalCodes> _gRepository;

        public McContractService(IRepository<MCContract> repository, BillingEntities context, IRepository<GlobalCodes> gRepository)
        {
            _repository = repository;
            _context = context;
            _gRepository = gRepository;
        }

        /// <summary>
        /// Gets the contract list.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public IEnumerable<McContractCustomModel> GetManagedCareByFacility(int corporateId, int facilityId, long userId)
        {
            var sqlParameters = new SqlParameter[4];
            sqlParameters[0] = new SqlParameter("pFId", facilityId);
            sqlParameters[1] = new SqlParameter("pCId", corporateId);
            sqlParameters[2] = new SqlParameter("pIsActive", true);
            sqlParameters[3] = new SqlParameter("pUserId", userId);

            using (var ms = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetManagedCareDataByFacility.ToString(), parameters: sqlParameters, isCompiled: false))
            {
                var mList = ms.GetResultWithJson<McContractCustomModel>(JsonResultsArray.ManagedCareResult.ToString());
                return mList;
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
            if (m.MCContractID > 0)
            {
                var current = _repository.GetSingle(m.MCContractID);
                m.BCCreatedBy = current.BCCreatedBy;
                m.BCCreatedDate = current.BCCreatedDate;
                _repository.UpdateEntity(m, m.MCContractID);
            }
            else
                _repository.Create(m);

            var list = GetManagedCareDataByFacility(m.FacilityId, m.CorporateId, true, m.BCCreatedBy);
            return list;
        }

        private IEnumerable<McContractCustomModel> GetManagedCareDataByFacility(long facilityId, long corporateId, bool activeStatus, long loggedinUserId)
        {
            var sqlParameters = new SqlParameter[4];
            sqlParameters[0] = new SqlParameter("pFId", facilityId);
            sqlParameters[1] = new SqlParameter("pCId", corporateId);
            sqlParameters[2] = new SqlParameter("pIsActive", activeStatus);
            sqlParameters[3] = new SqlParameter("pUserId", loggedinUserId);

            using (var ms = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetManagedCareDataByFacility.ToString(), parameters: sqlParameters, isCompiled: false))
            {
                var mList = ms.GetResultWithJson<McContractCustomModel>(JsonResultsArray.ManagedCareResult.ToString());
                return mList;
            }
            //return Enumerable.Empty<McContractCustomModel>();
        }
        /// <summary>
        /// Gets the mc contract detail.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public McContractCustomModel GetMcContractDetail(int id)
        {
            var item = _repository.Where(m => m.MCContractID == id).FirstOrDefault();
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
        private GeneralCodesCustomModel GetSelectedCodeParent1(string orderCode, string codeType, long facilityId, string tableNumber)
        {
            var vm = new GeneralCodesCustomModel();
            var sqlParams = new SqlParameter[4];

            sqlParams[0] = new SqlParameter("@pOrderCode", orderCode);
            sqlParams[1] = new SqlParameter("@pOrderCodeType", codeType);
            sqlParams[2] = new SqlParameter("@pFId", facilityId);
            sqlParams[3] = new SqlParameter("@pTableNumber", tableNumber);

            using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetSelectedCodeParent.ToString(), isCompiled: false, parameters: sqlParams))
            {
                try
                {
                    var c = r.ResultSetFor<GlobalCodeSaveModel>().FirstOrDefault();
                    if (c != null)
                    {
                        vm = new GeneralCodesCustomModel
                        {
                            GlobalCodeCategoryId = c.GlobalCodeCategoryValue,
                            GlobalCodeCategoryName = c.GlobalCodeCategoryNameStr,
                            GlobalCodeId = Convert.ToInt32(c.GlobalCodeValue),
                            GlobalCodeName = c.GlobalCodeName
                        };
                    }
                }
                catch (Exception ex)
                {
                    //throw;
                }
            }
            return vm;
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
            _repository.Delete(id);
            var list = GetManagedCareDataByFacility(facilityId, corporateId, true, userId);
            return list;
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
            var spName = string.Format("EXEC {0} @pMCCode", StoredProcedures.SPROC_GetMCOverview.ToString());
            var sqlParameters = new SqlParameter[1];
            sqlParameters[0] = new SqlParameter("pMCCode", MCCode);
            IEnumerable<McContractOverViewCustomModel> result = _context.Database.SqlQuery<McContractOverViewCustomModel>(spName, sqlParameters);
            var mcOverviewObj = result.ToList();

            var firstFileString = mcOverviewObj.FirstOrDefault();
            return firstFileString != null ? !string.IsNullOrEmpty(firstFileString.MCOverview) ? firstFileString.MCOverview.Replace("/n", "<br/><br/>") : "" : "";
        }

        /// <summary>
        /// Gets the name of the manage care.
        /// </summary>
        /// <param name="MCCode">The mc code.</param>
        /// <returns></returns>
        public string GetManageCareName(string MCCode)
        {
            var mcOverviewObj = _repository.Where(x => x.MCCode == MCCode);
            var firstFileString = mcOverviewObj.FirstOrDefault();
            return firstFileString != null ? firstFileString.ModelName : "";
        }

    }
}
