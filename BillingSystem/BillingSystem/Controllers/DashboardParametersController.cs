// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DashboardParametersController.cs" company="Spadez">
//   Omnihealthcare
// </copyright>
// <summary>
//   The dashboard parameters controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using BillingSystem.Bal.Interfaces;
    using BillingSystem.Common;
    using BillingSystem.Model;
    using BillingSystem.Model.CustomModel;
    using BillingSystem.Models;

    /// <summary>
    /// The dashboard parameters controller.
    /// </summary>
    public class DashboardParametersController : BaseController
    {
        private readonly IDashboardParametersService _service;

        public DashboardParametersController(IDashboardParametersService service)
        {
            _service = service;
        }

        #region Public Methods and Operators

        /// <summary>
        /// Delete the current DashboardParameters based on the DashboardParameters ID passed in the DashboardParametersModel
        /// </summary>
        /// <param name="id">
        /// The identifier.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult DeleteDashboardParameters(int id)
        {
            // Get DashboardParameters model object by current DashboardParameters ID
            DashboardParameters model = _service.GetDashboardParametersById(id);
            int userId = Helpers.GetLoggedInUserId();
            var list = new List<DashboardParametersCustomModel>();
            DateTime currentDate = Helpers.GetInvariantCultureDateTime();

            // Check If DashboardParameters model is not null
            if (model != null)
            {
                int corporateid = Helpers.GetSysAdminCorporateID();
                int facilityid = Helpers.GetDefaultFacilityId();

                model.ModifiedBy = userId;
                model.ModifiedDate = currentDate;

                model.CorporateId = corporateid;
                model.FacilityId = facilityid;

                // Update Operation of current DashboardParameters
                list = _service.DeleteDashboardParameters(model);

                // return deleted ID of current DashboardParameters as Json Result to the Ajax Call.
            }

            // Pass the ActionResult with List of DashboardParametersViewModel object to Partial View DashboardParametersList
            return PartialView(PartialViews.DashboardParametersList, list);
        }

        /// <summary>
        /// Get the details of the current DashboardParameters in the view model by ID
        /// </summary>
        /// <param name="id">
        /// The identifier.
        /// </param>
        /// <returns>
        /// The <see cref="JsonResult"/>.
        /// </returns>
        public JsonResult GetDashboardParametersDetails(int id)
        {
            var current = _service.GetDashboardParametersById(id);

            // Pass the ActionResult with the current DashboardParametersViewModel object as model to PartialView DashboardParametersAddEdit
            return Json(current);
        }

        /// <summary>
        /// Gets the parameters list by dashboard.
        /// </summary>
        /// <param name="dashboardType">
        /// Type of the dashboard.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult GetParametersListByDashboard(string dashboardType)
        {
            var list = _service.GetParametersListByDashboard(
                         Helpers.GetSysAdminCorporateID(), Helpers.GetFacilityIdNextDefaultCororateFacility(),
                         dashboardType);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///     Get the details of the DashboardParameters View in the Model DashboardParameters such as DashboardParametersList,
        ///     list of countries etc.
        /// </summary>
        /// <returns>
        ///     returns the actionresult in the form of current object of the Model DashboardParameters to be passed to View
        ///     DashboardParameters
        /// </returns>
        public ActionResult Index()
        {
            // Get the Entity list
            int corporateid = Helpers.GetSysAdminCorporateID();
            int facilityid = Helpers.GetDefaultFacilityId();

            // Intialize the View Model i.e. DashboardParametersView which is binded to Main View Index.cshtml under DashboardParameters
            var viewModel = new DashboardParametersView
            {
                DashboardParametersList = _service.GetDashboardParameters(corporateid, facilityid),
                CurrentDashboardParameters = new DashboardParameters()
            };

            // Pass the View Model in ActionResult to View DashboardParameters
            return View(viewModel);
        }

        /// <summary>
        /// Add New or Update the DashboardParameters based on if we pass the DashboardParameters ID in the
        ///     DashboardParametersViewModel object.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// returns the newly added or updated ID of DashboardParameters row
        /// </returns>
        public ActionResult SaveDashboardParameters(DashboardParameters model)
        {
            // Initialize the newId variable 
            int userId = Helpers.GetLoggedInUserId();
            DateTime currentDate = Helpers.GetInvariantCultureDateTime();
            var list = new List<DashboardParametersCustomModel>();

            // Check if Model is not null 
            if (model != null)
            {
                int corporateid = Helpers.GetSysAdminCorporateID();
                int facilityid = Helpers.GetDefaultFacilityId();
                model.CorporateId = corporateid;
                model.FacilityId = facilityid;

                if (model.ParameterId > 0)
                {
                    model.ModifiedBy = userId;
                    model.ModifiedDate = currentDate;
                }
                else
                {
                    model.CreatedBy = userId;
                    model.CreatedDate = currentDate;
                }

                // Call the AddDashboardParameters Method to Add / Update current DashboardParameters
                list = _service.SaveDashboardParameters(model);
            }

            // Pass the ActionResult with List of DashboardParametersViewModel object to Partial View DashboardParametersList
            return PartialView(PartialViews.DashboardParametersList, list);
        }

        /// <summary>
        /// The sort dashboard parameters.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult SortDashboardParameters()
        {
            int corporateid = Helpers.GetSysAdminCorporateID();
            int facilityid = Helpers.GetDefaultFacilityId();
            var list = _service.GetDashboardParameters(corporateid, facilityid);
            return PartialView(PartialViews.DashboardParametersList, list);
        }

        #endregion
    }
}