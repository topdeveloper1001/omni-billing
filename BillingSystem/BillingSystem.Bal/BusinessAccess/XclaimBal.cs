using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using BillingSystem.Repository.UOW;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.BusinessAccess
{
    public class XclaimBal : BaseBal
    {
        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>
        /// Return the Entity List
        /// </returns>
        public List<XClaimCustomModel> GetXclaim(string facilityid)
        {
            var list = new List<XClaimCustomModel>();
            using (var xclaimRep = UnitOfWork.XclaimRepository)
            {
                var facilityObj = GetFacilityByFacilityId(Convert.ToInt32(facilityid));
                var facilitynumber = facilityObj != null ? facilityObj.FacilityNumber : "0";
                var lstXclaim = xclaimRep.Where(x => x.FacilityID == facilityid || x.FacilityID.Equals(facilitynumber)).OrderByDescending(x => x.ClaimID).ToList();
                var paymentreturn = new XPaymentReturnBal();
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
                        ClaimGenerated = paymentreturn.GetClaimPayment(Convert.ToInt32(item.ClaimID)),
                        MCDiscount = item.MCDiscount,
                    }));
                }
            }
            return list;
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public int SaveXclaim(XClaim model)
        {
            using (var rep = UnitOfWork.XclaimRepository)
            {
                if (model.ClaimID > 0)
                    rep.UpdateEntity(model, Convert.ToInt32(model.ClaimID));
                else
                    rep.Create(model);
                return Convert.ToInt32(model.ClaimID);
            }
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="XclaimId">The xclaim identifier.</param>
        /// <returns></returns>
        public XClaim GetXclaimByID(int? XclaimId)
        {
            using (var rep = UnitOfWork.XclaimRepository)
            {
                var model = rep.Where(x => x.ClaimID == XclaimId).FirstOrDefault();
                return model;
            }
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
            using (var xclaimRep = UnitOfWork.XclaimRepository)
            {
                var lstXclaim = xclaimRep.GetAll().ToList();
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
                    var paymentreturn = new XPaymentReturnBal();
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
                        ClaimGenerated = paymentreturn.GetClaimPayment(Convert.ToInt32(item.ClaimID)),
                        MCDiscount = item.MCDiscount,
                    }));
                }
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
            using (var xclaimRep = UnitOfWork.XclaimRepository)
            {
                var facilityObj = GetFacilityByFacilityId(Convert.ToInt32(facilityid));
                var facilitynumber = facilityObj != null ? facilityObj.FacilityNumber : "0";
                var lstXclaim = xclaimRep.Where(x => x.FacilityID == facilityid || x.FacilityID.Equals(facilitynumber)).ToList();
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
                    var paymentreturn = new XPaymentReturnBal();
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
                        ClaimGenerated = paymentreturn.GetClaimPayment(Convert.ToInt32(item.ClaimID)),
                        MCDiscount = item.MCDiscount,
                    }));
                }
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
            using (var rep = UnitOfWork.XclaimRepository)
            {
                var model = rep.Where(x => x.EncounterID == encounterid).ToList();
                return model;
            }
        }

        /// <summary>
        /// Applies the advice payment.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityid">The facilityid.</param>
        /// <returns></returns>
        public bool ApplyAdvicePayment(int corporateId, int facilityid)
        {
            var result = false;
            using (var rep = UnitOfWork.XclaimRepository)
            {
                result = rep.ApplyAdvicePayment(corporateId, facilityid);
            }
            return result;
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
            bool result;
            using (var rep = UnitOfWork.XclaimRepository)
            {
                result = rep.ApplyAdvicePaymentInRemittanceAdvice(corporateId, facilityid, fileId);
            }
            return result;
        }
    }
}
