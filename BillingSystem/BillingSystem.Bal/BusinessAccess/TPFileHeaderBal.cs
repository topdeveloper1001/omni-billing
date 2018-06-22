using System.Collections.Generic;
using System.Linq;
using BillingSystem.Common;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.BusinessAccess
{
    public class TpFileHeaderBal : BaseBal
    {
        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public IEnumerable<TPFileHeaderCustomModel> TpFileHeaderList(int corporateId)
        {
            var list = new List<TPFileHeaderCustomModel>();
            using (var rep = UnitOfWork.TPFileHeaderRepository)
            {
                var tpFileHeaderList = rep.GetAll().ToList();
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
            using (var rep = UnitOfWork.TPFileXMLRepository)
            {
                var result = rep.Where(f => f.TPFileXMLID == id).FirstOrDefault();
                if (result != null)
                {
                    var formattedXml = XmlParser.GetFormattedXml(result.XFileXML);
                    return formattedXml;
                }
                return string.Empty;
            }
        }
    }
}
