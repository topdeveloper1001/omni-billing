using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IManagedCareService
    {
        int AddUptdateManagedCare(ManagedCare ManagedCare);
        ManagedCare GetManagedCareByID(int? ManagedCareId);
        List<ManagedCareCustomModel> GetManagedCareListByCorporate(int corporateId);
    }
}