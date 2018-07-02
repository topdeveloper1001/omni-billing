using System.Collections.Generic;
using System.Linq;
using BillingSystem.Model.CustomModel;
using System.Threading.Tasks;

using BillingSystem.Model;
using System.Data.SqlClient;
using BillingSystem.Common.Common;

using AutoMapper;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class ClinicianRosterService : IClinicianRosterService
    {
        private readonly IRepository<ClinicianRoster> _repository;
        private readonly BillingEntities _context;
        private readonly IMapper _mapper;

        public ClinicianRosterService(IRepository<ClinicianRoster> repository, BillingEntities context, IMapper mapper)
        {
            _repository = repository;
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ClinicianRosterCustomModel>> GetAll(long facilityId, long corporateId, long userId, long id = 0, bool aStatus = true)
        {
            return await GetSingleOrList(corporateId, facilityId, userId, aStatus, id);
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

        public async Task<ClinicianRosterCustomModel> GetSingle(long facilityId, long corporateId, long userId, long id = 0, bool aStatus = true)
        {
            var list = await GetSingleOrList(corporateId, facilityId, userId, aStatus, id);
            var vm = list.FirstOrDefault();
            return vm;
        }

        public async Task<string> Save(ClinicianRosterCustomModel vm, long userId)
        {
            var m = _mapper.Map<ClinicianRoster>(vm);
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

        public async Task<IEnumerable<ClinicianRosterCustomModel>> Delete(long facilityId, long corporateId, long userId, long id = 0, bool aStatus = true)
        {
            var sqlParams = new SqlParameter[4];
            sqlParams[0] = new SqlParameter("@Id", id);
            sqlParams[1] = new SqlParameter("@CId", corporateId);
            sqlParams[2] = new SqlParameter("@FId", facilityId);
            sqlParams[3] = new SqlParameter("@UserId", userId);

            using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SprocDeleteClinicianRoster.ToString(), false, parameters: sqlParams))
            {
                var list = (await r.ResultSetForAsync<ClinicianRosterCustomModel>()).ToList();
                return list;
            }
        }
    }
}
