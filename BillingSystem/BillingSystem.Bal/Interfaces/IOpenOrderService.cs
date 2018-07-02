using System;
using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IOpenOrderService
    {
        string CalculateLabResultSpecimanType(string ordercode, decimal? resultminvalue, int? patientId);

        int AddUpdateOpenOrderCustomModel(OpenOrderCustomModel vm);
        int[] AddUpdatePhysicianMultipleOpenOrder(List<OpenOrder> model);
        int AddUpdatePhysicianOpenOrder(OpenOrder model);
        bool ApprovePharmacyOrder(int id, string type, string comment);
        OrderCustomModel ApprovePharmacyOrder(int id, string type, string comment, long encounterId, bool withActivities, long categoryId, long physicianId, int corporateId, int facilityId);
        bool CancelOpenOrder(int orderid, int userid);
        DrugReactionCustomModel CheckDurgAllergy(string ordercode, int patientId, int erncounterid);
        bool CheckMulitpleOpenActivites(int openorderid);
        List<OpenOrderCustomModel> CustomOrdersListList(int userId, int numberOfDays, int facilityid, int cid);
        int GetActiveEncounterId(int patientId);
        List<OpenOrderCustomModel> GetAllOrdersByEncounterId(int encounterId);
        IEnumerable<EncounterCustomModel> GetEncountersListByPatientId(int patientId);
        OpenOrder GetFavOpenOrderDetail(long favorderId, long facilityId, long userId);
        List<OpenOrderCustomModel> GetFavoriteOrders(int userId, string CptTableNumber, string DrgTableNumber, string HcpcsTableNumber, string DrugTableNumber, string ServiceCodeTableNumber, string DiagnosisTableNumber);
        List<OpenOrderCustomModel> GetMostOrderedList(int userId, int numberOfDays);
        List<OpenOrderCustomModel> GetMostRecentOrders(int userId);
        OpenOrder GetOpenOrderDetail(int id);
        List<OrderActivityCustomModel> GetOrderActivitiesByOpenOrder(long openOrderId);
        int GetOrderIdByOrderCode(int phyId, string codeid);
        OrderCustomModel GetOrdersAndActivitiesByEncounter(long encounterId);
        List<OpenOrderCustomModel> GetOrdersByEncounterid(int encounterId, string CptTableNumber, string DrgTableNumber, string HcpcsTableNumber, string DrugTableNumber, string ServiceCodeTableNumber, string DiagnosisTableNumber);
        List<OpenOrderCustomModel> GetOrdersByPatientId(int patientId, string CptTableNumber, string DrgTableNumber, string HcpcsTableNumber, string DrugTableNumber, string ServiceCodeTableNumber, string DiagnosisTableNumber);
        List<OpenOrderCustomModel> GetOrdersByPhysician(int userId, int corporateId, int facilityId);
        List<OpenOrderCustomModel> GetOrdersByPhysicianId(int phyId, string CptTableNumber, string DrgTableNumber, string HcpcsTableNumber, string DrugTableNumber, string ServiceCodeTableNumber, string DiagnosisTableNumber);
        PatientSummaryTabData GetPatientSummaryDataOnLoad(long encId, long patientId);
        List<OpenOrderCustomModel> GetPhysicanFavoriteOrderedList(int userId, int facilityid, int corporateid);
        OrderCustomModel GetPhysicianOrderPlusActivityList(int encounterId, string orderStatus = "", long categoryId = 0, bool withActivities = false);
        List<OpenOrderCustomModel> GetPhysicianOrders(int encounterId, string orderStatus, string CptTableNumber, string DrgTableNumber, string HcpcsTableNumber, string DrugTableNumber, string ServiceCodeTableNumber, string DiagnosisTableNumber, string fId = "");
        List<OpenOrderCustomModel> GetPhysicianOrdersList(int encounterId, string orderStatus, long categoryId = 0);
        PhysicianTabData GetPhysicianOrNurseTabData(long encId, long patientId, long physicianId, int notesUserType);
        List<OpenOrderCustomModel> GetSearchedOrders(string text, string CptTableNumber, string DrgTableNumber, string HcpcsTableNumber, string DrugTableNumber, string ServiceCodeTableNumber, string DiagnosisTableNumber);
        List<OrderActivityCustomModel> OrderActivitiesByEncounterId(int encounterId);
        OrderCustomModel OrdersViewData(int physicianId, int mostRecentDays, int cId, int fId, int encId, string gcCategoryCodes, int patientId, string opCode, string exParam1);
        OrderCustomModel SavePhysicianOrder(OpenOrderCustomModel vm2, int physicianId, int mostRecentDays, int cId, int fId, int encId, string gcCategoryCodes, int patientId, string opCode, string exParam1 = "");
        int UpdateOpenOrderStatus(int orderId, string status, int userId, DateTime modifiedDate);
    }
}