
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Models;

namespace BillingSystem.Controllers
{

    /// <summary>
    /// The patient scheduler portal controller.
    /// </summary>
    public class PatientSchedulerPortalController : Controller
    {
        private readonly IEncounterService _eService;
        private readonly IPatientInfoService _piService;
        private readonly IAppointmentTypesService _atSerice;

        public PatientSchedulerPortalController(IEncounterService eService, IPatientInfoService piService, IAppointmentTypesService atSerice)
        {
            _eService = eService;
            _piService = piService;
            _atSerice = atSerice;
        }

        // GET: /PatientSchedulerPortal/
        #region Public Methods and Operators

        /// <summary>
        /// The index.
        /// </summary>
        /// <param name="pId">The p identifier.</param>
        /// <param name="fId">The f identifier.</param>
        /// <param name="cId">The c identifier.</param>
        /// <returns>
        /// The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult Index(int? pId, int? fId, int? cId)
        {
            var patientSchedulerPortalView = new PatientSchedulerPortalView();
            if (pId == null)
            {
                ViewBag.Message = "Invalid patient id!";
                ViewBag.MessageId = 1;
            }
            else
            {
                patientSchedulerPortalView.CorporateId = Convert.ToInt32(cId);
                patientSchedulerPortalView.FacilityId = Convert.ToInt32(fId);
                patientSchedulerPortalView.PatientId = Convert.ToInt32(pId);

                var patientRecentEncounter = _eService.GetEncountersListByPatientId(Convert.ToInt32(pId)).ToList();
                if (patientRecentEncounter.Any())
                {
                    var recentEncounter =
                        patientRecentEncounter.OrderByDescending(x => x.EncounterID).FirstOrDefault();
                    var physicianName = _eService.GetPhysicianName(Convert.ToInt32(recentEncounter.EncounterAttendingPhysician));

                    patientSchedulerPortalView.IsPreviousEncounter = true;
                    patientSchedulerPortalView.PreviousEncounterPhysicianId =
                        recentEncounter.EncounterAttendingPhysician.ToString();
                    patientSchedulerPortalView.PreviousEncounterPhysicianName = physicianName;

                    return View(patientSchedulerPortalView);

                }
            }

            patientSchedulerPortalView.IsPreviousEncounter = false;
            patientSchedulerPortalView.PreviousEncounterPhysicianId = string.Empty;
            patientSchedulerPortalView.PreviousEncounterPhysicianName = string.Empty;
            return View(patientSchedulerPortalView);
        }

        /// <summary>
        /// Gets the patient schedular.
        /// </summary>
        /// <param name="filters">The filters.</param>
        /// <returns></returns>
        public ActionResult GetPatientSchedular(List<SchedularTypeCustomModel> filters)
        {
            using (var schedularbal = new SchedulingBal())
            {
                var listtoReturn = new List<SchedulingCustomModel>();
                //var selectedStatusList = filters[0].StatusType;
                var selectedDate = filters[0].SelectedDate;
                //var facilityid = filters[0].Facility;
                var viewtype = filters[0].ViewType;
                var patientId = filters[0].PatientId;
                if (patientId > 0)
                {
                    listtoReturn.AddRange(schedularbal.GetPatientScheduling(patientId, selectedDate, viewtype));
                }

                var textlist = GetPatientSchedularCustomModel(listtoReturn);
                return Json(textlist, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Loads the patient schedulng data.
        /// </summary>
        /// <param name="selectedDate">The selected date.</param>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public ActionResult LoadPatientSchedulngData(string selectedDate, int patientId)
        {
            using (var schedularbal = new SchedulingBal())
            {
                var listtoReturn = new List<SchedulingCustomModel>();
                if (patientId > 0)
                {
                    listtoReturn.AddRange(schedularbal.GetPatientScheduling(patientId, Convert.ToDateTime(selectedDate), "1"));
                }
                var textlist = GetPatientSchedularCustomModel(listtoReturn);
                return Json(textlist, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Gets the patient details.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public ActionResult GetPatientDetails(string patientId)
        {
            var objToReturn = _piService.PatientInfoForSchedulingByPatient(Convert.ToInt32(patientId));
            return Json(objToReturn, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the physician patient data.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="phyId">The phy identifier.</param>
        /// <returns></returns>
        public ActionResult GetPhysicianPatientData(string patientId, string phyId)
        {
            var objToReturn = _piService.PatientInfoForSchedulingByPatient(Convert.ToInt32(patientId));
            return Json(objToReturn, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Saves the patient pre scheduling.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public ActionResult SavePatientPreScheduling(List<SchedulingCustomModel> model)
        {
            var notAvialableTimeslotslist = new List<NotAvialableTimeSlots>();
            var patientid = 0;
            var roomEquipmentnotaviable = new List<NotAvialableRoomEquipment>();
            if (model != null)
            {
                var corporateId = Helpers.GetSysAdminCorporateID();

                using (var oSchedulingBal = new SchedulingBal())
                {
                    var token = CommonConfig.GenerateLoginCode(8, false);
                    string physicianDeptid;
                    if (!string.IsNullOrEmpty(model[0].PhysicianId))
                    {
                        var physicianBal = new PhysicianBal().GetPhysicianById(Convert.ToInt32(model[0].PhysicianId));
                        physicianDeptid = physicianBal != null ? physicianBal.FacultyDepartment : string.Empty;
                    }
                    else
                    {
                        var physicianBal = new PhysicianBal().GetPhysicianById(Convert.ToInt32(model[0].AssociatedId));
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
                        var patientPhone = new PatientPhoneBal().GetPatientPersonalPhoneByPateintId(patientid);
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
                        new PatientPhoneBal().SavePatientPhone(patientPhonenumber);
                    }

                    // }
                    new PatientLoginDetailBal().UpdatePatientEmailId(
                        model[0].AssociatedId != 0 ? Convert.ToInt32(model[0].AssociatedId) : Convert.ToInt32(patientid),
                        model[0].PatientEmailId);

                    // ..... Remove the Old Data for the user in case of Edit
                    //oSchedulingBal.DeleteHolidayPlannerData(model[0].EventParentId, 0);
                    model[0].PatientId = model[0].PatientId != null && model[0].PatientId != 0
                                             ? model[0].PatientId
                                             : patientid;
                    for (var index = 0; index < model.Count; index++)
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
                        var app = _atSerice.GetAppointmentTypesById(Convert.ToInt32(item.TypeOfProcedure));
                        var appointmentType = app != null ? app.Name : string.Empty;
                        item.AppointmentType = appointmentType;

                        // item.PhysicianName = phName;
                        item.WeekDay = Convert.ToString(Helpers.GetWeekOfYearISO8601(Convert.ToDateTime(item.ScheduleFrom)));
                        item.IsActive = true;

                        item.EventParentId = item.IsRecurring == false || string.IsNullOrEmpty(item.EventParentId)
                                                 ? randomnumber
                                                 : item.EventParentId;

                        item.RecEventPId = item.IsRecurring == false || string.IsNullOrEmpty(item.EventParentId)
                                                ? Convert.ToInt32(randomnumber)
                                                : Convert.ToInt32(item.EventParentId);

                    }
                    foreach (var item in model.Where(item => !string.IsNullOrEmpty(item.RemovedAppointmentTypes)))
                    {
                        oSchedulingBal.DeleteHolidayPlannerData(string.Empty, Convert.ToInt32(item.RemovedAppointmentTypes), Convert.ToInt32(item.SchedulingType), "");
                    }

                    oSchedulingBal.SavePatientPreSchedulingList(model);
                    if (model.Count > 0 && model[0].Status == "1")//In Initial Booking mail will be sent to Patient
                    {
                        Helpers.SendAppointmentNotification(
                            model,
                            model[0].PatientEmailId,
                            Convert.ToString(model[0].EmailTemplateId),
                            Convert.ToInt32(model[0].PatientId),
                            Convert.ToInt32(model[0].PhysicianId),
                            2);
                    }
                    if (model.Count > 0 && model[0].Status == "3")// After Confirmation of the Appointmnet by Physician( from physician Login Screen) mail will be sent to patient.
                    {
                        Helpers.SendAppointmentNotification(
                            model,
                            model[0].PatientEmailId,
                            Convert.ToString(model[0].EmailTemplateId),
                            Convert.ToInt32(model[0].PatientId),
                            Convert.ToInt32(model[0].PhysicianId),
                            4);
                    }
                    if (model.Count > 0 && model[0].Status == "4")// After Cancel of the Appointmnet by Physician(from physician Login Screen) mail will be sent to patient.
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
            }

            var listobj = new
            {
                IsError = false,
                patientid = model != null ? model[0].PatientId : 0,
                notAvialableTimeslotslist,
                roomEquipmentnotaviable
            };

            return Json(listobj, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Gets the patient schedular custom model.
        /// </summary>
        /// <param name="facultyTimeslotslist">The faculty timeslotslist.</param>
        /// <returns></returns>
        private List<SchedulerCustomModelForCalender> GetPatientSchedularCustomModel(
           IEnumerable<SchedulingCustomModel> facultyTimeslotslist)
        {
            var departmentOpeninglist = new List<SchedulerCustomModelForCalender>();

            departmentOpeninglist.AddRange(
                facultyTimeslotslist.Select(
                    item =>
                    new SchedulerCustomModelForCalender
                    {
                        id =
                            !string.IsNullOrEmpty(item.EventId)
                                ? Convert.ToInt64(item.EventId)
                                : 0,
                        text = item.Comments,
                        start_date = item.ScheduleFrom.Value.ToString("MM/dd/yyyy HH:mm"),
                        end_date = item.ScheduleTo.Value.ToString("MM/dd/yyyy HH:mm"),
                        Availability = item.Status,
                        _timed = Convert.ToBoolean(item.IsRecurring),
                        IsRecurrance = Convert.ToBoolean(item.IsRecurring),
                        rec_pattern = item.RecPattern,
                        rec_type = item.RecType,
                        rec_pattern_type = item.RecPattern,
                        rec_type_type = item.RecType,
                        TimeSlotId = item.SchedulingId,
                        event_length = Convert.ToInt64(item.RecEventlength),
                        event_pid =
                            Convert.ToBoolean(item.IsRecurring)
                                ? Convert.ToInt32(item.EventId)
                                : 0,
                        Department = item.ExtValue1,
                        PatientName = item.PatientName,
                        PatientEmailId = item.PatientEmailId,
                        EmirateIdnumber = item.PatientEmirateIdNumber,
                        PatientPhoneNumber = item.PatientPhoneNumber,
                        PatientDOB =
                            item.PatientDOB.HasValue
                                ? item.PatientDOB.Value.ToShortDateString()
                                : string.Empty,
                        PhysicianComments = item.ExtValue5,
                        PhysicianSpeciality = item.PhysicianSpeciality,
                        physicianid = Convert.ToInt32(item.PhysicianId),
                        patientid = Convert.ToInt32(item.PatientId),
                        section_id =
                            string.IsNullOrEmpty(item.PhysicianId)
                                ? item.AssociatedId.ToString()
                                : item.PhysicianId,
                        SchedulingType = item.SchedulingType,
                        TypeOfProcedureCustomModel =
                            GetOtherProceduresByEventParentId(item.EventParentId, Convert.ToDateTime(item.ScheduleFrom)),
                        MultipleProcedure = Convert.ToBoolean(item.ExtValue3),
                        Rec_Start_date =
                         item.RecurringDateFrom != null
                             ? item.RecurringDateFrom.Value.ToShortDateString()
                             : string.Empty,
                        Rec_end_date = item.RecurringDateTill != null
                             ? item.RecurringDateTill.Value.ToShortDateString()
                             : string.Empty,
                        EventParentId = item.EventParentId,
                        location = item.FacilityId.ToString(),
                        TimeSlotTimeInterval = string.Empty,
                        VacationType = item.TypeOfProcedure,
                        PhysicianName = item.PhysicianName,
                        Rec_Start_date_type =
                          item.RecurringDateFrom != null
                              ? item.RecurringDateFrom.Value.ToShortDateString()
                              : string.Empty,
                        Rec_end_date_type =
                            item.RecurringDateTill != null
                                ? item.RecurringDateTill.Value.ToShortDateString()
                                : string.Empty,
                        AppointmentTypeStr = item.AppointmentTypeStr,
                        DepartmentName = item.DepartmentName,
                        PhysicianSpecialityStr = item.PhysicianSPL
                    }));

            return departmentOpeninglist;
        }

        /// <summary>
        /// Gets the other procedures by event parent identifier.
        /// </summary>
        /// <param name="eventparentId">The eventparent identifier.</param>
        /// <param name="scheduleFrom">The schedule from.</param>
        /// <returns></returns>
        private List<TypeOfProcedureCustomModel> GetOtherProceduresByEventParentId(string eventparentId, DateTime scheduleFrom)
        {
            using (var schedulingBal = new SchedulingBal())
            {
                var schdulingList = schedulingBal.GetOtherProceduresByEventParentId(eventparentId, scheduleFrom);
                return schdulingList;
            }
        }
        #endregion
    }
}