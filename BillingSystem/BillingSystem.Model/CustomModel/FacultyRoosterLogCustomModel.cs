// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FacultyRoosterLogCustomModel.cs" company="Spadez">
//   Omnihealthcare
// </copyright>
// <summary>
//   The faculty rooster log custom model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Model.CustomModel
{
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// The faculty rooster log custom model.
    /// </summary>
    [NotMapped]
    public class FacultyRoosterLogCustomModel : FacultyRooster
    {
        /// <summary>
        /// Gets or sets the physician department.
        /// </summary>
        /// <value>
        /// The physician department.
        /// </value>
        public string PhysicianDepartment { get; set; }

        /// <summary>
        /// Gets or sets the name of the physician.
        /// </summary>
        /// <value>
        /// The name of the physician.
        /// </value>
        public string PhysicianName { get; set; }

        /// <summary>
        /// Gets or sets the reason.
        /// </summary>
        /// <value>
        /// The reason.
        /// </value>
        public string Reason { get; set; }
    }
}