// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SchedulerFacultyTimeslotsCustomModel.cs" company="Spadez">
//   OmniHealthcare
// </copyright>
// <summary>
//   The scheduler faculty timeslots custom model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BillingSystem.Model.CustomModel
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    ///     The scheduler faculty timeslots custom model.
    /// </summary>
    [NotMapped]
    public class SchedulerCustomModelForCalender
    {
        #region Public Properties

        public string ExtValue5 { get; set; }

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
        /// Gets or sets the patient email id.
        /// </summary>
        public string PatientEmailId { get; set; }

        /// <summary>
        /// Gets or sets the patient name.
        /// </summary>
        public string PatientName { get; set; }

        /// <summary>
        /// Gets or sets the patient phone number.
        /// </summary>
        public string PatientPhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the patient dob.
        /// </summary>
        /// <value>
        /// The patient dob.
        /// </value>
        public string PatientDOB { get; set; }

        /// <summary>
        /// </summary>
        public string PhysicianSpeciality { get; set; }

        /// <summary>
        /// Gets or sets the physician comments.
        /// </summary>
        public string PhysicianComments { get; set; }

        /// <summary>
        ///     Gets or sets the identifier.
        /// </summary>
        /// <value>
        ///     The identifier.
        /// </value>
        public int TimeSlotId { get; set; }

        /// <summary>
        /// Gets or sets the time slot time interval.
        /// </summary>
        /// <value>
        /// The time slot time interval.
        /// </value>
        public string TimeSlotTimeInterval { get; set; }

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
        /// Gets or sets the name of the dept_.
        /// </summary>
        /// <value>
        /// The name of the dept_.
        /// </value>
        public string Department { get; set; }

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
        public long event_pid { get; set; }

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

        /// <summary>
        /// Gets or sets the section_id.
        /// </summary>
        /// <value>
        /// The section_id.
        /// </value>
        public string section_id { get; set; }

        /// <summary>
        /// Gets or sets the type of the scheduling.
        /// </summary>
        /// <value>
        /// The type of the scheduling.
        /// </value>
        public string SchedulingType { get; set; }

        /// <summary>
        /// Gets or sets the type of procedure custom model.
        /// </summary>
        /// <value>
        /// The type of procedure custom model.
        /// </value>
        public List<TypeOfProcedureCustomModel> TypeOfProcedureCustomModel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [multiple procedure].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [multiple procedure]; otherwise, <c>false</c>.
        /// </value>
        public bool MultipleProcedure { get; set; }

        /// <summary>
        /// Gets or sets the event parent identifier.
        /// </summary>
        /// <value>
        /// The event parent identifier.
        /// </value>
        public string EventParentId { get; set; }

        /// <summary>
        /// Gets or sets the type of the vacation.
        /// </summary>
        /// <value>
        /// The type of the vacation.
        /// </value>
        public string VacationType { get; set; }

        /// <summary>
        /// Gets or sets the emirate idnumber.
        /// </summary>
        /// <value>
        /// The emirate idnumber.
        /// </value>
        public string EmirateIdnumber { get; set; }

        /// <summary>
        /// Gets or sets the rec_end_date.
        /// </summary>
        /// <value>
        /// The rec_end_date.
        /// </value>
        public string Rec_end_date { get; set; }

        /// <summary>
        /// Gets or sets the rec_ start_date.
        /// </summary>
        /// <value>
        /// The rec_ start_date.
        /// </value>
        public string Rec_Start_date { get; set; }

        /// <summary>
        /// Gets or sets the name of the physician.
        /// </summary>
        /// <value>
        /// The name of the physician.
        /// </value>
        public string PhysicianName { get; set; }

        /// <summary>
        /// Gets or sets the rec_pattern_type.
        /// </summary>
        /// <value>
        /// The rec_pattern_type.
        /// </value>
        public string rec_pattern_type { get; set; }

        /// <summary>
        /// Gets or sets the rec_type_type.
        /// </summary>
        /// <value>
        /// The rec_type_type.
        /// </value>
        public string rec_type_type { get; set; }

        /// <summary>
        /// Gets or sets the rec_end_date_type.
        /// </summary>
        /// <value>
        /// The rec_end_date_type.
        /// </value>
        public string Rec_end_date_type { get; set; }

        /// <summary>
        /// Gets or sets the rec_ start_date_type.
        /// </summary>
        /// <value>
        /// The rec_ start_date_type.
        /// </value>
        public string Rec_Start_date_type { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is recurrance.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is recurrance; otherwise, <c>false</c>.
        /// </value>
        public bool IsRecurrance { get; set; }

        /// <summary>
        /// Gets or sets the appointment type string.
        /// </summary>
        /// <value>
        /// The appointment type string.
        /// </value>
        public string AppointmentTypeStr { get; set; }

        /// <summary>
        /// Gets or sets the name of the department.
        /// </summary>
        /// <value>
        /// The name of the department.
        /// </value>
        public string DepartmentName { get; set; }

        /// <summary>
        /// Gets or sets the physician speciality string.
        /// </summary>
        /// <value>
        /// The physician speciality string.
        /// </value>
        public string PhysicianSpecialityStr { get; set; }

        public string RoomAssignedSTR { get; set; }
        public string EquipmentAssignedSTR { get; set; }

        public long PhysicianReferredBy { get; set; }

        public string TypeOfVisit { get; set; }
        #endregion
    }

    /// <summary>
    /// Type Of Procedure Custom Model
    /// </summary>
    [NotMapped]
    public class TypeOfProcedureCustomModel
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the main identifier.
        /// </summary>
        /// <value>
        /// The main identifier.
        /// </value>
        public int MainId { get; set; }

        /// <summary>
        /// Gets or sets the type of procedure identifier.
        /// </summary>
        /// <value>
        /// The type of procedure identifier.
        /// </value>
        public string TypeOfProcedureId { get; set; }

        /// <summary>
        /// Gets or sets the name of the type of procedure.
        /// </summary>
        /// <value>
        /// The name of the type of procedure.
        /// </value>
        public string TypeOfProcedureName { get; set; }

        /// <summary>
        /// Gets or sets the date selected.
        /// </summary>
        /// <value>
        /// The date selected.
        /// </value>
        public string DateSelected { get; set; }

        /// <summary>
        /// Gets or sets the time from.
        /// </summary>
        /// <value>
        /// The time from.
        /// </value>
        public string TimeFrom { get; set; }

        /// <summary>
        /// Gets or sets the time to.
        /// </summary>
        /// <value>
        /// The time to.
        /// </value>
        public string TimeTo { get; set; }

        /// <summary>
        /// Gets or sets the event parent identifier.
        /// </summary>
        /// <value>
        /// The event parent identifier.
        /// </value>
        public string EventParentId { get; set; }

        /// <summary>
        /// Gets or sets the time slot time interval.
        /// </summary>
        /// <value>
        /// The time slot time interval.
        /// </value>
        public string TimeSlotTimeInterval { get; set; }

        /// <summary>
        /// Gets or sets the procedure availablity status.
        /// </summary>
        /// <value>
        /// The procedure availablity status.
        /// </value>
        public string ProcedureAvailablityStatus { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is recurrance.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is recurrance; otherwise, <c>false</c>.
        /// </value>
        public bool IsRecurrance { get; set; }

        /// <summary>
        /// Gets or sets the type of the rec_.
        /// </summary>
        /// <value>
        /// The type of the rec_.
        /// </value>
        public string Rec_Type { get; set; }

        /// <summary>
        /// Gets or sets the rec_ pattern.
        /// </summary>
        /// <value>
        /// The rec_ pattern.
        /// </value>
        public string Rec_Pattern { get; set; }

        /// <summary>
        /// Gets or sets the end_ by.
        /// </summary>
        /// <value>
        /// The end_ by.
        /// </value>
        public string end_By { get; set; }

        /// <summary>
        /// Gets or sets the rec_end_date.
        /// </summary>
        /// <value>
        /// The rec_end_date.
        /// </value>
        public string Rec_end_date { get; set; }

        /// <summary>
        /// Gets or sets the rec_ start_date.
        /// </summary>
        /// <value>
        /// The rec_ start_date.
        /// </value>
        public string Rec_Start_date { get; set; }

        public string DeptOpeningDays { get; set; }

        public string PhysicianId { get; set; }


        #endregion
    }
}