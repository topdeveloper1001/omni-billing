using System.Threading.Tasks;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Model.EntityDto;
using System.Collections.Generic;
using System;
using BillingSystem.Common.Common;
using BillingSystem.Common.Requests;
using BillingSystem.Repository.Interfaces;
using BillingSystem.Model;
using System.Data.SqlClient;
using BillingSystem.Common;
using BillingSystem.Repository.Common;
using System.Linq;
using System.Data;
using System.Globalization;

namespace BillingSystem.Bal.Service
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IRepository<GlobalCodes> _gRepository;
        private readonly IRepository<Scheduling> _schRepository;
        private readonly BillingEntities _context;

        public AppointmentService(IRepository<GlobalCodes> gRepository, IRepository<Scheduling> schRepository, BillingEntities context)
        {
            _gRepository = gRepository;
            _schRepository = schRepository;
            _context = context;
        }

        public async Task<ResponseData> BookAnAppointmentAsync(AppointmentDto a)
        {
            var clinicianRefferedBy = a.ClinicianReferredBy ?? a.ClinicianId;
            var appDetails = !string.IsNullOrEmpty(a.AppointmentDetails) ? a.AppointmentDetails : string.Empty;
            var title = !string.IsNullOrEmpty(a.Title) ? a.Title : string.Empty;

            var rn = new Random(DateTime.Now.Ticks.GetHashCode());
            var eventParentId = Convert.ToString(Math.Abs(rn.Next(int.MinValue, int.MaxValue)));
            var eventId = $"{eventParentId}1";
            var weekDay = Convert.ToString(CommonHelper.GetWeekOfYearISO8601(a.ScheduleDate));
            var loginToken = CommonHelper.GenerateToken(8);

            var sqlParams = new SqlParameter[20];
            sqlParams[0] = new SqlParameter("@pId", a.Id);
            sqlParams[1] = new SqlParameter("@pPatientId", a.PatientId);
            sqlParams[2] = new SqlParameter("@pClinicianId", a.ClinicianId);
            sqlParams[3] = new SqlParameter("@pAppointmentTypeId", a.AppointmentTypeId);
            sqlParams[4] = new SqlParameter("@pSpecialty", a.Specialty);
            sqlParams[5] = new SqlParameter("@pFacilityId", a.FacilityId.GetValueOrDefault());
            sqlParams[6] = new SqlParameter("@pAppointmentDate", a.ScheduleDate);
            sqlParams[7] = new SqlParameter("@pTimeFrom", a.TimeFrom);
            sqlParams[8] = new SqlParameter("@pTimeTill", a.TimeTill);
            sqlParams[9] = new SqlParameter("@pCreatedBy", a.PatientId);
            sqlParams[10] = new SqlParameter("@pTitle", title);
            sqlParams[11] = new SqlParameter("@pClinicianReferredBy", clinicianRefferedBy);
            sqlParams[12] = new SqlParameter("@pAppDetails", appDetails);
            sqlParams[13] = new SqlParameter("@pCountryId", a.CountryId);
            sqlParams[14] = new SqlParameter("@pStateId", a.StateId);
            sqlParams[15] = new SqlParameter("@pCityId", a.CityId);
            sqlParams[16] = new SqlParameter("@pEventId", eventId);
            sqlParams[17] = new SqlParameter("@pEventParentId", eventParentId);
            sqlParams[18] = new SqlParameter("@pWeekDay", weekDay);
            sqlParams[19] = new SqlParameter("@pToken", loginToken);

            using (var ms = _context.MultiResultSetSqlQuery(StoredProcsiOS.iSprocBookAnAppointment.ToString(), isCompiled: false, parameters: sqlParams))
            {
                var result = (await ms.ResultSetForAsync<ResponseData>()).FirstOrDefault();
                return result;
            }
        }

        public async Task<List<AppointmentTypeDto>> GetAppointmentTypesAsync()
        {
            using (var multiResultSet = _context.MultiResultSetSqlQuery(StoredProcsiOS.iSprocGetAppointmentTypes.ToString(), isCompiled: false))
            {
                var reader = await multiResultSet.GetReaderAsync();
                var result = GenericHelper.GetJsonResponse<AppointmentTypeDto>(reader, "AppointmentTypes");
                return result;
            }
        }

        public async Task<IEnumerable<TimeSlotsDto>> GetAvailableTimeSlotsAsync(long clinicianId, DateTime appointmentDate, long appointmentTypeId, bool isFirst, long specialtyId, string timeFrom = "", string timeTo = "", int maxCount = 5, long facilityId = 0)
        {
            var sqlParameters = new SqlParameter[10];
            sqlParameters[0] = new SqlParameter("pFromDate", appointmentDate);
            sqlParameters[1] = new SqlParameter("pToDate", appointmentDate);
            sqlParameters[2] = new SqlParameter("pAppointMentType", appointmentTypeId);
            sqlParameters[3] = new SqlParameter("pPhysicianId", clinicianId);
            sqlParameters[4] = new SqlParameter("pSpecialtyId", specialtyId);
            sqlParameters[5] = new SqlParameter("pFirst", isFirst);
            sqlParameters[6] = new SqlParameter("pTimeFrom", timeFrom);
            sqlParameters[7] = new SqlParameter("pTimeTill", timeTo);
            sqlParameters[8] = new SqlParameter("pMaxTimeSlotsCount", maxCount);
            sqlParameters[9] = new SqlParameter("pFacilityId", facilityId);

            using (var multiResultSet = _context.MultiResultSetSqlQuery(StoredProcsiOS.iSprocGetAvailableTimeSlots.ToString(), isCompiled: false, parameters: sqlParameters))
            {
                var result = await multiResultSet.GetResultWithJsonAsync<TimeSlotsDto>(JsonResultsArray.TimeSlotsData.ToString());
                //var result = await multiResultSet.GetJsonStringResult();
                return result;
            }
        }

        public async Task<IEnumerable<BookedAppointmentDto>> GetBookedAppointmentsAsync(UpcomingAppointmentsRequest m)
        {
            var status = !string.IsNullOrEmpty(m.StatusId) ? m.StatusId : string.Empty;

            var sqlParams = new SqlParameter[8];
            sqlParams[0] = new SqlParameter("pPatientId", m.PatientId.GetValueOrDefault());
            sqlParams[1] = new SqlParameter("pUserId", m.LoggedInUserId.GetValueOrDefault());
            sqlParams[2] = new SqlParameter("pOnlyUpcoming", SqlDbType.Bit);

            if (m.ShowsUpcomingOnly.HasValue)
                sqlParams[2].Value = m.ShowsUpcomingOnly.Value;
            else
                sqlParams[2].Value = DBNull.Value;

            sqlParams[3] = new SqlParameter("pId", m.Id.GetValueOrDefault());
            sqlParams[4] = new SqlParameter("pForToday", m.ForToday.GetValueOrDefault());
            sqlParams[5] = new SqlParameter("pFacilityId", m.FacilityId.GetValueOrDefault());
            sqlParams[6] = new SqlParameter("pStatus", status);
            sqlParams[7] = new SqlParameter("pAppointmentDate", SqlDbType.DateTime);

            if (m.AppointmentDate.HasValue)
                sqlParams[7].Value = m.AppointmentDate.Value;
            else
                sqlParams[7].Value = DBNull.Value;

            using (var ms = _context.MultiResultSetSqlQuery(StoredProcsiOS.iSprocGetBookedAppointments.ToString(), isCompiled: false, parameters: sqlParams))
            {
                var result = (await ms.ResultSetForAsync<BookedAppointmentDto>()).ToList();

                if (result != null && result.Any())
                    result = m.ShowsUpcomingOnly.HasValue ?
                        result.OrderBy(d => d.AppointmentDate)
                        .ThenBy(t => DateTime.ParseExact(t.TimeFrom, "HH:mm", CultureInfo.InvariantCulture)).ToList()
                        : result.OrderByDescending(d => d.AppointmentDate)
                        .ThenByDescending(t => DateTime.ParseExact(t.TimeFrom, "HH:mm", CultureInfo.InvariantCulture)).ToList();

                return result;
            }
        }

        public bool CancelAppointment(long appointmentId, int userId, string reason = "")
        {
            var model = _schRepository.GetSingle(appointmentId);
            if (model != null)
            {
                model.Status = "4"; //Cancel Status
                model.ModifiedBy = userId;
                model.ModifiedDate = DateTime.Now;

                if (!string.IsNullOrEmpty(reason))
                    model.Comments = reason;

                var result = _schRepository.Updatei(model, model.SchedulingId);
                return result > 0;

            }
            return false;
        }

        public async Task<IEnumerable<ClinicianDto>> GetCliniciansAndTheirTimeSlotsAsync(AvailableTimeSlotsRequest r)
        {
            var mainData = new List<ClinicianDto>();
            if (!TimeSpan.TryParse(r.TimeFrom, out TimeSpan fTime) || !TimeSpan.TryParse(r.TimeTill, out TimeSpan tTime))
            {
                r.TimeFrom = string.Empty;
                r.TimeTill = string.Empty;
            }

            var sqlParams = new SqlParameter[11];
            sqlParams[0] = new SqlParameter("pAppointmentDate", r.AppointmentDate);
            sqlParams[1] = new SqlParameter("pAppointMentType", r.AppointmentTypeId);
            sqlParams[2] = new SqlParameter("pPhysicianId", r.ClinicianId.GetValueOrDefault());
            sqlParams[3] = new SqlParameter("pSpecialtyId", r.SpecialtyId.GetValueOrDefault());
            sqlParams[4] = new SqlParameter("pFirst", r.IsFirst);
            sqlParams[5] = new SqlParameter("pCityId", r.CityId.GetValueOrDefault());
            sqlParams[6] = new SqlParameter("pTimeFrom", !string.IsNullOrEmpty(r.TimeFrom) ? r.TimeFrom.Trim() : string.Empty);
            sqlParams[7] = new SqlParameter("pTimeTill", !string.IsNullOrEmpty(r.TimeTill) ? r.TimeTill.Trim() : string.Empty);
            sqlParams[8] = new SqlParameter("pMaxTimeSlotsCount", r.RecordsCountRequested.GetValueOrDefault());
            sqlParams[9] = new SqlParameter("pFacilityId", r.FacilityId.GetValueOrDefault());
            sqlParams[10] = new SqlParameter("pStateId", r.StateId.GetValueOrDefault());

            using (var ms = _context.MultiResultSetSqlQuery(StoredProcsiOS.iSprocGetCliniciansAndTheirTimeSlots.ToString(), isCompiled: false, parameters: sqlParams))
                mainData = (await ms.GetResultWithJsonAsync<ClinicianDto>(JsonResultsArray.TimeSlotsData.ToString())).ToList();

            return mainData;
        }


        public async Task<string> SaveCliniciansOffTimingsAsync(VacationDto m, long loggedInUserId)
        {
            var sqlParams = new SqlParameter[15];

            if (m.FullDay)
            {
                m.TimeFrom = "00:00";
                m.TimeTo = "23:59";
            }

            sqlParams[0] = new SqlParameter("@Id", m.Id);
            sqlParams[1] = new SqlParameter("@ClinicianId", m.ClinicianId);
            sqlParams[2] = new SqlParameter("@ReasonId", m.ReasonId);
            sqlParams[3] = new SqlParameter("@Comments", string.IsNullOrEmpty(m.Comments) ? string.Empty : m.Comments);
            sqlParams[4] = new SqlParameter("@RosterTypeId", "1");
            sqlParams[5] = new SqlParameter("@DateFrom", m.DateFrom);
            sqlParams[6] = new SqlParameter("@TimeFrom", m.TimeFrom);
            sqlParams[7] = new SqlParameter("@DateTo", m.DateTo);
            sqlParams[8] = new SqlParameter("@TimeTo", m.TimeTo);
            sqlParams[9] = new SqlParameter("@CId", m.CorporateId);
            sqlParams[10] = new SqlParameter("@FId", m.FacilityId);
            sqlParams[11] = new SqlParameter("@DaysOfWeek", "ALL");
            sqlParams[12] = new SqlParameter("@UserId", loggedInUserId);
            sqlParams[13] = new SqlParameter("@ExtValue1", m.FullDay ? "1" : "0");
            sqlParams[14] = new SqlParameter("@ExtValue2", string.Empty);

            using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SprocSaveClinicianRoster.ToString(), false, parameters: sqlParams))
            {
                var saveResult = (await r.ResultSetForAsync<string>()).FirstOrDefault();
                return saveResult;
            }
        }

        public bool CompleteAppointment(long appointmentId, int userId, string reason = "")
        {
            var model = _schRepository.GetSingle(appointmentId);
            if (model != null)
            {
                model.Status = "14"; //Service Administered
                model.ModifiedBy = userId;
                model.ModifiedDate = DateTime.Now;

                if (!string.IsNullOrEmpty(reason))
                    model.Comments = reason;

                var result = _schRepository.Updatei(model, model.SchedulingId);
                return result > 0;
            }
            return false;
        }
    }
}
