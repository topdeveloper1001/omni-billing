using BillingSystem.Bal.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BillingSystem.Common;
using System.Data.Entity;
using BillingSystem.Model.EntityDto;
using System;

using BillingSystem.Model;
using System.Data.SqlClient;

using BillingSystem.Common.Common;

namespace BillingSystem.Bal.Service
{
    public class AddressService : IAddressService
    {
        private readonly IRepository<Country> _cRepository;
        private readonly IRepository<State> _sRepository;
        private readonly IRepository<Facility> _fRepository;
        private readonly IRepository<Users> _uRepository;
        private readonly IRepository<Corporate> _coRepository;
        private readonly BillingEntities _context;

        public AddressService(IRepository<Country> cRepository, IRepository<State> sRepository, IRepository<Facility> fRepository, IRepository<Users> uRepository, IRepository<Corporate> coRepository, BillingEntities context)
        {
            _cRepository = cRepository;
            _sRepository = sRepository;
            _fRepository = fRepository;
            _uRepository = uRepository;
            _coRepository = coRepository;
            _context = context;
        }

        public async Task<List<SelectList>> GetCountriesAsync()
        {
            var list = await _cRepository.Where(a => a.IsDeleted != true)
                .Select(c => new SelectList
                {
                    Name = c.CountryName.Trim(),
                    Value = c.CountryID
                }).OrderBy(a => a.Name).ToListAsync();

            return list;

        }

        public async Task<IEnumerable<SelectList>> GetCitiesAsync(long stateId)
        {
            var sqlParams = new SqlParameter[1];
            sqlParams[0] = new SqlParameter("pStateId", stateId);

            using (var ms = _context.MultiResultSetSqlQuery(StoredProcsiOS.iSprocGetCitiesByState.ToString(), isCompiled: false, parameters: sqlParams))
            {
                var result = await ms.GetResultWithJsonAsync<SelectList>(JsonResultsArray.Cities.ToString());
                return result;
            }
        }

        public async Task<List<SelectList>> GetStatesAsync(long countryId)
        {
            var list = await _sRepository.Where(a => a.IsActive && !a.IsDeleted && a.CountryID == countryId)
                .Select(c => new SelectList
                {
                    Name = c.StateName.Trim(),
                    Value = c.StateID
                }).OrderBy(a => a.Name).ToListAsync();

            return list;
        }

        public async Task<DefaultDataDto> GetDefaultDataAsync(long userId)
        {
            var dto = new DefaultDataDto();
            var sqlParams = new SqlParameter[1] { new SqlParameter("pUserId", userId) };
            using (var ms = _context.MultiResultSetSqlQuery(StoredProcsiOS.iSprocGetDefaultData.ToString(), false, sqlParams))
            {
                dto.Country = (await ms.ResultSetForAsync<SelectList>()).FirstOrDefault();
                dto.State = (await ms.ResultSetForAsync<SelectList>()).FirstOrDefault();
                dto.City = (await ms.ResultSetForAsync<SelectList>()).FirstOrDefault();
            }
            return dto;
        }

        public async Task<List<SelectList>> GetEntitiesByCityAsync(long cityId = 0, long corporateId = 0)
        {
            var entities = new List<SelectList>();
            var c = Convert.ToString(cityId);

            var list = await _fRepository.Where(f => (cityId == 0 || f.FacilityCity.Equals(c)) && (corporateId == 0 || f.CorporateID == corporateId)).ToListAsync();
            if (list.Any())
                foreach (var f in list)
                {
                    entities.Add(new SelectList
                    {
                        Name = f.FacilityName,
                        Value = Convert.ToInt64(f.FacilityId)
                    });
                }

            return entities;
        }

        public async Task<List<SelectList>> GetCorporatesAsync(long corporateId = 0, long userId = 0)
        {
            var entities = new List<SelectList>();

            if (userId > 0)
            {
                corporateId = await _uRepository.Where(a => a.UserID == userId).Select(c => c.CorporateId.Value).FirstOrDefaultAsync();
            }

            var list = await _coRepository.Where(f => (corporateId == 0 || f.CorporateID == corporateId)).ToListAsync();
            if (list.Any())
                foreach (var f in list)
                {
                    entities.Add(new SelectList
                    {
                        Name = f.CorporateName,
                        Value = Convert.ToInt64(f.CorporateID)
                    });
                }
            return entities;
        }
    }
}
