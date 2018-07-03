using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;


namespace BillingSystem.Bal.BusinessAccess
{
    public class MissingDataService : IMissingDataService
    {
        private readonly BillingEntities _context;
        private readonly IRepository<BillHeader> _bhRepository;
        private readonly IRepository<Encounter> _eRepository;
        private readonly IRepository<GlobalCodes> _gRepository;
        private readonly IRepository<PatientInfo> _piRepository;
        private readonly IRepository<Facility> _fRepository;
        private readonly IRepository<InsuranceCompany> _icRepository;
        private readonly IRepository<Corporate> _cRepository;

        public MissingDataService(BillingEntities context, IRepository<BillHeader> bhRepository, IRepository<Encounter> eRepository, IRepository<GlobalCodes> gRepository, IRepository<PatientInfo> piRepository, IRepository<Facility> fRepository, IRepository<InsuranceCompany> icRepository, IRepository<Corporate> cRepository)
        {
            _context = context;
            _bhRepository = bhRepository;
            _eRepository = eRepository;
            _gRepository = gRepository;
            _piRepository = piRepository;
            _fRepository = fRepository;
            _icRepository = icRepository;
            _cRepository = cRepository;
        }


        /// <summary>
        /// Gets the XML missing data.
        /// </summary>
        /// <param name="corporateid">The corporateid.</param>
        /// <param name="facilityid">The facilityid.</param>
        /// <returns></returns>
        public List<MissingDataCustomModel> GetXMLMissingData(int corporateid, int facilityid)
        {
            string spName = string.Format("EXEC {0} @pCorporateID, @pFacilityID", StoredProcedures.SPROC_GetXMLMissingDataDetail);
            var sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("pCorporateID", corporateid);
            sqlParameters[1] = new SqlParameter("pFacilityID", facilityid);
            IEnumerable<MissingDataCustomModel> result = _context.Database.SqlQuery<MissingDataCustomModel>(spName, sqlParameters);
            return result.ToList();
        }

        /// <summary>
        /// Gets all XML bill header list.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public List<BillHeaderCustomModel> GetAllXMLBillHeaderList(int corporateId, int facilityId)
        {
            var list = new List<BillHeaderCustomModel>();

            var lstBillHeader = corporateId > 0 ?
                _bhRepository.Where(a => (a.IsDeleted == null || !(bool)a.IsDeleted) && a.CorporateID != null && (int)a.CorporateID == corporateId && (int)a.FacilityID == facilityId).OrderByDescending(b => b.BillHeaderID).ThenByDescending(b => b.PatientID).ToList() :
                _bhRepository.Where(a => (a.IsDeleted == null || !(bool)a.IsDeleted)).OrderByDescending(b => b.BillHeaderID).ThenByDescending(b => b.PatientID).ToList();

            if (lstBillHeader.Count > 0)
            {
                lstBillHeader = lstBillHeader.Where(x => Convert.ToInt32(x.Status) == 45).ToList();
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
                    GrossChargesSum = i.PatientShare + i.PayerShareNet,
                    PatientShare = i.PatientShare,
                    PayerShareNet = i.PayerShareNet,
                    Status = GetBillHeaderStatus(i.Status),
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
                    InsuranceCompany = string.IsNullOrEmpty(i.PayerID) ? "-" : GetInsuranceCompanyNameByPayerId(Convert.ToInt32(i.PayerID)),
                    PatientName = GetPatientNameById(Convert.ToInt32(i.PatientID)),
                    BStatus = Convert.ToInt32(i.Status),
                    MCDiscount = i.MCDiscount,
                    EncounterStatus = GetEncounterStatusById(Convert.ToInt32(i.EncounterID)),
                    EncounterType = GetEncounterHomeCareRecuuringById(Convert.ToInt32(i.EncounterID))
                }));
                list = list.ToList();
            }
            return list;
        }
        private string GetInsuranceCompanyNameByPayerId(int PayerID)
        {
            var m = _icRepository.Find(x => x.InsuranceCompanyLicenseNumber.Equals(PayerID));
            return m != null ? m.InsuranceCompanyName : string.Empty;
        }

        private string GetEncounterStatusById(int? encounterID)
        {
            var m = _eRepository.Where(e => e.EncounterID == Convert.ToInt32(encounterID)).FirstOrDefault();
            return m == null ? m.EncounterEndTime != null ? "Ended" : "Active" : "NA";
        }
        private string GetEncounterNumberById(int? encounterID)
        {
            var m = _eRepository.Where(e => e.EncounterID == Convert.ToInt32(encounterID)).FirstOrDefault();
            return m != null ? m.EncounterNumber : string.Empty;
        }
        private string GetFacilityNameByFacilityId(int facilityId)
        {
            var m = _fRepository.GetSingle(facilityId);
            return m != null ? m.FacilityName : string.Empty;
        }
        private string GetNameByCorporateId(int corporateID)
        {
            var m = _cRepository.GetSingle(corporateID);
            return m != null ? m.CorporateName : string.Empty;
        }
        private string GetBillHeaderStatus(string status)
        {
            var gc = GetNameByGlobalCodeValue(
                status,
                Convert.ToInt32(GlobalCodeCategoryValue.BillHeaderStatus).ToString());
            return !string.IsNullOrEmpty(gc) ? gc : "Initialized";
        }

        private string GetPatientNameById(int PatientID)
        {
            var m = _piRepository.GetSingle(Convert.ToInt32(PatientID));
            return m != null ? m.PersonFirstName + " " + m.PersonLastName : string.Empty;
        }
        private string GetBillHeaderStatusByStatus(string status)
        {
            var gc = GetNameByGlobalCodeValue(status, Convert.ToInt32(GlobalCodeCategoryValue.BillHeaderStatus).ToString());
            return !string.IsNullOrEmpty(gc) ? gc : "Initialized";
        }
        private bool GetEncounterHomeCareRecuuringById(int id)
        {
            var encounter = _eRepository.Where(e => e.EncounterID == id).FirstOrDefault();
            return encounter != null && (encounter.HomeCareRecurring ?? false);

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
    }
}
