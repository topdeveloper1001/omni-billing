using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Common;
using BillingSystem.Repository.Interfaces;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{ 
    public class XFileHeaderService : IXFileHeaderService
    {
        private readonly IRepository<XFileHeader> _repository;
        private readonly IRepository<XFileXML> _fxRepository;
        private readonly IRepository<GlobalCodes> _gRepository;

        public XFileHeaderService(IRepository<XFileHeader> repository, IRepository<XFileXML> fxRepository, IRepository<GlobalCodes> gRepository)
        {
            _repository = repository;
            _fxRepository = fxRepository;
            _gRepository = gRepository;
        }

        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<XFileHeaderCustomModel> GetXFileHeader()
        {
            var list = new List<XFileHeaderCustomModel>();
            var lstXFileHeader = _repository.GetAll().ToList();
            if (lstXFileHeader.Count > 0)
            {
                list.AddRange(lstXFileHeader.Select(item => new XFileHeaderCustomModel
                {
                    FileID = item.FileID,
                    FileType = item.FileType,
                    SenderID = item.SenderID,
                    ReceiverID = item.ReceiverID,
                    TransactionDate = item.TransactionDate,
                    RecordCount = item.RecordCount,
                    DispositionFlag = item.DispositionFlag,
                    XPath = item.XPath,
                    Status = item.Status,
                    SentDate = item.SentDate,
                    AcknowledgeNum = item.AcknowledgeNum,
                    AcknowledgeDate = item.AcknowledgeDate,
                    ModifiedBy = item.ModifiedBy,
                    ModifiedDate = item.ModifiedDate,
                    CorporateID = item.CorporateID,
                    FacilityID = item.FacilityID,
                    StatusStr = GetStatusStr(item.Status)
                }));
            }
            return list;
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="XFileHeader"></param>
        /// <returns></returns>
        public int SaveXFileHeader(XFileHeader model)
        {
            if (model.FileID > 0)
                _repository.UpdateEntity(model, Convert.ToInt32(model.FileID));
            else
                _repository.Create(model);
            return Convert.ToInt32(model.FileID);
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="XFileHeaderId">The x file header identifier.</param>
        /// <returns></returns>
        public XFileHeader GetXFileHeaderByID(int? XFileHeaderId)
        {
            var model = _repository.Where(x => x.FileID == XFileHeaderId).FirstOrDefault();
            return model;
        }

        /// <summary>
        /// Gets the formatted XML string by x file identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public string GetRemittanceFormattedXmlStringByFileId(int id)
        {
            var result = _fxRepository.Where(f => f.FileXMLID == id).FirstOrDefault();
            if (result != null)
            {
                var formattedXml = XmlParser.GetFormattedXml(result.XFileXML1);
                return formattedXml;
            }
            return string.Empty;

        }

        /// <summary>
        /// Gets the x file header by c identifier.
        /// </summary>
        /// <param name="cId">The c identifier.</param>
        /// <param name="fId">The f identifier.</param>
        /// <returns></returns>
        public List<XFileHeaderCustomModel> GetXFileHeaderByCId(int cId, int fId)
        {
            var list = new List<XFileHeaderCustomModel>();
            var lstXFileHeader = _repository.Where(x => x.FacilityID == fId && x.CorporateID == cId && x.FileType.Equals("IN")).ToList().OrderByDescending(x => x.FileID).ToList();
            if (lstXFileHeader.Count > 0)
            {
                list.AddRange(lstXFileHeader.Select(item => new XFileHeaderCustomModel
                {
                    FileID = item.FileID,
                    FileType = item.FileType,
                    SenderID = item.SenderID,
                    ReceiverID = item.ReceiverID,
                    TransactionDate = item.TransactionDate,
                    RecordCount = item.RecordCount,
                    DispositionFlag = item.DispositionFlag,
                    XPath = item.XPath,
                    Status = item.Status,
                    SentDate = item.SentDate,
                    AcknowledgeNum = item.AcknowledgeNum,
                    AcknowledgeDate = item.AcknowledgeDate,
                    ModifiedBy = item.ModifiedBy,
                    ModifiedDate = item.ModifiedDate,
                    CorporateID = item.CorporateID,
                    FacilityID = item.FacilityID,
                    StatusStr = GetStatusStr(item.Status)
                }));
            }
            return list;
        }

        /// <summary>
        /// Gets the status string.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <returns></returns>
        private string GetStatusStr(string status)
        {
            var statusstr = string.Empty;
            statusstr = GetNameByGlobalCodeValue(status, "1766");
            return !string.IsNullOrEmpty(statusstr) ? statusstr : "Old Dirty Records";

        }
        private string GetNameByGlobalCodeValue(string codeValue, string categoryValue, string fId = "")
        {
            if (!string.IsNullOrEmpty(codeValue))
            {
                var gl = _gRepository.Where(g => g.GlobalCodeValue.Equals(codeValue) && !g.IsDeleted.Value && g.GlobalCodeCategoryValue.Equals(categoryValue) && (string.IsNullOrEmpty(fId) || g.FacilityNumber.Equals(fId))).FirstOrDefault();
                return gl != null ? gl.GlobalCodeName : string.Empty;
            }
            return string.Empty;
        }
    }

}
