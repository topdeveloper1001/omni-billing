using System.Web.Mvc;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Common;
using BillingSystem.Model.CustomModel;
using BillingSystem.Models;
using BillingSystem.Common.Common;

namespace BillingSystem.Controllers
{
    public class ReviewExpectedPaymentsController : BaseController
    {
        //
        // GET: /ReviewExpectedPayments/
        public ActionResult Index()
        {
            using (var bal = new PaymentBal())
            {
                var corporateId = Helpers.GetSysAdminCorporateID();
                var facilityId = Helpers.GetDefaultFacilityId();
                var viewData = new ReviewExpectedPaymentsView
                {
                    ExpectedPaymentInsNotPaidList = bal.GetExpectedPaymentInsNotPaid(corporateId, facilityId),
                    ExpectedPaymentPatientVarList = bal.GetExpectedPaymentPatientVar(corporateId, facilityId),
                    ExpectedPaymentInsVarianceList = bal.GetExpectedPaymentInsVariance(corporateId, facilityId),
                };
                return View(viewData);
            }
        }

        /// <summary>
        /// Gets the patient information search result.
        /// </summary>
        /// <param name="common">The common.</param>
        /// <returns></returns>
        public ActionResult GetPatientSearchResult(CommonModel common)
        {
            common.FacilityId = Helpers.GetDefaultFacilityId();
            common.CorporateId = Helpers.GetDefaultCorporateId();
            var bal = new PatientInfoBal();
            var objPatientInfoData = bal.GetPatientSearchResult(common);
            var viewpath = string.Format("../PatientSearch/{0}", PartialViews.PatientSearchList);
            //return PartialView("UserControls/_PatientSearchResult", objPatientInfoData);
            return PartialView(viewpath, objPatientInfoData);
        }

        /// <summary>
        /// Gets the patient account summary.
        /// </summary>
        /// <param name="PatientID">The patient identifier.</param>
        /// <returns></returns>
        public ActionResult GetPatientAccountSummary(int PatientID)
        {
            using (var paymentBal = new PaymentBal())
            {
                var applyPaymnetManual = paymentBal.GetPatientAccountStatement(PatientID);
                return PartialView("UserControls/_PatienAccountSummary", applyPaymnetManual);
            }
        }

        /// <summary>
        /// Gets the not received payment summary.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetNotReceivedPaymentList()
        {
            using (var paymentBal = new PaymentBal())
            {
                var corpoarteId = Helpers.GetSysAdminCorporateID();
                var facilityId = Helpers.GetDefaultFacilityId();
                var notReceivedpaymentsList = paymentBal.GetNoPaymentReceviedList(corpoarteId, facilityId);
                return PartialView("UserControls/_PatienAccountSummary", notReceivedpaymentsList);
            }
        }

        /// <summary>
        /// Gets the un matched payment list.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetUnMatchedPaymentList()
        {
            using (var paymentBal = new PaymentBal())
            {
                var corpoarteId = Helpers.GetSysAdminCorporateID();
                var facilityId = Helpers.GetDefaultFacilityId();
                var unMatchedpaymentsList = paymentBal.GetUnMactedPaymentList(corpoarteId, facilityId);
                return PartialView("UserControls/_PatienAccountSummary", unMatchedpaymentsList);
            }
        }


        public ActionResult SortExpactedPayment()
        {
             var corporateId = Helpers.GetSysAdminCorporateID();
            var facilityId = Helpers.GetDefaultFacilityId();
            var bal = new PaymentBal();
            var expectedPaymentInsNotPaidList = bal.GetExpectedPaymentInsNotPaid(corporateId, facilityId);
           return PartialView(PartialViews.ExpectedPaymentInsNotPaidListView, expectedPaymentInsNotPaidList);
        }
        public ActionResult SortPatientVarianceReport()
        {
            var corporateId = Helpers.GetSysAdminCorporateID();
            var facilityId = Helpers.GetDefaultFacilityId();
            var bal = new PaymentBal();
            var  expectedPaymentPatientVarList = bal.GetExpectedPaymentPatientVar(corporateId, facilityId);
            return PartialView(PartialViews.ExpectedPaymentPatientVarListView, expectedPaymentPatientVarList);
        }


    }
}