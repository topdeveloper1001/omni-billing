using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Repository.Common;
using System.Threading.Tasks;
using BillingSystem.Model.EntityDto;

namespace BillingSystem.Repository.GenericRepository
{
    public class ClinicianRosterRepository : GenericRepository<ClinicianRoster>
    {
        private readonly DbContext _context;

        public ClinicianRosterRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
            _context = context;
        }

        public async Task<IEnumerable<ClinicianRosterCustomModel>> GetSingleOrList(long cId, long fId, long userId, bool aStatus = true, long id = 0)
        {
            try
            {
                var sqlParameters = new SqlParameter[5];
                sqlParameters[0] = new SqlParameter("@CId", cId);
                sqlParameters[1] = new SqlParameter("@FId", fId);
                sqlParameters[2] = new SqlParameter("@UserId", userId);
                sqlParameters[3] = new SqlParameter("@Id", id);
                sqlParameters[4] = new SqlParameter("@AStatus", aStatus);

                using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetClinicianRosterList.ToString(), false, parameters: sqlParameters))
                {
                    var result = (await r.ResultSetForAsync<ClinicianRosterCustomModel>()).ToList();
                    return result;
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            //return new List<ClinicianRosterCustomModel>();
        }


        public async Task<IEnumerable<ClinicianRosterCustomModel>> SaveRecord(ClinicianRoster m, long userId)
        {
            try
            {
                var sqlParams = new SqlParameter[15];

                sqlParams[0] = new SqlParameter("@Id", m.Id);
                sqlParams[1] = new SqlParameter("@ClinicianId", m.ClinicianId);
                sqlParams[2] = new SqlParameter("@ReasonId", m.ReasonId);
                sqlParams[3] = new SqlParameter("@Comments", string.IsNullOrEmpty(m.Comments) ? string.Empty : m.Comments);
                sqlParams[4] = new SqlParameter("@RosterTypeId", m.RosterTypeId);
                sqlParams[5] = new SqlParameter("@DateFrom", m.DateFrom);
                sqlParams[6] = new SqlParameter("@TimeFrom", m.TimeFrom);
                sqlParams[7] = new SqlParameter("@DateTo", m.DateTo);
                sqlParams[8] = new SqlParameter("@TimeTo", m.TimeTo);
                sqlParams[9] = new SqlParameter("@CId", m.CorporateId);
                sqlParams[10] = new SqlParameter("@FId", m.FacilityId);
                sqlParams[11] = new SqlParameter("@DaysOfWeek", m.RepeatitiveDaysInWeek);
                sqlParams[12] = new SqlParameter("@UserId", userId);
                sqlParams[13] = new SqlParameter("@ExtValue1", string.IsNullOrEmpty(m.ExtValue1) ? string.Empty : m.ExtValue1);
                sqlParams[14] = new SqlParameter("@ExtValue2", string.IsNullOrEmpty(m.ExtValue2) ? string.Empty : m.ExtValue2);

                using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SprocSaveClinicianRoster.ToString(), false, parameters: sqlParams))
                {
                    //Already existing records of current physicians
                    var existingRecords = (await r.ResultSetForAsync<SchedulingCustomModel>()).ToList();

                    var saveResult = (await r.ResultSetForAsync<string>()).FirstOrDefault();
                    if (long.TryParse(saveResult, out long newId))
                    {
                        var list = (await r.ResultSetForAsync<ClinicianRosterCustomModel>()).ToList();
                        return list;
                    }
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            return new List<ClinicianRosterCustomModel>();
        }


        public async Task<string> SaveRecordV2(ClinicianRoster m, long userId)
        {
            try
            {
                var sqlParams = new SqlParameter[15];

                sqlParams[0] = new SqlParameter("@Id", m.Id);
                sqlParams[1] = new SqlParameter("@ClinicianId", m.ClinicianId);
                sqlParams[2] = new SqlParameter("@ReasonId", m.ReasonId);
                sqlParams[3] = new SqlParameter("@Comments", string.IsNullOrEmpty(m.Comments) ? string.Empty : m.Comments);
                sqlParams[4] = new SqlParameter("@RosterTypeId", m.RosterTypeId);
                sqlParams[5] = new SqlParameter("@DateFrom", m.DateFrom);
                sqlParams[6] = new SqlParameter("@TimeFrom", !string.IsNullOrEmpty(m.TimeFrom) ? m.TimeFrom : "00:00");
                sqlParams[7] = new SqlParameter("@DateTo", m.DateTo);
                sqlParams[8] = new SqlParameter("@TimeTo", !string.IsNullOrEmpty(m.TimeTo) ? m.TimeTo : "23:59");
                sqlParams[9] = new SqlParameter("@CId", m.CorporateId);
                sqlParams[10] = new SqlParameter("@FId", m.FacilityId);
                sqlParams[11] = new SqlParameter("@DaysOfWeek", m.RepeatitiveDaysInWeek);
                sqlParams[12] = new SqlParameter("@UserId", userId);
                sqlParams[13] = new SqlParameter("@ExtValue1", string.IsNullOrEmpty(m.ExtValue1) ? string.Empty : m.ExtValue1);
                sqlParams[14] = new SqlParameter("@ExtValue2", string.IsNullOrEmpty(m.ExtValue2) ? string.Empty : m.ExtValue2);

                using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SprocSaveClinicianRoster.ToString(), false, parameters: sqlParams))
                {
                    var saveResult = (await r.ResultSetForAsync<string>()).FirstOrDefault();
                    return saveResult;
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<ClinicianRosterCustomModel>> DeleteRecord(long cId, long fId, long userId, long id)
        {
            var sqlParams = new SqlParameter[4];
            sqlParams[0] = new SqlParameter("@Id", id);
            sqlParams[1] = new SqlParameter("@CId", cId);
            sqlParams[2] = new SqlParameter("@FId", fId);
            sqlParams[3] = new SqlParameter("@UserId", userId);

            using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SprocDeleteClinicianRoster.ToString(), false, parameters: sqlParams))
            {
                var list = (await r.ResultSetForAsync<ClinicianRosterCustomModel>()).ToList();
                return list;
            }
        }

        public async Task<string> SaveClinicianOffTimings(VacationDto m, long loggedInUserId)
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
    }
}
