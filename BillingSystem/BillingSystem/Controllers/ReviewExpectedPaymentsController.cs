using System.Web.Mvc;
using BillingSystem.Common;
using BillingSystem.Model.CustomModel;
using BillingSystem.Models;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Controllers
{
    public class ReviewExpectedPaymentsController : BaseController
    {
        private readonly IPatientInfoService _piService;
        private readonly IPaymentService _pService;

        public ReviewExpectedPaymentsController(IPatientInfoService piService, IPaymentService pService)
        {
            _piService = piService;
            _pService = pService;
        }

        //
        // GET: /ReviewExpectedPayments/
        public ActionResult Index()
        {
            var corporateId = Helpers.GetSysAdminCorporateID();
            var facilityId = Helpers.GetDefaultFacilityId();
            var viewData = new ReviewExpectedPaymentsView
            {
                ExpectedPaymentInsNotPaidList = _pService.GetExpectedPaymentInsNotPaid(corporateId, facilityId),
                ExpectedPaymentPatientVarList = _pService.GetExpectedPaymentPatientVar(corporateId, facilityId),
                ExpectedPaymentInsVarianceList = _pService.GetExpectedPaymentInsVariance(corporateId, facilityId),
            };
            return View(viewData);

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
            var objPatientInfoData = _piService.GetPatientSearchResult(common);
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
            var applyPaymnetManual = _pService.GetPatientAccountStatement(PatientID);
            return PartialView("UserControls/_PatienAccountSummary", applyPaymnetManual);
        }

        /// <summary>
        /// Gets the not received payment summary.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetNotReceivedPaymentList()
        {
            var corpoarteId = Helpers.GetSysAdminCorporateID();
            var facilityId = Helpers.GetDefaultFacilityId();
            var notReceivedpaymentsList = _pService.GetNoPaymentReceviedList(corpoarteId, facilityId);
            return PartialView("UserControls/_PatienAccountSummary", notReceivedpaymentsList);
        }

        /// <summary>
        /// Gets the un matched payment list.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetUnMatchedPaymentList()
        {
            var corpoarteId = Helpers.GetSysAdminCorporateID();
            var facilityId = Helpers.GetDefaultFacilityId();
            var unMatchedpaymentsList = _pService.GetUnMactedPaymentList(corpoarteId, facilityId);
            return PartialView("UserControls/_PatienAccountSummary", unMatchedpaymentsList);
        }


        public ActionResult SortExpactedPayment()
        {
            var corporateId = Helpers.GetSysAdminCorporateID();
            var facilityId = Helpers.GetDefaultFacilityId();
            var expectedPaymentInsNotPaidList = _pService.GetExpectedPaymentInsNotPaid(corporateId, facilityId);
            return PartialView(PartialViews.ExpectedPaymentInsNotPaidListView, expectedPaymentInsNotPaidList);
        }
        public ActionResult SortPatientVarianceReport()
        {
            var corporateId = Helpers.GetSysAdminCorporateID();
            var facilityId = Helpers.GetDefaultFacilityId();
            var expectedPaymentPatientVarList = _pService.GetExpectedPaymentPatientVar(corporateId, facilityId);
            return PartialView(PartialViews.ExpectedPaymentPatientVarListView, expectedPaymentPatientVarList);
        }


    }
}