using BillingSystem.Bal.Interfaces;
using BillingSystem.Common;
using BillingSystem.Common.Common;
using BillingSystem.Model.EntityDto;
using BillingSystem.Repository.UOW;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BillingSystem.Bal.Service
{
    public class PatientService : IPatientService
    {
        private readonly UnitOfWork _unitOfWork;
        public PatientService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<UserDto> SavePatientDetails(PatientDto p)
        {
            p.Password = EncryptDecrypt.GetEncryptedData(p.Password, string.Empty).Trim();

            if (string.IsNullOrEmpty(p.EmiratesID))
                p.EmiratesID = string.Empty;

            using (var rep = _unitOfWork.PatientInfoRepository)
            {
                var status = await rep.SavePatientInfoAsync(p);
                //p.Password = EncryptDecrypt.GetDecryptedData(p.Password, string.Empty).Trim();
                return status;
            }
        }

        public async Task<List<FavoriteClinicianDto>> GetFavoriteCliniciansAsync(long patientId)
        {
            using (var rep = _unitOfWork.FavoritesRepository)
            {
                var results = await rep.GetFavoriteCliniciansAsync(patientId);
                return results;
            }
        }

        public async Task<long> AddClinicianAsFavoriteAsync(FavoriteClinicianDto dt)
        {
            using (var rep = _unitOfWork.FavoritesRepository)
            {
                var result = await rep.AddClinicianAsFavorite(dt);
                return result;
            }
        }

        public int RemoveClinicianAsFavorite(long id)
        {
            using (var rep = _unitOfWork.FavoriteClinicianRepository)
            {
                var m = rep.GetSingle(id);
                if (m != null && m.Id > 0)
                {
                    var status = rep.Delete(m);
                    return status ?? 0;
                }
                else
                    return 0;
            }
        }
    }
}
