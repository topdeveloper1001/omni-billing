using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IDrugInstructionAndDosingService
    {
        DrugInstructionAndDosing GetDrugInstructionAndDosingById(int? id);
        List<DrugInstructionAndDosingCustomModel> GetDrugInstructionAndDosingList();
        List<DrugInstructionAndDosingCustomModel> SaveDrugInstructionAndDosing(DrugInstructionAndDosing model);
    }
}