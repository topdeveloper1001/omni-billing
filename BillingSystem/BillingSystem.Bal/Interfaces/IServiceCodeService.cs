using System;
using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IServiceCodeService
    {
        int AddUpdateServiceCode(ServiceCode model, string ServiceCodeTableNumber);
        List<ServiceCode> ExportServiceCodes(string text, string tableNumber);
        List<ServiceCode> GetFilteredCodes(string text, string ServiceCodeTableNumber);
        List<ServiceCodeCustomModel> GetFilteredCodesCModel(string text, string ServiceCodeTableNumber);
        List<ServiceCodeCustomModel> GetFilteredServiceCodes(string text, string tableNumber);
        List<ServiceCode> GetOveridableBedList(string ServiceCodeTableNumber);
        ServiceCode GetServiceCodeByCodeValue(string serviceCodeValue, string ServiceCodeTableNumber);
        ServiceCode GetServiceCodeById(int serviceCodeId);
        string GetServiceCodeDescription(string codeValue, string ServiceCodeTableNumber);
        int GetServiceCodePriceByCodeValue(string codeValue, DateTime? CodeEffectiveFrom);
        List<ServiceCode> GetServiceCodes(string ServiceCodeTableNumber);
        List<ServiceCodeCustomModel> GetServiceCodesActiveInActive(bool showInActive, string ServiceCodeTableNumber);
        List<ServiceCode> GetServiceCodesByCodeMainValue(string codeMainValue, int rowCount, string ServiceCodeTableNumber);
        List<ServiceCodeCustomModel> GetServiceCodesCustomList(string ServiceCodeTableNumber);
        List<ServiceCodeCustomModel> GetServiceCodesCustomModel(string ServiceCodeTableNumber);
        List<ServiceCode> GetServiceCodesList(string ServiceCodeTableNumber);
        List<ServiceCode> GetServiceCodesListOnDemand(int blockNumber, int blockSize, string ServiceCodeTableNumber);
        List<ServiceCodeCustomModel> GetServiceCodesListOnDemandCustom(int blockNumber, int blockSize, string ServiceCodeTableNumber);
    }
}