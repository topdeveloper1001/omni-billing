using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.IO;
using BillingSystem.Models;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;
using BillingSystem.Common;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common.Common;

namespace BillingSystem.Controllers
{
    public class FacilityStructureController : BaseController
    {
        private readonly IFacilityStructureService _service;
        private readonly IEncounterService _eService;
        private readonly IPatientInfoService _piService;
        private readonly IBedMasterService _bedService;
        private readonly IBedRateCardService _brService;
        private readonly IAppointmentTypesService _aService;
        private readonly IFacilityService _fService;
        private readonly IGlobalCodeService _gService;
        private readonly IEquipmentService _eqService;
        private readonly IDeptTimmingService _dtimService;
        private readonly IServiceCodeService _scService;
        private readonly IPhysicianService _phService;

        public FacilityStructureController(IFacilityStructureService service, IEncounterService eService, IPatientInfoService piService, IBedMasterService bedService, IBedRateCardService brService, IAppointmentTypesService aService, IFacilityService fService, IGlobalCodeService gService, IEquipmentService eqService, IDeptTimmingService dtimService, IServiceCodeService scService, IPhysicianService phService)
        {
            _service = service;
            _eService = eService;
            _piService = piService;
            _bedService = bedService;
            _brService = brService;
            _aService = aService;
            _fService = fService;
            _gService = gService;
            _eqService = eqService;
            _dtimService = dtimService;
            _scService = scService;
            _phService = phService;
        }


        #region Public Methods and Operators

        /// <summary>
        ///     Get the details of the FacilityStructure View in the Model FacilityStructure such as FacilityStructureList, list of
        ///     countries etc.
        /// </summary>
        /// <returns>
        ///     returns the actionresult in the form of current object of the Model FacilityStructure to be passed to View
        ///     FacilityStructure
        /// </returns>
        public ActionResult Index()
        {

            // Intialize the View Model i.e. FacilityStructureView which is binded to Main View Index.cshtml under FacilityStructure
            var fsView = new FacilityStructureView
            {
                // FacilityStructureList = facilityStructureList,
                FacilityStructureList = new List<FacilityStructureCustomModel>(),
                CurrentFacilityStructure = new FacilityStructureCustomModel { ServiceCodesList = GetBedOverrideList(), EquipmentList = new List<EquipmentMaster>(), AppointmentList = new List<AppointmentTypes>() }
            };

            // Pass the View Model in ActionResult to View FacilityStructure
            return View(fsView);
        }

        /// <summary>
        /// Binds the facility data.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult BindFacilityData()
        {
            var userIsAdmin = Helpers.GetLoggedInUserIsAdmin();
            var cId = Helpers.GetDefaultCorporateId();
            var facId = 0;
            if (!userIsAdmin)
            {
                facId = Helpers.GetDefaultFacilityId();
            }

            var facilityList = _fService.GetFacilities(cId, facId);
            var list =
                facilityList.Select(
                    item => new SelectListItem { Text = item.FacilityName, Value = Convert.ToString(item.FacilityId) })
                    .ToList();
            return Json(list);
        }

        /// <summary>
        /// Bind all the FacilityStructure list
        /// </summary>
        /// <param name="facilityid">
        /// The facilityid.
        /// </param>
        /// <param name="structureId">
        /// The structure identifier.
        /// </param>
        /// <param name="showInActive">
        /// if set to <c>true</c> [show in active].
        /// </param>
        /// <returns>
        /// action result with the partial view containing the FacilityStructure list object
        /// </returns>
        public ActionResult BindFacilityStructureGrid(string facilityid, int structureId, bool showInActive)
        {
            var currentFacilityId = 0;
            var fList = new List<FacilityStructureCustomModel>();
            if (Session[SessionNames.FacilityStructureData.ToString()] != null && structureId == 0)
            {
                var fdata = Session[SessionNames.FacilityStructureData.ToString()] as FacilityDataTemp;
                if (fdata != null)
                {
                    currentFacilityId = fdata.CurrentFacilityId;
                    fList = fdata.FacStructureData;
                }
            }

            // var facilityStructureList=facilityStructureBal.GetfacilityStructureData(Convert.ToInt32(facilityid), structureId,showInActive);

            var facilityStructureList = showInActive
                ? _service
                    .GetfacilityStructureData
                    (Convert.ToInt32(facilityid), structureId, true)
                : ((int.Parse(facilityid) == currentFacilityId && structureId == 0 &&
                    fList.Count > 0)
                    ? fList
                    : _service.GetfacilityStructureData(Convert.ToInt32(facilityid), structureId, false));
            //: bal.GetFacilityStructureCustom(facilityid, structureId));

            if (showInActive && int.Parse(facilityid) > 0 && structureId == 0)
            {
                currentFacilityId = Convert.ToInt32(facilityid);
                fList = facilityStructureList;
                var newObj = new FacilityDataTemp
                {
                    CurrentFacilityId = currentFacilityId,
                    FacStructureData = fList
                };
                Session[SessionNames.FacilityStructureData.ToString()] = newObj;
            }



            switch (structureId)
            {
                case 82:
                    return PartialView(PartialViews.FacilityStructureFloorList, facilityStructureList);
                case 83:
                    return PartialView(PartialViews.FacilityStructureDeptList, facilityStructureList);
                case 84:
                    return PartialView(PartialViews.FacilityStructureRoomsList, facilityStructureList);
                case 85:
                    return PartialView(PartialViews.FacilityStructureBedsList, facilityStructureList);
            }

            // Pass the ActionResult with List of FacilityStructureViewModel object to Partial View FacilityStructureList
            return PartialView(PartialViews.FacilityStructureList, facilityStructureList);
        }

        /// <summary>
        /// Bind all the FacilityStructure list
        /// </summary>
        /// <param name="facilityid">
        /// The facilityid.
        /// </param>
        /// <returns>
        /// action result with the partial view containing the FacilityStructure list object
        /// </returns>
        public ActionResult BindFacilityStructureGridTreeView(string facilityid)
        {

            var fList = new List<FacilityStructureCustomModel>();
            if (Session[SessionNames.FacilityStructureData.ToString()] != null)
            {
                var fdata = Session[SessionNames.FacilityStructureData.ToString()] as FacilityDataTemp;
                if (fdata != null && fdata.CurrentFacilityId == int.Parse(facilityid))
                    fList = fdata.FacStructureData;

            }
            var facilityStructureList = fList.Count > 0
               ? fList
               : _service.GetFacilityStructureCustom(facilityid, 0);

            // Pass the ActionResult with List of FacilityStructureViewModel object to Partial View FacilityStructureList
            var partialviewData = PartialView(PartialViews.FacilityStructureTreeView,
                facilityStructureList);
            return partialviewData;

        }



        /// <summary>
        ///     Bind all the FacilityStructure list
        /// </summary>
        /// <returns>
        ///     action result with the partial view containing the FacilityStructure list object
        /// </returns>
        public ActionResult BindFacilityStructureList()
        {

            var facilityId = Convert.ToString(Helpers.GetDefaultFacilityId());

            // Get the facilities list
            var list = _service.GetFacilityStructure(facilityId);
            list = list.Where(x => x.FacilityId.Equals(facilityId)).ToList();

            // Pass the ActionResult with List of FacilityStructureViewModel object to Partial View FacilityStructureList
            return PartialView(PartialViews.FacilityStructureList, list);

        }

        /// <summary>
        /// Delete the current FacilityStructure based on the FacilityStructure ID passed in the FacilityStructureModel
        /// </summary>
        /// <param name="facilityStructureId">
        /// The facility structure identifier.
        /// </param>
        /// <param name="bedid">
        /// The bedid.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult DeleteFacilityStructure(int facilityStructureId, int bedid)
        {
            var currentFacilityStructure =
                 _service.GetFacilityStructureById(Convert.ToInt32(facilityStructureId));

            // apply check if the current structure id have childs
            var canDelete = _service.CheckForChildrens(facilityStructureId);
            if (canDelete)
            {
                return Json(-1);
            }

            if (bedid != 0)
            {
                var canDeleteBed = _bedService.CheckIdBedOccupied(bedid);
                if (canDeleteBed)
                {
                    return Json(-2);
                }

            }

            var userid = Helpers.GetLoggedInUserId();

            // Check If FacilityStructure model is not null
            if (currentFacilityStructure != null)
            {
                if (currentFacilityStructure.GlobalCodeID == 85)
                {
                    _bedService.DeleteBedMasterById(Convert.ToInt32(facilityStructureId));
                }

                currentFacilityStructure.IsDeleted = true;
                currentFacilityStructure.DeletedBy = userid;
                currentFacilityStructure.DeletedDate = Helpers.GetInvariantCultureDateTime();

                // Update Operation of current FacilityStructure
                var result = _service.DeleteFacilityStructureById(Convert.ToInt32(facilityStructureId));

                // return deleted ID of current FacilityStructure as Json Result to the Ajax Call.
                return Json(result);
            }


            // Return the Json result as Action Result back JSON Call Success
            return Json(null);
        }

        /// <summary>
        /// Gets the type of the bed charges by bed.
        /// </summary>
        /// <param name="bedType">
        /// Type of the bed.
        /// </param>
        /// <returns>
        /// The <see cref="JsonResult"/>.
        /// </returns>
        public JsonResult GetBedChargesByBedType(int bedType)
        {
            var bedRate = _brService.GetBedRateByBedTypeId(bedType);
            return Json(bedRate, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Gets the bed override DDL.
        /// </summary>
        /// <param name="bedTypeId">
        /// The bed type identifier.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult GetBedOverrideDropdownData(int bedTypeId)
        {
            var bedTypeList = _gService.GetGlobalCodesByCategoryValue(Convert.ToString((int)GlobalCodeCategoryValue.Bedtypes));
            var currentBedtypeOrder = _gService.GetGlobalCodeByGlobalCodeId(bedTypeId);
            bedTypeList =
                bedTypeList.Where(x => x.ExternalValue2 == "1" && x.SortOrder < currentBedtypeOrder.SortOrder)
                    .ToList()
                    .OrderBy(so => so.SortOrder)
                    .ToList();
            return Json(bedTypeList);

        }

        /// <summary>
        /// Gets the bread crumbs.
        /// </summary>
        /// <param name="structureId">
        /// The structure identifier.
        /// </param>
        /// <param name="facilityid">
        /// The facilityid.
        /// </param>
        /// <param name="parentId">
        /// The parent identifier.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult GetBreadCrumbs(int structureId, string facilityid, string parentId)
        {
            var list = _service.GetFacilityStructureBreadCrumbs(structureId, facilityid, parentId);
            return Json(list);
        }

        /// <summary>
        /// Get the details of the current FacilityStructure in the view model by ID
        /// </summary>
        /// <param name="facilityStructureId">
        /// facilityStructureId
        /// </param>
        /// <returns>
        /// Partial View
        /// </returns>
        public ActionResult GetFacilityStructure(int facilityStructureId)
        {
            FacilityStructureCustomModel vm;
            var currentFacilityStructure = _service.GetFacilityStructureById(facilityStructureId);
            UBedMaster ubedmaster;
            ubedmaster = _bedService.GetBedMasterByStructureId(facilityStructureId);

            vm = new FacilityStructureCustomModel
            {
                TimingAdded = _dtimService.GetTimingAddedById(facilityStructureId),
                BedTypeId =
                    ubedmaster != null
                        ? ubedmaster.BedType
                        : 0,
                BedId =
                    ubedmaster != null
                        ? ubedmaster.BedId
                        : 0,
                FacilityStructureId =
                    currentFacilityStructure
                        .FacilityStructureId,
                GlobalCodeID =
                    currentFacilityStructure
                        .GlobalCodeID,
                FacilityStructureValue =
                    currentFacilityStructure
                        .FacilityStructureValue,
                FacilityStructureName =
                    currentFacilityStructure
                        .FacilityStructureName,
                Description =
                    currentFacilityStructure
                        .Description,
                ParentId =
                    currentFacilityStructure
                        .ParentId,
                ParentTypeGlobalID =
                    currentFacilityStructure
                        .ParentTypeGlobalID,
                FacilityId =
                    currentFacilityStructure
                        .FacilityId,
                SortOrder =
                    currentFacilityStructure
                        .SortOrder,
                IsActive =
                    currentFacilityStructure
                        .IsActive,
                CreatedBy =
                    currentFacilityStructure
                        .CreatedBy,
                CreatedDate =
                    currentFacilityStructure
                        .CreatedDate,
                ModifiedBy =
                    currentFacilityStructure
                        .ModifiedBy,
                ModifiedDate =
                    currentFacilityStructure
                        .ModifiedDate,
                IsDeleted =
                    currentFacilityStructure
                        .IsDeleted,
                DeletedBy =
                    currentFacilityStructure
                        .DeletedBy,
                DeletedDate =
                    currentFacilityStructure
                        .DeletedDate,
                ExternalValue1 =
                    currentFacilityStructure
                        .ExternalValue1,
                ExternalValue2 =
                    currentFacilityStructure
                        .ExternalValue2,
                ExternalValue3 =
                    currentFacilityStructure
                        .ExternalValue3,
                ExternalValue4 =
                    currentFacilityStructure
                        .ExternalValue4,
                ExternalValue5 =
                    currentFacilityStructure
                        .ExternalValue5,
                CanOverRideValue =
                    currentFacilityStructure
                        .GlobalCodeID
                    == Convert.ToInt32(
                        BaseFacilityStucture
                            .Bed)
                    && !string.IsNullOrEmpty(
                        currentFacilityStructure
                            .ExternalValue1)
                        ? "Yes"
                        : "No",
                CanOverRide =
                    currentFacilityStructure
                        .GlobalCodeID
                    == Convert.ToInt32(
                        BaseFacilityStucture
                            .Bed)
                    && !string.IsNullOrEmpty(
                        currentFacilityStructure
                            .ExternalValue1),
                AvailableInOverRideList =
                    currentFacilityStructure
                        .GlobalCodeID
                    == Convert.ToInt32(
                        BaseFacilityStucture
                            .Bed)
                    && !string.IsNullOrEmpty(
                        currentFacilityStructure
                            .ExternalValue2),
                OverRidePriority =
                    currentFacilityStructure
                        .GlobalCodeID
                    == Convert.ToInt32(
                        BaseFacilityStucture
                            .Bed)
                    && !string.IsNullOrEmpty(
                        currentFacilityStructure
                            .ExternalValue3)
                        ? Convert.ToInt32(
                            currentFacilityStructure
                                .ExternalValue3)
                        : 0,
                RevenueGLAccount =
                    currentFacilityStructure
                        .GlobalCodeID
                    == Convert.ToInt32(
                        BaseFacilityStucture
                            .Department)
                        ? currentFacilityStructure
                            .ExternalValue1
                        : string.Empty,
                ARMasterAccount =
                    currentFacilityStructure
                        .GlobalCodeID
                    == Convert.ToInt32(
                        BaseFacilityStucture
                            .Department)
                        ? currentFacilityStructure
                            .ExternalValue2
                        : string.Empty,
                NonChargeableRoom =
                    currentFacilityStructure
                        .GlobalCodeID
                    == Convert.ToInt32(
                        BaseFacilityStucture
                            .Rooms)
                    && !string.IsNullOrEmpty(
                        currentFacilityStructure
                            .ExternalValue1)
                        ? "Yes"
                        : "No",
                DeptClosingTime =
                    currentFacilityStructure
                        .DeptClosingTime,
                DeptOpeningDays =
                    currentFacilityStructure
                        .DeptOpeningDays,
                DeptOpeningTime =
                    currentFacilityStructure
                        .DeptOpeningTime,
                DeptTurnaroundTime =
                    currentFacilityStructure
                        .DeptTurnaroundTime,
                CorporateId =
                    _gService.GetCorporateIdFromFacilityId(Convert.ToInt32(currentFacilityStructure.FacilityId)),
                EquipmentIds = currentFacilityStructure.EquipmentIds,
                ServiceCodesList = GetBedOverrideList()
            };

            if (currentFacilityStructure.GlobalCodeID == 84)
            {
                var equipmentList = _eqService.GetEquipmentListByFacilityId(currentFacilityStructure.FacilityId, facilityStructureId);
                vm.EquipmentList = equipmentList;


                var appointmnetList = _aService.GetAppointmneTypesByFacilityId(Convert.ToInt32(currentFacilityStructure.FacilityId));
                vm.AppointmentList = appointmnetList;
            }
            else
            {
                vm.EquipmentList = new List<EquipmentMaster>();
                vm.AppointmentList = new List<AppointmentTypes>();
            }

            return PartialView(PartialViews.FacilityStructureAddEdit, vm);
        }

        /// <summary>
        /// Gets the facility stucture list TreeView.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult GetFacilityStuctureListTreeView()
        {
            var facilityId = Helpers.GetDefaultFacilityId();
            var facilityStructureList = _service.GetFacilityStructureCustom(Convert.ToString(facilityId), 0);
            return PartialView(PartialViews.FacilityStructureTreeView, facilityStructureList);

        }

        /// <summary>
        /// Gets the maximum sort order.
        /// </summary>
        /// <param name="facilityId">
        /// The facility identifier.
        /// </param>
        /// <param name="structureType">
        /// Type of the structure.
        /// </param>
        /// <returns>
        /// The <see cref="JsonResult"/>.
        /// </returns>
        public JsonResult GetMaxSortOrder(string facilityId, string structureType)
        {
            var sortOrder = _service.GetMaxSortOrder(facilityId, structureType);
            return Json(sortOrder);
        }

        /// <summary>
        /// Gets the parent value.
        /// </summary>
        /// <param name="structureId">
        /// The structure identifier.
        /// </param>
        /// <param name="facilityid">
        /// The facilityid.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult GetParentValue(int structureId, string facilityid)
        {
            var list = _service.GetFacilityStructureParent(structureId, facilityid);
            return Json(list);
        }

        /// <summary>
        /// Reset the FacilityStructure View Model and pass it to FacilityStructureAddEdit Partial View.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult ResetFacilityStructureForm()
        {
            //var serviceCodeBal = new ServiceCodeBal(Helpers.DefaultServiceCodeTableNumber);
            //var serviceCodesList =
            //    serviceCodeBal.GetServiceCodes()
            //        .Where(x => x.CanOverRide == 1)
            //        .OrderBy(X => X.ServiceCodeValue)
            //        .ToList();

            // Intialize the new object of FacilityStructure ViewModel
            var facilityStructureViewModel = new FacilityStructureCustomModel { ServiceCodesList = GetBedOverrideList() };

            // Pass the View Model as FacilityStructureViewModel to PartialView FacilityStructureAddEdit just to update the AddEdit partial view.
            return PartialView(PartialViews.FacilityStructureAddEdit, facilityStructureViewModel);
        }

        /// <summary>
        /// Add New or Update the FacilityStructure based on if we pass the FacilityStructure ID in the
        ///     FacilityStructureViewModel object.
        /// </summary>
        /// <param name="vm">
        /// pass the details of FacilityStructure in the view model
        /// </param>
        /// <returns>
        /// returns the newly added or updated ID of FacilityStructure row
        /// </returns>
        public ActionResult SaveFacilityStructure(FacilityStructureCustomModel vm)
        {
            // Initialize the newId variable 
            var newId = -1;
            var userid = Helpers.GetLoggedInUserId();
            var currentDate = Helpers.GetInvariantCultureDateTime();

            // Check if FacilityStructureViewModel 
            if (vm != null)
            {
                // Apply check if the data is inactive and it have cascading child with it.
                if (!vm.IsActive)
                {
                    var checkIfChildExist = _service.CheckForChildrens(vm.FacilityStructureId);
                    if (checkIfChildExist)
                        return Json(-1);
                }

                if (vm.FacilityStructureId > 0)
                {
                    vm.ModifiedBy = userid;
                    vm.ModifiedDate = Helpers.GetInvariantCultureDateTime();
                }
                else
                {
                    vm.CreatedBy = userid;
                    vm.CreatedDate = Helpers.GetInvariantCultureDateTime();
                }

                vm.IsDeleted = false;
                var facilitysturcture = new FacilityStructure
                {
                    FacilityStructureId =
                                                    vm.FacilityStructureId,
                    GlobalCodeID = vm.GlobalCodeID,
                    FacilityStructureValue =
                                                    vm.FacilityStructureValue,
                    FacilityStructureName =
                                                    vm.FacilityStructureName,
                    Description = vm.Description,
                    ParentId = vm.ParentId,
                    ParentTypeGlobalID =
                                                    vm.ParentTypeGlobalID,
                    FacilityId = vm.FacilityId,
                    SortOrder = vm.SortOrder,
                    IsActive = vm.IsActive,
                    CreatedBy = userid,
                    CreatedDate = currentDate,
                    ModifiedBy = vm.ModifiedBy,
                    ModifiedDate = currentDate,
                    IsDeleted = false,
                    DeletedBy = vm.DeletedBy,
                    DeletedDate = vm.DeletedDate,
                    ExternalValue1 =
                                                    vm.ExternalValue1 == "0"
                                                        ? string.Empty
                                                        : vm.ExternalValue1,
                    ExternalValue2 =
                                                    vm.ExternalValue2 == "0"
                                                        ? string.Empty
                                                        : vm.ExternalValue2,
                    ExternalValue3 =
                                                    vm.ExternalValue3 == "0"
                                                        ? string.Empty
                                                        : vm.ExternalValue3,
                    ExternalValue4 =
                                                    vm.ExternalValue4,

                    ExternalValue5 =
                                                    vm.ExternalValue5 == "0"
                                                        ? string.Empty
                                                        : vm.ExternalValue5,
                    EquipmentIds = vm.EquipmentIds

                };

                // Call the AddFacilityStructure Method to Add / Update current FacilityStructure
                if (!_service.CheckStructureExist(facilitysturcture))
                {
                    newId = _service.AddUptdateFacilityStructure(facilitysturcture);

                    /* On: 25 November, 2015
                     * By: Amit Jain
                     * Purpose: Add the Facility Structure reference to Equipment Master table conditionally.
                     */
                    if (!string.IsNullOrEmpty(facilitysturcture.EquipmentIds))
                    {
                        var eqList = facilitysturcture.EquipmentIds.Split(',').Select(Int32.Parse).ToList();
                        if (eqList.Count > 0)
                        {
                            _eqService.AddRoomIdToEquipments(facilitysturcture.FacilityStructureId,
                                eqList);
                        }
                    }


                    // Add Department Timmings 
                    if (newId > 0 && vm.GlobalCodeID == 83)
                    {

                        if (vm.DeptTimmingsList != null)
                        {
                            var facilityDepartmentList = new List<DeptTimming>();
                            facilityDepartmentList.AddRange(
                                vm.DeptTimmingsList.Select(item => new DeptTimming
                                {
                                    FacilityStructureID = newId,
                                    OpeningDayId = item.OpeningDayId,
                                    ClosingTime = item.ClosingTime,
                                    OpeningTime = item.OpeningTime,
                                    TrunAroundTime = item.TrunAroundTime,
                                    DeptTimmingId = 0,
                                    IsActive = true,
                                    CreatedBy = userid,
                                    CreatedDate = currentDate,
                                }));
                            _dtimService.SaveDeptTimmingList(facilityDepartmentList);

                        }
                        else
                        {
                            _dtimService.DeleteDepartmentTiming(newId);
                        }
                    }
                    else if (vm.FacilityStructureId > 0
                             && vm.GlobalCodeID == 85)
                    {
                        var getBedMasterObj = _bedService.GetBedMasterById(vm.BedId);
                        if (getBedMasterObj != null)
                        {
                            var ubedMasterToUpdate = new UBedMaster
                            {
                                BedId = vm.BedId,
                                FacilityId =
                                                                 Convert.ToInt32(
                                                                     vm.FacilityId),
                                FacilityStructureId = newId,
                                BedType = vm.BedTypeId,
                                Rate = null,
                                StartDate =
                                                                 vm.CreatedDate,
                                IsOccupied = getBedMasterObj.IsOccupied,
                                IsRateApplied = null,
                                SortOrder = null,
                                IsActive = true,
                                CreatedBy = getBedMasterObj.CreatedBy,
                                CreatedDate = getBedMasterObj.CreatedDate,
                                ModifiedBy =
                                                                 vm.ModifiedBy,
                                ModifiedDate =
                                                                 vm.ModifiedDate,
                                IsDeleted =
                                                                 vm.IsDeleted,
                                DeletedBy =
                                                                 vm.DeletedBy,
                                DeletedDate =
                                                                 vm.DeletedDate
                            };

                            // Call the AddBedMaster Method to Add / Update current BedMaster
                            _bedService.AddUpdateBedMaster(ubedMasterToUpdate);
                        }

                    }
                    else if (vm.GlobalCodeID == 85)
                    {
                        var ubedMasterToAdd = new UBedMaster
                        {
                            BedId = 0,
                            FacilityId =
                                                          Convert.ToInt32(
                                                              vm.FacilityId),
                            FacilityStructureId = newId,
                            BedType = vm.BedTypeId,
                            Rate = null,
                            StartDate = vm.CreatedDate,
                            IsOccupied = false,
                            IsRateApplied = null,
                            SortOrder = null,
                            IsActive = true,
                            CreatedBy = vm.CreatedBy,
                            CreatedDate = vm.CreatedDate,
                            ModifiedBy = vm.ModifiedBy,
                            ModifiedDate =
                                                          vm.ModifiedDate,
                            IsDeleted = vm.IsDeleted,
                            DeletedBy = vm.DeletedBy,
                            DeletedDate = vm.DeletedDate
                        };

                        // Call the AddBedMaster Method to Add / Update current BedMaster
                        _bedService.AddUpdateBedMaster(ubedMasterToAdd);

                    }
                }
                else
                {
                    return Json("isExist");
                }

            }

            return Json(newId);
        }

        /// <summary>
        /// Binds the department timmings.
        /// </summary>
        /// <param name="facilityStructureId">The facility structure identifier.</param>
        /// <returns></returns>
        public ActionResult BindDepartmentTimmings(int facilityStructureId)
        {
            var deptTimmingobj = _dtimService.GetDepTimingsById(facilityStructureId);
            return Json(deptTimmingobj, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBedChargesByBedType1(string bedType)
        {
            //var bedRateCardBal = new BedRateCardBal();
            //var bedRate = bedRateCardBal.GetBedRateByBedTypeId(bedType);
            //return Json(bedRate, JsonRequestBehavior.AllowGet);

            var obj = _scService.GetServiceCodeByCodeValue(bedType, Helpers.DefaultServiceCodeTableNumber);
            return Json(obj != null ? obj.ServiceCodePrice : 0, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetDepartmentsByFacility(int facilityId)
        {
            var list = new List<SelectListItem>();
            var corporateUsers = new List<PhysicianCustomModel>();

            var facilityDepartments = _service.GetFacilityDepartments(Helpers.GetSysAdminCorporateID(), facilityId.ToString());
            if (facilityDepartments.Any())
            {
                list.AddRange(facilityDepartments.Select(item => new SelectListItem
                {
                    Text = string.Format(" {0} ", item.FacilityStructureName),
                    Value = Convert.ToString(item.FacilityStructureId)
                }));
            }
            var cId = Helpers.GetSysAdminCorporateID().ToString();
            cId = facilityId == 0
                ? cId
                : Helpers.GetCorporateIdByFacilityId(Convert.ToInt32(facilityId)).ToString();
            var isAdmin = Helpers.GetLoggedInUserIsAdmin();
            var userid = Helpers.GetLoggedInUserId();
            corporateUsers = _phService.GetCorporatePhysiciansList(Convert.ToInt32(cId), isAdmin, userid, Convert.ToInt32(facilityId));

            var updatedList = new
            {
                deptList = list,
                phyList = corporateUsers
            };
            return Json(updatedList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult LoadFacilityDepartmentData(string facilityid)
        {
            // Get the facilities list
            var cId = Helpers.GetSysAdminCorporateID();
            var facilityDepartmentList = _service.GetFacilityDepartments(cId, facilityid);
            var viewpath = string.Format("../Scheduler/{0}", PartialViews.FacilityDepartmentListView);
            return PartialView(viewpath, facilityDepartmentList);

        }
        public ActionResult LoadFacilityRoomsData(string facilityid)
        {
            // Get the facilities list
            var cId = Helpers.GetSysAdminCorporateID();
            var lst = _service.GetFacilityRooms(cId, facilityid);
            var viewpath = string.Format("../Scheduler/{0}", PartialViews.FacilityRoomsListView);
            return PartialView(viewpath, lst);
        }


        /// <summary>
        /// Loads the facility rooms data custom.
        /// </summary>
        /// <param name="facilityid">The facilityid.</param>
        /// <returns></returns>
        public ActionResult LoadFacilityRoomsDataCustom(string facilityid)
        {
            // Get the facilities list
            var cId = Helpers.GetSysAdminCorporateID();
            var facilityDepartmentList = _service.GetFacilityRoomsCustomModel(cId, facilityid);
            var viewpath = string.Format("../Scheduler/{0}", PartialViews.FacilityRoomsListView);
            return PartialView(viewpath, facilityDepartmentList);
        }

        /// <summary>
        /// Gets the department rooms.
        /// </summary>
        /// <param name="filters">The filters.</param>
        /// <returns></returns>
        public ActionResult GetDepartmentRooms(List<SchedularTypeCustomModel> filters)
        {
            var selectedDepartmentList = filters[0].DeptData;
            var facilityid = filters[0].Facility;
            //var deptIds = string.Join(",", selectedDepartmentList.Select(x => x.Id));
            var facilityDepartmentList = _service.GetDepartmentRooms(selectedDepartmentList, facilityid);
            var viewpath = string.Format("../Scheduler/{0}", PartialViews.FacilityRoomsListView);
            return PartialView(viewpath, facilityDepartmentList);
        }
        public ActionResult ValidateDepartmentRooms(string facilityid, int deptid)
        {
            var lst = _service.GetDepartmentRooms(deptid, facilityid);
            return Json(lst.Count > 0, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetPhysiciansApptTypes(string facilityid, int deptid)
        {
            var lst = _service.GetDepartmentAppointmentTypes(deptid, facilityid);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFacilityRooms(int coporateId, int facilityId)
        {
            var facilityDepartmentList = _service.GetFacilityRooms(coporateId, facilityId.ToString());
            return Json(facilityDepartmentList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDepartmentNameByRoomId(int roomId)
        {
            var departmentName = _service.GetParentNameByFacilityStructureId(roomId);
            return Json(departmentName, JsonRequestBehavior.AllowGet);
        }




        #endregion


        private List<ServiceCode> GetBedOverrideList()
        {
            return _scService.GetOveridableBedList(Helpers.DefaultServiceCodeTableNumber);
        }

        #region Appointment Rooms View

        public ActionResult AppointmentRoomsView()
        {
            var fsView = new FacilityStructureView
            {
                FacilityStructureList = _service.GetAppointRoomAssignmentsList(Helpers.GetDefaultFacilityId(), string.Empty),
                CurrentFacilityStructure = new FacilityStructureCustomModel { AppointmentList = new List<AppointmentTypes>(), FacilityId = Convert.ToString(Helpers.GetDefaultFacilityId()) }
            };
            return View(fsView);

        }

        public PartialViewResult BindAppointmentListByFacility(int facilityId)
        {
            var list = _aService.GetAppointmneTypesByFacilityId(facilityId);
            return PartialView(PartialViews.AppointmentsListViewInFacilityStructure, list);

        }

        public ActionResult SaveAppointmentRooms(int facilityStructureId, string appointmentTypeIds)
        {
            var current = _service.GetFacilityStructureById(facilityStructureId);
            current.ExternalValue4 = !string.IsNullOrEmpty(appointmentTypeIds) ? appointmentTypeIds : string.Empty;
            _service.AddUptdateFacilityStructure(current);
            return Json(true, JsonRequestBehavior.AllowGet);

        }

        public ActionResult BindAppointmentRoomAssignmentList(int facilityId, string txtSearch)
        {

            var list = _service.GetAppointRoomAssignmentsList(facilityId, txtSearch);
            return PartialView(PartialViews.AssignedAppointmentsList, list);

        }

        public ActionResult EditAppointmentTypeRoomStructure(int facilityStructureId)
        {
            var current = _service.GetFacilityStructureById(facilityStructureId);
            return Json(current, JsonRequestBehavior.AllowGet);

        }
        #endregion







        [HttpPost]
        public ActionResult BindGlobalCodesDropdownData(int structureId, int bedType, bool loadFacilities)
        {
            var categories = new List<string> { "5001", "1001", };
            var bedRate = "0.00";

            List<DropdownListData> list;
            var listParentStructure = new List<DropdownListData>();
            var listFacilities = new List<DropdownListData>();
            list = _gService.GetListByCategoriesRange(categories);


            if (loadFacilities)
            {
                var userIsAdmin = Helpers.GetLoggedInUserIsAdmin();
                var cId = Helpers.GetDefaultCorporateId();
                var facId = 0;

                if (!userIsAdmin)
                    facId = Helpers.GetDefaultFacilityId();


                listFacilities = _fService.GetFacilityDropdownData(cId, facId);
            }

            if (structureId > 0)
            {
                var parentId = 0;
                switch (structureId)
                {
                    case 82:
                        parentId = 0;
                        break;
                    case 83:
                        parentId = 82;
                        break;
                    case 84:
                        parentId = 83;
                        break;
                    case 85:
                        parentId = 84;
                        break;
                }
                listParentStructure = _service.GetFacilityStructureListByParentId(parentId);
            }


            if (structureId == 85 && bedType > 0)
            {
                bedRate = _brService.GetBedRateByBedTypeId(bedType);
            }

            var jsonData = new
            {
                listFacilityStructure = list.Where(g => g.ExternalValue1.Equals("5001")).OrderBy(g => g.Value).ToList(),
                listBedTypes = list.Where(g => g.ExternalValue1.Equals("1001")).OrderBy(m => m.Text).ToList(),
                bedRate,
                listParentStructure,
                listFacilities
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// Get the details of the current FacilityStructure in the view model by ID
        /// </summary>
        /// <param name="facilityStructureId">
        /// facilityStructureId
        /// </param>
        /// <returns>
        /// Partial View
        /// </returns>
        public JsonResult GetFacilityStructureDetails(int facilityStructureId)
        {
            var model = _service.GetFacilityStructureById(facilityStructureId);

            var vm = new FacilityStructureCustomModel
            {
                FacilityStructureId = model.FacilityStructureId,
                GlobalCodeID = model.GlobalCodeID,
                FacilityStructureValue = model.FacilityStructureValue,
                FacilityStructureName = model.FacilityStructureName,
                Description = model.Description,
                ParentId = model.ParentId,
                ParentTypeGlobalID = model.ParentTypeGlobalID,
                FacilityId = model.FacilityId,
                SortOrder = model.SortOrder,
                IsActive = model.IsActive,
                CreatedBy = model.CreatedBy,
                CreatedDate = model.CreatedDate,
                ExternalValue1 = model.ExternalValue1,
                ExternalValue2 = model.ExternalValue2,
                ExternalValue3 = model.ExternalValue3,
                ExternalValue4 = model.ExternalValue4,
                ExternalValue5 = model.ExternalValue5,
                DeptClosingTime = model.DeptClosingTime,
                DeptOpeningDays = model.DeptOpeningDays,
                DeptOpeningTime = model.DeptOpeningTime,
                DeptTurnaroundTime = model.DeptTurnaroundTime,
            };

            var parentStructureId = 0;
            var listBedTypes = new List<DropdownListData>();
            List<DropdownListData> listParentStructure;
            var listDepTimings = new List<DeptTimming>();

            var partialViewServiceCodes = string.Empty;
            var fsType = Convert.ToInt32(model.GlobalCodeID);
            switch (fsType)
            {
                case 83:
                    listDepTimings = _dtimService.GetDepTimingsById(facilityStructureId);
                    vm.RevenueGLAccount = model.ExternalValue1;
                    vm.ARMasterAccount = model.ExternalValue2;
                    parentStructureId = 82;
                    break;

                case 84:
                    vm.NonChargeableRoom = !string.IsNullOrEmpty(model.ExternalValue1) ? "Yes" : "No";

                    parentStructureId = 83;
                    break;

                case 85:
                    parentStructureId = 84;

                    var ubedmaster = _bedService.GetBedMasterByStructureId(facilityStructureId);
                    vm.BedTypeId = ubedmaster.BedType;
                    vm.BedId = ubedmaster.BedId;

                    vm.BedCharge = _brService.GetBedRateByBedTypeId(Convert.ToInt32(ubedmaster.BedType));


                    vm.CanOverRide = !string.IsNullOrEmpty(model.ExternalValue1);
                    vm.AvailableInOverRideList = !string.IsNullOrEmpty(model.ExternalValue2);
                    vm.OverRidePriority = !string.IsNullOrEmpty(model.ExternalValue3) ? Convert.ToInt32(model.ExternalValue3) : 0;
                    vm.CanOverRideValue = !string.IsNullOrEmpty(model.ExternalValue1) ? "Yes" : "No";

                    var categories = new List<string> { "1001" };
                    listBedTypes = _gService.GetListByCategoriesRange(categories);

                    if (listBedTypes.Count > 0)
                        listBedTypes = listBedTypes.OrderBy(m => m.Text).ToList();

                    partialViewServiceCodes = RenderPartialViewToString(PartialViews.BedOverrideServiceCodesList, GetBedOverrideList());
                    break;
            }

            listParentStructure = _service.GetFacilityStructureListByParentId(parentStructureId);

            var jsonData = new
            {
                vm,
                listParentStructure,
                listBedTypes,
                listDepTimings,
                partialViewServiceCodes
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);

        }


        private string RenderPartialViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");

            ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }
    }


    public class FacilityDataTemp
    {
        public List<FacilityStructureCustomModel> FacStructureData { get; set; }
        public int CurrentFacilityId { get; set; }
    }
}