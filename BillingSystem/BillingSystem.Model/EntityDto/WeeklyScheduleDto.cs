using System.Collections.Generic;

namespace BillingSystem.Model.EntityDto
{
    public class WeeklyScheduleDto
    {
        public List<BookedPercentageData> WeeklyBookedPercentage { get; set; }
        public string PatientsSeenInCurrentMonth { get; set; }
        public string YtdPatientsSeen { get; set; }
        public string AveragePatientsSeenPerMonth { get; set; }
    }

    public class BookedPercentageData
    {
        public string Monday { get; set; }
        public string Tuesday { get; set; }
        public string Wednesday { get; set; }
        public string Thursday { get; set; }
        public string Friday { get; set; }
        public string Saturday { get; set; }
        public string Sunday { get; set; }
    }
}
