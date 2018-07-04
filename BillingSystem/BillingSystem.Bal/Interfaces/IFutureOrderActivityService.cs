using System.Collections.Generic;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model.Model;

namespace BillingSystem.Bal.Interfaces
{
    public interface IFutureOrderActivityService
    {
        List<FutureOrderActivityCustomModel> GetFutureOrderActivity();
        FutureOrderActivity GetFutureOrderActivityById(int? FutureOrderActivityId);
        int SaveFutureOrderActivity(FutureOrderActivity m);
    }
}