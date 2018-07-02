using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Model;
using BillingSystem.Common;
using BillingSystem.Model.CustomModel;
using BillingSystem.Repository.Interfaces;
using AutoMapper;
using System.Data.SqlClient;
using System.Data;
using BillingSystem.Common.Common;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class XMLBillingService : IXMLBillingService
    {
        private readonly IRepository<XFileHeader> _fhRepository;
        private readonly IRepository<XFileXML> _repository;


        private readonly IMapper _mapper;
        private readonly BillingEntities _context;

        public XMLBillingService(IRepository<XFileHeader> fhRepository, IRepository<XFileXML> repository, IMapper mapper, BillingEntities context)
        {
            _fhRepository = fhRepository;
            _repository = repository;
            _mapper = mapper;
            _context = context;
        }

        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<XFileHeader> GetXFileHeader(int facilityId, int corporateId)
        {
            var lstXFileHeader = _fhRepository.Where(X => X.FacilityID == facilityId && X.CorporateID == corporateId).OrderByDescending(_ => _.FileID).ToList();
            return lstXFileHeader;
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

            var lstXFileHeader = _fhRepository.Where(X => X.FacilityID == facilityId && X.CorporateID == corporateId).OrderByDescending(_ => _.FileID).ToList();
            if (lstXFileHeader.Count > 0)
                list.AddRange(MapValues(lstXFileHeader));
            return list;

        }
        private List<XFileHeaderCustomModel> MapValues(List<XFileHeader> m)
        {
            var list = new List<XFileHeaderCustomModel>();
            foreach (var model in m)
            {
                var vm = _mapper.Map<XFileHeaderCustomModel>(model);
                if (vm != null)
                {
                    vm.StatusBit = string.IsNullOrEmpty(vm.Status) && Convert.ToBoolean(vm.Status);
                    vm.StatusStr = string.IsNullOrEmpty(vm.Status) ? Convert.ToString(vm.Status) : string.Empty;
                }
                list.Add(vm);
            }

            return list;
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="XFileHeader"></param>
        /// <returns></returns>
        public int AddUptdateXFileHeader(XFileHeader XFileHeader)
        {
            if (XFileHeader.FileID > 0)
                _fhRepository.UpdateEntity(XFileHeader, Convert.ToInt32(XFileHeader.FileID));
            else
                _fhRepository.Create(XFileHeader);
            return Convert.ToInt32(XFileHeader.FileID);
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="shared"></param>
        /// <returns></returns>
        public XFileHeader GetXFileHeaderByID(int? XFileHeaderId)
        {
            var xFileHeader = _fhRepository.Where(x => x.FileID == XFileHeaderId).FirstOrDefault();
            return xFileHeader;
        }

        /// <summary>
        /// Gets the latest x file header identifier.
        /// </summary>
        /// <returns></returns>
        public int GetLatestXFileHeaderId()
        {
            var xFileHeader = _fhRepository.GetAll().OrderByDescending(x => x.FileID).FirstOrDefault();
            return xFileHeader != null ? Convert.ToInt32(xFileHeader.FileID) : 0;
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
            var sqlParameters = new SqlParameter[3];
            sqlParameters[0] = new SqlParameter
            {
                SqlDbType = SqlDbType.Xml,
                ParameterName = "XMLIN",
                Value = xml
            };
            sqlParameters[1] = new SqlParameter("FullPath", fullPath);
            sqlParameters[2] = new SqlParameter
            {
                ParameterName = "SuccessFlag",
                Value = sFlag,
                Direction = ParameterDirection.Output
            };
            _fhRepository.ExecuteCommand(StoredProcedures.XMLParser.ToString(), sqlParameters);
            return true;
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
            var spToExecute = string.Format("EXEC {0} @XMLIN, @FullPath, @CorporateID, @FacilityID, @SuccessFlag", StoredProcedures.XMLRemittanceAdviceParser.ToString());
            var sqlParameters = new SqlParameter[5];
            sqlParameters[0] = new SqlParameter
            {
                SqlDbType = SqlDbType.Xml,
                ParameterName = "XMLIN",
                Value = xml
            };
            sqlParameters[1] = new SqlParameter("FullPath", fullPath);
            sqlParameters[2] = new SqlParameter("CorporateID", corporateId);
            sqlParameters[3] = new SqlParameter("FacilityID", facilityId);
            sqlParameters[4] = new SqlParameter
            {
                ParameterName = "SuccessFlag",
                Value = sFlag,
                Direction = ParameterDirection.Output
            };
            //ExecuteCommand(spToExecute, sqlParameters);
            IEnumerable<XMLBillingFileStatus> result = _context.Database.SqlQuery<XMLBillingFileStatus>(spToExecute, sqlParameters);
            var filestatus = result.ToList();
            if (filestatus.Any())
            {
                return (filestatus.FirstOrDefault().SuccessFlag);
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the formatted XML string by x file identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public string GetFormattedXmlStringByXFileId(int id)
        {
            var result = _repository.Where(f => f.FileXMLID == id).FirstOrDefault();
            if (result != null)
            {
                var formattedXml = XmlParser.GetFormattedXml(result.XFileXML1);
                return formattedXml;
            }

            return string.Empty;
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
            var spToExecute = string.Format("EXEC {0} @pCID, @XMLIN, @FullPath,@pFID, @SuccessFlag,@UBatchNumber,@pExecuteDetails,@pLoggedInUserId", StoredProcedures.XMLParser.ToString());
            var sqlParameters = new SqlParameter[8];
            sqlParameters[0] = new SqlParameter("pCID", corporateId);
            sqlParameters[1] = new SqlParameter
            {
                SqlDbType = SqlDbType.Xml,
                ParameterName = "XMLIN",
                Value = xml
            };
            sqlParameters[2] = new SqlParameter("FullPath", fullPath);
            sqlParameters[3] = new SqlParameter("pFID", facilityId);
            sqlParameters[4] = new SqlParameter
            {
                ParameterName = "SuccessFlag",
                Value = sFlag,
                Direction = ParameterDirection.Output
            };
            sqlParameters[5] = new SqlParameter("UBatchNumber", batchNumber);
            sqlParameters[6] = new SqlParameter("pExecuteDetails", executeDetails);
            sqlParameters[7] = new SqlParameter("pLoggedInUserId", loggedinUserId);
            IEnumerable<XMLBillingFileStatus> result = _context.Database.SqlQuery<XMLBillingFileStatus>(spToExecute, sqlParameters);
            var filestatus = result.ToList();
            if (filestatus.Any())
            {
                return (filestatus.FirstOrDefault().SuccessFlag);
            }
            return string.Empty;
        }


        /// <summary>
        /// Gets the XML string by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public XFileXML GetXmlStringById(int id)
        {
            var result = _repository.Where(f => f.FileXMLID == id).FirstOrDefault();
            return result;
        }
    }
}

