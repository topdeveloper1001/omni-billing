// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FacultyRoosterMapper.cs" company="Spadez">
//   OmniHealthcare
// </copyright>
// <summary>
//   The faculty rooster mapper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Bal.Mapper
{
    using System;
    using System.Collections.Generic;

    using BillingSystem.Bal.BusinessAccess;
    using BillingSystem.Model;
    using BillingSystem.Model.CustomModel;

    /// <summary>
    /// The faculty rooster mapper.
    /// </summary>
    public class FacultyRoosterMapper : Mapper<FacultyRooster, FacultyRoosterCustomModel>
    {
        #region Public Methods and Operators

        /// <summary>
        /// The map model to view model.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="FacultyRoosterCustomModel"/>.
        /// </returns>
        public override FacultyRoosterCustomModel MapModelToViewModel(FacultyRooster model)
        {
            var vm = base.MapModelToViewModel(model);
            var facilitybal = new FacultyRoosterBal();
            vm.FacultyName = facilitybal.GetPhysicianName(Convert.ToInt32(model.FacultyId));
            vm.DepartmentName = facilitybal.GetDepartmentNameById(Convert.ToInt32(model.DeptId));
            if (model.FromDate != null)
            {
                vm.FromDateStr = model.FromDate.Value.ToString("MM/dd/yyyy");
                vm.FromTimeStr = model.FromDate.Value.ToString("HH:mm");
            }

            if (model.ToDate != null)
            {
                vm.ToDateStr = model.ToDate.Value.ToString("MM/dd/yyyy");
                vm.ToTimeStr = model.ToDate.Value.ToString("HH:mm");
            }

            if (model.ExtValue1 == "1")
            {
                vm.DepartmentName += " (Default Department)";
            }
            var timeslotsOverride = facilitybal.GetFacultyRoosterByFacultyid(Convert.ToInt32(model.FacultyId));
            vm.FacultyTimeslots = new List<FacultyRoosterTimeSlotCustomModel>();
            foreach (var item in timeslotsOverride)
            {
                vm.FacultyTimeslots.Add(new FacultyRoosterTimeSlotMapper().MapCustomModelToViewModel(item));
            }

            return vm;
        }
        #endregion
    }

    public class FacultyRoosterTimeSlotMapper : Mapper<FacultyRooster, FacultyRoosterTimeSlotCustomModel>
    {
        /// <summary>
        /// Maps the custom model to view model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public FacultyRoosterTimeSlotCustomModel MapCustomModelToViewModel(FacultyRooster model)
        {
            var vm = MapModelToViewModel(model);
            var facilitybal = new FacultyRoosterBal();
            vm.Id = model.Id;
            vm.FacultyName = facilitybal.GetPhysicianName(Convert.ToInt32(model.FacultyId));
            vm.DepartmentName = facilitybal.GetDepartmentNameById(Convert.ToInt32(model.DeptId));
            if (model.FromDate != null)
            {
                vm.FromDateStr = model.FromDate.Value.ToString("MM/dd/yyyy");
                vm.FromTimeStr = model.FromDate.Value.ToString("HH:mm");
            }

            if (model.ToDate != null)
            {
                vm.ToDateStr = model.ToDate.Value.ToString("MM/dd/yyyy");
                vm.ToTimeStr = model.ToDate.Value.ToString("HH:mm");
            }
            return vm;
        }
    }
}