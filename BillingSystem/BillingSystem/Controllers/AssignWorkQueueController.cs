// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssignWorkQueueController.cs" company="Spadez Solutions PVT. LTD.">
//   ServicesDotCom
// </copyright>
// <summary>
//   Defines the AssignWorkQueueController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using Bal.BusinessAccess;
    using Common;
    using Models;

    /// <summary>
    /// The assign work queue controller.
    /// </summary>
    public class AssignWorkQueueController : BaseController
    {
        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var scrubReportBal = new ScrubReportService();

            var corporateId = Helpers.GetSysAdminCorporateID();
            var facilityId = Helpers.GetDefaultFacilityId();
            var userId = Helpers.GetLoggedInUserId();
            var headerList = scrubReportBal.GetScrubHeaderList(
                corporateId,
                facilityId,
                Convert.ToInt32(0),
                userId,
                false);
            var allWithBillEdits =
                headerList.Where(a => a.Status != null && ((int)a.Status == 1 || (int)a.Status == 2))
                    .ToList()
                    .OrderByDescending(x => x.ScrubDate)
                    .ToList();
            var assignWorkQueueView = new AssignWorkQueueView { AssignWorkQueue = allWithBillEdits };
            return View(assignWorkQueueView);
        }
    }

}