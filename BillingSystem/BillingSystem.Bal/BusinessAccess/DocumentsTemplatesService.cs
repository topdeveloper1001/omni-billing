using BillingSystem.Bal.Interfaces;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Repository.Common;
using BillingSystem.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BillingSystem.Bal.BusinessAccess
{
    public class DocumentsTemplatesService : IDocumentsTemplatesService
    {
        private readonly IRepository<DocumentsTemplates> _repository;
        private readonly IRepository<GlobalCodes> _gRepository;
        private readonly BillingEntities _context;
        private readonly IRepository<Encounter> _eRepository;

        public DocumentsTemplatesService(IRepository<DocumentsTemplates> repository, IRepository<GlobalCodes> gRepository, BillingEntities context, IRepository<Encounter> eRepository)
        {
            _repository = repository;
            _gRepository = gRepository;
            _context = context;
            _eRepository = eRepository;
        }

        /// <summary>
        /// Gets the patient documents.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public IEnumerable<DocumentsTemplates> GetPatientDocuments(int patientId)
        {
            var lstDocumentsTemplates =
                _repository.GetAll()
                    .Where(_ => _.AssociatedID == patientId && (_.IsDeleted == null || _.IsDeleted == false))
                    .OrderByDescending(_ => _.DocumentsTemplatesID)
                    .ToList();
            return lstDocumentsTemplates;
        }

        /// <summary>
        /// Gets the patient documents.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        public IEnumerable<DocumentsTemplates> GetNurseDocuments(int patientId, int encounterId)
        {
            var lst = _repository.GetAll().Where(x => (x.IsDeleted == null || x.IsDeleted == false) && x.PatientID == patientId && x.EncounterID == encounterId && x.ExternalValue3 == "4950").OrderByDescending(y => y.DocumentsTemplatesID).ToList();
            return lst;
        }

        /// <summary>
        /// Adds the update document tempate.
        /// </summary>
        /// <param name="doc">The document.</param>
        /// <returns></returns>
        public int AddUpdateDocumentTempate(DocumentsTemplates doc)
        {
            var result = Convert.ToInt32(doc.DocumentsTemplatesID > 0 ? _repository.Updatei(doc, doc.DocumentsTemplatesID) : _repository.Create(doc));
            return result;
        }

        /// <summary>
        /// Gets the document by type and patient identifier.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public DocumentsTemplates GetDocumentByTypeAndPatientId(int type, int patientId)
        {
            var m = _repository.Where(d => d.AssociatedType == type && (d.AssociatedID != null && (int)d.AssociatedID == patientId) && (d.IsDeleted == null || d.IsDeleted == false)).FirstOrDefault();
            return m;
        }

        /// <summary>
        /// Gets the document by identifier.
        /// </summary>
        /// <param name="documentid">The documentid.</param>
        /// <returns></returns>
        public DocumentsTemplates GetDocumentById(int documentid)
        {
            var d = _repository.Where(a => a.DocumentsTemplatesID == documentid).FirstOrDefault();
            return d;
        }
        /// <summary>
        /// Gets the type of the documents custom model by.
        /// </summary>
        /// <param name="associatedType">Type of the associated.</param>
        /// <param name="pid">The pid.</param>
        /// <returns></returns>
        public IEnumerable<DocumentsTemplatesCustomModel> GetDocumentsCustomModelByType(int associatedType, int pid)
        {
            try
            {
                var d = new List<DocumentsTemplatesCustomModel>();
                var lst =
                    _repository.GetAll()
                        .Where(
                            _ =>
                                _.AssociatedType == associatedType && _.AssociatedID == pid &&
                                (_.IsDeleted == null || _.IsDeleted == false))
                        .OrderByDescending(_ => _.DocumentsTemplatesID)
                        .ToList();

                d.AddRange(lst.Select(item => new DocumentsTemplatesCustomModel()
                {
                    DocumentsTemplatesID = item.DocumentsTemplatesID,
                    DocumentTypeID = item.DocumentTypeID,
                    DocumentName = item.DocumentName,
                    DocumentNotes = item.DocumentNotes,
                    AssociatedID = item.AssociatedID,
                    AssociatedType = item.AssociatedType,
                    FileName = item.FileName.Split('.')[0],
                    FilePath = item.FilePath,
                    IsTemplate = item.IsTemplate,
                    IsRequired = item.IsRequired,
                    CreatedBy = item.CreatedBy,
                    CreatedDate = item.CreatedDate,
                    IsDeleted = item.IsDeleted,
                    DeletedBy = item.DeletedBy,
                    DeletedDate = item.DeletedDate,
                    ModifiedBy = item.ModifiedBy,
                    ModifiedDate = item.ModifiedDate,
                    CorporateID = item.CorporateID,
                    FacilityID = item.FacilityID,
                    PatientID = item.PatientID,
                    EncounterID = item.EncounterID,
                    DocumentType = item.IsTemplate ? "pdf" : "image",
                    EncounterNumber =
                        item.EncounterID == null
                            ? string.Empty
                            : _eRepository.Where(x => x.EncounterID == Convert.ToInt32(item.EncounterID)).FirstOrDefault().EncounterNumber,
                    ExternalValue1 = item.ExternalValue1,
                    ExternalValue2 = item.ExternalValue2,
                    ExternalValue3 = item.ExternalValue3,
                    OldMedicalRecordSoruce =
                        (item.AssociatedType == 4 && item.ExternalValue1 != null
                            ? GetNameByGlobalCodeId(Convert.ToInt32(item.ExternalValue1))
                            : string.Empty),
                    ReferenceNumber = item.AssociatedType == 4 ? item.ExternalValue2 : string.Empty,
                    //GridType = associatedType
                }));
                return d;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the patient custom documents.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="associatedType">Type of the associated.</param>
        /// <returns></returns>
        public IEnumerable<DocumentsTemplates> GetPatientCustomDocuments(int patientId, int associatedType)
        {
            try
            {
                var vmlst = new List<DocumentsTemplatesCustomModel>();
                var lst = _repository.GetAll().Where(x => x.AssociatedID == patientId && x.AssociatedType == associatedType && (x.IsDeleted == null || x.IsDeleted == false)).OrderByDescending(y => y.DocumentsTemplatesID).ToList();

                vmlst.AddRange(lst.Select(item => new DocumentsTemplatesCustomModel()
                {
                    DocumentsTemplatesID = item.DocumentsTemplatesID,
                    DocumentTypeID = item.DocumentTypeID,
                    DocumentName = item.DocumentName,
                    DocumentNotes = item.DocumentNotes,
                    AssociatedID = item.AssociatedID,
                    AssociatedType = item.AssociatedType,
                    FileName = item.FileName.Split('.')[0],
                    FilePath = item.FilePath,
                    IsTemplate = item.IsTemplate,
                    IsRequired = item.IsRequired,
                    CreatedBy = item.CreatedBy,
                    CreatedDate = item.CreatedDate,
                    IsDeleted = item.IsDeleted,
                    DeletedBy = item.DeletedBy,
                    DeletedDate = item.DeletedDate,
                    ModifiedBy = item.ModifiedBy,
                    ModifiedDate = item.ModifiedDate,
                    CorporateID = item.CorporateID,
                    FacilityID = item.FacilityID,
                    PatientID = item.PatientID,
                    EncounterID = item.EncounterID,
                    DocumentType = item.IsTemplate ? "pdf" : "image",
                    EncounterNumber =
                        item.EncounterID == null
                            ? string.Empty
                            : _eRepository.Where(x => x.EncounterID == Convert.ToInt32(item.EncounterID)).FirstOrDefault().EncounterNumber,
                    OldMedicalRecordSoruce =
                        (item.AssociatedType == 4 && item.ExternalValue1 != null
                            ? GetNameByGlobalCodeId(Convert.ToInt32(item.ExternalValue1))
                            : string.Empty),
                    ExternalValue1 = item.ExternalValue1,
                    ExternalValue2 = item.ExternalValue2,
                    ExternalValue3 = item.ExternalValue3,
                    ReferenceNumber = item.AssociatedType == 4 ? item.ExternalValue2 : string.Empty
                }));
                return vmlst;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private string GetNameByGlobalCodeId(int id)
        {
            var gl = _gRepository.Where(g => g.GlobalCodeID == id).FirstOrDefault();
            return gl != null ? gl.GlobalCodeName : string.Empty;
        }


        public List<DocumentsTemplates> GetListByAssociateType(int associatedType)
        {
            var list = _repository.Where(f => f.IsDeleted == false && f.AssociatedType == associatedType).OrderBy(m => m.ExternalValue2).ThenByDescending(h => h.CreatedDate).ToList();
            return list;
        }

        public DocumentsTemplates GetDocumentByType(int patientId, int documentTypeId)
        {
            var current = _repository.Where(m => m.PatientID == patientId && m.DocumentTypeID == documentTypeId).FirstOrDefault();
            return current;
        }
        public List<DocumentsTemplates> SavePatientDocuments(DocumentsTemplates model, out int? result)
        {
            result = -1;
            if (model.DocumentsTemplatesID > 0)
                result = _repository.Updatei(model, model.DocumentsTemplatesID);
            else
                result = _repository.Create(model);

            var list = _repository.Where(m => m.PatientID == model.PatientID && !m.DocumentName.ToLower().Trim().Equals("profilepicture") && m.AssociatedType == model.AssociatedType).ToList();
            return list;
        }

        public int DeleteDocument(int id)
        {
            _repository.Delete(id);
            return id;
        }

        public async Task<List<DocumentsTemplates>> GetPatientDocumentsList(int patientId)
        {
            var sqlParams = new SqlParameter[5];
            sqlParams[0] = new SqlParameter("@pFId", 0);
            sqlParams[1] = new SqlParameter("@pCId", 0);
            sqlParams[2] = new SqlParameter("@pUserId", 0);
            sqlParams[3] = new SqlParameter("@pPId", patientId);
            sqlParams[4] = new SqlParameter("@pExclusions", "profilepicture");

            using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetDocumentsByPatient.ToString(), false, parameters: sqlParams))
            {
                var docs = (await r.ResultSetForAsync<DocumentsTemplates>()).ToList();
                return docs;
            }

        }


        public async Task<IEnumerable<DocumentsTemplatesCustomModel>> SaveDocumentsAsync(DataTable dt, bool showDocsList, string exclusions)
        {
            var sqlParams = new SqlParameter[3];
            sqlParams[0] = new SqlParameter
            {
                ParameterName = "@pDocs",
                SqlDbType = SqlDbType.Structured,
                Value = dt,
                TypeName = "TypeDocumentTemplate"
            };
            sqlParams[1] = new SqlParameter("@pWithDocs", showDocsList);
            sqlParams[2] = new SqlParameter("@pExclusions", exclusions);

            using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SprocUploadFiles.ToString(), false, parameters: sqlParams))
            {
                IEnumerable<DocumentsTemplatesCustomModel> docs = null;

                if (showDocsList)
                    docs = (await r.ResultSetForAsync<DocumentsTemplatesCustomModel>()).ToList();
                return docs;
            }
        }
    }
}
