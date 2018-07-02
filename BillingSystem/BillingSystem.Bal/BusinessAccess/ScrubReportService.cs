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
    public class ScrubReportService : IScrubReportService
    {
        private readonly IRepository<ScrubHeader> _repository;
        private readonly IRepository<ScrubReport> _rRepository;
        private readonly IRepository<Users> _uRepository;
        private readonly IRepository<PatientInfo> _piRepository;
        private readonly IRepository<BillHeader> _bhRepository;
        private readonly IRepository<GlobalCodes> _gRepository;
        private readonly IRepository<RuleStep> _rsRepository;
        private readonly IRepository<RuleMaster> _rmRepository;
        private readonly IRepository<ErrorMaster> _erRepository;
        private readonly BillingEntities _context;
        private readonly IMapper _mapper;

        public ScrubReportService(IRepository<ScrubHeader> repository, IRepository<ScrubReport> rRepository, IRepository<Users> uRepository, IRepository<PatientInfo> piRepository, IRepository<BillHeader> bhRepository, IRepository<GlobalCodes> gRepository, IRepository<RuleStep> rsRepository, IRepository<RuleMaster> rmRepository, IRepository<ErrorMaster> erRepository, BillingEntities context, IMapper mapper)
        {
            _repository = repository;
            _rRepository = rRepository;
            _uRepository = uRepository;
            _piRepository = piRepository;
            _bhRepository = bhRepository;
            _gRepository = gRepository;
            _rsRepository = rsRepository;
            _rmRepository = rmRepository;
            _erRepository = erRepository;
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="billHeaderId">The bill header identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="createscrub">if set to <c>true</c> [createscrub].</param>
        /// <returns>
        /// Return the Entity List
        /// </returns>
        public List<ScrubHeaderCustomModel> GetScrubHeaderList(int corporateId, int facilityId, int billHeaderId, int userId, bool createscrub)
        {
            var list = new List<ScrubHeaderCustomModel>();
            if (createscrub)
                ApplyScrubBill(corporateId, facilityId, userId);

            var spName = string.Format("EXEC {0} @pCorporateID, @pFacilityID", StoredProcedures.SPROC_GetSrubHeaderDetail);
            var sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("pCorporateID", corporateId);
            sqlParameters[1] = new SqlParameter("pFacilityID", facilityId);
            IEnumerable<ScrubHeader> result = _context.Database.SqlQuery<ScrubHeader>(spName, sqlParameters);
            var headerList = result.ToList();

            if (headerList.Count > 0)
            {
                if (billHeaderId > 0)
                    headerList = headerList.Where(a => a.BillHeaderID != null && a.BillHeaderID == billHeaderId).ToList();

                list = MapModelToViewModel(headerList);

            }
            return list;
        }
        private bool ApplyScrubBill(int corporateId, int facilityId, int loggedUserId)
        {
            try
            {
                var sqlParameters = new SqlParameter[3];
                sqlParameters[0] = new SqlParameter("pCorporateID", corporateId);
                sqlParameters[1] = new SqlParameter("pFacilityID", facilityId);
                sqlParameters[2] = new SqlParameter("pExecutedBy", loggedUserId);
                _repository.ExecuteCommand(StoredProcedures.SPROC_ScrubBill_Batch.ToString(), sqlParameters);
                return true;

            }
            catch (Exception)
            {
                //throw ex;
            }
            return false;
        }

        private string GetNameByUserId(int? UserID)
        {
            var usersModel = _uRepository.Where(x => x.UserID == UserID && x.IsDeleted == false).FirstOrDefault();
            return usersModel != null ? usersModel.FirstName + " " + usersModel.LastName : string.Empty;
        }
        private string GetPatientNameById(int PatientID)
        {
            var m = _piRepository.GetSingle(Convert.ToInt32(PatientID));
            return m != null ? m.PersonFirstName + " " + m.PersonLastName : string.Empty;
        }
        private List<ScrubHeaderCustomModel> MapModelToViewModel(List<ScrubHeader> m)
        {
            var lst = new List<ScrubHeaderCustomModel>();
            foreach (var model in m)
            {
                var vm = _mapper.Map<ScrubHeaderCustomModel>(model);
                if (vm != null)
                {
                    vm.AssignedByUser = GetNameByUserId(model.AssignedBy);
                    vm.AssignedToUser = GetNameByUserId(model.AssignedTo);
                    vm.ExecutedByUser = GetNameByUserId(model.ExecutedBy);
                    vm.PatientName = GetPatientNameById(Convert.ToInt32(model.PatientID));
                    vm.BillHeaderStatus = GetBillHeaderStatusIDByBillHeaderId(Convert.ToInt32(model.BillHeaderID));
                    vm.Section = GetSectionValueByBillHeaderId(Convert.ToInt32(model.BillHeaderID));
                    vm.Failed = model.Failed ?? 0;
                    vm.NotApplicable = model.NotApplicable ?? 0;
                    vm.Passed = model.Passed ?? 0;
                    vm.Performed = model.Performed ?? 0;
                }
                lst.Add(vm);
            }

            return lst;

        }
        private int GetSectionValueByBillHeaderId(int billHeaderId)
        {
            var sectionValue = string.Empty;
            var bh = _bhRepository.Where(b => b.BillHeaderID == billHeaderId).FirstOrDefault();
            if (bh != null)
            {
                var status = Convert.ToString(Convert.ToInt32(GlobalCodeCategoryValue.BillHeaderStatus));
                sectionValue = _gRepository.Where(
                        g => g.GlobalCodeValue.Equals(bh.Status) && g.GlobalCodeCategoryValue.Equals(status))
                        .Select(e => e.ExternalValue5)
                        .FirstOrDefault();
            }
            return string.IsNullOrEmpty(sectionValue) ? 0 : Convert.ToInt32(sectionValue);
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
        private string GetBillHeaderStatusIDByBillHeaderId(int billHeaderId)
        {
            var bh = _bhRepository.Where(b => b.BillHeaderID == billHeaderId).FirstOrDefault();
            if (bh != null)
            {
                var gc = GetNameByGlobalCodeValue(
                    bh.Status,
                    Convert.ToString(Convert.ToInt32(GlobalCodeCategoryValue.BillHeaderStatus)));
                return gc;
            }
            return string.Empty;
        }
        private RuleStep GetRuleStepDetailsById(int ruleStepId)
        {
            var current = _rsRepository.Where(r => r.RuleStepID == ruleStepId).FirstOrDefault();
            return current ?? new RuleStep();

        }
        private RuleMaster GetRuleMasterDetailsById(int ruleMasterId)
        {
            var current = _rmRepository.Where(r => r.RuleMasterID == ruleMasterId).FirstOrDefault();
            return current ?? new RuleMaster();

        }
        private List<ScrubReportCustomModel> MapReportValues(List<ScrubReport> m)
        {
            var lst = new List<ScrubReportCustomModel>();
            foreach (var model in m)
            {
                var vm = _mapper.Map<ScrubReportCustomModel>(model);
                if (vm != null)
                {
                    var ruleStep = GetRuleStepDetailsById(Convert.ToInt32(vm.RuleStepID));
                    var ruleMaster = GetRuleMasterDetailsById(Convert.ToInt32(vm.RuleMasterID));
                    var rhsTooltip = ruleStep.RHSC;
                    var lhsToolTip = ruleStep.LHSC;
                    var ruleCode = ruleMaster.RuleCode + "  (Rule Step number: " + ruleStep.RuleStepNumber + " )";

                    // Case for Direct (3) and Query String (4) Values
                    if (string.IsNullOrEmpty(lhsToolTip))
                    {
                        lhsToolTip = "Direct Value Entered by User";
                    }

                    // Case for Direct (3) and Query String (4) Values
                    if (string.IsNullOrEmpty(rhsTooltip))
                    {
                        rhsTooltip = ruleStep.RHSFrom == 4
                                         ? "Calculated Value based on Custom-Query"
                                         : "Direct Value Entered by User";
                    }

                    // Case for Count Function Type (Column: QueryFunctionType in RuleStep Table)
                    if (!string.IsNullOrEmpty(ruleStep.QueryFunctionType)
                        && ruleStep.QueryFunctionType.Trim().Equals("2"))
                    {
                        lhsToolTip = "Calculated Value based on Custom-Query";
                    }

                    vm.CompareTypeText = GetNameByGlobalCodeValue(Convert.ToString(vm.CompareType), Convert.ToString((int)GlobalCodeCategoryValue.DataComparer));
                    vm.RuleMasterDesc = ruleStep.RuleStepDescription;
                    vm.RuleStepDesc = GetDescByRuleStepId(Convert.ToInt32(vm.RuleStepID));
                    vm.ErrorText = GetDescByErrorId(Convert.ToInt32(vm.ErrorID));
                    vm.LhsTooltip = lhsToolTip;
                    vm.RhsTooltip = rhsTooltip;
                    vm.ConStartDesc = GetNameByGlobalCodeValue(Convert.ToString(model.ConStart), Convert.ToString((int)GlobalCodeCategoryValue.ConditionStart));
                    vm.ConEndDesc = GetNameByGlobalCodeValue(Convert.ToString(model.ConEnd), Convert.ToString((int)GlobalCodeCategoryValue.ConditionEnd));
                    vm.RuleStepsValue = ruleCode;
                }

                lst.Add(vm);
            }

            return lst;

        }
        private string GetDescByRuleStepId(int id)
        {
            var model = _rsRepository.Where(a => a.RuleStepID == id).FirstOrDefault();
            return model != null ? model.RuleStepDescription : string.Empty;
        }

        private string GetDescByErrorId(int id)
        {
            var model = _erRepository.Where(a => a.ErrorMasterID == id).FirstOrDefault();
            return model != null ? model.ErrorDescription : string.Empty;
        }
        /// <summary>
        /// Gets the scrub report.
        /// </summary>
        /// <param name="scrubHeaderId">The scrub header identifier.</param>
        /// <param name="reportType">Type of the report.</param>
        /// <returns></returns>
        public List<ScrubReportCustomModel> GetScrubReport(int scrubHeaderId, int reportType)
        {
            try
            {
                var list = new List<ScrubReportCustomModel>();
                var lstScrubReport = reportType == 999
                    ? _rRepository.Where(a => a.IsActive != null && (bool)a.IsActive && a.ScrubHeaderID == scrubHeaderId)
                        .ToList()
                    : _rRepository.Where(
                        a =>
                            a.IsActive != null && (bool)a.IsActive && a.ScrubHeaderID == scrubHeaderId &&
                            a.Status == reportType).ToList();

                if (lstScrubReport.Count > 0)
                {
                    list = MapReportValues(lstScrubReport);

                    var ruleMasterId = list.Select(x => x.RuleMasterID).Distinct();
                    var masterId = ruleMasterId as IList<int?> ?? ruleMasterId.ToList();
                    for (var i = 0; i < masterId.Count(); i++)
                    {
                        int? objid = masterId[i];
                        if (i % 2 != 0)
                        {
                            list.Where(x => x.RuleMasterID == objid).ToList().ForEach(x => x.RuleGroup = "same");
                        }
                        else
                        {
                            list.Where(x => x.RuleMasterID == objid).ToList().ForEach(x => x.RuleGroup = "diff");
                        }
                    }
                }

                return list;
            }
            catch (Exception)
            {
                //throw ex;
            }
            return null;
        }

        /// <summary>
        /// Gets the scrub report detail by identifier.
        /// </summary>
        /// <param name="scrubReportId">The scrub report identifier.</param>
        /// <returns></returns>
        public ScrubReportCustomModel GetScrubReportDetailById(int scrubReportId, string BillEditRuleTableNumber)
        {
            var current = new ScrubReportCustomModel();
            var item = _rRepository.Where(r => r.ScrubReportID == scrubReportId).FirstOrDefault();
            if (item != null)
            {
                current = new ScrubReportCustomModel
                {
                    AssignedBy = item.AssignedBy,
                    AssignedDate = item.AssignedDate,
                    ScrubReportID = item.ScrubReportID,
                    AssignedTo = item.AssignedTo,
                    Status = item.Status,
                    BillActivityID = item.BillActivityID,
                    ExtValue1 = item.ExtValue1,
                    ExtValue2 = item.ExtValue2,
                    ExtValue3 = item.ExtValue3,
                    ExtValue4 = string.IsNullOrEmpty(item.ExtValue4) ? "0" : item.ExtValue4,
                    CompareType = item.CompareType,
                    IsActive = item.IsActive,
                    ModifiedBy = item.ModifiedBy,
                    ModifiedDate = item.ModifiedDate,
                    CreatedBy = item.CreatedBy,
                    CreatedDate = item.CreatedDate,
                    ErrorID = item.ErrorID,
                    RuleMasterID = item.RuleMasterID,
                    LHSV = item.LHSV,
                    RHSV = item.RHSV,
                    ScrubHeaderID = item.ScrubHeaderID,
                    RuleStepID = item.RuleStepID,
                    CompareTypeText = GetNameByGlobalCodeValue(Convert.ToInt32(item.CompareType).ToString(), Convert.ToInt32(GlobalCodeCategoryValue.DataComparer).ToString()),
                    RuleMasterDesc = GetDescByRuleMasterId(Convert.ToInt32(item.RuleMasterID), BillEditRuleTableNumber),
                    RuleStepDesc = GetDescByRuleStepId(Convert.ToInt32(item.RuleStepID)),
                    ErrorText = GetDescByErrorId(Convert.ToInt32(item.ErrorID)),

                    // PatientId = billactiviyBal.GetPatientIdByBillActivityId(Convert.ToInt32(item.BillActivityID)),
                    // EncounterId = billactiviyBal.GetEncounterIdByBillActivityId(Convert.ToInt32(item.BillActivityID)),
                };

                var ruleStep = _rsRepository.Where(r => r.RuleStepID == current.RuleStepID && r.RuleMasterID == current.RuleMasterID).FirstOrDefault();
                if (ruleStep != null)
                {
                    var dataType = GetNameByGlobalCodeValue(Convert.ToInt32(ruleStep.DataType).ToString(), Convert.ToInt32(GlobalCodeCategoryValue.DataTypes).ToString());
                    if (!string.IsNullOrEmpty(dataType))
                    {
                        current.CssClass = GetCssClassByGlobalCodeId(ruleStep.DataType.ToString());
                        if (dataType.ToLower().Equals("datetime"))
                        {
                            DateTime dtLhValue;
                            DateTime dtRhValue;
                            var lhValue = DateTime.TryParse(item.LHSV, out dtLhValue);
                            var rhValue = DateTime.TryParse(item.RHSV, out dtRhValue);

                            if (lhValue)
                            {
                                current.LHSV = dtLhValue.ToString("MM/dd/yyyy HH:mm");
                            }

                            if (rhValue)
                            {
                                current.RHSV = dtRhValue.ToString("MM/dd/yyyy HH:mm");
                            }
                        }
                    }

                    current.LHSVDesc = ruleStep.LHSC;
                    current.RHSVDesc = !string.IsNullOrEmpty(ruleStep.RHSC) ? ruleStep.RHSC : "Direct Value";
                    if (current.RuleMasterID != null)
                    {
                        // get rule master by rule master id to get the rule code
                        var rulemasterDetail = GetRuleMasterById(current.RuleMasterID, BillEditRuleTableNumber);
                        if (rulemasterDetail != null)
                        {
                            var errormaster = _erRepository.Where(e => e.ErrorCode == rulemasterDetail.RuleCode).FirstOrDefault();

                            // compare the rule code with error code and get the error resolution
                            current.ErrorResolutionTxt = errormaster != null
                                ? errormaster.ErrorResolution
                                : string.Empty;
                        }
                    }
                }
            }

            return current;
        }
        private string GetDescByRuleMasterId(int id, string BillEditRuleTableNumber)
        {
            var model = _rmRepository.Where(a => a.RuleMasterID == id && !string.IsNullOrEmpty(a.ExtValue9) && a.ExtValue9.Trim().Equals(BillEditRuleTableNumber)).FirstOrDefault();
            return model != null ? model.RuleDescription : string.Empty;
        }

        private RuleMaster GetRuleMasterById(int? ruleMasterId, string BillEditRuleTableNumber)
        {
            var ruleMaster = _rmRepository.Where(a => a.RuleMasterID == ruleMasterId && !string.IsNullOrEmpty(a.ExtValue9) && a.ExtValue9.Trim().Equals(BillEditRuleTableNumber)).FirstOrDefault();
            return ruleMaster;
        }
        /// <summary>
        /// Updates the scrub report detail with correction.
        /// </summary>
        /// <param name="scrubReportId">
        /// The scrub report identifier.
        /// </param>
        /// <param name="scrubHeaderId">
        /// The scrub header identifier.
        /// </param>
        /// <param name="lhsValue">
        /// The lhs Value.
        /// </param>
        /// <param name="rhsValue">
        /// The rhs Value.
        /// </param>
        /// <param name="loggedinUserId">
        /// The user identifier.
        /// </param>
        /// <param name="corporateId">
        /// The corporate Id.
        /// </param>
        /// <param name="facilityid">
        /// The facilityid.
        /// </param>
        /// <param name="correctionCodeId">
        /// The correction Code Id.
        /// </param>
        /// <returns>
        /// </returns>
        public bool UpdateScrubReportDetailWithCorrection(int scrubReportId, int scrubHeaderId, string lhsValue, string rhsValue, int loggedinUserId, int corporateId, int facilityid, string correctionCodeId)
        {
            var sqlParameters = new SqlParameter[8];
            sqlParameters[0] = new SqlParameter("pScrubHeaderID", scrubHeaderId);
            sqlParameters[1] = new SqlParameter("pScrubReportID", scrubReportId);
            sqlParameters[2] = new SqlParameter("pLHSV", lhsValue);
            sqlParameters[3] = new SqlParameter("pRHSV", rhsValue);
            sqlParameters[4] = new SqlParameter("pExecutedBy", loggedinUserId);
            sqlParameters[5] = new SqlParameter("pCorporateID", corporateId);
            sqlParameters[6] = new SqlParameter("pFacilityID", facilityid);
            sqlParameters[7] = new SqlParameter("pcorrectionCodeId", correctionCodeId);
            _rRepository.ExecuteCommand(StoredProcedures.SPROC_ScrubReportCorrections.ToString(), sqlParameters);
            return true;
        }

        /// <summary>
        /// Assigns the user to scrub header for bill edit.
        /// </summary>
        /// <param name="assignedTo">The assigned to.</param>
        /// <param name="scrubHeaderId">The scrub header identifier.</param>
        /// <param name="loggedUserId">The logged user identifier.</param>
        /// <param name="assignedDate">The assigned date.</param>
        /// <returns></returns>
        public string AssignUserToScrubHeaderForBillEdit(
            int assignedTo,
            int scrubHeaderId,
            int loggedUserId,
            DateTime assignedDate)
        {
            var model = _repository.Where(h => h.ScrubHeaderID == scrubHeaderId).FirstOrDefault();
            if (model != null)
            {
                model.AssignedBy = loggedUserId;
                model.AssignedTo = assignedTo;
                model.AssignedDate = assignedDate;
                _repository.Update(model);
                var user = GetNameByUserId(model.AssignedTo);
                return user;
            }

            return string.Empty;
        }

        /// <summary>
        /// Applies the scrub bill to specific bill header identifier.
        /// </summary>
        /// <param name="billHeaderId">The bill header identifier.</param>
        /// <param name="loggedUserId">The logged user identifier.</param>
        /// <returns></returns>
        public bool ApplyScrubBillToSpecificBillHeaderId(int billHeaderId, int loggedUserId)
        {
            var sqlParameters = new SqlParameter[3];
            sqlParameters[0] = new SqlParameter("pBillHeaderID", billHeaderId);
            sqlParameters[1] = new SqlParameter("pExecutedBy", loggedUserId);
            sqlParameters[2] = new SqlParameter
            {
                Direction = System.Data.ParameterDirection.Output,
                ParameterName = "pRETStatus",
                Value = 0,
                SqlDbType = System.Data.SqlDbType.Int
            };
            _repository.ExecuteCommand(StoredProcedures.SPROC_ScrubBill.ToString(), sqlParameters);
            return true;
        }

        #region Work Queues

        /// <summary>
        /// The get scrub header list work queues.
        /// </summary>
        /// <param name="corporateId">
        /// The corporate id.
        /// </param>
        /// <param name="facilityId">
        /// The facility id.
        /// </param>
        /// <param name="billHeaderId">
        /// The bill header id.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="createscrub">
        /// The createscrub.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<ScrubHeaderCustomModel> GetScrubHeaderListWorkQueues(
            int corporateId,
            int facilityId,
            int billHeaderId,
            int userId,
            bool createscrub)
        {
            var list = new List<ScrubHeaderCustomModel>();
            ////Call SP for Scrub Billing
            if (createscrub)
            {
                ApplyScrubBill(corporateId, facilityId, userId);
            }

            // var headerList = (corporateId > 0 && facilityId > 0) ? rep.Where(a => (a.IsActive != null && (bool)a.IsActive) && (int)a.CorporateID == corporateId && (int)a.FacilityID == facilityId && (int)a.BillHeaderID == billHeaderId).ToList() : rep.Where(a => (a.IsActive != null && (bool)a.IsActive) && (int)a.BillHeaderID == billHeaderId).ToList();
            var headerList = (corporateId > 0 && facilityId > 0)
                ? _repository.Where(a => (a.IsActive != null && (bool)a.IsActive)
                                 && (int)a.CorporateID == corporateId && (int)a.FacilityID == facilityId)
                    .GroupBy(g => g.BillHeaderID)
                    .Select(gg => gg.OrderByDescending(p => p.ScrubDate).FirstOrDefault())
                    .ToList()
                : _repository.Where(a => (a.IsActive != null && (bool)a.IsActive))
                    .GroupBy(g => g.BillHeaderID)
                    .Select(gg => gg.OrderByDescending(p => p.ScrubDate).FirstOrDefault())
                    .ToList();

            headerList = headerList.Where(a => a.AssignedTo != null && (int)a.AssignedTo == userId).ToList();

            if (headerList.Count > 0)
            {
                if (billHeaderId > 0)
                {
                    headerList = headerList.Where(a => a.BillHeaderID != null && (int)a.BillHeaderID == billHeaderId).ToList();
                }

                list.AddRange(headerList.Select(item => new ScrubHeaderCustomModel
                {
                    AssignedBy = item.AssignedBy,
                    AssignedDate = item.AssignedDate,
                    AssignedTo = item.AssignedTo,
                    BillHeaderID = item.BillHeaderID,
                    CorporateID = item.CorporateID,
                    EncounterID = item.EncounterID,
                    ExecutedBy = item.ExecutedBy,
                    ExtValue1 = item.ExtValue1,
                    ExtValue2 = item.ExtValue2,
                    ExtValue3 = item.ExtValue3,
                    ExtValue4 = item.ExtValue4,
                    FacilityID = item.FacilityID,
                    Failed = item.Failed ?? 0,
                    IsActive = item.IsActive,
                    ModifiedBy = item.ModifiedBy,
                    ModifiedDate = item.ModifiedDate,
                    NotApplicable = item.NotApplicable ?? 0,
                    Passed = item.Passed ?? 0,
                    PatientID = item.PatientID,
                    Performed = item.Performed ?? 0,
                    ScrubDate = item.ScrubDate,
                    ScrubHeaderID = item.ScrubHeaderID,
                    Status = item.Status,
                    AssignedByUser = GetNameByUserId(item.AssignedBy),
                    AssignedToUser = GetNameByUserId(item.AssignedTo),
                    ExecutedByUser = GetNameByUserId(item.ExecutedBy),
                    PatientName = GetPatientNameById(Convert.ToInt32(item.PatientID)),
                    BillHeaderStatus = GetBillHeaderStatusIDByBillHeaderId(Convert.ToInt32(item.BillHeaderID)),
                    Section = GetSectionValueByBillHeaderId(Convert.ToInt32(item.BillHeaderID))
                }));
            }

            return list;
        }

        #endregion

        /// <summary>
        /// Gets the scrub report by identifier.
        /// </summary>
        /// <param name="scrubReportId">The scrub report identifier.</param>
        /// <returns></returns>
        public ScrubReport GetScrubReportById(int scrubReportId)
        {
            var item = _rRepository.Where(r => r.ScrubReportID == scrubReportId).FirstOrDefault();
            return item ?? new ScrubReport();
        }

        /// <summary>
        /// Adds the update scrub report.
        /// </summary>
        /// <param name="scrubreportobj">The scrubreportobj.</param>
        /// <returns>INT</returns>
        public int AddUpdateScrubReport(ScrubReport scrubreportobj)
        {
            if (scrubreportobj.ScrubReportID > 0)
            {
                _rRepository.UpdateEntity(scrubreportobj, scrubreportobj.ScrubReportID);
            }
            else
            {
                _rRepository.Create(scrubreportobj);
            }

            return scrubreportobj.ScrubReportID;
        }

        /// <summary>
        /// Gets the scrub header by identifier.
        /// </summary>
        /// <param name="scrubHeaderid">The scrub headerid.</param>
        /// <returns>Scrub Header Class Obj</returns>
        public ScrubHeader GetScrubHeaderById(int scrubHeaderid)
        {
            var item = _repository.Where(r => r.ScrubHeaderID == scrubHeaderid).FirstOrDefault();
            return item ?? new ScrubHeader();
        }

        /// <summary>
        /// Sets the corrected diagnosis.
        /// </summary>
        /// <param name="corporateid">The corporateid.</param>
        /// <param name="facilityid">The facilityid.</param>
        /// <param name="patientid">The patientid.</param>
        /// <param name="encounterid">The encounterid.</param>
        /// <param name="loggedinUserId">The loggedin user identifier.</param>
        /// <param name="diagnosisCode">The diagnosis code.</param>
        /// <returns>INT</returns>
        public int SetCorrectedDiagnosis(int corporateid, int facilityid, int patientid, int encounterid, int loggedinUserId, string diagnosisCode)
        {
            var spName = string.Format("EXEC {0} @pCorporateID, @FacilityID, @PatientID, @EncounterID, @CreatedBy, @pDiagnosisCode ", StoredProcedures.SPROC_SetCorrectedDiagnosis.ToString());
            var sqlParameters = new SqlParameter[6];
            sqlParameters[0] = new SqlParameter("pCorporateID", corporateid);
            sqlParameters[1] = new SqlParameter("FacilityID", facilityid);
            sqlParameters[2] = new SqlParameter("PatientID", patientid);
            sqlParameters[3] = new SqlParameter("EncounterID", encounterid);
            sqlParameters[4] = new SqlParameter("CreatedBy", loggedinUserId);
            sqlParameters[5] = new SqlParameter("pDiagnosisCode", diagnosisCode);
            IEnumerable<ScrubCorrectionModel> result = _context.Database.SqlQuery<ScrubCorrectionModel>(spName, sqlParameters);

            var retStatus = result.ToList();
            if (retStatus.Any())
            {
                var returnStatus = retStatus.FirstOrDefault();
                return returnStatus != null
                    ? returnStatus.RetStatus
                    : 100;
            }


            return 100;
        }

        /// <summary>
        /// Gets the CSS class by global code identifier.
        /// </summary>
        /// <param name="datatype">
        /// The datatype.
        /// </param>
        /// <returns>
        /// String
        /// </returns>
        internal string GetCssClassByGlobalCodeId(string datatype)
        {
            var cssClass = "validate[required]";
            var codeType = (RuleStepDataType)Enum.Parse(typeof(RuleStepDataType), datatype);

            switch (codeType)
            {
                case RuleStepDataType.STRING:
                case RuleStepDataType.BIT:
                    break;
                case RuleStepDataType.DATETIME:
                    cssClass += " datetime";
                    break;
                case RuleStepDataType.DECIMAL:
                    cssClass = "validate[required,custom[number]]";
                    break;
                case RuleStepDataType.INT:
                    cssClass = "validate[required,custom[integer]]";
                    break;
                default:
                    break;
            }

            cssClass += " correctedValue";
            return cssClass;
        }

        public List<ScrubHeaderCustomModel> GetScrubSummaryList(int corporateId, int facilityId, DateTime? fromDate, DateTime? tillDate)
        {
            var spName = string.Format("EXEC {0} @pCorporateID, @pFacilityID, @pdtFrom,@pdtTill", StoredProcedures.SPROC_GetScrubberSummaryReport);
            var sqlParameters = new SqlParameter[4];
            sqlParameters[0] = new SqlParameter("pCorporateID", corporateId);
            sqlParameters[1] = new SqlParameter("pFacilityID", facilityId);
            sqlParameters[2] = new SqlParameter("pdtFrom", fromDate);
            sqlParameters[3] = new SqlParameter("pdtTill", tillDate);
            IEnumerable<ScrubHeaderCustomModel> result = _context.Database.SqlQuery<ScrubHeaderCustomModel>(spName, sqlParameters);
            return result.ToList();
        }


        public List<ScrubHeaderCustomModel> GetErrorDetailByRuleCode(int corporateId, int facilityId, DateTime? fromDate,
            DateTime? tillDate)
        {
            var spName = string.Format("EXEC {0} @pCorporateID, @pFacilityID, @pdtFrom, @pdtTill", StoredProcedures.SPROC_GetErrorDetailReportByRuleCode);
            var sqlParameters = new SqlParameter[4];
            sqlParameters[0] = new SqlParameter("pCorporateID", corporateId);
            sqlParameters[1] = new SqlParameter("pFacilityID", facilityId);
            sqlParameters[2] = new SqlParameter("pdtFrom", fromDate);
            sqlParameters[3] = new SqlParameter("pdtTill", tillDate);
            IEnumerable<ScrubHeaderCustomModel> result = _context.Database.SqlQuery<ScrubHeaderCustomModel>(spName, sqlParameters);
            return result.ToList();
        }

        public List<ScrubHeaderCustomModel> GetErrorSummaryByRuleCode(int corporateId, int facilityId, DateTime? fromDate,
            DateTime? tillDate)
        {
            var spName = string.Format("EXEC {0} @pCorporateID, @pFacilityID, @pdtFrom, @pdtTill", StoredProcedures.SPROC_GetErrorSummaryReportByRuleCode);
            var sqlParameters = new SqlParameter[4];
            sqlParameters[0] = new SqlParameter("pCorporateID", corporateId);
            sqlParameters[1] = new SqlParameter("pFacilityID", facilityId);
            sqlParameters[2] = new SqlParameter("pdtFrom", fromDate);
            sqlParameters[3] = new SqlParameter("pdtTill", tillDate);
            IEnumerable<ScrubHeaderCustomModel> result = _context.Database.SqlQuery<ScrubHeaderCustomModel>(spName, sqlParameters);
            return result.ToList();
        }


        /// <summary>
        /// Gets the scrub header by encounter. 
        /// Created for unit test cases check
        /// Created on 4th March 2016
        /// </summary>
        /// <param name="encounterid">The encounterid.</param>
        /// <returns></returns>
        public List<ScrubHeaderCustomModel> GetScrubHeaderByEncounter(int encounterid)
        {

            var list = new List<ScrubHeaderCustomModel>();
            var scrubSummary = _repository.Where(x => x.EncounterID == encounterid).ToList();
            if (scrubSummary.Count > 0)
                list = MapModelToViewModel(scrubSummary);
            return list;
        }

    }
}