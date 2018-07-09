using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using System.Transactions;
using BillingSystem.Common.Common;

using System.Data.SqlClient;

using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class BillHeaderService : IBillHeaderService
    {
        private readonly IRepository<BillHeader> _repository;
        private readonly BillingEntities _context;

        public BillHeaderService(IRepository<BillHeader> repository, BillingEntities context)
        {
            _repository = repository;
            _context = context;
        }

        private string GetNameByGlobalCodeValue(string codeValue, string categoryValue, string fId = "")
        {
            if (!string.IsNullOrEmpty(codeValue))
            {
                var gl = _context.GlobalCodes.Where(g => g.GlobalCodeValue.Equals(codeValue) && !g.IsDeleted.Value && g.GlobalCodeCategoryValue.Equals(categoryValue) && (string.IsNullOrEmpty(fId) || g.FacilityNumber.Equals(fId))).FirstOrDefault();
                return gl != null ? gl.GlobalCodeName : string.Empty;
            }
            return string.Empty;
        }
        private string GetCodeDescription(string orderCode, string orderType, string CptTableNumber, string DrgTableNumber, string HcpcsTableNumber, string DrugTableNumber,
          string ServiceCodeTableNumber, string DiagnosisTableNumber)
        {
            var codeDescription = string.Empty;

            if (!string.IsNullOrEmpty(orderCode) && !string.IsNullOrEmpty(orderType))
            {
                var codeType = (OrderType)Enum.Parse(typeof(OrderType), orderType);
                switch (codeType)
                {
                    case OrderType.CPT:
                        codeDescription = _context.CPTCodes.Where(x => x.CodeNumbering.Contains(orderCode) && x.CodeTableNumber.Trim().Equals(CptTableNumber)).FirstOrDefault().CodeDescription;
                        return codeDescription;
                    case OrderType.DRG:
                        codeDescription = _context.DRGCodes.Where(d => d.CodeNumbering == orderCode && d.CodeTableNumber.Trim().Equals(DrgTableNumber)).FirstOrDefault().CodeDescription;
                        return codeDescription;
                    case OrderType.HCPCS:
                        codeDescription = _context.HCPCSCodes.Where(x => x.CodeNumbering == orderCode && x.CodeTableNumber.Trim().Equals(HcpcsTableNumber)).FirstOrDefault().CodeDescription;
                        return codeDescription;
                    case OrderType.DRUG:
                        codeDescription = _context.Drug.Where(x => x.DrugCode == orderCode && x.DrugTableNumber.Trim().Equals(DrugTableNumber)).FirstOrDefault().DrugDescription;
                        return codeDescription;
                    case OrderType.BedCharges:
                    case OrderType.ServiceCode:
                        codeDescription = _context.ServiceCode.Where(s => s.ServiceCodeValue.Equals(orderCode) && s.ServiceCodeTableNumber.Trim().Equals(ServiceCodeTableNumber)).FirstOrDefault().ServiceCodeDescription;
                        return codeDescription;
                    case OrderType.Diagnosis:
                        codeDescription = _context.DiagnosisCode.Where(d => d.DiagnosisCode1 == orderCode && d.DiagnosisTableNumber.Trim().Equals(DiagnosisTableNumber)).FirstOrDefault().DiagnosisFullDescription;
                        return codeDescription;
                    default:
                        break;
                }
            }
            return codeDescription;
        }

        #region Generate Preliminary Bill Overview


        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns>
        /// Return the Entity List
        /// </returns>
        public List<BillHeaderCustomModel> GetBillHeaderListByEncounterId(int encounterId, int corporateId, int facilityId)
        {
            try
            {
                var list = new List<BillHeaderCustomModel>();
                var lstBillHeader = corporateId > 0 ?
                    _repository.Where(a => (a.IsDeleted == null || !(bool)a.IsDeleted && a.CorporateID != null && (int)a.CorporateID == corporateId && (int)a.FacilityID == facilityId)
                    && a.EncounterID == encounterId).OrderByDescending(b => b.BillDate).ToList() :
                    _repository.Where(a => (a.IsDeleted == null || !(bool)a.IsDeleted)
                    && a.EncounterID == encounterId).OrderByDescending(b => b.BillDate).ToList();

                if (lstBillHeader.Count > 0)
                {
                    list.AddRange(lstBillHeader.Select(i => new BillHeaderCustomModel
                    {
                        BillHeaderID = i.BillHeaderID,
                        BillNumber = i.BillNumber,
                        BillDate = i.BillDate,
                        CorporateID = i.CorporateID,
                        FacilityID = i.FacilityID,
                        PatientID = i.PatientID,
                        EncounterID = i.EncounterID,
                        PayerID = i.PayerID,
                        MemberID = i.MemberID,
                        Gross = i.Gross,
                        PatientShare = i.PatientShare,
                        PayerShareNet = i.PayerShareNet,
                        Status = GetNameByGlobalCodeValue(i.Status, Convert.ToInt32(GlobalCodeCategoryValue.BillHeaderStatus).ToString()),
                        DenialCode = i.DenialCode,
                        PaymentReference = i.PaymentReference,
                        DateSettlement = i.DateSettlement,
                        PaymentAmount = i.PaymentAmount,
                        PatientPayReference = i.PatientPayReference,
                        PatientDateSettlement = i.PatientDateSettlement,
                        PatientPayAmount = i.PatientPayAmount,
                        ClaimID = i.ClaimID,
                        FileID = i.FileID,
                        ARFileID = i.ARFileID,
                        CreatedBy = i.CreatedBy,
                        CreatedDate = i.CreatedDate,
                        ModifiedBy = i.ModifiedBy,
                        ModifiedDate = i.ModifiedDate,
                        IsDeleted = i.IsDeleted,
                        DeletedBy = i.DeletedBy,
                        DeletedDate = i.DeletedDate,
                        AuthID = i.AuthID,
                        AuthCode = i.AuthCode,
                        MCID = i.MCID,
                        MCPatientShare = i.MCPatientShare,
                        MCMultiplier = i.MCMultiplier,
                        CorporateName = GetNameByCorporateId(Convert.ToInt32(i.CorporateID)),
                        FacilityName = GetFacilityNameByFacilityId(Convert.ToInt32(i.FacilityID)),
                        EncounterNumber = encounterId > 0 ? _context.Encounter.FirstOrDefault(a => a.EncounterID == encounterId).EncounterNumber : string.Empty,
                        InsuranceCompany = string.IsNullOrEmpty(i.PayerID) ? "-" : GetInsuranceCompanyNameByPayerId(Convert.ToInt32(i.PayerID)),
                        PatientName = GetPatientNameById(Convert.ToInt32(i.PatientID)),
                        BStatus = Convert.ToInt32(i.Status),
                        BillHeaderStatus = GetNameByGlobalCodeValue(i.Status, Convert.ToInt32(GlobalCodeCategoryValue.BillHeaderStatus).ToString()),
                        MCDiscount = i.MCDiscount,
                        GrossChargesSum = i.PatientShare + i.PayerShareNet,
                        EncounterStatus = GetEncounterStatusById(i.EncounterID),
                        ActivityCost = i.ActivityCost
                    }));
                    list = list.Where(a => a.BStatus <= 45).ToList();
                }
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private string GetEncounterStatusById(int? encounterID)
        {
            var m = _context.Encounter.Where(e => e.EncounterID == Convert.ToInt32(encounterID)).FirstOrDefault();
            return m == null ? m.EncounterEndTime != null ? "Ended" : "Active" : "NA";
        }
        private string GetEncounterNumberById(int? encounterID)
        {
            var m = _context.Encounter.Where(e => e.EncounterID == Convert.ToInt32(encounterID)).FirstOrDefault();
            return m != null ? m.EncounterNumber : string.Empty;
        }
        private string GetNameByCorporateId(int corporateID)
        {
            var m = _context.Corporate.FirstOrDefault(a => a.CorporateID == corporateID);
            return m != null ? m.CorporateName : string.Empty;
        }

        private string GetFacilityNameByFacilityId(int facilityId)
        {
            var m = _context.Facility.FirstOrDefault(a => a.FacilityId == facilityId);
            return m != null ? m.FacilityName : string.Empty;
        }
        private string GetPatientNameById(int PatientID)
        {
            var m = _context.PatientInfo.FirstOrDefault(p => p.PatientID == PatientID);
            return m != null ? $"{m.PersonFirstName} {m.PersonLastName}" : string.Empty;
        }

        private string GetInsuranceCompanyNameByPayerId(int PayerID)
        {
            var m = _context.InsuranceCompany.FirstOrDefault(x => x.InsuranceCompanyLicenseNumber.Equals(PayerID));
            return m != null ? m.InsuranceCompanyName : string.Empty;
        }

        /// <summary>
        /// Gets the bill header list by patient identifier.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public List<BillHeaderCustomModel> GetBillHeaderListByPatientId(int patientId, int corporateId, int facilityId)
        {
            try
            {
                var list = new List<BillHeaderCustomModel>();
                var lstBillHeader = corporateId > 0
                    ? _repository.Where(
                        a =>
                            (a.IsDeleted == null ||
                             !(bool)a.IsDeleted && a.CorporateID != null && (int)a.CorporateID == corporateId &&
                             (int)a.FacilityID == facilityId)
                            && (int)a.PatientID == patientId).OrderByDescending(b => b.BillHeaderID).ToList()
                    : _repository.Where(a => (a.IsDeleted == null || !(bool)a.IsDeleted)
                                               && (int)a.PatientID == patientId)
                        .OrderByDescending(b => b.BillHeaderID)
                        .ToList();

                if (lstBillHeader.Count > 0)
                {
                    list.AddRange(lstBillHeader.Select(i => new BillHeaderCustomModel
                    {
                        BillHeaderID = i.BillHeaderID,
                        BillNumber = i.BillNumber,
                        BillDate = i.BillDate,
                        CorporateID = i.CorporateID,
                        FacilityID = i.FacilityID,
                        PatientID = i.PatientID,
                        EncounterID = i.EncounterID,
                        PayerID = i.PayerID,
                        MemberID = i.MemberID,
                        Gross = i.Gross,
                        PatientShare = i.PatientShare,
                        PayerShareNet = i.PayerShareNet,
                        Status = GetNameByGlobalCodeValue(i.Status, Convert.ToInt32(GlobalCodeCategoryValue.BillHeaderStatus).ToString()),
                        DenialCode = i.DenialCode,
                        PaymentReference = i.PaymentReference,
                        DateSettlement = i.DateSettlement,
                        PaymentAmount = i.PaymentAmount,
                        PatientPayReference = i.PatientPayReference,
                        PatientDateSettlement = i.PatientDateSettlement,
                        PatientPayAmount = i.PatientPayAmount,
                        ClaimID = i.ClaimID,
                        FileID = i.FileID,
                        ARFileID = i.ARFileID,
                        CreatedBy = i.CreatedBy,
                        CreatedDate = i.CreatedDate,
                        ModifiedBy = i.ModifiedBy,
                        ModifiedDate = i.ModifiedDate,
                        IsDeleted = i.IsDeleted,
                        DeletedBy = i.DeletedBy,
                        DeletedDate = i.DeletedDate,
                        AuthID = i.AuthID,
                        AuthCode = i.AuthCode,
                        MCID = i.MCID,
                        MCPatientShare = i.MCPatientShare,
                        MCMultiplier = i.MCMultiplier,
                        CorporateName = GetNameByCorporateId(Convert.ToInt32(i.CorporateID)),
                        FacilityName = GetFacilityNameByFacilityId(Convert.ToInt32(i.FacilityID)),
                        EncounterNumber = GetEncounterNumberById(patientId),
                        PatientName = GetPatientNameById(Convert.ToInt32(i.PatientID)),
                        BStatus = Convert.ToInt32(i.Status),
                        MCDiscount = i.MCDiscount,
                        GrossChargesSum = i.PatientShare + i.PayerShareNet,
                        EncounterStatus = GetEncounterStatusById(Convert.ToInt32(i.EncounterID)),
                        InsuranceCompany = string.IsNullOrEmpty(i.PayerID) ? "-" : GetInsuranceCompanyNameByPayerId(Convert.ToInt32(i.PayerID)),
                        ActivityCost = i.ActivityCost
                    }));

                    list = list.Where(a => a.BStatus <= 45).ToList();
                }

                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets all bill header list.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public List<BillHeaderCustomModel> GetAllBillHeaderList(int corporateId, int facilityId)
        {
            try
            {
                var list = new List<BillHeaderCustomModel>();
                var spName = string.Format("EXEC {0} @lFID,@lCID ", StoredProcedures.SPROC_BillHeaderList);
                var sqlParameters = new SqlParameter[2];
                sqlParameters[0] = new SqlParameter(InputParams.lFID.ToString(), facilityId);
                sqlParameters[1] = new SqlParameter(InputParams.lCID.ToString(), corporateId);
                IEnumerable<BillHeaderCustomModel> result = _context.Database.SqlQuery<BillHeaderCustomModel>(spName, sqlParameters);
                list = result.ToList();
                list = list.Where(a => a.EncounterStatus == "Active").ToList();

                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <returns></returns>
        public BillHeaderCustomModel GetBillHeaderById(int billHeaderId)
        {
            var customModel = new BillHeaderCustomModel();
            var m = _repository.Where(x => x.BillHeaderID == billHeaderId).FirstOrDefault();
            if (m != null)
            {
                customModel = new BillHeaderCustomModel
                {
                    BillHeaderID = m.BillHeaderID,
                    BillNumber = m.BillNumber,
                    BillDate = m.BillDate,
                    CorporateID = m.CorporateID,
                    FacilityID = m.FacilityID,
                    PatientID = m.PatientID,
                    EncounterID = m.EncounterID,
                    PayerID = m.PayerID,
                    MemberID = m.MemberID,
                    Gross = m.Gross,
                    PatientShare = m.PatientShare,
                    PayerShareNet = m.PayerShareNet,
                    Status = GetNameByGlobalCodeValue(m.Status, Convert.ToInt32(GlobalCodeCategoryValue.BillHeaderStatus).ToString()),
                    DenialCode = m.DenialCode,
                    PaymentReference = m.PaymentReference,
                    DateSettlement = m.DateSettlement,
                    PaymentAmount = m.PaymentAmount,
                    PatientPayReference = m.PatientPayReference,
                    PatientDateSettlement = m.PatientDateSettlement,
                    PatientPayAmount = m.PatientPayAmount,
                    ClaimID = m.ClaimID,
                    FileID = m.FileID,
                    ARFileID = m.ARFileID,
                    CreatedBy = m.CreatedBy,
                    CreatedDate = m.CreatedDate,
                    ModifiedBy = m.ModifiedBy,
                    ModifiedDate = m.ModifiedDate,
                    IsDeleted = m.IsDeleted,
                    DeletedBy = m.DeletedBy,
                    DeletedDate = m.DeletedDate,
                    AuthID = m.AuthID,
                    AuthCode = m.AuthCode,
                    MCID = m.MCID,
                    MCPatientShare = m.MCPatientShare,
                    MCMultiplier = m.MCMultiplier,
                    CorporateName = GetNameByCorporateId(Convert.ToInt32(m.CorporateID)),
                    FacilityName = GetFacilityNameByFacilityId(Convert.ToInt32(m.FacilityID)),
                    EncounterNumber = GetEncounterNumberById(Convert.ToInt32(m.EncounterID)),
                    MCDiscount = m.MCDiscount,
                    DueDate = m.DueDate,
                    GrossChargesSum = m.PatientShare + m.PayerShareNet,
                    EncounterStatus = GetEncounterStatusById(Convert.ToInt32(m.EncounterID)),
                    InsuranceCompany = string.IsNullOrEmpty(m.PayerID) ? "-" : GetInsuranceCompanyNameByPayerId(Convert.ToInt32(m.PayerID)),
                    ActivityCost = m.ActivityCost
                };
                var denialcodedesc = GetSearchedDenialsList(m.DenialCode).FirstOrDefault();
                if (denialcodedesc != null)
                {
                    customModel.DenialCodeDescritption = string.Format("{0} - {1}", denialcodedesc.ErrorCode,
                        denialcodedesc.ErrorDescription);
                }
            }
            return customModel;
        }
        private IEnumerable<ErrorMasterCustomModel> GetSearchedDenialsList(string text)
        {
            var list = new List<ErrorMasterCustomModel>();
            text = string.IsNullOrEmpty(text) ? string.Empty : text.ToLower().Trim();
            var lstErrorMaster = _context.ErrorMaster.Where(a => a.IsActive != null && (bool)a.IsActive && (a.ErrorDescription.ToLower().Contains(text) || a.ErrorCode.ToLower().Contains(text))).ToList();
            if (lstErrorMaster.Count > 0)
            {
                list.AddRange(lstErrorMaster.Select(item => new ErrorMasterCustomModel
                {
                    ErrorCode = item.ErrorCode,
                    ErrorDescription = item.ErrorDescription,
                    ErrorMasterID = item.ErrorMasterID,
                    CreatedBy = item.CreatedBy,
                    CreatedDate = item.CreatedDate,
                    ErrorResolution = item.ErrorResolution,
                    ErrorType = item.ErrorType,
                    ExtValue1 = item.ExtValue1,
                    ExtValue2 = item.ExtValue2,
                    ExtValue3 = item.ExtValue3,
                    ExtValue4 = item.ExtValue4,
                    FacilityID = item.FacilityID,
                    IsActive = item.IsActive,
                    ModifiedBy = item.ModifiedBy,
                    ModifiedDate = item.ModifiedDate
                }));


            }
            return list;
        }

        /// <summary>
        /// Gets the bill header detail by current encounter.
        /// </summary>
        /// <param name="mostRecentEncounterId">The most recent encounter identifier.</param>
        /// <returns></returns>
        public BillHeaderCustomModel GetBillHeaderDetailByCurrentEncounter(int mostRecentEncounterId)
        {
            BillHeaderCustomModel bHeader = null;
            var m = _repository.Where(x => x.EncounterID == mostRecentEncounterId).FirstOrDefault();
            if (m != null)
            {
                bHeader = new BillHeaderCustomModel
                {
                    BillHeaderID = m.BillHeaderID,
                    BillNumber = m.BillNumber,
                    BillDate = m.BillDate,
                    CorporateID = m.CorporateID,
                    FacilityID = m.FacilityID,
                    PatientID = m.PatientID,
                    EncounterID = m.EncounterID,
                    PayerID = m.PayerID,
                    MemberID = m.MemberID,
                    Gross = m.Gross,
                    PatientShare = m.PatientShare,
                    PayerShareNet = m.PayerShareNet,
                    Status = m.Status,
                    DenialCode = m.DenialCode,
                    PaymentReference = m.PaymentReference,
                    DateSettlement = m.DateSettlement,
                    PaymentAmount = m.PaymentAmount,
                    PatientPayReference = m.PatientPayReference,
                    PatientDateSettlement = m.PatientDateSettlement,
                    PatientPayAmount = m.PatientPayAmount,
                    ClaimID = m.ClaimID,
                    FileID = m.FileID,
                    ARFileID = m.ARFileID,
                    CreatedBy = m.CreatedBy,
                    CreatedDate = m.CreatedDate,
                    ModifiedBy = m.ModifiedBy,
                    ModifiedDate = m.ModifiedDate,
                    IsDeleted = m.IsDeleted,
                    DeletedBy = m.DeletedBy,
                    DeletedDate = m.DeletedDate,
                    CorporateName = GetNameByCorporateId(Convert.ToInt32(m.CorporateID)),
                    FacilityName = GetFacilityNameByFacilityId(Convert.ToInt32(m.FacilityID)),
                    MCDiscount = m.MCDiscount,
                    DueDate = m.DueDate,
                    GrossChargesSum = m.PatientShare + m.PayerShareNet,
                    EncounterStatus = GetEncounterStatusById(Convert.ToInt32(m.EncounterID)),
                    InsuranceCompany = string.IsNullOrEmpty(m.PayerID) ? "-" : GetInsuranceCompanyNameByPayerId(Convert.ToInt32(m.PayerID)),
                    ActivityCost = m.ActivityCost
                };
            }
            return bHeader;
        }

        /// <summary>
        /// Applies the bed charges and order bill.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        public bool ApplyBedChargesAndOrderBill(int encounterId)
        {
            var result = false;
            result = ApplyBedCharges(encounterId);

            var sqlParameters = new SqlParameter[1];
            sqlParameters[0] = new SqlParameter(InputParams.pEncounuterID.ToString(), encounterId);
            _repository.ExecuteCommand(StoredProcedures.SPROC_ApplyOrderToBill.ToString(), sqlParameters);
            //result = _repository.ApplyOrderBill(encounterId);
            return result;
        }
        private bool ApplyBedCharges(int encounterId)
        {
            var sqlParameters = new SqlParameter[1];
            sqlParameters[0] = new SqlParameter(InputParams.pEncounuterID.ToString(), encounterId);
            _repository.ExecuteCommand(StoredProcedures.SPROC_ApplyBedChargesToBill.ToString(), sqlParameters);
            return true;
        }

        /// <summary>
        /// Sets the bill header status.
        /// </summary>
        /// <param name="billHeaderIds">The bill header ids.</param>
        /// <param name="status">The status.</param>
        /// <param name="oldStatus">The old status.</param>
        /// <returns></returns>
        public bool SetBillHeaderStatus(List<int> billHeaderIds, string status, string oldStatus)
        {
            var result = false;
            using (var transScope = new TransactionScope())
            {
                //var list = rep.Where(b => billHeaderIds.Contains(b.BillHeaderID) && b.Status.Equals(oldStatus) && b.AuthID != null && b.MCID != null).ToList();
                // Removed the MCID check to fetch the data.
                var list = _repository.Where(b => billHeaderIds.Contains(b.BillHeaderID) && b.Status.Equals(oldStatus) && b.AuthID != null).ToList();
                if (list.Count > 0)
                {
                    list.Select(b => { b.Status = status; return b; }).ToList();
                    _repository.Update(list);
                    transScope.Complete();
                    result = true;
                }
            }
            return result;
        }

        /// <summary>
        /// Saves the manual payment.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public int SaveManualPayment(BillHeader model)
        {
            var updatedID = _repository.UpdateEntity(model, model.BillHeaderID);
            return updatedID == null ? 0 : Convert.ToInt32(updatedID);
        }

        /// <summary>
        /// Gets the bill header to update by identifier.
        /// </summary>
        /// <param name="billheaderid">The billheaderid.</param>
        /// <returns></returns>
        public BillHeader GetBillHeaderToUpdateById(int billheaderid)
        {
            var model = _repository.Where(x => x.BillHeaderID == billheaderid).FirstOrDefault();
            return model ?? new BillHeader();
        }

        /// <summary>
        /// Gets all encounter ids in bill header.
        /// </summary>
        /// <returns></returns>
        public List<int> GetAllEncounterIdsInBillHeader()
        {
            try
            {
                var lst = (from b in _repository.Where(c => c.EncounterID != null)
                           select b.EncounterID.Value).ToList();
                return lst.Distinct().ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets all bill header list by encounter identifier.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        public List<BillHeaderCustomModel> GetAllBillHeaderListByEncounterId(int encounterId)
        {
            try
            {
                var list = new List<BillHeaderCustomModel>();
                var lst = _repository.Where(a => a.IsDeleted == null || !(bool)a.IsDeleted
                    && a.EncounterID == encounterId).OrderByDescending(b => b.BillDate).ToList();

                if (lst.Count > 0)
                {
                    list.AddRange(lst.Select(i => new BillHeaderCustomModel
                    {
                        BillHeaderID = i.BillHeaderID,
                        BillNumber = i.BillNumber,
                        BillDate = i.BillDate,
                        CorporateID = i.CorporateID,
                        FacilityID = i.FacilityID,
                        PatientID = i.PatientID,
                        EncounterID = i.EncounterID,
                        PayerID = i.PayerID,
                        MemberID = i.MemberID,
                        Gross = i.Gross,
                        PatientShare = i.PatientShare,
                        PayerShareNet = i.PayerShareNet,
                        Status = GetNameByGlobalCodeValue(i.Status, Convert.ToInt32(GlobalCodeCategoryValue.BillHeaderStatus).ToString()),
                        DenialCode = i.DenialCode,
                        PaymentReference = i.PaymentReference,
                        DateSettlement = i.DateSettlement,
                        PaymentAmount = i.PaymentAmount,
                        PatientPayReference = i.PatientPayReference,
                        PatientDateSettlement = i.PatientDateSettlement,
                        PatientPayAmount = i.PatientPayAmount,
                        ClaimID = i.ClaimID,
                        FileID = i.FileID,
                        ARFileID = i.ARFileID,
                        CreatedBy = i.CreatedBy,
                        CreatedDate = i.CreatedDate,
                        ModifiedBy = i.ModifiedBy,
                        ModifiedDate = i.ModifiedDate,
                        IsDeleted = i.IsDeleted,
                        DeletedBy = i.DeletedBy,
                        DeletedDate = i.DeletedDate,
                        AuthID = i.AuthID,
                        AuthCode = i.AuthCode,
                        MCID = i.MCID,
                        MCPatientShare = i.MCPatientShare,
                        MCMultiplier = i.MCMultiplier,
                        CorporateName = GetNameByCorporateId(Convert.ToInt32(i.CorporateID)),
                        FacilityName = GetFacilityNameByFacilityId(Convert.ToInt32(i.FacilityID)),
                        EncounterNumber = GetEncounterNumberById(encounterId),
                        //InsuranceCompany = string.IsNullOrEmpty(i.PayerID) ? "Self" : GetInsuranceCompanyNameByPayerId(Convert.ToInt32(i.PayerID)),
                        PatientName = GetPatientNameById(Convert.ToInt32(i.PatientID)),
                        MCDiscount = i.MCDiscount,
                        DueDate = i.DueDate,
                        GrossChargesSum = i.PatientShare + i.PayerShareNet,
                        EncounterStatus = GetEncounterStatusById(Convert.ToInt32(i.EncounterID)),
                        InsuranceCompany = string.IsNullOrEmpty(i.PayerID) ? "-" : GetInsuranceCompanyNameByPayerId(Convert.ToInt32(i.PayerID)),
                        ActivityCost = i.ActivityCost
                    }));
                }
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Updates the bill headers by encounter identifier.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="isEncounterSelected">if set to <c>true</c> [is encounter selected].</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public List<BillHeaderCustomModel> UpdateBillHeadersByEncounterId(int encounterId, int patientId, bool isEncounterSelected, int corporateId, int facilityId)
        {
            var spName = string.Format("EXEC {0} @pCorporateID, @pFacilityID, @pEncounterID, @pReClaimFlag,@pClaimId",
                           StoredProcedures.SPROC_ApplyOrderActivityToBill);
            var sqlParameters = new SqlParameter[5];
            sqlParameters[0] = new SqlParameter(InputParams.pCorporateID.ToString(), corporateId);
            sqlParameters[1] = new SqlParameter(InputParams.pFacilityID.ToString(), facilityId);
            sqlParameters[2] = new SqlParameter(InputParams.pEncounterID.ToString(), encounterId);
            sqlParameters[3] = new SqlParameter(InputParams.pReClaimFlag.ToString(), string.Empty);
            sqlParameters[4] = new SqlParameter(InputParams.pClaimId.ToString(), 0);
            _repository.ExecuteCommand(StoredProcedures.SPROC_ApplyOrderActivityToBill.ToString(), sqlParameters);
            if (patientId > 0)
                return GetBillHeaderListByPatientId(patientId, corporateId, facilityId);
            return isEncounterSelected ? GetBillHeaderListByEncounterId(encounterId, corporateId, facilityId) : GetAllBillHeaderList(corporateId, facilityId);

        }

        /// <summary>
        /// Gets the bill headers by bill identifier.
        /// </summary>
        /// <param name="billid">The billid.</param>
        /// <returns></returns>
        public List<BillHeader> GetBillHeadersByBillId(int billid)
        {
            var lstEncounterIds = _repository.Where(x => x.BillHeaderID == billid && x.Status == "60").ToList();
            return lstEncounterIds;
        }

        /// <summary>
        /// Gets the bill header model list by encounter identifier.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        public List<BillHeader> GetBillHeaderModelListByEncounterId(int encounterId)
        {
            var lstBillHeader = _repository.Where(a => a.IsDeleted == null || !(bool)a.IsDeleted
                && a.EncounterID == encounterId).OrderByDescending(b => b.BillDate).ToList();
            return lstBillHeader;
        }

        #region Final Bills Overview
        /// <summary>
        /// Gets the final bill headers list.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="encounterSelected">if set to <c>true</c> [encounter selected].</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public List<BillHeaderCustomModel> GetFinalBillHeadersList(int encounterId, int patientId, bool encounterSelected, int corporateId, int facilityId)
        {
            try
            {
                var list = new List<BillHeaderCustomModel>();

                var spName = string.Format(
                       "EXEC {0} @pIsEnc,@pEncId,@pPid,@pFID,@pCID ",
                       StoredProcedures.SPROC_GetFinalBillHeadersList);
                var sqlParameters = new SqlParameter[5];
                sqlParameters[0] = new SqlParameter(InputParams.pIsEnc.ToString(), Convert.ToInt32(encounterSelected));
                sqlParameters[1] = new SqlParameter(InputParams.pEncId.ToString(), encounterId);
                sqlParameters[2] = new SqlParameter(InputParams.pPid.ToString(), patientId);
                sqlParameters[3] = new SqlParameter(InputParams.pFID.ToString(), facilityId);
                sqlParameters[4] = new SqlParameter(InputParams.pCID.ToString(), corporateId);
                IEnumerable<BillHeaderCustomModel> result = _context.Database.SqlQuery<BillHeaderCustomModel>(spName, sqlParameters);
                list = result.ToList();
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the pre XML file.
        /// </summary>
        /// <param name="billHeaderId">The bill header identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public List<BillHeaderPreXMLModel> GetPreXMLFile(int billHeaderId, int facilityId)
        {
            var spName = string.Format("EXEC {0} @pFacilityId,@pBillHeaderId", StoredProcedures.SPROC_GetPreliminaryXmlFile);
            var sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter(InputParams.pFacilityID.ToString(), facilityId);
            sqlParameters[1] = new SqlParameter(InputParams.pBillHeaderId.ToString(), billHeaderId);
            IEnumerable<BillHeaderPreXMLModel> result = _context.Database.SqlQuery<BillHeaderPreXMLModel>(spName, sqlParameters);
            return result.ToList();
        }

        #endregion

        /// <summary>
        /// Updates the bill header by bill header encounter identifier.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <param name="billheaderid">The billheaderid.</param>
        /// <param name="isEncounterSelected">if set to <c>true</c> [is encounter selected].</param>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public List<BillHeaderCustomModel> UpdateBillHeaderByBillHeaderEncounterId(int encounterId, int billheaderid, bool isEncounterSelected, int patientId, int corporateId, int facilityId)
        {
            UpdateBillHeadersByBillHeaderIdEncounterId(encounterId, billheaderid);
            if (patientId > 0)
                return GetBillHeaderListByPatientId(patientId, corporateId, facilityId);
            return isEncounterSelected ? GetBillHeaderListByEncounterId(encounterId, corporateId, facilityId) : GetAllBillHeaderList(corporateId, facilityId);
        }

        /// <summary>
        /// Sets the preliminary bill status by encounter identifier.
        /// </summary>
        /// <param name="encounterids">The encounterids.</param>
        /// <returns></returns>
        public bool SetPreliminaryBillStatusByEncounterId(List<int> encounterids, int userId)
        {
            var status = false;
            foreach (var encounterid in encounterids)
            {
                var spName = string.Format("EXEC {0} @pEncounterID , @pLoggedInUserId", StoredProcedures.SPROC_EncounterEndCheckBillEdit);
                var sqlParameters = new SqlParameter[2];
                sqlParameters[0] = new SqlParameter(InputParams.pEncounterID.ToString(), encounterid);
                sqlParameters[1] = new SqlParameter(InputParams.pLoggedInUserId.ToString(), userId);

                IEnumerable<EncounterEndCheckReturnStatus> result = _context.Database.SqlQuery<EncounterEndCheckReturnStatus>(spName, sqlParameters);
                if (result.Any())
                {
                    var m = result.FirstOrDefault();
                    status = m != null;
                }
            }
            return status;

        }

        /// <summary>
        /// Gets all bill header list by patient identifier.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public List<BillHeaderCustomModel> GetAllBillHeaderListByPatientId(int patientId)
        {
            try
            {
                var list = new List<BillHeaderCustomModel>();
                var lstBillHeader = _repository.Where(a => a.IsDeleted == null || !(bool)a.IsDeleted
                    && a.PatientID == patientId).OrderByDescending(b => b.BillDate).ToList();

                if (lstBillHeader.Any())
                {
                    list.AddRange(lstBillHeader.Select(i => new BillHeaderCustomModel
                    {
                        BillHeaderID = i.BillHeaderID,
                        BillNumber = i.BillNumber,
                        BillDate = i.BillDate,
                        CorporateID = i.CorporateID,
                        FacilityID = i.FacilityID,
                        PatientID = i.PatientID,
                        EncounterID = i.EncounterID,
                        PayerID = i.PayerID,
                        MemberID = i.MemberID,
                        Gross = i.Gross,
                        PatientShare = i.PatientShare,
                        PayerShareNet = i.PayerShareNet,
                        Status = GetNameByGlobalCodeValue(i.Status, Convert.ToInt32(GlobalCodeCategoryValue.BillHeaderStatus).ToString()),
                        DenialCode = i.DenialCode,
                        PaymentReference = i.PaymentReference,
                        DateSettlement = i.DateSettlement,
                        PaymentAmount = i.PaymentAmount,
                        PatientPayReference = i.PatientPayReference,
                        PatientDateSettlement = i.PatientDateSettlement,
                        PatientPayAmount = i.PatientPayAmount,
                        ClaimID = i.ClaimID,
                        FileID = i.FileID,
                        ARFileID = i.ARFileID,
                        CreatedBy = i.CreatedBy,
                        CreatedDate = i.CreatedDate,
                        ModifiedBy = i.ModifiedBy,
                        ModifiedDate = i.ModifiedDate,
                        IsDeleted = i.IsDeleted,
                        DeletedBy = i.DeletedBy,
                        DeletedDate = i.DeletedDate,
                        AuthID = i.AuthID,
                        AuthCode = i.AuthCode,
                        MCID = i.MCID,
                        MCPatientShare = i.MCPatientShare,
                        MCMultiplier = i.MCMultiplier,
                        CorporateName = GetNameByCorporateId(Convert.ToInt32(i.CorporateID)),
                        FacilityName = GetFacilityNameByFacilityId(Convert.ToInt32(i.FacilityID)),
                        EncounterNumber = GetEncounterNumberById(Convert.ToInt32(i.EncounterID)),
                        PatientName = GetPatientNameById(Convert.ToInt32(i.PatientID)),
                        MCDiscount = i.MCDiscount,
                        DueDate = i.DueDate,
                        GrossChargesSum = i.PatientShare + i.PayerShareNet,
                        EncounterStatus = GetEncounterStatusById(Convert.ToInt32(i.EncounterID)),
                        InsuranceCompany = string.IsNullOrEmpty(i.PayerID) ? "-" : GetInsuranceCompanyNameByPayerId(Convert.ToInt32(i.PayerID)),
                    }));
                }
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Applies the bed charges only.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        public int ApplyBedChargesOnly(int encounterId)
        {
            ApplyBedCharges(encounterId);
            return encounterId;
        }

        /// <summary>
        /// Realuclates the bill.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <param name="reclaimFlag">The reclaim flag.</param>
        /// <param name="calimId">The calim identifier.</param>
        /// <returns></returns>
        public bool RecalculateBill(int corporateId, int facilityId, int encounterId, string reclaimFlag, long calimId, int userId)
        {
            var sqlParameters = new SqlParameter[6];
            sqlParameters[0] = new SqlParameter(InputParams.pCorporateID.ToString(), corporateId);
            sqlParameters[1] = new SqlParameter(InputParams.pFacilityID.ToString(), facilityId);
            sqlParameters[2] = new SqlParameter(InputParams.pEncounterID.ToString(), encounterId);
            sqlParameters[3] = new SqlParameter(InputParams.pReClaimFlag.ToString(), reclaimFlag);
            sqlParameters[4] = new SqlParameter(InputParams.pClaimId.ToString(), calimId);
            sqlParameters[5] = new SqlParameter(InputParams.pLoggedInUserId.ToString(), userId);

            _repository.ExecuteCommand(StoredProcedures.SPROC_ReValuateCurrentBill.ToString(), sqlParameters);
            return true;
        }

        private void UpdateBillHeadersByBillHeaderIdEncounterId(int encounterId, int billheaderid)
        {
            var sqlParameters = new SqlParameter[10];
            sqlParameters[0] = new SqlParameter(InputParams.pEncounterID.ToString(), encounterId);
            sqlParameters[1] = new SqlParameter(InputParams.BillHeaderID.ToString(), billheaderid);
            sqlParameters[2] = new SqlParameter(InputParams.BillDetailLineNumber.ToString(), DBNull.Value);
            sqlParameters[3] = new SqlParameter(InputParams.BillNumber.ToString(), DBNull.Value);
            sqlParameters[4] = new SqlParameter(InputParams.AuthID.ToString(), DBNull.Value);
            sqlParameters[5] = new SqlParameter(InputParams.AuthType.ToString(), DBNull.Value);
            sqlParameters[6] = new SqlParameter(InputParams.AuthCode.ToString(), DBNull.Value);
            sqlParameters[7] = new SqlParameter(InputParams.SelfPayFlag.ToString(), DBNull.Value);
            sqlParameters[8] = new SqlParameter(InputParams.pReClaimFlag.ToString(), DBNull.Value);
            sqlParameters[9] = new SqlParameter(InputParams.pClaimId.ToString(), DBNull.Value);
            _repository.ExecuteCommand(StoredProcedures.SPROC_ApplyOrderToBillSetHeader.ToString(), sqlParameters);
        }
        /// <summary>
        /// Adds the billon encounter start.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <param name="billheaderid">The billheaderid.</param>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public bool AddBillonEncounterStart(int encounterId, int billheaderid, int patientId, int corporateId, int facilityId)
        {
            UpdateBillHeadersByBillHeaderIdEncounterId(encounterId, billheaderid);
            return true;
        }


        /// <summary>
        /// XMLs the scrub bill.
        /// </summary>
        /// <param name="calimId">The calim identifier.</param>
        /// <param name="userid">The userid.</param>
        /// <returns></returns>
        public bool XMLScrubBill(int claimId, int userid)
        {
            var sqlParameters = new SqlParameter[3];
            sqlParameters[0] = new SqlParameter(InputParams.pBillHeaderID.ToString(), claimId);
            sqlParameters[1] = new SqlParameter(InputParams.pExecutedBy.ToString(), userid);
            sqlParameters[2] = new SqlParameter
            {
                ParameterName = InputParams.pRETStatus.ToString(),
                Value = DBNull.Value,
                Size = Int32.MaxValue,
                Direction = System.Data.ParameterDirection.Output
            };
            _repository.ExecuteCommand(StoredProcedures.SPROC_ScrubBill.ToString(), sqlParameters);
            return true;
        }

        /// <summary>
        /// Gets the final bill payer headers list.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public List<BillHeaderCustomModel> GetFinalBillPayerHeadersList(int corporateId, int facilityId)
        {
            var spName = string.Format("EXEC {0} @pFID,@pCID", StoredProcedures.SPROC_GetPayerWiseFinalBills);
            var sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter(InputParams.pFID.ToString(), facilityId);
            sqlParameters[1] = new SqlParameter(InputParams.pCID.ToString(), corporateId);
            IEnumerable<BillHeaderCustomModel> result = _context.Database.SqlQuery<BillHeaderCustomModel>(spName, sqlParameters);
            return result.ToList();
        }

        /// <summary>
        /// Gets the final bill by payer headers list.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="payerIds">The payerid.</param>
        /// <returns></returns>
        public List<BillHeaderCustomModel> GetFinalBillByPayerHeadersList(int corporateId, int facilityId, string payerIds)
        {
            var sqlParameters = new SqlParameter[3];
            sqlParameters[0] = new SqlParameter(InputParams.pFID.ToString(), facilityId);
            sqlParameters[1] = new SqlParameter(InputParams.pCID.ToString(), corporateId);
            sqlParameters[2] = new SqlParameter(InputParams.pPayerId.ToString(), payerIds);
            using (var ms = _context.MultiResultSetSqlQuery(StoredProcedures.SPROC_GetFinalBillsByPayer.ToString(), isCompiled: false, parameters: sqlParameters))
            {
                var result = ms.GetResultWithJson<BillHeaderCustomModel>(JsonResultsArray.Claims.ToString());
                return result;
            }
        }

        /// <summary>
        /// Finds the claim.
        /// </summary>
        /// <param name="serachstring">The serachstring.</param>
        /// <param name="claimstatus">The claimstatus.</param>
        /// <param name="datefrom">The datefrom.</param>
        /// <param name="datetill">The datetill.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <returns></returns>
        public List<BillHeaderCustomModel> FindClaim(string serachstring, string claimstatus, DateTime? datefrom, DateTime? datetill, int facilityId, int corporateId, int fileid = 0)
        {
            var spName = string.Format("EXEC {0} @pSearchString, @pClaimStatus, @pDateFrom, @pDateTill, @pFID, @pCID, @pFileId", StoredProcedures.SPROC_FindEClaim);
            var sqlParameters = new SqlParameter[7];
            sqlParameters[0] = new SqlParameter(InputParams.pSearchString.ToString(), serachstring);
            sqlParameters[1] = new SqlParameter(InputParams.pClaimStatus.ToString(), claimstatus);
            sqlParameters[2] = new SqlParameter(InputParams.pDateFrom.ToString(), datefrom);
            sqlParameters[3] = new SqlParameter(InputParams.pDateTill.ToString(), datetill);
            sqlParameters[4] = new SqlParameter(InputParams.pFID.ToString(), facilityId);
            sqlParameters[5] = new SqlParameter(InputParams.pCID.ToString(), corporateId);
            sqlParameters[6] = new SqlParameter(InputParams.pFileId.ToString(), fileid);
            IEnumerable<BillHeaderCustomModel> result = _context.Database.SqlQuery<BillHeaderCustomModel>(spName, sqlParameters);
            return result.ToList();
        }


        /// <summary>
        /// Sends the e claims by payer.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="payerId">The payer identifier.</param>
        /// <param name="billHeaderIds">The bill header ids.</param>
        /// <returns></returns>
        public IEnumerable<BillHeaderXMLModel> SendEClaimsByPayer(int facilityId, string payerId, string billHeaderIds)
        {
            var spName = string.Format("EXEC {0} @SenderID, @DispositionFlag, @PayerID, @BillHeaderIds", StoredProcedures.SendEClaimByPayerIDs);
            var sqlParameters = new SqlParameter[4];
            sqlParameters[0] = new SqlParameter(InputParams.SenderID.ToString(), facilityId);
            sqlParameters[1] = new SqlParameter(InputParams.DispositionFlag.ToString(), "Test");
            sqlParameters[2] = new SqlParameter(InputParams.PayerID.ToString(), payerId);
            sqlParameters[3] = new SqlParameter(InputParams.BillHeaderIds.ToString(), billHeaderIds);
            IEnumerable<BillHeaderXMLModel> result = _context.Database.SqlQuery<BillHeaderXMLModel>(spName, sqlParameters);
            return result.ToList();
        }

        /// <summary>
        /// Finds the claim by file identifier.
        /// </summary>
        /// <param name="serachstring">The serachstring.</param>
        /// <param name="claimstatus">The claimstatus.</param>
        /// <param name="datefrom">The datefrom.</param>
        /// <param name="datetill">The datetill.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="fileid">The fileid.</param>
        /// <returns></returns>
        public List<BillHeaderCustomModel> FindClaimByFileId(string serachstring, string claimstatus, DateTime? datefrom, DateTime? datetill, int facilityId, int corporateId, int? fileid)
        {
            return FindClaim(serachstring, claimstatus, datefrom, datetill, facilityId, corporateId, Convert.ToInt32(fileid));
        }
    }
}