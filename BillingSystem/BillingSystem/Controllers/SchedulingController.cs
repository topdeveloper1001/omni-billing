// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SchedulingController.cs" company="Spadez">
//   This is the code to test the performance in the page
// </copyright>
// <summary>
//   The scheduling controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    using BillingSystem.Bal.BusinessAccess;
    using BillingSystem.Bal.Interfaces;
    using BillingSystem.Common;
    using BillingSystem.Model;
    using BillingSystem.Model.CustomModel;

    /// <summary>
    /// The scheduling controller.
    /// </summary>
    public class SchedulingController : Controller
    {
        private readonly IFacilityStructureService _fsService;
        private readonly IPatientInfoService _piService;
        private readonly IAppointmentTypesService _aService;

        public SchedulingController(IFacilityStructureService fsService, IPatientInfoService piService, IAppointmentTypesService aService)
        {
            _fsService = fsService;
            _piService = piService;
            _aService = aService;
        }

        // GET: /Schedular/
        #region Public Methods and Operators

        /// <summary>
        /// The index.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult Index()
        {
            return this.View();
        }

        /// <summary>
        /// Gets the corporate physicians.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public ActionResult GetCorporatePhysicians(string corporateId, string facilityId)
        {
            using (var phyBal = new PhysicianBal())
            {
                var cId = string.IsNullOrEmpty(corporateId) ? Helpers.GetSysAdminCorporateID().ToString() : corporateId;
                cId = string.IsNullOrEmpty(facilityId)
                          ? cId
                          : Helpers.GetCorporateIdByFacilityId(Convert.ToInt32(facilityId)).ToString();
                var isAdmin = Helpers.GetLoggedInUserIsAdmin();
                var userid = Helpers.GetLoggedInUserId();
                var corporateUsers = phyBal.GetCorporatePhysiciansList(Convert.ToInt32(cId), isAdmin, userid, Convert.ToInt32(facilityId));
                var viewpath = string.Format("../scheduling/{0}", PartialViews.PhysicianCheckBoxList);
                return PartialView(viewpath, corporateUsers);
            }
        }

        /// <summary>
        /// Loads the facility department data.
        /// </summary>
        /// <param name="facilityid">The facilityid.</param>
        /// <returns></returns>
        public ActionResult LoadFacilityDepartmentData(string facilityid)
        {
            
                // Get the facilities list
                var cId = Helpers.GetSysAdminCorporateID();
                var facilityDepartmentList = _fsService.GetFacilityDepartments(cId, facilityid);
                var viewpath = string.Format("../Scheduling/{0}", PartialViews.FacilityDepartmentListView);
                return PartialView(viewpath, facilityDepartmentList);
             
        }

        /// <summary>
        /// Gets the facilty list TreeView.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetFaciltyListTreeView()
        {
            // Initialize the Facility Communicator object
            using (var facilityBal = new FacilityBal())
            {
                // Get the facilities list
                var cId = Helpers.GetSysAdminCorporateID();
                var facilityList = facilityBal.GetFacilityList(cId);
                var viewpath = string.Format("../Scheduling/{0}", PartialViews.LocationListView);
                return PartialView(viewpath, facilityList);
            }
        }

        /// <summary>
        /// Gets the global codes check ListView.
        /// </summary>
        /// <param name="ggcValue">The GGC value.</param>
        /// <returns></returns>
        public ActionResult GetGlobalCodesCheckListView(string ggcValue)
        {
            using (var globalCodeBal = new GlobalCodeBal())
            {
                var globalCodelist = globalCodeBal.GetGCodesListByCategoryValue(ggcValue).Where(x => x.ExternalValue1 != "3").ToList();
                var viewpath = string.Format("../Scheduling/{0}", PartialViews.StatusCheckBoxList);
                return PartialView(viewpath, globalCodelist);
            }
        }


        /// <summary>
        /// Loads the schedulng data.
        /// </summary>
        /// <param name="selectedDate">The selected date.</param>
        /// <param name="facility">The facility.</param>
        /// <param name="viewType">Type of the view.</param>
        /// <returns></returns>
        public ActionResult LoadSchedulngData(string selectedDate, int facility, string viewType)
        {
            using (var schedularbal = new SchedulingBal())
            {
                var listtoconvert = new List<SchedulingCustomModel>();
                var selectedfacility = facility;
                var selectedViewType = viewType;
                var cId = Helpers.GetSysAdminCorporateID().ToString();
                cId = (facility) == 0
                          ? cId
                          : Helpers.GetCorporateIdByFacilityId(Convert.ToInt32(facility)).ToString();
                var isAdmin = Helpers.GetLoggedInUserIsAdmin();
                var userid = Helpers.GetLoggedInUserId();
                var corporateUsers = new PhysicianBal().GetCorporatePhysiciansList(
                    Convert.ToInt32(cId),
                    isAdmin,
                    userid,
                    facility);
                var selectedPhysicians = new List<SchedularFiltersCustomModel>();
                if (corporateUsers.Count > 0)
                {
                    selectedPhysicians.AddRange(corporateUsers.Select(item => new SchedularFiltersCustomModel() { Id = item.Physician.Id }));
                    var phyObj = this.ConvertArrayToCommaSeperatorString(selectedPhysicians);

                    // .....load physician based data
                    listtoconvert.AddRange(
                      schedularbal.GetSchedulingByPhysiciansData(
                          phyObj,
                          Convert.ToDateTime(selectedDate),
                          selectedViewType, Helpers.GetDefaultFacilityId()));
                }

                // .....load Holiday and vocation based data
                listtoconvert.AddRange(
                    schedularbal.GetHolidayPlannerData(
                        0,
                        Convert.ToDateTime(selectedDate),
                        selectedViewType,
                        Convert.ToInt32(selectedfacility)));

                // convert the data to json list object
                var listtoreturn = new
                {
                    savedSlots = GetTimeSlotsCustomModel2(listtoconvert),
                    selectedPhysicians,
                };
                return Json(listtoreturn, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetSchedularWithFilters(List<SchedularTypeCustomModel> filters)
        {
            using (var schedularbal = new SchedulingBal())
            {
                var listtoReturn = new List<SchedulingCustomModel>();

                var selectedPhysicianList = filters[0].PhysicianId;
                var selectedStatusList = filters[0].StatusType;
                var selectedDate = filters[0].SelectedDate;
                var selectedDepartmentList = filters[0].DeptData;
                var facilityid = filters[0].Facility;
                var viewtype = filters[0].ViewType;
                var patientId = filters[0].PatientId;
                var phyObj = this.ConvertArrayToCommaSeperatorString(selectedPhysicianList);
                if (patientId > 0)
                {
                    listtoReturn.AddRange(schedularbal.GetPatientScheduling(patientId, selectedDate, viewtype));
                }
                else
                {
                    if (selectedPhysicianList.All(item => item.Id != 0))
                    {
                        listtoReturn.AddRange(schedularbal.GetSchedulingByPhysiciansData(phyObj, selectedDate, viewtype, Helpers.GetDefaultFacilityId()));
                    }
                }

                if (selectedDepartmentList != null)
                {
                    // Consider External value 1 as the Department
                    if (selectedDepartmentList[0].Id != 0)
                    {
                        listtoReturn = listtoReturn.Count > 0
                                           ? listtoReturn.Where(
                                               x => selectedDepartmentList.Any(x1 => x1.Id.ToString() == x.ExtValue1))
                                                 .ToList()
                                           : new List<SchedulingCustomModel>();

                        if (listtoReturn.Count == 0)
                        {
                            foreach (var item in selectedDepartmentList)
                            {
                                listtoReturn.AddRange(
                                    schedularbal.GetSchedulingDeptDataByType(
                                        item.Id,
                                        selectedDate,
                                        viewtype,
                                        facilityid));
                            }
                        }
                    }
                }

                listtoReturn =
                    listtoReturn.Where(x => selectedStatusList.Any(x1 => x1.Id.ToString() == x.Status)).ToList();

                if (selectedDepartmentList[0].Id != 0)
                {
                    // bind the holiday planner based on the Selectd departments from the front end
                    foreach (var item in selectedDepartmentList)
                    {
                        listtoReturn.AddRange(
                            schedularbal.GetHolidayPlannerData(
                                item.Id,
                                Convert.ToDateTime(selectedDate),
                                viewtype,
                                Convert.ToInt32(facilityid)));
                    }
                }
                else
                {
                    // bind the holiday planner based on the selected Facility from the front end
                    foreach (var item in selectedPhysicianList.Where(item => item.Id != 0))
                    {
                        listtoReturn.AddRange(
                            schedularbal.GetHolidayPlannerData(
                                item.Id,
                                Convert.ToDateTime(selectedDate),
                                viewtype,
                                Convert.ToInt32(facilityid)));
                    }
                }

                var textlist = GetTimeSlotsCustomModel2(listtoReturn);
                return Json(textlist, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// Gets the facilities dropdown data on scheduler.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetFacilitiesDropdownDataOnScheduler()
        {
            var cId = Helpers.GetSysAdminCorporateID();
            var userisAdmin = Helpers.GetLoggedInUserIsAdmin();
            using (var facBal = new FacilityBal())
            {
                var facilities = userisAdmin ? facBal.GetFacilities(cId) : facBal.GetFacilities(cId, Helpers.GetDefaultFacilityId());
                if (facilities.Any())
                {
                    var list = new List<SelectListItem>();
                    list.AddRange(facilities.Select(item => new SelectListItem
                    {
                        Text = item.FacilityName,
                        Value = Convert.ToString(item.FacilityId),
                    }));

                    list = list.OrderBy(f => f.Text).ToList();
                    return Json(list);
                }
            }

            return Json(null);
        }

        /// <summary>
        /// Saves the patient scheduling.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public JsonResult SavePatientScheduling(List<SchedulingCustomModel> model)
        {
            var list = new SchedulingCustomModelView();
            var notAvialableTimeslotslist = new List<NotAvialableTimeSlots>();
            var patientid = 0;
            var roomEquipmentnotaviable = new List<NotAvialableRoomEquipment>();
            if (model != null)
            {
                var corporateId = Helpers.GetSysAdminCorporateID();
                var randomnumber = Helpers.GenerateCustomRandomNumber();
                using (var oSchedulingBal = new SchedulingBal())
                {
                    var token = CommonConfig.GenerateLoginCode(8, false);
                    // var phName = string.Empty;
                    // var physician = new PhysicianBal().GetPhysicianById(Convert.ToInt32(model[0].PhysicianId));
                    // phName = physician != null ? physician.PhysicianName : string.Empty;

                    // check to add the physician department
                    var physicianId = model[0].PhysicianId;
                    var physicianDeptid = string.Empty;
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

                    // if (model[0].AssociatedId == 0)
                    // {
                    // save Patient Info

                    var patientinfo = model[0].AssociatedId != 0 ? _piService.GetPatientInfoById(model[0].AssociatedId) :
                            new PatientInfo
                            {
                                CorporateId = corporateId,
                                CreatedBy = Helpers.GetLoggedInUserId(),
                                CreatedDate = Helpers.GetInvariantCultureDateTime(),
                                FacilityId = model[0].FacilityId,
                                PatientID = model[0].AssociatedId != 0 ? Convert.ToInt32(model[0].AssociatedId) : Convert.ToInt32(patientid),
                                PersonFirstName = model[0].PatientName,
                                PersonLastName = model[0].PatientName,
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
                    for (int index = 0; index < model.Count; index++)
                    {
                        var item = model[index];
                        var isSlotAvailable = Convert.ToBoolean(item.IsRecurring)
                                                  ? oSchedulingBal.CheckForDuplicateRecordRecurring(
                                                      Convert.ToDateTime(item.RecurringDateFrom),
                                                      Convert.ToDateTime(item.RecurringDateTill),
                                                      GetTimeToRecurrance(item),
                                                      Convert.ToDateTime(item.ScheduleTo).ToString("HH:mm"),
                                                      Convert.ToInt32(item.FacilityId),
                                                      item.SchedulingId,
                                                      Convert.ToInt32(item.PhysicianId),
                                                      item.RecPattern)
                                                  : oSchedulingBal.CheckForDuplicateRecord(
                                                      item.SchedulingId,
                                                      item.SchedulingType,
                                                      Convert.ToDateTime(item.ScheduleFrom),
                                                      Convert.ToDateTime(item.ScheduleTo),
                                                      Convert.ToInt32(item.AssociatedId),
                                                      Convert.ToInt32(item.PhysicianId),
                                                      Convert.ToInt32(item.FacilityId));

                        item.AssociatedId = item.AssociatedId != 0 ? item.AssociatedId : patientid;
                        item.CorporateId = corporateId;
                        item.ExtValue1 = physicianDeptid;
                        item.SchedulingId = item.SchedulingId != 0 ? item.SchedulingId : 0;
                        item.CreatedBy = Helpers.GetLoggedInUserId();
                        item.CreatedDate = Helpers.GetInvariantCultureDateTime();

                        item.ExtValue4 = token;
                        var appointmentType = string.Empty;
                        var app = _aService.GetAppointmentTypesById(Convert.ToInt32(item.TypeOfProcedure));
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
                        if (isSlotAvailable)
                        {
                            notAvialableTimeslotslist.Add(oSchedulingBal.GetNotAvialableTimeSlotsCustomModel(item));

                            // model.RemoveAt(index);
                            var listobjtoReturn = new
                            {
                                IsError = true,
                                patientid = model[0].PatientId,
                                notAvialableTimeslotslist,
                                roomEquipmentnotaviable
                            };
                            return this.Json(listobjtoReturn, JsonRequestBehavior.AllowGet);
                        }

                        // This method call will tell that the room and equipment is avialable for the type of procedure and for the selected date
                        var isRoomalloted = oSchedulingBal.GetAssignedRoomForProcedure(
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
                                roomEquipmentnotaviable.Add(new NotAvialableRoomEquipment()
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
                                return this.Json(listobjtoReturn, JsonRequestBehavior.AllowGet);
                            }

                            //Check should be bypassed if Appointment types are with the tag 'No Equipment Required'
                            var noEquipmentRequired = app != null && (!string.IsNullOrEmpty(app.ExtValue1) &&
                                                                      int.Parse(app.ExtValue1) == 1);

                            if (isRoomalloted.EquipmentId != 0 || noEquipmentRequired)
                            {
                                item.EquipmentAssigned = noEquipmentRequired ? 0 : isRoomalloted.EquipmentId;
                            }
                            else
                            {
                                roomEquipmentnotaviable.Add(new NotAvialableRoomEquipment()
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
                                return this.Json(listobjtoReturn, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }

                    list = oSchedulingBal.SavePatientScheduling(model);
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
                    // SendNotificationMail(model);
                }
            }

            var listobj = new
            {
                IsError = false,
                patientid = model[0].PatientId,
                notAvialableTimeslotslist,
                roomEquipmentnotaviable
            };

            return this.Json(listobj, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Converts the array to comma seperator string.
        /// </summary>
        /// <param name="listToParse">The list to parse.</param>
        /// <returns></returns>
        private string ConvertArrayToCommaSeperatorString(IEnumerable<SchedularFiltersCustomModel> listToParse)
        {
            var variablestring = string.Join(",", listToParse.Select(x => x.Id));
            return variablestring;
        }

        /// <summary>
        /// Gets the time slots custom model2.
        /// </summary>
        /// <param name="facultyTimeslotslist">The faculty timeslotslist.</param>
        /// <returns></returns>
        private List<SchedulerCustomModelForCalender> GetTimeSlotsCustomModel2(
            IEnumerable<SchedulingCustomModel> facultyTimeslotslist)
        {
            var listtoReturn = new List<SchedulerCustomModelForCalender>();

            listtoReturn.AddRange(
                facultyTimeslotslist.Select(
                    item =>
                    new SchedulerCustomModelForCalender
                    {
                        id =
                            string.IsNullOrEmpty(item.EventId)
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
                        PhysicianComments = item.Comments,
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
                    }));

            return listtoReturn;
        }

        /// <summary>
        /// Gets the other procedures by event parent identifier.
        /// </summary>
        /// <param name="eventparentId">The eventparent identifier.</param>
        /// <returns></returns>
        private List<TypeOfProcedureCustomModel> GetOtherProceduresByEventParentId(string eventparentId, DateTime scheduleFrom)
        {
            using (var schedulingBal = new SchedulingBal())
            {
                var schdulingList = schedulingBal.GetOtherProceduresByEventParentId(eventparentId, scheduleFrom);
                return schdulingList;
            }
        }

        /// <summary>
        /// Gets the time to recurrance.
        /// </summary>
        /// <param name="itemModel">The item model.</param>
        /// <returns></returns>
        private string GetTimeToRecurrance(Scheduling itemModel)
        {
            var timefrom = itemModel.RecurringDateFrom;
            var ticksforTimeFrom = Convert.ToDateTime(timefrom);
            var timeto = ticksforTimeFrom.AddTicks(Convert.ToInt64(itemModel.RecEventlength * 10000000));
            return timeto.ToString("HH:mm");
        }
        #endregion
    }
}