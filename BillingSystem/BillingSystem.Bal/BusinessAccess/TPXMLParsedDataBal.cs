using System.Collections.Generic;
using System.Linq;
using BillingSystem.Bal.Mapper;
using BillingSystem.Repository.UOW;
using BillingSystem.Model.CustomModel;
using System.Threading.Tasks;

namespace BillingSystem.Bal.BusinessAccess
{
    public class TPXMLParsedDataBal : BaseBal
    {

        private TPXMLParsedDataMapper TPXMLParsedDataMapper { get; set; }
        private TPFileHeaderMapper TPFileHeaderMapper { get; set; }

        public TPXMLParsedDataBal()
        {
            TPXMLParsedDataMapper = new TPXMLParsedDataMapper();
            TPFileHeaderMapper = new TPFileHeaderMapper();
        }

        /// <summary>
        /// TPXMLs the parsed data list.
        /// </summary>
        /// <param name="tpFileHeaderId">The tp file header identifier.</param>
        /// <returns></returns>
        public List<TPXMLParsedDataCustomModel> TPXMLParsedDataList(int tpFileHeaderId)
        {
            var list = new List<TPXMLParsedDataCustomModel>();
            using (var rep = UnitOfWork.TPXMLParsedDataRepository)
            {
                var lstTpxmlParsedData = rep.Where(t => t.TPFileID == tpFileHeaderId).ToList();
                if (lstTpxmlParsedData.Any())
                {
                    if (lstTpxmlParsedData.Any())
                    {
                        list.AddRange(lstTpxmlParsedData.Select(item => TPXMLParsedDataMapper.MapModelToViewModel(item)));
                    }
                    //list.AddRange(lstTpxmlParsedData.Select(item => new TPXMLParsedDataCustomModel
                    //{
                    //    ACode = item.ACode,
                    //    ANet = item.ANet,
                    //    AOrderingClinician = item.AOrderingClinician,
                    //    APriorAuthorizationID = item.APriorAuthorizationID,
                    //    AQuantity = item.AQuantity,
                    //    AStart = item.AStart,
                    //    AType = item.AType,
                    //    CClaimID = item.CClaimID,
                    //    CEmiratesIDNumber = item.CEmiratesIDNumber,
                    //    CGross = item.CGross,
                    //    CMemberID = item.CMemberID,
                    //    CNet = item.CNet,
                    //    CNPackageName = item.CNPackageName,
                    //    CPatientShare = item.CPatientShare,
                    //    CPayerID = item.CPayerID,
                    //    CProviderID = item.CProviderID,
                    //    DCode = item.DCode,
                    //    DType = item.DType,
                    //    EEnd = item.EEnd,
                    //    EEndType = item.EEndType,
                    //    EFacilityID = item.EFacilityID,
                    //    EPatientID = item.EPatientID,
                    //    EStart = item.EStart,
                    //    EStartType = item.EStartType,
                    //    EType = item.EType,
                    //    ModifiedBy = item.ModifiedBy,
                    //    ModifiedDate = item.ModifiedDate,
                    //    TPFileID = item.TPFileID,
                    //    TPXMLParsedDataID = item.TPXMLParsedDataID,
                    //}));
                }
            }
            return list;
        }

        /// <summary>
        /// TPXMLs the parsed data list cidfid.
        /// </summary>
        /// <param name="corporateid">The corporateid.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public List<TPXMLParsedDataCustomModel> TPXMLParsedDataListCIDFID(int corporateid, int facilityId)
        {
            var list = new List<TPXMLParsedDataCustomModel>();
            using (var rep = UnitOfWork.TPXMLParsedDataRepository)
            {
                var lstTpxmlParsedData = rep.Where(t => t.OMCorporateID == corporateid && t.OMFacilityID == facilityId).ToList().OrderByDescending(x => x.TPXMLParsedDataID).ToList();
                if (lstTpxmlParsedData.Any())
                {
                    list.AddRange(lstTpxmlParsedData.Select(item => TPXMLParsedDataMapper.MapModelToViewModel(item)));
                }
            }
            return list;
        }

        /// <summary>
        /// TPXMLs the files list cidfid.
        /// </summary>
        /// <param name="corporateid">The corporateid.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public List<TPFileHeaderCustomModel> TPXMLFilesListCIDFID(int corporateid, int facilityId)
        {
            //var list = new List<TPFileHeaderCustomModel>();
            using (var rep = UnitOfWork.TPFileHeaderRepository)
            {
                //var lstTpxmlParsedData = rep.Where(t => t.CorporateID == corporateid && t.FacilityID == facilityId).ToList().OrderByDescending(x => x.TPFileHeaderID).ToList();
                //if (lstTpxmlParsedData.Any())
                //{
                //    list.AddRange(lstTpxmlParsedData.Select(item => TPFileHeaderMapper.MapModelToViewModel(item)));
                //}

                var list = rep.GetHeaderListByFacilityId(corporateid, facilityId);
                return list;
            }
            //return list;
        }

        /// <summary>
        /// Deletes the XML parsed data.
        /// </summary>
        /// <param name="cid">The cid.</param>
        /// <param name="fid">The fid.</param>
        /// <returns></returns>
        public bool DeleteXMLParsedData(int cid, int fid)
        {
            using (var rep = UnitOfWork.TPFileHeaderRepository)
            {
                return rep.DeleteXMLParsedData(cid, fid);
            }
        }

        public List<TPXMLParsedDataCustomModel> GetXmlParsedData(long tpFileId)
        {
            using (var rep = UnitOfWork.TPXMLParsedDataRepository)
                return rep.GetXmlParsedData(tpFileId);
        }


        public List<TPFileHeaderCustomModel> DeleteAndThenGetXmlFileData(int corporateId, int facilityId, long fileId, bool? withDetails)
        {
            using (var rep = UnitOfWork.TPFileHeaderRepository)
                return rep.DeleteXmlDataByFileId(corporateId, facilityId, fileId, withDetails);
        }

        public bool ExecuteXmlFileDetails(int corporateId, int facilityid, long fileId)
        {
            using (var rep = UnitOfWork.TPFileHeaderRepository)
            {
                var result = rep.ExecuteXmlFileDetails(corporateId, facilityid, fileId);
                return result;
            }
        }
    }
}
