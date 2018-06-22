// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScrubReportBal.cs" company="Spadez">
//   Omni
// </copyright>
// <summary>
//   Scrub Report Bal Inherits Base bal class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Bal.BusinessAccess
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using BillingSystem.Bal.Mapper;
    using BillingSystem.Common.Common;
    using BillingSystem.Model;
    using BillingSystem.Model.CustomModel;

    using Elmah.ContentSyndication;

    /// <summary>
    /// Scrub Report Bal Inherits Base bal class
    /// </summary>
    public class ScrubReportBal : BaseBal
    {
        /// <summary>
        /// Gets or sets the scrub report mapper.
        /// </summary>
        /// <value>
        /// The scrub report mapper.
        /// </value>
        private ScrubReportMapper ScrubReportMapper { get; set; }
        private ScrubHeaderMapper ScrubHeaderMapper { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScrubReportBal"/> class.
        /// </summary>
        public ScrubReportBal()
        {
            ScrubReportMapper = new ScrubReportMapper();
            ScrubHeaderMapper = new ScrubHeaderMapper();
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="ScrubReportBal"/> class.
        /// </summary>
        /// <param name="billEditRuleTableNumber">The bill edit rule table number.</param>
        public ScrubReportBal(string billEditRuleTableNumber)
        {
            if (!string.IsNullOrEmpty(billEditRuleTableNumber))
                BillEditRuleTableNumber = billEditRuleTableNumber;

            ScrubReportMapper = new ScrubReportMapper();
        }

        //public List<ScrubReportCustomModel> GetScrubReportByCorporateAndFacility(int corporateId, int facilityId, int billHeaderId, int loggedInUserId)
        //{
        //    try
        //    {
        //        var list = new List<ScrubReportCustomModel>();
        //        using (var rep = UnitOfWork.ScrubReportRepository)
        //        {
        //            //Call SP for Scrub Billing
        //            var result = rep.ApplyScrubBill(billHeaderId, loggedInUserId);

        //            var lstScrubReport = (corporateId > 0 && facilityId > 0) ? rep.Where(a => (a.IsActive == null || !(bool)a.IsActive) && (a.CorporateID != null && (int)a.CorporateID == corporateId) && (a.FacilityID != null && (int)a.FacilityID == facilityId) && (int)a.BillHeaderID == billHeaderId).ToList()
        //                : rep.Where(a => a.IsActive != null && (bool)a.IsActive && (int)a.BillHeaderID == billHeaderId).ToList();

        //            if (lstScrubReport.Count > 0)
        //            {
        //                list.AddRange(lstScrubReport.Select(item => new ScrubReportCustomModel
        //                {
        //                    ErrorCode = item.ErrorCode,
        //                    ErrorDescription = item.ErrorDescription,
        //                    ScrubReportID = item.ScrubReportID,
        //                    CreatedBy = item.CreatedBy,
        //                    CreatedDate = item.CreatedDate,
        //                    ScrubDate = item.ScrubDate,
        //                    Status = item.Status,
        //                    ErrorType = item.ErrorType,
        //                    ExtValue1 = item.ExtValue1,
        //                    ExtValue2 = item.ExtValue2,
        //                    ExtValue3 = item.ExtValue3,
        //                    ExtValue4 = item.ExtValue4,
        //                    FacilityID = item.FacilityID,
        //                    IsActive = item.IsActive,
        //                    ModifiedBy = item.ModifiedBy,
        //                    ModifiedDate = item.ModifiedDate,
        //                    BillHeaderID = item.BillHeaderID,
        //                    RuleCode = item.RuleCode,
        //                    RuleDescription = item.RuleDescription,
        //                    RuleMasterID = item.RuleMasterID,
        //                    CorporateID = item.CorporateID,
        //                    EncounterID = item.EncounterID,
        //                    PatientID = item.PatientID,
        //                    ErrorID = item.ErrorID
        //                }));
        //            }
        //        }
        //        return list;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

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
        public List<ScrubHeaderCustomModel> GetScrubHeaderList(
            int corporateId,
            int facilityId,
            int billHeaderId,
            int userId,
            bool createscrub)
        {
            var list = new List<ScrubHeaderCustomModel>();
            using (var rep = UnitOfWork.ScrubHeaderRepository)
            {
                ////Call SP for Scrub Billing
                if (createscrub)
                    rep.ApplyScrubBill(corporateId, facilityId, userId);


                var headerList = rep.GetSrubHeaderDetail(corporateId, facilityId, userId);
                if (headerList.Count > 0)
                {
                    if (billHeaderId > 0)
                        headerList = headerList.Where(a => a.BillHeaderID != null && a.BillHeaderID == billHeaderId).ToList();

                    list.AddRange(headerList.Select(item => ScrubHeaderMapper.MapModelToViewModel(item)));

                    //list.AddRange(headerList.Select(item => new ScrubHeaderCustomModel
                    //{
                    //    AssignedBy = item.AssignedBy,
                    //    AssignedDate = item.AssignedDate,
                    //    AssignedTo = item.AssignedTo,
                    //    BillHeaderID = item.BillHeaderID,
                    //    CorporateID = item.CorporateID,
                    //    EncounterID = item.EncounterID,
                    //    ExecutedBy = item.ExecutedBy,
                    //    ExtValue1 = item.ExtValue1,
                    //    ExtValue2 = item.ExtValue2,
                    //    ExtValue3 = item.ExtValue3,
                    //    ExtValue4 = item.ExtValue4,
                    //    FacilityID = item.FacilityID,
                    //    Failed = item.Failed ?? 0,
                    //    IsActive = item.IsActive,
                    //    ModifiedBy = item.ModifiedBy,
                    //    ModifiedDate = item.ModifiedDate,
                    //    NotApplicable = item.NotApplicable ?? 0,
                    //    Passed = item.Passed ?? 0,
                    //    PatientID = item.PatientID,
                    //    Performed = item.Performed ?? 0,
                    //    ScrubDate = item.ScrubDate,
                    //    ScrubHeaderID = item.ScrubHeaderID,
                    //    Status = item.Status,
                    //    AssignedByUser = GetNameByUserId(item.AssignedBy),
                    //    AssignedToUser = GetNameByUserId(item.AssignedTo),
                    //    ExecutedByUser = GetNameByUserId(item.ExecutedBy),
                    //    PatientName = GetPatientNameById(Convert.ToInt32(item.PatientID)),
                    //    BillHeaderStatus = GetBillHeaderStatusIDByBillHeaderId(Convert.ToInt32(item.BillHeaderID)),
                    //    Section = GetSectionValueByBillHeaderId(Convert.ToInt32(item.BillHeaderID)),
                    //    BillPenality = item.BillPenality,
                    //    BillDaysLeftToEClaim = item.BillDaysLeftToEClaim,
                    //    EncounterNumber = item.EncounterNumber,
                    //    EncounterPatientType = item.EncounterPatientType
                    //}));
                }
            }
            return list;
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
                using (var rep = UnitOfWork.ScrubReportRepository)
                {
                    var lstScrubReport = reportType == 999
                        ? rep.Where(a => a.IsActive != null && (bool)a.IsActive && a.ScrubHeaderID == scrubHeaderId)
                            .ToList()
                        : rep.Where(
                            a =>
                                a.IsActive != null && (bool)a.IsActive && a.ScrubHeaderID == scrubHeaderId &&
                                a.Status == reportType).ToList();

                    if (lstScrubReport.Count > 0)
                    {
                        list.AddRange(lstScrubReport.Select(item => ScrubReportMapper.MapModelToViewModel(item)));

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
                }

                return list;
            }
            catch (Exception )
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
        public ScrubReportCustomModel GetScrubReportDetailById(int scrubReportId)
        {
            var current = new ScrubReportCustomModel();
            using (var rep = UnitOfWork.ScrubReportRepository)
            {
                var item = rep.Where(r => r.ScrubReportID == scrubReportId).FirstOrDefault();
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
                        RuleMasterDesc = GetDescByRuleMasterId(Convert.ToInt32(item.RuleMasterID)),
                        RuleStepDesc = GetDescByRuleStepId(Convert.ToInt32(item.RuleStepID)),
                        ErrorText = GetDescByErrorId(Convert.ToInt32(item.ErrorID)),

                        // PatientId = billactiviyBal.GetPatientIdByBillActivityId(Convert.ToInt32(item.BillActivityID)),
                        // EncounterId = billactiviyBal.GetEncounterIdByBillActivityId(Convert.ToInt32(item.BillActivityID)),
                    };

                    using (var ruleRep = UnitOfWork.RuleStepRepository)
                    {
                        var ruleStep =
                            ruleRep.Where(
                                r => r.RuleStepID == current.RuleStepID && r.RuleMasterID == current.RuleMasterID)
                                .FirstOrDefault();
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
                                using (var rulemasterBal = new RuleMasterBal(BillEditRuleTableNumber))
                                {
                                    // get rule master by rule master id to get the rule code
                                    var rulemasterDetail = rulemasterBal.GetRuleMasterById(current.RuleMasterID);
                                    if (rulemasterDetail != null)
                                    {
                                        using (var errormasterrep = UnitOfWork.ErrorMasterRepository)
                                        {
                                            var errormaster =
                                                errormasterrep.Where(e => e.ErrorCode == rulemasterDetail.RuleCode)
                                                    .FirstOrDefault();

                                            // compare the rule code with error code and get the error resolution
                                            current.ErrorResolutionTxt = errormaster != null
                                                ? errormaster.ErrorResolution
                                                : string.Empty;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return current;
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
        public bool UpdateScrubReportDetailWithCorrection(
            int scrubReportId,
            int scrubHeaderId,
            string lhsValue,
            string rhsValue,
            int loggedinUserId,
            int corporateId,
            int facilityid,
            string correctionCodeId)
        {
            var status = false;
            using (var rep = UnitOfWork.ScrubReportRepository)
            {
                status = rep.ScrubReportDetailCorrections(
                    scrubReportId,
                    scrubHeaderId,
                    lhsValue,
                    rhsValue,
                    loggedinUserId,
                    corporateId,
                    facilityid,
                    correctionCodeId);
            }

            return status;
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
            using (var rep = UnitOfWork.ScrubHeaderRepository)
            {
                var model = rep.Where(h => h.ScrubHeaderID == scrubHeaderId).FirstOrDefault();
                if (model != null)
                {
                    model.AssignedBy = loggedUserId;
                    model.AssignedTo = assignedTo;
                    model.AssignedDate = assignedDate;
                    rep.Update(model);
                    var user = GetNameByUserId(model.AssignedTo);
                    return user;
                }
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
            using (var rep = UnitOfWork.ScrubHeaderRepository)
            {
                var result = rep.ApplyScrubBillToSpecificBillHeaderId(billHeaderId, loggedUserId);
                return result;
            }
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
            using (var rep = UnitOfWork.ScrubHeaderRepository)
            {
                ////Call SP for Scrub Billing
                if (createscrub)
                {
                    rep.ApplyScrubBill(corporateId, facilityId, userId);
                }

                // var headerList = (corporateId > 0 && facilityId > 0) ? rep.Where(a => (a.IsActive != null && (bool)a.IsActive) && (int)a.CorporateID == corporateId && (int)a.FacilityID == facilityId && (int)a.BillHeaderID == billHeaderId).ToList() : rep.Where(a => (a.IsActive != null && (bool)a.IsActive) && (int)a.BillHeaderID == billHeaderId).ToList();
                var headerList = (corporateId > 0 && facilityId > 0)
                    ? rep.Where(a => (a.IsActive != null && (bool)a.IsActive)
                                     && (int)a.CorporateID == corporateId && (int)a.FacilityID == facilityId)
                        .GroupBy(g => g.BillHeaderID)
                        .Select(gg => gg.OrderByDescending(p => p.ScrubDate).FirstOrDefault())
                        .ToList()
                    : rep.Where(a => (a.IsActive != null && (bool)a.IsActive))
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
            using (var rep = UnitOfWork.ScrubReportRepository)
            {
                var item = rep.Where(r => r.ScrubReportID == scrubReportId).FirstOrDefault();
                return item ?? new ScrubReport();
            }
        }

        /// <summary>
        /// Adds the update scrub report.
        /// </summary>
        /// <param name="scrubreportobj">The scrubreportobj.</param>
        /// <returns>INT</returns>
        public int AddUpdateScrubReport(ScrubReport scrubreportobj)
        {
            using (var scrubReportRep = UnitOfWork.ScrubReportRepository)
            {
                if (scrubreportobj.ScrubReportID > 0)
                {
                    scrubReportRep.UpdateEntity(scrubreportobj, scrubreportobj.ScrubReportID);
                }
                else
                {
                    scrubReportRep.Create(scrubreportobj);
                }

                return scrubreportobj.ScrubReportID;
            }
        }

        /// <summary>
        /// Gets the scrub header by identifier.
        /// </summary>
        /// <param name="scrubHeaderid">The scrub headerid.</param>
        /// <returns>Scrub Header Class Obj</returns>
        public ScrubHeader GetScrubHeaderById(int scrubHeaderid)
        {
            using (var rep = UnitOfWork.ScrubHeaderRepository)
            {
                var item = rep.Where(r => r.ScrubHeaderID == scrubHeaderid).FirstOrDefault();
                return item ?? new ScrubHeader();
            }
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
            using (var rep = UnitOfWork.ScrubReportRepository)
            {
                var retStatus = rep.SetCorrectedDiagnosis(corporateid, facilityid, patientid, encounterid, loggedinUserId, diagnosisCode);
                if (retStatus.Any())
                {
                    var returnStatus = retStatus.FirstOrDefault();
                    return returnStatus != null
                        ? returnStatus.RetStatus
                        : 100;
                }
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

            var list = new List<ScrubHeaderCustomModel>();
            using (var rep = UnitOfWork.ScrubHeaderRepository)
            {
                var scrubSummary = rep.GetScrubSummaryDetail(corporateId, facilityId, fromDate, tillDate);
                if (scrubSummary.Count > 0)
                    list.AddRange(scrubSummary.Select(item => ScrubHeaderMapper.MapModelToViewModel(item)));
            }
            return list;
        }


        public List<ScrubHeaderCustomModel> GetErrorDetailByRuleCode(int corporateId, int facilityId, DateTime? fromDate,
            DateTime? tillDate)
        {
            var list = new List<ScrubHeaderCustomModel>();
            using (var rep = UnitOfWork.ScrubHeaderRepository)
            {
                //var scrubSummary = rep.GetErrorDetailByRuleCode(corporateId, facilityId, fromDate, tillDate);
                //if (scrubSummary.Count > 0)
                  //list.AddRange(scrubSummary.Select(item => ScrubHeaderMapper.MapModelToViewModel(item)));
                list =  rep.GetErrorDetailByRuleCode(corporateId, facilityId, fromDate, tillDate);
               
            }
            return list;
        }

        public List<ScrubHeaderCustomModel> GetErrorSummaryByRuleCode(int corporateId, int facilityId, DateTime? fromDate,
            DateTime? tillDate)
        {
            var list = new List<ScrubHeaderCustomModel>();
            using (var rep = UnitOfWork.ScrubHeaderRepository)
            {
                //var scrubSummary = rep.GetErrorSummaryByRuleCode(corporateId, facilityId, fromDate, tillDate);
                //if (scrubSummary.Count > 0)
                //    list.AddRange(scrubSummary.Select(item => ScrubHeaderMapper.MapModelToViewModel(item)));
                list = rep.GetErrorSummaryByRuleCode(corporateId, facilityId, fromDate, tillDate);

            }
            return list;
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
            using (var rep = UnitOfWork.ScrubHeaderRepository)
            {
                var scrubSummary = rep.Where(x => x.EncounterID == encounterid).ToList();
                if (scrubSummary.Count > 0)
                    list.AddRange(scrubSummary.Select(item => ScrubHeaderMapper.MapModelToViewModel(item)));
            }
            return list;
        }

    }
}