using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IDocumentsTemplatesService
    {
        int AddUpdateDocumentTempate(DocumentsTemplates doc);
        int DeleteDocument(int id);
        DocumentsTemplates GetDocumentById(int documentid);
        DocumentsTemplates GetDocumentByType(int patientId, int documentTypeId);
        DocumentsTemplates GetDocumentByTypeAndPatientId(int type, int patientId);
        IEnumerable<DocumentsTemplatesCustomModel> GetDocumentsCustomModelByType(int associatedType, int pid);
        List<DocumentsTemplates> GetListByAssociateType(int associatedType);
        IEnumerable<DocumentsTemplates> GetNurseDocuments(int patientId, int encounterId);
        IEnumerable<DocumentsTemplates> GetPatientCustomDocuments(int patientId, int associatedType);
        IEnumerable<DocumentsTemplates> GetPatientDocuments(int patientId);
        Task<List<DocumentsTemplates>> GetPatientDocumentsList(int patientId);
        Task<IEnumerable<DocumentsTemplatesCustomModel>> SaveDocumentsAsync(DataTable dt, bool showDocsList, string exclusions);
        List<DocumentsTemplates> SavePatientDocuments(DocumentsTemplates model, out int? result);
    }
}