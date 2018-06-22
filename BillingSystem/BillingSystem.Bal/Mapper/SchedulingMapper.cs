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
    public class SchedulingMapper : Mapper<Scheduling, SchedulingCustomModel>
    {
        /// <summary>
        /// Maps the model to view model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public override SchedulingCustomModel MapModelToViewModel(Scheduling model)
        {
            var vm = base.MapModelToViewModel(model);
            using (var physicianBal = new PhysicianBal())
            {
                var physicianObj = !string.IsNullOrEmpty(model.PhysicianId) ?
                    physicianBal.GetPhysicianCModelById(Convert.ToInt32(model.PhysicianId)) :
                    null;
                var patientObj = physicianBal.GetPatientCustomModelByPatientId(Convert.ToInt32(model.AssociatedId));
                if (physicianObj != null)
                {
                    vm.PhysicianSPL = physicianObj.UserSpecialityStr;
                    //vm.DepartmentName = physicianObj.UserDepartmentStr;
                    vm.PhysicianName = physicianObj.Physician.PhysicianName;
                    vm.PatientId = patientObj != null && patientObj.PatientInfo != null ? patientObj.PatientInfo.PatientID : 0;
                    vm.PatientName = patientObj != null && patientObj.PatientInfo != null ? patientObj.PatientInfo.PersonFirstName + " " + patientObj.PatientInfo.PersonLastName : string.Empty;
                    vm.PatientEmailId = new PatientLoginDetailBal().GetPatientEmail(patientObj.PatientInfo.PatientID);
                    vm.PatientEmirateIdNumber = patientObj != null && patientObj.PatientInfo != null ? patientObj.PatientInfo.PersonEmiratesIDNumber : string.Empty;
                    vm.PatientDOB = patientObj != null && patientObj.PatientInfo != null ? patientObj.PatientInfo.PersonBirthDate : DateTime.Now;
                    vm.MultipleProcedures = Convert.ToBoolean(vm.ExtValue3);
                    var firstOrDefault = patientObj != null && patientObj.PatientInfo != null
                        ? patientObj.PatientInfo.PatientPhone.FirstOrDefault(x => x.IsPrimary == true)
                        : null;

                    if (firstOrDefault != null)
                        vm.PatientPhoneNumber = patientObj.PatientInfo != null ? firstOrDefault.PhoneNo : string.Empty;

                    vm.PhysicianId = model.PhysicianId;
                    vm.DepartmentName = string.IsNullOrEmpty(model.ExtValue1)
                                            ? string.Empty
                                            : physicianBal.GetDepartmentNameById(Convert.ToInt32(model.ExtValue1));
                    vm.AppointmentTypeStr = string.IsNullOrEmpty(model.TypeOfProcedure)
                                                ? string.Empty
                                                : physicianBal.GetAppointmentTypeById(
                                                    Convert.ToInt32(model.TypeOfProcedure));
                    vm.FacilityName = physicianBal.GetFacilityNameByFacilityId(Convert.ToInt32(model.FacilityId));
                    vm.CorporateName = physicianBal.GetCorporateNameFromId(Convert.ToInt32(model.CorporateId));
                }
            }

            return vm;
        }

        /// <summary>
        /// Maps the view model to model.
        /// </summary>
        /// <param name="vm">The custommodel.</param>
        /// <returns></returns>
        public override Scheduling MapViewModelToModel(SchedulingCustomModel vm)
        {
            var m = base.MapViewModelToModel(vm);
            m.ExtValue3 = Convert.ToString(vm.MultipleProcedures);
            return m;
        }
    }


    /// <summary>
    /// Not Avialable Time Slot Scheduling Mapper
    /// </summary>
    public class NotAvialableTimeSlotSchedulingMapper : Mapper<Scheduling, NotAvialableTimeSlots>
    {
        /// <summary>
        /// Maps the model to view model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public override NotAvialableTimeSlots MapModelToViewModel(Scheduling model)
        {
            var vm = base.MapModelToViewModel(model);
            using (var physicianBal = new PhysicianBal())
            {
                if (model.SchedulingType == "2")
                {
                    vm.DateFrom = model.ScheduleFrom;
                    vm.DateTo = model.ScheduleTo;
                    vm.Reason = !string.IsNullOrEmpty(model.ExtValue2)
                                    ? "Some of the Physician under the facility have open appointments in their schedule."
                                    : model.SchedulingType == "2"
                                          ? "Physician have open appointments/vacation in the selected Date: " + model.ScheduleFrom.Value.ToShortDateString()
                                          : "Time slot is not available";
                }
                else
                {
                    var physicianObj = !string.IsNullOrEmpty(model.PhysicianId)
                                           ? physicianBal.GetPhysicianCModelById(Convert.ToInt32(model.PhysicianId))
                                           : null;
                    if (physicianObj != null)
                    {
                        vm.PhysicianSpl = physicianObj.UserSpecialityStr;
                        vm.PhysicianDepartment = physicianObj.UserDepartmentStr;
                        vm.PhysicianName = physicianObj.Physician.PhysicianName;
                        vm.DateFrom = model.ScheduleFrom;
                        vm.DateTo = model.ScheduleTo;
                        vm.Reason = !string.IsNullOrEmpty(model.ExtValue2)
                                        ? "Some of the Physician under the facility have open appointments/vacation in their schedule."
                                        : model.SchedulingType == "2"
                                              ? "Physician have open appointments/vacation in the selected Date/Date range."
                                              : "Time slot is not available";
                        vm.DateFromSTR = model.ScheduleFrom.Value.ToShortDateString();
                        vm.DateToSTR = model.ScheduleTo.Value.ToShortDateString();
                        vm.TimeFromStr = model.ScheduleFrom.Value.ToString("HH:mm");
                        vm.TimeTOStr = model.ScheduleTo.Value.ToString("HH:mm");
                    }
                }
            }

            return vm;
        }
    }
}