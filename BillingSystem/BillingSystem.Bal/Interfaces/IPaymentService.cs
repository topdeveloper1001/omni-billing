using System;
using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IPaymentService
    {
        bool ApplyManualPayment(int corporateId, int facilityId);
        IEnumerable<ReviewExpectedPaymentReport> GetExpectedPaymentInsNotPaid(int corporateid, int facilityid);
        IEnumerable<ReviewExpectedPaymentReport> GetExpectedPaymentInsVariance(int corporateid, int facilityid);
        IEnumerable<ReviewExpectedPaymentReport> GetExpectedPaymentPatientVar(int corporateid, int facilityid);
        string GetNameByGlobalCodeValue(string codeValue, string categoryValue, string fId = "");
        IEnumerable<AccountStatementCustomModel> GetNoPaymentReceviedList(int coporateid, int facilityid);
        IEnumerable<AccountStatementCustomModel> GetPatientAccountStatement(int patientId);
        List<PaymentCustomModel> GetPaymentBills(int patientId, int encounterId, int billHeaderId, int corporateId, int facilityId);
        List<PaymentCustomModel> GetPaymentByBillId(int billHeaderId);
        PaymentCustomModel GetPaymentById(long paymentId);
        PaymentViewDetail GetPaymentsListAndOthersData(long cId, long fId, long userId, long billHeaderId, long patientId = 0, long encounterId = 0, string billNumber = "", long paymentId = 0);
        IEnumerable<AccountStatementCustomModel> GetUnMactedPaymentList(int coporateid, int facilityid);
        PaymentViewDetail SaveAndApplyPayments(long cId, long fId, long userId, PaymentCustomModel vm, DateTime currentDateTime);
        long SaveCustomPayments(PaymentCustomModel vm);
        long SavePayments(Payment model);
    }
}