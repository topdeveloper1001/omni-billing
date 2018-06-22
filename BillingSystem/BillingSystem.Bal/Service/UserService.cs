using BillingSystem.Bal.Interfaces;
using System.Threading.Tasks;
using BillingSystem.Common;
using BillingSystem.Model.EntityDto;
using BillingSystem.Repository.UOW;
using System.Text.RegularExpressions;
using System.Linq;
using System.Data.Entity;
using System;
using System.Collections.Generic;

namespace BillingSystem.Bal.Service
{
    public class UserService : IUserService
    {
        private readonly UnitOfWork _unitOfWork;

        public UserService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<UserDto> AuthenticateAsync(string username, string password, string deviceToken, string platform, bool isPatient = false)
        {
            username = username.ToLower().Trim();

            var isEmail = Regex.IsMatch(username, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
            using (var rep = _unitOfWork.PatientInfoRepository)
            {
                var vm = await rep.AuthenticateAsync(username, EncryptDecrypt.GetEncryptedData(password, string.Empty), deviceToken, platform, isEmail, 0, isPatient);
                return vm;
            }
        }

        public async Task<UserDto> GetUserAsync(long userId, bool isPatient = true, string deviceToken = "", string platform = "")
        {
            using (var rep = _unitOfWork.PatientInfoRepository)
            {
                var vm = await rep.AuthenticateAsync(string.Empty, string.Empty, deviceToken, platform, false, userId, isPatient);

                if (vm != null && !string.IsNullOrEmpty(vm.Password))
                    vm.Password = EncryptDecrypt.GetDecryptedData(vm.Password, string.Empty);

                return vm;
            }
        }

        public async Task<bool> SaveUserLocationAsync(string lat, string lng, long userId, bool isPatient = true)
        {
            if (isPatient)
            {
                using (var rep = _unitOfWork.PatientLoginDetailRepository)
                {
                    var m = await rep.Where(p => p.PatientId == userId && p.IsDeleted != true).FirstOrDefaultAsync();
                    if (m != null)
                    {
                        m.Latitude = lat;
                        m.Longitude = lng;
                        m.ModifiedBy = Convert.ToInt32(userId);
                        m.ModifiedDate = DateTime.Now;
                        var result = rep.UpdateEntity(m, m.Id);
                        return result.HasValue;
                    }
                }
            }
            return false;
        }

        public async Task<List<PatientDto>> GetPatientsByUserIdAsync(long userId = 0)
        {
            using (var rep = _unitOfWork.PatientInfoRepository)
            {
                var vm = await rep.GetPatientsByUserIdAsync(userId);
                return vm;
            }
        }
    }
}
