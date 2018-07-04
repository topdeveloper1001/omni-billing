using System;
using System.Collections.Generic;
using System.Web.Mvc;

using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Models;

using RazorPDF;


namespace BillingSystem.Controllers
{ 
    public class ManualChargesTrackingController : BaseController
    {
        private readonly IManualChargesTrackingService _service;

        public ManualChargesTrackingController(IManualChargesTrackingService service)
        {
            _service = service;
        }


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
            int facilityid = Helpers.GetDefaultFacilityId();
            int corporateid = Helpers.GetSysAdminCorporateID();

            // Get the Entity list
            List<ManualChargesTrackingCustomModel> list = _service.GetManualChargesTrackingList(facilityid, corporateid);

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
            int facilityid = Helpers.GetDefaultFacilityId();
            int corporateid = Helpers.GetSysAdminCorporateID();

            // Get the Entity list
            List<ManualChargesTrackingCustomModel> list = _service.GetManualChargesTrackingListDateRange(
                facilityid,
                corporateid,
                fromDate,
                tilldate);

            // Pass the View Model in ActionResult to View ManualChargesTracking
            return this.PartialView(PartialViews.ManualChargesTrackingList, list);
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

            Response.AddHeader("Content-Type", "application/vnd.ms-excel");
            string firstdayOfMOnth = currentDateTime.Year + "/" + currentDateTime.Month + "/01";
            DateTime fromdateval = fromDate ?? Convert.ToDateTime(firstdayOfMOnth);
            DateTime tilldateval = tilldate ?? Helpers.GetInvariantCultureDateTime();
            int facilityid = Helpers.GetDefaultFacilityId();
            int corporateid = Helpers.GetSysAdminCorporateID();

            // Get the Entity list
            List<ManualChargesTrackingCustomModel> list = _service.GetManualChargesTrackingListDateRange(
                facilityid,
                corporateid,
                fromdateval,
                tilldateval);

            // Pass the View Model in ActionResult to View ManualChargesTracking
            return this.PartialView(PartialViews.ManualChargesTrackingList, list);
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

            string firstdayOfMOnth = currentDateTime + "/" + currentDateTime + "/01";
            DateTime fromdateval = fromDate ?? Convert.ToDateTime(firstdayOfMOnth);
            int facilityid = Helpers.GetDefaultFacilityId();
            int corporateid = Helpers.GetSysAdminCorporateID();
            DateTime tilldateval = tilldate ?? Helpers.GetInvariantCultureDateTime();
            var pdf =
                new PdfResult(
                    _service.GetManualChargesTrackingListDateRange(facilityid, corporateid, fromdateval, tilldateval),
                    "ManualChargesTrackingPDF");
            pdf.ViewBag.FromDate = fromdateval;
            pdf.ViewBag.TillDate = tilldateval;
            pdf.ViewBag.Title = "Manual Charges Tracking";
            return pdf;
        }
    }
}