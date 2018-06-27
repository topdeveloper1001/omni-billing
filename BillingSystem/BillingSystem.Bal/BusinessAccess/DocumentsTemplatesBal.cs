using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace BillingSystem.Bal.BusinessAccess
{
    public class DocumentsTemplatesBal : BaseBal
    {
        private readonly IRepository<Encounter> _eRepository;
        /// <summary>
        /// Gets the patient documents.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public IEnumerable<DocumentsTemplates> GetPatientDocuments(int patientId)
        {
            try
            {
                using (var documentsTemplatesRep = UnitOfWork.DocumentsTemplatesRepository)
                {
                    var lstDocumentsTemplates =
                        documentsTemplatesRep.GetAll()
                            .Where(_ => _.AssociatedID == patientId && (_.IsDeleted == null || _.IsDeleted == false))
                            .OrderByDescending(_ => _.DocumentsTemplatesID)
                            .ToList();
                    return lstDocumentsTemplates;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the patient documents.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        public IEnumerable<DocumentsTemplates> GetNurseDocuments(int patientId, int encounterId)
        {
            try
            {
                using (var documentsTemplatesRep = UnitOfWork.DocumentsTemplatesRepository)
                {
                    var lstDocumentsTemplates =
                        documentsTemplatesRep.GetAll()
                            .Where(_ => (_.IsDeleted == null || _.IsDeleted == false)
                                        && _.PatientID == patientId
                                        && _.EncounterID == encounterId
                                        && _.ExternalValue3 == "4950"
                            )
                            .OrderByDescending(_ => _.DocumentsTemplatesID)
                            .ToList();
                    return lstDocumentsTemplates;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Adds the update document tempate.
        /// </summary>
        /// <param name="doc">The document.</param>
        /// <returns></returns>
        public int AddUpdateDocumentTempate(DocumentsTemplates doc)
        {
            using (var rep = UnitOfWork.DocumentsTemplatesRepository)
            {
                var result = Convert.ToInt32(doc.DocumentsTemplatesID > 0 ? rep.UpdateEntity(doc, doc.DocumentsTemplatesID) : rep.Create(doc));
                return result;
            }
        }

        /// <summary>
        /// Gets the document by type and patient identifier.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public DocumentsTemplates GetDocumentByTypeAndPatientId(int type, int patientId)
        {
            using (var rep = UnitOfWork.DocumentsTemplatesRepository)
            {
                var doc =
                    rep.Where(
                        d =>
                            d.AssociatedType == type && (d.AssociatedID != null && (int)d.AssociatedID == patientId) &&
                            (d.IsDeleted == null || d.IsDeleted == false)).FirstOrDefault();
                return doc;
            }
        }

        /// <summary>
        /// Gets the document by identifier.
        /// </summary>
        /// <param name="documentid">The documentid.</param>
        /// <returns></returns>
        public DocumentsTemplates GetDocumentById(int documentid)
        {
            using (var rep = UnitOfWork.DocumentsTemplatesRepository)
            {
                var document = rep.Where(a => a.DocumentsTemplatesID == documentid).FirstOrDefault();

                //  string extension = System.IO.Path.GetExtension(document.FileName);
                //document.FileName= document.FileName.Substring(0, document.FileName.Length - extension.Length);


                return document;
            }
        }

        //public DocumentsTemplates GetDocumentByDocumentTypeId(int documentTypeid)
        //{
        //    using (var rep = UnitOfWork.DocumentsTemplatesRepository)
        //    {
        //        var document = rep.Where(a => a.DocumentTypeID == documentTypeid).FirstOrDefault();
        //        return document;
        //    }
        //}


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
                var documents = new List<DocumentsTemplatesCustomModel>();
                using (var documentsTemplatesRep = UnitOfWork.DocumentsTemplatesRepository)
                {
                    var lstDocumentsTemplates =
                        documentsTemplatesRep.GetAll()
                            .Where(
                                _ =>
                                    _.AssociatedType == associatedType && _.AssociatedID == pid &&
                                    (_.IsDeleted == null || _.IsDeleted == false))
                            .OrderByDescending(_ => _.DocumentsTemplatesID)
                            .ToList();

                    documents.AddRange(lstDocumentsTemplates.Select(item => new DocumentsTemplatesCustomModel()
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
                    return documents;
                }
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
                var documents = new List<DocumentsTemplatesCustomModel>();
                using (var documentsTemplatesRep = UnitOfWork.DocumentsTemplatesRepository)
                {
                    var lstDocumentsTemplates =
                        documentsTemplatesRep.GetAll()
                            .Where(
                                _ =>
                                   _.AssociatedID == patientId && _.AssociatedType == associatedType &&
                                    (_.IsDeleted == null || _.IsDeleted == false))
                            .OrderByDescending(_ => _.DocumentsTemplatesID)
                            .ToList();

                    documents.AddRange(lstDocumentsTemplates.Select(item => new DocumentsTemplatesCustomModel()
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
                    return documents;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<DocumentsTemplates> GetListByAssociateType(int associatedType)
        {
            using (var rep = UnitOfWork.DocumentsTemplatesRepository)
            {
                var list =
                    rep.Where(f => f.IsDeleted == false && f.AssociatedType == associatedType)
                        .OrderBy(m => m.ExternalValue2)
                        .ThenByDescending(h => h.CreatedDate)
                        .ToList();
                return list;
            }
        }

        public DocumentsTemplates GetDocumentByType(int patientId, int documentTypeId)
        {
            using (var rep = UnitOfWork.DocumentsTemplatesRepository)
            {
                var current = rep.Where(m => m.PatientID == patientId && m.DocumentTypeID == documentTypeId).FirstOrDefault();
                return current;
            }
        }
        public List<DocumentsTemplates> SavePatientDocuments(DocumentsTemplates model, out int? result)
        {
            result = -1;
            using (var rep = UnitOfWork.DocumentsTemplatesRepository)
            {
                if (model.DocumentsTemplatesID > 0)
                    result = rep.UpdateEntity(model, model.DocumentsTemplatesID);
                else
                    result = rep.Create(model);

                var list = rep.Where(m => m.PatientID == model.PatientID && !m.DocumentName.ToLower().Trim().Equals("profilepicture") && m.AssociatedType == model.AssociatedType).ToList();
                return list;
            }

        }

        public int DeleteDocument(int id)
        {
            using (var rep = UnitOfWork.DocumentsTemplatesRepository)
            {
                rep.Delete(id);
                return id;
            }
        }

        public async Task<List<DocumentsTemplates>> GetPatientDocumentsList(int patientId)
        {
            try
            {

                using (var rep = UnitOfWork.DocumentsTemplatesRepository)
                {
                    var list = await rep.GetDocumentsAsync(exclusions: "profilepicture", patientId: patientId);
                    return list;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<IEnumerable<DocumentsTemplatesCustomModel>> SaveDocumentsAsync(DataTable dt, bool showList, string exclusions)
        {
            using (var rep = UnitOfWork.DocumentsTemplatesRepository)
            {
                var result = await rep.SaveDocumentsAsync(dt, showList, exclusions);
                return result;
            }
        }
    }
}
