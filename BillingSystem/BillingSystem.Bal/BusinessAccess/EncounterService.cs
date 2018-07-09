using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using BillingSystem.Model;
using BillingSystem.Common.Common;
using BillingSystem.Model.CustomModel;
using System.Threading.Tasks;
using System.Data.SqlClient;
using AutoMapper;
using BillingSystem.Bal.Interfaces;


namespace BillingSystem.Bal.BusinessAccess
{
    public class EncounterService : IEncounterService
    {
        private readonly IRepository<Encounter> _repository;
        private readonly BillingEntities _context;
        private readonly IMapper _mapper;

        public EncounterService(IRepository<Encounter> repository, BillingEntities context, IMapper mapper)
        {
            _repository = repository;
            _context = context;
            _mapper = mapper;
        }

        private void UpdateBillDateOnEncounterEnds(int encounterId)
        {
            var sqlParameters = new SqlParameter[1];
            sqlParameters[0] = new SqlParameter("pEId", encounterId);
            _repository.ExecuteCommand(StoredProcedures.SPROC_UpdateBillDate.ToString(), sqlParameters);
        }

        /// <summary>
        /// Checks the is long term service.
        /// </summary>
        /// <param name="isDrgDone">if set to <c>true</c> [is DRG done].</param>
        /// <param name="bedservice">The bedservice.</param>
        /// <returns></returns>
        private bool CheckIsLongTermService(bool isDrgDone, string bedservice)
        {
            var bedserviceTrimed = string.IsNullOrEmpty(bedservice) ? "" : bedservice.Trim();
            switch (bedserviceTrimed)
            {
                case "17-12":
                    return true;
                case "17-13":
                    return true;
                case "17-14":
                    return true;
                case "17-15":
                    return true;
                case "17-16":
                    return true;
                default:
                    return isDrgDone;
            }
        }

        private InsuranceCompany GetInsuranceDetailsByPayorId(string payorId)
        {
            var m = _context.InsuranceCompany.Where(x => x.InsuranceCompanyLicenseNumber.Equals(payorId)).FirstOrDefault();
            return m;
        }

        /// <summary>
        /// Calculates the person age.
        /// </summary>
        /// <param name="birthDate">The birth date.</param>
        /// <param name="now">The now.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        private int CalculatePersonAge(DateTime? birthDate, DateTime now, int facilityId)
        {
            var currentDateTime = GetInvariantCultureDateTime(facilityId);
            var birthdateUpdated = birthDate ?? currentDateTime;
            int age = now.Year - birthdateUpdated.Year;
            if (now.Month < birthdateUpdated.Month || (now.Month == birthdateUpdated.Month && now.Day < birthdateUpdated.Day)) age--;
            return age;
        }

        private List<AuthorizationCustomModel> GetAuthorizationsByEncounterId(string authorizationEncounterId)
        {
            var list = new List<AuthorizationCustomModel>();
            var mList = _context.Authorization.Where(x => x.EncounterID == authorizationEncounterId).ToList();
            list = mList.Select(x => _mapper.Map<AuthorizationCustomModel>(x)).ToList();
            return list;
        }

        /// <summary>
        /// Determines whether [is bed assigned long term case] [the specified bed service code].
        /// </summary>
        /// <param name="bedServiceCode">The bed service code.</param>
        /// <returns></returns>
        private bool IsBedAssignedLongTermCase(string bedServiceCode)
        {
            /*Implemented a NULL check on 06102015*/
            if (string.IsNullOrEmpty(bedServiceCode))
                return false;

            switch (bedServiceCode.Trim())
            {
                case "17-13":
                    return true;
                case "17-14":
                    return true;
                case "17-15":
                    return true;
                case "17-16":
                    return true;
                default:
                    return false;
            }
        }

        private string GetNameByGlobalCodeValue(string codeValue, string categoryValue, string fId = "")
        {
            if (!string.IsNullOrEmpty(codeValue))
            {
                var gl = _context.GlobalCodes.Where(g => g.GlobalCodeValue.Equals(codeValue) && !g.IsDeleted.Value && g.GlobalCodeCategoryValue.Equals(categoryValue) && (string.IsNullOrEmpty(fId) || g.FacilityNumber.Equals(fId))).FirstOrDefault();
                return gl != null ? gl.GlobalCodeName : string.Empty;
            }
            return string.Empty;
        }

        private string GetCodeDescription(string orderCode, string orderType, string CptTableNumber, string DrgTableNumber, string HcpcsTableNumber, string DrugTableNumber,
          string ServiceCodeTableNumber, string DiagnosisTableNumber)
        {
            var codeDescription = string.Empty;

            if (!string.IsNullOrEmpty(orderCode) && !string.IsNullOrEmpty(orderType))
            {
                var codeType = (OrderType)Enum.Parse(typeof(OrderType), orderType);
                switch (codeType)
                {
                    case OrderType.CPT:
                        codeDescription = _context.CPTCodes.Where(x => x.CodeNumbering.Contains(orderCode) && x.CodeTableNumber.Trim().Equals(CptTableNumber)).FirstOrDefault().CodeDescription;
                        return codeDescription;
                    case OrderType.DRG:
                        codeDescription = _context.DRGCodes.Where(d => d.CodeNumbering == orderCode && d.CodeTableNumber.Trim().Equals(DrgTableNumber)).FirstOrDefault().CodeDescription;
                        return codeDescription;
                    case OrderType.HCPCS:
                        codeDescription = _context.HCPCSCodes.Where(x => x.CodeNumbering == orderCode && x.CodeTableNumber.Trim().Equals(HcpcsTableNumber)).FirstOrDefault().CodeDescription;
                        return codeDescription;
                    case OrderType.DRUG:
                        codeDescription = _context.Drug.Where(x => x.DrugCode == orderCode && x.DrugTableNumber.Trim().Equals(DrugTableNumber)).FirstOrDefault().DrugDescription;
                        return codeDescription;
                    case OrderType.BedCharges:
                    case OrderType.ServiceCode:
                        codeDescription = _context.ServiceCode.Where(s => s.ServiceCodeValue.Equals(orderCode) && s.ServiceCodeTableNumber.Trim().Equals(ServiceCodeTableNumber)).FirstOrDefault().ServiceCodeDescription;
                        return codeDescription;
                    case OrderType.Diagnosis:
                        codeDescription = _context.DiagnosisCode.Where(d => d.DiagnosisCode1 == orderCode && d.DiagnosisTableNumber.Trim().Equals(DiagnosisTableNumber)).FirstOrDefault().DiagnosisFullDescription;
                        return codeDescription;
                    //codeDescription = _sRepository.Where(s => s.ServiceCodeValue.Equals(orderCode) && s.ServiceCodeTableNumber.Trim().Equals(ServiceCodeTableNumber)).FirstOrDefault().ServiceCodeDescription;
                    //return codeDescription;
                    default:
                        break;
                }
            }
            return codeDescription;
        }

        private string GetNameByGlobalCodeValueAndCategoryValue(string categoryValue, string globalCodeValue)
        {
            if (!string.IsNullOrEmpty(categoryValue) && !string.IsNullOrEmpty(globalCodeValue))
            {
                var globalCode = _context.GlobalCodes.Where(c => c.GlobalCodeCategoryValue.Equals(categoryValue)
                && c.GlobalCodeValue.Equals(globalCodeValue)).FirstOrDefault();

                if (globalCode != null)
                    return globalCode.GlobalCodeName;
            }
            return string.Empty;
        }

        private string GetBedNameByInPatientEncounterId(string encounterId)
        {
            var bedName = string.Empty;
            var bedTypeAssigned = _context.MappingPatientBed.Where(m => m.EncounterID.Equals(encounterId) && m.EndDate == null).OrderByDescending(x => x.PatientID).FirstOrDefault();
            if (bedTypeAssigned != null)
            {
                var bedId = Convert.ToInt32(bedTypeAssigned.BedNumber);

                var bedMaster = _context.UBedMaster.Where(b => b.BedId == bedId).FirstOrDefault();
                if (bedMaster != null)
                {
                    var fsId = Convert.ToInt32(bedMaster.FacilityStructureId);

                    var fsStructure = _context.FacilityStructure.Where(f => f.FacilityStructureId == fsId).FirstOrDefault();
                    if (fsStructure != null)
                        bedName = fsStructure.FacilityStructureName;

                }
            }
            return bedName;
        }

        private string GetBedByInPatientEncounterId(string encounterId)
        {
            var bedmaster = "0";
            var bedTypeAssigned = _context.MappingPatientBed.Where(m => m.EncounterID.Equals(encounterId) && m.EndDate == null).FirstOrDefault();
            if (bedTypeAssigned != null)
            {
                var bedId = Convert.ToInt32(bedTypeAssigned.BedNumber);

                var bedMaster = _context.UBedMaster.Where(b => b.BedId == bedId).FirstOrDefault();
                if (bedMaster != null)
                {
                    bedmaster = bedMaster.BedId.ToString();
                }

            }
            return bedmaster;
        }

        private Authorization GetAuthorizationByEncounterId(string authorizationEncounterId)
        {
            var m = _context.Authorization.Where(x => x.EncounterID == authorizationEncounterId && (x.IsDeleted == null || x.IsDeleted == false)).FirstOrDefault();
            return m;
        }

        private string GetAuthorizationIdPayerByPatientId(int patientId)
        {
            var ins = _context.PatientInsurance.Where(e => e.PatientID == patientId).FirstOrDefault();
            return ins != null ? ins.InsuranceCompanyId.ToString() : string.Empty;
        }

        /// <summary>
        /// Gets the encounter list.
        /// </summary>
        /// <returns></returns>
        private List<Encounter> GetEncounterList()
        {
            var lst = _repository.GetAll().Include(f => f.PatientInfo).Where(x => x.EncounterEndTime == null).ToList();
            return lst;
        }

        private string GetOverRideBedTypeByInPatientEncounterId(string encounterId)
        {
            var str = string.Empty;
            var bedTypeAssigned = _context.MappingPatientBed.Where(m => m.EncounterID.Equals(encounterId)).FirstOrDefault();
            if (bedTypeAssigned != null)
            {
                str = Convert.ToInt32(bedTypeAssigned.OverrideBedType).ToString();
            }
            return str;
        }

        /// <summary>
        /// Encounters the state.
        /// </summary>
        /// <param name="endType">The end type.</param>
        /// <param name="patientType">Type of the patient.</param>
        /// <returns></returns>
        private string EncounterState(int endType, int patientType)
        {
            var encounterState = string.Empty;
            switch (patientType)
            {
                case 1:
                case 3:
                    encounterState = (endType > 0) ? EncounterStates.endencounter.ToString() : EncounterStates.outpatient.ToString();
                    break;
                case 2:
                    encounterState = (endType > 0) ? EncounterStates.discharge.ToString() : EncounterStates.admitpatient.ToString();
                    break;
            }
            return encounterState;
        }

        private string GetGlobalCodeNameByValueAndCategoryId(string categoryId, string globalCodeVal)
        {
            var globalCode = _context.GlobalCodes.Where(c => c.GlobalCodeCategoryValue.Equals(categoryId) && c.GlobalCodeValue == globalCodeVal).FirstOrDefault();
            if (globalCode != null)
                return globalCode.GlobalCodeName;
            return string.Empty;
        }

        private int GetGlobalCodeSotringByValueAndCategoryId(string categoryId, string globalCodeVal)
        {
            var globalCode = _context.GlobalCodes.Where(c => c.GlobalCodeCategoryValue.Equals(categoryId) && c.GlobalCodeValue == globalCodeVal).FirstOrDefault();
            if (globalCode != null)
                return Convert.ToInt32(globalCode.SortOrder);

            return 0;
        }

        private DRGCodes GetDrgCodesById(int id)
        {
            var drgCodes = _context.DRGCodes.Where(x => x.DRGCodesId == id).FirstOrDefault();
            return drgCodes;
        }

        private Diagnosis GetDiagnosisInfoByEncounterId(int encounterId)
        {
            var pDiagnosis = Convert.ToInt32(DiagnosisType.Primary);
            var m = _context.Diagnosis.Where(d => d.EncounterID == encounterId && d.DiagnosisType == pDiagnosis).FirstOrDefault();
            return m;
        }

        private Diagnosis GetDRGDiagnosisInfoByEncounterId(int encounterId)
        {
            var drgDiagnosis = Convert.ToInt32(DiagnosisType.DRG);
            var diagnosis = _context.Diagnosis.Where(d => d.EncounterID == encounterId && d.DiagnosisType == drgDiagnosis).FirstOrDefault();
            return diagnosis;

        }

        private string GetDateDifference(DateTime startTime, int patientType, int facilityId)
        {
            DateTime currentDate = GetInvariantCultureDateTime(facilityId);
            TimeSpan ts = currentDate - Convert.ToDateTime(startTime);

            var diff = patientType == (int)EncounterPatientType.OutPatient
                 ? string.Format("{0} Hours", ts.Hours)
                 : string.Format("{0} Days {1} Hours", ts.Days, ts.Hours);
            return diff;
        }

        public DateTime GetInvariantCultureDateTime(int facilityid)
        {
            var m = facilityid > 0 ? _context.Facility.Where(f => f.FacilityId == facilityid).FirstOrDefault() : new Facility { FacilityTimeZone = string.Empty };

            var timeZone = m != null && !string.IsNullOrEmpty(m.FacilityTimeZone) ? m.FacilityTimeZone : TimeZoneInfo.Utc.ToString();

            var tzi = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            var utcTime = DateTime.Now.ToUniversalTime();
            var convertedTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);
            return convertedTime;
        }

        public string GetPhysicianName(int physicianId)
        {
            var phylist = _context.Physician.Where(x => x.Id == physicianId).FirstOrDefault();
            return phylist != null ? phylist.PhysicianName : string.Empty;
        }

        //Function to Get Encounter BY Encounter ID
        /// <summary>
        /// Gets the encounter by encounter identifier.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        public Encounter GetEncounterByEncounterId(int encounterId)
        {
            var m = _repository.Where(x => x.EncounterID == encounterId).FirstOrDefault();
            return m;

        }

        //Function to get Encounter List By Patient ID
        /// <summary>
        /// Gets the encounter list by patient identifier.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public IEnumerable<EncounterCustomModel> GetEncounterListByPatientId(int patientId)
        {
            var list = new List<EncounterCustomModel>();

            var lstEncounter = _repository.Where(_ => _.PatientID == patientId).OrderByDescending(x => x.EncounterStartTime).ToList();
            var encountertypeCat = Convert.ToInt32(GlobalCodeCategoryValue.EncounterType);
            var encounterPatienttypeCat = Convert.ToInt32(GlobalCodeCategoryValue.EncounterPatientType);
            list.AddRange(lstEncounter.Select(item => new EncounterCustomModel
            {
                EncounterID = item.EncounterID,
                EncounterNumber = item.EncounterNumber,
                EncounterStartTime = item.EncounterStartTime,
                EncounterEndTime = item.EncounterEndTime,
                Charges = item.Charges,
                Payment = item.Payment,
                PatientID = item.PatientID,
                EncounterTypeName = GetNameByGlobalCodeValueAndCategoryValue(Convert.ToString(encountertypeCat),
                    item.EncounterType.ToString()),
                EncounterPatientTypeName = GetNameByGlobalCodeValueAndCategoryValue(Convert.ToString(encounterPatienttypeCat),
                    item.EncounterPatientType.ToString()),
            }));

            return list;
        }

        /// <summary>
        /// Adds the update encounter.
        /// </summary>
        /// <param name="e">The encounter.</param>
        /// <returns></returns>
        public int AddUpdateEncounter(Encounter e)
        {
            int result;
            var eEndType = e.EncounterEndType.HasValue
                               ? e.EncounterEndType.Value
                               : !string.IsNullOrEmpty(e.EncounterDischargePlan)
                                     ? Convert.ToInt32(e.EncounterDischargePlan)
                                     : 0;
            if (e.EncounterID > 0)
            {
                var model = _repository.Where(e1 => e1.EncounterID == e.EncounterID).FirstOrDefault();
                if (model != null)
                {
                    e.CreatedBy = model.CreatedBy;
                    e.CreatedDate = model.CreatedDate;
                }
                result = _repository.UpdateEntity(e, e.EncounterID) != null ? e.EncounterID : 0;

                /*
                 * Added by Amit Jain 
                 * Purpose: To update Bill Date with the Encounter End Date when Encounter Ends.
                 */
                if (eEndType > 0)
                    UpdateBillDateOnEncounterEnds(e.EncounterID);
            }
            else
                result = _repository.Create(e) != null ? e.EncounterID : 0;
            return result;
        }

        /// <summary>
        /// Checks the encounter number exist.
        /// </summary>
        /// <param name="uniqueNumber">The unique number.</param>
        /// <returns></returns>
        public bool CheckEncounterNumberExist(string uniqueNumber)
        {
            return _repository.Where(x => x.EncounterNumber.Contains(uniqueNumber)).Any();

        }

        /// <summary>
        /// Gets the encounter detail by patient identifier.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public EncounterCustomModel GetEncounterDetailByPatientId(int patientId)
        {
            EncounterCustomModel vm = new EncounterCustomModel();
            var encounterState = string.Empty;
            var encounter = _repository.Where(x => x.PatientID == patientId).Include(p => p.PatientInfo).OrderByDescending(c => c.EncounterID).FirstOrDefault();
            if (encounter != null)
            {
                var currentDateTime = GetInvariantCultureDateTime(Convert.ToInt32(encounter.EncounterFacility));
                if (encounter.EncounterEndTime == null)
                {
                    var patientName = string.Empty;
                    if (encounter.PatientInfo != null)
                        patientName = string.Format("{0} {1}", encounter.PatientInfo.PersonFirstName, encounter.PatientInfo.PersonLastName);

                    if (encounter.EncounterPatientType != null)
                    {
                        var endType = encounter.EncounterEndType != null ? Convert.ToInt32(encounter.EncounterEndType) : 0;
                        var patientType = encounter.EncounterPatientType != null ? Convert.ToInt32(encounter.EncounterPatientType) : 0;
                        encounterState = EncounterState(endType, patientType);
                    }

                    DateTime? dob = null;
                    if (encounter.PatientInfo != null && encounter.PatientInfo.PersonBirthDate.HasValue)
                        dob = encounter.PatientInfo.PersonBirthDate.Value;

                    vm = new EncounterCustomModel
                    {
                        Charges = encounter.Charges,
                        EncounterAccidentDate = encounter.EncounterAccidentDate,
                        EncounterAccidentRelated = encounter.EncounterAccidentRelated,
                        EncounterAccidentType = encounter.EncounterAccidentType,
                        EncounterAccomodation = encounter.EncounterAccomodation,
                        EncounterAccomodationReason = encounter.EncounterAccomodationReason,
                        EncounterAccomodationRequest = encounter.EncounterAccomodationRequest,
                        EncounterAcuteCareFacilitynonUAE = encounter.EncounterAcuteCareFacilitynonUAE,
                        EncounterAdmitReason = encounter.EncounterAdmitReason,
                        EncounterAdmitType = encounter.EncounterAdmitType,
                        EncounterAmbulatoryCondition = encounter.EncounterAmbulatoryCondition,
                        EncounterComment = encounter.EncounterComment,
                        EncounterConfidentialityLevel = encounter.EncounterConfidentialityLevel,
                        EncounterDeceasedDate = encounter.EncounterDeceasedDate,
                        PatientID = encounter.PatientID,
                        PatientName = patientName,
                        PersonEmiratesIDNumber = encounter.PatientInfo != null ? encounter.PatientInfo.PersonEmiratesIDNumber : string.Empty,
                        EncounterPatientType = encounter.EncounterPatientType,
                        EncounterModeofArrival = encounter.EncounterModeofArrival,
                        EncounterServiceCategory = encounter.EncounterServiceCategory,
                        EncounterID = encounter.EncounterID,
                        EncounterStartType = encounter.EncounterStartType,
                        EncounterType = encounter.EncounterType,
                        EncounterSpecialty = encounter.EncounterSpecialty,
                        EncounterInpatientAdmitDate = encounter.EncounterInpatientAdmitDate,
                        //EncounterEndTime = encounter.EncounterEndTime,
                        EncounterEndType = encounter.EncounterEndType,
                        EncounterStartTime = encounter.EncounterStartTime,
                        EncounterTransferHospital = encounter.EncounterTransferHospital,
                        EncounterTransferSource = encounter.EncounterTransferSource,
                        EncounterFacility = encounter.EncounterFacility,
                        EncounterFacilityID = encounter.EncounterFacilityID,
                        EncounterState = encounterState,
                        EncounterEndTime = currentDateTime,
                        BedName = GetBedNameByInPatientEncounterId(Convert.ToString(encounter.EncounterID)),
                        BedId = GetBedByInPatientEncounterId(Convert.ToString(encounter.EncounterID)),
                        EncounterAttendingPhysician = encounter.EncounterAttendingPhysician,
                        EncounterPhysicianType = encounter.EncounterPhysicianType,
                        Age = CalculatePersonAge(dob, currentDateTime, Convert.ToInt32(encounter.EncounterFacility)),
                        OverrideBedType = GetOverRideBedTypeByInPatientEncounterId(Convert.ToString(encounter.EncounterID))
                    };
                }
                else
                {
                    var patientInfo = _context.PatientInfo.Where(p => p.PatientID == patientId).FirstOrDefault();
                    if (patientInfo != null)
                    {
                        var patientName = string.Format("{0} {1}", patientInfo.PersonFirstName, patientInfo.PersonLastName);

                        vm = new EncounterCustomModel
                        {
                            PatientName = patientName,
                            PersonEmiratesIDNumber = patientInfo.PersonEmiratesIDNumber,
                            PersonMedicalRecordNumber = patientInfo.PersonMedicalRecordNumber,
                            PatientID = patientInfo.PatientID,
                            EncounterInpatientAdmitDate = currentDateTime,
                            EncounterStartTime = currentDateTime
                        };
                    }

                }
            }
            return vm;
        }

        /// <summary>
        /// Determines whether [is patient exist] [the specified patient identifier].
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public bool IsPatientExist(int patientId)
        {
            return _context.PatientInfo.Where(p => p.PatientID == patientId).Any();
        }

        public string GetInsuranceMemberIdByPatientId(int patientId)
        {
            var ins = _context.PatientInsurance.Where(e => e.PatientID == patientId).FirstOrDefault();
            return ins != null ? ins.PersonHealthCareNumber.ToString() : string.Empty;

        }

        /// <summary>
        /// Gets the encounter detail by identifier.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        public Encounter GetEncounterDetailById(Int32 encounterId)
        {
            var m = _repository.Where(_ => _.EncounterID == encounterId).SingleOrDefault();
            return m;

        }

        /// <summary>
        /// Gets the encounter state by patient identifier.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public Encounter GetEncounterStateByPatientId(int patientId)
        {
            var m = _repository.GetAll().Include(x => x.PatientInfo).ToList().Where(e => e.PatientID == patientId).OrderByDescending(x => x.EncounterStartTime).FirstOrDefault();
            return m;

        }

        /// <summary>
        /// Gets the encounter by patient identifier and active.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public Encounter GetEncounterByPatientIdAndActive(int patientId)
        {
            var lst = _repository.GetAll().Include(x => x.PatientInfo).ToList().Where(e => e.PatientID == patientId).ToList();
            var m = lst.Any(x => x.EncounterEndTime == null) ? lst.FirstOrDefault(x => x.EncounterEndTime == null) :
                lst.OrderByDescending(x => x.EncounterStartTime).FirstOrDefault();
            return m;
        }

        /// <summary>
        /// Gets the active encounters.
        /// </summary>
        /// <param name="common">The common.</param>
        /// <returns></returns>
        public List<EncounterCustomModel> GetActiveEncounters(CommonModel common, string DiagnosisTableNumber, string DrgTableNumber)
        {
            var list = new List<EncounterCustomModel>();
            var encountersList = GetEncounterList();
            encountersList = encountersList.Where(x => x.EncounterPatientType == common.EncounterPatientType && x.EncounterEndTime == null).OrderByDescending(m => m.EncounterStartTime).ToList();

            /*
            * Owner: Amit Jain
            * On: 20102014
            * Purpose: list should be according to the logged-in user's default facility
            */
            //Additions start here
            if (!string.IsNullOrEmpty(common.FacilityNumber))
                encountersList = encountersList.Where(l => l.EncounterFacility.Equals(common.FacilityNumber)).ToList();
            //Additions end here

            foreach (var item in encountersList)
            {
                var enId = Convert.ToString(item.EncounterID);
                var bedInfo = new EncounterCustomModel();

                //var bedName = bedMasterBal.GetBedNameByInPatientEncounterId(enId);
                if (item.EncounterPatientType == 2)
                {
                    var spName = string.Format("EXEC {0} @pPatientID", StoredProcedures.SPROC_GetPatientBedInformation);
                    var sqlParameters = new SqlParameter[1];
                    sqlParameters[0] = new SqlParameter("pPatientID", Convert.ToInt32(item.PatientID));
                    var result1 = _context.Database.SqlQuery<EncounterCustomModel>(spName, sqlParameters);
                    bedInfo = result1.FirstOrDefault();
                }
                //bedInfo = _mpRepository.GetPatientBedInformation(Convert.ToInt32(item.PatientID));


                //Check if current ENCOUNTER's primary diagnosis exists.
                bool isPDiagnosisDone;
                bool isDrgDone = false;
                string primaryDiagnosis;
                var expectedLengthofStay = "NA";
                var result = GetDiagnosisInfoByEncounterId(item.EncounterID);
                isPDiagnosisDone = result != null && result.DiagnosisID > 0;
                primaryDiagnosis = result != null ? result.DiagnosisCodeDescription : string.Empty;
                var drgObj = GetDRGDiagnosisInfoByEncounterId(item.EncounterID);
                if (drgObj != null)
                {
                    isDrgDone = true;
                    var drgcodeObj = GetDrgCodesById(Convert.ToInt32(drgObj.DRGCodeID));
                    expectedLengthofStay = drgcodeObj != null
                        ? drgcodeObj.Alos != null ? Convert.ToString(Math.Round(Convert.ToDecimal(drgcodeObj.Alos), 2, MidpointRounding.AwayFromZero)) : "NA"
                        : "NA";
                    expectedLengthofStay = IsBedAssignedLongTermCase(bedInfo.patientBedService)
                        ? "NA"
                        : expectedLengthofStay;
                }
                var encounterAuhtorized = GetAuthorizationByEncounterId(enId);
                var isEncounterauhtorized = encounterAuhtorized != null;

                list.Add(new EncounterCustomModel
                {
                    FirstName = item.PatientInfo.PersonFirstName,
                    LastName = item.PatientInfo.PersonLastName,
                    BirthDate = item.PatientInfo.PersonBirthDate,
                    PersonEmiratesIDNumber = item.PatientInfo.PersonEmiratesIDNumber,
                    EncounterNumber = item.EncounterNumber,
                    EncounterStartTime = item.EncounterStartTime,
                    EncounterID = item.EncounterID,
                    PatientID = item.PatientID,
                    PatientIsVIP = item.PatientInfo != null ? item.PatientInfo.PersonVIP : string.Empty,
                    IsPrimaryDiagnosisDone = isPDiagnosisDone,
                    IsEncounterAuthorized = isEncounterauhtorized,
                    PrimaryDiagnosisDescription = primaryDiagnosis,
                    patientBedService = bedInfo.patientBedService,
                    Room = bedInfo.Room,
                    DepartmentName = bedInfo.DepartmentName,
                    BedAssignedOn = bedInfo.BedAssignedOn,
                    BedRateApplicable = bedInfo.BedRateApplicable,
                    FloorName = bedInfo.FloorName,
                    BedName = bedInfo.BedName,
                    IsDRGExist = CheckIsLongTermService(isDrgDone, bedInfo.patientBedService),
                    AverageLengthofStay = CalulateAverageLengthOfStay(Convert.ToDateTime(item.EncounterStartTime), Convert.ToInt32(item.EncounterFacility)),
                    ExpectedLengthofStay = expectedLengthofStay,
                    PhysicianName = GetPhysicianName(Convert.ToInt32(item.EncounterAttendingPhysician)),
                    WaitingTime = GetDateDifference(Convert.ToDateTime(item.EncounterStartTime), Convert.ToInt32(item.EncounterPatientType), Convert.ToInt32(item.EncounterFacility)),
                    TriageValue = GetGlobalCodeNameByValueAndCategoryId("4952", item.Triage),
                    PatientStageName = GetGlobalCodeNameByValueAndCategoryId("4951", item.PatientState),
                    TriageSortingValue = GetGlobalCodeSotringByValueAndCategoryId("4952", item.Triage),
                });
                foreach (var itemobj in list)
                {
                    itemobj.ActualMoreThanExpected = itemobj.AverageLengthofStay != "NA" &&
                                                     itemobj.ExpectedLengthofStay != "NA" && Convert.ToDecimal(itemobj.AverageLengthofStay) >
                                                     Convert.ToDecimal(itemobj.ExpectedLengthofStay);
                }
            }
            return list;
        }

        public List<int> GetActiveEncounterIdsByPatientId(int patientId)
        {
            var list = new List<int>();
            list =
                _repository.Where(e => (int)e.PatientID == patientId && e.EncounterEndTime == null)
                    .OrderByDescending(d => d.EncounterStartTime)
                    .Select(item => item.EncounterID)
                    .ToList();
            return list;
        }

        /// <summary>
        /// Gets the encounter detail by encounter identifier.
        /// </summary>
        /// <param name="encounterid">The encounterid.</param>
        /// <returns></returns>
        public EncounterCustomModel GetEncounterDetailByEncounterID(int encounterid)
        {
            var vm = new EncounterCustomModel();
            var encounterState = string.Empty;
            var encounter = _repository.Where(x => x.EncounterID == encounterid).Include(p => p.PatientInfo).FirstOrDefault();
            if (encounter != null)
            {
                var currentDateTime = GetInvariantCultureDateTime(Convert.ToInt32(encounter.EncounterFacility));
                var patientName = string.Empty;
                if (encounter.PatientInfo != null)
                    patientName = string.Format("{0} {1}", encounter.PatientInfo.PersonFirstName, encounter.PatientInfo.PersonLastName);

                if (encounter.EncounterPatientType != null)
                {
                    var endType = encounter.EncounterEndType != null ? Convert.ToInt32(encounter.EncounterEndType) : 0;
                    var patientType = encounter.EncounterPatientType != null ? Convert.ToInt32(encounter.EncounterPatientType) : 0;
                    encounterState = EncounterState(endType, patientType);
                }
                var encountertypeCat = Convert.ToInt32(GlobalCodeCategoryValue.EncounterType);
                vm = new EncounterCustomModel();

                vm.Charges = encounter.Charges;
                vm.EncounterAccidentDate = encounter.EncounterAccidentDate;
                vm.EncounterAccidentRelated = encounter.EncounterAccidentRelated;
                vm.EncounterAccidentType = encounter.EncounterAccidentType;
                vm.EncounterAccomodation = encounter.EncounterAccomodation;
                vm.EncounterAccomodationReason = encounter.EncounterAccomodationReason;
                vm.EncounterAccomodationRequest = encounter.EncounterAccomodationRequest;
                vm.EncounterAcuteCareFacilitynonUAE = encounter.EncounterAcuteCareFacilitynonUAE;
                vm.EncounterAdmitReason = encounter.EncounterAdmitReason;
                vm.EncounterAdmitType = encounter.EncounterAdmitType;
                vm.EncounterAmbulatoryCondition = encounter.EncounterAmbulatoryCondition;
                vm.EncounterComment = encounter.EncounterComment;
                vm.EncounterConfidentialityLevel = encounter.EncounterConfidentialityLevel;
                vm.EncounterDeceasedDate = encounter.EncounterDeceasedDate;
                vm.PatientID = encounter.PatientID;
                vm.PatientName = patientName;
                vm.PersonEmiratesIDNumber = encounter.PatientInfo != null ? encounter.PatientInfo.PersonEmiratesIDNumber : string.Empty;
                vm.EncounterPatientType = encounter.EncounterPatientType;
                vm.EncounterModeofArrival = encounter.EncounterModeofArrival;
                vm.EncounterServiceCategory = encounter.EncounterServiceCategory;
                vm.EncounterID = encounter.EncounterID;
                vm.EncounterNumber = encounter.EncounterNumber;
                vm.EncounterStartType = encounter.EncounterStartType;
                vm.EncounterType = encounter.EncounterType;
                vm.EncounterSpecialty = encounter.EncounterSpecialty;
                vm.EncounterInpatientAdmitDate = encounter.EncounterInpatientAdmitDate;
                vm.EncounterEndType = encounter.EncounterEndType;
                vm.EncounterStartTime = encounter.EncounterStartTime;
                vm.EncounterTransferHospital = encounter.EncounterTransferHospital;
                vm.EncounterTransferSource = encounter.EncounterTransferSource;
                vm.EncounterFacility = encounter.EncounterFacility;
                vm.EncounterFacilityID = encounter.EncounterFacilityID;
                vm.EncounterState = encounterState;
                vm.EncounterEndTime = currentDateTime;
                vm.BedName = GetBedNameByInPatientEncounterId(Convert.ToString(encounter.EncounterID));
                vm.BedId = GetBedByInPatientEncounterId(Convert.ToString(encounter.EncounterID));
                vm.EncounterAttendingPhysician = encounter.EncounterAttendingPhysician;
                vm.EncounterPhysicianType = encounter.EncounterPhysicianType;
                vm.PersonMedicalRecordNumber = encounter.PatientInfo != null ? encounter.PatientInfo.PersonMedicalRecordNumber : string.Empty;
                vm.PatientInfo = encounter.PatientInfo;
                vm.EncounterPatientTypeName = GetNameByGlobalCodeValueAndCategoryValue(Convert.ToString((int)GlobalCodeCategoryValue.EncounterPatientType), encounter.EncounterPatientType.ToString());
                vm.EncounterSpecialityName = GetNameByGlobalCodeValueAndCategoryValue(Convert.ToInt32(GlobalCodeCategoryValue.EncounterSpecialty).ToString(), encounter.EncounterSpecialty.ToString());
                vm.EncounterModeOfArrivalName = GetNameByGlobalCodeValueAndCategoryValue(Convert.ToInt32(GlobalCodeCategoryValue.EncounterModeofArrival).ToString(), encounter.EncounterModeofArrival.ToString());
                vm.EncounterServiceCategoryName = GetNameByGlobalCodeValueAndCategoryValue(Convert.ToInt32(GlobalCodeCategoryValue.EncounterServiceCategory).ToString(), encounter.EncounterServiceCategory.ToString());
                vm.EncounterPhysicianTypeName = GetNameByGlobalCodeValueAndCategoryValue(Convert.ToInt32(GlobalCodeCategoryValue.EncounterPhysicianType).ToString(), encounter.EncounterPhysicianType.ToString());
                vm.EncounterPhysicianName = GetPhysicianName(Convert.ToInt32(encounter.EncounterAttendingPhysician));
                vm.EncounterAdmitTypeName = GetNameByGlobalCodeValueAndCategoryValue(Convert.ToInt32(GlobalCodeCategoryValue.EncounterAdmitType).ToString(), encounter.EncounterAdmitType.ToString());
                vm.Age = CalculatePersonAge(encounter.PatientInfo.PersonBirthDate, currentDateTime, Convert.ToInt32(encounter.EncounterFacility));
                vm.OverrideBedType = GetOverRideBedTypeByInPatientEncounterId(Convert.ToString(encounter.EncounterID));
                vm.EncounterTypeName = GetNameByGlobalCodeValueAndCategoryValue(Convert.ToString(encountertypeCat), encounter.EncounterType.ToString());
                vm.VirtuallyDischarge = encounter.EncounterDischargePlan != null ? "Discharge" : "";
                vm.VirtuallyDischargeOn = encounter.EncounterDischargePlan != null ? encounter.EncounterDischargeLocation : null;

                if (encounter != null && encounter.PatientInfo != null && !string.IsNullOrEmpty(encounter.PatientInfo.PersonInsuranceCompany))
                {
                    var ins = GetInsuranceDetailsByPayorId(encounter.PatientInfo.PersonInsuranceCompany);
                    if (ins != null)
                    {
                        vm.InsuranceCompanyName = ins.InsuranceCompanyName;
                        vm.InsuranceCompanyPhoneNumber = ins.InsuranceCompanyMainPhone;
                        vm.InsuranceCompanyFaxNumber = ins.InsuranceCompanyFax;
                        vm.InsuranceCompanyAddress = $"{ ins.InsuranceCompanyStreetAddress} { ins.InsuranceCompanyStreetAddress2}";
                    }
                }
                vm.EncounterAuthorizationList = GetAuthorizationsByEncounterId(vm.EncounterID.ToString());

                if (vm.EncounterAuthorizationList.Any(a => a.AuthorizationEnd.HasValue && a.AuthorizationEnd.Value >= currentDateTime.Date))
                {
                    vm.EncounterAuthorization = vm.EncounterAuthorizationList.Where(a => a.AuthorizationEnd.HasValue && a.AuthorizationEnd.Value >= currentDateTime.Date).FirstOrDefault();

                    if (vm.EncounterAuthorization != null)
                    {
                        vm.EncounterAuthorization.AuthorizationMemberID = GetInsuranceMemberIdByPatientId(Convert.ToInt32(vm.PatientID));
                        vm.EncounterAuthorization.AuthorizationIDPayer = GetAuthorizationIdPayerByPatientId(Convert.ToInt32(vm.PatientID));
                    }
                }
                else
                {
                    vm.EncounterAuthorization = new Authorization()
                    {
                        EncounterID = vm.EncounterID.ToString(),
                        PatientID = vm.PatientID,
                        AuthorizationStart = vm.EncounterStartTime,
                        AuthorizationMemberID = GetInsuranceMemberIdByPatientId(Convert.ToInt32(vm.PatientID)),
                        AuthorizationIDPayer = GetAuthorizationIdPayerByPatientId(Convert.ToInt32(vm.PatientID)),
                    };
                }
            }
            return vm;
        }

        /// <summary>
        /// Gets the active encounter by pateint identifier.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public Encounter GetActiveEncounterByPateintId(int patientId)
        {
            try
            {
                var lstEncounter = _repository.Where(_ => _.PatientID == patientId && _.EncounterEndTime == null)
                        .OrderByDescending(x => x.EncounterID).FirstOrDefault();
                return lstEncounter;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the XML active encounter by pateint identifier.
        /// </summary>
        /// <param name="PatientId">The patient identifier.</param>
        /// <returns></returns>
        public Encounter GetXMLActiveEncounterByPateintId(int PatientId)
        {
            var m = _repository.Where(_ => _.PatientID == PatientId)
                    .OrderByDescending(x => x.EncounterID)
                    .FirstOrDefault();
            return m;
        }

        //Function to Get Encounter number BY Encounter ID
        /// <summary>
        /// Gets the encounter number by encounter identifier.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        public string GetEncounterNumberByEncounterId(int encounterId)
        {
            var encounter = _repository.Where(x => x.EncounterID == encounterId).FirstOrDefault();
            return encounter != null ? encounter.EncounterNumber : string.Empty;

        }

        /// <summary>
        /// Gets the encounter chart data.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="displayType">The display type.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="tillDate">The till date.</param>
        /// <returns></returns>
        public IEnumerable<EncounterExtension> GetEncounterChartData(int facilityId, int displayType, DateTime fromDate, DateTime tillDate)
        {
            var list = new List<EncounterExtension>();
            var spName = string.Format("EXEC {0} @pFacilityID, @pDisplayTypeID, @pFromDate, @pTillDate", StoredProcedures.SPROC_GetDBEncounter);
            var sqlParameters = new SqlParameter[4];
            sqlParameters[0] = new SqlParameter(InputParams.pFacilityID.ToString(), facilityId);
            sqlParameters[1] = new SqlParameter(InputParams.pDisplayTypeID.ToString(), displayType);
            sqlParameters[2] = new SqlParameter(InputParams.pFromDate.ToString(), fromDate);
            sqlParameters[3] = new SqlParameter(InputParams.pTillDate.ToString(), tillDate);
            IEnumerable<EncounterExtension> result = _context.Database.SqlQuery<EncounterExtension>(spName, sqlParameters);

            var r = result.ToList();
            if (r != null && r.Count > 0)
            {
                list.AddRange(r.Select(item => new EncounterExtension
                {
                    Title = item.Title,
                    Code = item.Code,
                    XAxis = item.XAxis,
                    YAxis = item.YAxis
                }));
            }
            return list;
        }

        /// <summary>
        /// Gets the unclosed encounters.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <returns></returns>
        public List<EncounterCustomModel> GetUnclosedEncounters(int facilityId, int corporateId)
        {
            var spName = string.Format("EXEC {0} @pCorporateID, @pFacilityID", StoredProcedures.SPROC_GetJobClosedEncounterAndOrders);
            var sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter(InputParams.pCorporateID.ToString(), corporateId);
            sqlParameters[1] = new SqlParameter(InputParams.pFacilityID.ToString(), facilityId);
            IEnumerable<EncounterCustomModel> result = _context.Database.SqlQuery<EncounterCustomModel>(spName, sqlParameters);
            var list = result.ToList();
            if (list.Count > 0)
            {
                var globalCodeCategoryValue = Convert.ToString((int)GlobalCodeCategoryValue.EncounterPatientType);
                list = list.Select(item =>
                {
                    item.EncounterPatientTypeName = GetNameByGlobalCodeValueAndCategoryValue(globalCodeCategoryValue, Convert.ToString(item.EncounterPatientType));
                    return item;
                }).ToList();
            }
            return list;
        }

        /// <summary>
        /// Gets the encounter end check.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        public int GetEncounterEndCheck(int encounterId, int userId)
        {
            var spName = string.Format("EXEC {0} @pEncounterID , @pLoggedInUserId", StoredProcedures.SPROC_EncounterEndCheckBillEdit);
            var sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter(InputParams.pEncounterID.ToString(), encounterId);
            sqlParameters[1] = new SqlParameter(InputParams.pLoggedInUserId.ToString(), userId);

            IEnumerable<EncounterEndCheckReturnStatus> result = _context.Database.SqlQuery<EncounterEndCheckReturnStatus>(spName, sqlParameters);

            if (result.Any())
            {
                var encounterCheckReturnStatus = result.FirstOrDefault();
                return encounterCheckReturnStatus != null
                    ? encounterCheckReturnStatus.RetStatus
                    : 0;
            }
            return 0;
        }

        /// <summary>
        /// Gets the patient bed information by bed identifier.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="bedId">The bed identifier.</param>
        /// <param name="serviceCodeValue">The service code value.</param>
        /// <returns></returns>
        public EncounterCustomModel GetPatientBedInformationByBedId(int patientId, int bedId, string serviceCodeValue)
        {
            var result = GetPatientBedInformationByBedId(patientId, bedId, serviceCodeValue);
            return result;
        }

        /// <summary>
        /// Gets the patient bed information by patient identifier.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public EncounterCustomModel GetPatientBedInformationByPatientId(int patientId)
        {
            var spName = string.Format("EXEC {0} @pPatientID", StoredProcedures.SPROC_GetPatientBedInformation);
            var sqlParameters = new SqlParameter[1];
            sqlParameters[0] = new SqlParameter(InputParams.pPatientID.ToString(), patientId);
            var result = _context.Database.SqlQuery<EncounterCustomModel>(spName, sqlParameters);
            var bedInfo = result.FirstOrDefault();
            return bedInfo ?? new EncounterCustomModel();

        }

        /// <summary>
        /// Calulates the average length of stay.
        /// </summary>
        /// <param name="endcounterstartTime">The endcounterstart time.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public string CalulateAverageLengthOfStay(DateTime endcounterstartTime, int facilityId)
        {

            var currentdatetime = GetInvariantCultureDateTime(facilityId).ToShortDateString();
            var totaldays = (Convert.ToDateTime(currentdatetime) - Convert.ToDateTime(endcounterstartTime.ToShortDateString())).TotalDays;
            //return Math.Round(Convert.ToDecimal(totaldays - 1), 2, MidpointRounding.AwayFromZero).ToString();
            return Math.Round(Convert.ToDecimal(totaldays), 2, MidpointRounding.AwayFromZero).ToString();
        }

        /// <summary>
        /// Gets the active encounter chart data.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="displayType">The display type.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="tillDate">The till date.</param>
        /// <returns></returns>
        public List<ClaimDenialPercentage> GetActiveEncounterChartData(int facilityId, int displayType, DateTime fromDate, DateTime tillDate)
        {
            var spName = string.Format("EXEC {0} @pFacilityID, @pDisplayTypeID, @pFromDate, @pTillDate", StoredProcedures.SPROC_GetActiveEncounterGraphs);
            var sqlParameters = new SqlParameter[4];
            sqlParameters[0] = new SqlParameter(InputParams.pFacilityID.ToString(), facilityId);
            sqlParameters[1] = new SqlParameter(InputParams.pDisplayTypeID.ToString(), displayType);
            sqlParameters[2] = new SqlParameter(InputParams.pFromDate.ToString(), fromDate);
            sqlParameters[3] = new SqlParameter(InputParams.pTillDate.ToString(), tillDate);
            IEnumerable<ClaimDenialPercentage> result = _context.Database.SqlQuery<ClaimDenialPercentage>(spName, sqlParameters);
            return result.ToList();
        }

        /// <summary>
        /// Gets the encounter open status.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public bool GetEncounterOpenStatus(int patientId)
        {
            if (patientId > 0)
            {
                var result = _repository.Where(e => e.PatientID == patientId && !e.EncounterEndTime.HasValue).Any();
                return result;
            }
            return false;
        }

        /// <summary>
        /// Gets the encounters by patient identifier.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public List<EncounterCustomModel> GetEncountersByPatientId(int patientId)
        {
            var list = new List<EncounterCustomModel>();
            var lstEncounter = _repository.Where(_ => _.PatientID == patientId).OrderByDescending(x => x.EncounterStartTime).ToList();
            var eCat = (int)GlobalCodeCategoryValue.EncounterType;
            var eTypeCat = (int)GlobalCodeCategoryValue.EncounterPatientType;

            list.AddRange(lstEncounter.Select(item => new EncounterCustomModel
            {
                EncounterID = item.EncounterID,
                EncounterNumber = item.EncounterNumber,
                EncounterStartTime = item.EncounterStartTime,
                EncounterEndTime = item.EncounterEndTime,
                Charges = item.Charges,
                Payment = item.Payment,
                PatientID = item.PatientID,
                EncounterTypeName = GetNameByGlobalCodeValueAndCategoryValue(eCat.ToString(),
                        item.EncounterType.ToString()),
                EncounterPatientTypeName = GetNameByGlobalCodeValueAndCategoryValue(eTypeCat.ToString(),
                        item.EncounterPatientType.ToString()),
                BillHeaderId = _context.BillHeader.Where(a => a.EncounterID == item.EncounterID && a.PatientID == item.PatientID).FirstOrDefault().BillHeaderID
            }));
            return list;
        }



        /// <summary>
        /// Adds the bed charges for transfer patient.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <param name="currentdatetime">The currentdatetime.</param>
        /// <returns></returns>
        public bool AddBedChargesForTransferPatient(int encounterId, DateTime currentdatetime)
        {
            var sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("ComputedOn", currentdatetime);
            sqlParameters[1] = new SqlParameter("EncounterID", encounterId);
            _repository.ExecuteCommand(StoredProcedures.SPROC_ReValuateBedChargesPerEncounter.ToString(), sqlParameters);
            return true;
        }

        /// <summary>
        /// Gets the cmo dashboard data.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="corporateid">The corporateid.</param>
        /// <returns></returns>
        /// 
        public List<CMODashboardModel> GetCMODashboardData(int facilityId, int corporateid)
        {
            var spName = string.Format("EXEC {0} @CID,@FID", StoredProcedures.SPROC_GetCMODashboardData);
            var sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("CID", corporateid);
            sqlParameters[1] = new SqlParameter("FID", facilityId);
            IEnumerable<CMODashboardModel> result = _context.Database.SqlQuery<CMODashboardModel>(spName, sqlParameters);
            return result.ToList();
        }


        public List<Encounter> GetEncounterByUserId(int userId)
        {
            var list = _repository.Where(x => x.CreatedBy == userId || x.ModifiedBy == userId).ToList();
            return list;

        }

        public List<PatientEvaluationSetCustomModel> GetPreActiveEncounters(int encounterId, int patientId)
        {
            var list = new List<PatientEvaluationSetCustomModel>();
            var physicianName = string.Empty;
            var encounterNumber = string.Empty;

            var evaluationSetList = _context.PatientEvaluationSet.Where(i => i.PatientId == patientId && (i.EncounterId == encounterId || encounterId == 0)).ToList();
            if (evaluationSetList.Count > 0)
            {
                var currentEncounter = GetEncounterByEncounterId(encounterId);
                if (currentEncounter != null)
                {
                    physicianName = GetPhysicianName(currentEncounter.EncounterAttendingPhysician.GetValueOrDefault());
                    encounterNumber = currentEncounter.EncounterNumber;
                }
                list.AddRange(evaluationSetList.Select(item => new PatientEvaluationSetCustomModel
                {
                    SetId = item.SetId,
                    ENMStartdate = item.CreatedDate,
                    PhysicianName = physicianName,
                    EncounterNumber = encounterNumber,
                    DocumentName = string.IsNullOrEmpty(item.FormType) ? "Evaluation Management" : Convert.ToString(item.FormType),
                    ExtValue2 = item.ExtValue2,
                    CreatedBy = item.CreatedBy,
                    CompletedBy = _context.Users.Where(x => x.UserID == item.CreatedBy.GetValueOrDefault() && x.IsDeleted == false).FirstOrDefault().UserName
                }));

            }
            return list;
        }

        //public Encounter GetEncounterDetail(int encounterId)
        //{
        //    var currenDate = GetInvariantCultureDateTime(8);
        //    var list = new Encounter();
        //    using (var eRep = UnitOfWork.EncounterRepository)
        //    {
        //         list = eRep.Where(x => x.EncounterID == encounterId).FirstOrDefault();

        //        if (list.EncounterEndTime != null)
        //        {
        //            var dateDiff = (list.EncounterEndTime - list.EncounterStartTime);

        //        }
        //        else
        //        {
        //            var timeSpan = (currenDate - list.EncounterStartTime);
        //        }
        //        return list;
        //    }
        //}

        #region Triage Levels in Encounters
        public string UpdateTriageLevelInEncounter(int encounterId, string triageLevel)
        {
            string result = string.Empty;
            if (encounterId > 0)
            {
                var model = _repository.Where(e => e.EncounterID == encounterId).FirstOrDefault();
                if (model != null)
                    model.Triage = triageLevel;

                _repository.UpdateEntity(model, encounterId);
                result = GetNameByGlobalCodeValue(triageLevel, Convert.ToString((int)GlobalCodeCategoryValue.PatientTriageLevels));
            }
            return result;
        }
        #endregion

        public string UpdatePatitentStageInEncounter(int encounterId, string patientStage)
        {
            string result = string.Empty;
            if (encounterId > 0)
            {
                var model = _repository.Where(e => e.EncounterID == encounterId).FirstOrDefault();
                if (model != null)
                    model.PatientState = patientStage;

                _repository.UpdateEntity(model, encounterId);
                result = GetNameByGlobalCodeValue(patientStage, Convert.ToString((int)GlobalCodeCategoryValue.PatientState));
            }
            return result;
        }


        public List<Encounter> GetActiveMotherDropdownData(int corporateId)
        {
            //var patientInfoBal = new PatientInfoBal();
            var encountersList = GetEncounterList();
            encountersList = encountersList.Where(x => x.EncounterPatientType == Convert.ToInt32(EncounterPatientType.InPatient)
                && x.EncounterEndTime == null && x.CorporateID == corporateId).OrderByDescending(m => m.EncounterStartTime).ToList();
            //var patientInfoList = patientInfoBal.GetPatientDetailsByPatientId();


            return encountersList;
        }

        /// <summary>
        /// Gets the encounters list by patient identifier.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <returns></returns>
        public List<Encounter> GetEncountersListByPatientId(int p)
        {
            return _repository.Where(x => x.PatientID == p).ToList();
        }


        public List<PatientEvaluationSetCustomModel> GetNurseAssessmentData(int encounterId, int patientId)
        {
            var list = new List<PatientEvaluationSetCustomModel>();
            var physicianName = string.Empty;
            var encounterNumber = string.Empty;

            var evaluationSetList = _context.PatientEvaluationSet.Where(i => i.PatientId == patientId && (i.EncounterId == encounterId || encounterId == 0) && i.ExtValue2.Trim().Equals("99")).ToList();
            if (evaluationSetList.Count > 0)
            {
                var currentEncounter = GetEncounterByEncounterId(encounterId);
                if (currentEncounter != null)
                {
                    physicianName = GetPhysicianName(Convert.ToInt32(currentEncounter.EncounterAttendingPhysician));
                    encounterNumber = currentEncounter.EncounterNumber;
                }
                list.AddRange(evaluationSetList.Select(item => new PatientEvaluationSetCustomModel
                {
                    SetId = item.SetId,
                    ENMStartdate = item.CreatedDate,
                    PhysicianName = physicianName,
                    EncounterNumber = encounterNumber,
                    CreatedDate = item.CreatedDate,
                    DocumentName = Convert.ToString(item.FormType),
                    Title = "View Assessment Form"
                }));
            }
            return list;
        }

        /// <summary>
        /// Gets the encounter status before update.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        public int GetEncounterStatusBeforeUpdate(int encounterId)
        {
            var spName = string.Format("EXEC {0} @pEncounterID", StoredProcedures.SPROC_EncounterEndChecks_SA);
            var sqlParameters = new SqlParameter[1];
            sqlParameters[0] = new SqlParameter("pEncounterID", encounterId);
            IEnumerable<EncounterEndCheckReturnStatus> result = _context.Database.SqlQuery<EncounterEndCheckReturnStatus>(spName, sqlParameters);

            var r = result.ToList();
            if (r.Any())
            {
                var encounterCheckReturnStatus = r.FirstOrDefault();
                return encounterCheckReturnStatus != null
                    ? encounterCheckReturnStatus.RetStatus
                    : 0;
            }

            return 0;
        }

        /// <summary>
        /// Adds the virtual discharge log.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <returns></returns>
        public bool AddVirtualDischargeLog(int encounterId, int facilityId, int corporateId)
        {
            var sqlParameters = new SqlParameter[3];
            sqlParameters[0] = new SqlParameter("pEncId", encounterId);
            sqlParameters[1] = new SqlParameter("pFid", facilityId);
            sqlParameters[2] = new SqlParameter("pCid", corporateId);
            _repository.ExecuteCommand(StoredProcedures.SPROC_AddVirtualDischargeLog.ToString(), sqlParameters);
            return true;
        }

        /// <summary>
        /// Gets the encounter end check virtual discharge.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <param name="loggedInUserId">The logged in user identifier.</param>
        /// <returns></returns>
        public int GetEncounterEndCheckVirtualDischarge(int encounterId, int loggedInUserId)
        {
            var spName = string.Format("EXEC {0} @pEncounterID, @pLoggedInUserId", StoredProcedures.SPROC_EncounterEndCheckBillEdit_VirtualDischarge);
            var sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("pEncounterID", encounterId);
            sqlParameters[1] = new SqlParameter("pLoggedInUserId", loggedInUserId);
            IEnumerable<EncounterEndCheckReturnStatus> result = _context.Database.SqlQuery<EncounterEndCheckReturnStatus>(spName, sqlParameters);
            if (result.Any())
            {
                var encounterCheckReturnStatus = result.FirstOrDefault();
                return encounterCheckReturnStatus != null
                    ? encounterCheckReturnStatus.RetStatus
                    : 0;
            }
            return 0;
        }

        public List<EncounterCustomModel> GetAllActiveEncounters(string facilityNumber, List<int> patientTypes)
        {
            var spName = string.Format("EXEC {0}  @FacilityId", StoredProcedures.SPORC_GetEncounterData);
            var sqlParameters = new SqlParameter[1];
            sqlParameters[0] = new SqlParameter("FacilityId", facilityNumber);
            IEnumerable<EncounterCustomModel> result = _context.Database.SqlQuery<EncounterCustomModel>(spName, sqlParameters);
            return result.ToList();
        }

        public string GetTriageData(int encounterId)
        {
            var tValue = "999";
            var triData = _repository.Where(x => x.EncounterID == encounterId).FirstOrDefault();
            if (triData != null)
                tValue = triData.Triage;
            return tValue;
        }

        public string GetPatientStateData(int encounterId)
        {
            var sValue = "999";
            var stateData = _repository.Where(x => x.EncounterID == encounterId).FirstOrDefault();
            if (stateData != null)
                sValue = stateData.PatientState;

            return sValue;
        }

        /// <summary>
        /// Gets the encounter detail by encounter identifier.
        /// </summary>
        /// <param name="encounterid">The encounterid.</param>
        /// <returns></returns>
        public async Task<EncounterCustomModel> GetAuthorizationViewDataAsync(int encounterid)
        {
            var vm = new EncounterCustomModel();
            var encounterState = string.Empty;


            var encounter = _repository.Where(x => x.EncounterID == encounterid).Include(p => p.PatientInfo).FirstOrDefault();
            if (encounter != null)
            {
                var currentDateTime = GetInvariantCultureDateTime(Convert.ToInt32(encounter.EncounterFacility));
                var patientName = string.Empty;
                if (encounter.PatientInfo != null)
                    patientName = string.Format("{0} {1}", encounter.PatientInfo.PersonFirstName, encounter.PatientInfo.PersonLastName);

                if (encounter.EncounterPatientType != null)
                {
                    var endType = encounter.EncounterEndType != null ? Convert.ToInt32(encounter.EncounterEndType) : 0;
                    var patientType = encounter.EncounterPatientType != null ? Convert.ToInt32(encounter.EncounterPatientType) : 0;
                    encounterState = EncounterState(endType, patientType);
                }
                var encountertypeCat = Convert.ToInt32(GlobalCodeCategoryValue.EncounterType);
                vm = new EncounterCustomModel();

                vm.Charges = encounter.Charges;
                vm.EncounterAccidentDate = encounter.EncounterAccidentDate;
                vm.EncounterAccidentRelated = encounter.EncounterAccidentRelated;
                vm.EncounterAccidentType = encounter.EncounterAccidentType;
                vm.EncounterAccomodation = encounter.EncounterAccomodation;
                vm.EncounterAccomodationReason = encounter.EncounterAccomodationReason;
                vm.EncounterAccomodationRequest = encounter.EncounterAccomodationRequest;
                vm.EncounterAcuteCareFacilitynonUAE = encounter.EncounterAcuteCareFacilitynonUAE;
                vm.EncounterAdmitReason = encounter.EncounterAdmitReason;
                vm.EncounterAdmitType = encounter.EncounterAdmitType;
                vm.EncounterAmbulatoryCondition = encounter.EncounterAmbulatoryCondition;
                vm.EncounterComment = encounter.EncounterComment;
                vm.EncounterConfidentialityLevel = encounter.EncounterConfidentialityLevel;
                vm.EncounterDeceasedDate = encounter.EncounterDeceasedDate;
                vm.PatientID = encounter.PatientID;
                vm.PatientName = patientName;
                vm.PersonEmiratesIDNumber = encounter.PatientInfo != null ? encounter.PatientInfo.PersonEmiratesIDNumber : string.Empty;
                vm.EncounterPatientType = encounter.EncounterPatientType;
                vm.EncounterModeofArrival = encounter.EncounterModeofArrival;
                vm.EncounterServiceCategory = encounter.EncounterServiceCategory;
                vm.EncounterID = encounter.EncounterID;
                vm.EncounterNumber = encounter.EncounterNumber;
                vm.EncounterStartType = encounter.EncounterStartType;
                vm.EncounterType = encounter.EncounterType;
                vm.EncounterSpecialty = encounter.EncounterSpecialty;
                vm.EncounterInpatientAdmitDate = encounter.EncounterInpatientAdmitDate;
                vm.EncounterEndType = encounter.EncounterEndType;
                vm.EncounterStartTime = encounter.EncounterStartTime;
                vm.EncounterTransferHospital = encounter.EncounterTransferHospital;
                vm.EncounterTransferSource = encounter.EncounterTransferSource;
                vm.EncounterFacility = encounter.EncounterFacility;
                vm.EncounterFacilityID = encounter.EncounterFacilityID;
                vm.EncounterState = encounterState;
                vm.EncounterEndTime = currentDateTime;
                vm.BedName = GetBedNameByInPatientEncounterId(Convert.ToString(encounter.EncounterID));
                vm.BedId = GetBedByInPatientEncounterId(Convert.ToString(encounter.EncounterID));
                vm.EncounterAttendingPhysician = encounter.EncounterAttendingPhysician;
                vm.EncounterPhysicianType = encounter.EncounterPhysicianType;
                vm.PersonMedicalRecordNumber = encounter.PatientInfo != null ? encounter.PatientInfo.PersonMedicalRecordNumber : string.Empty;
                vm.PatientInfo = encounter.PatientInfo;
                vm.EncounterPatientTypeName = GetNameByGlobalCodeValueAndCategoryValue(Convert.ToString((int)GlobalCodeCategoryValue.EncounterPatientType), encounter.EncounterPatientType.ToString());
                vm.EncounterSpecialityName = GetNameByGlobalCodeValueAndCategoryValue(Convert.ToInt32(GlobalCodeCategoryValue.EncounterSpecialty).ToString(), encounter.EncounterSpecialty.ToString());
                vm.EncounterModeOfArrivalName = GetNameByGlobalCodeValueAndCategoryValue(Convert.ToInt32(GlobalCodeCategoryValue.EncounterModeofArrival).ToString(), encounter.EncounterModeofArrival.ToString());
                vm.EncounterServiceCategoryName = GetNameByGlobalCodeValueAndCategoryValue(Convert.ToInt32(GlobalCodeCategoryValue.EncounterServiceCategory).ToString(), encounter.EncounterServiceCategory.ToString());
                vm.EncounterPhysicianTypeName = GetNameByGlobalCodeValueAndCategoryValue(Convert.ToInt32(GlobalCodeCategoryValue.EncounterPhysicianType).ToString(), encounter.EncounterPhysicianType.ToString());
                vm.EncounterPhysicianName = GetPhysicianName(Convert.ToInt32(encounter.EncounterAttendingPhysician));
                vm.EncounterAdmitTypeName = GetNameByGlobalCodeValueAndCategoryValue(Convert.ToInt32(GlobalCodeCategoryValue.EncounterAdmitType).ToString(), encounter.EncounterAdmitType.ToString());
                vm.Age = CalculatePersonAge(encounter.PatientInfo.PersonBirthDate, currentDateTime, Convert.ToInt32(encounter.EncounterFacility));
                vm.OverrideBedType = GetOverRideBedTypeByInPatientEncounterId(Convert.ToString(encounter.EncounterID));
                vm.EncounterTypeName = GetNameByGlobalCodeValueAndCategoryValue(Convert.ToString(encountertypeCat), encounter.EncounterType.ToString());
                vm.VirtuallyDischarge = encounter.EncounterDischargePlan != null ? "Discharge" : "";
                vm.VirtuallyDischargeOn = encounter.EncounterDischargePlan != null ? encounter.EncounterDischargeLocation : null;

                if (encounter != null && encounter.PatientInfo != null && !string.IsNullOrEmpty(encounter.PatientInfo.PersonInsuranceCompany))
                {
                    var ins = GetInsuranceDetailsByPayorId(encounter.PatientInfo.PersonInsuranceCompany);
                    if (ins != null)
                    {
                        vm.InsuranceCompanyName = ins.InsuranceCompanyName;
                        vm.InsuranceCompanyPhoneNumber = ins.InsuranceCompanyMainPhone;
                        vm.InsuranceCompanyFaxNumber = ins.InsuranceCompanyFax;
                        vm.InsuranceCompanyAddress = $"{ ins.InsuranceCompanyStreetAddress} { ins.InsuranceCompanyStreetAddress2}";
                    }
                }
                vm.EncounterAuthorizationList = GetAuthorizationsByEncounterId(vm.EncounterID.ToString());

                var pId = Convert.ToInt32(vm.PatientID);
                if (vm.EncounterAuthorizationList.Any())
                {
                    vm.AuthDocs = new List<DocumentsTemplates>();
                    var docs = await GetPatientDocumentsList(pId);
                    if (docs.Any())
                        vm.AuthDocs = docs.Where(a => a.DocumentName.Equals("Authorization File")).ToList();
                }

                if (vm.EncounterAuthorizationList.Any(a => a.AuthorizationEnd.HasValue && a.AuthorizationEnd.Value >= currentDateTime.Date))
                {
                    vm.EncounterAuthorization = vm.EncounterAuthorizationList.Where(a => a.AuthorizationEnd.HasValue && a.AuthorizationEnd.Value >= currentDateTime.Date).FirstOrDefault();

                    if (vm.EncounterAuthorization != null)
                    {
                        vm.EncounterAuthorization.AuthorizationMemberID = GetInsuranceMemberIdByPatientId(Convert.ToInt32(vm.PatientID));
                        vm.EncounterAuthorization.AuthorizationIDPayer = GetAuthorizationIdPayerByPatientId(Convert.ToInt32(vm.PatientID));
                    }
                }
                else
                {
                    vm.EncounterAuthorization = new Authorization()
                    {
                        EncounterID = vm.EncounterID.ToString(),
                        PatientID = vm.PatientID,
                        AuthorizationStart = vm.EncounterStartTime,
                        AuthorizationMemberID = GetInsuranceMemberIdByPatientId(Convert.ToInt32(vm.PatientID)),
                        AuthorizationIDPayer = GetAuthorizationIdPayerByPatientId(Convert.ToInt32(vm.PatientID)),
                    };
                }
            }

            return vm;

        }

        public bool EncounterOpenOrders(int encounterId)
        {
            return _context.OpenOrder.Where(p => p.CreatedBy != null && (int)p.EncounterID == encounterId && p.OrderStatus == "1")
                    .ToList().Any();

        }

        private async Task<List<DocumentsTemplates>> GetPatientDocumentsList(int patientId)
        {
            var sqlParams = new SqlParameter[5];
            sqlParams[0] = new SqlParameter("@pFId", 0);
            sqlParams[1] = new SqlParameter("@pCId", 0);
            sqlParams[2] = new SqlParameter("@pUserId", 0);
            sqlParams[3] = new SqlParameter("@pPId", patientId);
            sqlParams[4] = new SqlParameter("@pExclusions", "profilepicture");

            using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetDocumentsByPatient.ToString(), false, parameters: sqlParams))
            {
                var docs = (await r.ResultSetForAsync<DocumentsTemplates>()).ToList();
                return docs;
            }

        }

        public bool PatientEncounterOpenOrders(int patientId)
        {
            var patientCurrentEncounterid = _repository.Where(e => e.PatientID == patientId && e.EncounterEndTime == null).FirstOrDefault().EncounterID;
            return EncounterOpenOrders(patientCurrentEncounterid);
        }
    }
}
