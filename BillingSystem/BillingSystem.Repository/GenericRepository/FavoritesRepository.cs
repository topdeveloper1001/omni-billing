using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.EntityDto;
using BillingSystem.Repository.Common;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BillingSystem.Repository.GenericRepository
{
    public class FavoritesRepository : GenericRepository<UserDefinedDescriptions>
    {
        private readonly DbContext _context;

        public FavoritesRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
            _context = context;
        }

        public async Task<List<FavoriteClinicianDto>> GetFavoriteCliniciansAsync(long patientId)
        {
            var sqlParams = new SqlParameter[1] { new SqlParameter("pPatientId", patientId) };

            using (var ms = _context.MultiResultSetSqlQuery(StoredProcsiOS.iSprocGetFavoriteClinicians.ToString(), isCompiled: false, parameters: sqlParams))
            {
                var result = (await ms.ResultSetForAsync<FavoriteClinicianDto>()).ToList();
                return result;
            }
        }

        public OpenOrder GetFavoriteOrderById(long favoriteId, long facilityId, long userId)
        {
            var sqlParams = new SqlParameter[3] { new SqlParameter("pFavoriteId", favoriteId), new SqlParameter("pFacilityId", facilityId), new SqlParameter("pUserId", userId) };

            using (var ms = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetFavoriteOrderById.ToString(), isCompiled: false, parameters: sqlParams))
            {
                var result = ms.GetSingleResultWithJson<OpenOrder>(JsonResultsArray.FavoriteResult.ToString());
                return result;
            }
        }

        public async Task<long> AddClinicianAsFavorite(FavoriteClinicianDto dt)
        {
            var sqlParams = new SqlParameter[2]
            {
                new SqlParameter("pPatientId", dt.PatientId),
                new SqlParameter("pClinicianId", dt.ClinicianId)
            };

            using (var ms = _context.MultiResultSetSqlQuery(StoredProcsiOS.iSprocAddFavoriteClinician.ToString(), isCompiled: false, parameters: sqlParams))
            {
                var result = (await ms.ResultSetForAsync<long>()).FirstOrDefault();
                return result;
            }
        }
    }
}
