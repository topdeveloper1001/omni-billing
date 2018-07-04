using System;
using System.Collections.Generic;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IPlaceOfServiceService
    {
        long DeleteRecord(long id, int userId, DateTime dateTime);
        PlaceOfServiceCustomModel GetById(long id);
        IEnumerable<PlaceOfServiceCustomModel> GetListByEntity(long facilityId, long corporateId);
        long SaveRecord(PlaceOfServiceCustomModel vm);
    }
}