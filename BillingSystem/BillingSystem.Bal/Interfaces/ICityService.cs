using System.Collections.Generic;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface ICityService
    {
        List<CityCustomModel> GetCityListByStateId(int stateId);
    }
}