// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HolidayPlannerDetails.cs" company="Spadez">
//   OmniHealthcare
// </copyright>
// <summary>
//   The holiday planner details.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BillingSystem.Model
{
    using System;

    public class HolidayPlannerDetails
    {
        public int Id { get; set; }

        public int? HolidayPlannerId { get; set; }

        public DateTime? HolidayDate { get; set; }

        public bool? IsActive { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string Description { get; set; }

        public string SlotColor { get; set; }

        public string EventId { get; set; }

        public bool? IsRecurring { get; set; }

        public string RecType { get; set; }

        public string RecPattern { get; set; }

        public long? RecEventLength { get; set; }

        public int? RecEventId { get; set; }

        public DateTime? RecDateFrom { get; set; }

        public DateTime? RecDateTill { get; set; }

        public int WeekNumber { get; set; }

        public bool? IsFullDay { get; set; }
    }
}