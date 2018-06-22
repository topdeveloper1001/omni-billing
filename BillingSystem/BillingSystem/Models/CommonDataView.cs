using System.Collections.Generic;
using BillingSystem.Model;


namespace BillingSystem.Models
{
    public class CommonDataView
    {
        public List<Country> CountryList { get; set; }
        public List<State> StatesList { get; set; }
        public List<City> CityList { get; set; }

        public int CountryId { get; set; }
        public int StateId { get; set; }
        public int CityId { get; set; }
    }
}