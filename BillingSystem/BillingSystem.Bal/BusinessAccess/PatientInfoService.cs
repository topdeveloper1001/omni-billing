using System;
using BillingSystem.Model;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using BillingSystem.Model.CustomModel;
using BillingSystem.Common.Common;


using System.Threading.Tasks;
using System.Data;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class PatientInfoService : IPatientInfoService
    {
        private readonly IRepository<Authorization> _aurepository;
        private readonly IRepository<Encounter> _eRepository;
        private readonly IRepository<PatientInfo> _repository;
        private readonly IRepository<Facility> _fRepository;
        private readonly IRepository<MaxValues> _mvRepository;
        private readonly IRepository<Corporate> _cRepository;
        private readonly IRepository<PatientPhone> _phRepository;
        private readonly IRepository<PatientLoginDetail> _plRepository;
        private readonly IRepository<PatientInsurance> _pinRepository;
        private readonly IRepository<DocumentsTemplates> _dtRepository;
        private readonly BillingEntities _context;

        public PatientInfoService(IRepository<Authorization> aurepository, IRepository<Encounter> eRepository, IRepository<PatientInfo> repository, IRepository<Facility> fRepository, IRepository<MaxValues> mvRepository, IRepository<Corporate> cRepository, IRepository<PatientPhone> phRepository, IRepository<PatientLoginDetail> plRepository, IRepository<PatientInsurance> pinRepository, IRepository<DocumentsTemplates> dtRepository, BillingEntities context)
        {
            _aurepository = aurepository;
            _eRepository = eRepository;
            _repository = repository;
            _fRepository = fRepository;
            _mvRepository = mvRepository;
            _cRepository = cRepository;
            _phRepository = phRepository;
            _plRepository = plRepository;
            _pinRepository = pinRepository;
            _dtRepository = dtRepository;
            _context = context;
        }

        private string GetCorporateNameFromId(int corpId)
        {
            var corpName = "";
            var obj = _cRepository.Where(f => f.CorporateID == corpId).FirstOrDefault();
            if (obj != null) corpName = obj.CorporateName;
            return corpName;
        }
        public PatientInfo GetPatientInfoByEncounterId(int encounterId)
        {
            var patientInfo = new PatientInfo();
            var patientId = _eRepository.Where(e => e.EncounterID == encounterId).FirstOrDefault() != null ? _eRepository.Where(e => e.EncounterID == encounterId).FirstOrDefault().PatientID : 0;

            if (patientId > 0) patientInfo = _repository.Where(p => p.PatientID == patientId && p.IsDeleted == false).FirstOrDefault();

            return patientInfo;
        }
        private DateTime GetInvariantCultureDateTime(int facilityid)
        {

            var facilityObj = _fRepository.Where(f => f.FacilityId == Convert.ToInt32(facilityid)).FirstOrDefault() != null ? _fRepository.Where(f => f.FacilityId == Convert.ToInt32(facilityid)).FirstOrDefault().FacilityTimeZone : TimeZoneInfo.Utc.ToString();
            var tzi = TimeZoneInfo.FindSystemTimeZoneById(facilityObj);
            var utcTime = DateTime.Now.ToUniversalTime();
            var convertedTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);
            return convertedTime;
        }
        //public PatientInfoService(string cptTableNumber, string serviceCodeTableNumber, string drgTableNumber, string drugTableNumber, string hcpcsTableNumber, string diagnosisTableNumber)
        //{
        //    if (!string.IsNullOrEmpty(cptTableNumber))
        //        CptTableNumber = cptTableNumber;

        //    if (!string.IsNullOrEmpty(serviceCodeTableNumber))
        //        ServiceCodeTableNumber = serviceCodeTableNumber;

        //    if (!string.IsNullOrEmpty(drgTableNumber))
        //        DrgTableNumber = drgTableNumber;

        //    if (!string.IsNullOrEmpty(drugTableNumber))
        //        DrugTableNumber = drugTableNumber;

        //    if (!string.IsNullOrEmpty(hcpcsTableNumber))
        //        HcpcsTableNumber = hcpcsTableNumber;

        //    if (!string.IsNullOrEmpty(diagnosisTableNumber))
        //        DiagnosisTableNumber = diagnosisTableNumber;
        //}


        /// <summary>
        /// Get the patient Info by Id
        /// </summary>
        /// <param name="patientId"></param>
        public PatientInfo GetPatientInfoById(int? patientId)
        {
            if (patientId > 0)
            {
                var patientInfo = _repository.Where(x => x.PatientID == patientId && x.IsDeleted != true).FirstOrDefault();
                return patientInfo;

            }
            return new PatientInfo();
        }

        /// <summary>
        /// Get the patient Info by Id
        /// </summary>
        /// <param name="patientId"></param>
        public PatientInfoCustomModel GetPatientInfoCustomModelById(int? patientId)
        {
            var vm = new PatientInfoCustomModel();
            var model = _repository.Where(x => x.PatientID == patientId && x.IsDeleted == false).FirstOrDefault();
            vm.PatientInfo = model ?? new PatientInfo();

            if (model != null)
            {
                vm.PatientName = string.Format("{0} {1}", model.PersonFirstName, model.PersonLastName);
                vm.PersonAge = CalculatePersonAge(model.PersonBirthDate, GetInvariantCultureDateTime(Convert.ToInt32(model.FacilityId)));
                vm.PatientIsVIP = !string.IsNullOrEmpty(model.PersonVIP);
                vm.PatientMaritalStatus =
                    !string.IsNullOrEmpty(model.PersonMaritalStatus);
                vm.PatientSex = !string.IsNullOrEmpty(model.PersonGender);

                /*
                 * Owner: Amit Jain 
                 * On: 17112014
                 * Purpose: Made changes in the code of fetching the Patient's Profile Image. 
                 */
                var profileImage = _dtRepository.Where(d => d.AssociatedType == Convert.ToInt32(AttachmentType.ProfilePicture) && (d.AssociatedID != null && d.AssociatedID == model.PatientID) && (d.IsDeleted == null || d.IsDeleted == false)).FirstOrDefault();// GetDocumentByTypeAndPatientId(, model.PatientID);
                if (profileImage != null)
                {
                    vm.ProfilePicImagePath = profileImage.FilePath;
                    vm.DocumentTemplateId = profileImage.DocumentsTemplatesID;
                }
                else
                    vm.ProfilePicImagePath = "/images/BlankProfilePic.png";

            }
            else
            {
                vm.PatientIsVIP = false;
                vm.PatientMaritalStatus = false;
                vm.PatientSex = false;
            }

            return vm;
        }



        /// <summary>
        /// Calculates the person age.
        /// </summary>
        /// <param name="birthDate">The birth date.</param>
        /// <param name="now">The now.</param>
        /// <returns></returns>
        public int CalculatePersonAge(DateTime? birthDate, DateTime now)
        {
            var birthdateUpdated = birthDate ?? now;
            var age = now.Year - birthdateUpdated.Year;
            if (now.Month < birthdateUpdated.Month || (now.Month == birthdateUpdated.Month && now.Day < birthdateUpdated.Day)) age--;
            return age;
        }

        /// <summary>
        /// Save the Patient info from the view model and restuen the id of saved patient.
        /// </summary>
        /// <returns>Saved patient id</returns>
        public int AddUpdatePatientInfo(PatientInfo patientInfo)
        {
            if (patientInfo.PatientID > 0)
            {
                var current = _repository.GetSingle(patientInfo.PatientID);
                patientInfo.CreatedBy = current.CreatedBy;
                patientInfo.CreatedDate = current.CreatedDate;
                patientInfo.CorporateId = current.CorporateId;
                patientInfo.FacilityId = current.FacilityId;
                patientInfo.IsDeleted = current.IsDeleted;
                patientInfo.PersonType = current.PersonType;
                patientInfo.PersonMedicalRecordNumber = !string.IsNullOrEmpty(patientInfo.PersonMedicalRecordNumber)
                    ? patientInfo.PersonMedicalRecordNumber : current.PersonMedicalRecordNumber;
                _repository.UpdateEntity(patientInfo, patientInfo.PatientID);
            }
            else
            {
                _repository.Create(patientInfo);
                if (patientInfo.PatientID > 0)
                    SaveMaxMedicalRecordNumber(patientInfo.PersonMedicalRecordNumber);
            }
            return patientInfo.PatientID;
        }


        /// <summary>
        /// Get the Patinet Serach value
        /// </summary>
        /// <returns>Return the user after login</returns>
        public List<PatientInfoCustomModel> GetPatientSearchResult(CommonModel vm)
        {
            var patientSearchResults = new List<PatientInfo>();
            var returnCustomLst = new List<PatientInfoCustomModel>();
            if (!string.IsNullOrEmpty(vm.PersonLastName))
                patientSearchResults.AddRange(
                    _repository.Where(
                        p => p.FacilityId == vm.FacilityId &&
                             !string.IsNullOrEmpty(p.PersonLastName) &&
                             p.PersonLastName.ToLower().Contains(vm.PersonLastName.ToLower())));

            if (!string.IsNullOrEmpty(vm.PersonPassportNumber))
                patientSearchResults.AddRange(
                    _repository.Where(
                        p =>
                            !string.IsNullOrEmpty(p.PersonPassportNumber) &&
                            p.FacilityId == vm.FacilityId &&
                            p.PersonPassportNumber.ToLower()
                                .Contains(vm.PersonPassportNumber.ToLower())));


            if (!string.IsNullOrEmpty(vm.PersonEmiratesIDNumber))
                patientSearchResults.AddRange(
                    _repository.Where(
                        p =>
                            !string.IsNullOrEmpty(p.PersonEmiratesIDNumber) &&
                            p.FacilityId == vm.FacilityId &&
                            p.PersonEmiratesIDNumber.ToLower()
                                .Contains(vm.PersonEmiratesIDNumber.ToLower())));

            if (vm.PersonBirthDate.HasValue)
                patientSearchResults.AddRange(
                    _repository.Where(
                        p =>
                            p.PersonBirthDate.HasValue && p.FacilityId == vm.FacilityId &&
                            p.PersonBirthDate.Value.Day == vm.PersonBirthDate.Value.Day &&
                            p.PersonBirthDate.Value.Month == vm.PersonBirthDate.Value.Month &&
                            p.PersonBirthDate.Value.Year == vm.PersonBirthDate.Value.Year
                        ));

            if (!string.IsNullOrEmpty(vm.ContactMobilePhone))
                patientSearchResults.AddRange(_repository.Where(
                    p => p.FacilityId == vm.FacilityId &&
                         !string.IsNullOrEmpty(p.PersonContactNumber) &&
                         p.PersonContactNumber.ToLower()
                             .Contains(vm.ContactMobilePhone.ToLower())));


            patientSearchResults = patientSearchResults.Distinct().ToList();

            returnCustomLst.AddRange(from patientInfo in patientSearchResults
                                     let encounterObj = _eRepository.Where(_ => _.PatientID == patientInfo.PatientID && _.EncounterEndTime == null).FirstOrDefault()
                                     let authorizationObj = encounterObj != null ? GetAuthorizationByEncounterId(Convert.ToString(encounterObj.EncounterID)) : null
                                     select new PatientInfoCustomModel
                                     {
                                         PatientInfo = patientInfo,
                                         IsEncounterExist = encounterObj != null,
                                         IsAuthorizationExist = encounterObj != null && authorizationObj != null,
                                         PersonAge = patientInfo != null && patientInfo.PersonBirthDate.HasValue ? CalculatePersonAge(patientInfo.PersonBirthDate.Value, GetInvariantCultureDateTime(Convert.ToInt32(patientInfo.FacilityId))) : 0,
                                         FacilityName = GetFacilityNameByFacilityId(Convert.ToInt32(patientInfo.FacilityId)),
                                         CorporateName = GetCorporateNameFromId(Convert.ToInt32(patientInfo.CorporateId)),
                                         PatientActiveEncounterFacilityId = encounterObj != null ? Convert.ToInt32(encounterObj.EncounterFacility) : 0
                                     });

            return returnCustomLst.ToList();

        }

        private string GetFacilityNameByFacilityId(int facilityId)
        {
            var m = _fRepository.GetSingle(facilityId);
            return m != null ? m.FacilityName : string.Empty;
        }
        private Authorization GetAuthorizationByEncounterId(string authorizationEncounterId)
        {
            var authorization =
                _aurepository.Where(
                    x => x.EncounterID == authorizationEncounterId && (x.IsDeleted == null || x.IsDeleted == false))
                    .FirstOrDefault();
            return authorization;
        }
        /// <summary>
        /// Checks the patient active encounter.
        /// </summary>
        /// <param name="patientidint">The patientidint.</param>
        /// <param name="encountertypeint">The encountertypeint.</param>
        /// <returns></returns>
        public object CheckPatientActiveEncounter(int patientidint, int encountertypeint)
        {
            bool isActiveEncounter;
            isActiveEncounter = _eRepository.Where(x => x.PatientID == patientidint && x.EncounterType == encountertypeint && x.EncounterEndTime == null).Any();
            return isActiveEncounter;
        }

        /// <summary>
        /// Checks if birth date exists.
        /// </summary>
        /// <param name="birthDate">The birth date.</param>
        /// <returns></returns>
        public bool CheckIfBirthDateExists(DateTime birthDate)
        {
            var isExists = _repository.GetAll().Any(x => DateTime.Compare(x.PersonBirthDate.Value.Date, birthDate.Date) == 0);
            return isExists;
        }

        /// <summary>
        /// Checks if passport exists.
        /// </summary>
        /// <param name="passportNumber">The passport number.</param>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public bool CheckIfPassportExists(string passportNumber, int patientId)
        {
            var isExists = patientId > 0
                ? _repository.Where(x => x.PersonPassportNumber.Equals(passportNumber) && x.PatientID != patientId).Any()
                : _repository.Where(x => x.PersonPassportNumber.Equals(passportNumber)).Any();
            return isExists;
        }

        /// <summary>
        /// Checks if emirates identifier exists.
        /// </summary>
        /// <param name="emiratesId">The emirates identifier.</param>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="lastname">The lastname.</param>
        /// <param name="bDate">The birth date.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public bool CheckIfEmiratesIdExists(string emiratesId, int patientId, string lastname, DateTime bDate, int facilityId)//added lastname and Birthdate by ashwani, issue with date comparison
        {
            if (emiratesId != null)
            {
                var isExists = patientId > 0
                       ? _repository.Where(
                           x =>
                               x.PersonEmiratesIDNumber.Equals(emiratesId) && x.PatientID != patientId &&
                               x.PersonLastName.Contains(lastname) && x.FacilityId == facilityId &&
                               x.PersonBirthDate.Value.Year == bDate.Year &&
                               x.PersonBirthDate.Value.Month == bDate.Month &&
                               x.PersonBirthDate.Value.Day == bDate.Day).Any()
                       : _repository.Where(
                           x => x.PersonEmiratesIDNumber.Equals(emiratesId) && x.PersonLastName.Contains(lastname) &&
                                x.PersonBirthDate.Value.Year == bDate.Year && x.FacilityId == facilityId &&
                                x.PersonBirthDate.Value.Month == bDate.Month &&
                                x.PersonBirthDate.Value.Day == bDate.Day).Any();
                return isExists;

            }
            return false;
        }

        /// <summary>
        /// Checks if medical record number.
        /// </summary>
        /// <param name="medicalRecordNumber">The medical record number.</param>
        /// <returns></returns>
        public bool CheckIfMedicalRecordNumber(string medicalRecordNumber)
        {
            var patientInfo = _repository.Where(p => p.PersonMedicalRecordNumber.Equals(medicalRecordNumber)).FirstOrDefault();
            return patientInfo != null;

        }
        //function to get auto generate medical number
        public int GetMaxMedicalRecordNumber()
        {
            var key = Convert.ToInt32(MaxValueKeys.MedicalRecordNumber);
            var max = _mvRepository.Where(x => x.AssociatedKey == key).FirstOrDefault();
            return max != null ? Convert.ToInt32(max.MaxValue) : 1;

        }

        /// <summary>
        /// Save max medical record number in the table MaxValues
        /// </summary>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        void SaveMaxMedicalRecordNumber(string maxValue)
        {
            var key = Convert.ToInt32(MaxValueKeys.MedicalRecordNumber);
            var max = _mvRepository.Where(x => x.AssociatedKey == key).FirstOrDefault();
            if (max == null)
            {
                var newRecord = new MaxValues
                {
                    AssociatedKey = Convert.ToInt32(MaxValueKeys.MedicalRecordNumber),
                    MaxValue = "1000000001"
                };
                _mvRepository.Create(newRecord);
                return;
            }
            if (!string.IsNullOrEmpty(maxValue) && Convert.ToInt32(maxValue) == Convert.ToInt32(max.MaxValue))
            {
                max.MaxValue = Convert.ToString(Convert.ToInt32(maxValue) + 1);
                _mvRepository.UpdateEntity(max, max.Id);

            }
        }

        /// <summary>
        /// Gets the patient list.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public List<PatientInfo> GetPatientList(int facilityId)
        {
            var result = _repository.Where(p => p.FacilityId == facilityId).OrderBy(p => p.PersonFirstName).ToList();
            return result;

        }

        public string GetPersonMotherNameById(int patientId)
        {
            var patient = _repository.Where(p => p.PatientID == patientId).FirstOrDefault();
            return patient != null
                       ? string.Format("{0}", patient.PersonMotherName)
                       : string.Empty;
        }
        public string GetPatientNameById(int PatientID)
        {
            var m = _repository.GetSingle(Convert.ToInt32(PatientID));
            return m != null ? m.PersonFirstName + " " + m.PersonLastName : string.Empty;
        }


        public bool CheckForDuplicateHealthCareNumber(string memberId, int patientId, int insCompanyId, int insPlanId, int insPolicyId)
        {
            if (string.IsNullOrEmpty(memberId))
                return false;


            var isExists = patientId > 0
                ? _pinRepository.Where(x =>
                    x.PatientID != patientId && x.PersonHealthCareNumber == memberId &&
                    x.InsuranceCompanyId == insCompanyId && x.InsurancePlanId == insPlanId).Any()
                : _pinRepository.Where(
                    x =>
                        x.PersonHealthCareNumber == memberId && x.InsuranceCompanyId == insCompanyId &&
                        x.InsurancePlanId == insPlanId).Any();
            return isExists;

        }

        /// <summary>
        /// Checks if email exists.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        public bool CheckIfEmailExists(string email)
        {
            var isExists = _plRepository.Where(x => x.Email.Equals(email)).Any();
            return isExists;

        }

        /// <summary>
        /// Gets the patient detail by emailid.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        public PatientInfo GetPatientDetailByEmailid(string email)
        {
            var emailexist = _plRepository.Where(x => x.Email.Equals(email)).FirstOrDefault();
            var patientdata = _repository.Where(x => x.PatientID == emailexist.PatientId).FirstOrDefault();
            return patientdata;
        }

        /// <summary>
        /// Checks for duplicate email.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public bool CheckForDuplicateEmail(string email, int patientId)
        {
            email = email.ToLower().Trim();

            var isExists = patientId > 0
                ? _plRepository.Where(x =>
                    x.PatientId != patientId && x.Email.ToLower().Trim().Equals(email)).Any()
                : _plRepository.Where(
                    x => x.Email.ToLower().Trim().Equals(email)).Any();
            return isExists;

        }

        /// <summary>
        /// Method is used to get the patient name
        /// </summary>
        /// <param name="patientName">Name of the patient.</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId"></param>
        /// <returns></returns>
        public IEnumerable<PatientInfo> GetPatientInfoByPatientName(string patientName, int corporateId, long facilityId)
        {
            var sqlParameters = new SqlParameter[3];
            sqlParameters[0] = new SqlParameter("pText", patientName);
            sqlParameters[1] = new SqlParameter("pFId", facilityId);
            sqlParameters[2] = new SqlParameter("pCId", corporateId);

            using (var ms = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetPatientSearchResults.ToString(), parameters: sqlParameters, isCompiled: false))
            {
                var list = ms.GetResultWithJson<PatientInfo>(JsonResultsArray.PatientSearchResults.ToString());
                if (list != null)
                    return list;
            }

            return Enumerable.Empty<PatientInfo>().ToList();
        }


        public int GetNextPatientId()
        {
            var nextId = _repository.GetAll().Select(p => p.PatientID).Max();
            return nextId + 1;

        }


        private List<PatientInfoXReturnPaymentCustomModel> GetPatientEncounterDetail(int corporateId, int facilityId)
        {
            //SPROC_GetXPaymentReturnDenialClaims
            try
            {
                if (_context != null)
                {
                    string spName = string.Format("EXEC {0} @pCorporateID, @pFacilityID",
                        StoredProcedures.SPROC_GetPatientEnciunterInPayment);
                    var sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("pCorporateID", corporateId);
                    sqlParameters[1] = new SqlParameter("pFacilityID", facilityId);
                    IEnumerable<PatientInfoXReturnPaymentCustomModel> result =
                        _context.Database.SqlQuery<PatientInfoXReturnPaymentCustomModel>(spName, sqlParameters);
                    return result.ToList();

                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the patient search result in payment.
        /// </summary>
        /// <param name="common">The common.</param>
        /// <returns></returns>
        public List<PatientInfoXReturnPaymentCustomModel> GetPatientSearchResultInPayment(CommonModel common)
        {
            var patientInfoLst =
                     GetPatientEncounterDetail(common.CorporateId, common.FacilityId);
            var returnCustomLst = new List<PatientInfoXReturnPaymentCustomModel>();

            if (patientInfoLst.Count > 0)
            {
                if (!string.IsNullOrEmpty(common.PersonLastName))
                    returnCustomLst.AddRange(
                        patientInfoLst.Where(
                            p => p.PersonLastName.ToLower().Contains(common.PersonLastName.ToLower())).ToList());
                if (!string.IsNullOrEmpty(common.PersonPassportNumber))
                    returnCustomLst.AddRange(
                        patientInfoLst.Where(
                            p =>
                                p.PersonPassportNumber != null &&
                                p.PersonPassportNumber.ToLower().Contains(common.PersonPassportNumber.ToLower()))
                            .ToList());
                if (!string.IsNullOrEmpty(common.PersonEmiratesIDNumber))
                    returnCustomLst.AddRange(
                        patientInfoLst.Where(
                            p =>
                                p.PersonEmiratesIDNumber != null && p.PersonEmiratesIDNumber.ToLower()
                                    .Contains(common.PersonEmiratesIDNumber.ToLower())).ToList());

                if (common.PersonBirthDate.HasValue)
                    returnCustomLst.AddRange(
                        patientInfoLst.Where(
                            p => p.PersonBirthdate.Date.Equals(common.PersonBirthDate.Value.Date)).ToList());

                if (!string.IsNullOrEmpty(common.ContactMobilePhone))
                    returnCustomLst.AddRange(
                        patientInfoLst.Where(
                            p =>
                                !string.IsNullOrEmpty(p.PhoneNo) &&
                                p.PhoneNo.ToLower().Contains(common.ContactMobilePhone.ToLower())).ToList());
            }
            return returnCustomLst.ToList();

        }


        public PatientInfoCustomModel PatientDetailsByPatientIdForDropdown(int patientId)
        {
            var current = _repository.Where(x => x.PatientID == patientId && x.PersonGender.Trim().ToLower().Equals("female")).FirstOrDefault();
            var patientinfoCustomModel = new PatientInfoCustomModel
            {
                PatientInfo = current
            };
            return patientinfoCustomModel;
        }

        /// <summary>
        /// Patients the information for scheduling by patient.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public RegisterPatientCustomModel PatientInfoForSchedulingByPatient(int patientId)
        {
            var current = _repository.Where(x => x.PatientID == patientId).FirstOrDefault();
            if (current != null)
            {
                var patientinfoCustomModel = new RegisterPatientCustomModel
                {
                    CorporateId = Convert.ToInt32(current.CorporateId),
                    FacilityId = Convert.ToInt32(current.FacilityId),
                    PersonFirstName = current.PersonFirstName + ' ' + current.PersonLastName,
                    EmirateId = current.PersonEmiratesIDNumber,
                    PersonDateOfBirth = current.PersonBirthDate.HasValue ? current.PersonBirthDate.Value.ToShortDateString() : string.Empty,
                    PersonEmailId = GetPatientEmailAddress(patientId),
                    PersonGender = current.PersonGender,
                    PersonPhoneNumber = GetPatientPhoneNumberById(patientId)
                };
                return patientinfoCustomModel;
            }

            return new RegisterPatientCustomModel();

        }
        private string GetPatientEmailAddress(int patientId)
        {
            var m = _plRepository.Where(x => x.PatientId == patientId && (x.IsDeleted == null || x.IsDeleted == false)).FirstOrDefault().Email;
            return m;

        }
        private string GetPatientPhoneNumberById(int patientId)
        {
            var phoneType = Convert.ToInt32(PhoneType.MobilePhone);
            var patientPhoneModel = _phRepository.Where(x => x.PatientID == patientId && x.IsDeleted == false && x.PhoneType == phoneType).FirstOrDefault();
            return patientPhoneModel.PhoneNo;
        }

        public List<PatientInfoCustomModel> GetPatientSearchResultByCId(CommonModel patientSearch)
        {
            var returnLst = new List<PatientInfo>();
            var returnCustomLst = new List<PatientInfoCustomModel>();
            if (!string.IsNullOrEmpty(patientSearch.PersonLastName))
                returnLst.AddRange(
                    _repository.Where(
                        p => p.CorporateId == patientSearch.CorporateId &&
                             !string.IsNullOrEmpty(p.PersonLastName) &&
                             p.PersonLastName.ToLower().Contains(patientSearch.PersonLastName.ToLower())));

            if (!string.IsNullOrEmpty(patientSearch.PersonPassportNumber))
                returnLst.AddRange(
                    _repository.Where(
                        p =>
                            !string.IsNullOrEmpty(p.PersonPassportNumber) &&
                            p.CorporateId == patientSearch.CorporateId &&
                            p.PersonPassportNumber.ToLower()
                                .Contains(patientSearch.PersonPassportNumber.ToLower())));


            if (!string.IsNullOrEmpty(patientSearch.PersonEmiratesIDNumber))
                returnLst.AddRange(
                    _repository.Where(
                        p =>
                            !string.IsNullOrEmpty(p.PersonEmiratesIDNumber) &&
                            p.CorporateId == patientSearch.CorporateId &&
                            p.PersonEmiratesIDNumber.ToLower()
                                .Contains(patientSearch.PersonEmiratesIDNumber.ToLower())));

            if (patientSearch.PersonBirthDate.HasValue)
                returnLst.AddRange(
                    _repository.Where(
                        p =>
                            p.PersonBirthDate.HasValue && p.CorporateId == patientSearch.CorporateId &&
                            p.PersonBirthDate.Value.Day == patientSearch.PersonBirthDate.Value.Day &&
                            p.PersonBirthDate.Value.Month == patientSearch.PersonBirthDate.Value.Month &&
                            p.PersonBirthDate.Value.Year == patientSearch.PersonBirthDate.Value.Year
                        ));

            if (!string.IsNullOrEmpty(patientSearch.ContactMobilePhone))
                returnLst.AddRange(_repository.Where(
                    p => p.CorporateId == patientSearch.CorporateId &&
                         !string.IsNullOrEmpty(p.PersonContactNumber) &&
                         p.PersonContactNumber.ToLower()
                             .Contains(patientSearch.ContactMobilePhone.ToLower())));

            returnLst = returnLst.Distinct().ToList();
            returnCustomLst.AddRange(from patientInfo in returnLst
                                     let encounterObj = _eRepository.Where(_ => _.PatientID == patientInfo.PatientID && _.EncounterEndTime == null).FirstOrDefault()
                                     let authorizationObj = encounterObj != null ? GetAuthorizationByEncounterId(Convert.ToString(encounterObj.EncounterID)) : null
                                     select new PatientInfoCustomModel
                                     {
                                         PatientInfo = patientInfo,
                                         IsEncounterExist = encounterObj != null,
                                         IsAuthorizationExist = encounterObj != null && authorizationObj != null,
                                         PersonAge = patientInfo != null && patientInfo.PersonBirthDate.HasValue ? CalculatePersonAge(patientInfo.PersonBirthDate.Value, GetInvariantCultureDateTime(Convert.ToInt32(patientInfo.FacilityId))) : 0,
                                         FacilityName = GetFacilityNameByFacilityId(Convert.ToInt32(patientInfo.FacilityId))
                                     });

            return returnCustomLst.ToList();
        }

        public List<PatientInfo> GetPatientsByFacilityId(int facilityId)
        {
            var patientInfo = new List<PatientInfo>();
            patientInfo = _repository.Where(
                        p => p.FacilityId == facilityId).ToList();
            return patientInfo;
        }

        #region Scheduler Overview Methods

        /// <summary>
        /// Method is used to get the patient name
        /// </summary>
        /// <returns></returns>
        public List<PatientInfo> GetPatientNames(int fId, int cId)
        {
            List<PatientInfo> patientInfo;
            patientInfo = _repository.Where(p => p.CorporateId == cId && p.FacilityId == fId).ToList();

            return patientInfo;
        }

        #endregion


        public PatientInfoViewData GetPatientInfoOnLoad(long patientId, long encounterId)
        {
            var sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("PId", patientId);
            sqlParameters[1] = new SqlParameter("EId", encounterId);

            using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetPatientInfoView.ToString(), parameters: sqlParameters, isCompiled: false))
            {
                var vm = new PatientInfoViewData();

                //Result Set 1
                var patientInfo = r.ResultSetFor<PatientInfo>().FirstOrDefault();               //Result Set 1 i.e. Main Patient Info
                vm.PatientInfo = r.ResultSetFor<PatientInfoCustomModel>().FirstOrDefault();     //Result Set 2  i.e. Patient Info

                if (vm.PatientInfo != null)
                {
                    vm.PatientInfo.PatientInfo = patientInfo;
                    vm.PatientInfo.PatientIsVIP = !string.IsNullOrEmpty(patientInfo.PersonVIP);
                }

                var insurances = r.ResultSetFor<PatientInsuranceCustomModel>().ToList();        //Result Set 3 i.e. PatientInsuranceCustomModel
                if (insurances.Any())
                {
                    vm.PatientInsurance = insurances.FirstOrDefault(a => a.IsPrimary.HasValue && a.IsPrimary.Value);
                    if (insurances.Count > 1 && insurances.Any(a => !a.IsPrimary.HasValue || !a.IsPrimary.Value))
                    {
                        var ins = insurances.FirstOrDefault(a => !a.IsPrimary.HasValue || !a.IsPrimary.Value);
                        vm.PatientInsurance.CompanyId2 = ins.InsuranceCompanyId;
                        vm.PatientInsurance.Plan2 = ins.InsurancePlanId;
                        vm.PatientInsurance.Policy2 = ins.InsurancePolicyId;
                        vm.PatientInsurance.StartDate2 = ins.Startdate;
                        vm.PatientInsurance.EndDate2 = ins.Expirydate;
                        vm.PatientInsurance.PatientInsuranceId2 = ins.PatientInsuraceID;
                        vm.PatientInsurance.PersonHealthCareNumber2 = ins.PersonHealthCareNumber;
                    }
                    else
                    {
                        var now = DateTime.Now;
                        vm.PatientInsurance.StartDate2 = new DateTime(now.Year, now.Month, 1);
                        vm.PatientInsurance.EndDate2 = new DateTime(now.Year, now.Month, 1).AddMonths(1).AddDays(-1);
                    }
                }
                vm.PatientLoginInfo = r.ResultSetFor<PatientLoginDetailCustomModel>().FirstOrDefault();         //Result Set 4 i.e. PatientLoginDetailCustomModel
                vm.PatientPhone = r.ResultSetFor<PatientPhone>().FirstOrDefault();                              //Result Set 5 i.e. PatientPhone
                vm.EncounterOpen = r.ResultSetFor<bool>().FirstOrDefault();                                     //Result Set 6 i.e. EncounterOpen
                return vm;
            }
        }

        public List<PatientInfoCustomModel> GetPatientSearchResultAndOtherData(CommonModel m)
        {
            var sqlParameters = new SqlParameter[10];
            sqlParameters[0] = new SqlParameter("pUserId", m.UserId);
            sqlParameters[1] = new SqlParameter("pLastName", !string.IsNullOrEmpty(m.PersonLastName) ? m.PersonLastName : string.Empty);
            sqlParameters[2] = new SqlParameter("pPassport", !string.IsNullOrEmpty(m.PersonPassportNumber) ? m.PersonPassportNumber : string.Empty);
            sqlParameters[3] = new SqlParameter("pMobilePhone", !string.IsNullOrEmpty(m.ContactMobilePhone) ? m.ContactMobilePhone : string.Empty);
            sqlParameters[4] = new SqlParameter("pBirthDate", SqlDbType.DateTime)
            {
                Value = m.PersonBirthDate == null ? (object)DBNull.Value : m.PersonBirthDate
            };
            sqlParameters[5] = new SqlParameter("pSSN", !string.IsNullOrEmpty(m.PersonEmiratesIDNumber) ? m.PersonEmiratesIDNumber : string.Empty);
            sqlParameters[6] = new SqlParameter("pWithAccessedRoles", m.ShowAccessedTabs);
            sqlParameters[7] = new SqlParameter("pFId", m.FacilityId);
            sqlParameters[8] = new SqlParameter("pCId", m.CorporateId);
            sqlParameters[9] = new SqlParameter("pRoleId", m.RoleKey);

            using (var ms = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetPatientSearchResultAndOtherData.ToString(), parameters: sqlParameters, isCompiled: false))
            {
                var mList = ms.GetResultWithJson<PatientInfo>(JsonResultsArray.PatientInfo.ToString());

                if (mList.Any())
                {
                    var list = ms.GetResultWithJson<PatientInfoCustomModel>(JsonResultsArray.PatientSearchResults.ToString());

                    foreach (var item in list)
                        item.PatientInfo = mList.Where(p => p.PatientID == item.Id).FirstOrDefault();
                    return list;
                }
            }
            return new List<PatientInfoCustomModel>();
        }


        public async Task<ResponseData> SavePatientInfo(PatientInfo patientInfo, PatientInsuranceCustomModel ins, PatientAddressRelation a
            , int userId, DateTime currentDate, string tokenId, string codeValue, List<DocumentsTemplates> docs)
        {
            var dtDocsData = ExtensionMethods.CreateCommonDatatable();

            foreach (var item in docs)
                dtDocsData.Rows.Add(new object[] { 0, item.DocumentTypeID, string.Empty, item.DocumentName, string.Empty, item.FilePath
                        , string.Empty, string.Empty });

            if (a == null)
                a = new PatientAddressRelation();

            if (ins.Startdate.Year == 1 || ins.Expirydate.Year == 1)
            {
                var dtNow = DateTime.Now;
                ins.Startdate = new DateTime(dtNow.Year, dtNow.Month, 1);
                ins.Expirydate = ins.Startdate.AddMonths(1).AddDays(-1);
            }

            SqlParameter[] sqlParams = new SqlParameter[47]
                {
                    new SqlParameter("@pPatientId", patientInfo.PatientID),
                    new SqlParameter("@pFId", patientInfo.FacilityId.GetValueOrDefault()),
                    new SqlParameter("@pUserId", userId),
                    new SqlParameter("@pCurrentDate", currentDate),
                    new SqlParameter("@pCId", patientInfo.CorporateId.GetValueOrDefault()),
                    new SqlParameter("@pInsCompanyId", ins.InsuranceCompanyId),
                    new SqlParameter("@pInsurancePlanId", ins.InsurancePlanId),
                    new SqlParameter("@pMemberId", !string.IsNullOrEmpty(ins.PersonHealthCareNumber)? ins.PersonHealthCareNumber:string.Empty),
                    new SqlParameter("@pEmail", !string.IsNullOrEmpty(patientInfo.PersonEmailAddress)? patientInfo.PersonEmailAddress:string.Empty),
                    new SqlParameter("@pSSN", !string.IsNullOrEmpty(patientInfo.PersonEmiratesIDNumber)? patientInfo.PersonEmiratesIDNumber:string.Empty),
                    new SqlParameter("@pMRN", !string.IsNullOrEmpty(patientInfo.PersonMedicalRecordNumber)? patientInfo.PersonMedicalRecordNumber:string.Empty),
                    new SqlParameter("@pFinanceNo", !string.IsNullOrEmpty(patientInfo.PersonFinancialNumber)? patientInfo.PersonFinancialNumber:string.Empty),
                    new SqlParameter("@pMasterPatientNo", !string.IsNullOrEmpty(patientInfo.PersonMasterPatientNumber)? patientInfo.PersonMasterPatientNumber:string.Empty),
                    new SqlParameter("@pPassportNo", !string.IsNullOrEmpty(patientInfo.PersonPassportNumber)? patientInfo.PersonPassportNumber:string.Empty),
                    new SqlParameter("@pPassportExpiry", patientInfo.PersonPassportExpirtyDate==null ? DateTime.Now.AddDays(-10) :patientInfo.PersonPassportExpirtyDate.GetValueOrDefault()),
                    new SqlParameter("@pLastName", !string.IsNullOrEmpty(patientInfo.PersonLastName)? patientInfo.PersonLastName:string.Empty),
                    new SqlParameter("@pFirstName", !string.IsNullOrEmpty(patientInfo.PersonFirstName)? patientInfo.PersonFirstName:string.Empty),
                    new SqlParameter("@pNationality", !string.IsNullOrEmpty(patientInfo.PersonNationality)? patientInfo.PersonNationality:"199"),
                    new SqlParameter("@pVipStatus", !string.IsNullOrEmpty(patientInfo.PersonVIP)? patientInfo.PersonVIP:string.Empty),
                    new SqlParameter("@pGender", !string.IsNullOrEmpty(patientInfo.PersonGender)?patientInfo.PersonGender:string.Empty),
                    new SqlParameter("@pContactNo", !string.IsNullOrEmpty(patientInfo.PersonContactNumber)?patientInfo.PersonContactNumber:string.Empty),
                    new SqlParameter("@pDOB", patientInfo.PersonBirthDate.GetValueOrDefault()),
                    new SqlParameter("@pAgeInYears", patientInfo.PersonAge.GetValueOrDefault()),
                    new SqlParameter("@pMaritalStatus", !string.IsNullOrEmpty(patientInfo.PersonMaritalStatus) ? patientInfo.PersonMaritalStatus:string.Empty),
                    new SqlParameter("@pInsuranceStart", ins.Startdate),
                    new SqlParameter("@pInsuranceEnd", ins.Expirydate),
                    new SqlParameter("@pInsurancePolicyId", ins.InsurancePolicyId),
                    new SqlParameter("@pInsuranceCompanyId2", ins.CompanyId2),
                    new SqlParameter("@pInsurancePlanId2", ins.Plan2),
                    new SqlParameter("@pMemberId2", !string.IsNullOrEmpty(ins.PersonHealthCareNumber2)? ins.PersonHealthCareNumber2:string.Empty),
                    new SqlParameter("@pInsuranceStart2", ins.Policy2 <=0 ? DateTime.Now : ins.StartDate2),
                    new SqlParameter("@pInsuranceEnd2", ins.Policy2 <=0 ? DateTime.Now :ins.EndDate2),
                    new SqlParameter("@pInsurancePolicyId2", ins.Policy2),
                    new SqlParameter {
                                        ParameterName = "@pDataArray",
                                        SqlDbType = SqlDbType.Structured,
                                        Value = dtDocsData,
                                        TypeName = "ValuesArrayT"
                                    },
                    new SqlParameter("@pStreetAddress1", !string.IsNullOrEmpty(a.StreetAddress1)?a.StreetAddress1:string.Empty),
                    new SqlParameter("@pStreetAddress2", !string.IsNullOrEmpty(a.StreetAddress2)?a.StreetAddress2:string.Empty),
                    new SqlParameter("@pPOBox", string.IsNullOrEmpty(a.POBox) ? string.Empty : a.POBox),
                    new SqlParameter("@pCityId", a.CityID),
                    new SqlParameter("@pStateId", a.StateID),
                    new SqlParameter("@pCountryId", a.CountryID),
                    new SqlParameter("@pAddressFirstName", string.IsNullOrEmpty(a.FirstName) ? string.Empty : a.FirstName),
                    new SqlParameter("@pAddressLastName", string.IsNullOrEmpty(a.LastName) ? string.Empty : a.LastName),
                    new SqlParameter("@ZipCode", string.IsNullOrEmpty(a.ZipCode) ? string.Empty : a.ZipCode),
                    new SqlParameter("@pRelationType", a.PatientAddressRelationType),
                    new SqlParameter("@pToken", !string.IsNullOrEmpty(tokenId)? tokenId:string.Empty),
                    new SqlParameter("@pCode", !string.IsNullOrEmpty(codeValue)? codeValue:string.Empty),
                    new SqlParameter("@pEmiratesIDExpiry", patientInfo.PersonEmiratesIDExpiration==null ? DateTime.Now.AddDays(-10) : patientInfo.PersonEmiratesIDExpiration)
                };

            using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SprocSavePatientInfo.ToString(), isCompiled: false, parameters: sqlParams))
            {
                try
                {
                    var result = await r.SingleResultSetAsync<ResponseData>();
                    return result;
                }
                catch (Exception ex)
                {
                    if ((ins.Startdate != null && ins.Startdate.Year == 1)
                        || (ins.Expirydate != null && ins.Expirydate.Year == 1))
                        return new ResponseData { Message = "Insurance Dates are not valid", Status = -3 };


                    return new ResponseData { Message = "Error in the execution of the Save Patient Info API", Status = 0 };
                }
            }

        }

        public PatientInfoCustomModel GetPatientDetailsByPatientId(int patientId, int encounterId = 0, bool showEncounters = false)
        {
            var vm = new PatientInfoCustomModel();
            var sqlParameters = new SqlParameter[3];
            sqlParameters[0] = new SqlParameter("PId", patientId);
            sqlParameters[1] = new SqlParameter("EId", encounterId);
            sqlParameters[2] = new SqlParameter("ShowEncounters", showEncounters);

            using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetPatientDetails.ToString(), parameters: sqlParameters, isCompiled: false))
            {
                var patientInfo = r.ResultSetFor<PatientInfo>().FirstOrDefault();
                vm = r.ResultSetFor<PatientInfoCustomModel>().FirstOrDefault();

                if (vm != null)
                {
                    vm.PatientInfo = patientInfo;

                    if (showEncounters)
                        vm.CurrentEncounter = r.ResultSetFor<Encounter>().FirstOrDefault();
                }
            }
            return vm;

        }
        public int GetPatientIdByEncounterId(int encounterId)
        {
            if (encounterId > 0)
            {
                var en = _eRepository.Where(e => e.EncounterID == encounterId).FirstOrDefault();
                return en != null ? Convert.ToInt32(en.PatientID) : 0;
            }
            return 0;
        }
    }
}
