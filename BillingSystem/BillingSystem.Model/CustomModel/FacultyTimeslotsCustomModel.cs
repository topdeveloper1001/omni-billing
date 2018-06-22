// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FacultyTimeslotsCustomModel.cs" company="SPadez">
//   OmniHealthcare
// </copyright>
// <summary>
//   The faculty timeslots custom model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Specialized;

namespace BillingSystem.Model.CustomModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    ///     The faculty timeslots custom model.
    /// </summary>
    [NotMapped]
    public class FacultyTimeslotsCustomModel : FacultyTimeslots
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the block date.
        /// </summary>
        /// <value>
        ///     The block date.
        /// </value>
        public DateTime BlockDate { get; set; }

        /// <summary>
        ///     Gets or sets the block time.
        /// </summary>
        /// <value>
        ///     The block time.
        /// </value>
        public string[] BlockTime { get; set; }

        /// <summary>
        ///     Gets or sets the name of the department.
        /// </summary>
        /// <value>
        ///     The name of the department.
        /// </value>
        public string DepartmentName { get; set; }

        /// <summary>
        ///     Gets or sets the dept closing time.
        /// </summary>
        /// <value>
        ///     The dept closing time.
        /// </value>
        public string DeptClosingTime { get; set; }

        /// <summary>
        ///     Gets or sets the dept opening days.
        /// </summary>
        /// <value>
        ///     The dept opening days.
        /// </value>
        public string DeptOpeningDays { get; set; }

        /// <summary>
        ///     Gets or sets the dept opening time.
        /// </summary>
        /// <value>
        ///     The dept opening time.
        /// </value>
        public string DeptOpeningTime { get; set; }

        /// <summary>
        ///     Gets or sets the faculty lunch time from.
        /// </summary>
        /// <value>
        ///     The faculty lunch time from.
        /// </value>
        public string FacultyLunchTimeFrom { get; set; }

        /// <summary>
        ///     Gets or sets the faculty lunch time till.
        /// </summary>
        /// <value>
        ///     The faculty lunch time till.
        /// </value>
        public string FacultyLunchTimeTill { get; set; }

        /// <summary>
        ///     Gets or sets the lunch end time.
        /// </summary>
        /// <value>
        ///     The lunch end time.
        /// </value>
        public string LunchEndTime { get; set; }

        /// <summary>
        ///     Gets or sets the lunch start time.
        /// </summary>
        /// <value>
        ///     The lunch start time.
        /// </value>
        public string LunchStartTime { get; set; }

        #endregion
    }

    /// <summary>
    ///     The scdeduler faculty timeslots custom model.
    /// </summary>
    [NotMapped]
    public class SchedulerFacultyTimeslotsCustomModel
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the availablility.
        /// </summary>
        /// <value>
        ///     The availablility.
        /// </value>
        public string Availability { get; set; }

        /// <summary>
        ///     Gets or sets the lunch time from.
        /// </summary>
        /// <value>
        ///     The lunch time from.
        /// </value>
        public string LunchTimeFrom { get; set; }

        /// <summary>
        ///     Gets or sets the faculty lunch time till.
        /// </summary>
        /// <value>
        ///     The faculty lunch time till.
        /// </value>
        public string LunchTimeTill { get; set; }

        /// <summary>
        /// </summary>
        public string PhySpeciality { get; set; }

        /// <summary>
        ///     Gets or sets the identifier.
        /// </summary>
        /// <value>
        ///     The identifier.
        /// </value>
        public int TimeSlotId { get; set; }

        /// <summary>
        /// </summary>
        public string VisitType { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="ScdedulerFacultyTimeslotsCustomModel" /> is _timed.
        /// </summary>
        /// <value>
        ///     <c>true</c> if _timed; otherwise, <c>false</c>.
        /// </value>
        public bool _timed { get; set; }

        /// <summary>
        ///     Gets or sets the color.
        /// </summary>
        /// <value>
        ///     The color.
        /// </value>
        public string color { get; set; }

        /// <summary>
        ///     Gets or sets the dept_ closingtime.
        /// </summary>
        /// <value>
        ///     The dept_ closingtime.
        /// </value>
        public string dept_Closingtime { get; set; }

        /// <summary>
        ///     Gets or sets the name of the dept_.
        /// </summary>
        /// <value>
        ///     The name of the dept_.
        /// </value>
        public string dept_Name { get; set; }

        /// <summary>
        ///     Gets or sets the dept_ opening days.
        /// </summary>
        /// <value>
        ///     The dept_ opening days.
        /// </value>
        public string dept_OpeningDays { get; set; }

        /// <summary>
        ///     Gets or sets the dept_ openingtime.
        /// </summary>
        /// <value>
        ///     The dept_ openingtime.
        /// </value>
        public string dept_Openingtime { get; set; }

        /// <summary>
        ///     Gets or sets the end_date.
        /// </summary>
        public string end_date { get; set; }

        /// <summary>
        ///     Gets or sets the event_length.
        /// </summary>
        /// <value>
        ///     The event_length.
        /// </value>
        public long event_length { get; set; }

        /// <summary>
        ///     Gets or sets the event_pid.
        /// </summary>
        /// <value>
        ///     The event_pid.
        /// </value>
        public int event_pid { get; set; }

        /// <summary>
        ///     Gets or sets the facility approved
        /// </summary>
        public string facilityapproved { get; set; }

        /// <summary>
        ///     Gets or sets the identifier.
        /// </summary>
        /// <value>
        ///     The identifier.
        /// </value>
        public long id { get; set; }

        /// <summary>
        /// </summary>
        public string location { get; set; }

        /// <summary>
        ///     Gets or sets the lunch end time.
        /// </summary>
        /// <value>
        ///     end time of lunch.
        /// </value>
        public string lunch_endtime { get; set; }

        /// <summary>
        ///     Gets or sets the lunch start time.
        /// </summary>
        /// <value>
        ///     start time of lunch.
        /// </value>
        public string lunch_starttime { get; set; }

        /// <summary>
        ///     Gets or sets the patient confirmed
        /// </summary>
        public string patientconfirmed { get; set; }

        /// <summary>
        /// </summary>
        public int patientid { get; set; }

        /// <summary>
        /// </summary>
        public int physicianid { get; set; }

        /// <summary>
        ///     Gets or sets the rec_pattern.
        /// </summary>
        /// <value>
        ///     The rec_pattern.
        /// </value>
        public string rec_pattern { get; set; }

        /// <summary>
        ///     Gets or sets the rec_type.
        /// </summary>
        /// <value>
        ///     The rec_type.
        /// </value>
        public string rec_type { get; set; }

        /// <summary>
        ///     Gets or sets the start_date.
        /// </summary>
        public string start_date { get; set; }

        /// <summary>
        ///     Gets or sets the text.
        /// </summary>
        /// <value>
        ///     The text.
        /// </value>
        public string text { get; set; }

        #endregion
    }

    /// <summary>
    ///     Faculty Time slots Custom Model View
    /// </summary>
    [NotMapped]
    public class FacultyTimeslotsCustomModelView
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the department opening slots list.
        /// </summary>
        /// <value>
        ///     The department opening slots list.
        /// </value>
        public List<FacultyTimeslotsCustomModel> DepartmentOpeningSlotsList { get; set; }

        /// <summary>
        ///     Gets or sets the faculty saved slots list.
        /// </summary>
        /// <value>
        ///     The faculty saved slots list.
        /// </value>
        public List<FacultyTimeslotsCustomModel> FacultySavedSlotsList { get; set; }

        #endregion
    }

    /// <summary>
    ///     Scheduler Faculty Time slots Custom Model View
    /// </summary>
    [NotMapped]
    public class SchedulerFacultyTimeslotsCustomModelView
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the department opening slots list.
        /// </summary>
        /// <value>
        ///     The department opening slots list.
        /// </value>
        public List<SchedulerFacultyTimeslotsCustomModel> DepartmentOpeningSlotsList { get; set; }

        /// <summary>
        ///     Gets or sets the faculty saved slots list.
        /// </summary>
        /// <value>
        ///     The faculty saved slots list.
        /// </value>
        public List<SchedulerFacultyTimeslotsCustomModel> FacultySavedSlotsList { get; set; }

        #endregion
    }

    /// <summary>
    ///     Schedular Type Custom Model
    /// </summary>
    [NotMapped]
    public class SchedularTypeCustomModel
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the facility.
        /// </summary>
        /// <value>
        ///     The facility.
        /// </value>
        public string Facility { get; set; }

        /// <summary>
        ///     Gets or sets the physician identifier.
        /// </summary>
        /// <value>
        ///     The physician identifier.
        /// </value>
        public List<SchedularFiltersCustomModel> PhysicianId { get; set; }

        /// <summary>
        ///     Gets or sets the selected date.
        /// </summary>
        /// <value>
        ///     The selected date.
        /// </value>
        public DateTime SelectedDate { get; set; }

        /// <summary>
        ///     Gets or sets the type of the status.
        /// </summary>
        /// <value>
        ///     The type of the status.
        /// </value>
        public List<SchedularFiltersCustomModel> StatusType { get; set; }

        /// <summary>
        /// Gets or sets the dept identifier.
        /// </summary>
        /// <value>
        /// The dept identifier.
        /// </value>
        public List<SchedularFiltersCustomModel> DeptData { get; set; }

        /// <summary>
        ///     Gets or sets the type of the view.
        /// </summary>
        /// <value>
        ///     The type of the view.
        /// </value>
        public string ViewType { get; set; }

        /// <summary>
        /// Gets or sets the patient identifier.
        /// </summary>
        /// <value>
        /// The patient identifier.
        /// </value>
        public int PatientId { get; set; }


        /// <summary>
        /// Gets or sets the room identifier.
        /// </summary>
        /// <value>
        /// The room identifier.
        /// </value>
        public List<SchedularFiltersCustomModel> RoomIds { get; set; }
        #endregion
    }

    /// <summary>
    ///     Schedular Filters Custom Model
    /// </summary>
    public class SchedularFiltersCustomModel
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the identifier.
        /// </summary>
        /// <value>
        ///     The identifier.
        /// </value>
        public int Id { get; set; }
        public string Name { get; set; }

        #endregion
    }

    /// <summary>
    /// Schedular over view custom model
    /// </summary>
    public class SchedularOverViewCustomModel
    {
        #region Returned Model from AvailableSlotsMonthlyView
        public int DPID { get; set; }
        public string PhysicianName { get; set; }
        public int PID { get; set; }
        public int RID { get; set; }
        public string STime { get; set; }
        public string ETime { get; set; }
        public string D1 { get; set; }
        public string D2 { get; set; }
        public string D3 { get; set; }
        public string D4 { get; set; }
        public string D5 { get; set; }
        public string D6 { get; set; }
        public string D7 { get; set; }
        public string D8 { get; set; }
        public string D9 { get; set; }
        public string D10 { get; set; }
        public string D11 { get; set; }
        public string D12 { get; set; }
        public string D13 { get; set; }
        public string D14 { get; set; }
        public string D15 { get; set; }
        public string D16 { get; set; }
        public string D17 { get; set; }
        public string D18 { get; set; }
        public string D19 { get; set; }
        public string D20 { get; set; }
        public string D21 { get; set; }
        public string D22 { get; set; }
        public string D23 { get; set; }
        public string D24 { get; set; }
        public string D25 { get; set; }
        public string D26 { get; set; }
        public string D27 { get; set; }
        public string D28 { get; set; }
        public string D29 { get; set; }
        public string D30 { get; set; }
        public string D31 { get; set; }
        #endregion
        #region Passing Model to AvailableSlotsMonthlyView
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string FromTime { get; set; }
        public string ToTime { get; set; }
        public string TimeSlotFrequency { get; set; }
        public string AppointmentType { get; set; }
        public int FacilityId { get; set; }
        public int DepartmentId { get; set; }
        public int CorporateId { get; set; }
        public int Physician { get; set; }
        public int Patient { get; set; }
        public int Room { get; set; }
        public int ViewType { get; set; }
        #endregion
    }
}