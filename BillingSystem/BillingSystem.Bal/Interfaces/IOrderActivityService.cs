using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IOrderActivityService
    {
        int[] AddUptdateOrderActivity(List<OrderActivity> orderActivity);
        int AddUptdateOrderActivity(OrderActivity orderActivity);
        bool ApplyOrderActivityToBill(int corporateId, int facilityId, int encounterId, string reclaimFlag, long claimId);
        string CalculateLabResultType(string ordercode, decimal? resultminvalue, int? patientId);
        int? CalculateLabResultUOMType(string ordercode, decimal? resultminvalue, int? patientId);
        int CloseOrderActivity(int orderActivityId);
        bool CreatePartiallyexecutedActivity(int activityid, decimal quantity, string actvityStatus);
        GenerateBarCode GetBarCodeDetails(int orderActivityId);
        List<OrderActivityCustomModel> GetLabOrderActivitiesByPhysician(int userId, int status, string orderCategoryId, int isActiveEncountersOnly, int encounterId);
        OrderActivityCustomModel GetLabOrderActivityByActivityId(int activityId, string CptTableNumber, string DrgTableNumber, string HcpcsTableNumber, string DrugTableNumber, string ServiceCodeTableNumber, string DiagnosisTableNumber);
        List<LabOrderActivityResultStatus> GetLabResultStatusString(int ordercode, decimal resultminvalue, int patientId);
        List<MarViewCustomModel> GetMARView(MarViewCustomModel oMarViewCustomModel);
        List<OrderActivityCustomModel> GetOrderActivitiesByEncounterId(int encounterId, string CptTableNumber, string DrgTableNumber, string HcpcsTableNumber, string DrugTableNumber, string ServiceCodeTableNumber, string DiagnosisTableNumber);
        List<OrderActivityCustomModel> GetOrderActivitiesByEncounterIdSP(int encounterId);
        List<OrderActivity> GetOrderActivitiesByOrderId(int ordersId);
        List<OrderActivityCustomModel> GetOrderActivitiesByPatientId(int? patientId, string CptTableNumber, string DrgTableNumber, string HcpcsTableNumber, string DrugTableNumber, string ServiceCodeTableNumber, string DiagnosisTableNumber);
        List<OrderActivity> GetOrderActivity();
        OrderActivity GetOrderActivityByID(int? OrderActivityId);
        OrderActivityCustomModel GetOrderActivityByIDVM(int orderActivityId);
        List<OrderActivityCustomModel> GetOrderActivityCustom(string CptTableNumber, string DrgTableNumber, string HcpcsTableNumber, string DrugTableNumber, string ServiceCodeTableNumber, string DiagnosisTableNumber);
        List<OrderActivityCustomModel> GetPCActivitiesByEncounterId(int encounterId);
        List<OrderActivityCustomModel> GetPCClosedActivitiesByEncounterId(int encounterId);
        bool PharamacyOrderActivityAdministered(int orderId);
    }
}