using System.Collections.Generic;
using System.Linq;
using BillingSystem.Bal.Mapper;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.BusinessAccess
{
    public class XAdviceXMLParsedDataBal : BaseBal
    {
        private XMLRemmitanceAdviceMapper XMLRemmitanceAdviceMapper { get; set; }

        public XAdviceXMLParsedDataBal()
        {
            XMLRemmitanceAdviceMapper = new XMLRemmitanceAdviceMapper();
        }

        /// <summary>
        /// Gets the x advice XML parsed data.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public List<XAdviceXMLParsedData> GetXAdviceXMLParsedData(int corporateId, int facilityId)
        {
            using (var rep = UnitOfWork.XAdviceXMLParsedDataRepository)
            {
                var list = corporateId > 0 ? rep.Where(a => a.CorporateID != null && a.FacilityID != null && (int)a.CorporateID == corporateId && (int)a.FacilityID == facilityId).ToList() : rep.GetAll().ToList();
                return list;
            }
        }

        /// <summary>
        /// Gets the x advice XML parsed data custom.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public List<XAdviceXMLParsedDataCustomModel> GetXAdviceXMLParsedDataCustom(int corporateId, int facilityId)
        {
            var listtoreturn = new List<XAdviceXMLParsedDataCustomModel>();
            using (var rep = UnitOfWork.XAdviceXMLParsedDataRepository)
            {
                listtoreturn = rep.GetXAdviceXMLParsedDataCustom(corporateId, facilityId);
                //var list = corporateId > 0 ? rep.Where(a => a.CorporateID != null && a.FacilityID != null && (int)a.CorporateID == corporateId && (int)a.FacilityID == facilityId).ToList() : rep.GetAll().ToList();
                //listtoreturn.AddRange(list.Select(item => XMLRemmitanceAdviceMapper.MapModelToViewModel(item)));
                return listtoreturn;
            }
        }

        public List<TPXMLParsedDataCustomModel> GetXMLFileData(int corporateId, int facilityId)
        {
            var listtoreturn = new List<TPXMLParsedDataCustomModel>();
            using (var rep = UnitOfWork.TPXMLParsedDataRepository)
            {
                var list = corporateId > 0
                    ? rep.Where(
                        a =>
                            a.OMCorporateID != null && a.OMFacilityID != null && (int) a.OMCorporateID == corporateId &&
                            (int) a.OMFacilityID == facilityId).ToList()
                    : rep.GetAll().ToList();
                //listtoreturn.AddRange(list.Select(item => XMLRemmitanceAdviceMapper.MapModelToViewModel(item)));

                return null;
            }
        }

        /// <summary>
        /// Gets the x advice XML parsed data by identifier.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="fileId">The file identifier.</param>
        /// <returns></returns>
        public List<XAdviceXMLParsedDataCustomModel> GetXAdviceXmlParsedDataById(int corporateId, int facilityId, int fileId)
        {
            var listtoreturn = new List<XAdviceXMLParsedDataCustomModel>();
            using (var rep = UnitOfWork.XAdviceXMLParsedDataRepository)
            {
                listtoreturn = rep.GetXAdviceXmlParsedDataById(corporateId, facilityId, fileId);
                return listtoreturn;
            }
        }
    }
}

