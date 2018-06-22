using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using System.Transactions;
using BillingSystem.Common.Common;
using Elmah.ContentSyndication;

namespace BillingSystem.Bal.BusinessAccess
{
    public class BillHeaderBal : BaseBal
    {
        public BillHeaderBal()
        {

        }

        public BillHeaderBal(string cptTableNumber, string serviceCodeTableNumber, string drgTableNumber, string drugTableNumber, string hcpcsTableNumber, string diagnosisTableNumber)
        {
            if (!string.IsNullOrEmpty(cptTableNumber))
                CptTableNumber = cptTableNumber;

            if (!string.IsNullOrEmpty(serviceCodeTableNumber))
                ServiceCodeTableNumber = serviceCodeTableNumber;

            if (!string.IsNullOrEmpty(drgTableNumber))
                DrgTableNumber = drgTableNumber;

            if (!string.IsNullOrEmpty(drugTableNumber))
                DrugTableNumber = drugTableNumber;

            if (!string.IsNullOrEmpty(hcpcsTableNumber))
                HcpcsTableNumber = hcpcsTableNumber;

            if (!string.IsNullOrEmpty(diagnosisTableNumber))
                DiagnosisTableNumber = diagnosisTableNumber;
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
                using (var billHeaderRep = UnitOfWork.BillHeaderRepository)
                {
                    var lstBillHeader = corporateId > 0 ?
                        billHeaderRep.Where(a => (a.IsDeleted == null || !(bool)a.IsDeleted && a.CorporateID != null && (int)a.CorporateID == corporateId && (int)a.FacilityID == facilityId)
                        && a.EncounterID == encounterId).OrderByDescending(b => b.BillDate).ToList() :
                        billHeaderRep.Where(a => (a.IsDeleted == null || !(bool)a.IsDeleted)
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
                            EncounterNumber = GetEncounterNumberById(encounterId),
                            InsuranceCompany = string.IsNullOrEmpty(i.PayerID) ? "-" : GetInsuranceCompanyNameByPayerId(i.PayerID),
                            PatientName = GetPatientNameById(Convert.ToInt32(i.PatientID)),
                            BStatus = Convert.ToInt32(i.Status),
                            BillHeaderStatus = GetNameByGlobalCodeValue(i.Status, Convert.ToInt32(GlobalCodeCategoryValue.BillHeaderStatus).ToString()),
                            MCDiscount = i.MCDiscount,
                            GrossChargesSum = i.PatientShare + i.PayerShareNet,
                            EncounterStatus = GetEncounterStatusById(Convert.ToInt32(i.EncounterID)),
                            ActivityCost = i.ActivityCost
                        }));
                        list = list.Where(a => a.BStatus <= 45).ToList();
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
                using (var billHeaderRep = UnitOfWork.BillHeaderRepository)
                {
                    var lstBillHeader = corporateId > 0
                        ? billHeaderRep.Where(
                            a =>
                                (a.IsDeleted == null ||
                                 !(bool)a.IsDeleted && a.CorporateID != null && (int)a.CorporateID == corporateId &&
                                 (int)a.FacilityID == facilityId)
                                && (int)a.PatientID == patientId).OrderByDescending(b => b.BillHeaderID).ToList()
                        : billHeaderRep.Where(a => (a.IsDeleted == null || !(bool)a.IsDeleted)
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
                            EncounterNumber = GetEncounterNumberById(patientId),
                            PatientName = GetPatientNameById(Convert.ToInt32(i.PatientID)),
                            BStatus = Convert.ToInt32(i.Status),
                            MCDiscount = i.MCDiscount,
                            GrossChargesSum = i.PatientShare + i.PayerShareNet,
                            EncounterStatus = GetEncounterStatusById(Convert.ToInt32(i.EncounterID)),
                            InsuranceCompany = string.IsNullOrEmpty(i.PayerID) ? "-" : GetInsuranceCompanyNameByPayerId(i.PayerID),
                            ActivityCost = i.ActivityCost
                        }));

                        list = list.Where(a => a.BStatus <= 45).ToList();
                    }
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
                using (var billHeaderRep = UnitOfWork.BillHeaderRepository)
                {
                    list = billHeaderRep.GetBillHeaderList(facilityId, corporateId);
                    //var lstBillHeader = corporateId > 0 ?
                    //    billHeaderRep.Where(a => (a.IsDeleted == null || !(bool)a.IsDeleted) && a.CorporateID != null && (int)a.CorporateID == corporateId && (int)a.FacilityID == facilityId).OrderByDescending(b => b.BillHeaderID).ThenByDescending(b => b.PatientID).ToList() :
                    //    billHeaderRep.Where(a => (a.IsDeleted == null || !(bool)a.IsDeleted)).OrderByDescending(b => b.BillHeaderID).ThenByDescending(b => b.PatientID).ToList();

                    //if (lstBillHeader.Count > 0)
                    //{
                    //    lstBillHeader = lstBillHeader.Where(x => Convert.ToInt32(x.Status) <= 45).ToList();
                    //    list.AddRange(lstBillHeader.Select(i => new BillHeaderCustomModel
                    //    {
                    //        BillHeaderID = i.BillHeaderID,
                    //        BillNumber = i.BillNumber,
                    //        BillDate = i.BillDate,
                    //        CorporateID = i.CorporateID,
                    //        FacilityID = i.FacilityID,
                    //        PatientID = i.PatientID,
                    //        EncounterID = i.EncounterID,
                    //        PayerID = i.PayerID,
                    //        MemberID = i.MemberID,
                    //        Gross = i.Gross,
                    //        GrossChargesSum = i.PatientShare + i.PayerShareNet,
                    //        PatientShare = i.PatientShare,
                    //        PayerShareNet = i.PayerShareNet,
                    //        Status = GetBillHeaderStatus(i.Status),
                    //        DenialCode = i.DenialCode,
                    //        PaymentReference = i.PaymentReference,
                    //        DateSettlement = i.DateSettlement,
                    //        PaymentAmount = i.PaymentAmount,
                    //        PatientPayReference = i.PatientPayReference,
                    //        PatientDateSettlement = i.PatientDateSettlement,
                    //        PatientPayAmount = i.PatientPayAmount,
                    //        ClaimID = i.ClaimID,
                    //        FileID = i.FileID,
                    //        ARFileID = i.ARFileID,
                    //        CreatedBy = i.CreatedBy,
                    //        CreatedDate = i.CreatedDate,
                    //        ModifiedBy = i.ModifiedBy,
                    //        ModifiedDate = i.ModifiedDate,
                    //        IsDeleted = i.IsDeleted,
                    //        DeletedBy = i.DeletedBy,
                    //        DeletedDate = i.DeletedDate,
                    //        AuthID = i.AuthID,
                    //        AuthCode = i.AuthCode,
                    //        MCID = i.MCID,
                    //        MCPatientShare = i.MCPatientShare,
                    //        MCMultiplier = i.MCMultiplier,
                    //        CorporateName = GetNameByCorporateId(Convert.ToInt32(i.CorporateID)),
                    //        FacilityName = GetFacilityNameByFacilityId(Convert.ToInt32(i.FacilityID)),
                    //        EncounterNumber = GetEncounterNumberById(Convert.ToInt32(i.EncounterID)),
                    //        InsuranceCompany = string.IsNullOrEmpty(i.PayerID) ? "-" : GetInsuranceCompanyNameByPayerId(Convert.ToInt32(i.PayerID)),
                    //        PatientName = GetPatientNameById(Convert.ToInt32(i.PatientID)),
                    //        BStatus = Convert.ToInt32(i.Status),
                    //        MCDiscount = i.MCDiscount,
                    //        EncounterStatus = GetEncounterStatusById(Convert.ToInt32(i.EncounterID)),
                    //        EncounterType = GetEncounterHomeCareRecuuringById(Convert.ToInt32(i.EncounterID)),
                    //        ActivityCost = i.ActivityCost
                    //    }));
                    //}
                        list = list.Where(a => a.EncounterStatus == "Active").ToList();
                }
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
            using (var BillHeaderRep = UnitOfWork.BillHeaderRepository)
            {
                using (var bal = new ErrorMasterBal())
                {
                    var model = BillHeaderRep.Where(x => x.BillHeaderID == billHeaderId).FirstOrDefault();
                    if (model != null)
                    {
                        customModel = new BillHeaderCustomModel
                        {
                            BillHeaderID = model.BillHeaderID,
                            BillNumber = model.BillNumber,
                            BillDate = model.BillDate,
                            CorporateID = model.CorporateID,
                            FacilityID = model.FacilityID,
                            PatientID = model.PatientID,
                            EncounterID = model.EncounterID,
                            PayerID = model.PayerID,
                            MemberID = model.MemberID,
                            Gross = model.Gross,
                            PatientShare = model.PatientShare,
                            PayerShareNet = model.PayerShareNet,
                            Status = GetBillHeaderStatus(model.Status),
                            DenialCode = model.DenialCode,
                            PaymentReference = model.PaymentReference,
                            DateSettlement = model.DateSettlement,
                            PaymentAmount = model.PaymentAmount,
                            PatientPayReference = model.PatientPayReference,
                            PatientDateSettlement = model.PatientDateSettlement,
                            PatientPayAmount = model.PatientPayAmount,
                            ClaimID = model.ClaimID,
                            FileID = model.FileID,
                            ARFileID = model.ARFileID,
                            CreatedBy = model.CreatedBy,
                            CreatedDate = model.CreatedDate,
                            ModifiedBy = model.ModifiedBy,
                            ModifiedDate = model.ModifiedDate,
                            IsDeleted = model.IsDeleted,
                            DeletedBy = model.DeletedBy,
                            DeletedDate = model.DeletedDate,
                            AuthID = model.AuthID,
                            AuthCode = model.AuthCode,
                            MCID = model.MCID,
                            MCPatientShare = model.MCPatientShare,
                            MCMultiplier = model.MCMultiplier,
                            CorporateName = GetNameByCorporateId(Convert.ToInt32(model.CorporateID)),
                            FacilityName = GetFacilityNameByFacilityId(Convert.ToInt32(model.FacilityID)),
                            EncounterNumber = GetEncounterNumberById(Convert.ToInt32(model.EncounterID)),
                            MCDiscount = model.MCDiscount,
                            DueDate = model.DueDate,
                            GrossChargesSum = model.PatientShare + model.PayerShareNet,
                            EncounterStatus = GetEncounterStatusById(Convert.ToInt32(model.EncounterID)),
                            InsuranceCompany = string.IsNullOrEmpty(model.PayerID) ? "-" : GetInsuranceCompanyNameByPayerId(model.PayerID),
                            ActivityCost = model.ActivityCost
                        };
                        var denialcodedesc = bal.GetSearchedDenialsList(model.DenialCode).FirstOrDefault();
                        if (denialcodedesc != null)
                        {
                            customModel.DenialCodeDescritption = string.Format("{0} - {1}", denialcodedesc.ErrorCode,
                                denialcodedesc.ErrorDescription);
                        }
                    }
                }
            }
            return customModel;
        }

        /// <summary>
        /// Gets the bill header detail by current encounter.
        /// </summary>
        /// <param name="mostRecentEncounterId">The most recent encounter identifier.</param>
        /// <returns></returns>
        public BillHeaderCustomModel GetBillHeaderDetailByCurrentEncounter(int mostRecentEncounterId)
        {
            BillHeaderCustomModel bHeader = null;
            using (var billHeaderRep = UnitOfWork.BillHeaderRepository)
            {
                var item = billHeaderRep.Where(x => x.EncounterID == mostRecentEncounterId).FirstOrDefault();
                if (item != null)
                {
                    using (var patientBal = new PatientInfoBal())
                    {
                        bHeader = new BillHeaderCustomModel
                                    {
                                        BillHeaderID = item.BillHeaderID,
                                        BillNumber = item.BillNumber,
                                        BillDate = item.BillDate,
                                        CorporateID = item.CorporateID,
                                        FacilityID = item.FacilityID,
                                        PatientID = item.PatientID,
                                        EncounterID = item.EncounterID,
                                        PayerID = item.PayerID,
                                        MemberID = item.MemberID,
                                        Gross = item.Gross,
                                        PatientShare = item.PatientShare,
                                        PayerShareNet = item.PayerShareNet,
                                        Status = item.Status,
                                        DenialCode = item.DenialCode,
                                        PaymentReference = item.PaymentReference,
                                        DateSettlement = item.DateSettlement,
                                        PaymentAmount = item.PaymentAmount,
                                        PatientPayReference = item.PatientPayReference,
                                        PatientDateSettlement = item.PatientDateSettlement,
                                        PatientPayAmount = item.PatientPayAmount,
                                        ClaimID = item.ClaimID,
                                        FileID = item.FileID,
                                        ARFileID = item.ARFileID,
                                        CreatedBy = item.CreatedBy,
                                        CreatedDate = item.CreatedDate,
                                        ModifiedBy = item.ModifiedBy,
                                        ModifiedDate = item.ModifiedDate,
                                        IsDeleted = item.IsDeleted,
                                        DeletedBy = item.DeletedBy,
                                        DeletedDate = item.DeletedDate,
                                        CorporateName = GetNameByCorporateId(Convert.ToInt32(item.CorporateID)),
                                        FacilityName = GetFacilityNameByFacilityId(Convert.ToInt32(item.FacilityID)),
                                        MCDiscount = item.MCDiscount,
                                        DueDate = item.DueDate,
                                        GrossChargesSum = item.PatientShare + item.PayerShareNet,
                                        EncounterStatus = GetEncounterStatusById(Convert.ToInt32(item.EncounterID)),
                                        InsuranceCompany = string.IsNullOrEmpty(item.PayerID) ? "-" : GetInsuranceCompanyNameByPayerId(item.PayerID),
                                        ActivityCost = item.ActivityCost
                                    };
                    }
                }
            }
            return bHeader;
        }

        ///// <summary>
        ///// Sends the e claims.
        ///// </summary>
        ///// <param name="facilityId">The facility identifier.</param>
        ///// <returns></returns>
        //public IEnumerable<BillHeaderXMLModel> SendEClaims(int facilityId)
        //{
        //    using (var rep = UnitOfWork.BillHeaderRepository)
        //    {
        //        //foreach (var id in billHeaderIds)
        //        //{
        //        //    var billHeaderId = Convert.ToInt32(id);
        //        //    var billHeader = rep.Where(b => b.BillHeaderID == billHeaderId).FirstOrDefault();
        //        //    if (billHeader != null)
        //        //    {
        //        //        billHeader.Status = "1";
        //        //        rep.UpdateEntity(billHeader, billHeader.BillHeaderID);
        //        //        //status = true;
        //        //    }
        //        //}
        //        var xmlFilesList = rep.SendEClaims(facilityId, "Test");
        //        return xmlFilesList;
        //    }
        //}

        /*
         * Owner: Amit Jain
         * On: 25112014
         * Purpose: Apply the Bed Charges and Order Bill against the current Encounter ID
         */
        /// <summary>
        /// Applies the bed charges and order bill.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        public bool ApplyBedChargesAndOrderBill(int encounterId)
        {
            var result = false;
            using (var rep = UnitOfWork.BillHeaderRepository)
            {
                //Apply Bed Charges to the current Encounter ID
                result = rep.ApplyBedCharges(encounterId);

                //Apply Order Bill to the current Encounter ID
                result = rep.ApplyOrderBill(encounterId);
            }
            return result;
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
                using (var rep = UnitOfWork.BillHeaderRepository)
                {
                    //var list = rep.Where(b => billHeaderIds.Contains(b.BillHeaderID) && b.Status.Equals(oldStatus) && b.AuthID != null && b.MCID != null).ToList();
                    // Removed the MCID check to fetch the data.
                    var list = rep.Where(b => billHeaderIds.Contains(b.BillHeaderID) && b.Status.Equals(oldStatus) && b.AuthID != null).ToList();
                    if (list.Count > 0)
                    {
                        list.Select(b => { b.Status = status; return b; }).ToList();
                        rep.Update(list);
                        transScope.Complete();
                        result = true;
                    }
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
            using (var rep = UnitOfWork.BillHeaderRepository)
            {
                var updatedID = rep.UpdateEntity(model, model.BillHeaderID);
                return updatedID == null ? 0 : Convert.ToInt32(updatedID);
            }
        }

        /// <summary>
        /// Gets the bill header to update by identifier.
        /// </summary>
        /// <param name="billheaderid">The billheaderid.</param>
        /// <returns></returns>
        public BillHeader GetBillHeaderToUpdateById(int billheaderid)
        {
            using (var billHeaderRep = UnitOfWork.BillHeaderRepository)
            {
                var model = billHeaderRep.Where(x => x.BillHeaderID == billheaderid).FirstOrDefault();
                return model ?? new BillHeader();
            }
        }

        /// <summary>
        /// Gets all encounter ids in bill header.
        /// </summary>
        /// <returns></returns>
        public List<int> GetAllEncounterIdsInBillHeader()
        {
            try
            {
                using (var billHeaderRep = UnitOfWork.BillHeaderRepository)
                {
                    var lstEncounterIds = (from b in billHeaderRep.Where(c => c.EncounterID != null)
                                           select b.EncounterID.Value).ToList();
                    return lstEncounterIds.Distinct().ToList();
                }
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
                using (var billHeaderRep = UnitOfWork.BillHeaderRepository)
                {
                    var lstBillHeader = billHeaderRep.Where(a => a.IsDeleted == null || !(bool)a.IsDeleted
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
                            EncounterNumber = GetEncounterNumberById(encounterId),
                            //InsuranceCompany = string.IsNullOrEmpty(i.PayerID) ? "Self" : GetInsuranceCompanyNameByPayerId(Convert.ToInt32(i.PayerID)),
                            PatientName = GetPatientNameById(Convert.ToInt32(i.PatientID)),
                            MCDiscount = i.MCDiscount,
                            DueDate = i.DueDate,
                            GrossChargesSum = i.PatientShare + i.PayerShareNet,
                            EncounterStatus = GetEncounterStatusById(Convert.ToInt32(i.EncounterID)),
                            InsuranceCompany = string.IsNullOrEmpty(i.PayerID) ? "-" : GetInsuranceCompanyNameByPayerId(i.PayerID),
                            ActivityCost = i.ActivityCost
                        }));
                    }
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
            using (var rep = UnitOfWork.OrderActivityRepository)
            {
                rep.ApplyOrderActivityToBill(encounterId, corporateId, facilityId, string.Empty, 0);
                //rep.UpdateBillHeadersByEncounterId(encounterId);
                if (patientId > 0)
                    return GetBillHeaderListByPatientId(patientId, corporateId, facilityId);
                return isEncounterSelected ? GetBillHeaderListByEncounterId(encounterId, corporateId, facilityId) : GetAllBillHeaderList(corporateId, facilityId);
            }
        }

        /// <summary>
        /// Gets the bill headers by bill identifier.
        /// </summary>
        /// <param name="billid">The billid.</param>
        /// <returns></returns>
        public List<BillHeader> GetBillHeadersByBillId(int billid)
        {
            try
            {
                using (var billHeaderRep = UnitOfWork.BillHeaderRepository)
                {
                    var lstEncounterIds =
                        billHeaderRep.Where(x => x.BillHeaderID == billid && x.Status == "60").ToList();
                    return lstEncounterIds;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the bill header model list by encounter identifier.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        public List<BillHeader> GetBillHeaderModelListByEncounterId(int encounterId)
        {
            try
            {
                using (var billHeaderRep = UnitOfWork.BillHeaderRepository)
                {
                    var lstBillHeader = billHeaderRep.Where(a => a.IsDeleted == null || !(bool)a.IsDeleted
                        && a.EncounterID == encounterId).OrderByDescending(b => b.BillDate).ToList();
                    return lstBillHeader;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
                var finalStatusList = new List<int>
                {
                    Convert.ToInt32(BillHeaderStatus.F1),
                    Convert.ToInt32(BillHeaderStatus.F2),
                    Convert.ToInt32(BillHeaderStatus.F3)
                };

                using (var billHeaderRep = UnitOfWork.BillHeaderRepository)
                {
                    list = billHeaderRep.GetFinalBillHeadersList(
                        encounterSelected,
                        encounterId,
                        patientId,
                        facilityId,
                        corporateId);
                    //var lstBillHeader = encounterSelected
                    //    ? (corporateId > 0 ?
                    //    billHeaderRep.Where(
                    //        a => (a.IsDeleted == null || !(bool)a.IsDeleted) && (int)a.EncounterID == encounterId && (a.CorporateID != null && (int)a.CorporateID == corporateId) && (int)a.FacilityID == facilityId)
                    //        .OrderByDescending(b => b.BillHeaderID)
                    //        .ThenByDescending(b => b.PatientID)
                    //        .ToList() :
                    //        billHeaderRep.Where(
                    //        a => (a.IsDeleted == null || !(bool)a.IsDeleted) && (int)a.EncounterID == encounterId)
                    //        .OrderByDescending(b => b.BillHeaderID)
                    //        .ThenByDescending(b => b.PatientID)
                    //        .ToList())
                    //    : (
                    //        patientId > 0
                    //        ? (corporateId > 0 ? billHeaderRep.Where(
                    //                a => (a.IsDeleted == null || !(bool)a.IsDeleted) && (int)a.PatientID == patientId && (a.CorporateID != null && (int)a.CorporateID == corporateId) && (int)a.FacilityID == facilityId)
                    //                .OrderByDescending(b => b.BillHeaderID)
                    //                .ThenByDescending(b => b.PatientID)
                    //                .ToList() : billHeaderRep.Where(
                    //                a => (a.IsDeleted == null || !(bool)a.IsDeleted) && (int)a.PatientID == patientId)
                    //                .OrderByDescending(b => b.BillHeaderID)
                    //                .ThenByDescending(b => b.PatientID)
                    //                .ToList())
                    //            : (corporateId > 0 ? billHeaderRep.Where(a => (a.IsDeleted == null || !(bool)a.IsDeleted) && (a.CorporateID != null && (int)a.CorporateID == corporateId) && (int)a.FacilityID == facilityId)
                    //                .OrderByDescending(b => b.BillHeaderID)
                    //                .ThenByDescending(b => b.PatientID)
                    //                .ToList() : billHeaderRep.Where(a => (a.IsDeleted == null || !(bool)a.IsDeleted))
                    //                .OrderByDescending(b => b.BillHeaderID)
                    //                .ThenByDescending(b => b.PatientID)
                    //                .ToList())
                    //            );


                    //if (lstBillHeader.Count > 0)
                    //{
                    //    list.AddRange(lstBillHeader.Select(i => new BillHeaderCustomModel
                    //    {
                    //        BillHeaderID = i.BillHeaderID,
                    //        BillNumber = i.BillNumber,
                    //        BillDate = i.BillDate,
                    //        CorporateID = i.CorporateID,
                    //        FacilityID = i.FacilityID,
                    //        PatientID = i.PatientID,
                    //        EncounterID = i.EncounterID,
                    //        PayerID = i.PayerID,
                    //        MemberID = i.MemberID,
                    //        Gross = i.Gross,
                    //        PatientShare = i.PatientShare,
                    //        PayerShareNet = i.PayerShareNet,
                    //        Status = GetBillHeaderStatus(i.Status),
                    //        DenialCode = i.DenialCode,
                    //        PaymentReference = i.PaymentReference,
                    //        DateSettlement = i.DateSettlement,
                    //        PaymentAmount = i.PaymentAmount,
                    //        PatientPayReference = i.PatientPayReference,
                    //        PatientDateSettlement = i.PatientDateSettlement,
                    //        PatientPayAmount = i.PatientPayAmount,
                    //        ClaimID = i.ClaimID,
                    //        FileID = i.FileID,
                    //        ARFileID = i.ARFileID,
                    //        CreatedBy = i.CreatedBy,
                    //        CreatedDate = i.CreatedDate,
                    //        ModifiedBy = i.ModifiedBy,
                    //        ModifiedDate = i.ModifiedDate,
                    //        IsDeleted = i.IsDeleted,
                    //        DeletedBy = i.DeletedBy,
                    //        DeletedDate = i.DeletedDate,
                    //        AuthID = i.AuthID,
                    //        AuthCode = i.AuthCode,
                    //        MCID = i.MCID,
                    //        MCPatientShare = i.MCPatientShare,
                    //        MCMultiplier = i.MCMultiplier,
                    //        CorporateName = GetNameByCorporateId(Convert.ToInt32(i.CorporateID)),
                    //        FacilityName = GetFacilityNameByFacilityId(Convert.ToInt32(i.FacilityID)),
                    //        EncounterNumber = GetEncounterNumberById(Convert.ToInt32(i.EncounterID)),
                    //        PatientName = GetPatientNameById(Convert.ToInt32(i.PatientID)),
                    //        BStatus = Convert.ToInt32(i.Status),
                    //        MCDiscount = i.MCDiscount,
                    //        DueDate = i.DueDate,
                    //        GrossChargesSum = i.PatientShare + i.PayerShareNet,
                    //        EncounterStatus = GetEncounterStatusById(Convert.ToInt32(i.EncounterID)),
                    //        InsuranceCompany = string.IsNullOrEmpty(i.PayerID) ? "-" : GetInsuranceCompanyNameByPayerId(Convert.ToInt32(i.PayerID)),
                    //        EncounterPatientType = GetEncounterTypeById(Convert.ToInt32(i.EncounterID))
                    //    }));

                    //    list = list.Where(a => finalStatusList.Contains(a.BStatus)).ToList();
                    //}
                }
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
            try
            {
                using (var billHeaderRep = UnitOfWork.BillHeaderRepository)
                {
                    var result = billHeaderRep.GetPreXMLFile(billHeaderId, facilityId);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
            using (var rep = UnitOfWork.BillHeaderRepository)
            {
                rep.UpdateBillHeadersByBillHeaderIdEncounterId(encounterId, billheaderid);
                //rep.UpdateBillHeadersByEncounterId(encounterId);
                if (patientId > 0)
                    return GetBillHeaderListByPatientId(patientId, corporateId, facilityId);
                return isEncounterSelected ? GetBillHeaderListByEncounterId(encounterId, corporateId, facilityId) : GetAllBillHeaderList(corporateId, facilityId);
            }
        }

        /// <summary>
        /// Sets the preliminary bill status by encounter identifier.
        /// </summary>
        /// <param name="encounterids">The encounterids.</param>
        /// <returns></returns>
        public bool SetPreliminaryBillStatusByEncounterId(List<int> encounterids, int userId)
        {
           
            using (var rep = UnitOfWork.EncounterRepository)
            {
                var status = false;
                foreach (var encounterid in encounterids)
                {
                    var result = rep.GetEncounterEndCheck(encounterid, userId);
                    if (result.Any())
                    {
                        var encounterCheckReturnStatus = result.FirstOrDefault();
                        status = encounterCheckReturnStatus != null;
                    }
                }
                return status;
            }
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
                using (var billHeaderRep = UnitOfWork.BillHeaderRepository)
                {
                    var lstBillHeader = billHeaderRep.Where(a => a.IsDeleted == null || !(bool)a.IsDeleted
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
                            //InsuranceCompany = string.IsNullOrEmpty(i.PayerID) ? "Self" : GetInsuranceCompanyNameByPayerId(Convert.ToInt32(i.PayerID)),
                            PatientName = GetPatientNameById(Convert.ToInt32(i.PatientID)),
                            MCDiscount = i.MCDiscount,
                            DueDate = i.DueDate,
                            GrossChargesSum = i.PatientShare + i.PayerShareNet,
                            EncounterStatus = GetEncounterStatusById(Convert.ToInt32(i.EncounterID)),
                            InsuranceCompany = string.IsNullOrEmpty(i.PayerID) ? "-" : GetInsuranceCompanyNameByPayerId(i.PayerID),
                        }));
                    }
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
            using (var rep = UnitOfWork.BillHeaderRepository)
            {
                rep.ApplyBedCharges(encounterId);
                return encounterId;
            }
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
            using (var billHeaderRep = UnitOfWork.BillHeaderRepository)
            {
                billHeaderRep.RecaluclateBill(corporateId, facilityId, encounterId, reclaimFlag, calimId, userId);
                return true;
            }
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
            using (var rep = UnitOfWork.BillHeaderRepository)
            {
                rep.UpdateBillHeadersByBillHeaderIdEncounterId(encounterId, billheaderid);
                return true;
            }
        }


        /// <summary>
        /// XMLs the scrub bill.
        /// </summary>
        /// <param name="calimId">The calim identifier.</param>
        /// <param name="userid">The userid.</param>
        /// <returns></returns>
        public bool XMLScrubBill(int calimId, int userid)
        {
            using (var billHeaderRep = UnitOfWork.BillHeaderRepository)
            {
                var xmlScrubClaimStatus = billHeaderRep.ScrubXMLBill(calimId, userid);
                return xmlScrubClaimStatus;
            }
        }

        /// <summary>
        /// Gets the final bill payer headers list.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public List<BillHeaderCustomModel> GetFinalBillPayerHeadersList(int corporateId, int facilityId)
        {
            using (var billHeaderRep = UnitOfWork.BillHeaderRepository)
            {
                var xmlScrubClaimStatus = billHeaderRep.GetFinalBillPayerHeadersList(corporateId, facilityId);
                return xmlScrubClaimStatus;
            }
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
            using (var billHeaderRep = UnitOfWork.BillHeaderRepository)
            {
                var xmlScrubClaimStatus = billHeaderRep.GetFinalBillByPayerHeadersList(corporateId, facilityId, payerIds);
                return xmlScrubClaimStatus;
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
        public List<BillHeaderCustomModel> FindClaim(
            string serachstring,
            string claimstatus,
            DateTime? datefrom,
            DateTime? datetill,
            int facilityId,
            int corporateId)
        {
            var listtoRetrun = new List<BillHeaderCustomModel>();
            using (var rep = UnitOfWork.BillHeaderRepository)
            {
                listtoRetrun = rep.FindEClaims(
                    serachstring,
                    claimstatus,
                    datefrom,
                    datetill,
                    facilityId,
                    corporateId,
                    0);
            }

            return listtoRetrun;
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
            using (var rep = UnitOfWork.BillHeaderRepository)
            {
                var xmlFilesList = rep.SendEClaimsByPayerIds(facilityId, "Test", payerId, billHeaderIds);
                return xmlFilesList;
            }
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
        public List<BillHeaderCustomModel> FindClaimByFileId(
            string serachstring,
            string claimstatus,
            DateTime? datefrom,
            DateTime? datetill,
            int facilityId,
            int corporateId,
            int? fileid)
        {
            var listtoRetrun = new List<BillHeaderCustomModel>();
            using (var rep = UnitOfWork.BillHeaderRepository)
            {
                listtoRetrun = rep.FindEClaims(
                    serachstring,
                    claimstatus,
                    datefrom,
                    datetill,
                    facilityId,
                    corporateId,
                    fileid);
            }

            return listtoRetrun;
        }
    }
}