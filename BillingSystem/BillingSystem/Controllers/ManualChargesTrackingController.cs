// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManualChargesTrackingController.cs" company="Spadez">
//   OmniHealth Care
// </copyright>
// <summary>
//   TODO The manual charges tracking controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    using BillingSystem.Bal.BusinessAccess;
    using BillingSystem.Common;
    using BillingSystem.Model;
    using BillingSystem.Model.CustomModel;
    using BillingSystem.Models;

    using RazorPDF;

    /// <summary>
    /// TODO The manual charges tracking controller.
    /// </summary>
    public class ManualChargesTrackingController : BaseController
    {
        /// <summary>
        ///     Get the details of the ManualChargesTracking View in the Model ManualChargesTracking such as
        ///     ManualChargesTrackingList, list of countries etc.
        /// </summary>
        /// <returns>
        ///     returns the actionresult in the form of current object of the Model ManualChargesTracking to be passed to View
        ///     ManualChargesTracking
        /// </returns>
        public ActionResult Index()
        {
            // Initialize the ManualChargesTracking BAL object
            using (var bal = new ManualChargesTrackingBal())
            {
                int facilityid = Helpers.GetDefaultFacilityId();
                int corporateid = Helpers.GetSysAdminCorporateID();

                // Get the Entity list
                List<ManualChargesTrackingCustomModel> list = bal.GetManualChargesTrackingList(facilityid, corporateid);

                // Intialize the View Model i.e. ManualChargesTrackingView which is binded to Main View Index.cshtml under ManualChargesTracking
                var viewModel = new ManualChargesTrackingView
                                    {
                                        ManualChargesTrackingList = list, 
                                        CurrentManualChargesTracking =
                                            new ManualChargesTracking()
                                    };

                // Pass the View Model in ActionResult to View ManualChargesTracking
                return View(viewModel);
            }
        }

        /// <summary>
        /// Binds the report.
        /// </summary>
        /// <param name="fromDate">
        /// From date.
        /// </param>
        /// <param name="tilldate">
        /// The tilldate.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult BindReport(DateTime fromDate, DateTime tilldate)
        {
            using (var bal = new ManualChargesTrackingBal())
            {
                int facilityid = Helpers.GetDefaultFacilityId();
                int corporateid = Helpers.GetSysAdminCorporateID();

                // Get the Entity list
                List<ManualChargesTrackingCustomModel> list = bal.GetManualChargesTrackingListDateRange(
                    facilityid, 
                    corporateid, 
                    fromDate, 
                    tilldate);

                // Pass the View Model in ActionResult to View ManualChargesTracking
                return this.PartialView(PartialViews.ManualChargesTrackingList, list);
            }
        }

        /// <summary>
        /// Exports to excel.
        /// </summary>
        /// <param name="fromDate">
        /// From date.
        /// </param>
        /// <param name="tilldate">
        /// The tilldate.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult ExportToExcel(DateTime? fromDate, DateTime? tilldate)
        {
            var currentDateTime = Helpers.GetInvariantCultureDateTime();

            this.Response.AddHeader("Content-Type", "application/vnd.ms-excel");
            using (var bal = new ManualChargesTrackingBal())
            {
                string firstdayOfMOnth = currentDateTime.Year + "/" + currentDateTime.Month + "/01";
                DateTime fromdateval = fromDate ?? Convert.ToDateTime(firstdayOfMOnth);
                DateTime tilldateval = tilldate ?? Helpers.GetInvariantCultureDateTime();
                int facilityid = Helpers.GetDefaultFacilityId();
                int corporateid = Helpers.GetSysAdminCorporateID();

                // Get the Entity list
                List<ManualChargesTrackingCustomModel> list = bal.GetManualChargesTrackingListDateRange(
                    facilityid, 
                    corporateid, 
                    fromdateval, 
                    tilldateval);

                // Pass the View Model in ActionResult to View ManualChargesTracking
                return this.PartialView(PartialViews.ManualChargesTrackingList, list);
            }
        }

        /// <summary>
        /// Exports to PDF.
        /// </summary>
        /// <param name="fromDate">
        /// From date.
        /// </param>
        /// <param name="tilldate">
        /// The tilldate.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult ExportToPDF(DateTime? fromDate, DateTime? tilldate)
        {
            var currentDateTime = Helpers.GetInvariantCultureDateTime();

            using (var bal = new ManualChargesTrackingBal())
            {
                string firstdayOfMOnth = currentDateTime + "/" + currentDateTime + "/01";
                DateTime fromdateval = fromDate ?? Convert.ToDateTime(firstdayOfMOnth);
                int facilityid = Helpers.GetDefaultFacilityId();
                int corporateid = Helpers.GetSysAdminCorporateID();
                DateTime tilldateval = tilldate ?? Helpers.GetInvariantCultureDateTime();
                var pdf =
                    new PdfResult(
                        bal.GetManualChargesTrackingListDateRange(facilityid, corporateid, fromdateval, tilldateval), 
                        "ManualChargesTrackingPDF");
                pdf.ViewBag.FromDate = fromdateval;
                pdf.ViewBag.TillDate = tilldateval;
                pdf.ViewBag.Title = "Manual Charges Tracking";
                return pdf;
            }
        }
    }
}