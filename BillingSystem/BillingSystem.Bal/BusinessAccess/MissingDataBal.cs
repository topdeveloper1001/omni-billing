using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.BusinessAccess
{
     public class MissingDataBal:BaseBal
    {
        /// <summary>
        /// Gets the XML missing data.
        /// </summary>
        /// <param name="corporateid">The corporateid.</param>
        /// <param name="facilityid">The facilityid.</param>
        /// <returns></returns>
         public List<MissingDataCustomModel> GetXMLMissingData(int corporateid, int facilityid)
         {
             using (var pinfoRep = UnitOfWork.PatientInfoRepository)
             {
                 var patientInfoLst = pinfoRep.GetXMLMissingData(corporateid, facilityid);
                 return patientInfoLst;
             }
         }

         /// <summary>
         /// Gets all XML bill header list.
         /// </summary>
         /// <param name="corporateId">The corporate identifier.</param>
         /// <param name="facilityId">The facility identifier.</param>
         /// <returns></returns>
         public List<BillHeaderCustomModel> GetAllXMLBillHeaderList(int corporateId, int facilityId)
         {
             try
             {
                 var list = new List<BillHeaderCustomModel>();
                 using (var billHeaderRep = UnitOfWork.BillHeaderRepository)
                 {
                     var lstBillHeader = corporateId > 0 ?
                         billHeaderRep.Where(a => (a.IsDeleted == null || !(bool)a.IsDeleted) && a.CorporateID != null && (int)a.CorporateID == corporateId && (int)a.FacilityID == facilityId).OrderByDescending(b => b.BillHeaderID).ThenByDescending(b => b.PatientID).ToList() :
                         billHeaderRep.Where(a => (a.IsDeleted == null || !(bool)a.IsDeleted)).OrderByDescending(b => b.BillHeaderID).ThenByDescending(b => b.PatientID).ToList();

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
                             InsuranceCompany = string.IsNullOrEmpty(i.PayerID) ? "-" : GetInsuranceCompanyNameByPayerId(i.PayerID),
                             PatientName = GetPatientNameById(Convert.ToInt32(i.PatientID)),
                             BStatus = Convert.ToInt32(i.Status),
                             MCDiscount = i.MCDiscount,
                             EncounterStatus = GetEncounterStatusById(Convert.ToInt32(i.EncounterID)),
                             EncounterType = GetEncounterHomeCareRecuuringById(Convert.ToInt32(i.EncounterID))
                         }));
                         list = list.ToList();
                     }
                 }
                 return list;
             }
             catch (Exception ex)
             {
                 throw ex;
             }
         }
    }
}
