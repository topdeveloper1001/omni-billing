using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface ILabTestOrderSetService
    {
        LabTestOrderSet GetDetailById(int? id);
        List<LabTestOrderSetCustomModel> GetLabOrderSetList();
        List<LabTestOrderSetCustomModel> SaveLabTestOrderSet(LabTestOrderSet model);
    }
}