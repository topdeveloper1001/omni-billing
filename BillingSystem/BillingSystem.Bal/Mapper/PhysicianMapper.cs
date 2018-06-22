// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PhysicianMapper.cs" company="SPadez">
//   OmniHealthcare
// </copyright>
// <summary>
//   The physician mapper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Bal.Mapper
{
    using System;

    using BillingSystem.Bal.BusinessAccess;
    using BillingSystem.Common.Common;
    using BillingSystem.Model;
    using BillingSystem.Model.CustomModel;

    /// <summary>
    /// The physician mapper.
    /// </summary>
    public class PhysicianMapper : Mapper<Physician, PhysicianCustomModel>
    {
        #region Public Methods and Operators

        /// <summary>
        /// The map model to view model.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="PhysicianCustomModel"/>.
        /// </returns>
        public override PhysicianCustomModel MapModelToViewModel(Physician model)
        {
            PhysicianCustomModel vm = base.MapModelToViewModel(model);
            if (vm != null)
            {
                using (var bal = new PhysicianBal())
                {
                    vm.Physician = model;
                    vm.PrimaryFacilityName =
                        bal.GetFacilityNameByFacilityId(Convert.ToInt32(model.PhysicianPrimaryFacility));
                    vm.SecondaryFacilityName =
                        bal.GetFacilityNameByFacilityId(Convert.ToInt32(model.PhysicianSecondaryFacility));
                    vm.ThirdFacilityName = bal.GetFacilityNameByFacilityId(
                        Convert.ToInt32(model.PhysicianThirdFacility));
                    vm.PhysicanLicenseTypeName =
                        bal.GetNameByLicenseTypeIdAndUserTypeId(
                            Convert.ToString(model.PhysicianLicenseType), 
                            Convert.ToString(model.UserType));

                    using (var rBal = new RoleBal()) vm.UserTypeStr = rBal.GetRoleNameById(Convert.ToInt32(model.UserType));

                    int depId = Convert.ToInt32(model.FacultyDepartment);
                    vm.UserDepartmentStr = depId > 0
                                               ? bal.GetDepartmentNameById(Convert.ToInt32(model.FacultyDepartment))
                                               : string.Empty;

                    vm.UserSpecialityStr = bal.GetNameByGlobalCodeValue(
                        Convert.ToString(model.FacultySpeciality), 
                        Convert.ToString((int)GlobalCodeCategoryValue.PhysicianSpecialties));
                }
            }

            return vm;
        }

        #endregion
    }
}