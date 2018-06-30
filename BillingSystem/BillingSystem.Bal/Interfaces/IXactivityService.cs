using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IXactivityService
    {
        List<XActivity> GetXActivities();
        List<XActivityCustomModel> GetXactivity();
        List<XActivityCustomModel> GetXactivityByClaimId(int? claimId);
        List<XActivityCustomModel> GetXactivityByEncounterId(int? encounterId);
        int SaveXactivity(XActivity model);
    }
}