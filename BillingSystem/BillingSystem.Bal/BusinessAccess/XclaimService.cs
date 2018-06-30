using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Repository.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class XclaimService : IXclaimService
    {
        private readonly IRepository<XClaim> _repository;
        private readonly IRepository<XPaymentReturn> _prRepository;
        private readonly IRepository<PatientInfo> _piRepository;
        private readonly IRepository<Facility> _fRepository;

        public XclaimService(IRepository<XClaim> repository, IRepository<XPaymentReturn> prRepository, IRepository<PatientInfo> piRepository, IRepository<Facility> fRepository)
        {
            _repository = repository;
            _prRepository = prRepository;
            _piRepository = piRepository;
            _fRepository = fRepository;
        }


        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>
        /// Return the Entity List
        /// </returns>
        public List<XClaimCustomModel> GetXclaim(string facilityid)
        {
            var list = new List<XClaimCustomModel>();
            var facilityObj = GetFacilityByFacilityId(Convert.ToInt32(facilityid));
            var facilitynumber = facilityObj != null ? facilityObj.FacilityNumber : "0";
            var lstXclaim = _repository.Where(x => x.FacilityID == facilityid || x.FacilityID.Equals(facilitynumber)).OrderByDescending(x => x.ClaimID).ToList();
            if (lstXclaim.Count > 0)
            {
                list.AddRange(lstXclaim.Select(item => new XClaimCustomModel
                {
                    XClaimID = item.XClaimID,
                    ClaimID = item.ClaimID,
                    EncounterID = item.EncounterID,
                    IDPayer = item.IDPayer,
                    MemberID = item.MemberID,
                    PayerID = item.PayerID,
                    ProviderID = item.ProviderID,
                    EmiratesIDNumber = item.EmiratesIDNumber,
                    Gross = item.Gross,
                    PatientShare = item.PatientShare,
                    Net = item.Net,
                    FacilityID = item.FacilityID,
                    FType = item.FType,
                    PatientID = item.PatientID,
                    EligibilityIDPayer = item.EligibilityIDPayer,
                    StartDate = item.StartDate,
                    EndDate = item.EndDate,
                    StartType = item.StartType,
                    EndType = item.EndType,
                    TransferSource = item.TransferSource,
                    TransferDestination = item.TransferDestination,
                    DenialCode = item.DenialCode,
                    PaymentReference = item.PaymentReference,
                    DateSettlement = item.DateSettlement,
                    PaymentAmount = item.PaymentAmount,
                    PatientPayReference = item.PatientPayReference,
                    PatientDateSettlement = item.PatientDateSettlement,
                    PatientPayAmount = item.PatientPayAmount,
                    Status = item.Status,
                    FileID = item.FileID,
                    ARFileID = item.ARFileID,
                    ModifiedBy = item.ModifiedBy,
                    ModifiedDate = item.ModifiedDate,
                    PatientName = GetPatientNameById(Convert.ToInt32(item.PatientID)),
                    ClaimGenerated = GetClaimPayment(Convert.ToInt32(item.ClaimID)),
                    MCDiscount = item.MCDiscount,
                }));
            }
            return list;
        }
        private bool GetClaimPayment(int claimid)
        {
            var lstXPaymentReturn = _prRepository.Where(x => x.ID == claimid).ToList();
            return lstXPaymentReturn.Any();
        }
        private Facility GetFacilityByFacilityId(int facilityId)
        {
            var facility = new Facility();
            var fid = Convert.ToInt32(facilityId);
            if (facilityId > 0)
                facility = _fRepository.Where(f => f.FacilityId == fid).FirstOrDefault();

            return facility;
        }
        private string GetPatientNameById(int PatientID)
        {
            var m = _piRepository.GetSingle(Convert.ToInt32(PatientID));
            return m != null ? m.PersonFirstName + " " + m.PersonLastName : string.Empty;
        }
        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public int SaveXclaim(XClaim model)
        {
            if (model.ClaimID > 0)
                _repository.UpdateEntity(model, Convert.ToInt32(model.ClaimID));
            else
                _repository.Create(model);
            return Convert.ToInt32(model.ClaimID);
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="XclaimId">The xclaim identifier.</param>
        /// <returns></returns>
        public XClaim GetXclaimByID(int? XclaimId)
        {
            var model = _repository.Where(x => x.ClaimID == XclaimId).FirstOrDefault();
            return model;
        }

        /// <summary>
        /// Gets the xclaim by parameters.
        /// </summary>
        /// <param name="pid">The pid.</param>
        /// <param name="eid">The eid.</param>
        /// <param name="claimid">The claimid.</param>
        /// <returns></returns>
        public List<XClaimCustomModel> GetXclaimByParameters(string pid, Int64 eid, Int64 claimid)
        {
            var list = new List<XClaimCustomModel>();
            var lstXclaim = _repository.GetAll().ToList();
            if (pid != "0")
            {
                lstXclaim = lstXclaim.Where(x => x.PatientID == pid).ToList();
            }
            if (eid != 0)
            {
                lstXclaim = lstXclaim.Where(x => x.EncounterID == eid).ToList();
            }
            if (claimid != 0)
            {
                lstXclaim = lstXclaim.Where(x => x.ClaimID == claimid).ToList();
            }

            if (lstXclaim.Any())
            {
                list.AddRange(lstXclaim.Select(item => new XClaimCustomModel
                {
                    XClaimID = item.XClaimID,
                    ClaimID = item.ClaimID,
                    EncounterID = item.EncounterID,
                    IDPayer = item.IDPayer,
                    MemberID = item.MemberID,
                    PayerID = item.PayerID,
                    ProviderID = item.ProviderID,
                    EmiratesIDNumber = item.EmiratesIDNumber,
                    Gross = item.Gross,
                    PatientShare = item.PatientShare,
                    Net = item.Net,
                    FacilityID = item.FacilityID,
                    FType = item.FType,
                    PatientID = item.PatientID,
                    EligibilityIDPayer = item.EligibilityIDPayer,
                    StartDate = item.StartDate,
                    EndDate = item.EndDate,
                    StartType = item.StartType,
                    EndType = item.EndType,
                    TransferSource = item.TransferSource,
                    TransferDestination = item.TransferDestination,
                    DenialCode = item.DenialCode,
                    PaymentReference = item.PaymentReference,
                    DateSettlement = item.DateSettlement,
                    PaymentAmount = item.PaymentAmount,
                    PatientPayReference = item.PatientPayReference,
                    PatientDateSettlement = item.PatientDateSettlement,
                    PatientPayAmount = item.PatientPayAmount,
                    Status = item.Status,
                    FileID = item.FileID,
                    ARFileID = item.ARFileID,
                    ModifiedBy = item.ModifiedBy,
                    ModifiedDate = item.ModifiedDate,
                    PatientName = GetPatientNameById(Convert.ToInt32(item.PatientID)),
                    ClaimGenerated = GetClaimPayment(Convert.ToInt32(item.ClaimID)),
                    MCDiscount = item.MCDiscount,
                }));
            }
            return list;
        }

        /// <summary>
        /// Gets the xclaim by facility parameters.
        /// </summary>
        /// <param name="facilityid">The facilityid.</param>
        /// <param name="pid">The pid.</param>
        /// <param name="eid">The eid.</param>
        /// <param name="claimid">The claimid.</param>
        /// <returns></returns>
        public List<XClaimCustomModel> GetXclaimByFacilityParameters(string facilityid, string pid, Int64 eid, Int64 claimid)
        {
            var list = new List<XClaimCustomModel>();
            var facilityObj = GetFacilityByFacilityId(Convert.ToInt32(facilityid));
            var facilitynumber = facilityObj != null ? facilityObj.FacilityNumber : "0";
            var lstXclaim = _repository.Where(x => x.FacilityID == facilityid || x.FacilityID.Equals(facilitynumber)).ToList();
            if (pid != "0")
            {
                lstXclaim = lstXclaim.Where(x => x.PatientID == pid).ToList();
            }
            if (eid != 0)
            {
                lstXclaim = lstXclaim.Where(x => x.EncounterID == eid).ToList();
            }
            if (claimid != 0)
            {
                lstXclaim = lstXclaim.Where(x => x.ClaimID == claimid).ToList();
            }
            if (lstXclaim.Any())
            {
                list.AddRange(lstXclaim.Select(item => new XClaimCustomModel
                {
                    XClaimID = item.XClaimID,
                    ClaimID = item.ClaimID,
                    EncounterID = item.EncounterID,
                    IDPayer = item.IDPayer,
                    MemberID = item.MemberID,
                    PayerID = item.PayerID,
                    ProviderID = item.ProviderID,
                    EmiratesIDNumber = item.EmiratesIDNumber,
                    Gross = item.Gross,
                    PatientShare = item.PatientShare,
                    Net = item.Net,
                    FacilityID = item.FacilityID,
                    FType = item.FType,
                    PatientID = item.PatientID,
                    EligibilityIDPayer = item.EligibilityIDPayer,
                    StartDate = item.StartDate,
                    EndDate = item.EndDate,
                    StartType = item.StartType,
                    EndType = item.EndType,
                    TransferSource = item.TransferSource,
                    TransferDestination = item.TransferDestination,
                    DenialCode = item.DenialCode,
                    PaymentReference = item.PaymentReference,
                    DateSettlement = item.DateSettlement,
                    PaymentAmount = item.PaymentAmount,
                    PatientPayReference = item.PatientPayReference,
                    PatientDateSettlement = item.PatientDateSettlement,
                    PatientPayAmount = item.PatientPayAmount,
                    Status = item.Status,
                    FileID = item.FileID,
                    ARFileID = item.ARFileID,
                    ModifiedBy = item.ModifiedBy,
                    ModifiedDate = item.ModifiedDate,
                    PatientName = GetPatientNameById(Convert.ToInt32(item.PatientID)),
                    ClaimGenerated = GetClaimPayment(Convert.ToInt32(item.ClaimID)),
                    MCDiscount = item.MCDiscount,
                }));
            }
            return list;
        }

        /// <summary>
        /// Gets the xclaim by encounter identifier.
        /// </summary>
        /// <param name="encounterid">The encounterid.</param>
        /// <returns></returns>
        public List<XClaim> GetXclaimByEncounterId(long encounterid)
        {
            var model = _repository.Where(x => x.EncounterID == encounterid).ToList();
            return model;
        }

        /// <summary>
        /// Applies the advice payment.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityid">The facilityid.</param>
        /// <returns></returns>
        public bool ApplyAdvicePayment(int corporateId, int facilityid)
        {
            var sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("pCorporateID", corporateId);
            sqlParameters[1] = new SqlParameter("pFacilityID", facilityid);

            _repository.ExecuteCommand(StoredProcedures.SPROC_ApplyAdvicePayments.ToString(), sqlParameters);
            return true;
        }

        /// <summary>
        /// Applies the advice payment in remittance advice.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityid">The facilityid.</param>
        /// <param name="fileId">The file identifier.</param>
        /// <returns></returns>
        public bool ApplyAdvicePaymentInRemittanceAdvice(int corporateId, int facilityid, int fileId)
        {
            var sqlParameters = new SqlParameter[3];
            sqlParameters[0] = new SqlParameter("pCorporateID", corporateId);
            sqlParameters[1] = new SqlParameter("pFacilityID", facilityid);
            sqlParameters[2] = new SqlParameter("pFileId", fileId);

            _repository.ExecuteCommand(StoredProcedures.SPROC_ApplyAdvicePaymentsByFileID.ToString(), sqlParameters);
            return true;
        }
    }
}
