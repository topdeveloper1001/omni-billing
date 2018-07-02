using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Models;

namespace BillingSystem.Controllers
{


    /// <summary>
    /// The pre scheduling queue controller.
    /// </summary>
    public class PreSchedulingQueueController : BaseController
    {
        private readonly IPatientInfoService _piService;
        private readonly IAppointmentTypesService _atService;
        private readonly ISchedulingService _schService;
        private readonly IUsersService _uService;
        private readonly IPatientPhoneService _ppService;
        private readonly IPatientLoginDetailService _pldService;

        public PreSchedulingQueueController(IPatientInfoService piService, IAppointmentTypesService atService, ISchedulingService schService, IUsersService uService, IPatientPhoneService ppService, IPatientLoginDetailService pldService)
        {
            _piService = piService;
            _atService = atService;
            _schService = schService;
            _uService = uService;
            _ppService = ppService;
            _pldService = pldService;
        }


        // GET: /PreSchedulingQueue/
        #region Public Methods and Operators

        /// <summary>
        /// The index.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult Index()
        {
            var cId = Helpers.GetSysAdminCorporateID();
            var fId = Helpers.GetDefaultFacilityId();
            var preschedulingList = _schService.GetPreSchedulingList(cId, fId);
            var viewToReturn = new PreSchedulingQueueView() { PreSchedulingList = preschedulingList };
            return View(viewToReturn);
        }

        /// <summary>
        /// Gets the pre scheduling data by identifier.
        /// </summary>
        /// <param name="schId">The SCH identifier.</param>
        /// <returns></returns>
        public ActionResult GetPreSchedulingDataById(int schId)
        {
            return null;
        }


        public JsonResult SavePatientScheduling(List<SchedulingCustomModel> model)
        {
            var notAvialableTimeslotslist = new List<NotAvialableTimeSlots>();
            var patientid = 0;
            var roomEquipmentnotaviable = new List<NotAvialableRoomEquipment>();
            if (model != null)
            {
                var corporateId = Helpers.GetSysAdminCorporateID();

                var token = CommonConfig.GenerateLoginCode(8, false);
                // var phName = string.Empty;
                // var physician = new PhysicianBal().GetPhysicianById(Convert.ToInt32(model[0].PhysicianId));
                // phName = physician != null ? physician.PhysicianName : string.Empty;

                // check to add the physician department
                var physicianId = model[0].PhysicianId;
                var physicianDeptid = string.Empty;
                if (!string.IsNullOrEmpty(model[0].PhysicianId))
                {
                    var physicianBal = _uService.GetPhysicianById(Convert.ToInt32(model[0].PhysicianId));
                    physicianDeptid = physicianBal != null ? physicianBal.FacultyDepartment : string.Empty;
                }
                else
                {
                    var physicianBal = _uService.GetPhysicianById(Convert.ToInt32(model[0].AssociatedId));
                    physicianDeptid = physicianBal != null ? physicianBal.FacultyDepartment : string.Empty;
                }

                var patientinfo = model[0].AssociatedId != 0 ? _piService.GetPatientInfoById(model[0].AssociatedId) :
                        new PatientInfo
                        {
                            CorporateId = corporateId,
                            CreatedBy = Helpers.GetLoggedInUserId(),
                            CreatedDate = Helpers.GetInvariantCultureDateTime(),
                            FacilityId = model[0].FacilityId,
                            PatientID = model[0].AssociatedId != 0 ? Convert.ToInt32(model[0].AssociatedId) : Convert.ToInt32(patientid),
                            PersonFirstName = model[0].PatientName.Split(' ')[0],
                            PersonLastName = model[0].PatientName.Replace(model[0].PatientName.Split(' ')[0], string.Empty).TrimStart(),
                            IsDeleted = false,
                        };
                patientinfo.PersonBirthDate = model[0].PatientDOB;
                patientinfo.PersonEmailAddress = model[0].PatientEmailId;
                patientinfo.PersonEmiratesIDNumber = model[0].PatientEmirateIdNumber;

                patientid = _piService.AddUpdatePatientInfo(patientinfo);

                // Add the patient phone number
                if (patientid > 0)
                {
                    var patientPhone = _ppService.GetPatientPersonalPhoneByPateintId(patientid);
                    var patientPhonenumber = patientPhone ?? new PatientPhone
                    {
                        CreatedBy = Helpers.GetLoggedInUserId(),
                        CreatedDate =
                            Helpers.GetInvariantCultureDateTime(),
                        PatientID = patientid,
                        IsPrimary = true,
                        PhoneType = 2,
                        IsDeleted = false
                    };
                    patientPhonenumber.PhoneNo = model[0].PatientPhoneNumber;
                    _ppService.SavePatientPhone(patientPhonenumber);
                }

                // }
                _pldService.UpdatePatientEmailId(
                    model[0].AssociatedId != 0 ? Convert.ToInt32(model[0].AssociatedId) : Convert.ToInt32(patientid),
                    model[0].PatientEmailId);

                // ..... Remove the Old Data for the user in case of Edit
                //_schService.DeleteHolidayPlannerData(model[0].EventParentId, 0);
                model[0].PatientId = model[0].PatientId != null && model[0].PatientId != 0
                                         ? model[0].PatientId
                                         : patientid;
                for (int index = 0; index < model.Count; index++)
                {
                    var randomnumber = Helpers.GenerateCustomRandomNumber();
                    var item = model[index];
                    item.AssociatedId = item.AssociatedId != 0 ? item.AssociatedId : patientid;
                    item.CorporateId = corporateId;
                    item.ExtValue1 = physicianDeptid;
                    item.SchedulingId = item.SchedulingId != 0 ? item.SchedulingId : 0;
                    item.CreatedBy = Helpers.GetLoggedInUserId();
                    item.CreatedDate = Helpers.GetInvariantCultureDateTime();
                    item.EventId = Helpers.GenerateCustomRandomNumber();
                    item.ExtValue4 = token;
                    var appointmentType = string.Empty;
                    var app = _atService.GetAppointmentTypesById(Convert.ToInt32(item.TypeOfProcedure));
                    appointmentType = app != null ? app.Name : string.Empty;
                    item.AppointmentType = appointmentType;

                    // item.PhysicianName = phName;
                    item.WeekDay = Helpers.GetWeekOfYearISO8601(Convert.ToDateTime(item.ScheduleFrom)).ToString();
                    item.IsActive = true;

                    item.EventParentId = item.IsRecurring == false || string.IsNullOrEmpty(item.EventParentId)
                                             ? randomnumber
                                             : item.EventParentId;

                    item.RecEventPId = item.IsRecurring == false || string.IsNullOrEmpty(item.EventParentId)
                                            ? Convert.ToInt32(randomnumber)
                                            : Convert.ToInt32(item.EventParentId);


                    // This method call will tell that the room and equipment is avialable for the type of procedure and for the selected date
                    var isRoomalloted = _schService.GetAssignedRoomForProcedure(
                        Convert.ToInt32(item.FacilityId),
                        Convert.ToInt32(item.TypeOfProcedure),
                        Convert.ToDateTime(item.ScheduleFrom),
                        Convert.ToDateTime(item.ScheduleFrom).ToString("HH:mm"),
                        Convert.ToDateTime(item.ScheduleTo).ToString("HH:mm"), Convert.ToInt32(item.RoomAssigned));

                    if (isRoomalloted != null)
                    {
                        if (isRoomalloted.RoomId != 0)
                        {
                            item.RoomAssigned = isRoomalloted.RoomId;
                        }
                        else
                        {
                            roomEquipmentnotaviable.Add(new NotAvialableRoomEquipment
                            {
                                EquipmentId = 0,
                                EquipmentNameId = string.Empty,
                                TypeOfProcedureStr = appointmentType,
                                RoomName = string.Empty,
                                RoomId = isRoomalloted.RoomId,
                                Reason = "Room is not available in the selected timeslot for procedure " + appointmentType
                            });
                            var listobjtoReturn = new
                            {
                                IsError = true,
                                patientid = model[0].PatientId,
                                notAvialableTimeslotslist,
                                roomEquipmentnotaviable
                            };
                            return Json(listobjtoReturn, JsonRequestBehavior.AllowGet);
                        }

                        /*
                         * Updated BY: Amit Jain
                         * On: 30 Jan, 2016
                         * Here, In case Equipment Required is False, that means System doesn't need an appointment 
                         * for that specific Appointment Type or Procedure Type. 
                         * Otherwise, Check If Equipment ID has some value. Now, After checking, it still shows 0, 
                         * that means it doesn't have any Equipment available. And in that case, it shouldn't allow 
                         * the User to save the Appointment and so, show proper message to the User.
                         */
                        //Check should be bypassed if Appointment types are with the tag 'No Equipment Required'
                        var equipmentRequired = app != null && (!string.IsNullOrEmpty(app.ExtValue1) &&
                                                                  int.Parse(app.ExtValue1) == 1);

                        //Below changes done by Amit Jain on 30 Jan, 2016
                        //if (isRoomalloted.EquipmentId == 0 || !equipmentRequired)
                        //{
                        //    item.EquipmentAssigned = equipmentRequired ? 0 : isRoomalloted.EquipmentId;
                        //}
                        if (isRoomalloted.EquipmentId == 0 && !equipmentRequired)
                            item.EquipmentAssigned = 0;
                        else
                        {
                            roomEquipmentnotaviable.Add(new NotAvialableRoomEquipment
                            {
                                EquipmentId = isRoomalloted.EquipmentId,
                                EquipmentNameId = string.Empty,
                                TypeOfProcedureStr = appointmentType,
                                RoomName = string.Empty,
                                RoomId = 0,
                                Reason = "Equipment Is not available in the selected timeslot for procedure " + appointmentType
                            });
                            var listobjtoReturn = new
                            {
                                IsError = true,
                                patientid = model[0].PatientId,
                                notAvialableTimeslotslist,
                                roomEquipmentnotaviable
                            };
                            return Json(listobjtoReturn, JsonRequestBehavior.AllowGet);
                        }
                    }
                }

                foreach (var item in model.Where(item => !string.IsNullOrEmpty(item.RemovedAppointmentTypes)))
                {
                    _schService.DeleteHolidayPlannerData(
                        string.Empty,
                        Convert.ToInt32(item.RemovedAppointmentTypes),
                        Convert.ToInt32(item.SchedulingType),
                        string.Empty);
                }

                _schService.SavePatientScheduling(model);

                if (model.Count > 0 && model[0].Status == "1") //In Initial Booking mail will be sent to Patient
                {
                    Helpers.SendAppointmentNotification(
                        model,
                        model[0].PatientEmailId,
                        Convert.ToString(model[0].EmailTemplateId),
                        Convert.ToInt32(model[0].PatientId),
                        Convert.ToInt32(model[0].PhysicianId),
                        2);
                }

                if (model.Count > 0 && model[0].Status == "3") // After Confirmation of the Appointmnet by Physician( from physician Login Screen) mail will be sent to patient.
                {
                    Helpers.SendAppointmentNotification(
                        model,
                        model[0].PatientEmailId,
                        Convert.ToString(model[0].EmailTemplateId),
                        Convert.ToInt32(model[0].PatientId),
                        Convert.ToInt32(model[0].PhysicianId),
                        4);
                }

                if (model.Count > 0 && model[0].Status == "4") // After Cancel of the Appointmnet by Physician(from physician Login Screen) mail will be sent to patient.
                {
                    Helpers.SendAppointmentNotification(
                        model,
                        model[0].PatientEmailId,
                        Convert.ToString(model[0].EmailTemplateId),
                       Convert.ToInt32(model[0].PatientId),
                        Convert.ToInt32(model[0].PhysicianId),
                        5);
                }
            }

            var listobj = new
            {
                IsError = false,
                patientid = model[0].PatientId,
                notAvialableTimeslotslist,
                roomEquipmentnotaviable
            };

            return Json(listobj, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}