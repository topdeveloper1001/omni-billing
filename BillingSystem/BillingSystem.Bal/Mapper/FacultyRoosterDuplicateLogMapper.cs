// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FacultyRoosterDuplicateLogMapper.cs" company="Spadez">
//   Omnihealthcare
// </copyright>
// <summary>
//   The faculty rooster duplicate log mapper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Bal.Mapper
{
    using System;

    using BillingSystem.Bal.BusinessAccess;
    using BillingSystem.Model;
    using BillingSystem.Model.CustomModel;

    /// <summary>
    /// The faculty rooster duplicate log mapper.
    /// </summary>
    public class FacultyRoosterDuplicateLogMapper : Mapper<FacultyRooster, FacultyRoosterLogCustomModel>
    {
        #region Public Methods and Operators

        /// <summary>
        /// The map model to view model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="type">The type.</param>
        /// <returns>
        /// The <see cref="FacultyRoosterCustomModel" />.
        /// </returns>
        public FacultyRoosterLogCustomModel MapModelToViewModel(FacultyRooster model, int type)
        {
            var vm = base.MapModelToViewModel(model);
            using (var physicianBal = new PhysicianBal())
            {
                var physicianObj = (model.FacultyId) != null
                                       ? physicianBal.GetPhysicianCModelById(Convert.ToInt32(model.FacultyId))
                                       : null;
                if (physicianObj != null)
                {
                    vm.PhysicianDepartment = physicianObj.UserDepartmentStr;
                    vm.PhysicianName = physicianObj.Physician.PhysicianName;
                    vm.Reason = type == 1
                                    ? "User already have schedule between selected dates for selected department. From Date : "
                                      + model.FromDate.Value.ToString("MM-dd-yyyy HH:mm") + " ,Till Date : "
                                      + model.ToDate.Value.ToString("MM-dd-yyyy HH:mm") + "."
                                    : type == 2
                                          ? "User already have schedule between selected dates. From Date : "
                                            + model.FromDate.Value.ToString("MM-dd-yyyy HH:mm") + " ,Till Date : "
                                            + model.ToDate.Value.ToString("MM-dd-yyyy HH:mm") + "."
                                          : type == 3
                                                ? "User have lunch timming between selected date time range. From Date : "
                                                  + model.FromDate.Value.ToString("MM-dd-yyyy HH:mm") + " ,Till Date : "
                                                  + model.ToDate.Value.ToString("MM-dd-yyyy HH:mm") + "."
                                                : string.Empty;
                }
            }

            return vm;
        }
        #endregion
    }
}