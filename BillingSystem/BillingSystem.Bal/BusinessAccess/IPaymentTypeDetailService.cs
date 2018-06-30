using System.Collections.Generic;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model.Model;

namespace BillingSystem.Bal.Interfaces
{
    public interface IPaymentTypeDetailService
    {
        List<PaymentTypeDetailCustomModel> GetPaymentTypeDetail();
        PaymentTypeDetail GetPaymentTypeDetailById(int? paymentTypeDetailId);
        PaymentTypeDetail GetPaymentTypeDetailByPaymentId(int? paymentId);
        int SavePaymentTypeDetail(PaymentTypeDetail model);
    }
}