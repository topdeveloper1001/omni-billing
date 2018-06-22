// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FacultyRoosterCustomModel.cs" company="Spadez">
//   Omnihealthcare
// </copyright>
// <summary>
//   The faculty rooster custom model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Model.CustomModel
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    ///     The faculty rooster custom model.
    /// </summary>
    [NotMapped]
    public class FacultyRoosterCustomModel : FacultyRooster
    {
        /// <summary>
        /// Gets or sets the name of the department.
        /// </summary>
        /// <value>
        /// The name of the department.
        /// </value>
        public string DepartmentName { get; set; }

        /// <summary>
        /// Gets or sets the name of the faculty.
        /// </summary>
        /// <value>
        /// The name of the faculty.
        /// </value>
        public string FacultyName { get; set; }

        /// <summary>
        /// Gets or sets the faculty type string.
        /// </summary>
        /// <value>
        /// The faculty type string.
        /// </value>
        public string FacultyTypeStr { get; set; }

        /// <summary>
        /// Gets or sets the name of the faculty.
        /// </summary>
        /// <value>
        /// The name of the faculty.
        /// </value>
        public string FromDateStr { get; set; }

        /// <summary>
        /// Gets or sets the faculty type string.
        /// </summary>
        /// <value>
        /// The faculty type string.
        /// </value>
        public string ToDateStr { get; set; }

        /// <summary>
        /// Gets or sets from time string.
        /// </summary>
        /// <value>
        /// From time string.
        /// </value>
        public string FromTimeStr { get; set; }

        /// <summary>
        /// Gets or sets to time string.
        /// </summary>
        /// <value>
        /// To time string.
        /// </value>
        public string ToTimeStr { get; set; }

        /// <summary>
        /// Gets or sets the faculty timeslots.
        /// </summary>
        /// <value>
        /// The faculty timeslots.
        /// </value>
        public List<FacultyRoosterTimeSlotCustomModel> FacultyTimeslots { get; set; }
    }

    /// <summary>
    /// Faculty Rooster TimeSlot Custom Model
    /// </summary>
     [NotMapped]
    public class FacultyRoosterTimeSlotCustomModel
    {
        /// <summary>
        /// Gets or sets the name of the department.
        /// </summary>
        /// <value>
        /// The name of the department.
        /// </value>
        public string DepartmentName { get; set; }

        /// <summary>
        /// Gets or sets the name of the faculty.
        /// </summary>
        /// <value>
        /// The name of the faculty.
        /// </value>
        public string FacultyName { get; set; }

        /// <summary>
        /// Gets or sets the name of the faculty.
        /// </summary>
        /// <value>
        /// The name of the faculty.
        /// </value>
        public string FromDateStr { get; set; }

        /// <summary>
        /// Gets or sets the faculty type string.
        /// </summary>
        /// <value>
        /// The faculty type string.
        /// </value>
        public string ToDateStr { get; set; }

        /// <summary>
        /// Gets or sets from time string.
        /// </summary>
        /// <value>
        /// From time string.
        /// </value>
        public string FromTimeStr { get; set; }

        /// <summary>
        /// Gets or sets to time string.
        /// </summary>
        /// <value>
        /// To time string.
        /// </value>
        public string ToTimeStr { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }
    }
}