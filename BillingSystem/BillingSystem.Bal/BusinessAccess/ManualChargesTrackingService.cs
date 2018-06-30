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
    public class ManualChargesTrackingService : IManualChargesTrackingService
    {
        private readonly IRepository<GlobalCodes> _gRepository;
        private readonly IRepository<ManualChargesTracking> _repository;
        private readonly IRepository<Users> _uRepository;
        private readonly IRepository<BillHeader> _blRepository;
        private readonly IRepository<InsuranceCompany> _icRepository;
        private readonly IRepository<ErrorMaster> _erRepository;
        private readonly IRepository<Encounter> _eRepository;
        private readonly IRepository<Corporate> _cRepository;
        private readonly IRepository<Facility> _fRepository;
        private readonly IRepository<PatientInfo> _piRepository;

        public ManualChargesTrackingService(IRepository<GlobalCodes> gRepository, IRepository<ManualChargesTracking> repository, IRepository<Users> uRepository, IRepository<BillHeader> blRepository, IRepository<InsuranceCompany> icRepository, IRepository<ErrorMaster> erRepository, IRepository<Encounter> eRepository, IRepository<Corporate> cRepository, IRepository<Facility> fRepository, IRepository<PatientInfo> piRepository)
        {
            _gRepository = gRepository;
            _repository = repository;
            _uRepository = uRepository;
            _blRepository = blRepository;
            _icRepository = icRepository;
            _erRepository = erRepository;
            _eRepository = eRepository;
            _cRepository = cRepository;
            _fRepository = fRepository;
            _piRepository = piRepository;
        }



        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>
        /// Return the Entity List
        /// </returns>
        public List<ManualChargesTrackingCustomModel> GetManualChargesTrackingList(int facilityid, int corporateid)
        {
            var list = new List<ManualChargesTrackingCustomModel>();
            var lstManualChargesTracking =
                _repository.Where(
                    a =>
                        (a.IsVisible == null || (bool)a.IsVisible) && a.CorporateID == corporateid &&
                        a.FacilityID == facilityid).ToList();
            if (lstManualChargesTracking.Count > 0)
            {
                list.AddRange(lstManualChargesTracking.Select(item => new ManualChargesTrackingCustomModel
                {
                    ManualChargesTrackingID = item.ManualChargesTrackingID,
                    TrackingType = item.TrackingType,
                    TrackingTypeNameVal = item.TrackingTypeNameVal,
                    TrackingColumnName = item.TrackingColumnName,
                    TrackingTableName = item.TrackingTableName,
                    TrackingValue = item.TrackingValue,
                    TrackingBillStatus = item.TrackingBillStatus,
                    BillHeaderID = item.BillHeaderID,
                    EncounterID = item.EncounterID,
                    PatientID = item.PatientID,
                    FacilityID = item.FacilityID,
                    CorporateID = item.CorporateID,
                    CreatedBy = item.CreatedBy,
                    CreatedDate = item.CreatedDate,
                    ModifiedBy = item.ModifiedBy,
                    ModifiedDate = item.ModifiedDate,
                    DeletedBy = item.DeletedBy,
                    DeletedDate = item.DeletedDate,
                    ExternalValue1 = item.ExternalValue1,
                    ExternalValue2 = item.ExternalValue2,
                    ExternalValue3 = item.ExternalValue3,
                    ExternalValue4 = item.ExternalValue4,
                    ExternalValue5 = item.ExternalValue5,
                    ExternalValue6 = item.ExternalValue6,
                    IsVisible = item.IsVisible,
                    TrackingTypeStr = GetNameByGlobalCodeValue(item.TrackingType, "1019"),
                    TrackingTypeNameValStr = GetNameByGlobalCodeValue(item.TrackingTypeNameVal, "1201"),
                    FacilityName = GetFacilityNameByFacilityId(item.FacilityID),
                    CorporateName = GetNameByCorporateId(item.CorporateID),
                    EncounterNumber = GetEncounterNumberById(item.EncounterID),
                    PatientName = GetPatientNameById(item.PatientID),
                    BillNumber = GetBillNumberByBillHeaderId(item.BillHeaderID),
                    TrackingBillStatusStr = GetBillHeaderStatusByStatus(item.TrackingBillStatus),
                    UpdatedBy = GetNameByUserId(item.CreatedBy)
                }));
            }
            return list.OrderByDescending(i => i.ManualChargesTrackingID).ToList();
        }
        private string GetPatientNameById(int PatientID)
        {
            var m = _piRepository.GetSingle(Convert.ToInt32(PatientID));
            return m != null ? m.PersonFirstName + " " + m.PersonLastName : string.Empty;
        }
        private BillHeaderCustomModel GetBillHeaderById(int billHeaderId)
        {
            var customModel = new BillHeaderCustomModel();
            var m = _blRepository.Where(x => x.BillHeaderID == billHeaderId).FirstOrDefault();
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
        private string GetEncounterNumberById(int? encounterID)
        {
            var m = _eRepository.Where(e => e.EncounterID == Convert.ToInt32(encounterID)).FirstOrDefault();
            return m != null ? m.EncounterNumber : string.Empty;
        }
        private string GetNameByCorporateId(int corporateID)
        {
            var m = _cRepository.GetSingle(corporateID);
            return m != null ? m.CorporateName : string.Empty;
        }
        private string GetFacilityNameByFacilityId(int facilityId)
        {
            var m = _fRepository.GetSingle(facilityId);
            return m != null ? m.FacilityName : string.Empty;
        }
        private string GetEncounterStatusById(int? encounterID)
        {
            var m = _eRepository.Where(e => e.EncounterID == Convert.ToInt32(encounterID)).FirstOrDefault();
            return m == null ? m.EncounterEndTime != null ? "Ended" : "Active" : "NA";
        }
        private IEnumerable<ErrorMasterCustomModel> GetSearchedDenialsList(string text)
        {
            var list = new List<ErrorMasterCustomModel>();
            text = string.IsNullOrEmpty(text) ? string.Empty : text.ToLower().Trim();
            var lstErrorMaster = _erRepository.Where(a => a.IsActive != null && (bool)a.IsActive && (a.ErrorDescription.ToLower().Contains(text) || a.ErrorCode.ToLower().Contains(text))).ToList();
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

        private string GetInsuranceCompanyNameByPayerId(int PayerID)
        {
            var m = _icRepository.Find(x => x.InsuranceCompanyLicenseNumber.Equals(PayerID));
            return m != null ? m.InsuranceCompanyName : string.Empty;
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
        /// Gets the manual charges tracking list date range.
        /// </summary>
        /// <param name="facilityid">The facilityid.</param>
        /// <param name="corporateid">The corporateid.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="tilldate">The tilldate.</param>
        /// <returns></returns>
        public List<ManualChargesTrackingCustomModel> GetManualChargesTrackingListDateRange(int facilityid, int corporateid, DateTime fromDate, DateTime tilldate)
        {
            var list = new List<ManualChargesTrackingCustomModel>();
            tilldate = tilldate.AddHours(23);
            var lstManualChargesTracking =
                _repository.Where(
                    a =>
                        (a.IsVisible == null || (bool)a.IsVisible) && a.CorporateID == corporateid &&
                        a.FacilityID == facilityid).ToList();
            lstManualChargesTracking = lstManualChargesTracking.Where(a => a.CreatedDate >= fromDate && a.CreatedDate <= tilldate).ToList();
            if (lstManualChargesTracking.Count > 0)
            {
                list.AddRange(lstManualChargesTracking.Select(item => new ManualChargesTrackingCustomModel
                {
                    ManualChargesTrackingID = item.ManualChargesTrackingID,
                    TrackingType = item.TrackingType,
                    TrackingTypeNameVal = item.TrackingTypeNameVal,
                    TrackingColumnName = item.TrackingColumnName,
                    TrackingTableName = item.TrackingTableName,
                    TrackingValue = item.TrackingValue,
                    TrackingBillStatus = item.TrackingBillStatus,
                    BillHeaderID = item.BillHeaderID,
                    EncounterID = item.EncounterID,
                    PatientID = item.PatientID,
                    FacilityID = item.FacilityID,
                    CorporateID = item.CorporateID,
                    CreatedBy = item.CreatedBy,
                    CreatedDate = item.CreatedDate,
                    ModifiedBy = item.ModifiedBy,
                    ModifiedDate = item.ModifiedDate,
                    DeletedBy = item.DeletedBy,
                    DeletedDate = item.DeletedDate,
                    ExternalValue1 = item.ExternalValue1,
                    ExternalValue2 = item.ExternalValue2,
                    ExternalValue3 = item.ExternalValue3,
                    ExternalValue4 = item.ExternalValue4,
                    ExternalValue5 = item.ExternalValue5,
                    ExternalValue6 = item.ExternalValue6,
                    IsVisible = item.IsVisible,
                    TrackingTypeStr = GetNameByGlobalCodeValue(item.TrackingType, "1019"),
                    TrackingTypeNameValStr = GetNameByGlobalCodeValue(item.TrackingTypeNameVal, "1201"),
                    FacilityName = GetFacilityNameByFacilityId(item.FacilityID),
                    CorporateName = GetNameByCorporateId(item.CorporateID),
                    EncounterNumber = GetEncounterNumberById(item.EncounterID),
                    PatientName = GetPatientNameById(item.PatientID),
                    BillNumber = GetBillNumberByBillHeaderId(item.BillHeaderID),
                    TrackingBillStatusStr = GetBillHeaderStatusByStatus(item.TrackingBillStatus),
                    UpdatedBy = GetNameByUserId(item.CreatedBy)
                }));
            }
            return list.OrderByDescending(i => i.ManualChargesTrackingID).ToList();
        }
        private string GetBillNumberByBillHeaderId(int billHeaderId)
        {
            var billheaderobj = _blRepository.Where(x => x.BillHeaderID == billHeaderId).FirstOrDefault();// GetBillHeaderById(billHeaderId);
            return billheaderobj != null ? billheaderobj.BillNumber : "NA";
        }
        private string GetNameByUserId(int? UserID)
        {
            try
            {
                var usersModel = _uRepository.Where(x => x.UserID == UserID && x.IsDeleted == false).FirstOrDefault();
                return usersModel != null ? usersModel.FirstName + " " + usersModel.LastName : string.Empty;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private string GetBillHeaderStatusByStatus(string status)
        {
            var gc = GetNameByGlobalCodeValue(status, Convert.ToInt32(GlobalCodeCategoryValue.BillHeaderStatus).ToString());
            return !string.IsNullOrEmpty(gc) ? gc : "Initialized";
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="ManualChargesTrackingId">The manual charges tracking identifier.</param>
        /// <returns></returns>
        public ManualChargesTracking GetManualChargesTrackingByID(int? ManualChargesTrackingId)
        {
            var model = _repository.Where(x => x.ManualChargesTrackingID == ManualChargesTrackingId).FirstOrDefault();
            return model;
        }

        /// <summary>
        /// Adds the uptdate corporate.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public int AddUptdateManualChargesTracking(ManualChargesTracking model)
        {
            if (model.ManualChargesTrackingID > 0)
            {
                _repository.UpdateEntity(model, model.ManualChargesTrackingID);
            }
            else
                _repository.Create(model);
            return model.CorporateID;
        }
    }
}
