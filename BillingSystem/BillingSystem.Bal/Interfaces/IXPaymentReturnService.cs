using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IXPaymentReturnService
    {
        bool GenerateRemittanceInfo(int claimid, int corporateId, int facilityId);
        bool GenerateRemittanceXmlFile(int corporateId, int facilityId);
        bool GetClaimPayment(int claimid);
        XPaymentReturn GetXPaymentModelReturnById(int? xPaymentReturnId);
        List<XPaymentReturnCustomModel> GetXPaymentReturn();
        List<XPaymentReturnCustomModel> GetXPaymentReturnByClaimId(int claimId);
        XPaymentReturnCustomModel GetXPaymentReturnById(int? xPaymentReturnId);
        List<XPaymentReturn> GetXPaymentReturnModelByClaimId(int claimId);
        int SaveXPaymentReturn(XPaymentReturn model);
    }
}