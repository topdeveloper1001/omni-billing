using System.Web.Mvc;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common;
using BillingSystem.Models;

namespace BillingSystem.Controllers
{
    public class MissingDataController : BaseController
    {
        private readonly IEncounterService _eService;
        private readonly IMissingDataService _service;

        public MissingDataController(IEncounterService eService, IMissingDataService service)
        {
            _eService = eService;
            _service = service;
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var corporateid = Helpers.GetSysAdminCorporateID();
            var facilityId = Helpers.GetDefaultFacilityId();
            var missingDatalist = _service.GetXMLMissingData(corporateid, facilityId);
            var billHeaderList = _service.GetAllXMLBillHeaderList(corporateid, facilityId);
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
            var corporateid = Helpers.GetSysAdminCorporateID();
            var facilityId = Helpers.GetDefaultFacilityId();
            var billHeaderList = _service.GetAllXMLBillHeaderList(corporateid, facilityId);
            return PartialView(PartialViews.XMLBillHeaderList, billHeaderList);

        }
    }
}