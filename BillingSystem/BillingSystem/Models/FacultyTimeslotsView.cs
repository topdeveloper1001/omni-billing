// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FacultyTimeslotsView.cs" company="">
//   
// </copyright>
// <summary>
//   The faculty timeslots view.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BillingSystem.Models
{
    using System.Collections.Generic;

    using BillingSystem.Model;
    using BillingSystem.Model.CustomModel;

    /// <summary>
    ///  The faculty timeslots view.
    /// </summary>
    public class FacultyTimeslotsView
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the current faculty times.
        /// </summary>
        public FacultyTimeslots CurrentFacultyTimesSlots { get; set; }

        /// <summary>
        /// Gets or sets the faculty times list.
        /// </summary>
        public IEnumerable<FacultyTimeslotsCustomModel> FacultyTimesSlotsList { get; set; }

        #endregion
    }
}