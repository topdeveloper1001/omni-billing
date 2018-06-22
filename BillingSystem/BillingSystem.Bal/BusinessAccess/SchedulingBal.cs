// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FacultyTimeslotsBal.cs" company="">
//   
// </copyright>
// <summary>
//   The faculty timeslots bal.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Data;

namespace BillingSystem.Bal.BusinessAccess
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Mapper;
    using Model;
    using Model.CustomModel;

    /// <summary>
    ///     The faculty timeslots bal.
    /// </summary>
    public class SchedulingBal : BaseBal
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="FacultyTimeslotsBal" /> class.
        /// </summary>
        public SchedulingBal()
        {
            SchedulingMapper = new SchedulingMapper();
            NotAvialableTimeSlotSchedulingMapper = new NotAvialableTimeSlotSchedulingMapper();
            FacultyTimeslotsMapper = new FacultyTimeslotsMapper();
            HolidayPlannerMapper = new HolidayPlannerMapper();
            HolidayPlannerDetailsMapper = new HolidayPlannerDetailsMapper();
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the scheduling mapper.
        /// </summary>
        /// <value>
        ///     The scheduling mapper.
        /// </value>
        private SchedulingMapper SchedulingMapper { get; set; }

        private NotAvialableTimeSlotSchedulingMapper NotAvialableTimeSlotSchedulingMapper { get; set; }

        /// <summary>
        ///     Gets or sets the faculty timeslots mapper.
        /// </summary>
        /// <value>
        ///     The faculty timeslots mapper.
        /// </value>
        private FacultyTimeslotsMapper FacultyTimeslotsMapper { get; set; }

        /// <summary>
        ///     Gets or sets the Holiday planner mapper.
        /// </summary>
        /// <value>
        ///     The Holiday planner mapper.
        /// </value>
        private HolidayPlannerMapper HolidayPlannerMapper { get; set; }

        /// <summary>
        ///     Gets or sets the Holiday planner details mapper.
        /// </summary>
        /// <value>
        ///     The Holiday planner details mapper.
        /// </value>
        private HolidayPlannerDetailsMapper HolidayPlannerDetailsMapper { get; set; }

        #endregion

        /// <summary>
        /// Saves the patient scheduling.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public SchedulingCustomModelView SavePatientScheduling(IEnumerable<SchedulingCustomModel> model)
        {
            using (var rep = UnitOfWork.SchedulingRepository)
            {
                var schedulingObjList = new List<Scheduling>();

                schedulingObjList.AddRange(model.Select(item => SchedulingMapper.MapViewModelToModel(item)));

                if (schedulingObjList.Any())
                {
                    foreach (var item in schedulingObjList)
                    {
                        if (item.SchedulingId > 0)
                        {
                            DeleteHolidayPlannerData(item.IsRecurring == true ? item.EventParentId : string.Empty,
                                item.SchedulingId, Convert.ToInt32(item.SchedulingType), "");
                        }

                        rep.Create(item);

                        if (item.IsRecurring == true)
                            rep.CreateRecurringSchedularEvents(item.SchedulingId);
                    }
                }

                var list = new SchedulingCustomModelView();

                return list;
            }
        }

        /// <summary>
        /// Saves the holiday scheduling.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public List<SkippedHolidaysData> SaveHolidayScheduling(List<SchedulingCustomModel> model)
        {
            var listtoReturn = new List<SkippedHolidaysData>();

            using (var rep = UnitOfWork.SchedulingRepository)
            {
                var schedulingObjList = new List<Scheduling>();
                var isFacilityHoliday = false;
                var schedulingIds = new List<int>();
                schedulingObjList.AddRange(model.Select(item => SchedulingMapper.MapViewModelToModel(item)));
                if (schedulingObjList.Any())
                {
                    foreach (var item in schedulingObjList)
                    {
                        rep.Create(item);
                        if (!string.IsNullOrEmpty(item.ExtValue2))
                        {
                            isFacilityHoliday = true;
                            schedulingIds.Add(item.SchedulingId);
                        }
                    }
                }

                var associatedId = Convert.ToString(model[0].AssociatedId);

                if (isFacilityHoliday)
                {
                    var rep1 = UnitOfWork.SchedulingRepository;
                    var schIds = (string.Join(",", schedulingIds));
                    rep1.ScheduleHolidays(schIds, "", associatedId);
                }
                return listtoReturn;
            }
        }


        /// <summary>
        /// Gets the type of the scheduling data by.
        /// </summary>
        /// <param name="selectedDate">The selected date.</param>
        /// <param name="type">The type.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public List<SchedulingCustomModel> GetSchedulingDataByType(List<string> phyList, DateTime selectedDate, string type, int facilityId)
        {
            var listOfPatientScheduling = new List<Scheduling>();
            var listOfPatientSchedulingCustomModel = new List<SchedulingCustomModel>();
            try
            {
                using (var rep = UnitOfWork.SchedulingRepository)
                {
                    listOfPatientScheduling = phyList.Count > 0
                        ? rep.Where(
                            x =>
                                (phyList.Contains(x.PhysicianId)
                                 || (x.AssociatedType == "2" && phyList.Contains(x.AssociatedId.ToString())))).ToList()
                        : rep.Where(
                            x => (x.FacilityId == facilityId)).ToList();

                    listOfPatientScheduling = listOfPatientScheduling.Where(
                        x =>
                        x.ScheduleTo != null && (x.ScheduleFrom != null && (x.ScheduleFrom.Value.ToShortDateString() == selectedDate.ToShortDateString()
                                                                            || x.ScheduleTo.Value.ToShortDateString() == selectedDate.ToShortDateString()))).ToList();
                    listOfPatientSchedulingCustomModel.AddRange(listOfPatientScheduling.Select(item => SchedulingMapper.MapModelToViewModel(item)));
                    return listOfPatientSchedulingCustomModel;
                }
            }
            catch (Exception)
            {
                return listOfPatientSchedulingCustomModel;
            }
        }

        /// <summary>
        /// Gets the type of the scheduling dept data by.
        /// </summary>
        /// <param name="deptId">The dept identifier.</param>
        /// <param name="selectedDate">The selected date.</param>
        /// <param name="type">The type.</param>
        /// <param name="facilityIdstr">The facility idstr.</param>
        /// <returns></returns>
        public List<SchedulingCustomModel> GetSchedulingDeptDataByType(int deptId, DateTime selectedDate, string type, string facilityIdstr)
        {
            var listOfDeptScheduling = new List<SchedulingCustomModel>();
            try
            {
                using (var rep = UnitOfWork.SchedulingRepository)
                {
                    var deptIdStr = deptId.ToString();
                    var facilityId = Convert.ToInt32(facilityIdstr);
                    var schedulingObj = deptIdStr != "0"
                                                  ? rep.Where(
                                                      x => x.ExtValue1 == deptIdStr
                                                       && x.SchedulingType == type).ToList()
                                                  : rep.Where(
                                                      x => (x.FacilityId == facilityId && x.SchedulingType == type))
                                                        .ToList();

                    schedulingObj = schedulingObj.Where(
                        x =>
                        x.ScheduleTo != null && (x.ScheduleFrom != null && (x.ScheduleFrom.Value.ToShortDateString() == selectedDate.ToShortDateString()
                                                                            || x.ScheduleTo.Value.ToShortDateString() == selectedDate.ToShortDateString()))).ToList();
                    listOfDeptScheduling.AddRange(schedulingObj.Select(item => SchedulingMapper.MapModelToViewModel(item)));
                    return listOfDeptScheduling;
                }
            }
            catch (Exception)
            {
                return listOfDeptScheduling;
            }
        }

        /// <summary>
        /// Gets the other procedures by event parent identifier.
        /// </summary>
        /// <param name="eventparentId">The eventparent identifier.</param>
        /// <param name="scheduleFrom">The schedule from.</param>
        /// <returns></returns>
        public List<TypeOfProcedureCustomModel> GetOtherProceduresByEventParentId(string eventparentId, DateTime scheduleFrom)
        {
            var listtoReturn = new List<TypeOfProcedureCustomModel>();
            try
            {
                using (var rep = UnitOfWork.SchedulingRepository)
                {
                    var valueToGet = rep.Where(x => x.EventParentId == eventparentId).ToList();
                    valueToGet =
                        valueToGet.Where(
                            x => x.ScheduleFrom.Value.ToShortDateString() == scheduleFrom.ToShortDateString()).ToList();
                    listtoReturn.AddRange(
                        valueToGet.Select(item => new TypeOfProcedureCustomModel
                        {
                            DateSelected = item.ScheduleFrom.Value.ToShortDateString(),
                            TimeFrom = Convert.ToBoolean(item.IsRecurring) ? item.RecurringDateFrom.Value.ToString("HH:mm") : item.ScheduleFrom.Value.ToString("HH:mm"),
                            TimeTo = Convert.ToBoolean(item.IsRecurring) ? GetTimeToRecurrance(item) : item.ScheduleTo.Value.ToString("HH:mm"),
                            TypeOfProcedureId = item.TypeOfProcedure,
                            EventParentId = item.EventParentId,
                            MainId = item.SchedulingId,
                            TypeOfProcedureName = GetAppointmentNameById(Convert.ToInt32(item.TypeOfProcedure), Convert.ToInt32(item.CorporateId), Convert.ToInt32(item.FacilityId)),
                            TimeSlotTimeInterval = GetAppointmentTypeTimeIntervalById(Convert.ToInt32(item.TypeOfProcedure), Convert.ToInt32(item.CorporateId), Convert.ToInt32(item.FacilityId)),
                            ProcedureAvailablityStatus = item.Status,
                            IsRecurrance = Convert.ToBoolean(item.IsRecurring),
                            Rec_Pattern = item.RecPattern,
                            Rec_Type = item.RecType,
                            end_By = Convert.ToBoolean(item.IsRecurring) ? item.RecurringDateTill.Value.ToShortDateString() : item.ScheduleTo.Value.ToShortDateString(),//item.ScheduleTo.Value.ToShortDateString(),
                            Rec_Start_date = Convert.ToBoolean(item.IsRecurring) ? item.RecurringDateFrom.Value.ToShortDateString() : item.ScheduleFrom.Value.ToShortDateString(),
                            Rec_end_date = Convert.ToBoolean(item.IsRecurring) ? item.RecurringDateTill.Value.ToShortDateString() : item.ScheduleTo.Value.ToShortDateString(),
                            DeptOpeningDays = GetDeptOpeningDaysForPhysician(Convert.ToInt32(item.PhysicianId))
                        }));
                }

                return listtoReturn;
            }
            catch (Exception)
            {
                return listtoReturn;
            }
        }

        /// <summary>
        /// Gets the holiday planner data.
        /// </summary>
        /// <param name="physicianId">The physician identifier.</param>
        /// <param name="selectedDate">The selected date.</param>
        /// <param name="type">The type.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public List<SchedulingCustomModel> GetHolidayPlannerData(int physicianId, DateTime selectedDate, string type, int facilityId)
        {
            var listOfHolidaySchedulingCustomModel = new List<SchedulingCustomModel>();
            try
            {
                using (var rep = UnitOfWork.SchedulingRepository)
                {
                    var physicianIdStr = physicianId.ToString();

                    // listOfPatientScheduling = rep.GetAll().ToList();
                    var listOfHolidayScheduling = physicianIdStr != "0"
                        ? rep.Where(
                            x =>
                                (x.AssociatedId == physicianId) && (x.AssociatedType == "2")
                                && (x.FacilityId == facilityId && x.SchedulingType == "2"))
                            .ToList()
                        : new List<Scheduling>();

                    // : rep.Where(
                    // x => (x.FacilityId == facilityId && x.SchedulingType == "2"))
                    // .ToList();
                    switch (type)
                    {
                        // 1 =daily  , 2= weekly , 3=Monthly
                        case "1":
                            listOfHolidayScheduling =
                                listOfHolidayScheduling.Where(
                                    x =>
                                    x.ScheduleTo != null
                                    && (x.ScheduleFrom != null
                                        && (x.ScheduleFrom.Value.Date == selectedDate.Date
                                            || x.ScheduleTo.Value.Date
                                            == selectedDate.Date))).ToList();

                            // listOfHolidayScheduling.Where(
                            // x =>(x.ScheduleFrom.Value.ToShortDateString() == selectedDate.ToShortDateString()
                            // || x.ScheduleTo.Value.ToShortDateString()
                            // == selectedDate.ToShortDateString())).ToList();
                            break;
                        case "2":
                            var weeknumber = GetWeekOfYearISO8601Bal(selectedDate).ToString();
                            listOfHolidayScheduling =
                                listOfHolidayScheduling.Where(x => x.WeekDay == weeknumber).ToList();
                            break;
                        case "3":
                            var monthStartDate = new DateTime(selectedDate.Year, selectedDate.Month, 1);
                            var monthEndDate = monthStartDate.AddMonths(1).AddDays(-1);
                            listOfHolidayScheduling =
                                listOfHolidayScheduling.Where(
                                    x =>
                                    x.ScheduleTo != null
                                    && (x.ScheduleFrom != null
                                        && (x.ScheduleFrom.Value.Date >= monthStartDate.Date && x.ScheduleTo.Value.Date <= monthEndDate.Date))).ToList();
                            break;
                        default:
                            listOfHolidayScheduling =
                                listOfHolidayScheduling.Where(
                                    x =>
                                    x.ScheduleTo != null
                                    && (x.ScheduleFrom != null
                                        && (x.ScheduleFrom.Value.ToShortDateString() == selectedDate.ToShortDateString()
                                            || x.ScheduleTo.Value.ToShortDateString()
                                            == selectedDate.ToShortDateString()))).ToList();
                            break;
                    }

                    listOfHolidaySchedulingCustomModel.AddRange(listOfHolidayScheduling.Select(item => SchedulingMapper.MapModelToViewModel(item)));
                    return listOfHolidaySchedulingCustomModel;
                }
            }
            catch (Exception)
            {
                return listOfHolidaySchedulingCustomModel;
            }
        }

        /// <summary>
        /// Deletes the holiday planner data.
        /// </summary>
        /// <param name="eventParentid">The event parentid.</param>
        /// <param name="schedulingid">The schedulingid.</param>
        /// <param name="schedulingType">The schedulingType.</param>
        /// <param name="extValue3">The external value3</param>
        /// <returns></returns>
        public bool DeleteHolidayPlannerData(string eventParentid, int schedulingid, int schedulingType, string extValue3)
        {
            try
            {
                using (var rep = UnitOfWork.SchedulingRepository)
                {
                    var schedulingObjList = !string.IsNullOrEmpty(eventParentid) ? rep.Where(x => x.EventParentId == eventParentid).ToList() :
                        rep.Where(x => x.SchedulingId == schedulingid).ToList();

                    //if (schedulingType == 1)
                    //{
                    //    schedulingObjList = schedulingObjList.Where(x => x.ScheduleTo > DateTime.Now).ToList();
                    //}
                    //else
                    //{
                    //    schedulingObjList =
                    //        schedulingObjList.Where(
                    //            x =>
                    //                x.ScheduleFrom.HasValue && x.ScheduleFrom.Value.Date >= DateTime.Now.Date &&
                    //                x.ExtValue3 == extValue3).ToList();
                    //}

                    if (schedulingObjList.Count > 0)
                    {
                        rep.Delete(schedulingObjList);
                        return true;
                    }

                    return false;

                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Checks for duplicate record.
        /// </summary>
        /// <param name="schedulingId">The scheduling identifier.</param>
        /// <param name="schedulingType">Type of the scheduling.</param>
        /// <param name="starTime">The star time.</param>
        /// <param name="endTime">The end time.</param>
        /// <param name="userid">The userid.</param>
        /// <param name="physicianid">The physicianid.</param>
        /// <param name="facilityid">The facilityid.</param>
        /// <returns></returns>
        public bool CheckForDuplicateRecord(
            int schedulingId,
            string schedulingType,
            DateTime starTime,
            DateTime endTime,
            int userid,
            int physicianid,
            int facilityid)
        {
            using (var rep = UnitOfWork.SchedulingRepository)
            {
                var result = rep.CheckTimeSlotAvailabilityForScheduling(
                    schedulingId,
                    schedulingType,
                    starTime,
                    starTime.ToString("HH:mm"),
                    endTime.ToString("HH:mm"),
                    userid,
                    physicianid,
                    facilityid);
                return result.Count > 0 && result[0].TimeSlotAvailable > 0;

                // var facilityIdStr = facilityid.ToString();
                // var schedulingObjList = schedulingId != 0
                // ? rep.Where(
                // x =>
                // x.SchedulingId != schedulingId && x.ScheduleFrom >= starTime
                // && x.ScheduleTo <= endTime
                // && (x.PhysicianId == physicianid
                // || (x.AssociatedId == userid && x.AssociatedType == "2") || (x.AssociatedId == facilityid && x.AssociatedType == "2" && x.ExtValue2 == facilityIdStr)))
                // .FirstOrDefault()
                // : rep.Where(
                // x =>
                // x.ScheduleFrom >= starTime && x.ScheduleTo <= endTime
                // && (x.PhysicianId == physicianid
                // || (x.AssociatedId == userid && x.AssociatedType == "2") || (x.AssociatedId == facilityid && x.AssociatedType == "2" && x.ExtValue2 == facilityIdStr)))
                // .FirstOrDefault();
                // return schedulingObjList == null;
            }
        }


        /// <summary>
        /// Gets the not avialable time slots custom model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public NotAvialableTimeSlots GetNotAvialableTimeSlotsCustomModel(SchedulingCustomModel model)
        {
            var notavialabletimeslot = NotAvialableTimeSlotSchedulingMapper.MapModelToViewModel(model);
            return notavialabletimeslot;
        }

        /// <summary>
        /// Gets the scheduling custom model by identifier.
        /// </summary>
        /// <param name="schedulingid">The schedulingid.</param>
        /// <returns></returns>
        public SchedulingCustomModel GetSchedulingCustomModelById(int schedulingid)
        {
            using (var rep = UnitOfWork.SchedulingRepository)
            {
                var schedulingObj = rep.Where(x => x.SchedulingId == schedulingid).FirstOrDefault();
                var schedularCustomModel = SchedulingMapper.MapModelToViewModel(schedulingObj);
                return schedularCustomModel;
            }
        }

        public List<SchedulingCustomModel> GetSchedulingListByPatient(int patientId, string physicianId, string vToken)
        {
            var list = new List<SchedulingCustomModel>();
            using (var rep = UnitOfWork.SchedulingRepository)
            {
                var schedulingObj =
                    rep.Where(
                        x =>
                            x.AssociatedId == patientId && x.PhysicianId.Trim().Equals(physicianId) &&
                            x.ExtValue4.Trim().Equals(vToken)).OrderByDescending(s => s.ScheduleFrom).ToList();
                list.AddRange(schedulingObj.Select(item => SchedulingMapper.MapModelToViewModel(item)));
                return list;
            }
        }


        public bool UpdateSchedulingEvents(List<SchedulingCustomModel> list)
        {
            var schedulingObjList = new List<Scheduling>();
            schedulingObjList.AddRange(list.Select(item => SchedulingMapper.MapViewModelToModel(item)));

            using (var rep = UnitOfWork.SchedulingRepository)
            {

                foreach (var item in schedulingObjList)
                {
                    if (item.SchedulingId > 0)
                    {
                        rep.UpdateEntity(item, item.SchedulingId);
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// Gets the assigned room for procedure.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="appointmentType">Type of the appointment.</param>
        /// <param name="scheduledDate">The scheduled date.</param>
        /// <param name="timeFrom">The time from.</param>
        /// <param name="timeTo">The time to.</param>
        /// <param name="roomId">The room Id.</param>
        /// <returns></returns>
        public RoomEquipmentAvialability GetAssignedRoomForProcedure(int facilityId, int appointmentType, DateTime scheduledDate, string timeFrom, string timeTo, Int32 roomId)
        {
            using (var rep = UnitOfWork.SchedulingRepository)
            {
                var roomaviableList = rep.CheckRoomsAndEquipmentsForScheduling(facilityId, appointmentType, scheduledDate, timeFrom, timeTo, roomId);
                return roomaviableList.FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets the available time slots.
        /// </summary>
        /// <param name="facilityid">The facilityid.</param>
        /// <param name="physicianId">The physician identifier.</param>
        /// <param name="dateselected">The dateselected.</param>
        /// <param name="typeofproc">The typeofproc.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public IEnumerable<AvailabilityTimeSlotForPopupCustomModel> GetAvailableTimeSlots(int facilityid, int physicianId, DateTime dateselected, string typeofproc, out DateTime timeSlotDate, bool firstAvailable = false)
        {
            using (var rep = UnitOfWork.SchedulingRepository)
            {
                var roomaviableList = rep.GetAvailableTimeSlots(facilityid, physicianId, dateselected, typeofproc, out timeSlotDate, firstAvailable);
                return roomaviableList.ToList();
            }
        }

        /// <summary>
        /// Checks for duplicate record recurring.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="timeFrom">The time from.</param>
        /// <param name="timeTo">The time to.</param>
        /// <param name="pFacilityid">The p facilityid.</param>
        /// <param name="pSchedulingId">The p scheduling identifier.</param>
        /// <param name="pPhysicianid">The p physicianid.</param>
        /// <param name="pRecPattern">The p record pattern.</param>
        /// <returns></returns>
        public bool CheckForDuplicateRecordRecurring(DateTime startDate, DateTime endDate, string timeFrom, string timeTo, int pFacilityid, int pSchedulingId, int pPhysicianid, string pRecPattern)
        {
            using (var rep = UnitOfWork.SchedulingRepository)
            {
                var result = rep.CheckTimeSlotAvailabilityRecurringForScheduling(
                    startDate,
                    endDate,
                    timeFrom,
                    timeTo,
                    pFacilityid,
                    pSchedulingId,
                    pPhysicianid,
                    pRecPattern);
                return result.Count > 0 && result[0].TimeSlotAvailable > 0;
            }
        }

        /// <summary>
        /// Gets the patient scheduling.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="selectedDate">The selected date.</param>
        /// <param name="viewtype">The viewtype.</param>
        /// <returns></returns>
        public List<SchedulingCustomModel> GetPatientScheduling(int patientId, DateTime selectedDate, string viewtype)
        {
            var listOfPatientScheduling = new List<SchedulingCustomModel>();
            try
            {
                using (var rep = UnitOfWork.SchedulingRepository)
                {
                    var result = rep.GetPatientSchedulingEvents(viewtype, selectedDate, patientId);
                    //listOfPatientScheduling.AddRange(result.Select(item => SchedulingMapper.MapModelToViewModel(item)));
                    return result;
                }
            }
            catch (Exception)
            {
                return listOfPatientScheduling;
            }
        }

        /// <summary>
        /// Gets the patient next scheduling.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="selectedDate">The selected date.</param>
        /// <returns></returns>
        public List<SchedulingCustomModel> GetPatientNextScheduling(int patientId, DateTime selectedDate)
        {
            var listOfPatientScheduling = new List<SchedulingCustomModel>();
            try
            {
                using (var rep = UnitOfWork.SchedulingRepository)
                {
                    var result = rep.GetPatientNextSchedulingEvents(selectedDate, patientId);
                    //listOfPatientScheduling.AddRange(result.Select(item => SchedulingMapper.MapModelToViewModel(item)));
                    return result;
                }
            }
            catch (Exception)
            {
                return listOfPatientScheduling;
            }
        }

        /// <summary>
        /// Gets the scheduling by physicians data.
        /// </summary>
        /// <param name="physicianIdlist">The physician idlist.</param>
        /// <param name="selectedDate">The selected date.</param>
        /// <param name="viewtype">The viewtype.</param>
        /// <returns></returns>
        public List<SchedulingCustomModel> GetSchedulingByPhysiciansData(string physicianIdlist, DateTime selectedDate, string viewtype, int facilityid)
        {
            var listOfPatientScheduling = new List<SchedulingCustomModel>();
            try
            {
                using (var rep = UnitOfWork.SchedulingRepository)
                {
                    var result = rep.GetSchedulingEvents(viewtype, selectedDate, physicianIdlist, facilityid);
                    //listOfPatientScheduling.AddRange(result.Select(item => SchedulingMapper.MapModelToViewModel(item)));
                    //return listOfPatientScheduling;
                    return result;
                }
            }
            catch (Exception)
            {
                return listOfPatientScheduling;
            }
        }

        /// <summary>
        /// Gets the dept opening days for physician.
        /// </summary>
        /// <param name="physicianId">The physician identifier.</param>
        /// <returns></returns>
        public string GetDeptOpeningDaysForPhysician(int physicianId)
        {
            var phyDpeartmentObj = new PhysicianBal().GetPhysicianById(physicianId);
            if (phyDpeartmentObj != null)
            {
                var phyDeptId = phyDpeartmentObj.FacultyDepartment;
                var deptTimmingobj = new DeptTimmingBal().GetDeptTimmingByDepartmentId(Convert.ToInt32(phyDeptId));
                var deptOpeningdays = deptTimmingobj.Select(x => x.OpeningDayId).ToList();
                var objToReturn = string.Join(",", deptOpeningdays);
                return objToReturn;
            }

            return string.Empty;
        }

        /// <summary>
        /// Method to get the data for Over View popup
        /// </summary>
        /// <param name="oSchedularOverViewCustomModel"></param>
        /// <returns></returns>
        public List<SchedularOverViewCustomModel> GetOverView(SchedularOverViewCustomModel oSchedularOverViewCustomModel)
        {
            var lstSchedularOverViewCustomModel = new List<SchedularOverViewCustomModel>();
            try
            {
                using (var rep = UnitOfWork.SchedulingRepository)
                {
                    var result = rep.GetOverView(oSchedularOverViewCustomModel);
                    return result;
                }
            }
            catch (Exception)
            {
                return lstSchedularOverViewCustomModel;
            }
        }


        public List<SchedulingCustomModel> GetiSchedulingData(string associatedId, DateTime selectedDate, string viewtype, bool isPatient)
        {
            var listOfPatientScheduling = new List<SchedulingCustomModel>();
            try
            {
                using (var rep = UnitOfWork.SchedulingRepository)
                {
                    var result = rep.GetiSchedulingEvents(associatedId, selectedDate, viewtype, isPatient);
                    return result;
                }
            }
            catch (Exception)
            {
                return listOfPatientScheduling;
            }
        }

        /// <summary>
        /// Saves the patient pre scheduling list.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public SchedulingCustomModelView SavePatientPreSchedulingList(List<SchedulingCustomModel> model)
        {
            using (var rep = UnitOfWork.SchedulingRepository)
            {
                var schedulingMapper = new SchedulingMapper();
                var schedulingObjList = new List<Scheduling>();
                schedulingObjList.AddRange(model.Select(schedulingMapper.MapViewModelToModel));
                if (schedulingObjList.Any())
                {
                    var schedulingBal = new SchedulingBal();
                    foreach (var item in schedulingObjList)
                    {
                        if (item.SchedulingId > 0)
                        {
                            if (item.IsRecurring == true)
                            {
                                schedulingBal.DeleteHolidayPlannerData(item.EventParentId, 0, Convert.ToInt32(item.SchedulingType), "");
                            }
                            else
                            {
                                schedulingBal.DeleteHolidayPlannerData(string.Empty, item.SchedulingId, Convert.ToInt32(item.SchedulingType), "");
                            }
                        }

                        rep.Create(item);

                        // if (item.IsRecurring == true)
                        // {
                        //    rep.CreateRecurringSchedularEvents(item.SchedulingId);
                        // }
                    }
                }

                var list = new SchedulingCustomModelView();
                return list;
            }
        }

        /// <summary>
        /// Gets the pre scheduling list.
        /// </summary>
        /// <param name="cId">The c identifier.</param>
        /// <param name="fId">The f identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public List<SchedulingCustomModel> GetPreSchedulingList(int cId, int fId)
        {
            using (var rep = UnitOfWork.SchedulingRepository)
            {
                var schedulingMapper = new SchedulingMapper();
                var listroConvert = fId != 0
                                        ? rep.Where(
                                            x => x.CorporateId == cId && x.FacilityId == fId && x.Status == "90")
                                              .ToList()
                                        : rep.Where(
                                            x => x.CorporateId == cId && x.Status == "90")
                                              .ToList();
                var schedulingObjList = new List<SchedulingCustomModel>();
                schedulingObjList.AddRange(listroConvert.Select(schedulingMapper.MapModelToViewModel));
                return schedulingObjList;
            }
        }

        /// <summary>
        /// Gets the phy previous vacations.
        /// </summary>
        /// <param name="facilityid">The facilityid.</param>
        /// <param name="physicianId">The physician identifier.</param>
        /// <returns></returns>
        public List<SchedulingCustomModel> GetPhyPreviousVacations(int facilityid, int physicianId)
        {
            var listOfPhysicianHoidays = new List<SchedulingCustomModel>();
            try
            {
                using (var rep = UnitOfWork.SchedulingRepository)
                {
                    listOfPhysicianHoidays = rep.GetPhyPreviousVacations(facilityid, physicianId);
                    return listOfPhysicianHoidays;
                }
            }
            catch (Exception)
            {
                return listOfPhysicianHoidays;
            }
        }

        /// <summary>
        /// Gets the facility holidays.
        /// </summary>
        /// <param name="facilityid">The facilityid.</param>
        /// <returns></returns>
        public List<SchedulingCustomModel> GetFacilityHolidays(int facilityid)
        {
            var listOfFacilityHoidays = new List<SchedulingCustomModel>();
            try
            {
                using (var rep = UnitOfWork.SchedulingRepository)
                {
                    listOfFacilityHoidays = rep.GetFacilityHolidays(facilityid);
                    return listOfFacilityHoidays;
                }
            }
            catch (Exception)
            {
                return listOfFacilityHoidays;
            }
        }

        public bool DeleteHolidaysByEventParentID(string eventParentId)
        {
            try
            {
                using (var rep = UnitOfWork.SchedulingRepository)
                {
                    var valueToDelete = rep.Where(x => x.EventParentId.Equals(eventParentId));
                    rep.Delete(valueToDelete);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }







        public RoomEquipmentAvialability GetAssignedRoomForProcedure(int facilityId, int appointmentType, DateTime scheduledDate, string timeFrom, string timeTo, int roomId, int schedulingId, int pId)
        {
            using (var rep = UnitOfWork.SchedulingRepository)
            {
                var roomaviableList = rep.CheckRoomsAndEquipments(facilityId, appointmentType, scheduledDate, timeFrom,
                    timeTo, roomId, schedulingId, pId);
                return roomaviableList.FirstOrDefault();
            }
        }


        /// <summary>
        /// Gets the type of the scheduling dept data by.
        /// </summary>
        /// <param name="deptList">The dept identifier.</param>
        /// <param name="selectedDate">The selected date.</param>
        /// <param name="type">The type.</param>
        /// <param name="facilityId"></param>
        /// <returns></returns>
        public List<SchedulingCustomModel> GetSchedulingDataByDepartments(List<string> deptList, DateTime selectedDate, string type, int facilityId)
        {
            var listOfDeptScheduling = new List<SchedulingCustomModel>();
            try
            {
                using (var rep = UnitOfWork.SchedulingRepository)
                {
                    var schedulingObj =
                        rep.Where(
                            x => (deptList.Count == 0 || deptList.Contains(x.ExtValue1)) && x.SchedulingType == type
                                 && x.FacilityId == facilityId
                                 && x.ScheduleTo.HasValue &&
                                 (x.ScheduleFrom.HasValue &&
                                  (x.ScheduleFrom.Value.Date == selectedDate.Date
                                   || x.ScheduleTo.Value.Date == selectedDate.Date)))
                            .ToList();

                    listOfDeptScheduling.AddRange(schedulingObj.Select(item => SchedulingMapper.MapModelToViewModel(item)));
                    return listOfDeptScheduling;
                }
            }
            catch (Exception)
            {
                return listOfDeptScheduling;
            }
        }



        public List<SchedulingCustomModel> GetSchedulingDataByRooms(List<int> roomsList, DateTime selectedDate)
        {
            var listOfDeptScheduling = new List<SchedulingCustomModel>();
            try
            {
                using (var rep = UnitOfWork.SchedulingRepository)
                {
                    var schedulingObj =
                        rep.Where(x => x.RoomAssigned.HasValue && roomsList.Contains(x.RoomAssigned.Value)
                                       && x.ScheduleTo != null &&
                                       x.SchedulingType == "1"
                                       &&
                                       (x.ScheduleFrom != null &&
                                        (x.ScheduleFrom.Value.ToShortDateString() ==
                                         selectedDate.ToShortDateString()
                                         ||
                                         x.ScheduleTo.Value.ToShortDateString() ==
                                         selectedDate.ToShortDateString()))).ToList();

                    listOfDeptScheduling.AddRange(schedulingObj.Select(item => SchedulingMapper.MapModelToViewModel(item)));
                    return listOfDeptScheduling;
                }
            }
            catch (Exception)
            {
                return listOfDeptScheduling;
            }
        }



        public List<SchedulerCustomModelForCalender> GetSchedulerData(int viewType, DateTime selectedDate, string phyIds, int fId, string depIds, string roomIds, string statusIds, string sectionType, int patientId)
        {
            using (var rep = UnitOfWork.SchedulingRepository)
            {
                var result = rep.GetSchedulerDataUpdated(viewType, selectedDate, phyIds, fId, depIds, roomIds, statusIds,
                    sectionType, patientId);
                return result;
            }
        }


        public bool RemoveJustDeletedSchedulings(string eventParentid, List<int> listSchIds, int schedulingType, string extValue3)
        {
            try
            {
                using (var rep = UnitOfWork.SchedulingRepository)
                {
                    var mList = !string.IsNullOrEmpty(eventParentid) ? rep.Where(x => x.EventParentId == eventParentid).ToList() :
                        rep.Where(x => listSchIds.Contains(x.SchedulingId)).ToList();

                    mList = schedulingType == 1
                        ? mList.Where(x => x.ScheduleTo > DateTime.Now).ToList()
                        : mList.Where(x => x.ScheduleFrom.HasValue && x.ScheduleFrom.Value.Date >= DateTime.Now.Date &&
                                           x.ExtValue3 == extValue3).ToList();

                    if (mList.Count > 0)
                    {
                        rep.Delete(mList);
                        return true;
                    }

                    return false;

                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<SchedulerCustomModelForCalender> GetSchedulerData(int viewType, DateTime selectedDate, string phyIds, int fId, string depIds, string roomIds, string statusIds, string sectionType, int patientId, out List<SchedulingCustomModel> nextList)
        {
            using (var rep = UnitOfWork.SchedulingRepository)
            {
                var result = rep.GetSchedulerDataUpdated(viewType, selectedDate, phyIds, fId, depIds, roomIds, statusIds,
                                    sectionType, patientId, out nextList);
                return result;
            }
        }


        public int SavePatientInfoInScheduler(int cId, int fId, DateTime pDate, int pId, string firstName, string lastName, DateTime? dob, string email, string emirateId, int loggedUserId, string phone, int age, string newPwd = "")
        {
            using (var rep = UnitOfWork.SchedulingRepository)
            {
                var patientId = rep.SavePatientInfoInScheduler(cId, fId, pDate, pId, firstName, lastName, dob, email, emirateId, loggedUserId, phone, age, newPwd);
                return patientId;
            }
        }


        public bool AddUpdatePatientScheduling(List<SchedulingCustomModel> model, int facilityId, List<int> tobedeleted)
        {
            var result = false;
            using (var rep = UnitOfWork.SchedulingRepository)
            {
                var mList = new List<Scheduling>();
                var data = new List<SchCreatedData>();

                /* %%%%---- Delete Operation STARTS---%%%% */
                var deletedList = rep.Where(s => tobedeleted.Contains(s.SchedulingId)).ToList();
                if (deletedList.Count > 0)
                    rep.Delete(deletedList);
                /* %%%%---- Delete Operation ENDS---%%%% */

                mList.AddRange(model.Select(item => SchedulingMapper.MapViewModelToModel(item)));
                if (mList.Any())
                {
                    /* %%%%---- Update Operation STARTS---%%%% */
                    var updateList = mList.Where(s => s.SchedulingId > 0).ToList();
                    if (updateList.Count > 0)
                    {
                        var ids = updateList.Select(a => a.SchedulingId).ToList();
                        data = GetCreatedDateByIds(ids, facilityId);

                        updateList.ForEach(u => data.Where(a1 => a1.Id == u.SchedulingId).ToList().ForEach(a =>
                        {
                            a.IsRecurring = (u.IsRecurring == true);
                            a.IsNew = false;
                            u.CreatedBy = a.CreatedBy;
                            u.CreatedDate = a.CreatedDate;
                        }));

                        foreach (var u in updateList)
                            rep.UpdateEntity(u, u.SchedulingId);

                        //rep.Update(updateList);
                    }
                    /* %%%%---- Update Operation ENDS---%%%% */


                    /* %%%%---- Create Operation starts---%%%% */
                    var allNewlist = mList.Where(a => a.SchedulingId == 0).ToList();

                    if (allNewlist.Count > 0)
                        rep.Create(allNewlist);

                    data.AddRange(allNewlist.Select(item => new SchCreatedData
                    {
                        Id = item.SchedulingId,
                        IsRecurring = (item.IsRecurring == true),
                        IsNew = true
                    }));
                    /* %%%%---- Create Operation ends---%%%% */



                    /* %%%%---- Create Recurring Operation STARTS---%%%% */
                    var rList = data.Where(a => a.IsRecurring && model.Any(a1 => a1.SchedulingId == a.Id && a1.UpdateFlag))
                            .Select(f => f.Id)
                            .ToList();
                    if (rList.Count > 0)
                    {
                        var ids = string.Join(",", rList);
                        var st = rep.CreateRecurringSchedularEvents(ids, facilityId);
                        result = st > 0;
                    }
                    /* %%%%---- Create Recurring Operation ENDS---%%%% */
                }
            }
            return result;
        }


        private List<SchCreatedData> GetCreatedDateByIds(List<int> ids, int facilitId)
        {
            using (var rep = UnitOfWork.SchedulingRepository)
            {
                var dt = GetInvariantCultureDateTime(facilitId);
                var mList = rep.Where(s => ids.Contains(s.SchedulingId));
                if (mList.Any())
                {
                    var list = mList.Select(a => new SchCreatedData
                    {
                        Id = a.SchedulingId,
                        CreatedBy = a.CreatedBy ?? 1,
                        CreatedDate = a.CreatedDate ?? dt
                    }).ToList();
                    return list;
                }
                return new List<SchCreatedData>();
            }
        }

        public List<SchedulingCustomModel> ValidateScheduling(DataTable dt, int facilityId, int userId, out int status)
        {
            using (var rep = UnitOfWork.SchedulingRepository)
                return rep.ValidateSchedulingAppointment(dt, facilityId, userId, out status);
        }

        public bool UpdateAppointmentStatus(long schedulingId, string status, int userId, DateTime currentDatetime)
        {
            using (var rep = UnitOfWork.SchedulingRepository)
            {
                var current = rep.GetSingle(schedulingId);
                if (current != null)
                {
                    current.Status = status;
                    current.ModifiedBy = userId;
                    current.ModifiedDate = currentDatetime;

                    var result = rep.UpdateEntity(current, schedulingId);
                    return result >= 0;
                }
            }
            return false;
        }


        public List<Scheduling> MapVMToModel(List<SchedulingCustomModel> vm)
        {
            if (vm != null)
                return vm.Select(a => SchedulingMapper.MapViewModelToModel(a)).ToList();

            return Enumerable.Empty<Scheduling>().ToList();
        }

    }

    public class SchCreatedData
    {
        public int Id { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsRecurring { get; set; }
        public bool IsNew { get; set; }
    }
}