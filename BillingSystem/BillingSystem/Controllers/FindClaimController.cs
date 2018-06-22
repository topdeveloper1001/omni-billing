// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FindClaimController.cs" company="Spadez">
//   Omnihealthcare
// </copyright>
// <summary>
//   The find claim controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Web.Mvc;

    using BillingSystem.Bal.BusinessAccess;
    using BillingSystem.Common;
    using BillingSystem.Model.CustomModel;
    using BillingSystem.Models;

    /// <summary>
    /// The find claim controller.
    /// </summary>
    public class FindClaimController : Controller
    {
        // GET: /FindClaim/
        #region Public Methods and Operators

        /// <summary>
        /// The index.
        /// </summary>
        /// <param name="fileId">The file identifier.</param>
        /// <returns>
        /// The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult Index(int? fileId)
        {
            using (var bal = new BillHeaderBal())
            {
                var fid = Helpers.GetDefaultFacilityId();
                var cid = Helpers.GetSysAdminCorporateID();
                var currentdate = Helpers.GetInvariantCultureDateTime();
                var firstDayOfMonth = new DateTime(currentdate.Year, currentdate.Month, 1);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                var claimsList = new List<BillHeaderCustomModel>();
                if (fileId != null)
                {
                    // ---- view batch file claims
                    claimsList = bal.FindClaimByFileId(
                        string.Empty,
                        string.Empty,
                        firstDayOfMonth,
                        lastDayOfMonth,
                        fid,
                        cid,
                        fileId);
                }
                else
                {
                    claimsList = bal.FindClaim(
                        string.Empty,
                        string.Empty,
                        firstDayOfMonth,
                        lastDayOfMonth,
                        fid,
                        cid);
                }
                var findClaimsview = new FindClaimsModel()
                                          {
                                              ClaimsList = claimsList,
                                              MonthStartDate = firstDayOfMonth,
                                              MonthEndDate = lastDayOfMonth
                                          };
                return View(findClaimsview);
            }
        }


        /// <summary>
        /// Gets the claims with filter.
        /// </summary>
        /// <param name="serachstring">The serachstring.</param>
        /// <param name="txtDateFrom">The text date from.</param>
        /// <param name="txtDateTill">The text date till.</param>
        /// <param name="rbtnShowAutoClosed">The RBTN show automatic closed.</param>
        /// <param name="claimstatus">The claimstatus.</param>
        /// <returns></returns>
        public ActionResult GetClaimsWithFilter(string serachstring, DateTime? txtDateFrom, DateTime? txtDateTill, int rbtnShowAutoClosed, string claimstatus)
        {
            using (var bal = new BillHeaderBal())
            {
                var fid = Helpers.GetDefaultFacilityId();
                var cid = Helpers.GetSysAdminCorporateID();
                var claimsList = bal.FindClaim(serachstring, claimstatus, txtDateFrom, txtDateTill, fid, cid);
                claimsList = rbtnShowAutoClosed == 1 ? claimsList.Where(x => x.IsAutoClosed == 1).ToList() : claimsList;
                return this.PartialView(PartialViews.FindClaimList, claimsList);
            }
        }
        #endregion
    }
}