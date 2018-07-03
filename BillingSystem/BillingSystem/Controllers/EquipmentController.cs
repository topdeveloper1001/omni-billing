using System.Linq;
using BillingSystem.Models;
using System.Web.Mvc;
using BillingSystem.Common;
using BillingSystem.Model;
using System;
using BillingSystem.Model.CustomModel;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Controllers
{
    public class EquipmentController : BaseController
    {
        private readonly IAppointmentTypesService _atService;
        private readonly IEquipmentService _service;
        private readonly IFacilityService _fService;
        private readonly IFacilityStructureService _fsService;

        public EquipmentController(IAppointmentTypesService atService, IEquipmentService service, IFacilityService fService, IFacilityStructureService fsService)
        {
            _atService = atService;
            _service = service;
            _fService = fService;
            _fsService = fsService;
        }


        //
        // GET: /Equipment/
        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var facilityid = Helpers.GetDefaultFacilityId();

            var facilitynumber = _fService.GetFacilityNumberById(facilityid);
            //Get the facilities list
            //var equipmentList = _service.GetEquipmentList().ToList();
            var equipmentList = _service.GetEquipmentList(false, Convert.ToString(facilityid)).ToList();
            //.Where(x => x.FacilityId == facilitynumber).ToList();

            //Intialize the View Model i.e. FacilityView which is binded to Main View Index.cshtml under Facility
            var equipmentView = new EquipmentView
            {
                EquipmentList = equipmentList,
                CurrentEquipment = new EquipmentMaster()
            };

            //Pass the View Model in ActionResult to View Facility
            return View(equipmentView);
        }


        /// <summary>
        /// Get the details of the current facility in the view model by ID
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public ActionResult GetEquipment(CommonModel model)
        {
            var currentequipment = _service.GetEquipmentByMasterId(Convert.ToInt32(model.Id));
            //If the view is shown in ViewMode only, then ViewBag.ViewOnly is set to true otherwise false.
            ViewBag.ViewOnly = !string.IsNullOrEmpty(model.ViewOnly);

            //Pass the ActionResult with the current FacilityViewModel object as model to PartialView FacilityAddEdit
            return PartialView(PartialViews.EquipmentAddEdit, currentequipment);
        }
        /// <summary>
        /// Bind all the equipment list
        /// </summary>
        /// <returns>
        /// action result with the partial view containing the facility list object
        /// </returns>
        [HttpPost]
        public ActionResult BindEquipmentList(string facilityId)
        {
            var equipmentList = _service.GetEquipmentList(false, facilityId);

            //Pass the ActionResult with List of FacilityViewModel object to Partial View FacilityList
            return PartialView(PartialViews.EquipmentList, equipmentList);

        }

        public ActionResult BindDisabledRecords(bool showIsDisabled, string facilityId)
        {
            var list = _service.GetEquipmentList(showIsDisabled, facilityId);
            return PartialView(PartialViews.EquipmentList, list);
        }
        /// <summary>
        /// Add New or Update the equipment based on if we pass the Facility ID in the FacilityViewModel object.
        /// </summary>
        /// <param name="equipment">The equipment.</param>
        /// <returns>
        /// returns the newly added or updated ID of facility row
        /// </returns>
        /// []
        [HttpPost]
        public ActionResult SaveEquipment(EquipmentMaster equipment)
        {
            //Initialize the newId variable 
            var newId = -1;
            if (equipment != null)
            {
                var userId = Helpers.GetLoggedInUserId();
                var currentDateTime = Helpers.GetInvariantCultureDateTime();
                equipment.IsDeleted = false;
                if (equipment.EquipmentMasterId > 0)
                {
                    equipment.ModifiedBy = userId;
                    equipment.ModifiedDate = currentDateTime;
                }
                else
                {
                    equipment.CreatedBy = userId;
                    equipment.CreatedDate = currentDateTime;
                }

                // Add / Update current Equipment
                newId = _service.AddUpdateEquipment(equipment);
            }

            return Json(newId);
        }

        /// <summary>
        /// Delete the current equipment based on the Facility ID passed in the SharedViewModel
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public ActionResult DeleteEquipment(CommonModel model)
        {
            //Get facility model object by current equipment ID
            var currentequipment = _service.GetEquipmentByMasterId(Convert.ToInt32(model.Id));

            //var structureId = _service.GetFacilityStructureIdByEquipmentMasterId(Convert.ToInt32(model.Id));
            var equipmentId = _service.CheckEquipmentInScheduling(Convert.ToInt32(model.Id));
            if (equipmentId)
            {
                return Json(1);
            }
            if (currentequipment != null)
            {
                currentequipment.EquipmentModel = currentequipment.EquipmentModel;
                currentequipment.IsDeleted = true;
                currentequipment.DeletedBy = Helpers.GetLoggedInUserId();
                currentequipment.DeletedDate = Helpers.GetInvariantCultureDateTime();
                //Update Operation of current equipment
                _service.AddUpdateEquipment(currentequipment);

                //Get the facilities list
                var equipmentList = model.ShowDisabled
                    ? _service.GetEquipmentList(true, Convert.ToString(currentequipment.FacilityId))
                    //: _service.GetEquipmentList();
                    : _service.GetEquipmentList(false, Convert.ToString(currentequipment.FacilityId));
                //Pass the ActionResult with List of FacilityViewModel object to Partial View FacilityList
                return PartialView(PartialViews.EquipmentList, equipmentList);
            }

            return Json(null);
        }

        /// <summary>
        /// Reset the Facility View Model and pass it to equipmentAddEdit Partial View.
        /// </summary>
        /// <returns></returns>
        public ActionResult ResetEquipmentForm()
        {
            //Intialize the new object of equipment 
            var equipmentMaster = new EquipmentMaster();

            //Pass the View Model as equipmentViewModel to PartialView equipmentAddEdit just to update the AddEdit partial view.
            return PartialView(PartialViews.EquipmentAddEdit, equipmentMaster);
        }

        public ActionResult GetEquipmentsData(int id)
        {
            var currentequipment = _service.GetEquipmentByMasterId(id);
            var jsonResult = new
            {
                currentequipment.FacilityId,
                currentequipment.IsEquipmentFixed,
                currentequipment.EquipmentType,
                EquipmentAquistionDate = currentequipment.EquipmentAquistionDate.GetShortDateString3(),
                currentequipment.EquipmentDisabled,
                EquipmentDisabledDate = currentequipment.EquipmentDisabledDate.GetShortDateString3(),
                currentequipment.EquipmentIsInsured,
                currentequipment.EquipmentMasterId,
                currentequipment.EquipmentModel,
                currentequipment.EquipmentName,
                currentequipment.EquipmentSerialNumber,
                currentequipment.TurnAroundTime,
                currentequipment.BaseLocation,
                currentequipment.CorporateId,
                currentequipment.FacilityStructureId
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }





        public ActionResult BindAppointmentTypesDropdown(int corporateId, int facilityId)
        {
            // Get the facilities list
            //var appointmentTypesList = appointmentTypesBal.GetAppointmentTypes();
            var appointmentTypesList = _atService.GetAppointmentTypesData(corporateId, facilityId, true);

            return Json(appointmentTypesList, JsonRequestBehavior.AllowGet);

        }


        public ActionResult BindRoomsInEquipments(string facilityId)
        {
            var list = _fsService.GetRoomsByFacilityId(facilityId);
            return Json(list);
        }

        public ActionResult BindDeletedRecords(bool showIsDeleted, string facilityId)
        {
            var list = _service.GetDeletedEquipmentList(showIsDeleted, facilityId);
            return PartialView(PartialViews.EquipmentList, list);
        }



        public ActionResult DeactivateActivateEquipment(EquipmentCustomModel model)
        {
            int eid = -1;

            var list = _service.GetEquipmentDataByMasterId(model.EquipmentMasterId);
            foreach (var eq in list)
            {
                model.EquipmentName = eq.EquipmentName;
                model.FacilityId = eq.FacilityId;
                model.EquipmentType = eq.EquipmentType;
                model.EquipmentModel = eq.EquipmentModel;
                model.EquipmentSerialNumber = eq.EquipmentSerialNumber;
                model.EquipmentIsInsured = eq.EquipmentIsInsured;
                model.EquipmentAquistionDate = eq.EquipmentAquistionDate;
                model.CorporateId = eq.CorporateId;
                model.ModifiedBy = Helpers.GetLoggedInUserId();
                model.ModifiedDate = Helpers.GetInvariantCultureDateTime();
                model.IsDeleted = eq.IsDeleted;
                model.IsEquipmentFixed = eq.IsEquipmentFixed;
                model.TurnAroundTime = eq.TurnAroundTime;
                model.BaseLocation = eq.BaseLocation;
                model.FacilityStructureId = eq.FacilityStructureId;
                if (model.ActiveDeactive)
                {
                    model.EquipmentDisabled = false;
                }
                else
                {
                    model.EquipmentDisabled = true;
                }

            }
            eid = _service.UpdateEuipmentCustomModel(model);



            return Json(eid);
        }
    }
}