using BillingSystem.Common.Common;
using BillingSystem.Common.Requests;
using BillingSystem.Model.EntityDto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BillingSystem.Bal.Interfaces
{
    public interface IAppointmentService
    {
        Task<List<AppointmentTypeDto>> GetAppointmentTypesAsync();
        Task<ResponseData> BookAnAppointmentAsync(AppointmentDto a);
        Task<IEnumerable<BookedAppointmentDto>> GetBookedAppointmentsAsync(UpcomingAppointmentsRequest m);
        bool CancelAppointment(long appointmentId, int userId, string reason = "");
        bool CompleteAppointment(long appointmentId, int userId, string reason = "");
        Task<IEnumerable<ClinicianDto>> GetCliniciansAndTheirTimeSlotsAsync(AvailableTimeSlotsRequest r);
        Task<string> SaveCliniciansOffTimingsAsync(VacationDto m, long loggedInUserId);
    }
}
