// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PhysicianCustomModel.cs" company="Spadez">
//   OmniHealth care
// </copyright>
// <summary>
//   The physician custom model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Model.CustomModel
{
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// The physician custom model.
    /// </summary>
    [NotMapped]
    public class PhysicianCustomModel
    {
        /// <summary>
        /// Gets or sets the physician.
        /// </summary>
        public Physician Physician { get; set; }

        /// <summary>
        /// Gets or sets the primary facility name.
        /// </summary>
        public string PrimaryFacilityName { get; set; }

        /// <summary>
        /// Gets or sets the secondary facility name.
        /// </summary>
        public string SecondaryFacilityName { get; set; }

        /// <summary>
        /// Gets or sets the third facility name.
        /// </summary>
        public string ThirdFacilityName { get; set; }

        /// <summary>
        /// Gets or sets the physican license type name.
        /// </summary>
        public string PhysicanLicenseTypeName { get; set; }

        /// <summary>
        /// Gets or sets the user type str.
        /// </summary>
        public string UserTypeStr { get; set; }

        /// <summary>
        /// Gets or sets the user speciality str.
        /// </summary>
        public string UserSpecialityStr { get; set; }

        /// <summary>
        /// Gets or sets the user department str.
        /// </summary>
        public string UserDepartmentStr { get; set; }


        /// <summary>
        /// Gets or sets the encounter number.
        /// </summary>
        /// <value>
        /// The encounter number.
        /// </value>
        public string EncounterNumber { get; set; }

    }

    [NotMapped]
    public class PhysicianViewModel : Physician
    {
        /// <summary>
        /// Gets or sets the primary facility name.
        /// </summary>
        public string PrimaryFacilityName { get; set; }

        /// <summary>
        /// Gets or sets the secondary facility name.
        /// </summary>
        public string SecondaryFacilityName { get; set; }

        /// <summary>
        /// Gets or sets the third facility name.
        /// </summary>
        public string ThirdFacilityName { get; set; }

        /// <summary>
        /// Gets or sets the physican license type name.
        /// </summary>
        public string PhysicanLicenseTypeName { get; set; }

        /// <summary>
        /// Gets or sets the user type str.
        /// </summary>
        public string UserTypeStr { get; set; }

        /// <summary>
        /// Gets or sets the user speciality str.
        /// </summary>
        public string UserSpecialityStr { get; set; }

        /// <summary>
        /// Gets or sets the user department str.
        /// </summary>
        public string UserDepartmentStr { get; set; }


        /// <summary>
        /// Gets or sets the encounter number.
        /// </summary>
        /// <value>
        /// The encounter number.
        /// </value>
        public string EncounterNumber { get; set; }

        public string ClinicianName { get; set; }

        public string OtherFacilities { get; set; }


        public IEnumerable<DropdownListData> Facilities { get; set; } = new List<DropdownListData>();
    }
}
