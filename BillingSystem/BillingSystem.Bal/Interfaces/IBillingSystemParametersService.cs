using BillingSystem.Model;
using System.Collections.Generic;

namespace BillingSystem.Bal.Interfaces
{
    public interface IBillingSystemParametersService
    {
        BillingSystemParameters GetDetailsByBillingParameterId(int billingParameterId);
        BillingSystemParameters GetDetailsByCorporateAndFacility(int corporateId, string facilityNumber);
        int SaveBillingSystemParameters(BillingSystemParameters model);
        bool SaveRecordsFortableNumber(string tableNumber, IEnumerable<string> selectedCodeid, string typeid);
        int SaveTableNumber(BillingCodeTableSet model);
    }
}