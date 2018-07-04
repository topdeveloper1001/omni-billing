using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IOpenOrderActivityScheduleService
    {
        int AddUpdateOpenOrderActivitySchedule(OpenOrderActivitySchedule model);
        OpenOrderActivitySchedule GetOpenOrderActivityScheduleById(int id);
        List<OrderActivityCustomModel> GetOrderActivitiesByEncounterId(int encounterId);
    }
}