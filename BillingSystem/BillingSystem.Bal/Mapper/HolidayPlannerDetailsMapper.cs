// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HolidayPlannerDetailsMapper.cs" company="Spadez">
//   OmniHealthcare
// </copyright>
// <summary>
//   The holiday planner details mapper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace BillingSystem.Bal.Mapper
{
    using BillingSystem.Model;
    using BillingSystem.Model.CustomModel;
    using System.Globalization;

    /// <summary>
    /// The holiday planner details mapper.
    /// </summary>
    public class HolidayPlannerDetailsMapper : Mapper<HolidayPlannerDetails, HolidayPlannerDetailsCustomModel>
    {
        #region Public Methods and Operators

        /// <summary>
        /// The map model to view model.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="HolidayPlannerDetailsCustomModel"/>.
        /// </returns>
        public override HolidayPlannerDetailsCustomModel MapModelToViewModel(HolidayPlannerDetails model)
        {
            var vm = base.MapModelToViewModel(model);
            if (vm != null)
            {
                vm.start_date = model.RecDateFrom.HasValue
                    ? model.RecDateFrom.Value.ToString("MM/dd/yyyy HH:mm")
                    : string.Empty;
                vm.end_date = model.RecDateTill.HasValue
                    ? model.RecDateTill.Value.ToString("MM/dd/yyyy HH:mm")
                    : string.Empty;
                vm.event_pid = model.RecEventId.HasValue ? model.RecEventId.Value : 0;
                vm.rec_type = model.RecType;
                vm.rec_pattern = model.RecPattern;
                vm.event_length = model.RecEventLength.HasValue ? model.RecEventLength.Value : 0;
                vm.rec_type = model.RecType;
                vm.full_day = model.IsFullDay.HasValue && model.IsFullDay.Value;
                vm._timed = model.IsRecurring.HasValue && model.IsRecurring.Value;
                vm.text = model.Description;
                vm.WeekNumber = GetWeekOfYearIso8601(model.RecDateFrom);
            }
            return vm;
        }

        public HolidayPlannerDetailsCustomModel ConvertModelToViewModel(HolidayPlannerDetails model)
        {
            var vm = base.MapModelToViewModel(model);
            return vm;
        }

        public override HolidayPlannerDetails MapViewModelToModel(HolidayPlannerDetailsCustomModel vm)
        {
            var model = base.MapViewModelToModel(vm);
            if (model != null)
            {
                var dt = DateTime.Now;
                if (!string.IsNullOrEmpty(vm.start_date) && DateTime.TryParse(vm.start_date, out dt))
                {
                    model.RecDateFrom = dt;
                    model.HolidayDate = dt;
                    model.WeekNumber = GetWeekOfYearIso8601(dt);
                }

                if (!string.IsNullOrEmpty(vm.end_date) && DateTime.TryParse(vm.end_date, out dt))
                    model.RecDateTill = dt;

                
            }
            return model;
        }

        /// <summary>
        /// Gets the week of year is o8601.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        private int GetWeekOfYearIso8601(DateTime? date)
        {
            if (date.HasValue)
            {
                var day = (int)CultureInfo.CurrentCulture.Calendar.GetDayOfWeek(date.Value);
                return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(date.Value.AddDays(4 - (day == 0 ? 7 : day)),
                    CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            }
            return 0;
        }

        #endregion
    }
}