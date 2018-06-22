using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Model;
using BillingSystem.Common;

namespace BillingSystem.Bal.BusinessAccess
{
    using BillingSystem.Bal.Mapper;
    using BillingSystem.Model.CustomModel;

    public class XMLBillingBal : BaseBal
    {

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AppointmentTypesBal"/> class.
        /// </summary>
        public XMLBillingBal()
        {
            XMLBillingMapper = new XMLBillingMapper();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the holiday planner mapper.
        /// </summary>
        private XMLBillingMapper XMLBillingMapper { get; set; }

        #endregion

        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<XFileHeader> GetXFileHeader(int facilityId, int corporateId)
        {
            try
            {
                using (var xFileHeaderRep = UnitOfWork.XFileHeaderRepository)
                {
                    var lstXFileHeader = xFileHeaderRep.Where(X => X.FacilityID == facilityId && X.CorporateID == corporateId).OrderByDescending(_ => _.FileID).ToList();
                    return lstXFileHeader;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the x file header c model.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <returns></returns>
        public List<XFileHeaderCustomModel> GetXFileHeaderCModel(int facilityId, int corporateId)
        {
            var list = new List<XFileHeaderCustomModel>();
            try
            {
                using (var xFileHeaderRep = UnitOfWork.XFileHeaderRepository)
                {
                    var lstXFileHeader = xFileHeaderRep.Where(X => X.FacilityID == facilityId && X.CorporateID == corporateId).OrderByDescending(_ => _.FileID).ToList();
                    if (lstXFileHeader.Count > 0)
                        list.AddRange(lstXFileHeader.Select(item => XMLBillingMapper.MapModelToViewModel(item)));
                    return list;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get Entity Name By Id
        /// </summary>
        /// <returns>Return the Entity Respository</returns>
        // public string GetXFileHeaderNameById(int? XFileHeaderID)
        // {
        //   using (var XFileHeaderRep = UnitOfWork.XFileHeaderRepository)
        //   {
        //       var iQueryabletransactions = XFileHeaderRep.Where(a => a.XFileHeaderId == XFileHeaderID).FirstOrDefault();
        //       return (iQueryabletransactions != null) ? iQueryabletransactions.XFileHeaderName : string.Empty;
        //   }
        //}

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="XFileHeader"></param>
        /// <returns></returns>
        public int AddUptdateXFileHeader(XFileHeader XFileHeader)
        {
            using (var xFileHeaderRep = UnitOfWork.XFileHeaderRepository)
            {
                if (XFileHeader.FileID > 0)
                    xFileHeaderRep.UpdateEntity(XFileHeader, Convert.ToInt32(XFileHeader.FileID));
                else
                    xFileHeaderRep.Create(XFileHeader);
                return Convert.ToInt32(XFileHeader.FileID);
            }
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="shared"></param>
        /// <returns></returns>
        public XFileHeader GetXFileHeaderByID(int? XFileHeaderId)
        {
            using (var xFileHeaderRep = UnitOfWork.XFileHeaderRepository)
            {
                var xFileHeader = xFileHeaderRep.Where(x => x.FileID == XFileHeaderId).FirstOrDefault();
                return xFileHeader;
            }
        }

        /// <summary>
        /// Gets the latest x file header identifier.
        /// </summary>
        /// <returns></returns>
        public int GetLatestXFileHeaderId()
        {
            using (var xFileHeaderRep = UnitOfWork.XFileHeaderRepository)
            {
                var xFileHeader = xFileHeaderRep.GetAll().OrderByDescending(x => x.FileID).FirstOrDefault();
                return xFileHeader != null ? Convert.ToInt32(xFileHeader.FileID) : 0;
            }
        }

        /// <summary>
        /// Imports the XML bills.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <param name="fullPath">The full path.</param>
        /// <param name="sFlag">if set to <c>true</c> [s flag].</param>
        /// <returns></returns>
        public bool ImportXmlBills(string xml, string fullPath, bool sFlag)
        {
            using (var rep = UnitOfWork.XFileHeaderRepository)
            {
                var result = rep.ImportXMLBilling(xml, fullPath, sFlag);
                return result;
            }
        }

        /// <summary>
        /// Remittances the XML parser.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <param name="fullPath">The full path.</param>
        /// <param name="sFlag">if set to <c>true</c> [s flag].</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public string RemittanceXMLParser(string xml, string fullPath, bool sFlag, int corporateId, int facilityId)
        {
            using (var rep = UnitOfWork.XFileXMLRepository)
            {
                var result = rep.XMLRemittanceAdviceParser(xml, fullPath, sFlag, corporateId, facilityId);
                return result;
            }
        }

        /// <summary>
        /// Gets the formatted XML string by x file identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public string GetFormattedXmlStringByXFileId(int id)
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
        /// XMLs the bill file parser.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <param name="fullPath">The full path.</param>
        /// <param name="sFlag">if set to <c>true</c> [s flag].</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="batchNumber">The batch number.</param>
        /// <returns></returns>
        public string XMLBillFileParser(string xml, string fullPath, bool sFlag, int corporateId, int facilityId, string batchNumber, bool? executeDetails = false, long? loggedinUserId = 0)
        {
            using (var rep = UnitOfWork.XFileXMLRepository)
            {
                var result = rep.XMLBillFileParser(xml, fullPath, sFlag, corporateId, facilityId, batchNumber, executeDetails, loggedinUserId);
                return result;
            }
        }


        /// <summary>
        /// Gets the XML string by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public XFileXML GetXmlStringById(int id)
        {
            using (var rep = UnitOfWork.XFileXMLRepository)
            {
                var result = rep.Where(f => f.FileXMLID == id).FirstOrDefault();
                return result;
            }
        }
    }
}

