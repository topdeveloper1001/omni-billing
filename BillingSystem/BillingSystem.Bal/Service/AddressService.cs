using BillingSystem.Bal.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BillingSystem.Common;
using BillingSystem.Repository.UOW;
using System.Data.Entity;
using BillingSystem.Model.EntityDto;
using System;

namespace BillingSystem.Bal.Service
{
    public class AddressService : IAddressService
    {
        private readonly UnitOfWork _uow;

        public AddressService(UnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<SelectList>> GetCountriesAsync()
        {
            using (var rep = _uow.CountryRepository)
            {
                var list = await rep.Where(a => a.IsDeleted != true)
                    .Select(c => new SelectList
                    {
                        Name = c.CountryName.Trim(),
                        Value = c.CountryID
                    }).OrderBy(a => a.Name).ToListAsync();

                return list;
            }
        }

        public async Task<IEnumerable<SelectList>> GetCitiesAsync(long stateId)
        {
            using (var rep = _uow.CityRepository)
            {
                var list = await rep.GetCitiesByStateAsync(stateId);
                return list;
            }
        }

        public async Task<List<SelectList>> GetStatesAsync(long countryId)
        {
            using (var rep = _uow.StateRepository)
            {
                var list = await rep.Where(a => a.IsActive && !a.IsDeleted && a.CountryID == countryId)
                    .Select(c => new SelectList
                    {
                        Name = c.StateName.Trim(),
                        Value = c.StateID
                    }).OrderBy(a => a.Name).ToListAsync();

                return list;
            }
        }

        public async Task<DefaultDataDto> GetDefaultDataAsync(long userId)
        {
            using (var rep = _uow.PatientInfoRepository)
                return await rep.GetDefaultDataAsync(userId);
        }

        public async Task<List<SelectList>> GetEntitiesByCityAsync(long cityId = 0, long corporateId = 0)
        {
            var entities = new List<SelectList>();
            var c = Convert.ToString(cityId);
            using (var rep = _uow.FacilityRepository)
            {
                var list = await rep.Where(f => (cityId == 0 || f.FacilityCity.Equals(c)) && (corporateId == 0 || f.CorporateID == corporateId)).ToListAsync();
                if (list.Any())
                    foreach (var f in list)
                    {
                        entities.Add(new SelectList
                        {
                            Name = f.FacilityName,
                            Value = Convert.ToInt64(f.FacilityId)
                        });
                    }
            }
            return entities;
        }

        public async Task<List<SelectList>> GetCorporatesAsync(long corporateId = 0, long userId = 0)
        {
            var entities = new List<SelectList>();

            if (userId > 0)
            {
                using (var rep = _uow.UsersRepository)
                    corporateId = await rep.Where(a => a.UserID == userId).Select(c => c.CorporateId.Value).FirstOrDefaultAsync();
            }

            using (var rep = _uow.CorporateRepository)
            {
                var list = await rep.Where(f => (corporateId == 0 || f.CorporateID == corporateId)).ToListAsync();
                if (list.Any())
                    foreach (var f in list)
                    {
                        entities.Add(new SelectList
                        {
                            Name = f.CorporateName,
                            Value = Convert.ToInt64(f.CorporateID)
                        });
                    }
            }
            return entities;
        }
    }
}
