// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FacultyTimeslotsMapper.cs" company="SPadez">
//   Omnihealthcare
// </copyright>
// <summary>
//   The faculty timeslots mapper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BillingSystem.Bal.Mapper
{
    using System;
    using System.Linq;

    using BillingSystem.Bal.BusinessAccess;
    using BillingSystem.Model;
    using BillingSystem.Model.CustomModel;

    /// <summary>
    ///     The faculty timeslots mapper.
    /// </summary>
    public class FacultyTimeslotsMapper : Mapper<FacultyTimeslots, FacultyTimeslotsCustomModel>
    {
        #region Public Methods and Operators

        /// <summary>
        /// Maps the model to view model.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="FacultyTimeslotsCustomModel"/>.
        /// </returns>
        public override FacultyTimeslotsCustomModel MapModelToViewModel(FacultyTimeslots model)
        {
            var vm = base.MapModelToViewModel(model);
            using (var bal = new BaseBal())
            {
                var facultyObj = bal.GetFacultyByUserId(model.UserID);
                vm.FacultyLunchTimeFrom = model.SlotAvailability == "3"
                                              ? model.AvailableDateFrom.Value.ToShortDateString() +" "+  model.AvailableDateFrom.Value.ToString("HH:mm")
                                               : string.Empty;
                vm.FacultyLunchTimeTill = model.SlotAvailability == "3"
                                              ? model.AvailableDateTill.Value.ToShortDateString() + " " + model.AvailableDateTill.Value.ToString("HH:mm")
                                              : string.Empty;
              

                vm.LunchStartTime = Convert.ToDateTime(model.AvailableDateFrom).Minute > 0
                    ? Convert.ToString(Convert.ToDateTime(model.AvailableDateFrom).Hour) + ".5"
                    : Convert.ToString(Convert.ToDateTime(model.AvailableDateFrom).Hour);
                vm.LunchEndTime = Convert.ToDateTime(model.AvailableDateTill).Minute > 0
                    ? Convert.ToString(Convert.ToDateTime(model.AvailableDateTill).Hour) + ".5"
                    : Convert.ToString(Convert.ToDateTime(model.AvailableDateTill).Hour);
            }

            return vm;
        }

        /// <summary>
        /// Departments the opening slot map model to view model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public FacultyTimeslotsCustomModel DepartmentOpeningSlotMapModelToViewModel(FacultyTimeslots model)
        {
            var vm = base.MapModelToViewModel(model);
            //using (var bal = new BaseBal())
            //{
            //    var facultyObj = bal.GetPhysicianById(model.UserID);
            //    var facultyDepartment = facultyObj != null && !string.IsNullOrEmpty(facultyObj.FacultyDepartment)
            //                                   ? facultyObj.FacultyDepartment
            //                                   : string.Empty;

            //    var facilityStruct = new FacilityStructureService();
            //    var facilityObj = string.IsNullOrEmpty(facultyDepartment)
            //                                        ? null
            //                          : facilityStruct.GetFacilityStructureById(Convert.ToInt32(facultyDepartment));
            //    vm.DepartmentName = facilityObj != null ? facilityObj.FacilityStructureName : string.Empty;
            //    var departmentid = facilityObj != null ? facilityObj.FacilityStructureId : 0;

            //    //if (model.AvailableDateFrom != null)
            //    //{
            //    //    vm.FacultyLunchTimeFrom = facultyObj != null
            //    //                              && !string.IsNullOrEmpty(facultyObj.FacultyLunchTimeFrom)
            //    //                                  ? model.AvailableDateFrom.Value.ToShortDateString() + " "
            //    //                                    + facultyObj.FacultyLunchTimeFrom
            //    //                                  : string.Empty;
            //    //}

            //    //if (model.AvailableDateTill != null)
            //    //{
            //    //    vm.FacultyLunchTimeTill = facultyObj != null
            //    //                              && !string.IsNullOrEmpty(facultyObj.FacultyLunchTimeTill)
            //    //                                  ? model.AvailableDateTill.Value.ToShortDateString() + " "
            //    //                                    + facultyObj.FacultyLunchTimeTill
            //    //                                  : string.Empty;
            //    //}

            //    var departmenttimeslots = new DeptTimmingBal();
            //    var departmentTimmings = departmenttimeslots.GetDeptTimmingByDepartmentId(departmentid);
            //    if (departmentTimmings != null)
            //    {
            //        if (facilityObj != null)
            //        {
            //            var openingday = ((int)(Convert.ToDateTime(model.AvailableDateFrom).DayOfWeek)).ToString();
            //            var departmentFacilityobj = departmentTimmings.FirstOrDefault(x => x.OpeningDayId == openingday);
            //            if (departmentFacilityobj != null)
            //            {
            //                vm.DeptOpeningDays =
            //                    ((int)(Convert.ToDateTime(model.AvailableDateFrom).DayOfWeek)).ToString();
            //                vm.DeptOpeningTime = departmentFacilityobj.OpeningTime;
            //                vm.DeptClosingTime = departmentFacilityobj.ClosingTime;
            //            }
            //        }
            //    }
            //}

            return vm;
        }

        #endregion
    }
}