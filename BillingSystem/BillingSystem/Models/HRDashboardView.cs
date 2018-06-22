// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HRDashboardView.cs" company="Spadez">
//   Omnihealthcare
// </copyright>
// <screenOwner>
// Shashank (Last Modified on Feb 11 2016)
// </screenOwner>
// <summary>
//   The hr dashboard view.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Models
{
    using System.Collections.Generic;

    using BillingSystem.Model.CustomModel;

    /// <summary>
    /// The hr dashboard view.
    /// </summary>
    public class HRDashboardView
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
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }

        #endregion
    }
}