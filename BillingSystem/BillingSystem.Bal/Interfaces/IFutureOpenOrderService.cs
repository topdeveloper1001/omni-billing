using System.Collections.Generic;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model.Model;

namespace BillingSystem.Bal.Interfaces
{
    public interface IFutureOpenOrderService
    {
        bool AddCurrentEncToFutureOrders(string[] ordersId, int encId, int cid, int fid);
        List<FutureOpenOrderCustomModel> GetFutureOpenOrder(string CptTableNumber, string ServiceCodeTableNumber, string DrgTableNumber, string DrugTableNumber, string HcpcsTableNumber, string DiagnosisTableNumber);
        List<FutureOpenOrderCustomModel> GetFutureOpenOrderByEncId(int? encounterId, string CptTableNumber, string ServiceCodeTableNumber, string DrgTableNumber, string DrugTableNumber, string HcpcsTableNumber, string DiagnosisTableNumber);
        FutureOpenOrder GetFutureOpenOrderById(int? FutureOpenOrderId);
        List<FutureOpenOrderCustomModel> GetFutureOpenOrderByPatientId(int? pid);
        int SaveFutureOpenOrder(FutureOpenOrder model);
    }
}