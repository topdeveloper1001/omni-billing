using System;
using System.Collections.Generic;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IBillingModifierService
    {
        long DeleteRecord(long id, int userId, DateTime dateTime);
        BillingModifierCustomModel GetById(long id);
        IEnumerable<BillingModifierCustomModel> GetListByEntity(long facilityId, long corporateId);
        long SaveRecord(BillingModifierCustomModel vm);
    }
}