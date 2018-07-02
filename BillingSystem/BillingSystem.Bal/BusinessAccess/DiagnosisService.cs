
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using AutoMapper;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;



namespace BillingSystem.Bal.BusinessAccess
{
    public class DiagnosisService : IDiagnosisService
    {
        private readonly IRepository<Diagnosis> _repository;
        private readonly IRepository<DiagnosisCode> _dRepository;
        private readonly IRepository<Facility> _fRepository;
        private readonly BillingEntities _context;
        private readonly IMapper _mapper;

        public DiagnosisService(IRepository<Diagnosis> repository, IRepository<DiagnosisCode> dRepository, IRepository<Facility> fRepository, BillingEntities context, IMapper mapper)
        {
            _repository = repository;
            _dRepository = dRepository;
            _fRepository = fRepository;
            _context = context;
            _mapper = mapper;
        }

        private DateTime GetInvariantCultureDateTime(int facilityid)
        {
            var facilityObj = _fRepository.Where(f => f.FacilityId == Convert.ToInt32(facilityid)).FirstOrDefault() != null ? _fRepository.Where(f => f.FacilityId == Convert.ToInt32(facilityid)).FirstOrDefault().FacilityTimeZone : TimeZoneInfo.Utc.ToString();
            var tzi = TimeZoneInfo.FindSystemTimeZoneById(facilityObj);
            var utcTime = DateTime.Now.ToUniversalTime();
            var convertedTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);
            return convertedTime;
        }
        public bool CheckIfPrimaryDiagnosis(int patientId, int encounterId, int diagnosisType)
        {
            bool isExists;
            isExists = _repository.Where(d => d.PatientID == patientId && d.EncounterID == encounterId && d.IsDeleted != true && d.DiagnosisType == diagnosisType).Any();
            return isExists;
        }
        //Function to get all GlobalCodes
        /// <summary>
        /// Gets all diagnosis codes.
        /// </summary>
        /// <returns></returns>
        public List<DiagnosisCode> GetAllDiagnosisCodes(int facilityId, string DiagnosisTableNumber)
        {
            var currentDateTime = GetInvariantCultureDateTime(facilityId);
            var list = _dRepository.Where(s => (s.DiagnosisEffectiveStartDate == null || (s.DiagnosisEffectiveStartDate <= currentDateTime && s.DiagnosisEffectiveEndDate >= currentDateTime)) && s.DiagnosisTableNumber.Trim().Equals(DiagnosisTableNumber)).ToList();
            return list;
        }


        /// <summary>
        /// Gets the diagnosis list.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public List<DiagnosisCustomModel> GetDiagnosisList(int patientId)
        {
            var list = _repository.Where(d => d.PatientID != null && d.PatientID == patientId).OrderBy(ss => ss.DiagnosisType).ToList();
            return list.Select(x => _mapper.Map<DiagnosisCustomModel>(x)).OrderByDescending(x => x.CreatedDate).ToList();
        }

        /// <summary>
        /// Gets the diagnosis list.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="encounterid">The encounterid.</param>
        /// <returns></returns>
        public List<DiagnosisCustomModel> GetDiagnosisList(int patientId, int encounterid)
        {
            var list = _repository.Where(d => d.PatientID != null && (int)d.PatientID == patientId && d.EncounterID == encounterid).OrderBy(ss => ss.DiagnosisType).ThenByDescending(x => x.CreatedDate).ToList();
            return list.Select(x => _mapper.Map<DiagnosisCustomModel>(x)).OrderByDescending(x => x.CreatedDate).ToList();
        }

        /// <summary>
        /// Gets the previous diagnosis list.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="encounterid">The encounterid.</param>
        /// <returns></returns>
        public List<DiagnosisCustomModel> GetPreviousDiagnosisList(int patientId, int encounterid)
        {
            var list = _repository.Where(d => d.PatientID != null && d.PatientID == patientId && d.EncounterID != encounterid).OrderBy(ss => ss.DiagnosisType).ToList();
            return list.Select(x => _mapper.Map<DiagnosisCustomModel>(x)).OrderByDescending(x => x.CreatedDate).ToList();

        }

        /// <summary>
        /// Gets the filtered diagnosis codes.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public IEnumerable<DiagnosisCode> GetFilteredDiagnosisCodes(string keyword, long userId, long facilityId, string DiagnosisTableNumber)
        {
            var sqlParams = new SqlParameter[4];
            sqlParams[0] = new SqlParameter("@pUserId", userId);
            sqlParams[1] = new SqlParameter("@pKeyword", keyword);
            sqlParams[2] = new SqlParameter("@pTableNumber", DiagnosisTableNumber);
            sqlParams[3] = new SqlParameter("@pFId", facilityId);

            using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetDiagnosisCodes.ToString(), isCompiled: false, parameters: sqlParams))
            {
                try
                {
                    var list = r.GetResultWithJson<DiagnosisCode>(JsonResultsArray.DiagnosisCodes.ToString());
                    return list;
                }
                catch
                {
                    return Enumerable.Empty<DiagnosisCode>().ToList();
                }
            }
        }

        /// <summary>
        /// Saves the diagnosis.
        /// </summary>
        /// <param name="vm">The model.</param>
        /// <returns></returns>
        public int SaveDiagnosis(DiagnosisCustomModel vm, string DiagnosisTableNumber)
        {
            var m = _mapper.Map<Diagnosis>(vm);
            var diagnosisCode = GetDiagnosisCodeById(vm.DiagnosisCode, DiagnosisTableNumber);
            if (m.DiagnosisType == (int)DiagnosisType.Primary || m.DiagnosisType == (int)DiagnosisType.Secondary)
                m.DiagnosisCodeId = diagnosisCode.DiagnosisTableNumberId;

            if (m.DiagnosisID > 0)
                _repository.UpdateEntity(m, m.DiagnosisID);
            else
                _repository.Create(m);

            return m.DiagnosisID;
        }

        /// <summary>
        /// Gets the diagnosis code by identifier.
        /// </summary>
        /// <param name="codeId">The code identifier.</param>
        /// <returns></returns>
        private DiagnosisCode GetDiagnosisCodeById(string codeId, string DiagnosisTableNumber)
        {
            var diagnosis = _dRepository.Where(d => d.DiagnosisCode1 == codeId && d.DiagnosisTableNumber.Trim().Equals(DiagnosisTableNumber)).FirstOrDefault();
            return diagnosis ?? new DiagnosisCode();

        }

        /// <summary>
        /// Gets the new diagnosis by encounter identifier.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public DiagnosisCustomModel GetNewDiagnosisByEncounterId(int encounterId, int patientId)
        {
            //var m = new Diagnosis();
            var vm = new DiagnosisCustomModel();// _mapper.Map<DiagnosisCustomModel>(m);

            return vm;
        }

        /// <summary>
        /// Gets the diagnosis type by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public string GetDiagnosisTypeById(int id)
        {
            switch (id)
            {
                case 1:
                    return DiagnosisType.Primary.ToString() + " Diagnosis";
                case 2:
                    return DiagnosisType.Secondary.ToString() + " Diagnosis";
                case 3:
                    return "DRG";
                case 4:
                    return "Major CPT";
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Gets the diagnosis by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public Diagnosis GetDiagnosisById(string id)
        {
            var currentId = Convert.ToInt32(id);
            var m = _repository.Where(d => d.DiagnosisID == currentId).FirstOrDefault();
            return m;

        }

        /// <summary>
        /// Checks if any diagnosis exists.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        public int CheckIfAnyDiagnosisExists(int encounterId)
        {
            var m = GetDiagnosisInfoByEncounterId(encounterId);
            return m != null ? m.DiagnosisID : 0;
        }

        /// <summary>
        /// Gets the diagnosis information by encounter identifier.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        public Diagnosis GetDiagnosisInfoByEncounterId(int encounterId)
        {
            var pDiagnosis = Convert.ToInt32(DiagnosisType.Primary);
            var m = _repository.Where(d => d.EncounterID == encounterId && d.DiagnosisType == pDiagnosis).FirstOrDefault();
            return m;
        }

        /// <summary>
        /// Gets the diagnosis list by encounter identifier.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        public List<DiagnosisCustomModel> GetDiagnosisListByEncounterId(int encounterId)
        {
            var list = _repository.Where(d => d.PatientID != null && (int)d.EncounterID == encounterId).OrderBy(ss => ss.DiagnosisType).ToList();
            return list.Select(x => _mapper.Map<DiagnosisCustomModel>(x)).OrderByDescending(x => x.CreatedDate).ToList();
        }

        /// <summary>
        /// Checks if duplicate diagnosis against current encounter.
        /// </summary>
        /// <param name="diagnosisCode">The diagnosis code.</param>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <param name="diagnosisId">The diagnosis identifier.</param>
        /// <returns></returns>
        public bool CheckIfDuplicateDiagnosisAgainstCurrentEncounter(string diagnosisCode, int encounterId, int diagnosisId)
        {
            var result = diagnosisId == 0
                    ? _repository.Where(d => d.EncounterID == encounterId && d.DiagnosisCode.Equals(diagnosisCode) && d.IsDeleted == false).Any()
                    : _repository.Where(d => d.DiagnosisID != diagnosisId && d.EncounterID == encounterId && d.DiagnosisCode.Equals(diagnosisCode) && d.IsDeleted == false).Any();
            return result;
        }


        /// <summary>
        /// Gets the DRG diagnosis information by encounter identifier.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        public Diagnosis GetDRGDiagnosisInfoByEncounterId(int encounterId)
        {
            var drgDiagnosis = Convert.ToInt32(DiagnosisType.DRG);
            var diagnosis = _repository.Where(d => d.EncounterID == encounterId && d.DiagnosisType == drgDiagnosis).FirstOrDefault();
            return diagnosis;

        }

        public IEnumerable<DiagnosisCode> ExportFilteredDiagnosisCodes(string text, string tableNumber)
        {
            if (!string.IsNullOrEmpty(text))
                text = text.ToLower();

            var searchString = text.Split(' ');

            var list = _dRepository.Where(s => s.DiagnosisTableNumber.Trim().Equals(tableNumber)).ToList();

            var nList = list.Where(s => (s.DiagnosisCode1.ToLower().Contains(text) || s.DiagnosisFullDescription.ToLower().ContainsAny(searchString) || s.DiagnosisMediumDescription.ToLower().ContainsAny(searchString) || s.ShortDescription.ToLower().ContainsAny(searchString))).OrderBy(m => m.DiagnosisFullDescription).ToList();

            return nList;

        }


        public List<DiagnosisCode> GetFilteredDiagnosis(List<string> codeList, string DiagnosisTableNumber)
        {
            var list = _dRepository.Where(s => (s.DiagnosisTableNumber.Trim().Equals(DiagnosisTableNumber) && codeList.Contains(s.DiagnosisCode1))).ToList();
            return list;
        }

        public DiagnosisTabData GetDiagnosisTabData(long pId, long eId = 0, long physicianId = 0, string diagnosisTn = "", string drgTn = "")
        {
            var vm = new DiagnosisTabData();
            var sqlParameters = new SqlParameter[5];
            sqlParameters[0] = new SqlParameter("PId", pId);
            sqlParameters[1] = new SqlParameter("EId", eId);
            sqlParameters[2] = new SqlParameter("PhysicianId", physicianId);
            sqlParameters[3] = new SqlParameter("DiagnosisTN", diagnosisTn);
            sqlParameters[4] = new SqlParameter("DrgTN", drgTn);

            using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetDiagnosisTabData.ToString(), parameters: sqlParameters, isCompiled: false))
            {
                vm.CurrentDiagnosisList = r.GetResultWithJson<DiagnosisCustomModel>(JsonResultsArray.Diagnosis.ToString());
                vm.PreviousDiagnosisList = r.GetResultWithJson<DiagnosisCustomModel>(JsonResultsArray.Diagnosis.ToString());
                vm.FavOrdersList = r.GetResultWithJson<FavoritesCustomModel>(JsonResultsArray.FavoriteDiagnosis.ToString());
                return vm;
            }
        }

        public IEnumerable<DiagnosisCustomModel> GetCurrentDiagnosisData(long pId, long eid)
        {
            var sqlParameters = new SqlParameter[4];
            sqlParameters[0] = new SqlParameter("pPId", pId);
            sqlParameters[1] = new SqlParameter("pEId", eid);
            sqlParameters[2] = new SqlParameter("pEncounterNumber", DBNull.Value);
            sqlParameters[3] = new SqlParameter("pDRGTN", string.Empty);

            using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetCurrentDiagnosisData.ToString(), parameters: sqlParameters, isCompiled: false))
                return r.GetResultWithJson<DiagnosisCustomModel>(JsonResultsArray.Diagnosis.ToString());
        }

        public DiagnosisTabData DeleteCurrentDiagnosis(long userId, long id, string drgTn)
        {
            var result = new DiagnosisTabData { ExecutionStatus = 0 };
            var sqlParameters = new SqlParameter[3];
            sqlParameters[0] = new SqlParameter("pPId", userId);
            sqlParameters[1] = new SqlParameter("pId", id);
            sqlParameters[2] = new SqlParameter("pDRGTN", drgTn);

            using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SprocDeleteDiagnosisById.ToString(), parameters: sqlParameters, isCompiled: false))
            {
                result.ExecutionStatus = r.ResultSetFor<int>().FirstOrDefault();
                if (result.ExecutionStatus > 0)
                {
                    result.PrimaryExists = r.ResultSetFor<bool>().FirstOrDefault();
                    result.MajorCPTExists = r.ResultSetFor<bool>().FirstOrDefault();
                    result.MajorDRGExists = r.ResultSetFor<bool>().FirstOrDefault();

                    result.CurrentDiagnosisList = r.GetResultWithJson<DiagnosisCustomModel>(JsonResultsArray.Diagnosis.ToString());
                }
            }
            return result;
        }
    }
}
