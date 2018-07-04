using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IDrugAllergyLogService
    {
        DrugAllergyLog GetDrugAllergyLogById(int? id);
        List<DrugAllergyLogCustomModel> GetDrugAllergyLogList(int corporateId, int facilityid);
        List<DrugAllergyLogCustomModel> SaveDrugAllergyLog(DrugAllergyLog model);
        bool SaveDrugAllergyLogCustom(DrugAllergyLog model);
    }
}