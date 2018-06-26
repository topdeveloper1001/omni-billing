using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface ICountryService
    {
        List<Country> GetCountries();
        CountryCustomModel GetCountryInfoByCountryID(int countryId);
        List<CountryCustomModel> GetCountryWithCode();
        List<BillingCodeTableSet> GetTableNumbersList(string typeId);
    }
}