using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Repository.UOW;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.BusinessAccess
{
    public class XactivityBal : BaseBal
    {
        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>
        /// Return the Entity List
        /// </returns>
        public List<XActivityCustomModel> GetXactivity()
        {
            var list = new List<XActivityCustomModel>();
            using (var xactivityRep = UnitOfWork.XactivityRepository)
            {
                var lstXactivity = xactivityRep.GetAll().ToList();
                if (lstXactivity.Count > 0)
                {
                    list.AddRange(lstXactivity.Select(item => new XActivityCustomModel
                    {

                    }));
                }
            }
            return list;
        }

        /// <summary>
        /// Gets the x activities.
        /// </summary>
        /// <returns></returns>
        public List<XActivity> GetXActivities()
        {
            using (var xactivityRep = UnitOfWork.XactivityRepository)
            {
                var lstXactivity = xactivityRep.GetAll().ToList();
                return lstXactivity;
            }
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public int SaveXactivity(XActivity model)
        {
            using (var rep = UnitOfWork.XactivityRepository)
            {
                if (model.XActivity1 > 0)
                    rep.UpdateEntity(model, Convert.ToInt32(model.XActivity1));
                else
                    rep.Create(model);
                return Convert.ToInt32(model.XActivity1);
            }
        }


        /// <summary>
        /// Gets the xactivity by encounter identifier.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        public List<XActivityCustomModel> GetXactivityByEncounterId(int? encounterId)
        {
            var list = new List<XActivityCustomModel>();
            using (var rep = UnitOfWork.XactivityRepository)
            {
                var model = rep.Where(x => x.EncounterID == encounterId).ToList();
                if (model.Count > 0)
                {
                    list.AddRange(model.Select(item => new XActivityCustomModel
                    {
                        XActivity1 = item.XActivity1,
                        EncounterID = item.EncounterID,
                        ActivityID = item.ActivityID,
                        DType = item.DType,
                        DCode = item.DCode,
                        StartDate = item.StartDate,
                        AType = item.AType,
                        ACode = item.ACode,
                        Quantity = item.Quantity,
                        OrderingClinician = item.OrderingClinician,
                        OrderDate = item.OrderDate,
                        Clinician = item.Clinician,
                        OrderCloseDate = item.OrderCloseDate,
                        PriorAuthorizationID = item.PriorAuthorizationID,
                        Gross = item.Gross,
                        PatientShare = item.PatientShare,
                        Net = item.Net,
                        DenialCode = item.DenialCode,
                        PaymentReference = item.PaymentReference,
                        DateSettlement = item.DateSettlement,
                        PaymentAmount = item.PaymentAmount,
                        PatientPayReference = item.PatientPayReference,
                        PatientDateSettlement = item.PatientDateSettlement,
                        PatientPayAmount = item.PatientPayAmount,
                        Status = item.Status,
                        ClaimID = item.ClaimID,
                        FileID = item.FileID,
                        ARFileID = item.ARFileID,
                        ModifiedBy = item.ModifiedBy,
                        ModifiedDate = item.ModifiedDate,
                        ActivityType = GetNameByGlobalCodeId(Convert.ToInt32(item.AType)),
                        MCDiscount = item.MCDiscount,
                       
                    }));
                }
                return list;
            }
        }

        /// <summary>
        /// Gets the xactivity by claim identifier.
        /// </summary>
        /// <param name="claimId">The claim identifier.</param>
        /// <returns></returns>
        public List<XActivityCustomModel> GetXactivityByClaimId(int? claimId)
        {
            var list = new List<XActivityCustomModel>();
            using (var rep = UnitOfWork.XactivityRepository)
            {
                var model = rep.Where(x => x.ClaimID == claimId).ToList();
                if (model.Count > 0)
                {
                    list.AddRange(model.Select(item => new XActivityCustomModel
                    {
                        XActivity1 = item.XActivity1,
                        EncounterID = item.EncounterID,
                        ActivityID = item.ActivityID,
                        DType = item.DType,
                        DCode = item.DCode,
                        StartDate = item.StartDate,
                        AType = item.AType,
                        ACode = item.ACode,
                        Quantity = item.Quantity,
                        OrderingClinician = item.OrderingClinician,
                        OrderDate = item.OrderDate,
                        Clinician = item.Clinician,
                        OrderCloseDate = item.OrderCloseDate,
                        PriorAuthorizationID = item.PriorAuthorizationID,
                        Gross = item.Gross,
                        PatientShare = item.PatientShare,
                        Net = item.Net,
                        DenialCode = item.DenialCode,
                        PaymentReference = item.PaymentReference,
                        DateSettlement = item.DateSettlement,
                        PaymentAmount = item.PaymentAmount,
                        PatientPayReference = item.PatientPayReference,
                        PatientDateSettlement = item.PatientDateSettlement,
                        PatientPayAmount = item.PatientPayAmount,
                        Status = item.Status,
                        ClaimID = item.ClaimID,
                        FileID = item.FileID,
                        ARFileID = item.ARFileID,
                        ModifiedBy = item.ModifiedBy,
                        ModifiedDate = item.ModifiedDate,
                        ActivityType = GetNameByGlobalCodeId(Convert.ToInt32(item.AType)),
                        MCDiscount = item.MCDiscount,
                    }));
                }
                return list;
            }
        }
    }
}
