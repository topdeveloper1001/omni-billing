using System.Collections.Generic;
using System.Threading.Tasks;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IAuthorizationService
    {
        int AddUptdateAuthorization(Authorization authorization);
        List<BillHeaderXMLModel> GenerateEAuthorizationFile(int facilityId, string dispositionFlag);
        List<AuthorizationCustomModel> GetAuthorization();
        Authorization GetAuthorizationByEncounterId(string authorizationEncounterId);
        Authorization GetAuthorizationById(int? authorizationId);
        List<AuthorizationCustomModel> GetAuthorizationsByEncounterId(string authorizationEncounterId);
        Task<AuthorizationViewData> SaveAuthorizationAsync(AuthorizationCustomModel vm, string docsJson, bool withAuthList, bool withDocs);
        AuthorizationViewData SaveAuthorizationAsync1(AuthorizationCustomModel vm, string docsJson, bool withAuthList, bool withDocs);
    }
}