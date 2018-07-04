using BillingSystem.Bal.Interfaces;
using BillingSystem.Common;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using System;
using System.Web.Mvc;
using System.Linq;
using System.Collections.Generic;
using BillingSystem.Models;
using Microsoft.Ajax.Utilities;

namespace BillingSystem.Controllers
{
    public class EncounterController : BaseController
    {
        private readonly IEncounterService _service;
        private readonly IBedChargesService _bcservice;
        private readonly IBedRateCardService _brcservice;
        private readonly IBedMasterService _bedservice;
        private readonly IFacilityStructureService _fsservice;
        private readonly IBillHeaderService _bhservice;
        private readonly IUsersService _uService;
        private readonly IFacilityService _fService;
        private readonly IMappingPatientBedService _mpbService;
        private readonly IFutureOpenOrderService _foService;
        private readonly IPhysicianService _phService;
        private readonly IGlobalCodeService _gService;

        public EncounterController(IEncounterService service, IBedChargesService bcservice, IBedRateCardService brcservice, IBedMasterService bedservice, IFacilityStructureService fsservice, IBillHeaderService bhservice, IUsersService uService, IFacilityService fService, IMappingPatientBedService mpbService, IFutureOpenOrderService foService, IPhysicianService phService, IGlobalCodeService gService)
        {
            _service = service;
            _bcservice = bcservice;
            _brcservice = brcservice;
            _bedservice = bedservice;
            _fsservice = fsservice;
            _bhservice = bhservice;
            _uService = uService;
            _fService = fService;
            _mpbService = mpbService;
            _foService = foService;
            _phService = phService;
            _gService = gService;
        }

        // GET: /Encounter/
        #region Public Methods and Operators

        /// <summary>
        /// Indexes the specified patient identifier.
        /// </summary>
        /// <param name="patientId">
        /// The patient identifier.
        /// </param>
        /// <param name="messageId">
        /// The message identifier.
        /// </param>
        /// <param name="encId">
        /// The enc identifier.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult Index(string patientId, string messageId, string encId)
        {
            if (!string.IsNullOrEmpty(patientId) && _service.IsPatientExist(Convert.ToInt32(patientId)))
            {
                ViewBag.IsEncounterStart = !string.IsNullOrEmpty(messageId)
                                                && (Convert.ToInt32(messageId)
                                                    == Convert.ToInt32(EncounterStates.admitpatient)
                                                    || Convert.ToInt32(messageId)
                                                    == Convert.ToInt32(EncounterStates.outpatient)
                                                    || Convert.ToInt32(messageId)
                                                    == Convert.ToInt32(EncounterStates.transferpatient)
                                                    || Convert.ToInt32(messageId)
                                                    == Convert.ToInt32(EncounterStates.editencounter));
                ViewBag.Heading = GetHeading(!string.IsNullOrEmpty(messageId) ? Convert.ToInt32(messageId) : 1);
                ViewBag.PatientType = encId;
                var vm = !string.IsNullOrEmpty(encId) ?
                    _service.GetEncounterDetailByEncounterID(Convert.ToInt32(encId)) :
                    _service.GetEncounterDetailByPatientId(Convert.ToInt32(patientId));


                vm.EncounterEndTime = Helpers.GetInvariantCultureDateTime();

                /*
                * Owner: Amit Jain
                * On: 20102014
                * Purpose: Add the logged-in user's default facility to the patient info
                */
                // Additions start here
                var facilityId = Helpers.GetDefaultFacilityId();
                if (facilityId > 0)
                {
                    var defaultFacility = _fService.GetFacilityById(facilityId);
                    vm.FacilityName = defaultFacility.FacilityName;
                    vm.EncounterFacility = Convert.ToString(facilityId);
                }

                // Additions end here
                switch (messageId)
                {
                    case "1":
                        vm.EncounterPatientType = Convert.ToInt32(EncounterPatientType.InPatient);
                        break;
                    case "3":
                    case "5":
                    case "6":
                    case "9":
                        if (!string.IsNullOrEmpty(encId))
                        {
                            vm.EncounterPatientType = Convert.ToInt32(EncounterPatientType.InPatient);
                            var bedInfo = _service.GetPatientBedInformationByPatientId(
                                    Convert.ToInt32(vm.PatientID));
                            vm.BedName = bedInfo.BedName;
                            vm.FloorName = bedInfo.FloorName;
                            vm.Room = bedInfo.Room;
                            vm.DepartmentName = bedInfo.DepartmentName;
                            vm.patientBedService = bedInfo.patientBedService;
                            vm.BedAssignedOn = bedInfo.BedAssignedOn;
                            vm.BedRateApplicable = bedInfo.BedRateApplicable;
                        }

                        break;
                    case "2":
                    case "4":
                        if (vm.EncounterPatientType
                            != Convert.ToInt32(EncounterPatientType.ERPatient))
                        {
                            vm.EncounterPatientType =
                                Convert.ToInt32(EncounterPatientType.OutPatient);
                        }

                        break;
                }

                if (!vm.EncounterStartTime.HasValue)
                    vm.EncounterStartTime = Helpers.GetInvariantCultureDateTime();

                if (!vm.EncounterInpatientAdmitDate.HasValue)
                    vm.EncounterInpatientAdmitDate = Helpers.GetInvariantCultureDateTime();

                vm.EncounterPatientTypecheck = messageId == "6" ? "1" : messageId;
                return View(vm);
            }


            return RedirectToAction(ActionResults.patientSearch, ControllerNames.patientSearch, new { messageId });
        }

        /// <summary>
        /// Adds the update encounter.
        /// </summary>
        /// <param name="model">
        /// The encounter.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult AddUpdateEncounter(EncounterCustomModel model)
        {
            var currentUserId = Helpers.GetLoggedInUserId();
            var cId = Helpers.GetSysAdminCorporateID();
            var fId = Helpers.GetDefaultFacilityId();
            var eEndType = model.EncounterEndType;
            var currentDatetime = _fService.GetInvariantCultureDateTime(fId);

            if (model.EncounterID > 0)
            {
                model.ModifiedBy = currentUserId;
                model.ModifiedDate = currentDatetime;
            }
            else
            {
                if (model.EncounterStartTime.HasValue)
                {
                    var sd = model.EncounterStartTime.Value;
                    model.EncounterStartTime = new DateTime(sd.Year, sd.Month, sd.Day, currentDatetime.Hour,
                        currentDatetime.Minute, currentDatetime.Second, currentDatetime.Millisecond);
                }
                model.CreatedBy = currentUserId;
                model.CreatedDate = currentDatetime;
            }

            if (eEndType.HasValue && eEndType.Value > 0)
            {
                if (model.EncounterEndType == 5)
                {
                    model.EncounterDischargePlan = model.EncounterEndType.ToString();
                    model.EncounterDischargeLocation = model.EncounterEndTime.ToString();

                    // ---- line is missing for the encounter type 6 which means Not discharge.
                    // ------ This needs to be done here as below we have to create new encounter for the Paitnet
                    // encounter.EncounterPatientTypecheck = encounter.EncounterEndType.ToString();
                    model.EncounterEndType = null;
                    model.EncounterEndTime = null;
                }

                // Added the check as per the new task : 26a Orders open when trying to discharge
                // Added on 13 jan 2016 time : 11:54 AM
                // Added by Shashank
                if (string.IsNullOrEmpty(model.EncounterDischargePlan))
                {
                    if (_service.EncounterOpenOrders(model.EncounterID))
                        return Json("OrderError");
                }
                var encounterEndStatus = ChekEncounterEndStatusBeforeUpdate(model.EncounterID);
                switch (encounterEndStatus)
                {
                    case 1:
                        break;
                    case 99:
                        return Json("AuthError");
                    case 0:
                        break;
                }
            }


            Encounter existingEncounter;
            if (model.EncounterPatientTypecheck == "5")
            {
                existingEncounter = _service.GetEncounterByEncounterId(model.EncounterID);
                existingEncounter.CreatedBy = model.CreatedBy;
                existingEncounter.CreatedDate = model.CreatedDate;
                existingEncounter.ModifiedBy = currentUserId;
                existingEncounter.ModifiedDate = currentDatetime;
                existingEncounter.EncounterEndType = Convert.ToInt32(EncounterEndTypeEnum.Notdischarged);
                existingEncounter.CorporateID = cId;
                existingEncounter.EncounterEndTime = model.EncounterEndTime;

                _service.AddUpdateEncounter(existingEncounter);
                model.EncounterNumber = existingEncounter.EncounterNumber;
                var encId = StartEncounter(model);

                //if (patientFutureOrders.Any())
                //{
                //    var objtoreturn = new
                //    {
                //        IsFutureOpenOrders = true,
                //        encId = encId,
                //        FutureOpenOrders = patientFutureOrders
                //    };
                //    return Json(encId);
                //}
                return Json(encId);
            }

            if (model.EncounterID == 0 && model.EncounterEndTime == null)
            {
                var encId = StartEncounter(model);

                var foList = _foService.GetFutureOpenOrderByPatientId(model.PatientID);

                // Check for the Future Open orders for the patient by the physician
                if (foList.Any())
                {
                    var objtoreturn = new
                    {
                        IsFutureOpenOrders = true,
                        encId,
                        FutureOpenOrders = foList
                    };
                    return Json(objtoreturn, JsonRequestBehavior.AllowGet);
                }
                return Json(encId);
            }

            existingEncounter = _service.GetEncounterByEncounterId(model.EncounterID);
            existingEncounter.CreatedBy = model.CreatedBy;
            existingEncounter.CreatedDate = model.CreatedDate;
            existingEncounter.ModifiedBy = currentUserId;
            existingEncounter.ModifiedDate = currentDatetime;
            existingEncounter.EncounterPatientType = model.EncounterPatientType;
            existingEncounter.EncounterConfidentialityLevel = model.EncounterConfidentialityLevel;
            existingEncounter.EncounterServiceCategory = model.EncounterServiceCategory;
            existingEncounter.PatientID = model.PatientID;
            existingEncounter.EncounterTransferHospital = model.EncounterTransferHospital;
            existingEncounter.EncounterFacility = model.EncounterFacility;
            existingEncounter.EncounterTransferSource = model.EncounterTransferSource;
            existingEncounter.EncounterAdmitReason = model.EncounterAdmitReason;
            existingEncounter.EncounterType = model.EncounterType;
            existingEncounter.EncounterStartType = model.EncounterStartType;
            existingEncounter.EncounterSpecialty = model.EncounterSpecialty;
            existingEncounter.EncounterModeofArrival = model.EncounterModeofArrival;
            existingEncounter.EncounterAdmitType = model.EncounterAdmitType;
            existingEncounter.EncounterAccidentRelated = model.EncounterAccidentRelated;
            existingEncounter.EncounterAccidentType = model.EncounterAccidentType;
            existingEncounter.EncounterEndType = model.EncounterEndType;
            existingEncounter.EncounterEndTime = model.EncounterEndTime;
            existingEncounter.EncounterAttendingPhysician = model.EncounterAttendingPhysician;
            existingEncounter.EncounterPhysicianType = model.EncounterPhysicianType;
            existingEncounter.CorporateID = cId;
            existingEncounter.HomeCareRecurring = model.HomeCareRecurring;
            existingEncounter.EncounterDischargePlan = model.EncounterDischargePlan;
            existingEncounter.EncounterDischargeLocation = model.EncounterDischargeLocation;
            existingEncounter.EncounterStartTime = model.EncounterStartTime;
            _service.AddUpdateEncounter(existingEncounter);


            if (model.EncounterPatientType == Convert.ToInt32(EncounterPatientType.InPatient))
            {
                if (eEndType != 5)
                {
                    var objtoUpdate = _mpbService.GetMappingPatientBedByEncounterId(Convert.ToString(model.EncounterID));

                    // Get Mapping Patient Bed data
                    if (objtoUpdate != null)
                    {
                        // Discharge the patient
                        if (objtoUpdate.BedNumber == model.patientBedId
                            && model.patientBedEndDate != null)
                        {
                            // ....Discharge patient logic Check
                            objtoUpdate.PatientID = Convert.ToInt32(existingEncounter.PatientID);
                            objtoUpdate.EncounterID = Convert.ToString(existingEncounter.EncounterID);
                            objtoUpdate.EndDate = Helpers.GetInvariantCultureDateTime();
                            if (!string.IsNullOrEmpty(model.OverrideBedType))
                            {
                                var bedOverrideType = _bedservice.GetBedTypeByServiceCode(model.OverrideBedType);
                                objtoUpdate.OverrideBedType = bedOverrideType;
                            }
                            else
                                objtoUpdate.OverrideBedType = null;


                            // new added column on nov 1 2014
                            var isMapped = _mpbService.AddUpdateMappingPatientBed(objtoUpdate);
                            if (isMapped > 0 && model.patientBedEndDate != null)
                            {
                                var patientBedId = Convert.ToInt32(objtoUpdate.BedNumber);
                                var patientBedobj = _bedservice.GetBedMasterById(patientBedId);
                                patientBedobj.IsOccupied = false;
                                _bedservice.AddUpdateBedMaster(patientBedobj);

                            }
                        }

                        // ....Transfer Bed logic
                        else if (objtoUpdate.BedNumber != model.patientBedId
                                 && model.patientBedEndDate == null)
                        {
                            // ....Transfer Bed logic
                            objtoUpdate.PatientID = Convert.ToInt32(existingEncounter.PatientID);
                            objtoUpdate.EncounterID = Convert.ToString(existingEncounter.EncounterID);
                            objtoUpdate.EndDate = Helpers.GetInvariantCultureDateTime();

                            // Update the previous occupied BEd and Mapping Bed table
                            var isMapped = _mpbService.AddUpdateMappingPatientBed(objtoUpdate);
                            if (isMapped > 0 && model.patientBedEndDate == null)
                            {
                                var patientBedId = Convert.ToInt32(objtoUpdate.BedNumber);
                                var patientBedobj = _bedservice.GetBedMasterById(patientBedId);
                                patientBedobj.IsOccupied = false;
                                var isUpdated = _bedservice.AddUpdateBedMaster(patientBedobj);
                                if (isUpdated > 0)
                                {

                                    var newBedToMap =
                                        _bedservice.GetBedMasterById(
                                            Convert.ToInt32(model.patientBedId));
                                    var mappingPatientBedTransfer = new MappingPatientBed
                                    {
                                        MappingPatientBedId = 0,
                                        FacilityNumber =
                                            Convert.ToInt32(existingEncounter.EncounterFacility),
                                        RoomNumber = "0",
                                        BedNumber = model.patientBedId,
                                        PatientID = Convert.ToInt32(model.PatientID),
                                        EncounterID = Convert.ToString(existingEncounter.EncounterID),
                                        StartDate = Convert.ToDateTime(model.patientBedStartDate),
                                        ServiceCode = Convert.ToInt32(model.patientBedService),
                                        ExpectedEndDate =
                                            Convert.ToDateTime(model.patientBedExpectedEndDate),
                                        OverrideBedType
                                            = !string.IsNullOrEmpty(model.OverrideBedType)
                                                ? _bedservice.GetBedTypeByServiceCode
                                                    (model.OverrideBedType)
                                                : null,
                                        Corporateid = cId,
                                        FacilityStructureID = newBedToMap.FacilityStructureId,
                                        BedType = newBedToMap.BedType
                                    };
                                    var isTransferMapped = _mpbService.AddUpdateMappingPatientBed(mappingPatientBedTransfer);
                                    if (isTransferMapped > 0)
                                    {
                                        var patientBedIdTransfer =
                                            Convert.ToInt32(model.patientBedId);
                                        var patientBedTransferobj =
                                            _bedservice.GetBedMasterById(patientBedIdTransfer);
                                        patientBedTransferobj.IsOccupied = true;
                                        _bedservice.AddUpdateBedMaster(
                                                patientBedTransferobj);


                                        // Logic to calculate the Encounter bed charges when updated bed charges
                                        // via calling this SPROC [SPROC_BedChargesNightlyPerEncounter]
                                        // New changes Use this Proc Instead of previous [SPROC_ReValuateBedChargesPerEncounter]
                                        _service.AddBedChargesForTransferPatient(
                                            model.EncounterID,
                                            Helpers.GetInvariantCultureDateTime());
                                    }
                                }

                            }
                        }
                        else
                        {
                            var objEncounterdetais = _service.GetEncounterDetailById(model.EncounterID);
                            if (objEncounterdetais != null)
                            {
                                var newBedToMap = _bedservice.GetBedMasterById(Convert.ToInt32(model.patientBedId));
                                var mappingPatientBed = new MappingPatientBed
                                {
                                    MappingPatientBedId = 0,
                                    FacilityNumber = Convert.ToInt32(objEncounterdetais.EncounterFacility),
                                    RoomNumber = "0",
                                    BedNumber = model.patientBedId,
                                    PatientID = Convert.ToInt32(model.PatientID),
                                    EncounterID = Convert.ToString(objEncounterdetais.EncounterID),
                                    StartDate = Convert.ToDateTime(model.patientBedStartDate),
                                    ServiceCode = Convert.ToInt32(model.patientBedService),
                                    ExpectedEndDate = Convert.ToDateTime(model.patientBedExpectedEndDate),
                                    OverrideBedType = !string.IsNullOrEmpty(model.OverrideBedType)
                                    ? _bedservice.GetBedTypeByServiceCode(model.OverrideBedType)
                                    : null,
                                    Corporateid = cId,
                                    FacilityStructureID = newBedToMap.FacilityStructureId,
                                    BedType = newBedToMap.BedType
                                };
                                var isMapped = _mpbService.AddUpdateMappingPatientBed(mappingPatientBed);
                                if (isMapped > 0)
                                {
                                    var patientBedId = Convert.ToInt32(model.patientBedId);
                                    var patientBedobj = _bedservice.GetBedMasterById(patientBedId);
                                    patientBedobj.IsOccupied = true;
                                    _bedservice.AddUpdateBedMaster(patientBedobj);
                                }
                            }
                        }
                    }
                }
            }

            // return Json(encounter.EncounterID);
            if (model.EncounterEndType != null && model.EncounterEndType != 0)
            {
                var encounterEndStatus = CheckEncounterEndStatus(model.EncounterID);
                switch (encounterEndStatus)
                {
                    case 1:
                        return Json("Success");
                    case 99:
                        return Json("AuthError");
                    case 0:
                        break;
                }
            }

            // --- Check is added to scrub the bill manually if the patient is virtually discharged
            // -- Who :Shashank Awasthy
            // --  Why : To scrub the Virtual discharge patient.
            // -- need to be changed after the confirmation from the client regarding this flow.
            if (!string.IsNullOrEmpty(model.EncounterDischargePlan))
            {
                var addVitruallDischargeLog = AddVirtualDischargeLog(model.EncounterID, fId, cId);
                //var encounterEndStatus = CheckEncounterEndStatusVirtualDischarge(model.EncounterID);
                var encounterEndStatus = CheckEncounterEndStatusVirtualDischarge(model.EncounterID, Convert.ToInt32(currentUserId));

                switch (encounterEndStatus)
                {
                    case 1:
                        return Json("Success");
                    case 99:
                        return Json("AuthError");
                    case 0:
                        break;
                }
            }

            return Json(model.EncounterID);

        }

        /// <summary>
        /// Filters the bed grid view.
        /// </summary>
        /// <param name="floorid">
        /// The floorid.
        /// </param>
        /// <param name="departmentid">
        /// The departmentid.
        /// </param>
        /// <param name="roomid">
        /// The roomid.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult FilterBedGridView(int floorid, int departmentid, int roomid)
        {
            /*
             * Owner: Amit Jain
             * On: 20102014
             * Purpose: Get the default faclity ID set by loggedin user.
             */
            var facilityId = Helpers.GetDefaultFacilityId();
            var corporateId = Helpers.GetSysAdminCorporateID();

            // Need to change later and get the list by type. && _.IsOccupied == false
            // var availableBedList = bal.GetAvialableBedMasterList(facilityId);
            var availableBedList = _bedservice.GetBedStrutureForFacility(facilityId, corporateId, Helpers.DefaultServiceCodeTableNumber);
            availableBedList = floorid > 0
                                   ? availableBedList.Where(x => x.FloorID == floorid).ToList()
                                   : availableBedList;
            availableBedList = departmentid > 0
                                   ? availableBedList.Where(x => x.DepartmentID == departmentid).ToList()
                                   : availableBedList;
            availableBedList = roomid > 0 ? availableBedList.Where(x => x.RoomID == roomid).ToList() : availableBedList;
            if (availableBedList != null)
            {
                availableBedList.ForEach(
                   rate =>
                   rate.MedRateCardlist = _brcservice.GetBedRateCardsListByBedType(Convert.ToString(rate.BedType), rate.RoomNonChargeAble));
            }

            return PartialView(PartialViews.BedSearchFilterGrid, availableBedList);
        }

        /// <summary>
        /// Gets the facility structure ddl data.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult GetFacilityStructureDDlData()
        {
            var facilityId = Helpers.GetDefaultFacilityId();
            var facilitystrutreList = _fsservice.GetFacilityStructureForDDL(Convert.ToString(facilityId));
            var facilityFloors =
                facilitystrutreList.Where(x => x.GlobalCodeID == Convert.ToInt32(BaseFacilityStucture.Floor))
                    .OrderBy(x => x.SortOrder)
                    .ToList();
            var facilityDepartment =
                facilitystrutreList.Where(x => x.GlobalCodeID == Convert.ToInt32(BaseFacilityStucture.Department))
                    .OrderBy(x => x.SortOrder)
                    .ToList();
            var facilityRooms =
                facilitystrutreList.Where(x => x.GlobalCodeID == Convert.ToInt32(BaseFacilityStucture.Rooms))
                    .OrderBy(x => x.SortOrder)
                    .ToList();
            var jsonResult = new { flList = facilityFloors, dtList = facilityDepartment, rmList = facilityRooms };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the floor departments.
        /// </summary>
        /// <param name="floorid">
        /// The floorid.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult GetFloorDepartments(int floorid)
        {
            var facilityId = Helpers.GetDefaultFacilityId();

            // var facilitystrutreList = facilitStructureBalyBal.GetFacilityStructure(Convert.ToString(facilityId));
            var facilitystrutreList = _fsservice.GetFacilityStructureForDDL(Convert.ToString(facilityId));
            var facilityDepartment = facilitystrutreList.Where(x => x.GlobalCodeID == Convert.ToInt32(BaseFacilityStucture.Department) && x.ParentId == floorid).OrderBy(x => x.SortOrder).ToList();
            var jsonResult = new { dtList = facilityDepartment, };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the floor departments rooms.
        /// </summary>
        /// <param name="departmentid">
        /// The departmentid.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult GetFloorDepartmentsRooms(int departmentid)
        {
            var facilityId = Helpers.GetDefaultFacilityId();

            // var facilitystrutreList = facilitStructureBalyBal.GetFacilityStructure(Convert.ToString(facilityId));
            var facilitystrutreList = _fsservice.GetFacilityStructureForDDL(Convert.ToString(facilityId));
            var facilityDepartment =
                facilitystrutreList.Where(
                    x => x.GlobalCodeID == Convert.ToInt32(BaseFacilityStucture.Rooms) && x.ParentId == departmentid)
                    .OrderBy(x => x.SortOrder)
                    .ToList();
            var jsonResult = new { rmList = facilityDepartment, };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the patient bed information.
        /// </summary>
        /// <param name="patientId">
        /// The patient identifier.
        /// </param>
        /// <param name="bedId">
        /// The bed identifier.
        /// </param>
        /// <param name="serviceCodeValue">
        /// The service code value.
        /// </param>
        /// <returns>
        /// The <see cref="JsonResult"/>.
        /// </returns>
        public JsonResult GetPatientBedInformation(int patientId, int bedId, string serviceCodeValue)
        {
            if (bedId > 0)
            {
                var result = _service.GetPatientBedInformationByBedId(
                    patientId,
                    bedId,
                    serviceCodeValue);
                var jsonResult =
                    new
                    {
                        result.FloorName,
                        result.DepartmentName,
                        result.Room,
                        result.BedName,
                        result.BedAssignedOn,
                        result.BedRateApplicable,
                        result.patientBedService
                    };
                return Json(jsonResult, JsonRequestBehavior.AllowGet);
            }

            return Json(null);

        }

        /// <summary>
        /// Gets the type of the physicians by physician.
        /// </summary>
        /// <param name="physicianTypeId">
        /// The physician type identifier.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult GetPhysiciansByPhysicianType(string physicianTypeId)
        {
            var facilityid = Helpers.GetDefaultFacilityId();

            // Need to change later and get the list by type.
            var physicianList = _phService.GetPhysiciansByPhysicianTypeId(Convert.ToInt32(physicianTypeId), facilityid);
            return Json(physicianList);
        }

        /// <summary>
        /// Selects the bed grid.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult SelectBedGrid()
        {
            /*
             * Owner: Amit Jain
             * On: 20102014
             * Purpose: Get the default faclity ID set by loggedin user.
             */
            var facilityId = Helpers.GetDefaultFacilityId();
            var corporateId = Helpers.GetSysAdminCorporateID();

            var availableBedList = _bedservice.GetBedStrutureForFacility(facilityId, corporateId, Helpers.DefaultServiceCodeTableNumber);
            var bedAssignementView = new BedAssignmentView
            {
                AvailableBedList = new List<FacilityBedStructureCustomModel>(),
                OccupiedBedList = new List<FacilityBedStructureCustomModel>()
            };
            if (availableBedList != null && availableBedList.Any())
            {
                bedAssignementView.AvailableBedList = availableBedList.Where(_ => _.IsOccupied == false).ToList();
                bedAssignementView.OccupiedBedList = availableBedList.Where(_ => _.IsOccupied).ToList();
            }

            // GetBedRateCardsListByBedType
            if (bedAssignementView.AvailableBedList != null)
            {
                bedAssignementView.AvailableBedList.ForEach(
                   rate =>
                   rate.MedRateCardlist = _brcservice.GetBedRateCardsListByBedType(Convert.ToString(rate.BedType), rate.RoomNonChargeAble));
            }

            return PartialView(PartialViews.BedMappingPatientBedAssignment, bedAssignementView);
        }

        /// <summary>
        /// Encounters the open orders pending.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        public ActionResult EncounterOpenOrdersPending(int encounterId)
        {
            return Json(_service.EncounterOpenOrders(encounterId));

        }

        /// <summary>
        /// Patients the encounter order pending.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public ActionResult PatientEncounterOrderPending(int patientId)
        {
            return Json(_service.PatientEncounterOpenOrders(patientId));

        }

        /// <summary>
        /// Patients the encounter order pending pinfo.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public ActionResult PatientEncounterOrderPendingPinfo(int patientId)
        {
            //return Json(bal.PatientEncounterOpenOrders(patientId));
            var pendingorders = _service.PatientEncounterOpenOrders(patientId);
            var currentEncounter = _service.GetActiveEncounterByPateintId(patientId);
            if (currentEncounter != null)
            {
                return Json(new { encID = currentEncounter.EncounterID, pendingorders, encType = currentEncounter.EncounterPatientType });
            }

            return Json(pendingorders);

        }

        /// <summary>
        /// Gets the patient future order.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public ActionResult GetPatientFutureOrder(int patientId)
        {
            var futureOrderList = _foService.GetFutureOpenOrderByPatientId(patientId);
            return PartialView(PartialViews.FutureOpenOrders, futureOrderList);
        }

        /// <summary>
        /// Adds the future orders.
        /// </summary>
        /// <param name="orderIds">The order ids.</param>
        /// <param name="encId">The enc identifier.</param>
        /// <returns></returns>
        public ActionResult AddFutureOrders(string[] orderIds, int encId)
        {
            var cid = Helpers.GetSysAdminCorporateID();
            var fid = Helpers.GetDefaultFacilityId();
            var orderadded = _foService.AddCurrentEncToFutureOrders(orderIds, encId, cid, fid);
            return Json(orderadded);
        }


        #endregion

        #region Methods

        /// <summary>
        /// Checks the encounter end status.
        /// </summary>
        /// <param name="encounterid">
        /// The encounterid.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private int CheckEncounterEndStatus(int encounterid)
        {
            var userId = Helpers.GetLoggedInUserId();

            var encounterEndStatus = _service.GetEncounterEndCheck(encounterid, userId);
            return encounterEndStatus;

        }


        /// <summary>
        /// Checks the encounter end status virtual discharge.
        /// </summary>
        /// <param name="encounterid">The encounterid.</param>
        /// <param name="loggedInUserId">The logged in user identifier.</param>
        /// <returns></returns>
        private int CheckEncounterEndStatusVirtualDischarge(int encounterid, int loggedInUserId)
        {
            var encounterEndStatus = _service.GetEncounterEndCheckVirtualDischarge(encounterid, loggedInUserId);
            return encounterEndStatus;

        }

        /// <summary>
        /// Cheks the encounter end status before update.
        /// </summary>
        /// <param name="encounterid">The encounterid.</param>
        /// <returns></returns>
        private int ChekEncounterEndStatusBeforeUpdate(int encounterid)
        {
            var encounterEndStatus = _service.GetEncounterStatusBeforeUpdate(encounterid);
            return encounterEndStatus;

        }

        /// <summary>
        /// Gets the heading.
        /// </summary>
        /// <param name="messageId">
        /// The message identifier.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetHeading(int messageId)
        {
            var message = string.Empty;
            switch (messageId)
            {
                case 1:
                    message = "Admit Patient";
                    break;
                case 2:
                    message = "Start OutPatient Encounter";
                    break;
                case 3:
                    message = "Discharge Patient";
                    break;
                case 4:
                    message = "End Encounter";
                    break;
                case 5:
                    message = "Admit Patient";
                    break;
                case 6:
                    message = "Edit Active Encounter";
                    break;
                case 9:
                    message = "Virtually Discharge Patient";
                    break;
            }

            return message;
        }

        /// <summary>
        /// Starts the encounter.
        /// </summary>
        /// <param name="model">
        /// The encounter.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private int StartEncounter(EncounterCustomModel model)
        {
            var coporateid = Helpers.GetSysAdminCorporateID();
            var facilityId = Helpers.GetDefaultFacilityId();
            var datetimeLocal = Helpers.GetInvariantCultureDateTime();

            var rn = new Random(Helpers.GetInvariantCultureDateTime().Ticks.GetHashCode()); // 2
            var uniqueId = rn.Next(100, int.MaxValue);
            while (_service.CheckEncounterNumberExist(Convert.ToString(uniqueId)))
                uniqueId = rn.Next(100, int.MaxValue);


            var encounterNumber = !string.IsNullOrEmpty(model.EncounterNumber)
                                         ? model.EncounterNumber
                                         : string.Format(
                                             "{0}{1}{2}{3}",
                                             model.EncounterFacility,
                                             model.PatientID,
                                             model.EncounterPatientType,
                                             uniqueId);

            var encounterid =
                _service.AddUpdateEncounter(
                    new Encounter
                    {
                        EncounterPatientType = model.EncounterPatientType,
                        EncounterConfidentialityLevel = model.EncounterConfidentialityLevel,
                        EncounterServiceCategory = model.EncounterServiceCategory,
                        PatientID = model.PatientID,
                        EncounterTransferHospital = model.EncounterTransferHospital,
                        EncounterFacility = model.EncounterFacility,
                        EncounterTransferSource = model.EncounterTransferSource,
                        EncounterAdmitReason = model.EncounterAdmitReason,
                        EncounterType = model.EncounterType,
                        EncounterStartType = model.EncounterStartType,
                        EncounterSpecialty = model.EncounterSpecialty,
                        EncounterModeofArrival = model.EncounterModeofArrival,
                        EncounterAdmitType = model.EncounterAdmitType,
                        EncounterAccidentRelated = model.EncounterAccidentRelated,
                        EncounterAccidentType = model.EncounterAccidentType,
                        EncounterEndType = model.EncounterEndType,
                        EncounterAttendingPhysician = model.EncounterAttendingPhysician,
                        EncounterPhysicianType = model.EncounterPhysicianType,
                        EncounterNumber = encounterNumber,
                        EncounterStartTime = Convert.ToDateTime(model.EncounterStartTime),
                        EncounterInpatientAdmitDate = datetimeLocal,
                        EncounterAccidentDate = datetimeLocal,
                        EncounterRegistrationDate = datetimeLocal,
                        EncounterEndTime = null,
                        CorporateID = coporateid,
                        HomeCareRecurring = model.HomeCareRecurring,
                        CreatedBy = model.CreatedBy,
                        CreatedDate = model.CreatedDate,
                        EncounterID = model.EncounterID,
                        ModifiedBy = model.ModifiedBy,
                        ModifiedDate = model.ModifiedDate,
                    });

            if (model.EncounterPatientType == Convert.ToInt32(EncounterPatientType.InPatient))
            {
                var objEncounterdetais = _service.GetEncounterDetailById(encounterid);
                if (objEncounterdetais != null)
                {
                    var newBedToMap = _bedservice.GetBedMasterById(Convert.ToInt32(model.patientBedId));
                    var mappingPatientBed = new MappingPatientBed
                    {
                        MappingPatientBedId = 0,
                        FacilityNumber = Convert.ToInt32(objEncounterdetais.EncounterFacility),
                        RoomNumber = "0",
                        BedNumber = model.patientBedId,
                        PatientID = Convert.ToInt32(model.PatientID),
                        EncounterID = Convert.ToString(objEncounterdetais.EncounterID),
                        StartDate = Convert.ToDateTime(model.EncounterStartTime),
                        ServiceCode = Convert.ToInt32(model.patientBedService),
                        ExpectedEndDate = Convert.ToDateTime(model.patientBedExpectedEndDate),
                        OverrideBedType = !string.IsNullOrEmpty(model.OverrideBedType) ? _bedservice.GetBedTypeByServiceCode(model.OverrideBedType) : null,
                        Corporateid = coporateid,
                        FacilityStructureID = newBedToMap.FacilityStructureId,
                        BedType = newBedToMap.BedType
                    };
                    var isMapped = _mpbService.AddUpdateMappingPatientBed(mappingPatientBed);
                    if (isMapped > 0)
                    {
                        var patientBedId = Convert.ToInt32(model.patientBedId);
                        var patientBedobj = _bedservice.GetBedMasterById(patientBedId);
                        patientBedobj.IsOccupied = true;
                        _bedservice.AddUpdateBedMaster(patientBedobj);
                    }
                }
            }

            _bhservice.AddBillonEncounterStart(encounterid, 0, Convert.ToInt32(model.PatientID), coporateid, facilityId);


            return encounterid;

        }

        /// <summary>
        /// Adds the virtual discharge log.
        /// </summary>
        /// <param name="encounterid">The encounterid.</param>
        /// <returns></returns>
        private bool AddVirtualDischargeLog(int encounterid, int facilityId, int corporateId)
        {
            var encounterEndStatus = _service.AddVirtualDischargeLog(encounterid, facilityId, corporateId);
            return encounterEndStatus;

        }

        //public ActionResult GetPreEvalutionData(int PatientId)
        //{

        //}


        public JsonResult BindLicenseTypes(string roleName)
        {
            var value = new List<string> { roleName };
            var ltList = new List<DropdownListData>();

            ltList = _gService.GetGCodesListByCategoryValue("1104", value, string.Empty);

            return Json(ltList, JsonRequestBehavior.AllowGet);
        }


        public ActionResult BindAllEncountersDataOnLoad()
        {
            var fId = Helpers.GetDefaultFacilityId();
            var cId = Helpers.GetSysAdminCorporateID();
            List<DropdownListData> allList;

            var uList = new List<PhysicianViewModel>();
            var ltList = new List<DropdownListData>();


            allList = _gService.GetListByCategoriesRange(new List<string> { "1104", "1121" });
            uList = _uService.GetPhysiciansByRole(fId, cId);

            var jsonData = new
            {
                sPList = allList.Where(g => g.ExternalValue1.Equals("1121")).ToList(),
                licenceList = allList.Where(g => g.ExternalValue2.Equals("Physicians") && g.ExternalValue1.Equals("1104")).ToList(),
                uList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);

        }
        #endregion
    }
}
