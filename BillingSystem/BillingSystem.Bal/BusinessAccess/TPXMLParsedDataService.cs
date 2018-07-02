using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using AutoMapper;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;



namespace BillingSystem.Bal.BusinessAccess
{
    public class TPXMLParsedDataService : ITPXMLParsedDataService
    {
        private readonly IRepository<TPXMLParsedData> _repository;
        private readonly IRepository<BillHeader> _blRepository;
        private readonly IRepository<Corporate> _cRepository;
        private readonly IRepository<PatientInfo> _piRepository;
        private readonly IRepository<Facility> _fRepository;
        private readonly IRepository<InsuranceCompany> _icRepository;
        private readonly IRepository<GlobalCodes> _gRepository;
        private readonly IMapper _mapper;
        private readonly BillingEntities _context;

        public TPXMLParsedDataService(IRepository<TPXMLParsedData> repository, IRepository<BillHeader> blRepository, IRepository<Corporate> cRepository, IRepository<PatientInfo> piRepository, IRepository<Facility> fRepository, IRepository<InsuranceCompany> icRepository, IRepository<GlobalCodes> gRepository, IMapper mapper, BillingEntities context)
        {
            _repository = repository;
            _blRepository = blRepository;
            _cRepository = cRepository;
            _piRepository = piRepository;
            _fRepository = fRepository;
            _icRepository = icRepository;
            _gRepository = gRepository;
            _mapper = mapper;
            _context = context;
        }


        /// <summary>
        /// TPXMLs the parsed data list.
        /// </summary>
        /// <param name="tpFileHeaderId">The tp file header identifier.</param>
        /// <returns></returns>
        public List<TPXMLParsedDataCustomModel> TPXMLParsedDataList(int tpFileHeaderId)
        {
            var list = new List<TPXMLParsedDataCustomModel>();
            var lstTpxmlParsedData = _repository.Where(t => t.TPFileID == tpFileHeaderId).ToList();
            if (lstTpxmlParsedData.Any())
            {
                if (lstTpxmlParsedData.Any())
                {
                    list.AddRange(MapValues(lstTpxmlParsedData));
                }
            }
            return list;
        }

        private List<TPXMLParsedDataCustomModel> MapValues(List<TPXMLParsedData> m)
        {
            var lst = new List<TPXMLParsedDataCustomModel>();
            foreach (var model in m)
            {
                var vm = _mapper.Map<TPXMLParsedDataCustomModel>(model);
                if (vm != null)
                {
                    vm.BillNumber = GetBillNumberByBillHeaderId(Convert.ToInt32(vm.OMBillID));
                    vm.CorporateName = GetCorporateNameFromId(Convert.ToInt32(vm.OMCorporateID));
                    vm.FacilityName = GetFacilityNameByFacilityId(Convert.ToInt32(vm.OMFacilityID));
                    vm.EncounterType = GetNameByGlobalCodeValue(vm.EType, "1107");
                    vm.EncounterStartType = GetNameByGlobalCodeValue(vm.EStartType, "1116");
                    vm.EncounterEndType = GetNameByGlobalCodeValue(vm.EEndType, "1114");
                    vm.InsuranceCompany = !string.IsNullOrEmpty(vm.CPayerID) ? GetInsuranceCompanyNameByPayerId(vm.CPayerID) : vm.CPayerID;
                    vm.PatientName = GetPatientNameById(Convert.ToInt32(vm.OMPatientID));
                }
                lst.Add(vm);
            }
            return lst;
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
        private string GetInsuranceCompanyNameByPayerId(string payorId)
        {
            var ins = _icRepository.Where(e => e.InsuranceCompanyLicenseNumber.Equals(payorId)).FirstOrDefault();
            return ins != null ? ins.InsuranceCompanyName : string.Empty;

        }
        private string GetFacilityNameByFacilityId(int facilityId)
        {
            Facility facility = null;
            if (facilityId > 0)
            {
                facility = _fRepository.Where(f => f.FacilityId == facilityId).FirstOrDefault();
            }
            return facility != null ? facility.FacilityName : string.Empty;
        }
        private string GetPatientNameById(int patientId)
        {
            var patient = _piRepository.Where(p => p.PatientID == patientId).FirstOrDefault();
            return patient != null
                       ? string.Format("{0} {1}", patient.PersonFirstName, patient.PersonLastName)
                       : string.Empty;
        }
        private string GetBillNumberByBillHeaderId(int billHeaderId)
        {
            var billheaderobj = _blRepository.Where(x => x.BillHeaderID == billHeaderId).FirstOrDefault();// GetBillHeaderById(billHeaderId);
            return billheaderobj != null ? billheaderobj.BillNumber : "NA";
        }
        private string GetCorporateNameFromId(int corpId)
        {
            var corpName = "";
            var obj = _cRepository.Where(f => f.CorporateID == corpId).FirstOrDefault();
            if (obj != null) corpName = obj.CorporateName;
            return corpName;
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
            var lstTpxmlParsedData = _repository.Where(t => t.OMCorporateID == corporateid && t.OMFacilityID == facilityId).ToList().OrderByDescending(x => x.TPXMLParsedDataID).ToList();
            if (lstTpxmlParsedData.Any())
            {
                list.AddRange(MapValues(lstTpxmlParsedData));
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
            string spName = $"EXEC {StoredProcedures.SprocGetTpFileHeaderListByFacilityId} @CId,@FId";
            var sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("CId", corporateid);
            sqlParameters[1] = new SqlParameter("@FId", facilityId);
            var result = _context.Database.SqlQuery<TPFileHeaderCustomModel>(spName, sqlParameters);
            return result.ToList();
        }

        /// <summary>
        /// Deletes the XML parsed data.
        /// </summary>
        /// <param name="cid">The cid.</param>
        /// <param name="fid">The fid.</param>
        /// <returns></returns>
        public bool DeleteXMLParsedData(int cid, int fid)
        {
            var sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("pCID", cid);
            sqlParameters[1] = new SqlParameter("pFID", fid);
            _repository.ExecuteCommand(StoredProcedures.Delete_XMlParsedData_SA.ToString(), sqlParameters);
            return true;
        }

        public List<TPXMLParsedDataCustomModel> GetXmlParsedData(long tpFileId)
        {
            var spName = $"EXEC {StoredProcedures.SPROCGetXMLParsedDataByFileId} @FileId";
            var sqlParameters = new SqlParameter[1];
            sqlParameters[0] = new SqlParameter("FileId", tpFileId);

            using (var r = _context.MultiResultSetSqlQuery(spName, parameters: sqlParameters))
            {
                var result = r.ResultSetFor<TPXMLParsedDataCustomModel>().ToList();
                return result != null ? result : new List<TPXMLParsedDataCustomModel>();
            }
        }


        public List<TPFileHeaderCustomModel> DeleteAndThenGetXmlFileData(int corporateId, int facilityId, long fileId, bool? withDetails)
        {
            string spName = $"EXEC {StoredProcedures.SprocDeleteXmlParsedDataByFileId} @pCId,@pFId,@pFileId,@pWithDetails";
            var sqlParameters = new SqlParameter[4];
            sqlParameters[0] = new SqlParameter("pCId", corporateId);
            sqlParameters[1] = new SqlParameter("pFId", facilityId);
            sqlParameters[2] = new SqlParameter("pFileId", fileId);
            sqlParameters[3] = new SqlParameter("pWithDetails", withDetails);
            var result = _context.Database.SqlQuery<TPFileHeaderCustomModel>(spName, sqlParameters);
            return result.ToList();
        }

        public bool ExecuteXmlFileDetails(int corporateId, int facilityid, long fileId)
        {
            var sqlParameters = new SqlParameter[3];
            sqlParameters[0] = new SqlParameter("pCID", corporateId);
            sqlParameters[1] = new SqlParameter("pFID", facilityid);
            sqlParameters[2] = new SqlParameter("pFileHeaderId", fileId);
            _repository.ExecuteCommand(StoredProcedures.SprocXmlParseDetails.ToString(), sqlParameters);
            return true;
        }
    }
}
