using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IPreSchedulingLinkService
    {
        PreSchedulingLink CheckforPreviousData(int? cId, int? fId);
        bool DeletePreSchedulingLink(PreSchedulingLink model);
        List<PreSchedulingLinkCustomModel> GetPreSchedulingLink(int cid, int fid);
        PreSchedulingLink GetPreSchedulingLinkById(int? PreSchedulingLinkId);
        int SavePreSchedulingLink(PreSchedulingLink model);
    }
}