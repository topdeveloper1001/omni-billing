using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IXPaymentFileXMLService
    {
        string GetFirstXPaymentFileXML(int claimid);
        List<XPaymentFileXMLCustomModel> GetXPaymentFileXML(int claimid);
        XPaymentFileXML GetXPaymentFileXMLByID(int? XPaymentFileXMLId);
        long SaveXPaymentFileXML(XPaymentFileXML model);
    }
}