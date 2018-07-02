using System.Collections.Generic;
using System.Linq;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;


namespace BillingSystem.Bal.BusinessAccess
{
    public class TpFileHeaderService : ITpFileHeaderService
    {
        private readonly IRepository<TPFileHeader> _repository;
        private readonly IRepository<TPFileXML> _tRepository;

        public TpFileHeaderService(IRepository<TPFileHeader> repository, IRepository<TPFileXML> tRepository)
        {
            _repository = repository;
            _tRepository = tRepository;
        }


        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public IEnumerable<TPFileHeaderCustomModel> TpFileHeaderList(int corporateId)
        {
            var list = new List<TPFileHeaderCustomModel>();
            var tpFileHeaderList = _repository.GetAll().ToList();
            if (tpFileHeaderList.Any())
            {
                list.AddRange(tpFileHeaderList.Select(item => new TPFileHeaderCustomModel
                {
                    AcknowledgeDate = item.AcknowledgeDate,
                    AcknowledgeNum = item.AcknowledgeNum,
                    CorporateID = item.CorporateID,
                    DispositionFlag = item.DispositionFlag,
                    FacilityID = item.FacilityID,
                    FailFilePath = item.FailFilePath,
                    FileName = item.FileName,
                    FileType = item.FileType,
                    INFilePath = item.INFilePath,
                    IsActive = item.IsActive,
                    ModifiedBy = item.ModifiedBy,
                    ModifiedDate = item.ModifiedDate,
                    ReceiverID = item.ReceiverID,
                    RecordCount = item.RecordCount,
                    SenderID = item.SenderID,
                    SentDate = item.SentDate,
                    Status = item.Status,
                    SuccessFilePath = item.SuccessFilePath,
                    TPFileHeaderID = item.TPFileHeaderID,
                    TransactionDate = item.TransactionDate
                }));
            }
            return list;
        }

        /// <summary>
        /// Gets the formatted XML string by x file identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public string GetFormattedXmlStringByXFileId(int id)
        {
            var result = _tRepository.Where(f => f.TPFileXMLID == id).FirstOrDefault();
            if (result != null)
            {
                var formattedXml = XmlParser.GetFormattedXml(result.XFileXML);
                return formattedXml;
            }
            return string.Empty;
        }
    }
}
