using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Repository.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class XAdviceXMLParsedDataService : IXAdviceXMLParsedDataService
    {
        private readonly IRepository<XAdviceXMLParsedData> _repository;
        private readonly IRepository<TPXMLParsedData> _tRepository;
        private readonly BillingEntities _context;

        public XAdviceXMLParsedDataService(IRepository<XAdviceXMLParsedData> repository, IRepository<TPXMLParsedData> tRepository, BillingEntities context)
        {
            _repository = repository;
            _tRepository = tRepository;
            _context = context;
        }

        /// <summary>
        /// Gets the x advice XML parsed data.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public List<XAdviceXMLParsedData> GetXAdviceXMLParsedData(int corporateId, int facilityId)
        {
            var list = corporateId > 0 ? _repository.Where(a => a.CorporateID != null && a.FacilityID != null && a.CorporateID == corporateId && (int)a.FacilityID == facilityId).ToList() : _repository.GetAll().ToList();
            return list;
        }

        /// <summary>
        /// Gets the x advice XML parsed data custom.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public List<XAdviceXMLParsedDataCustomModel> GetXAdviceXMLParsedDataCustom(int corporateId, int facilityId)
        {
            var spName = string.Format("EXEC {0} @CId, @FId", StoredProcedures.SPROC_GetXMLParsedDataRemittanceAdvice);
            var sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("CId", corporateId);
            sqlParameters[1] = new SqlParameter("FId", facilityId);
            IEnumerable<XAdviceXMLParsedDataCustomModel> result = _context.Database.SqlQuery<XAdviceXMLParsedDataCustomModel>(spName, sqlParameters);
            return result.ToList();
        }

        public List<TPXMLParsedDataCustomModel> GetXMLFileData(int corporateId, int facilityId)
        {
            var listtoreturn = new List<TPXMLParsedDataCustomModel>();
                var list = corporateId > 0
                    ? _tRepository.Where(
                        a =>
                            a.OMCorporateID != null && a.OMFacilityID != null && (int)a.OMCorporateID == corporateId &&
                            (int)a.OMFacilityID == facilityId).ToList()
                    : _tRepository.GetAll().ToList();
                //listtoreturn.AddRange(list.Select(item => XMLRemmitanceAdviceMapper.MapModelToViewModel(item)));

                return null;
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
            var spName = string.Format("EXEC {0} @pCId, @pFId, @pFileId", StoredProcedures.SPROC_GetXMLParsedDataRemittanceAdviceById);
            var sqlParameters = new SqlParameter[3];
            sqlParameters[0] = new SqlParameter("pCId", corporateId);
            sqlParameters[1] = new SqlParameter("pFId", facilityId);
            sqlParameters[2] = new SqlParameter("pFileId", fileId);
            IEnumerable<XAdviceXMLParsedDataCustomModel> result = _context.Database.SqlQuery<XAdviceXMLParsedDataCustomModel>(spName, sqlParameters);
            return result.ToList();
        }
    }
}

