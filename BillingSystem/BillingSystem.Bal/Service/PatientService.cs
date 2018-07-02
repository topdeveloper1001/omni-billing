using BillingSystem.Bal.Interfaces;
using BillingSystem.Common;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.EntityDto;


using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BillingSystem.Bal.Service
{
    public class PatientService : IPatientService
    {
        private readonly IRepository<PatientInfo> _repository;
        private readonly IRepository<FavoriteClinician> _favrepository;
        private readonly BillingEntities _context;
         
        public async Task<UserDto> SavePatientDetails(PatientDto p)
        {
            p.Password = EncryptDecrypt.GetEncryptedData(p.Password, string.Empty).Trim();

            if (string.IsNullOrEmpty(p.EmiratesID))
                p.EmiratesID = string.Empty;

           
                var status = await SavePatientInfoAsync(p);
                //p.Password = EncryptDecrypt.GetDecryptedData(p.Password, string.Empty).Trim();
                return status;
            
        }
        public async Task<UserDto> SavePatientInfoAsync(PatientDto p)
        {
            //If the request goes for new patient, then Email is mandatory otherwise not.
            var email = p.PatientID == 0 ? p.Email : Convert.ToString(p.Email);
            var dt = string.IsNullOrEmpty(p.DeviceToken) ? string.Empty : p.DeviceToken;
            var pl = string.IsNullOrEmpty(p.Platform) ? string.Empty : p.Platform;
            var city = !string.IsNullOrEmpty(p.City) ? p.City : string.Empty;

            var sqlParameters = new SqlParameter[14];
            sqlParameters[0] = new SqlParameter("pPid", p.PatientID);
            sqlParameters[1] = new SqlParameter("pFirstName", p.FirstName);
            sqlParameters[2] = new SqlParameter("pLastName", p.LastName);
            sqlParameters[3] = new SqlParameter("pDob", p.DOB);
            sqlParameters[4] = new SqlParameter("pEmail", email);
            sqlParameters[5] = new SqlParameter("pEmirates", p.EmiratesID);
            sqlParameters[6] = new SqlParameter("pPhone", p.PhoneNo);
            sqlParameters[7] = new SqlParameter("pPwd", p.Password);
            sqlParameters[8] = new SqlParameter("pGender", p.Gender);
            sqlParameters[9] = new SqlParameter("pDeviceToken", dt);
            sqlParameters[10] = new SqlParameter("pPlatform", pl);
            sqlParameters[11] = new SqlParameter("pCityId", p.CityId);
            sqlParameters[12] = new SqlParameter("pStateId", p.StateId);
            sqlParameters[13] = new SqlParameter("pCountryId", p.CountryId);

            using (var ms = _context.MultiResultSetSqlQuery(StoredProcsiOS.iSprocSavePatientInfo.ToString(), parameters: sqlParameters, isCompiled: false))
            {
                var isSaved = ms.ResultSetFor<bool>().FirstOrDefault();
                if (isSaved)
                {
                    var list = await ms.GetResultWithJsonAsync<UserDto>(JsonResultsArray.UserDto.ToString());
                    return list.FirstOrDefault();
                }
            }

            return Enumerable.Empty<UserDto>().FirstOrDefault();
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

        public async Task<long> AddClinicianAsFavoriteAsync(FavoriteClinicianDto dt)
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

        public int RemoveClinicianAsFavorite(long id)
        {
                var m = _favrepository.GetSingle(id);
                if (m != null && m.Id > 0)
                {
                    var status = _favrepository.Delete(m);
                    return status;
                }
                else
                    return 0;
        }
    }
}
