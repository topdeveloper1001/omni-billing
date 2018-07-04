using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface ICatalogService
    {
        long DeleteCatalogData(Catalog model);
        Catalog GetCatalogById(int id);
        long SaveCatalog(Catalog model);
        List<CatalogCustomModel> GetCatalogData(int corporateId, int facilityId);
        List<Catalog> GetCatalogByFacilityId(int facilityId);
    }
}