using System.Threading.Tasks;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Model.EntityDto;
using BillingSystem.Repository.UOW;
using System.Collections.Generic;
using System;
using BillingSystem.Common.Common;
using BillingSystem.Common.Requests;

namespace BillingSystem.Bal.Service
{
    public class AppointmentService : IAppointmentService
    {
        private readonly UnitOfWork _uow;

        public AppointmentService(UnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ResponseData> BookAnAppointmentAsync(AppointmentDto a)
        {
            using (var rep = _uow.SchedulingRepository)
                return await rep.BookAnAppointmentAsync(a);
        }

        public async Task<List<AppointmentTypeDto>> GetAppointmentTypesAsync()
        {
            using (var rep = _uow.AppointmentTypesRepository)
                return await rep.GetAppointmentTypesAsync();
        }

        public async Task<IEnumerable<TimeSlotsDto>> GetAvailableTimeSlotsAsync(long clinicianId, DateTime appointmentDate, long appointmentTypeId, bool isFirst, long specialtyId, string timeFrom = "", string timeTo = "", int maxCount = 5, long facilityId = 0)
        {
            using (var rep = _uow.SchedulingRepository)
                return await rep.GetAvailableTimeSlotsAsync(clinicianId, appointmentDate, appointmentTypeId, specialtyId, isFirst, facilityId, timeFrom, timeTo, maxCount);
        }

        public async Task<IEnumerable<BookedAppointmentDto>> GetBookedAppointmentsAsync(UpcomingAppointmentsRequest m)
        {
            using (var rep = _uow.SchedulingRepository)
            {
                var result = await rep.GetBookedAppointments(m);
                return result;
            }
        }

        public bool CancelAppointment(long appointmentId, int userId, string reason = "")
        {
            using (var rep = _uow.SchedulingRepository)
            {
                var model = rep.GetSingle(appointmentId);
                if (model != null)
                {
                    model.Status = "4"; //Cancel Status
                    model.ModifiedBy = userId;
                    model.ModifiedDate = DateTime.Now;

                    if (!string.IsNullOrEmpty(reason))
                        model.Comments = reason;

                    var result = rep.UpdateEntity(model, model.SchedulingId);
                    return result.HasValue && result.Value > 0;
                }
            }
            return false;
        }

        public async Task<IEnumerable<ClinicianDto>> GetCliniciansAndTheirTimeSlotsAsync(AvailableTimeSlotsRequest r)
        {
            using (var rep = _uow.SchedulingRepository)
            {
                var result = await rep.GetCliniciansAndTheirTimeSlotsAsync(r);
                return result;
            }
        }


        public async Task<string> SaveCliniciansOffTimingsAsync(VacationDto m, long loggedInUserId)
        {
            using (var rep = _uow.ClinicianRosterRepository)
            {
                var result = await rep.SaveClinicianOffTimings(m, loggedInUserId);
                return result;
            }
        }

        public bool CompleteAppointment(long appointmentId, int userId, string reason = "")
        {
            using (var rep = _uow.SchedulingRepository)
            {
                var model = rep.GetSingle(appointmentId);
                if (model != null)
                {
                    model.Status = "14"; //Service Administered
                    model.ModifiedBy = userId;
                    model.ModifiedDate = DateTime.Now;

                    if (!string.IsNullOrEmpty(reason))
                        model.Comments = reason;

                    var result = rep.UpdateEntity(model, model.SchedulingId);
                    return result.HasValue && result.Value > 0;
                }
            }
            return false;
        }
    }
}
