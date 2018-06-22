using System;
using BillingSystem.Common;
using BillingSystem.Model;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Model.CustomModel;
using BillingSystem.Common.Common;

namespace BillingSystem.Bal.BusinessAccess
{
    using BillingSystem.Model.EntityDto;
    using BillingSystem.Repository.Common;
    using System.Data.Entity;
    using System.Threading.Tasks;

    public class PatientInfoBal : BaseBal
    {
        public PatientInfoBal()
        {
        }

        public PatientInfoBal(string cptTableNumber, string serviceCodeTableNumber, string drgTableNumber, string drugTableNumber, string hcpcsTableNumber, string diagnosisTableNumber)
        {
            if (!string.IsNullOrEmpty(cptTableNumber))
                CptTableNumber = cptTableNumber;

            if (!string.IsNullOrEmpty(serviceCodeTableNumber))
                ServiceCodeTableNumber = serviceCodeTableNumber;

            if (!string.IsNullOrEmpty(drgTableNumber))
                DrgTableNumber = drgTableNumber;

            if (!string.IsNullOrEmpty(drugTableNumber))
                DrugTableNumber = drugTableNumber;

            if (!string.IsNullOrEmpty(hcpcsTableNumber))
                HcpcsTableNumber = hcpcsTableNumber;

            if (!string.IsNullOrEmpty(diagnosisTableNumber))
                DiagnosisTableNumber = diagnosisTableNumber;
        }

        /// <summary>
        /// Get the patient Info by Id
        /// </summary>
        /// <param name="patientId"></param>
        public PatientInfo GetPatientInfoById(int? patientId)
        {
            if (patientId > 0)
            {
                using (var patientInfoRep = UnitOfWork.PatientInfoRepository)
                {
                    var patientInfo = patientInfoRep.Where(x => x.PatientID == patientId && x.IsDeleted != true).FirstOrDefault();
                    return patientInfo;
                }
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
            using (var patientInfoRep = UnitOfWork.PatientInfoRepository)
            {
                var model = patientInfoRep.Where(x => x.PatientID == patientId && x.IsDeleted == false).FirstOrDefault();
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
                    using (var docBal = new DocumentsTemplatesBal())
                    {
                        var profileImage = docBal.GetDocumentByTypeAndPatientId(Convert.ToInt32(AttachmentType.ProfilePicture), model.PatientID);
                        if (profileImage != null)
                        {
                            vm.ProfilePicImagePath = profileImage.FilePath;
                            vm.DocumentTemplateId = profileImage.DocumentsTemplatesID;
                        }
                        else
                            vm.ProfilePicImagePath = "/images/BlankProfilePic.png";

                    }
                }
                else
                {
                    vm.PatientIsVIP = false;
                    vm.PatientMaritalStatus = false;
                    vm.PatientSex = false;
                }
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
            using (var rep = UnitOfWork.PatientInfoRepository)
            {
                if (patientInfo.PatientID > 0)
                {
                    var current = rep.GetSingle(patientInfo.PatientID);
                    patientInfo.CreatedBy = current.CreatedBy;
                    patientInfo.CreatedDate = current.CreatedDate;
                    patientInfo.CorporateId = current.CorporateId;
                    patientInfo.FacilityId = current.FacilityId;
                    patientInfo.IsDeleted = current.IsDeleted;
                    patientInfo.PersonType = current.PersonType;
                    patientInfo.PersonMedicalRecordNumber = !string.IsNullOrEmpty(patientInfo.PersonMedicalRecordNumber)
                        ? patientInfo.PersonMedicalRecordNumber : current.PersonMedicalRecordNumber;
                    rep.UpdateEntity(patientInfo, patientInfo.PatientID);
                }
                else
                {
                    rep.Create(patientInfo);
                    if (patientInfo.PatientID > 0)
                        SaveMaxMedicalRecordNumber(patientInfo.PersonMedicalRecordNumber);
                }
                return patientInfo.PatientID;
            }
        }


        /// <summary>
        /// Get the Patinet Serach value
        /// </summary>
        /// <returns>Return the user after login</returns>
        public List<PatientInfoCustomModel> GetPatientSearchResult(CommonModel vm)
        {
            var patientSearchResults = new List<PatientInfo>();
            var returnCustomLst = new List<PatientInfoCustomModel>();
            using (var rep = UnitOfWork.PatientInfoRepository)
            {
                //Remove the check based on Facility Id Search
                if (!string.IsNullOrEmpty(vm.PersonLastName))
                    patientSearchResults.AddRange(
                        rep.Where(
                            p => p.FacilityId == vm.FacilityId &&
                                 !string.IsNullOrEmpty(p.PersonLastName) &&
                                 p.PersonLastName.ToLower().Contains(vm.PersonLastName.ToLower())));

                if (!string.IsNullOrEmpty(vm.PersonPassportNumber))
                    patientSearchResults.AddRange(
                        rep.Where(
                            p =>
                                !string.IsNullOrEmpty(p.PersonPassportNumber) &&
                                p.FacilityId == vm.FacilityId &&
                                p.PersonPassportNumber.ToLower()
                                    .Contains(vm.PersonPassportNumber.ToLower())));


                if (!string.IsNullOrEmpty(vm.PersonEmiratesIDNumber))
                    patientSearchResults.AddRange(
                        rep.Where(
                            p =>
                                !string.IsNullOrEmpty(p.PersonEmiratesIDNumber) &&
                                p.FacilityId == vm.FacilityId &&
                                p.PersonEmiratesIDNumber.ToLower()
                                    .Contains(vm.PersonEmiratesIDNumber.ToLower())));

                if (vm.PersonBirthDate.HasValue)
                    patientSearchResults.AddRange(
                        rep.Where(
                            p =>
                                p.PersonBirthDate.HasValue && p.FacilityId == vm.FacilityId &&
                                p.PersonBirthDate.Value.Day == vm.PersonBirthDate.Value.Day &&
                                p.PersonBirthDate.Value.Month == vm.PersonBirthDate.Value.Month &&
                                p.PersonBirthDate.Value.Year == vm.PersonBirthDate.Value.Year
                            ));

                if (!string.IsNullOrEmpty(vm.ContactMobilePhone))
                    patientSearchResults.AddRange(rep.Where(
                        p => p.FacilityId == vm.FacilityId &&
                             !string.IsNullOrEmpty(p.PersonContactNumber) &&
                             p.PersonContactNumber.ToLower()
                                 .Contains(vm.ContactMobilePhone.ToLower())));


                patientSearchResults = patientSearchResults.Distinct().ToList();

                var authorizationBal = new AuthorizationBal();
                var encounterBal = new EncounterBal();
                returnCustomLst.AddRange(from patientInfo in patientSearchResults
                                         let encounterObj = encounterBal.GetActiveEncounterByPateintId(patientInfo.PatientID)
                                         let authorizationObj = encounterObj != null ? authorizationBal.GetAuthorizationByEncounterId(Convert.ToString(encounterObj.EncounterID)) : null
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
            using (var encounterRep = UnitOfWork.EncounterRepository)
            {
                isActiveEncounter =
                    encounterRep.Where(
                        _ =>
                            _.PatientID == patientidint && _.EncounterType == encountertypeint &&
                            _.EncounterEndTime == null).Any();
            }
            return isActiveEncounter;
        }

        /// <summary>
        /// Checks if birth date exists.
        /// </summary>
        /// <param name="birthDate">The birth date.</param>
        /// <returns></returns>
        public bool CheckIfBirthDateExists(DateTime birthDate)
        {
            using (var rep = UnitOfWork.PatientInfoRepository)
            {
                var isExists = rep.GetAll().Any(x => DateTime.Compare(x.PersonBirthDate.Value.Date, birthDate.Date) == 0);
                return isExists;
            }
        }

        /// <summary>
        /// Checks if passport exists.
        /// </summary>
        /// <param name="passportNumber">The passport number.</param>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public bool CheckIfPassportExists(string passportNumber, int patientId)
        {
            using (var rep = UnitOfWork.PatientInfoRepository)
            {
                var isExists = patientId > 0
                    ? rep.Where(x => x.PersonPassportNumber.Equals(passportNumber) && x.PatientID != patientId).Any()
                    : rep.Where(x => x.PersonPassportNumber.Equals(passportNumber)).Any();
                return isExists;
            }
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
            //const string firstEmirate = "111-1111-1111111-1";
            //const string secondEmirate = "999-9999-9999999-9";
            //if (emiratesId == null)
            //{
            //    emiratesId = firstEmirate;
            //}

            //if (emiratesId.Equals(firstEmirate) || emiratesId.Equals(secondEmirate))
            //    return false;
            if (emiratesId != null)
            {
                using (var rep = UnitOfWork.PatientInfoRepository)
                {
                    //var isExists = patientId > 0 ? rep.Where(x => x.PersonEmiratesIDNumber.Equals(emiratesId) && x.PatientID != patientId && x.PersonLastName.Contains(Lastname) && x.PersonBirthDate == BirthDate).Any() : rep.Where(x => x.PersonEmiratesIDNumber.Equals(emiratesId) && x.PersonLastName.Contains(Lastname) && x.PersonBirthDate.Value.Year == BirthDate.Year && x.PersonBirthDate.Value.Month == BirthDate.Month && x.PersonBirthDate.Value.Day == BirthDate.Day).Any();
                    var isExists = patientId > 0
                        ? rep.Where(
                            x =>
                                x.PersonEmiratesIDNumber.Equals(emiratesId) && x.PatientID != patientId &&
                                x.PersonLastName.Contains(lastname) && x.FacilityId == facilityId &&
                                x.PersonBirthDate.Value.Year == bDate.Year &&
                                x.PersonBirthDate.Value.Month == bDate.Month &&
                                x.PersonBirthDate.Value.Day == bDate.Day).Any()
                        : rep.Where(
                            x => x.PersonEmiratesIDNumber.Equals(emiratesId) && x.PersonLastName.Contains(lastname) &&
                                 x.PersonBirthDate.Value.Year == bDate.Year && x.FacilityId == facilityId &&
                                 x.PersonBirthDate.Value.Month == bDate.Month &&
                                 x.PersonBirthDate.Value.Day == bDate.Day).Any();
                    return isExists;
                }
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
            using (var rep = UnitOfWork.PatientInfoRepository)
            {
                var patientInfo = rep.Where(p => p.PersonMedicalRecordNumber.Equals(medicalRecordNumber)).FirstOrDefault();
                return patientInfo != null;
            }
        }

        //public int GetAutoGenerateMedicalNumber()
        //{
        //    using (var rep = UnitOfWork.PatientInfoRepository)
        //    {
        //        var maxNumber = rep.GetAll().Max(x => x.PersonMedicalRecordNumber);
        //        return Convert.ToInt32(maxNumber) + 1;
        //    }
        //}

        //function to get auto generate medical number
        public int GetMaxMedicalRecordNumber()
        {
            using (var rep = UnitOfWork.MaxValuesRepository)
            {
                var key = Convert.ToInt32(MaxValueKeys.MedicalRecordNumber);
                var max = rep.Where(x => x.AssociatedKey == key).FirstOrDefault();
                return max != null ? Convert.ToInt32(max.MaxValue) : 1;
            }
        }

        /// <summary>
        /// Save max medical record number in the table MaxValues
        /// </summary>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        void SaveMaxMedicalRecordNumber(string maxValue)
        {
            using (var rep = UnitOfWork.MaxValuesRepository)
            {
                var key = Convert.ToInt32(MaxValueKeys.MedicalRecordNumber);
                var max = rep.Where(x => x.AssociatedKey == key).FirstOrDefault();
                if (max == null)
                {
                    var newRecord = new MaxValues
                    {
                        AssociatedKey = Convert.ToInt32(MaxValueKeys.MedicalRecordNumber),
                        MaxValue = "1000000001"
                    };
                    rep.Create(newRecord);
                    return;
                }
                if (!string.IsNullOrEmpty(maxValue) && Convert.ToInt32(maxValue) == Convert.ToInt32(max.MaxValue))
                {
                    max.MaxValue = Convert.ToString(Convert.ToInt32(maxValue) + 1);
                    rep.UpdateEntity(max, max.Id);
                }
            }
        }

        /// <summary>
        /// Gets the patient list.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public List<PatientInfo> GetPatientList(int facilityId)
        {
            using (var rep = UnitOfWork.PatientInfoRepository)
            {
                var result = rep.Where(p => p.FacilityId == facilityId).OrderBy(p => p.PersonFirstName).ToList();
                return result;
            }
        }



        public bool CheckForDuplicateHealthCareNumber(string memberId, int patientId, int insCompanyId, int insPlanId, int insPolicyId)
        {
            if (string.IsNullOrEmpty(memberId))
                return false;

            using (var rep = UnitOfWork.PatientInsuranceRepository)
            {
                var isExists = patientId > 0
                    ? rep.Where(x =>
                        x.PatientID != patientId && x.PersonHealthCareNumber == memberId &&
                        x.InsuranceCompanyId == insCompanyId && x.InsurancePlanId == insPlanId).Any()
                    : rep.Where(
                        x =>
                            x.PersonHealthCareNumber == memberId && x.InsuranceCompanyId == insCompanyId &&
                            x.InsurancePlanId == insPlanId).Any();
                return isExists;
            }
        }

        /// <summary>
        /// Checks if email exists.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        public bool CheckIfEmailExists(string email)
        {
            using (var rep = UnitOfWork.PatientLoginDetailRepository)
            {
                var isExists = rep.Where(x => x.Email.Equals(email)).Any();
                return isExists;
            }
        }

        /// <summary>
        /// Gets the patient detail by emailid.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        public PatientInfo GetPatientDetailByEmailid(string email)
        {
            using (var rep = UnitOfWork.PatientInfoRepository)
            {
                using (var repemail = UnitOfWork.PatientLoginDetailRepository)
                {
                    var emailexist = repemail.Where(x => x.Email.Equals(email)).FirstOrDefault();
                    var patientdata = rep.Where(x => x.PatientID == emailexist.PatientId).FirstOrDefault();
                    return patientdata;
                }
            }
        }

        /// <summary>
        /// Checks for duplicate email.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public bool CheckForDuplicateEmail(string email, int patientId)
        {
            using (var rep = UnitOfWork.PatientLoginDetailRepository)
            {
                email = email.ToLower().Trim();

                var isExists = patientId > 0
                    ? rep.Where(x =>
                        x.PatientId != patientId && x.Email.ToLower().Trim().Equals(email)).Any()
                    : rep.Where(
                        x => x.Email.ToLower().Trim().Equals(email)).Any();
                return isExists;
            }
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
            IEnumerable<PatientInfo> patientInfo = null;
            using (var patientInfoRep = UnitOfWork.PatientInfoRepository)
                patientInfo = patientInfoRep.GetPatientSearchResults(facilityId, corporateId, patientName);
            return patientInfo;
        }


        public int GetNextPatientId()
        {
            using (var rep = UnitOfWork.PatientInfoRepository)
            {
                var nextId = rep.GetAll().Select(p => p.PatientID).Max();
                return nextId + 1;
            }
        }



        /// <summary>
        /// Gets the patient search result in payment.
        /// </summary>
        /// <param name="common">The common.</param>
        /// <returns></returns>
        public List<PatientInfoXReturnPaymentCustomModel> GetPatientSearchResultInPayment(CommonModel common)
        {
            using (var rep = UnitOfWork.PatientInfoRepository)
            {
                var patientInfoLst =
                    rep.GetPatientEncounterDetail(common.CorporateId, common.FacilityId);
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
        }


        public PatientInfoCustomModel PatientDetailsByPatientIdForDropdown(int patientId)
        {
            using (var pRep = UnitOfWork.PatientInfoRepository)
            {
                var current = pRep.Where(x => x.PatientID == patientId && x.PersonGender.Trim().ToLower().Equals("female")).FirstOrDefault();
                var patientinfoCustomModel = new PatientInfoCustomModel
                {
                    PatientInfo = current
                };
                return patientinfoCustomModel;
            }
        }

        /// <summary>
        /// Patients the information for scheduling by patient.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public RegisterPatientCustomModel PatientInfoForSchedulingByPatient(int patientId)
        {
            using (var pRep = UnitOfWork.PatientInfoRepository)
            {
                var current = pRep.Where(x => x.PatientID == patientId).FirstOrDefault();
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
        }

        public List<PatientInfoCustomModel> GetPatientSearchResultByCId(CommonModel patientSearch)
        {
            var returnLst = new List<PatientInfo>();
            var returnCustomLst = new List<PatientInfoCustomModel>();
            using (var rep = UnitOfWork.PatientInfoRepository)
            {
                if (!string.IsNullOrEmpty(patientSearch.PersonLastName))
                    returnLst.AddRange(
                        rep.Where(
                            p => p.CorporateId == patientSearch.CorporateId &&
                                 !string.IsNullOrEmpty(p.PersonLastName) &&
                                 p.PersonLastName.ToLower().Contains(patientSearch.PersonLastName.ToLower())));

                if (!string.IsNullOrEmpty(patientSearch.PersonPassportNumber))
                    returnLst.AddRange(
                        rep.Where(
                            p =>
                                !string.IsNullOrEmpty(p.PersonPassportNumber) &&
                                p.CorporateId == patientSearch.CorporateId &&
                                p.PersonPassportNumber.ToLower()
                                    .Contains(patientSearch.PersonPassportNumber.ToLower())));


                if (!string.IsNullOrEmpty(patientSearch.PersonEmiratesIDNumber))
                    returnLst.AddRange(
                        rep.Where(
                            p =>
                                !string.IsNullOrEmpty(p.PersonEmiratesIDNumber) &&
                                p.CorporateId == patientSearch.CorporateId &&
                                p.PersonEmiratesIDNumber.ToLower()
                                    .Contains(patientSearch.PersonEmiratesIDNumber.ToLower())));

                if (patientSearch.PersonBirthDate.HasValue)
                    returnLst.AddRange(
                        rep.Where(
                            p =>
                                p.PersonBirthDate.HasValue && p.CorporateId == patientSearch.CorporateId &&
                                p.PersonBirthDate.Value.Day == patientSearch.PersonBirthDate.Value.Day &&
                                p.PersonBirthDate.Value.Month == patientSearch.PersonBirthDate.Value.Month &&
                                p.PersonBirthDate.Value.Year == patientSearch.PersonBirthDate.Value.Year
                            ));

                if (!string.IsNullOrEmpty(patientSearch.ContactMobilePhone))
                    returnLst.AddRange(rep.Where(
                        p => p.CorporateId == patientSearch.CorporateId &&
                             !string.IsNullOrEmpty(p.PersonContactNumber) &&
                             p.PersonContactNumber.ToLower()
                                 .Contains(patientSearch.ContactMobilePhone.ToLower())));

                returnLst = returnLst.Distinct().ToList();
                var authorizationBal = new AuthorizationBal();
                var encounterBal = new EncounterBal();
                returnCustomLst.AddRange(from patientInfo in returnLst
                                         let encounterObj = encounterBal.GetActiveEncounterByPateintId(patientInfo.PatientID)
                                         let authorizationObj = encounterObj != null ? authorizationBal.GetAuthorizationByEncounterId(Convert.ToString(encounterObj.EncounterID)) : null
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
        }

        public List<PatientInfo> GetPatientsByFacilityId(int facilityId)
        {
            var patientInfo = new List<PatientInfo>();
            using (var rep = UnitOfWork.PatientInfoRepository)
            {
                patientInfo = rep.Where(
                            p => p.FacilityId == facilityId).ToList();
                return patientInfo;
            }
        }

        #region Scheduler Overview Methods

        /// <summary>
        /// Method is used to get the patient name
        /// </summary>
        /// <returns></returns>
        public List<PatientInfo> GetPatientNames(int fId, int cId)
        {
            List<PatientInfo> patientInfo;
            using (var rep = UnitOfWork.PatientInfoRepository)
                patientInfo = rep.Where(p => p.CorporateId == cId && p.FacilityId == fId).ToList();

            return patientInfo;
        }

        #endregion


        public PatientInfoViewData GetPatientInfoOnLoad(long patientId, long encounterId)
        {
            using (var rep = UnitOfWork.PatientInfoRepository)
            {
                return rep.GetPatientInfoAll(encounterId, patientId);
            }
        }

        public List<PatientInfoCustomModel> GetPatientSearchResultAndOtherData(CommonModel m)
        {
            using (var rep = UnitOfWork.PatientInfoRepository)
                return rep.GetPatientSearchResultAndOtherData(m);
        }


        public async Task<ResponseData> SavePatientInfo(PatientInfo patientInfo, PatientInsuranceCustomModel ins, PatientAddressRelation a
            , int userId, DateTime currentDate, string tokenId, string codeValue, List<DocumentsTemplates> docs)
        {
            using (var rep = UnitOfWork.PatientInfoRepository)
            {
                var dtDocsData = ExtensionMethods.CreateCommonDatatable();

                foreach (var item in docs)
                    dtDocsData.Rows.Add(new object[] { 0, item.DocumentTypeID, string.Empty, item.DocumentName, string.Empty, item.FilePath
                        , string.Empty, string.Empty });

                var result = await rep.SavePatientInfo(patientInfo, ins, a, userId, currentDate, tokenId, codeValue, dtDocsData);
                return result;
            }
        }

        #region Methods Not In Use
        ///// <summary>
        ///// Gets the patient detail by encounter identifier.
        ///// </summary>
        ///// <param name="encounterId">The encounter identifier.</param>
        ///// <returns></returns>
        //public PatientInfoCustomModel GetPatientDetailByEncounterId(int encounterId)
        //{
        //    var patientId = 0;
        //    var patientInfoCustomModel = new PatientInfoCustomModel();
        //    if (encounterId > 0)
        //    {
        //        using (var patientInfoRep = UnitOfWork.PatientInfoRepository)
        //        {
        //            //Get Patient ID from Encounter table by current Encounter ID
        //            using (var enBal = new EncounterBal())
        //            {
        //                var id = enBal.GetEncounterByEncounterId(encounterId).PatientID;
        //                patientId = Convert.ToInt32(id);
        //            }

        //            //Get Patient Details
        //            var patientInfoval = patientInfoRep.Where(x => x.PatientID == patientId && x.IsDeleted != null && x.IsDeleted == false).FirstOrDefault();

        //            if (patientInfoval != null)
        //            {
        //                patientInfoCustomModel.PatientInfo = patientInfoval;
        //                patientInfoCustomModel.PatientName = string.Format("{0} {1}", patientInfoval.PersonFirstName, patientInfoval.PersonLastName);
        //                patientInfoCustomModel.PersonAge = CalculatePersonAge(patientInfoval.PersonBirthDate, DateTime.Now);
        //                patientInfoCustomModel.PatientIsVIP = !string.IsNullOrEmpty(patientInfoval.PersonVIP);
        //                patientInfoCustomModel.PatientMaritalStatus =
        //                    !string.IsNullOrEmpty(patientInfoval.PersonMaritalStatus);
        //                patientInfoCustomModel.PatientSex = !string.IsNullOrEmpty(patientInfoval.PersonGender);
        //            }
        //            else
        //            {
        //                patientInfoCustomModel.PatientIsVIP = false;
        //                patientInfoCustomModel.PatientMaritalStatus = false;
        //                patientInfoCustomModel.PatientSex = false;
        //            }
        //        }
        //    }
        //    return patientInfoCustomModel;
        //}

        ///// <summary>
        ///// Gets the patient age by patient identifier.
        ///// </summary>
        ///// <param name="patientId">The patient identifier.</param>
        ///// <returns></returns>
        //public int GetPatientAgeByPatientId(int? patientId)
        //{
        //    using (var patientInfoRep = UnitOfWork.PatientInfoRepository)
        //    {
        //        var patientInfo = patientInfoRep.Where(x => x.PatientID == patientId && x.IsDeleted != null && x.IsDeleted == false).FirstOrDefault();
        //        return patientInfo != null && patientInfo.PersonAge != null
        //            ? Convert.ToInt32(patientInfo.PersonAge)
        //            : patientInfo != null
        //                ? CalculatePersonAge(patientInfo.PersonBirthDate, DateTime.Now)
        //                : 0;
        //    }
        //}

        ///// <summary>
        ///// Gets the patient denail search result.
        ///// </summary>
        ///// <param name="patientSearch">The patient search.</param>
        ///// <returns></returns>
        //public List<PatientInfoCustomModel> GetPatientDenailSearchResult(CommonModel patientSearch)
        //{
        //    try
        //    {
        //        var returnLst = new List<PatientInfo>();
        //        var returnCustomLst = new List<PatientInfoCustomModel>();
        //        using (var patientInfoRep = UnitOfWork.PatientInfoRepository)
        //        {
        //            var list = patientInfoRep.GetAll().ToList();

        //            /*
        //            * Owner: Amit Jain
        //            * On: 20102014
        //            * Purpose: Search should be according to the logged-in user's default facility
        //            */
        //            var facilityId = patientSearch.FacilityId;
        //            if (facilityId > 0)
        //                list = list.Where(l => l.FacilityId == facilityId).ToList();

        //            if (list.Count > 0)
        //            {
        //                if (!string.IsNullOrEmpty(patientSearch.PersonLastName))
        //                    returnLst.AddRange(list.Where(p => p.PersonLastName.ToLower().Contains(patientSearch.PersonLastName.ToLower())).ToList());
        //                if (!string.IsNullOrEmpty(patientSearch.PersonPassportNumber))
        //                    returnLst.AddRange(list.Where(p => p.PersonPassportNumber.ToLower().Contains(patientSearch.PersonPassportNumber.ToLower())).ToList());
        //                if (!string.IsNullOrEmpty(patientSearch.PersonEmiratesIDNumber))
        //                    returnLst.AddRange(list.Where(p => p.PersonEmiratesIDNumber.ToLower().Contains(patientSearch.PersonEmiratesIDNumber.ToLower())).ToList());
        //                if (patientSearch.PersonBirthDate != null)
        //                    returnLst.AddRange(list.Where(p => p.PersonBirthDate == patientSearch.PersonBirthDate).ToList());
        //                if (!string.IsNullOrEmpty(patientSearch.ContactMobilePhone))
        //                    returnLst.AddRange(list.Where(p => !string.IsNullOrEmpty(p.PersonContactMobileNumber) && p.PersonContactMobileNumber.ToLower().Contains(patientSearch.ContactMobilePhone.ToLower())).ToList());
        //            }
        //            returnLst = returnLst.Distinct().ToList();
        //            foreach (var item in returnLst)
        //            {
        //                item.PersonAge = CalculatePersonAge(Convert.ToDateTime(item.PersonBirthDate), DateTime.Now);
        //            }
        //            var authorizationBal = new AuthorizationBal();
        //            var encounterBal = new EncounterBal();
        //            foreach (var patientInfo in returnLst)
        //            {
        //                var encounterObj = encounterBal.GetActiveEncounterByPateintId(patientInfo.PatientID);
        //                var authorizationObj = encounterObj != null
        //                    ? authorizationBal.GetAuthorizationByEncounterId(encounterObj.EncounterID.ToString())
        //                    : null;
        //                var patientinfoCustomModel = new PatientInfoCustomModel()
        //                {
        //                    PatientInfo = patientInfo,
        //                    IsEncounterExist = encounterObj != null,
        //                    IsAuthorizationExist = encounterObj != null && authorizationObj != null,
        //                };
        //                returnCustomLst.Add(patientinfoCustomModel);
        //            }
        //            //return returnLst.ToList();
        //            return returnCustomLst.ToList();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        #endregion
    }
}
