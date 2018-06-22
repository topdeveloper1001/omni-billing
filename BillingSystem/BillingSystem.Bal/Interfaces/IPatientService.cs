using BillingSystem.Model.EntityDto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BillingSystem.Bal.Interfaces
{
    public interface IPatientService
    {
        Task<UserDto> SavePatientDetails(PatientDto p);
        Task<List<FavoriteClinicianDto>> GetFavoriteCliniciansAsync(long patientId);
        Task<long> AddClinicianAsFavoriteAsync(FavoriteClinicianDto dt);
        int RemoveClinicianAsFavorite(long id);
    }
}
