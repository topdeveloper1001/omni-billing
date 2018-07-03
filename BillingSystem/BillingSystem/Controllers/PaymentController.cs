// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PaymentController.cs" company="Spadez">
//   OmniHealthcare
// </copyright>
// <summary>
//   The payment controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using BillingSystem.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BillingSystem.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Models;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Controllers
{
    public class PaymentController : BaseController
    {
        private readonly IPatientInfoService _piService;
        private readonly IPaymentService _service;
        private readonly IPaymentTypeDetailService _ptdService;
        private readonly IEncounterService _eService;

        public PaymentController(IPatientInfoService piService, IPaymentService service, IPaymentTypeDetailService ptdService, IEncounterService eService)
        {
            _piService = piService;
            _service = service;
            _ptdService = ptdService;
            _eService = eService;
        }


        #region Public Methods and Operators

        /// <summary>
        ///     Applies the payment manual to bill.
        /// </summary>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult ApplyPaymentManualToBill()
        {
            int facilityId = Helpers.GetDefaultFacilityId();
            int corporateId = Helpers.GetSysAdminCorporateID();
            bool applyPaymnetManual = _service.ApplyManualPayment(corporateId, facilityId);
            return this.Json(applyPaymnetManual);
        }

        /// <summary>
        /// Binds the payment list.
        /// </summary>
        /// <param name="patientId">
        /// The patient identifier.
        /// </param>
        /// <param name="encounterId">
        /// The encounter identifier.
        /// </param>
        /// <param name="billHeaderId">
        /// The bill header identifier.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult BindPaymentList(int? patientId, int? encounterId, int? billHeaderId)
        {
            int corporateId = Helpers.GetDefaultCorporateId();
            int facilityId = Helpers.GetDefaultFacilityId();

            List<PaymentCustomModel> list = _service.GetPaymentBills(
                Convert.ToInt32(patientId),
                Convert.ToInt32(encounterId),
                Convert.ToInt32(billHeaderId),
                corporateId,
                facilityId);
            list = list.Where(x => x.PayType == 500).ToList(); // only show Manual Payments
            return this.PartialView(PartialViews.PaymentListView, list);
        }

        /// <summary>
        /// Gets the payment detail.
        /// </summary>
        /// <param name="paymentId">
        /// The payment identifier.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult GetPaymentDetail(int paymentId)
        {
            PaymentCustomModel result = _service.GetPaymentById(paymentId);
            var paymentList = _ptdService.GetPaymentTypeDetailByPaymentId(paymentId) != null ? _ptdService.GetPaymentTypeDetailByPaymentId(paymentId) : new PaymentTypeDetail();

            if (result == null)
            {
                return this.Json(null);
            }

            var jsonResult =
                new
                {
                    result.PayedDate,
                    PaymentDate = result.PayDate,
                    result.PayReference,
                    PatientFor = result.PayFor,
                    BillNumber = result.PayBillNumber,
                    BillId = result.PayBillID,
                    EncounterId = result.PayEncounterID,
                    CreatedDate = result.PayCreatedDate,
                    CreatedBy = result.PayCreatedBy,
                    result.PayAmount,
                    result.PaymentID,
                    paymentList.CardHolderName,
                    paymentList.CardNumber,
                    paymentList.ExpiryMonth,
                    paymentList.ExpiryYear,
                    result.PaymentTypeId,
                    paymentList.PaymentType,
                    paymentList.ExtValue1


                };

            return this.Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Indexes the specified patient identifier.
        /// </summary>
        /// <param name="patientId">
        /// The patient identifier.
        /// </param>
        /// <param name="encounterId">
        /// The encounter identifier.
        /// </param>
        /// <param name="billHeaderId">
        /// The bill header identifier.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult Index(int? patientId, int? encounterId, int? billHeaderId)
        {
            var corporateId = Helpers.GetSysAdminCorporateID();
            var facilityId = Helpers.GetDefaultFacilityId();

            var list = _service.GetPaymentBills(
                Convert.ToInt32(patientId),
                Convert.ToInt32(encounterId),
                Convert.ToInt32(billHeaderId),
                corporateId,
                facilityId);

            list = list.Where(x => x.PayType == 500).ToList(); // only show User entered Payments
            var paymentsView = new PaymentsView
            {
                PaymentsList = list,
                CurrentPayment = new PaymentCustomModel()
                {
                    CurrentPaymentTypeDetail = new PaymentTypeDetail()
                },
                PatientSearchList = new List<PatientInfoXReturnPaymentCustomModel>(),
                PatientSearch = new PatientInfo(),
                //CurrentPaymentTypeDetail = new PaymentTypeDetail()
            };
            return View(paymentsView);
        }

        /// <summary>
        /// Saves the manual payment.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        //public ActionResult SaveManualPayment(Payment model)
        //{
        //    using (var _service = new PaymentBal())
        //    {
        //        DateTime currentDateTime = Helpers.GetInvariantCultureDateTime();
        //        int userId = Helpers.GetLoggedInUserId();
        //        model.PayCorporateID = Helpers.GetSysAdminCorporateID();
        //        model.PayFacilityID = Helpers.GetDefaultFacilityId();
        //        if (model.PaymentID > 0)
        //        {
        //            model.PayModifiedBy = userId;
        //            model.PayModifiedDate = currentDateTime;
        //        }
        //        else
        //        {
        //            model.PayCreatedBy = userId;
        //            model.PayCreatedDate = currentDateTime;
        //        }

        //        long result = _service.SavePayments(model);

        //        // Added Code here to apply the manual payment to the Billheader.
        //        using (var paymentBal = new PaymentBal())
        //        {
        //            int corporateId = Helpers.GetDefaultCorporateId();
        //            int facilityId = Helpers.GetDefaultFacilityId();
        //            bool applyPaymnetManual = paymentBal.ApplyManualPayment(corporateId, facilityId);
        //        }

        //        return this.Json(result);
        //    }
        //}



        public ActionResult GetPatientInfoSearchResult(CommonModel common)
        {
            var facilityid = Helpers.GetDefaultFacilityId();
            var corporateid = Helpers.GetSysAdminCorporateID();
            common.FacilityId = facilityid;
            common.CorporateId = corporateid;

            var objPatientInfoData = _piService.GetPatientSearchResultInPayment(common);

            //ViewBag.Message = null;

            //if (objPatientInfoData.Count > 0)
            //{
            //    using (var rolebal = new RoleTabsBal())
            //    {
            //        var roleId = Helpers.GetDefaultRoleId();
            //        objPatientInfoData[0].PatientInfoAccessible = rolebal.CheckIfTabNameAccessibleToGivenRole("Register new patient", ControllerAccess.PatientInfo.ToString(), ActionNameAccess.RegisterPatient.ToString(), Convert.ToInt32(roleId));
            //        objPatientInfoData[0].EhrViewAccessible = rolebal.CheckIfTabNameAccessibleToGivenRole("EHR", ControllerAccess.Summary.ToString(), ActionNameAccess.PatientSummary.ToString(), Convert.ToInt32(roleId));
            //        objPatientInfoData[0].AuthorizationViewAccessible = rolebal.CheckIfTabNameAccessibleToGivenRole("Obtain Insurance Authorization", ControllerAccess.Authorization.ToString(), ActionNameAccess.AuthorizationMain.ToString(), Convert.ToInt32(roleId));
            //        objPatientInfoData[0].BillHeaderViewAccessible =
            //            rolebal.CheckIfTabNameAccessibleToGivenRole("Generate Preliminary Bill",
            //                ControllerAccess.BillHeader.ToString(), ActionNameAccess.Index.ToString(),
            //                Convert.ToInt32(roleId));

            //        var objSession = Session[SessionNames.SessionClass.ToString()] as SessionClass;
            //        if (objSession != null)
            //        {
            //            objPatientInfoData[0].SchedularViewAccessible = objSession.SchedularAccessible;
            //        }
            //    }
            //}
            return PartialView(PartialViews.PatientSearchResultList, objPatientInfoData);
        }

        //public ActionResult GetPatientInfoSearchResult1(CommonModel common)
        //{
        //    var facilityid = Helpers.GetDefaultFacilityId();
        //    var corporateid = Helpers.GetSysAdminCorporateID();
        //    common.FacilityId = facilityid;
        //    common.CorporateId = corporateid;
        //    var _service = new UploadChargesBal();
        //    var objPatientInfoData = _service.GetXPaymentReturnDenialClaims(common);
        //    ViewBag.Message = null;
        //   return PartialView(PartialViews.PatientSearchResultList, objPatientInfoData);
        //}




        public ActionResult GetPatientName(int patientId)
        {
            var patientName = _piService.GetPatientNameById(patientId);
            return Json(patientName, JsonRequestBehavior.AllowGet);
        }



        public ActionResult SaveManualPayment(PaymentCustomModel m)
        {
            var ptd = new PaymentTypeDetail();

            var currentDateTime = Helpers.GetInvariantCultureDateTime();
            var userId = Helpers.GetLoggedInUserId();
            m.PayCorporateID = Helpers.GetSysAdminCorporateID();
            m.PayFacilityID = Helpers.GetDefaultFacilityId();

            /*----Saving Payment Details to Payment Section--*/
            if (m.PaymentID > 0)
            {
                ptd = _ptdService.GetPaymentTypeDetailByPaymentId(Convert.ToInt32(m.PaymentID));
                m.PayModifiedBy = userId;
                m.PayModifiedDate = currentDateTime;
                m.PaymentTypeId = Convert.ToInt32(m.PTDPaymentType);
            }
            else
            {
                m.PayCreatedBy = userId;
                m.PayCreatedDate = currentDateTime;
                m.PaymentTypeId = Convert.ToInt32(m.PTDPaymentType);
            }
            var result = _service.SaveCustomPayments(m);
            /*----Saving Payment Details to Payment Section--*/


            /*----Saving Payment-Type Details to Payment Section--*/
            var paymentTypeDetail = new PaymentTypeDetail
            {
                Id = ptd.Id,
                PaymentType = Convert.ToString(m.PaymentTypeId),
                CardHolderName = m.PTDCardHolderName,
                CardNumber = m.PTDCardNumber,
                ExpiryMonth = m.PTDExpiryMonth,
                ExpiryYear = m.PTDExpiryYear,
                CreatedBy = userId,
                PaymentId = Convert.ToInt32(result),
                ExtValue1 = m.PTDSecurityNumber
            };

            _ptdService.SavePaymentTypeDetail(paymentTypeDetail);
            /*----Saving Payment-Type Details to Payment Section--*/


            /*----Apply Manual Payments Section --*/
            // Added Code here to apply the manual payment to the Billheader.
            var corporateId = Helpers.GetDefaultCorporateId();
            var facilityId = Helpers.GetDefaultFacilityId();
            var applyPaymnetManual = _service.ApplyManualPayment(corporateId, facilityId);
            /*----Apply Manual Payments Section --*/


            return Json(result);
        }


        //public ActionResult GetPaymentTypeDetail(int paymentId)
        //{
        //    using (var paymentBal=new PaymentTypeDetailBal())
        //    {
        //        var paymentList = paymentBal.GetPaymentTypeDetailByPaymentId(paymentId);
        //        var jsonData = new
        //        {
        //            paymentList.PaymentId,
        //            paymentList.PaymentType,
        //            paymentList.CardHolderName,
        //            paymentList.CardNumber,
        //            paymentList.ExpiryMonth,
        //            paymentList.ExpiryYear
        //        };
        //        return this.Json(jsonData, JsonRequestBehavior.AllowGet);
        //    }
        //}




        public ActionResult GetEncounterNumberById(int encounterId)
        {
            var encounterNumber = _eService.GetEncounterNumberByEncounterId(encounterId);
            return Json(encounterNumber, JsonRequestBehavior.AllowGet);
        }

        #endregion


        public ActionResult SaveAndApplyManualPayments(PaymentCustomModel vm)
        {
            var currentDateTime = Helpers.GetInvariantCultureDateTime();
            var userId = Helpers.GetLoggedInUserId();
            vm.PayCreatedBy = userId;
            vm.PayCreatedDate = currentDateTime;
            vm.PaymentTypeId = Convert.ToInt32(vm.PTDPaymentType);
            vm.PayCorporateID = Helpers.GetSysAdminCorporateID();
            vm.PayFacilityID = Helpers.GetDefaultFacilityId();
            var pl = string.Empty;

            var cm = _service.SaveAndApplyPayments(vm.PayCorporateID.GetValueOrDefault(), vm.PayFacilityID.GetValueOrDefault(), userId, vm, currentDateTime);

            if (cm.PaymentsList.Any())
                pl = RenderPartialViewToStringBase(PartialViews.PaymentListView, cm.PaymentsList);

            var jsonData = new JsonResult { Data = new { Success = cm.Success, pl, pd = cm.PaymentDetails, patients = cm.PatientsList }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            return jsonData;
        }



        public JsonResult GetPaymentsRelatedData(long billHeaderId, long? patientId, long? eId, string billNo, long? payId)
        {
            var currentDateTime = Helpers.GetInvariantCultureDateTime();
            var userId = Helpers.GetLoggedInUserId();
            var cId = Helpers.GetSysAdminCorporateID();
            var fId = Helpers.GetDefaultFacilityId();
            billNo = string.IsNullOrEmpty(billNo) ? string.Empty : billNo;
            var pl = string.Empty;
            var cm = _service.GetPaymentsListAndOthersData(cId, fId, userId, billHeaderId, patientId: patientId.GetValueOrDefault(), encounterId: eId.GetValueOrDefault(), billNumber: billNo, paymentId: payId.GetValueOrDefault());

            if (cm.PaymentsList.Any())
                pl = RenderPartialViewToStringBase(PartialViews.PaymentListView, cm.PaymentsList);

            var jsonData = new JsonResult { Data = new { pl, pd = cm.PaymentDetails, patients = cm.PatientsList }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            return jsonData;
        }
    }
}