// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XMLBillScrubberDashboardView.cs" company="Spadez">
//   OmniHealthcare
// </copyright>
// <summary>
//   The xml bill scrubber dashboard view.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Models
{
    using System.Collections.Generic;

    using BillingSystem.Model.CustomModel;

    /// <summary>
    /// The xml bill scrubber dashboard view.
    /// </summary>
    public class XMLBillScrubberDashboardView
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the dashboard type.
        /// </summary>
        public int DashboardType { get; set; }

        /// <summary>
        /// Gets or sets the facility id.
        /// </summary>
        public int FacilityId { get; set; }

        /// <summary>
        /// Gets or sets the section 10 remarks list.
        /// </summary>
        public List<DashboardRemarkCustomModel> Section10RemarksList { get; set; }

        /// <summary>
        /// Gets or sets the section 1 remarks list.
        /// </summary>
        public List<DashboardRemarkCustomModel> Section1RemarksList { get; set; }

        /// <summary>
        /// Gets or sets the section 2 remarks list.
        /// </summary>
        public List<DashboardRemarkCustomModel> Section2RemarksList { get; set; }

        /// <summary>
        /// Gets or sets the section 3 remarks list.
        /// </summary>
        public List<DashboardRemarkCustomModel> Section3RemarksList { get; set; }

        /// <summary>
        /// Gets or sets the section 4 remarks list.
        /// </summary>
        public List<DashboardRemarkCustomModel> Section4RemarksList { get; set; }

        /// <summary>
        /// Gets or sets the section 5 remarks list.
        /// </summary>
        public List<DashboardRemarkCustomModel> Section5RemarksList { get; set; }

        /// <summary>
        /// Gets or sets the section 6 remarks list.
        /// </summary>
        public List<DashboardRemarkCustomModel> Section6RemarksList { get; set; }

        /// <summary>
        /// Gets or sets the section 7 remarks list.
        /// </summary>
        public List<DashboardRemarkCustomModel> Section7RemarksList { get; set; }

        /// <summary>
        /// Gets or sets the section 8 remarks list.
        /// </summary>
        public List<DashboardRemarkCustomModel> Section8RemarksList { get; set; }

        /// <summary>
        /// Gets or sets the section 9 remarks list.
        /// </summary>
        public List<DashboardRemarkCustomModel> Section9RemarksList { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }

        #endregion
    }
}