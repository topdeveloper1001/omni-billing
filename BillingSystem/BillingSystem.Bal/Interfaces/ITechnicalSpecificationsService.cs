using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface ITechnicalSpecificationsService
    {
        bool CheckDuplicateTechnicalSpecification(int id, long itemID, int? corporateId, int? facilityId);
        int DeleteTechnicalSpecificationsData(TechnicalSpecifications model);
        TechnicalSpecifications GetTechnicalSpecificationById(int id);
        int SaveTechnicalSpecifications(TechnicalSpecifications model);
        List<TechnicalSpecificationsCustomModel> GetTechnicalSpecificationsData(int corporateId, int facilityId);
        List<TechnicalSpecifications> GetTechnicalSpecificationsByFacilityId(int facilityId);
    }
}