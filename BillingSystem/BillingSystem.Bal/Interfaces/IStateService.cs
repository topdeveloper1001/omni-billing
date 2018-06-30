using System.Collections.Generic;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IStateService
    {
        List<StateCustomModel> GetStatesByCountryId(int countryId);
    }
}