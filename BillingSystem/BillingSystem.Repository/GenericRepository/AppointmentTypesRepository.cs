using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using System.Threading.Tasks;
using BillingSystem.Repository.Common;
using BillingSystem.Model.EntityDto;

namespace BillingSystem.Repository.GenericRepository
{
    public class AppointmentTypesRepository : GenericRepository<AppointmentTypes>
    {
        private readonly DbContext _context;

        public AppointmentTypesRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
            _context = context;
        }

        public List<AppointmentTypesCustomModel> GetAppointmentTypesData(int corporateId, int facilityId, bool showInActive)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @FacilityId,@CorporateId, @ShowInActive", StoredProcedures.SPORC_GetAppointmentTypes);
                    var sqlParameters = new SqlParameter[3];
                    sqlParameters[0] = new SqlParameter("FacilityId", facilityId);
                    sqlParameters[1] = new SqlParameter("CorporateId", corporateId);
                    sqlParameters[2] = new SqlParameter("ShowInActive", showInActive);
                    IEnumerable<AppointmentTypesCustomModel> result = _context.Database.SqlQuery<AppointmentTypesCustomModel>(spName, sqlParameters);
                    return result.ToList();
                }
                return null;
            }
            catch (System.Exception ex)
            {
                throw ex;
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

        public async Task<List<ClinicianDto>> GetCliniciansBySpecialyAsync(int specialityId)
        {
            var sqlParameters = new SqlParameter[1];
            sqlParameters[0] = new SqlParameter("sId", specialityId);

            using (var multiResultSet = _context.MultiResultSetSqlQuery(StoredProcsiOS.iSprocGetCliniciansBySpecialty.ToString(), isCompiled: false, parameters: sqlParameters))
            {
                var reader = await multiResultSet.GetReaderAsync();
                var result = GenericHelper.GetJsonResponse<ClinicianDto>(reader, "Clinicians");
                return result;
            }
        }

        public async Task<List<LocationDto>> GetLocationsByClinicianAsync(int clincianId)
        {
            var sqlParameters = new SqlParameter[1];
            sqlParameters[0] = new SqlParameter("clinicianId", clincianId);

            using (var multiResultSet = _context.MultiResultSetSqlQuery(StoredProcsiOS.iSprocGetLocationsByClinician.ToString(), isCompiled: false, parameters: sqlParameters))
            {
                var reader = await multiResultSet.GetReaderAsync();
                var result = GenericHelper.GetJsonResponse<LocationDto>(reader, "Locations");
                return result;
            }
        }
    }
}
