using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IDrugInteractionsService
    {
        DrugInteractions GetDrugInteractionsById(int? id);
        List<DrugInteractionsCustomModel> GetDrugInteractionsList();
        List<DrugInteractionsCustomModel> SaveDrugInteractions(DrugInteractions model);
    }
}