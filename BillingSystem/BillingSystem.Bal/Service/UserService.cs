using BillingSystem.Bal.Interfaces;
using System.Threading.Tasks;
using BillingSystem.Common;
using BillingSystem.Model.EntityDto;
using System.Text.RegularExpressions;
using System.Linq;
using System.Data.Entity;
using System;
using System.Collections.Generic;
using BillingSystem.Repository.Interfaces;
using BillingSystem.Model;
using System.Data.SqlClient;
using BillingSystem.Repository.Common;
using BillingSystem.Common.Common;

namespace BillingSystem.Bal.Service
{
    public class UserService : IUserService
    {
        private readonly IRepository<PatientLoginDetail> _pdRepository;
        private readonly BillingEntities _context;

        public UserService(IRepository<PatientLoginDetail> pdRepository, BillingEntities context)
        {
            _pdRepository = pdRepository;
            _context = context;
        }

        public async Task<UserDto> AuthenticateAsync(string username, string password, string deviceToken, string platform, bool isPatient = false)
        {
            username = username.ToLower().Trim();

            var isEmail = Regex.IsMatch(username, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
            var sqlParameters = new SqlParameter[7];
            sqlParameters[0] = new SqlParameter("pUsername", username);
            sqlParameters[1] = new SqlParameter("pPassword", password);
            sqlParameters[2] = new SqlParameter("pDeviceToken", deviceToken);
            sqlParameters[3] = new SqlParameter("pPlatform", platform);
            sqlParameters[4] = new SqlParameter("pIsEmail", isEmail);
            sqlParameters[5] = new SqlParameter("pUserId", 0);
            sqlParameters[6] = new SqlParameter("pIsPatient", isPatient);

            using (var multiResultSet = _context.MultiResultSetSqlQuery(StoredProcsiOS.iSprocAuthenticateUser.ToString(), parameters: sqlParameters, isCompiled: false))
            {
                var reader = await multiResultSet.GetReaderAsync();
                var isAuthenticated = multiResultSet.ResultSetFor<bool>(reader).FirstOrDefault();
                var isNext = isAuthenticated ? await reader.NextResultAsync() : false;
                if (isAuthenticated && isNext)
                {
                    var result = GenericHelper.GetJsonResponse<UserDto>(reader, "UserDto");
                    return result != null && result.Count > 0 ? result.FirstOrDefault() : null;
                }
            }
            return Enumerable.Empty<UserDto>().FirstOrDefault();
        }
        public async Task<UserDto> AuthenticateAsync(string username, string password, string deviceToken, string platform, bool isEmail, long userId = 0, bool isPatient = false)
        {
            var sqlParameters = new SqlParameter[7];
            sqlParameters[0] = new SqlParameter("pUsername", username);
            sqlParameters[1] = new SqlParameter("pPassword", password);
            sqlParameters[2] = new SqlParameter("pDeviceToken", deviceToken);
            sqlParameters[3] = new SqlParameter("pPlatform", platform);
            sqlParameters[4] = new SqlParameter("pIsEmail", isEmail);
            sqlParameters[5] = new SqlParameter("pUserId", userId);
            sqlParameters[6] = new SqlParameter("pIsPatient", isPatient);

            using (var multiResultSet = _context.MultiResultSetSqlQuery(StoredProcsiOS.iSprocAuthenticateUser.ToString(), parameters: sqlParameters, isCompiled: false))
            {
                var reader = await multiResultSet.GetReaderAsync();
                var isAuthenticated = multiResultSet.ResultSetFor<bool>(reader).FirstOrDefault();
                var isNext = isAuthenticated ? await reader.NextResultAsync() : false;
                if (isAuthenticated && isNext)
                {
                    var result = GenericHelper.GetJsonResponse<UserDto>(reader, "UserDto");
                    return result != null && result.Count > 0 ? result.FirstOrDefault() : null;
                }
            }
            return Enumerable.Empty<UserDto>().FirstOrDefault();
        }

        public async Task<UserDto> GetUserAsync(long userId, bool isPatient = true, string deviceToken = "", string platform = "")
        {
            var vm = await AuthenticateAsync(string.Empty, string.Empty, deviceToken, platform, false, userId, isPatient);

            if (vm != null && !string.IsNullOrEmpty(vm.Password))
                vm.Password = EncryptDecrypt.GetDecryptedData(vm.Password, string.Empty);

            return vm;
        }

        public async Task<bool> SaveUserLocationAsync(string lat, string lng, long userId, bool isPatient = true)
        {
            if (isPatient)
            {
                var m = await _pdRepository.Where(p => p.PatientId == userId && p.IsDeleted != true).FirstOrDefaultAsync();
                if (m != null)
                {
                    m.Latitude = lat;
                    m.Longitude = lng;
                    m.ModifiedBy = Convert.ToInt32(userId);
                    m.ModifiedDate = DateTime.Now;
                    var result = _pdRepository.Updatei(m, m.Id);
                    return result > 0;
                }
            }

            return false;
        }

        public async Task<List<PatientDto>> GetPatientsByUserIdAsync(long userId = 0)
        {
            var sqlParams = new SqlParameter[1];
            sqlParams[0] = new SqlParameter("pUserId", userId);
            using (var ms = _context.MultiResultSetSqlQuery(StoredProcsiOS.iSprocGetPatientsByUserId.ToString(), isCompiled: false, parameters: sqlParams))
            {
                var result = await ms.GetResultWithJsonAsync<PatientDto>(JsonResultsArray.PatientSearchResults.ToString());
                return result;
            }
        }
    }
}
