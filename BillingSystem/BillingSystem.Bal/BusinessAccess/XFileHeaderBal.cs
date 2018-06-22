using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Repository.UOW;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.BusinessAccess
{
    using BillingSystem.Common;

    public class XFileHeaderBal : BaseBal
    {
        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<XFileHeaderCustomModel> GetXFileHeader()
        {
            var list = new List<XFileHeaderCustomModel>();
            using (var xFileHeaderRep = UnitOfWork.XFileHeaderRepository)
            {
                var lstXFileHeader = xFileHeaderRep.GetAll().ToList();
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
                        StatusStr = this.GetStatusStr(item.Status)
                    }));
                }
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
            using (var rep = UnitOfWork.XFileHeaderRepository)
            {
                if (model.FileID > 0)
                    rep.UpdateEntity(model, Convert.ToInt32(model.FileID));
                else
                    rep.Create(model);
                return Convert.ToInt32(model.FileID);
            }
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="XFileHeaderId">The x file header identifier.</param>
        /// <returns></returns>
        public XFileHeader GetXFileHeaderByID(int? XFileHeaderId)
        {
            using (var rep = UnitOfWork.XFileHeaderRepository)
            {
                var model = rep.Where(x => x.FileID == XFileHeaderId).FirstOrDefault();
                return model;
            }
        }

        /// <summary>
        /// Gets the formatted XML string by x file identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public string GetRemittanceFormattedXmlStringByFileId(int id)
        {
            using (var rep = UnitOfWork.XFileXMLRepository)
            {
                var result = rep.Where(f => f.FileXMLID == id).FirstOrDefault();
                if (result != null)
                {
                    var formattedXml = XmlParser.GetFormattedXml(result.XFileXML1);
                    return formattedXml;
                }
                return string.Empty;
            }
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
            using (var xFileHeaderRep = UnitOfWork.XFileHeaderRepository)
            {
                var lstXFileHeader = xFileHeaderRep.Where(x => x.FacilityID == fId && x.CorporateID == cId && x.FileType.Equals("IN")).ToList().OrderByDescending(x => x.FileID).ToList();
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
                        StatusStr = this.GetStatusStr(item.Status)
                    }));
                }
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
            statusstr = this.GetNameByGlobalCodeValue(status, "1766");
            return !string.IsNullOrEmpty(statusstr) ? statusstr : "Old Dirty Records";
            //switch (status)
            //{
            //    case "0":
            //        statusstr = "Charges Not Applied";
            //        break;
            //    case "1":
            //        statusstr = "Charges Applied";
            //        break;
            //    case "9":
            //        statusstr = "Invalid XML (Record Count does not match)";
            //        break;
            //    case "99":
            //        statusstr = "Duplicate File";
            //        break;
            //    case "999":
            //        statusstr = "Duplicate File";
            //        break;
            //}
            //return statusstr;
        }
    }

}
