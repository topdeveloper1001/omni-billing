// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SchedulingRepository.cs" company="Spadez">
//   Omni Health care
// </copyright>
// <summary>
//   The scheduling repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Data;
using BillingSystem.Repository.Common;

namespace BillingSystem.Repository.GenericRepository
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.SqlClient;
    using System.Linq;

    using BillingSystem.Common.Common;
    using Model;
    using Model.CustomModel;
    using BillingSystem.Model.EntityDto;
    using System.Threading.Tasks;
    using BillingSystem.Common;
    using System.Globalization;
    using BillingSystem.Common.Requests;

    /// <summary>
    /// The scheduling repository.
    /// </summary>
    public class SchedulingRepository : GenericRepository<Scheduling>
    {
        #region Fields

        /// <summary>
        /// The _context.
        /// </summary>
        private readonly DbContext _context;

        public SqlParameter[] Params { get; private set; }

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulingRepository"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public SchedulingRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
            _context = context;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Sends the e claims.
        /// </summary>
        /// <param name="facilityId">
        /// The facility identifier.
        /// </param>
        /// <param name="appointmentType">
        /// Type of the appointment.
        /// </param>
        /// <param name="scheduledDate">
        /// The scheduled date.
        /// </param>
        /// <param name="timeFrom">
        /// The time from.
        /// </param>
        /// <param name="timeTo">
        /// The time to.
        /// </param>
        /// <param name="roomId">
        /// The room Id.
        /// </param> 
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<RoomEquipmentAvialability> CheckRoomsAndEquipmentsForScheduling(
            int facilityId,
            int appointmentType,
            DateTime scheduledDate,
            string timeFrom,
            string timeTo,
            Int32 roomId)
        {
            try
            {
                if (_context != null)
                {
                    var spName =
                        string.Format(
                            "EXEC {0} @FacilityId, @AppointmentType, @ScheduledDate, @TimeFrom, @TimeTo, @AvailableRoom",
                            StoredProcedures.SPROC_CheckRoomsAndEquipmentsForScheduling_New);
                    var sqlParameters = new SqlParameter[6];
                    sqlParameters[0] = new SqlParameter("FacilityId", facilityId);
                    sqlParameters[1] = new SqlParameter("AppointmentType", appointmentType);
                    sqlParameters[2] = new SqlParameter("ScheduledDate", scheduledDate);
                    sqlParameters[3] = new SqlParameter("TimeFrom", timeFrom);
                    sqlParameters[4] = new SqlParameter("TimeTo", timeTo);
                    sqlParameters[5] = new SqlParameter("AvailableRoom", roomId);
                    IEnumerable<RoomEquipmentAvialability> result = _context.Database.SqlQuery<RoomEquipmentAvialability>(
                        spName,
                        sqlParameters);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return null;
        }


        /// <summary>
        /// Checks the time slot availability for scheduling.
        /// </summary>
        /// <param name="schedulingId">The scheduling identifier.</param>
        /// <param name="selectedDate">The selected date.</param>
        /// <param name="timeFrom">The time from.</param>
        /// <param name="timeTo">The time to.</param>
        /// <param name="userid">The userid.</param>
        /// <param name="physicianid">The physicianid.</param>
        /// <param name="facilityid">The facilityid.</param>
        /// <returns></returns>
        public List<TimeSlotAvailabilityCustomModel> CheckTimeSlotAvailabilityForScheduling(
            int schedulingId,
            string schedulingType,
            DateTime selectedDate,
            string timeFrom,
            string timeTo,
            int userid,
            int physicianid,
            int facilityid)
        {
            try
            {
                if (_context != null)
                {
                    var spName =
                        string.Format(
                            "EXEC {0} @pSchedulingId, @pSchedulingType, @pSelectedDate, @TimeFrom, @TimeTo, @pUserid, @pPhysicianid, @pFacilityid",
                            StoredProcedures.SPROC_GetTimeSlotAvailablity);
                    var sqlParameters = new SqlParameter[8];
                    sqlParameters[0] = new SqlParameter("pSchedulingId", schedulingId);
                    sqlParameters[1] = new SqlParameter("pSchedulingType", schedulingType);
                    sqlParameters[2] = new SqlParameter("pSelectedDate", selectedDate);
                    sqlParameters[3] = new SqlParameter("TimeFrom", timeFrom);
                    sqlParameters[4] = new SqlParameter("TimeTo", timeTo);
                    sqlParameters[5] = new SqlParameter("pUserid", userid);
                    sqlParameters[6] = new SqlParameter("pPhysicianid", physicianid);
                    sqlParameters[7] = new SqlParameter("pFacilityid", facilityid);
                    IEnumerable<TimeSlotAvailabilityCustomModel> result = _context.Database.SqlQuery<TimeSlotAvailabilityCustomModel>(
                        spName, sqlParameters);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return null;
        }


        /// <summary>
        /// Checks the time slot availability recurring for scheduling.
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
        public List<TimeSlotAvailabilityCustomModel> CheckTimeSlotAvailabilityRecurringForScheduling(
            DateTime startDate, DateTime endDate, string timeFrom, string timeTo, int pFacilityid, int pSchedulingId, int pPhysicianid, string pRecPattern)
        {
            try
            {
                if (_context != null)
                {
                    var spName =
                        string.Format(
                            "EXEC {0} @StartDate, @EndDate, @TimeFrom, @TimeTo, @pFacilityid, @pSchedulingId, @pPhysicianid, @pRec_Pattern",
                            StoredProcedures.SPROC_GetTimeSlotAvialablotyForRecurrance);
                    var sqlParameters = new SqlParameter[8];
                    sqlParameters[0] = new SqlParameter("StartDate", startDate);
                    sqlParameters[1] = new SqlParameter("EndDate", endDate);
                    sqlParameters[2] = new SqlParameter("TimeFrom", timeFrom);
                    sqlParameters[3] = new SqlParameter("TimeTo", timeTo);
                    sqlParameters[4] = new SqlParameter("pFacilityid", pFacilityid);
                    sqlParameters[5] = new SqlParameter("pSchedulingId", pSchedulingId);
                    sqlParameters[6] = new SqlParameter("pPhysicianid", pPhysicianid);
                    sqlParameters[7] = new SqlParameter("pRec_Pattern", pRecPattern);
                    IEnumerable<TimeSlotAvailabilityCustomModel> result = _context.Database.SqlQuery<TimeSlotAvailabilityCustomModel>(
                        spName, sqlParameters);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return null;
        }


        /// <summary>
        /// Gets the available time slots.
        /// </summary>
        /// <param name="facilityid">The facilityid.</param>
        /// <param name="physicianId">The physician identifier.</param>
        /// <param name="dateselected">The dateselected.</param>
        /// <param name="typeofproc">The typeofproc.</param>
        /// <returns></returns>
        public IEnumerable<AvailabilityTimeSlotForPopupCustomModel> GetAvailableTimeSlots(int facilityid, int physicianId, DateTime dateselected, string typeofproc, out DateTime timeSlotDate, bool firstAvailable = false)
        {
            timeSlotDate = DateTime.Now;
            try
            {
                var sqlParameters = new SqlParameter[6];
                sqlParameters[0] = new SqlParameter("pFromDate", dateselected);
                sqlParameters[1] = new SqlParameter("pToDate", dateselected);
                sqlParameters[2] = new SqlParameter("pAppointMentType", typeofproc);
                sqlParameters[3] = new SqlParameter("pFacilityId", facilityid);
                sqlParameters[4] = new SqlParameter("pPhysicianId", physicianId);
                sqlParameters[5] = new SqlParameter("pFirst", firstAvailable);

                using (var multiResultSet = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetAvailableTimeSlots.ToString(), isCompiled: false, parameters: sqlParameters))
                {
                    var mainList = multiResultSet.ResultSetFor<AvailabilityTimeSlotForPopupCustomModel>().ToList();

                    if (firstAvailable)
                        timeSlotDate = multiResultSet.ResultSetFor<DateTime>().FirstOrDefault();

                    return mainList;
                }
            }
            catch (Exception)
            {
                //throw ex;
            }

            var dummyList = new List<AvailabilityTimeSlotForPopupCustomModel> { new AvailabilityTimeSlotForPopupCustomModel { TimeSlot = "0" } };
            return dummyList;
        }

        /// <summary>
        /// Gets the scheduling events.
        /// </summary>
        /// <param name="viewType">Type of the view.</param>
        /// <param name="selectedDate">The selected date.</param>
        /// <param name="physicianIdList">The physician identifier list.</param>
        /// <returns></returns>
        public List<SchedulingCustomModel> GetSchedulingEvents(
            string viewType,
            DateTime selectedDate,
            string physicianIdList, int facilityid)
        {
            try
            {
                if (_context != null)
                {
                    var spName =
                        string.Format(
                            "EXEC {0} @ViewType, @selectedDate, @PhysicianIdlist,@pfacilityId",
                            StoredProcedures.SPROC_GetSchedulingEvents_Back);
                    var sqlParameters = new SqlParameter[4];
                    sqlParameters[0] = new SqlParameter("ViewType", viewType);
                    sqlParameters[1] = new SqlParameter("selectedDate", selectedDate);
                    sqlParameters[2] = new SqlParameter("PhysicianIdlist", physicianIdList);
                    sqlParameters[3] = new SqlParameter("pfacilityId", facilityid);
                    IEnumerable<SchedulingCustomModel> result = _context.Database.SqlQuery<SchedulingCustomModel>(
                        spName,
                        sqlParameters);
                    return result.ToList();
                }
            }
            catch (Exception)
            {
            }

            return null;
        }

        /// <summary>
        /// Gets the patient scheduling events.
        /// </summary>
        /// <param name="viewType">Type of the view.</param>
        /// <param name="selectedDate">The selected date.</param>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public List<SchedulingCustomModel> GetPatientSchedulingEvents(string viewType, DateTime selectedDate, int patientId)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @ViewType, @selectedDate, @PatientId",
                        StoredProcedures.SPROC_GetPatientSchedulingEvents);
                    var sqlParameters = new SqlParameter[3];
                    sqlParameters[0] = new SqlParameter("ViewType", viewType);
                    sqlParameters[1] = new SqlParameter("selectedDate", selectedDate);
                    sqlParameters[2] = new SqlParameter("PatientId", patientId);
                    IEnumerable<SchedulingCustomModel> result = _context.Database.SqlQuery<SchedulingCustomModel>(spName, sqlParameters);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return null;
        }

        /// <summary>
        /// Gets the patient next scheduling events.
        /// </summary>
        /// <param name="selectedDate">The selected date.</param>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public List<SchedulingCustomModel> GetPatientNextSchedulingEvents(
            DateTime selectedDate,
            int patientId)
        {
            try
            {
                if (_context != null)
                {
                    var spName =
                        string.Format(
                            "EXEC {0} @selectedDate, @PatientId",
                            StoredProcedures.SPROC_GetPatientNextSchedulingEvents);
                    var sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("selectedDate", selectedDate);
                    sqlParameters[1] = new SqlParameter("PatientId", patientId);
                    IEnumerable<SchedulingCustomModel> result = _context.Database.SqlQuery<SchedulingCustomModel>(
                        spName,
                        sqlParameters);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return null;
        }


        /// <summary>
        /// Creates the recurring schedular events.
        /// </summary>
        /// <param name="pSchedulingId">The p scheduling identifier.</param>
        public void CreateRecurringSchedularEvents(
            int pSchedulingId)
        {
            try
            {
                if (_context != null)
                {
                    var spName =
                        string.Format(
                            "EXEC {0} @pSchedulingId",
                            StoredProcedures.SPROC_CreateRecurringEventsSchedular);
                    var sqlParameters = new SqlParameter[1];
                    sqlParameters[0] = new SqlParameter("pSchedulingId", pSchedulingId);
                    ExecuteCommand(spName, sqlParameters);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Creates the recurring schedular holidays.
        /// </summary>
        /// <param name="pSchedulingIds">The p scheduling ids.</param>
        /// <param name="pMulitpleProcedure">The p mulitple procedure.</param>
        /// <param name="pAssociatedId">The p associated identifier.</param>
        /// <returns></returns>
        public List<SkippedHolidaysData> CreateRecurringSchedularHolidays(
            string pSchedulingIds, string pMulitpleProcedure, string pAssociatedId)
        {
            try
            {
                if (_context != null)
                {
                    var spName =
                        "EXEC SPROC_CreateFacilityHolidays_V1 @pSchedulingIds,@pMulitpleProcedure,@pAssociatedId ";
                    var sqlParameters = new SqlParameter[3];
                    sqlParameters[0] = new SqlParameter("pSchedulingIds", pSchedulingIds);
                    sqlParameters[1] = new SqlParameter("pMulitpleProcedure", pMulitpleProcedure);
                    sqlParameters[2] = new SqlParameter("pAssociatedId", Convert.ToInt32(pAssociatedId));

                    // ExecuteCommand(spName, sqlParameters);
                    //IEnumerable<SkippedHolidaysData> result = this._context.Database.SqlQuery<SkippedHolidaysData>(
                    //    spName, sqlParameters);
                    IEnumerable<SkippedHolidaysData> result = _context.Database.SqlQuery<SkippedHolidaysData>(
                        spName, sqlParameters);
                    return result.ToList();
                }
            }
            catch (Exception)
            {
            }
            return null;
        }

        /// <summary>
        /// Method to get the data for Over View popup
        /// </summary>
        /// <param name="oSchedularOverViewCustomModel"></param>
        /// <returns></returns>
        public List<SchedularOverViewCustomModel> GetOverView(SchedularOverViewCustomModel oSchedularOverViewCustomModel)
        {
            try
            {
                if (_context != null)
                {
                    var spName =
                        string.Format(
                            "EXEC {0} @StartDate, @EndDate, @StartTime, @EndTime, @AvgTimeSlot, @PassedPhysician, @PassedRoom, @DepartmentID, @FacilityID, @ViewType",
                            StoredProcedures.AvailableSlotsMonthlyView_V1);
                    var sqlParameters = new SqlParameter[10];
                    sqlParameters[0] = new SqlParameter("StartDate", oSchedularOverViewCustomModel.FromDate);
                    sqlParameters[1] = new SqlParameter("EndDate", oSchedularOverViewCustomModel.ToDate ?? "01/01/01");
                    sqlParameters[2] = new SqlParameter("StartTime", oSchedularOverViewCustomModel.FromTime);
                    sqlParameters[3] = new SqlParameter("EndTime", oSchedularOverViewCustomModel.ToTime);
                    sqlParameters[4] = new SqlParameter("AvgTimeSlot", oSchedularOverViewCustomModel.TimeSlotFrequency);
                    sqlParameters[5] = new SqlParameter("PassedPhysician", oSchedularOverViewCustomModel.Physician);
                    sqlParameters[6] = new SqlParameter("PassedRoom", oSchedularOverViewCustomModel.Room);
                    sqlParameters[7] = new SqlParameter("DepartmentID", oSchedularOverViewCustomModel.DepartmentId);
                    sqlParameters[8] = new SqlParameter("FacilityID", oSchedularOverViewCustomModel.FacilityId);
                    sqlParameters[9] = new SqlParameter("ViewType", oSchedularOverViewCustomModel.ViewType);
                    IEnumerable<SchedularOverViewCustomModel> result = _context.Database.SqlQuery<SchedularOverViewCustomModel>(
                        spName,
                        sqlParameters);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return null;
        }



        public List<SchedulingCustomModel> GetiSchedulingEvents(string associatedId, DateTime selectedDate, string viewtype, bool isPatient)
        {
            try
            {
                if (_context != null)
                {
                    var spName =
                        string.Format(
                            "EXEC {0} @ViewType, @selectedDate, @AssociatedId,@IsPatient",
                            StoredProcedures.SPROC_GetPhyVacationsEvents);
                    var sqlParameters = new SqlParameter[4];
                    sqlParameters[0] = new SqlParameter("ViewType", viewtype);
                    sqlParameters[1] = new SqlParameter("selectedDate", selectedDate);
                    sqlParameters[2] = new SqlParameter("AssociatedId", associatedId);
                    sqlParameters[3] = new SqlParameter("IsPatient", isPatient);
                    IEnumerable<SchedulingCustomModel> result = _context.Database.SqlQuery<SchedulingCustomModel>(
                        spName, sqlParameters);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return null;
        }
        #endregion

        /// <summary>
        /// Gets the phy previous vacations.
        /// </summary>
        /// <param name="facilityid">The facilityid.</param>
        /// <param name="physicianId">The physician identifier.</param>
        /// <returns></returns>
        public List<SchedulingCustomModel> GetPhyPreviousVacations(int facilityid, int physicianId)
        {
            try
            {
                if (_context != null)
                {
                    var spName =
                        string.Format(
                            "EXEC {0} @facilityid, @physicianId",
                            StoredProcedures.SPROC_GetPhyVacationsEvents);
                    var sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("facilityid", facilityid);
                    sqlParameters[1] = new SqlParameter("physicianId", physicianId);
                    IEnumerable<SchedulingCustomModel> result = _context.Database.SqlQuery<SchedulingCustomModel>(
                        spName, sqlParameters);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return null;
        }

        /// <summary>
        /// Gets the facility holidays.
        /// </summary>
        /// <param name="facilityid">The facilityid.</param>
        /// <returns></returns>
        public List<SchedulingCustomModel> GetFacilityHolidays(int facilityid)
        {
            try
            {
                if (_context != null)
                {
                    var spName =
                        string.Format(
                            "EXEC {0} @facilityid",
                            StoredProcedures.SPROC_GetFacilityHolidays);
                    var sqlParameters = new SqlParameter[1];
                    sqlParameters[0] = new SqlParameter("facilityid", facilityid);
                    IEnumerable<SchedulingCustomModel> result = _context.Database.SqlQuery<SchedulingCustomModel>(
                        spName, sqlParameters);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return null;
        }

        public int ScheduleHolidays(string pSchedulingIds, string pMulitpleProcedure, string pAssociatedId)
        {
            try
            {
                if (_context != null)
                {
                    var spName =
                        "EXEC SPROC_CreateFacilityHolidays_V1 @pSchedulingIds,@pMulitpleProcedure,@pAssociatedId ";
                    var sqlParameters = new SqlParameter[3];
                    sqlParameters[0] = new SqlParameter("pSchedulingIds", pSchedulingIds);
                    sqlParameters[1] = new SqlParameter("pMulitpleProcedure", pMulitpleProcedure);
                    sqlParameters[2] = new SqlParameter("pAssociatedId", Convert.ToInt32(pAssociatedId));
                    var result = _context.Database.SqlQuery<int>(
                        spName, sqlParameters);
                    return result.FirstOrDefault();
                }
            }
            catch (Exception)
            {
            }
            return 0;
        }


        public List<RoomEquipmentAvialability> CheckRoomsAndEquipments(int facilityId, int appointmentType, DateTime scheduledDate, string timeFrom, string timeTo, Int32 roomId, int schId, int pId)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format(
                        "EXEC {0} @FacilityId, @AppointmentType, @ScheduledDate, @TimeFrom, @TimeTo, @AvailableRoom, @SchId ,@pId",
                        StoredProcedures.SprocCheckRoomsAndOthersAvailibilty);

                    var sqlParameters = new SqlParameter[8];
                    sqlParameters[0] = new SqlParameter("FacilityId", facilityId);
                    sqlParameters[1] = new SqlParameter("AppointmentType", appointmentType);
                    sqlParameters[2] = new SqlParameter("ScheduledDate", scheduledDate);
                    sqlParameters[3] = new SqlParameter("TimeFrom", timeFrom);
                    sqlParameters[4] = new SqlParameter("TimeTo", timeTo);
                    sqlParameters[5] = new SqlParameter("AvailableRoom", roomId);
                    sqlParameters[6] = new SqlParameter("SchId", schId);
                    sqlParameters[7] = new SqlParameter("pId", pId);

                    IEnumerable<RoomEquipmentAvialability> result = _context.Database.SqlQuery<RoomEquipmentAvialability>(
                        spName, sqlParameters);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return null;
        }


        public List<SchedulerCustomModelForCalender> GetSchedulerDataUpdated(int viewType, DateTime selectedDate, string phyIds, int fId, string depIds, string roomIds, string statusIds, string sectionType, int patientId)
        {
            try
            {
                if (_context != null)
                {
                    if (phyIds.Equals("0"))
                        phyIds = string.Empty;

                    if (roomIds.Equals("0"))
                        roomIds = string.Empty;

                    if (depIds.Equals("0"))
                        depIds = string.Empty;

                    if (statusIds.Equals("0"))
                        statusIds = string.Empty;


                    var spName = string.Format(
                        "EXEC {0} @pVT, @pDate, @pPhyIds, @pFId, @pDIds, @pRoomIds, @pStatusIds, @pSectionType, @pPatientId",
                        StoredProcedures.SPROC_GetSchedulerDataUpdated);

                    var sqlParameters = new SqlParameter[9];
                    sqlParameters[0] = new SqlParameter("pVT", viewType);
                    sqlParameters[1] = new SqlParameter("pDate", selectedDate);
                    sqlParameters[2] = new SqlParameter("pPhyIds", phyIds);
                    sqlParameters[3] = new SqlParameter("pFId", fId);
                    sqlParameters[4] = new SqlParameter("pDIds", depIds);
                    sqlParameters[5] = new SqlParameter("pRoomIds", roomIds);
                    sqlParameters[6] = new SqlParameter("pStatusIds", statusIds);
                    sqlParameters[7] = new SqlParameter("pSectionType", sectionType);
                    sqlParameters[8] = new SqlParameter("@pPatientId", patientId);

                    //We Assume here: _context is your EF DbContext
                    using (var multiResultSet = _context.MultiResultSetSqlQuery(spName, parameters: sqlParameters))
                    {
                        var mainList = multiResultSet.ResultSetFor<SchedulerCustomModelForCalender>().ToList();

                        if (mainList.Any())
                        {
                            var innerList = multiResultSet.ResultSetFor<TypeOfProcedureCustomModel>().ToList();

                            var parentId = string.Empty;
                            var list2 = new List<TypeOfProcedureCustomModel>();

                            foreach (var item in mainList)
                            {
                                if (string.IsNullOrEmpty(parentId) || !item.EventParentId.Equals(parentId))
                                    list2 = innerList.Where(p => p.EventParentId.Equals(item.EventParentId)).ToList();

                                item.TypeOfProcedureCustomModel = list2;
                            }
                        }
                        return mainList;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return null;
        }


        public List<SchedulerCustomModelForCalender> GetSchedulerDataUpdated(int viewType, DateTime selectedDate, string phyIds, int fId, string depIds, string roomIds, string statusIds, string sectionType, int patientId, out List<SchedulingCustomModel> nextList)
        {
            nextList = new List<SchedulingCustomModel>();
            try
            {
                if (_context != null)
                {
                    if (phyIds.Equals("0"))
                        phyIds = string.Empty;

                    if (roomIds.Equals("0"))
                        roomIds = string.Empty;

                    if (depIds.Equals("0"))
                        depIds = string.Empty;

                    if (statusIds.Equals("0"))
                        statusIds = string.Empty;


                    var spName = string.Format(
                        "EXEC {0} @pVT, @pDate, @pPhyIds, @pFId, @pDIds, @pRoomIds, @pStatusIds, @pSectionType, @pPatientId",
                        StoredProcedures.SPROC_GetPatientSchedulerDataUpdated);

                    var sqlParameters = new SqlParameter[9];
                    sqlParameters[0] = new SqlParameter("pVT", viewType);
                    sqlParameters[1] = new SqlParameter("pDate", selectedDate);
                    sqlParameters[2] = new SqlParameter("pPhyIds", phyIds);
                    sqlParameters[3] = new SqlParameter("pFId", fId);
                    sqlParameters[4] = new SqlParameter("pDIds", depIds);
                    sqlParameters[5] = new SqlParameter("pRoomIds", roomIds);
                    sqlParameters[6] = new SqlParameter("pStatusIds", statusIds);
                    sqlParameters[7] = new SqlParameter("pSectionType", sectionType);
                    sqlParameters[8] = new SqlParameter("@pPatientId", patientId);

                    //We Assume here: _context is your EF DbContext
                    using (var multiResultSet = _context.MultiResultSetSqlQuery(spName, parameters: sqlParameters))
                    {
                        var mainList = multiResultSet.ResultSetFor<SchedulerCustomModelForCalender>().ToList();

                        if (mainList.Any())
                        {
                            var innerList = multiResultSet.ResultSetFor<TypeOfProcedureCustomModel>().ToList();

                            var parentId = string.Empty;
                            var list2 = new List<TypeOfProcedureCustomModel>();

                            foreach (var item in mainList)
                            {
                                if (string.IsNullOrEmpty(parentId) || !item.EventParentId.Equals(parentId))
                                    list2 = innerList.Where(p => p.EventParentId.Equals(item.EventParentId)).ToList();

                                item.TypeOfProcedureCustomModel = list2;
                            }
                        }


                        nextList = multiResultSet.ResultSetFor<SchedulingCustomModel>().ToList();

                        return mainList;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return null;
        }

        public int SavePatientInfoInScheduler(int cId, int fId, DateTime pDate, int pId, string firstName, string lastName, DateTime? dob, string email, string emirateId, int loggedUserId, string phone, int age, string newPwd = "")
        {
            try
            {
                if (_context != null)
                {
                    var spName =
                        string.Format("EXEC {0} @pCId,@pFId,@pDate,@pPid,@pFirstName,@pLastName,@pDob,@pEmail,@pEmirates,@pLoggedInUserId,@pPhone,@pAge",
                        StoredProcedures.SavePatientInfoInScheduler);

                    var sqlParameters = new SqlParameter[13];
                    sqlParameters[0] = new SqlParameter("pCId", cId);
                    sqlParameters[1] = new SqlParameter("pFId", fId);
                    sqlParameters[2] = new SqlParameter("pDate", pDate);
                    sqlParameters[3] = new SqlParameter("pPid", pId);
                    sqlParameters[4] = new SqlParameter("pFirstName", firstName);
                    sqlParameters[5] = new SqlParameter("pLastName", lastName);
                    sqlParameters[6] = new SqlParameter("pDob", dob);
                    sqlParameters[7] = new SqlParameter("pEmail", email);
                    sqlParameters[8] = new SqlParameter("pEmirates", !string.IsNullOrEmpty(emirateId) ? emirateId : string.Empty);
                    sqlParameters[9] = new SqlParameter("pLoggedInUserId", loggedUserId);
                    sqlParameters[10] = new SqlParameter("pPhone", phone);
                    sqlParameters[11] = new SqlParameter("pAge", age);
                    sqlParameters[12] = new SqlParameter("pPwd", newPwd);
                    var result = _context.Database.SqlQuery<int>(spName, sqlParameters);
                    return result.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return 0;
        }

        public int CreateRecurringSchedularEvents(string ids, int facilityId)
        {
            var result = 0;
            try
            {
                if (_context != null)
                {
                    var spName =
                        string.Format("EXEC {0} @pSchedulingId", StoredProcedures.SPROC_CreateRecurringEventsSchedularMultiple);
                    var sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("pSchedulingIds", ids);
                    sqlParameters[1] = new SqlParameter("pFacilityId", facilityId);
                    ExecuteCommand(spName, sqlParameters);
                    result = _context.Database.SqlQuery<int>(spName, sqlParameters).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public List<SchedulingCustomModel> ValidateSchedulingAppointment(DataTable dt, int facilityId, int userId, out int status)
        {
            status = 0;
            try
            {
                var sqlParameters = new SqlParameter[3];
                sqlParameters[0] = new SqlParameter
                {
                    ParameterName = "pSchedulingList",
                    SqlDbType = SqlDbType.Structured,
                    Value = dt,
                    TypeName = "SchedulerArrayTT"
                };
                sqlParameters[1] = new SqlParameter("pFId", facilityId);
                sqlParameters[2] = new SqlParameter("pUserId", userId);

                //We Assume here: _context is your EF DbContext
                using (var ms = _context.MultiResultSetSqlQuery(StoredProcedures.SprocValidateAppointment.ToString(), isCompiled: false, parameters: sqlParameters))
                {
                    var mainList = ms.ResultSetFor<SchedulingCustomModel>().ToList();
                    status = ms.ResultSetFor<int>().FirstOrDefault();
                    return mainList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// Get the list of Time Slots available while booking an appointment.
        /// </summary>
        /// <param name="physicianId"></param>
        /// <param name="dateselected"></param>
        /// <param name="typeofproc"></param>
        /// <param name="firstAvailable"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TimeSlotsDto>> GetAvailableTimeSlotsAsync(long physicianId, DateTime dateselected, long typeofproc
            , long specialtyId, bool firstAvailable = false, long facilityId = 0, string timeFrom = "", string timeTo = "", int maxCount = 0)
        {
            var sqlParameters = new SqlParameter[10];
            sqlParameters[0] = new SqlParameter("pFromDate", dateselected);
            sqlParameters[1] = new SqlParameter("pToDate", dateselected);
            sqlParameters[2] = new SqlParameter("pAppointMentType", typeofproc);
            sqlParameters[3] = new SqlParameter("pPhysicianId", physicianId);
            sqlParameters[4] = new SqlParameter("pSpecialtyId", specialtyId);
            sqlParameters[5] = new SqlParameter("pFirst", firstAvailable);
            sqlParameters[6] = new SqlParameter("pTimeFrom", timeFrom);
            sqlParameters[7] = new SqlParameter("pTimeTill", timeTo);
            sqlParameters[8] = new SqlParameter("pMaxTimeSlotsCount", maxCount);
            sqlParameters[9] = new SqlParameter("pFacilityId", facilityId);

            using (var multiResultSet = _context.MultiResultSetSqlQuery(StoredProcsiOS.iSprocGetAvailableTimeSlots.ToString(), isCompiled: false, parameters: sqlParameters))
            {
                var result = await multiResultSet.GetResultWithJsonAsync<TimeSlotsDto>(JsonResultsArray.TimeSlotsData.ToString());
                //var result = await multiResultSet.GetJsonStringResult();
                return result;
            }
        }

        /// <summary>
        /// This action will save entry in the Appointment table and so, do the pre-booking in the system.
        /// </summary>
        /// <param name="a">Instance of the ApplicationDto</param>
        /// <returns>Result of the Database Operation</returns>
        public async Task<ResponseData> BookAnAppointmentAsync(AppointmentDto a)
        {
            var clinicianRefferedBy = a.ClinicianReferredBy ?? a.ClinicianId;
            var appDetails = !string.IsNullOrEmpty(a.AppointmentDetails) ? a.AppointmentDetails : string.Empty;
            var title = !string.IsNullOrEmpty(a.Title) ? a.Title : string.Empty;

            var rn = new Random(DateTime.Now.Ticks.GetHashCode());
            var eventParentId = Convert.ToString(Math.Abs(rn.Next(int.MinValue, int.MaxValue)));
            var eventId = $"{eventParentId}1";
            var weekDay = Convert.ToString(CommonHelper.GetWeekOfYearISO8601(a.ScheduleDate));
            var loginToken = CommonHelper.GenerateToken(8);

            var sqlParams = new SqlParameter[20];
            sqlParams[0] = new SqlParameter("@pId", a.Id);
            sqlParams[1] = new SqlParameter("@pPatientId", a.PatientId);
            sqlParams[2] = new SqlParameter("@pClinicianId", a.ClinicianId);
            sqlParams[3] = new SqlParameter("@pAppointmentTypeId", a.AppointmentTypeId);
            sqlParams[4] = new SqlParameter("@pSpecialty", a.Specialty);
            sqlParams[5] = new SqlParameter("@pFacilityId", a.FacilityId.GetValueOrDefault());
            sqlParams[6] = new SqlParameter("@pAppointmentDate", a.ScheduleDate);
            sqlParams[7] = new SqlParameter("@pTimeFrom", a.TimeFrom);
            sqlParams[8] = new SqlParameter("@pTimeTill", a.TimeTill);
            sqlParams[9] = new SqlParameter("@pCreatedBy", a.PatientId);
            sqlParams[10] = new SqlParameter("@pTitle", title);
            sqlParams[11] = new SqlParameter("@pClinicianReferredBy", clinicianRefferedBy);
            sqlParams[12] = new SqlParameter("@pAppDetails", appDetails);
            sqlParams[13] = new SqlParameter("@pCountryId", a.CountryId);
            sqlParams[14] = new SqlParameter("@pStateId", a.StateId);
            sqlParams[15] = new SqlParameter("@pCityId", a.CityId);
            sqlParams[16] = new SqlParameter("@pEventId", eventId);
            sqlParams[17] = new SqlParameter("@pEventParentId", eventParentId);
            sqlParams[18] = new SqlParameter("@pWeekDay", weekDay);
            sqlParams[19] = new SqlParameter("@pToken", loginToken);

            using (var ms = _context.MultiResultSetSqlQuery(StoredProcsiOS.iSprocBookAnAppointment.ToString(), isCompiled: false, parameters: sqlParams))
            {
                var result = (await ms.ResultSetForAsync<ResponseData>()).FirstOrDefault();
                return result;
            }
        }

        public async Task<IEnumerable<BookedAppointmentDto>> GetBookedAppointments(UpcomingAppointmentsRequest m)
        {
            var status = !string.IsNullOrEmpty(m.StatusId) ? m.StatusId : string.Empty;

            var sqlParams = new SqlParameter[8];
            sqlParams[0] = new SqlParameter("pPatientId", m.PatientId.GetValueOrDefault());
            sqlParams[1] = new SqlParameter("pUserId", m.LoggedInUserId.GetValueOrDefault());
            sqlParams[2] = new SqlParameter("pOnlyUpcoming", SqlDbType.Bit);

            if (m.ShowsUpcomingOnly.HasValue)
                sqlParams[2].Value = m.ShowsUpcomingOnly.Value;
            else
                sqlParams[2].Value = DBNull.Value;

            sqlParams[3] = new SqlParameter("pId", m.Id.GetValueOrDefault());
            sqlParams[4] = new SqlParameter("pForToday", m.ForToday.GetValueOrDefault());
            sqlParams[5] = new SqlParameter("pFacilityId", m.FacilityId.GetValueOrDefault());
            sqlParams[6] = new SqlParameter("pStatus", status);
            sqlParams[7] = new SqlParameter("pAppointmentDate", SqlDbType.DateTime);

            if (m.AppointmentDate.HasValue)
                sqlParams[7].Value = m.AppointmentDate.Value;
            else
                sqlParams[7].Value = DBNull.Value;

            using (var ms = _context.MultiResultSetSqlQuery(StoredProcsiOS.iSprocGetBookedAppointments.ToString(), isCompiled: false, parameters: sqlParams))
            {
                var result = (await ms.ResultSetForAsync<BookedAppointmentDto>()).ToList();

                if (result != null && result.Any())
                    result = m.ShowsUpcomingOnly.HasValue ?
                        result.OrderBy(d => d.AppointmentDate)
                        .ThenBy(t => DateTime.ParseExact(t.TimeFrom, "HH:mm", CultureInfo.InvariantCulture)).ToList()
                        : result.OrderByDescending(d => d.AppointmentDate)
                        .ThenByDescending(t => DateTime.ParseExact(t.TimeFrom, "HH:mm", CultureInfo.InvariantCulture)).ToList();

                return result;
            }
        }

        public async Task<IEnumerable<ClinicianDto>> GetCliniciansAndTheirTimeSlotsAsync(AvailableTimeSlotsRequest r)
        {
            var mainData = new List<ClinicianDto>();
            if (!TimeSpan.TryParse(r.TimeFrom, out TimeSpan fTime) || !TimeSpan.TryParse(r.TimeTill, out TimeSpan tTime))
            {
                r.TimeFrom = string.Empty;
                r.TimeTill = string.Empty;
            }

            var sqlParams = new SqlParameter[11];
            sqlParams[0] = new SqlParameter("pAppointmentDate", r.AppointmentDate);
            sqlParams[1] = new SqlParameter("pAppointMentType", r.AppointmentTypeId);
            sqlParams[2] = new SqlParameter("pPhysicianId", r.ClinicianId.GetValueOrDefault());
            sqlParams[3] = new SqlParameter("pSpecialtyId", r.SpecialtyId.GetValueOrDefault());
            sqlParams[4] = new SqlParameter("pFirst", r.IsFirst);
            sqlParams[5] = new SqlParameter("pCityId", r.CityId.GetValueOrDefault());
            sqlParams[6] = new SqlParameter("pTimeFrom", !string.IsNullOrEmpty(r.TimeFrom) ? r.TimeFrom.Trim() : string.Empty);
            sqlParams[7] = new SqlParameter("pTimeTill", !string.IsNullOrEmpty(r.TimeTill) ? r.TimeTill.Trim() : string.Empty);
            sqlParams[8] = new SqlParameter("pMaxTimeSlotsCount", r.RecordsCountRequested.GetValueOrDefault());
            sqlParams[9] = new SqlParameter("pFacilityId", r.FacilityId.GetValueOrDefault());
            sqlParams[10] = new SqlParameter("pStateId", r.StateId.GetValueOrDefault());

            using (var ms = _context.MultiResultSetSqlQuery(StoredProcsiOS.iSprocGetCliniciansAndTheirTimeSlots.ToString(), isCompiled: false, parameters: sqlParams))
                mainData = (await ms.GetResultWithJsonAsync<ClinicianDto>(JsonResultsArray.TimeSlotsData.ToString())).ToList();

            return mainData;
        }

        public async Task<ClinicianDetailDto> GetClinicianDetailsForBookingAnAppoinment(long clinicianId)
        {
            var sqlParams = new SqlParameter[1];
            sqlParams[0] = new SqlParameter("pClinicianId", clinicianId);

            using (var ms = _context.MultiResultSetSqlQuery(StoredProcsiOS.iSprocGetClinicianDetailsForRescheduling.ToString(), isCompiled: false, parameters: sqlParams))
            {
                var result = (await ms.GetResultWithJsonAsync<ClinicianDetailDto>(JsonResultsArray.ClinicanDetail.ToString())).FirstOrDefault();
                return result;
            }
        }

        public async Task<AppointmentDetailsDto> GetCliniciansAndSpecialitiesAsync(long appointmentTypeId, int cityId = 0, long facilityId = 0)
        {
            var sqlParams = new SqlParameter[3];
            sqlParams[0] = new SqlParameter("pAppointmentTypeId", appointmentTypeId);
            sqlParams[1] = new SqlParameter("pCityId", cityId);
            sqlParams[2] = new SqlParameter("pFacilityId", facilityId);

            using (var ms = _context.MultiResultSetSqlQuery(StoredProcsiOS.iSprocGetCliniciansAndSpecialtiesByAppType.ToString(), isCompiled: false, parameters: sqlParams))
            {
                var result = (await ms.GetResultWithJsonAsync<AppointmentDetailsDto>(JsonResultsArray.CliniciansData.ToString())).FirstOrDefault();
                return result;
            }
        }

        public async Task<string> GetCliniciansAndSpecialitiesAsync2(long appointmentTypeId, int cityId = 0, long facilityId = 0)
        {
            var sqlParams = new SqlParameter[3];
            sqlParams[0] = new SqlParameter("pAppointmentTypeId", appointmentTypeId);
            sqlParams[1] = new SqlParameter("pCityId", cityId);
            sqlParams[2] = new SqlParameter("pfacilityId", facilityId);

            using (var ms = _context.MultiResultSetSqlQuery(StoredProcsiOS.iSprocGetCliniciansAndSpecialtiesByAppType.ToString(), isCompiled: false, parameters: sqlParams))
            {
                var result = await ms.GetJsonStringResult();
                return result;
            }
        }


        public async Task<WeeklyScheduleDto> GetAppointmentsWeeklyScheduledDataAsync(long userId = 0, long facilityId = 0)
        {
            var sqlParams = new SqlParameter[2];
            sqlParams[0] = new SqlParameter("pUserId", userId);
            sqlParams[1] = new SqlParameter("pFacilityId", facilityId);

            using (var ms = _context.MultiResultSetSqlQuery(StoredProcsiOS.iSprocGetAppointmentsWeeklyScheduledData.ToString(), isCompiled: false, parameters: sqlParams))
            {
                var result = (await ms.GetResultWithJsonAsync<WeeklyScheduleDto>(JsonResultsArray.AppointmentWeeklyScheduledData.ToString())).FirstOrDefault();
                return result;
            }
        }
    }
}