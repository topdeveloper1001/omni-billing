// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FacultyTimeslotsCustomModel.cs" company="">
//   
// </copyright>
// <summary>
//   The faculty timeslots custom model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BillingSystem.Model.CustomModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    using Elmah;

    /// <summary>
    ///     The faculty timeslots custom model.
    /// </summary>
    [NotMapped]
    public class SchedulingCustomModel : Scheduling
    {
        public bool ClinicianChanged { get; set; }

        /// <summary>
        /// Gets or sets the name of the department.
        /// </summary>
        /// <value>
        /// The name of the department.
        /// </value>
        public string DepartmentName { get; set; }

        /// <summary>
        /// Gets or sets the physician SPL.
        /// </summary>
        /// <value>
        /// The physician SPL.
        /// </value>
        public string PhysicianSPL { get; set; }

        /// <summary>
        /// Gets or sets the dept opening time.
        /// </summary>
        /// <value>
        /// The dept opening time.
        /// </value>
        public string DeptOpeningTime { get; set; }

        /// <summary>
        /// Gets or sets the dept closing time.
        /// </summary>
        /// <value>
        /// The dept closing time.
        /// </value>
        public string DeptClosingTime { get; set; }

        /// <summary>
        /// Gets or sets the dept opening days.
        /// </summary>
        /// <value>
        /// The dept opening days.
        /// </value>
        public string DeptOpeningDays { get; set; }

        /// <summary>
        /// Gets or sets the block date.
        /// </summary>
        /// <value>
        /// The block date.
        /// </value>
        public DateTime BlockDate { get; set; }

        /// <summary>
        /// Gets or sets the block time.
        /// </summary>
        /// <value>
        /// The block time.
        /// </value>
        //public string[] BlockTime { get; set; }

        /// <summary>
        /// Gets or sets the patient identifier.
        /// </summary>
        /// <value>
        /// The patient identifier.
        /// </value>
        public int? PatientId { get; set; }

        /// <summary>
        /// Gets or sets the name of the patient.
        /// </summary>
        /// <value>
        /// The name of the patient.
        /// </value>
        public string PatientName { get; set; }

        /// <summary>
        /// Gets or sets the patient dob.
        /// </summary>
        /// <value>
        /// The patient dob.
        /// </value>
        public DateTime? PatientDOB { get; set; }

        /// <summary>
        /// Gets or sets the patient email identifier.
        /// </summary>
        /// <value>
        /// The patient email identifier.
        /// </value>
        public string PatientEmailId { get; set; }

        /// <summary>
        /// Gets or sets the patient phone number.
        /// </summary>
        /// <value>
        /// The patient phone number.
        /// </value>
        public string PatientPhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [multiple procedures].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [multiple procedures]; otherwise, <c>false</c>.
        /// </value>
        public bool MultipleProcedures { get; set; }

        /// <summary>
        /// Gets or sets the name of the physician.
        /// </summary>
        /// <value>
        /// The name of the physician.
        /// </value>
        public string PhysicianName { get; set; }

        /// <summary>
        /// Gets or sets the type of the appointment.
        /// </summary>
        /// <value>
        /// The type of the appointment.
        /// </value>
        public string AppointmentType { get; set; }

        /// <summary>
        /// Gets or sets the email template identifier.
        /// </summary>
        /// <value>
        /// The email template identifier.
        /// </value>
        public int? EmailTemplateId { get; set; }

        /// <summary>
        /// Gets or sets the patient emirate identifier number.
        /// </summary>
        /// <value>
        /// The patient emirate identifier number.
        /// </value>
        public string PatientEmirateIdNumber { get; set; }

        /// <summary>
        /// Gets or sets the appointment type string.
        /// </summary>
        /// <value>
        /// The appointment type string.
        /// </value>
        public string AppointmentTypeStr { get; set; }

        /// <summary>
        /// Gets or sets the room assigned string.
        /// </summary>
        /// <value>
        /// The room assigned string.
        /// </value>
        public string RoomAssignedSTR { get; set; }

        /// <summary>
        /// Gets or sets the room assigned string.
        /// </summary>
        /// <value>
        /// The room assigned string.
        /// </value>
        public string EquipmentAssignedSTR { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string SelectedPhysicianId { get; set; }

        /// <summary>
        /// Store external val3 column value
        /// </summary>
        public string ExternalValue3 { get; set; }

        /// <summary>
        /// Store remove appointment types value
        /// </summary>
        public string RemovedAppointmentTypes { get; set; }

        /// <summary>
        /// Gets or sets the name of the facility.
        /// </summary>
        /// <value>
        /// The name of the facility.
        /// </value>
        public string FacilityName { get; set; }

        /// <summary>
        /// Gets or sets the name of the corporate.
        /// </summary>
        /// <value>
        /// The name of the corporate.
        /// </value>
        public string CorporateName { get; set; }

        public bool UpdateFlag { get; set; }
    }

    [NotMapped]
    public class SchedulingCustomModelView
    {
        /// <summary>
        /// Gets or sets the department opening slots list.
        /// </summary>
        /// <value>
        /// The department opening slots list.
        /// </value>
        public List<FacultyTimeslotsCustomModel> DepartmentOpeningSlotsList { get; set; }

        /// <summary>
        /// Gets or sets the department opening slots list.
        /// </summary>
        /// <value>
        /// The department opening slots list.
        /// </value>
        public List<FacultyTimeslotsCustomModel> LunchSlotsList { get; set; }

        /// <summary>
        /// Gets or sets the department opening slots list.
        /// </summary>
        /// <value>
        /// The department opening slots list.
        /// </value>
        public List<SchedulerFacultyTimeslotsCustomModel> SchDepartmentOpeningSlotsList { get; set; }

        /// <summary>
        /// Gets or sets the lunch break list.
        /// </summary>
        /// <value>
        /// The lunch break slots list.
        /// </value>
        public List<SchedulerFacultyTimeslotsCustomModel> SchLunchSlotsList { get; set; }

        /// <summary>
        /// Gets or sets the patient scheduling data list
        /// </summary>
        /// <value>
        /// The patient scheduling data list.
        /// </value>
        public List<SchedulingCustomModel> PatientSchedulingDataList { get; set; }

        /// <summary>
        /// Gets or sets the patient scheduling data list
        /// </summary>
        /// <value>
        /// The patient scheduling data list.
        /// </value>
        public List<SchedulerFacultyTimeslotsCustomModel> SchPatientSchedulingDataList { get; set; }

        /// <summary>
        /// Gets or sets the physician speciality
        /// </summary>
        /// <value>
        /// Physician speciality
        /// </value>
        public string PhysicianSpeciality { get; set; }

        /// <summary>
        /// Gets or sets the physician location
        /// </summary>
        /// <value>
        /// Physician location
        /// </value>
        public string PhysicianLocation { get; set; }

        /// <summary>
        /// Gets or sets the Holiday list
        /// </summary>
        /// <value>
        /// The physician holiday list.
        /// </value>
        public List<HolidayPlannerDetailsCustomModel> HolidayList { get; set; }

        /// <summary>
        /// Gets or sets the Holiday scheduling data list
        /// </summary>
        /// <value>
        /// The physician scheduling data list.
        /// </value>
        public List<SchedulerFacultyTimeslotsCustomModel> SchHolidayList { get; set; }

    }

    [NotMapped]
    public class NotAvialableTimeSlots
    {
        /// <summary>
        /// Gets or sets the date from.
        /// </summary>
        /// <value>
        /// The date from.
        /// </value>
        public DateTime? DateFrom { get; set; }

        /// <summary>
        /// Gets or sets the date to.
        /// </summary>
        /// <value>
        /// The date to.
        /// </value>
        public DateTime? DateTo { get; set; }

        /// <summary>
        /// Gets or sets the name of the physician.
        /// </summary>
        /// <value>
        /// The name of the physician.
        /// </value>
        public string PhysicianName { get; set; }

        /// <summary>
        /// Gets or sets the physician SPL.
        /// </summary>
        /// <value>
        /// The physician SPL.
        /// </value>
        public string PhysicianSpl { get; set; }

        /// <summary>
        /// Gets or sets the physician department.
        /// </summary>
        /// <value>
        /// The physician department.
        /// </value>
        public string PhysicianDepartment { get; set; }

        /// <summary>
        /// Gets or sets the reason.
        /// </summary>
        /// <value>
        /// The reason.
        /// </value>
        public string Reason { get; set; }

        /// <summary>
        /// Gets or sets the date from string.
        /// </summary>
        /// <value>
        /// The date from string.
        /// </value>
        public string DateFromSTR { get; set; }


        /// <summary>
        /// Gets or sets the date to string.
        /// </summary>
        /// <value>
        /// The date to string.
        /// </value>
        public string DateToSTR { get; set; }


        /// <summary>
        /// Gets or sets the time from string.
        /// </summary>
        /// <value>
        /// The time from string.
        /// </value>
        public string TimeFromStr { get; set; }

        /// <summary>
        /// Gets or sets the time to string.
        /// </summary>
        /// <value>
        /// The time to string.
        /// </value>
        public string TimeTOStr { get; set; }
    }

    [NotMapped]
    public class NotAvialableRoomEquipment
    {
        /// <summary>
        /// Gets or sets the reason.
        /// </summary>
        /// <value>
        /// The reason.
        /// </value>
        public string Reason { get; set; }

        /// <summary>
        /// Gets or sets the name of the room.
        /// </summary>
        /// <value>
        /// The name of the room.
        /// </value>
        public string RoomName { get; set; }

        /// <summary>
        /// Gets or sets the room identifier.
        /// </summary>
        /// <value>
        /// The room identifier.
        /// </value>
        public int RoomId { get; set; }

        /// <summary>
        /// Gets or sets the equipment name identifier.
        /// </summary>
        /// <value>
        /// The equipment name identifier.
        /// </value>
        public string EquipmentNameId { get; set; }

        /// <summary>
        /// Gets or sets the equipment identifier.
        /// </summary>
        /// <value>
        /// The equipment identifier.
        /// </value>
        public int EquipmentId { get; set; }

        /// <summary>
        /// Gets or sets the type of procedure string.
        /// </summary>
        /// <value>
        /// The type of procedure string.
        /// </value>
        public string TypeOfProcedureStr { get; set; }
    }

    [NotMapped]
    public class RoomEquipmentAvialability
    {
        /// <summary>
        /// Gets or sets the room identifier.
        /// </summary>
        /// <value>
        /// The room identifier.
        /// </value>
        public int RoomId { get; set; }

        /// <summary>
        /// Gets or sets the equipment identifier.
        /// </summary>
        /// <value>
        /// The equipment identifier.
        /// </value>
        public int EquipmentId { get; set; }

        public int DepartmentId { get; set; }

        public bool IsAppointed { get; set; }

        public bool IsEquipmentRequired { get; set; }
    }

    /// <summary>
    /// Time Slot Availability Custom Model
    /// </summary>
    [NotMapped]
    public class TimeSlotAvailabilityCustomModel
    {
        /// <summary>
        /// Gets or sets the time slot available.
        /// </summary>
        /// <value>
        /// The time slot available.
        /// </value>
        public int TimeSlotAvailable { get; set; }
    }

    [NotMapped]
    public class AvailabilityTimeSlotForPopupCustomModel
    {
        /// <summary>
        /// Gets or sets the time slot.
        /// </summary>
        /// <value>
        /// The time slot.
        /// </value>
        public string TimeSlot { get; set; }
        public int ID { get; set; }
        public int PhysicianId { get; set; }
        public int DeptId { get; set; }
        public string DeptName { get; set; }
        public string Clinician { get; set; }
        public string DepOpeningDays { get; set; }
    }

    [NotMapped]
    public class SkippedHolidaysData
    {
        /// <summary>
        /// Gets or sets the time slot.
        /// </summary>
        /// <value>
        /// The time slot.
        /// </value>
        public int? PhysicianId { get; set; }

        /// <summary>
        /// Gets or sets the holiday date.
        /// </summary>
        /// <value>
        /// The holiday date.
        /// </value>
        public DateTime? HolidayDate { get; set; }

        /// <summary>
        /// Gets or sets the name of the physician.
        /// </summary>
        /// <value>
        /// The name of the physician.
        /// </value>
        public string PhysicianName { get; set; }

        /// <summary>
        /// Gets or sets the type of the scheduling.
        /// </summary>
        /// <value>
        /// The type of the scheduling.
        /// </value>
        public int? SchedulingType { get; set; }
    }
}