using System.Web.Mvc;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common;
using BillingSystem.Models;

namespace BillingSystem.Controllers
{
    public class MissingDataController : BaseController
    {
        private readonly IEncounterService _eService;

        public MissingDataController(IEncounterService eService)
        {
            _eService = eService;
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var bal = new MissingDataBal();
            var corporateid = Helpers.GetSysAdminCorporateID();
            var facilityId = Helpers.GetDefaultFacilityId();
            var missingDatalist = bal.GetXMLMissingData(corporateid, facilityId);
            var billHeaderList = bal.GetAllXMLBillHeaderList(corporateid, facilityId);
            //Intialize the View Model i.e. BedMaster which is binded to PhysicianView
            var missingDataView = new MissingDataView()
            {
                MissingDataList = missingDatalist,
                BillReadyForScrub = billHeaderList
            };
            return View(missingDataView);
        }


        /// <summary>
        /// Scrubs the XML bill.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        public ActionResult ScrubXMLBill(int encounterId)
        {
            var userId = Helpers.GetLoggedInUserId();
            var billheaderObj = _eService.GetEncounterEndCheck(encounterId, userId);
            var bal = new MissingDataBal();
            var corporateid = Helpers.GetSysAdminCorporateID();
            var facilityId = Helpers.GetDefaultFacilityId();
            var billHeaderList = bal.GetAllXMLBillHeaderList(corporateid, facilityId);
            return PartialView(PartialViews.XMLBillHeaderList, billHeaderList);

        }
    }
}