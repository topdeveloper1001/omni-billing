using System.Linq;
using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Repository.Interfaces;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class CountryService : ICountryService
    {
        private readonly IRepository<Country> _repository;
        private readonly IRepository<BillingCodeTableSet> _bcRepository;

        public CountryService(IRepository<Country> repository, IRepository<BillingCodeTableSet> bcRepository)
        {
            _repository = repository;
            _bcRepository = bcRepository;
        }


        /// <summary>
        /// Get the Countries
        /// </summary>
        /// <returns>Return the list of Countries </returns>
        public List<Country> GetCountries()
        {
            var list = _repository.GetAll().OrderBy(c => c.CountryName).ToList();
            return list;
        }

        public CountryCustomModel GetCountryInfoByCountryID(int countryId)
        {
            var objCountry = _repository.Where(s => s.CountryID == countryId && s.IsDeleted == false).FirstOrDefault();
            CountryCustomModel objCountryCustomModel = new CountryCustomModel();
            objCountryCustomModel.CodeValue = objCountry.CodeValue;// (from y in objCountry select new CountryCustomModel { CodeValue = y.co });
            return objCountryCustomModel;

        }
        public List<BillingCodeTableSet> GetTableNumbersList(string typeId)
        {
            return _bcRepository.Where(d => d.CodeTableType.Trim().Equals(typeId) || string.IsNullOrEmpty(typeId))
                    .OrderBy(f => f.CodeTableType)
                    .ToList();

        }

        public List<CountryCustomModel> GetCountryWithCode()
        {
            var list = new List<CountryCustomModel>();
            var countries = _repository.GetAll().ToList();
            if (countries.Any())
            {
                list.AddRange(countries.Select(item => new CountryCustomModel
                {
                    CountryID = item.CountryID,
                    CountryName = item.CountryName,
                    CodeValue = item.CodeValue,
                    CountryWithCode = string.Format("{0}-{1}", item.CountryName, item.CodeValue)
                }));

                list = list.OrderBy(c => c.CountryName).ToList();
            }
            return list;
        }

    }
}
