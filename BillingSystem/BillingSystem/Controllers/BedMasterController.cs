using System;
using System.Web.Mvc;

using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Models;

namespace BillingSystem.Controllers
{ 
    /// <summary>
    /// The bed master controller.
    /// </summary>
    public class BedMasterController : BaseController
    {
        private readonly IBedMasterService _bedService;
        private readonly IFacilityStructureService _fsService;

        public BedMasterController(IBedMasterService bedService, IFacilityStructureService fsService)
        {
            _bedService = bedService;
            _fsService = fsService;
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var facilityId = 0;
            var corporateid = 0;
            if (Session[SessionNames.SessionClass.ToString()] != null)
            {
                var session = Session[SessionNames.SessionClass.ToString()] as SessionClass;
                facilityId = session.FacilityId;
                corporateid = session.CorporateId;
            }
            var bedMasterList = _bedService.GetBedMasterListByRole(facilityId, corporateid);

            // Intialize the View Model i.e. BedMaster which is binded to PhysicianView
            var bedMasterModel = new BedMasterView
            {
                BedMasterList = bedMasterList,
                CurrentBedMaster = new UBedMaster()
            };

            // Pass the View Model in ActionResult to View Physician
            return View(bedMasterModel);
        }

        /// <summary>
        /// Gets the facility list.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetFacilityList()
        {
            var facilityId = Helpers.GetDefaultFacilityId();
            var corporateid = Helpers.GetSysAdminCorporateID();
            var bal = new FacilityService();
            var bedMasterList = bal.GetFacilitiesByRoles(facilityId, corporateid);
            return Json(bedMasterList);
        }

        /// <summary>
        /// Gets the bed name list.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public ActionResult GetBedNameList(string facilityId)
        {
            var bedMasterList = _fsService.GetFacilityBeds(facilityId);
            return Json(bedMasterList);
        }

        /// <summary>
        /// Gets the services list.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetServicesList()
        {
            var serviceCodeBal = new ServiceCodeService(Helpers.DefaultServiceCodeTableNumber);
            var serviceCodeList = serviceCodeBal.GetServiceCodes();
            return Json(serviceCodeList);
        }

        /// <summary>
        /// Bind all the BedMaster list
        /// </summary>
        /// <returns>
        /// action result with the partial view containing the facility list object
        /// </returns>
        public ActionResult BindBedMasterList()
        {
            var facilityId = 0;
            var corporateid = 0;
            if (Session[SessionNames.SessionClass.ToString()] != null)
            {
                var session = Session[SessionNames.SessionClass.ToString()] as SessionClass;
                facilityId = session.FacilityId;
                corporateid = session.CorporateId;
            }
            var bedMasterList = _bedService.GetBedMasterListByRole(facilityId, corporateid);
            return PartialView(PartialViews.BedMasterlist, bedMasterList);

        }

        /// <summary>
        /// Bind all the BedMaster list
        /// </summary>
        /// <param name="_facilityId">The _facility identifier.</param>
        /// <returns>
        /// action result with the partial view containing the facility list object
        /// </returns>
        public ActionResult BindBedMasterListByfacility(int _facilityId)
        {
            var bedMasterList = _bedService.GetBedMasterListByRole(_facilityId, 0);
            return PartialView(PartialViews.BedMasterlist, bedMasterList);

        }

        /// <summary>
        /// Add New or Update the facility based on if we pass the BedMaster ID in the BedMasterViewModel object.
        /// </summary>
        /// <param name="bedMasterModel">pass the details of BedMaster in the view model</param>
        /// <returns>
        /// returns the newly added or updated ID of BedMaster row
        /// </returns>
        public ActionResult SaveBedMaster(UBedMaster bedMasterModel)
        {
            // Initialize the newId variable 
            var newId = -1;

            // Check if BedMasterViewModel 
            if (bedMasterModel != null)
            {
                if (bedMasterModel.BedId > 0)
                {
                    bedMasterModel.ModifiedBy = Helpers.GetLoggedInUserId();
                    bedMasterModel.ModifiedDate = Helpers.GetInvariantCultureDateTime();
                }

                // Call the AddBedMaster Method to Add / Update current BedMaster
                newId = _bedService.AddUpdateBedMaster(bedMasterModel);

            }
            return Json(newId);
        }

        /// <summary>
        /// Get the details of the current BedMaster in the view model by ID
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public ActionResult GetBedMaster(CommonModel model)
        {
            // Call the AddBedMaster Method to Add / Update current BedMaster
            var currentBedMaster = _bedService.GetBedMasterById(Convert.ToInt32(model.Id));

            // If the view is shown in ViewMode only, then ViewBag.BedMaster is set to true otherwise false.
            ViewBag.ViewOnly = !string.IsNullOrEmpty(model.ViewOnly);

            // Pass the ActionResult with the current BedMasterViewModel object as model to PartialView BedMasterAddEdit
            return PartialView(PartialViews.BedMasterAddEdit, currentBedMaster);

        }

        /// <summary>
        /// Delete the current facility based on the BedMaster ID passed in the SharedViewModel
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public ActionResult DeleteBedMaster(CommonModel model)
        {
            // Get facility model object by current facility ID
            var currentBedMaster = _bedService.GetBedMasterById(Convert.ToInt32(model.Id));

            // Check If facility model is not null
            if (currentBedMaster != null)
            {
                currentBedMaster.IsDeleted = true;
                currentBedMaster.DeletedBy = Helpers.GetLoggedInUserId();
                currentBedMaster.DeletedDate = Helpers.GetInvariantCultureDateTime();

                // Update Operation of current facility
                var result = _bedService.AddUpdateBedMaster(currentBedMaster);

                // return deleted ID of current facility as Json Result to the Ajax Call.
                return Json(result);
            }


            // Return the Json result as Action Result back JSON Call Success
            return Json(null);
        }

        /// <summary>
        /// Reset the BedMaster View Model and pass it to BedMasterAddEdit Partial View.
        /// </summary>
        /// <returns></returns>
        public ActionResult ResetBedMasterForm()
        {
            // Intialize the new object of BedMaster ViewModel
            var bedMasterModel = new UBedMaster();

            // Pass the View Model as BedMasterViewModel to PartialView BedMasterAddEdit just to update the AddEdit partial view.
            return PartialView(PartialViews.BedMasterAddEdit, bedMasterModel);
        }


        #region Beds Overview
        /// <summary>
        /// Overviews this instance.
        /// </summary>
        /// <returns></returns>
        public ActionResult Overview()
        {
            return View();
        }
        #endregion
    }
}