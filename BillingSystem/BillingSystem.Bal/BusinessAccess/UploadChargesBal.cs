using System.Collections.Generic;
using System.Linq;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.BusinessAccess
{
    public class UploadChargesBal : BaseBal
    {
        /// <summary>
        ///     Gets the x payment return denial claims.
        /// </summary>
        /// <param name="common">The common.</param>
        /// <returns></returns>
        public List<PatientInfoXReturnPaymentCustomModel> GetXPaymentReturnDenialClaims(CommonModel common)
        {
            using (var patientinforep = UnitOfWork.PatientInfoRepository)
            {
                var patientInfoLst =
                    patientinforep.GetXPaymentReturnDenialClaims(common.CorporateId,
                        common.FacilityId);
                var returnCustomLst = new List<PatientInfoXReturnPaymentCustomModel>();
                if (patientInfoLst.Count > 0)
                {
                    if (!string.IsNullOrEmpty(common.PersonLastName))
                        returnCustomLst.AddRange(
                            patientInfoLst.Where(
                                p => p.PersonLastName.ToLower().Contains(common.PersonLastName.ToLower())).ToList());
                    if (!string.IsNullOrEmpty(common.PersonPassportNumber))
                        returnCustomLst.AddRange(
                            patientInfoLst.Where(
                                p =>
                                    p.PersonPassportNumber != null &&
                                    p.PersonPassportNumber.ToLower().Contains(common.PersonPassportNumber.ToLower()))
                                .ToList());
                    if (!string.IsNullOrEmpty(common.PersonEmiratesIDNumber))
                        returnCustomLst.AddRange(
                            patientInfoLst.Where(
                                p =>
                                    p.PersonEmiratesIDNumber != null && p.PersonEmiratesIDNumber.ToLower()
                                        .Contains(common.PersonEmiratesIDNumber.ToLower())).ToList());

                    if (common.PersonBirthDate.HasValue)
                        returnCustomLst.AddRange(
                            patientInfoLst.Where(
                                p => p.PersonBirthdate.Date.Equals(common.PersonBirthDate.Value.Date)).ToList());

                    if (!string.IsNullOrEmpty(common.ContactMobilePhone))
                        returnCustomLst.AddRange(
                            patientInfoLst.Where(
                                p =>
                                    !string.IsNullOrEmpty(p.PhoneNo) &&
                                    p.PhoneNo.ToLower().Contains(common.ContactMobilePhone.ToLower())).ToList());
                }
                return returnCustomLst.ToList();
            }
        }

        /// <summary>
        ///     Gets the bill details by bill header identifier.
        /// </summary>
        /// <param name="billHeaderId">The bill header identifier.</param>
        /// <returns></returns>
        public List<BillDetailCustomModel> GetBillDetailsByBillHeaderId(int billHeaderId)
        {
            using (var billHeaderinforep = UnitOfWork.BillHeaderRepository)
            {
                var billheaderactivitiesList =
                    billHeaderinforep.GetBillDetailsByBillHeaderId(billHeaderId);
                var totalrow = new BillDetailCustomModel
                {
                    ActivityType = string.Empty,
                    ActivityTypeName = string.Empty,
                    ActivityCode = string.Empty,
                    BillNumber = string.Empty,
                    BillHeaderID = 0,
                    BillActivityID = 0,
                    CorporateID = 0,
                    EncounterID = 0,
                    FacilityID = 0,
                    OrderedOn = null,
                    ExecutedOn = null,
                    QuantityOrdered = null,
                    ActivityCodeDescription = "Total",
                    GrossCharges = billheaderactivitiesList.Sum(x => x.GrossCharges),
                    PayerShareNet = billheaderactivitiesList.Sum(x => x.PayerShareNet),
                    PatientShare = billheaderactivitiesList.Sum(x => x.PatientShare),
                    GrossChargesSum =
                        billheaderactivitiesList.Sum(x => x.PayerShareNet) +
                        billheaderactivitiesList.Sum(x => x.PatientShare)
                };
                billheaderactivitiesList.Add(totalrow);
                return billheaderactivitiesList;
            }
        }

        /// <summary>
        ///     Deletes the bill activity.
        /// </summary>
        /// <param name="billActivityId">The bill activity identifier.</param>
        /// <param name="userid">The userid.</param>
        /// <param name="billHeaderId">The bill header identifier.</param>
        /// <returns></returns>
        public List<BillDetailCustomModel> DeleteBillActivity(int billActivityId, int userid, int billHeaderId)
        {
            using (var rep = UnitOfWork.BillHeaderRepository)
            {
                var billheaderactivitiesList = new List<BillDetailCustomModel>();
                var isupdated = rep.DeleteBillActivity(billActivityId, userid);
                if (isupdated)
                {
                    billheaderactivitiesList = rep.GetBillDetailsByBillHeaderId(billHeaderId);

                    var totalrow = new BillDetailCustomModel
                    {
                        ActivityType = string.Empty,
                        ActivityTypeName = string.Empty,
                        ActivityCode = string.Empty,
                        BillNumber = string.Empty,
                        BillHeaderID = 0,
                        BillActivityID = 0,
                        CorporateID = 0,
                        EncounterID = 0,
                        FacilityID = 0,
                        OrderedOn = null,
                        ExecutedOn = null,
                        QuantityOrdered = null,
                        ActivityCodeDescription = "Total",
                        GrossCharges = billheaderactivitiesList.Sum(x => x.GrossCharges),
                        PayerShareNet = billheaderactivitiesList.Sum(x => x.PayerShareNet),
                        PatientShare = billheaderactivitiesList.Sum(x => x.PatientShare),
                        GrossChargesSum =
                            billheaderactivitiesList.Sum(x => x.PayerShareNet) +
                            billheaderactivitiesList.Sum(x => x.PatientShare)
                    };
                    billheaderactivitiesList.Add(totalrow);
                }
                return billheaderactivitiesList;
            }
        }


        public List<PatientInfoXReturnPaymentCustomModel> GetXPaymentReturnDenialClaimsByPatientId(int patientId, int encounterId, int billHeaderId)
        {
            using (var patientinforep = UnitOfWork.PatientInfoRepository)
            {
                var patientInfoLst =
                    patientinforep.GetXPaymentReturnDenialClaimsByPatientId(patientId, encounterId, billHeaderId);
                return patientInfoLst;
            }
        }

        /// <summary>
        /// Gets the virtual discharge details.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <param name="billHeaderId">The bill header identifier.</param>
        /// <returns></returns>
        //public List<PatientInfoXReturnPaymentCustomModel> GetVirtualDischargeDetails(int patientId, int encounterId, int billHeaderId)
        //{
        //    using (var patientinforep = UnitOfWork.PatientInfoRepository)
        //    {
        //        var patientInfoLst =
        //            patientinforep.GetXPaymentReturnDenialClaimsByPatientId(patientId, encounterId, billHeaderId);
        //        return patientInfoLst;
        //    }
        //}
        //public List<BillDetailCustomModel> DeleteBillActivityById(int billActivityId, int userid, int billHeaderId)
        //{
        //    try
        //    {
        //        using (BillHeaderRepository billHeaderinforep = UnitOfWork.BillHeaderRepository)
        //        {
        //            var billheaderactivitiesList = new List<BillDetailCustomModel>();
        //            bool isupdated = billHeaderinforep.AddUpdateManualCharges(billActivityId, userid);
        //            if (isupdated)
        //            {
        //                billheaderactivitiesList = billHeaderinforep.GetBillDetailsByBillHeaderId(billHeaderId);
        //            }
        //            return billheaderactivitiesList;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
    }
}