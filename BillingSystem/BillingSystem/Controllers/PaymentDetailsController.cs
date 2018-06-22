using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Models;
using iTextSharp.text;

namespace BillingSystem.Controllers
{
    public class PaymentDetailsController : BaseController
    {
        //
        // GET: /PaymentDetails/
        public ActionResult Index()
        {
            var paymentsDetails = new PaymentDetailsView()
            {
                BillHeaderDetails = new BillHeader(),
                PaymentDetails = new List<Payment>(),
                PatientSearchList = new List<PatientInfoXReturnPaymentCustomModel>(),
                PatientSearch = new PatientInfo()
            };
            return View(paymentsDetails);
        }

        /// <summary>
        /// Gets the payment details.
        /// </summary>
        /// <param name="BillHeaderId">The bill header identifier.</param>
        /// <returns></returns>
        public ActionResult GetPaymentDetails(int BillHeaderId)
        {
            using (var billheaderbal = new BillHeaderBal())
            {
                var vm = new PaymentDetailsCustomModel();
                var billPaymentBal = new PaymentBal();
                var billPaymentObj = billPaymentBal.GetPaymentByBillId(BillHeaderId);
                var insurencePayment = billPaymentObj.Where(x => x.PayType == 100).ToList();
                var patientPayment = billPaymentObj.Where(x => x.PayType == 500).ToList();
                var billDetails = billheaderbal.GetBillHeaderById(BillHeaderId);
                if (billDetails != null)
                {
                    vm.PatientSharePayable = Convert.ToDecimal(billDetails.PatientShare);
                    vm.InsSharePayable = Convert.ToDecimal(billDetails.PayerShareNet);
                    vm.GrossSharePayable = Convert.ToDecimal(Convert.ToDecimal(billDetails.PatientShare) + Convert.ToDecimal(billDetails.PayerShareNet));

                    vm.PatientSharePaid = Convert.ToDecimal(billDetails.PatientPayAmount);
                    vm.InsSharePaid = Convert.ToDecimal(billDetails.PaymentAmount);
                    vm.GrossSharePaid = Convert.ToDecimal(Convert.ToDecimal(billDetails.PatientPayAmount) + Convert.ToDecimal(billDetails.PaymentAmount));

                    vm.PatientShareBalance = Convert.ToDecimal(Convert.ToDecimal(billDetails.PatientShare) - Convert.ToDecimal(billDetails.PatientPayAmount));
                    vm.InsShareBalance = Convert.ToDecimal(Convert.ToDecimal(billDetails.PayerShareNet) - Convert.ToDecimal(billDetails.PaymentAmount));
                    vm.GrossShareBalance = Convert.ToDecimal(Convert.ToDecimal(vm.PatientShareBalance) + Convert.ToDecimal(vm.InsShareBalance));

                    if (insurencePayment.Any())
                    {
                        var expectedInsurencePaymentAmount = Convert.ToDecimal(0.00);
                        var TotalPaidAmount = Convert.ToDecimal(0.00);
                        var TotalAppliedAmount = Convert.ToDecimal(0.00);
                        var TotalUNAppliedAmount = Convert.ToDecimal(0.00);
                        foreach (var paymentCustomModel in insurencePayment)
                        {
                            expectedInsurencePaymentAmount += Convert.ToDecimal(paymentCustomModel.PayNETAmount);
                            TotalPaidAmount += Convert.ToDecimal(paymentCustomModel.PayAmount);
                            TotalAppliedAmount += Convert.ToDecimal(paymentCustomModel.PayAppliedAmount);
                            TotalUNAppliedAmount += Convert.ToDecimal(paymentCustomModel.PayUnAppliedAmount);
                        }
                        vm.InsPayment = expectedInsurencePaymentAmount;
                        vm.InsTotalPaid = TotalPaidAmount;
                        vm.InsApplied = TotalAppliedAmount;
                        vm.InsUnapplied = TotalUNAppliedAmount;
                    }
                    if (patientPayment.Any())
                    {
                        var expectedInsurencePaymentAmount = Convert.ToDecimal(0.00);
                        var totalPatientPaidAmount = Convert.ToDecimal(0.00);
                        var totalPatientAppliedAmount = Convert.ToDecimal(0.00);
                        var totalPatientUnAppliedAmount = Convert.ToDecimal(0.00);
                        foreach (var patientpaymentCustomModel in patientPayment)
                        {
                            expectedInsurencePaymentAmount += Convert.ToDecimal(patientpaymentCustomModel.PayNETAmount);
                            totalPatientPaidAmount += Convert.ToDecimal(patientpaymentCustomModel.PayAmount);
                            totalPatientAppliedAmount += Convert.ToDecimal(patientpaymentCustomModel.PayAppliedAmount);
                            totalPatientUnAppliedAmount += Convert.ToDecimal(patientpaymentCustomModel.PayUnAppliedAmount);
                        }
                        vm.PatientPayment = expectedInsurencePaymentAmount;
                        vm.PatientTotalPaid = totalPatientPaidAmount;
                        vm.PatientApplied = totalPatientAppliedAmount;
                        vm.PatientUnApplied = totalPatientUnAppliedAmount;
                    }
                    // paymentDetailCustomModel.PaymentDetails = insurencePayment;
                }
                return Json(vm);
            }
        }

        /// <summary>
        /// Gets the payment details list.
        /// </summary>
        /// <param name="BillHeaderId">The bill header identifier.</param>
        /// <returns></returns>
        public ActionResult GetPaymentDetailsList(int BillHeaderId)
        {
            var billPaymentBal = new PaymentBal();
            var billPaymentObj = billPaymentBal.GetPaymentByBillId(BillHeaderId);
            var insurencePayment = billPaymentObj.Where(x => x.PayType == 100).ToList();
            return PartialView("UserControls/_PaymentDetailList", insurencePayment);
        }

        /// <summary>
        /// Gets the manual payment details list.
        /// </summary>
        /// <param name="BillHeaderId">The bill header identifier.</param>
        /// <returns></returns>
        public ActionResult GetManualPaymentDetailsList(int BillHeaderId)
        {
            var billPaymentBal = new PaymentBal();
            var billPaymentObj = billPaymentBal.GetPaymentByBillId(BillHeaderId);
            var insurencePayment = billPaymentObj.Where(x => x.PayType == 500).ToList();
            return PartialView("UserControls/_PaymentDetailManualList", insurencePayment);
        }

        /// <summary>
        /// Gets the x activities list.
        /// </summary>
        /// <param name="BillHeaderId">The bill header identifier.</param>
        /// <returns></returns>
        public ActionResult GetXActivitiesList(int BillHeaderId)
        {
            var billPaymentBal = new PaymentBal();
            var billPaymentObj = billPaymentBal.GetPaymentByBillId(BillHeaderId);
            var xactivitiesBal = new XactivityBal();
            var xActivitesList = xactivitiesBal.GetXactivityByClaimId(BillHeaderId);
            var xActivitesCustomList = xActivitesList.Where(t2 => billPaymentObj.Any(t1 => t2.ActivityID == t1.PayActivityID)).ToList();
            var xActivitesMergePayment = new List<XActivityCustomModel>();
            foreach (var xActivityCustomModel in xActivitesCustomList)
            {
                var paymentobj = billPaymentObj.FirstOrDefault(x => x.PayActivityID == Convert.ToInt32(xActivityCustomModel.ActivityID));
                var xActivityCustomModelObj = new XActivityCustomModel()
                {
                    XActivity1 = xActivityCustomModel.XActivity1,
                    EncounterID = xActivityCustomModel.EncounterID,
                    ActivityID = xActivityCustomModel.ActivityID,
                    DType = xActivityCustomModel.DType,
                    DCode = xActivityCustomModel.DCode,
                    StartDate = xActivityCustomModel.StartDate,
                    AType = xActivityCustomModel.AType,
                    ACode = xActivityCustomModel.ACode,
                    Quantity = xActivityCustomModel.Quantity,
                    OrderingClinician = xActivityCustomModel.OrderingClinician,
                    OrderDate = xActivityCustomModel.OrderDate,
                    Clinician = xActivityCustomModel.Clinician,
                    OrderCloseDate = xActivityCustomModel.OrderCloseDate,
                    PriorAuthorizationID = xActivityCustomModel.PriorAuthorizationID,
                    Gross = xActivityCustomModel.Gross,
                    PatientShare = xActivityCustomModel.PatientShare,
                    Net = xActivityCustomModel.Net,
                    DenialCode = xActivityCustomModel.DenialCode,
                    PaymentReference = xActivityCustomModel.PaymentReference,
                    DateSettlement = xActivityCustomModel.DateSettlement,
                    PaymentAmount = xActivityCustomModel.PaymentAmount,
                    PatientPayReference = xActivityCustomModel.PatientPayReference,
                    PatientDateSettlement = xActivityCustomModel.PatientDateSettlement,
                    PatientPayAmount = xActivityCustomModel.PatientPayAmount,
                    Status = xActivityCustomModel.Status,
                    ClaimID = xActivityCustomModel.ClaimID,
                    FileID = xActivityCustomModel.FileID,
                    ARFileID = xActivityCustomModel.ARFileID,
                    ModifiedBy = xActivityCustomModel.ModifiedBy,
                    ModifiedDate = xActivityCustomModel.ModifiedDate,
                    ActivityType = xActivityCustomModel.ActivityType,
                    AppliedAmount = paymentobj.PayAppliedAmount,
                    UnAppliedAmount = paymentobj.PayUnAppliedAmount,
                };
                xActivitesMergePayment.Add(xActivityCustomModelObj);
            }
            return PartialView("UserControls/_XActivitesCustomList", xActivitesMergePayment);
        }



        /// <summary>
        /// Gets the patient information search result.
        /// </summary>
        /// <param name="common">The common.</param>
        /// <returns></returns>
        public ActionResult GetPatientInfoSearchResult(CommonModel common)
        {
            var facilityid = Helpers.GetDefaultFacilityId();
            var corporateid = Helpers.GetSysAdminCorporateID();
            common.FacilityId = facilityid;
            common.CorporateId = corporateid;
            var bal = new PatientInfoBal();
            //var objPatientInfoData = bal.GetPatientSearchResult(common);
            var objPatientInfoData = bal.GetPatientSearchResultInPayment(common);
            return PartialView(PartialViews.SearchResultListInDetail, objPatientInfoData);
        }
    }
}