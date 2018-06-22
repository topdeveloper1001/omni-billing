﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using BillingSystem.Model;
using BillingSystem.Common.Common;
using BillingSystem.Model.CustomModel;
using BillingSystem.Repository.GenericRepository;
using BillingSystem.Repository.UOW;
using System.Threading.Tasks;

namespace BillingSystem.Bal.BusinessAccess
{
    public class EncounterBal : BaseBal
    {
        public EncounterBal()
        {
        }

        public EncounterBal(string cptTableNumber, string drgTableNumber, string serviceCodeTableNumber, string diagnosisTableNumber, string drugTableNumber)
        {
            CptTableNumber = cptTableNumber;
            DrgTableNumber = drgTableNumber;
            ServiceCodeTableNumber = serviceCodeTableNumber;
            DrugTableNumber = drugTableNumber;
            DiagnosisTableNumber = diagnosisTableNumber;
        }

        //Function to Get Encounter List
        /// <summary>
        /// Gets the encounter list.
        /// </summary>
        /// <returns></returns>
        private List<Encounter> GetEncounterList()
        {
            using (var encounterRep = UnitOfWork.EncounterRepository)
            {
                var lstEncounter = encounterRep.GetAll().Include(f => f.PatientInfo).Where(x => x.EncounterEndTime == null).ToList();
                return lstEncounter;
            }
        }

        //Function to Get Encounter BY Encounter ID
        /// <summary>
        /// Gets the encounter by encounter identifier.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        public Encounter GetEncounterByEncounterId(int encounterId)
        {
            using (var encounterRep = UnitOfWork.EncounterRepository)
            {
                var encounter = encounterRep.Where(x => x.EncounterID == encounterId).FirstOrDefault();
                return encounter;
            }
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
            using (var encounterRep = UnitOfWork.EncounterRepository)
            {
                var lstEncounter = encounterRep.Where(_ => _.PatientID == patientId).OrderByDescending(x => x.EncounterStartTime).ToList();
                var globalCodeBal = new GlobalCodeBal();
                var encountertypeCat =
                    Convert.ToInt32(GlobalCodeCategoryValue.EncounterType);
                var encounterPatienttypeCat =
                    Convert.ToInt32(GlobalCodeCategoryValue.EncounterPatientType);
                list.AddRange(lstEncounter.Select(item => new EncounterCustomModel
                {
                    EncounterID = item.EncounterID,
                    EncounterNumber = item.EncounterNumber,
                    EncounterStartTime = item.EncounterStartTime,
                    EncounterEndTime = item.EncounterEndTime,
                    Charges = item.Charges,
                    Payment = item.Payment,
                    PatientID = item.PatientID,
                    EncounterTypeName = globalCodeBal.GetNameByGlobalCodeValueAndCategoryValue(Convert.ToString(encountertypeCat),
                        item.EncounterType.ToString()),
                    EncounterPatientTypeName = globalCodeBal.GetNameByGlobalCodeValueAndCategoryValue(Convert.ToString(encounterPatienttypeCat),
                        item.EncounterPatientType.ToString()),
                }));
            }
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
            using (var rep = UnitOfWork.EncounterRepository)
            {
                var eEndType = e.EncounterEndType.HasValue
                                   ? e.EncounterEndType.Value
                                   : !string.IsNullOrEmpty(e.EncounterDischargePlan)
                                         ? Convert.ToInt32(e.EncounterDischargePlan)
                                         : 0;
                if (e.EncounterID > 0)
                {
                    var model = rep.Where(e1 => e1.EncounterID == e.EncounterID).FirstOrDefault();
                    if (model != null)
                    {
                        e.CreatedBy = model.CreatedBy;
                        e.CreatedDate = model.CreatedDate;
                    }
                    result = rep.UpdateEntity(e, e.EncounterID) != null ? e.EncounterID : 0;

                    /*
                     * Added by Amit Jain 
                     * Purpose: To update Bill Date with the Encounter End Date when Encounter Ends.
                     */
                    if (eEndType > 0)
                        UpdateBillDateOnEncounterEnds(e.EncounterID);
                }
                else
                    result = rep.Create(e) != null ? e.EncounterID : 0;
            }
            return result;
        }

        /// <summary>
        /// Checks the encounter number exist.
        /// </summary>
        /// <param name="uniqueNumber">The unique number.</param>
        /// <returns></returns>
        public bool CheckEncounterNumberExist(string uniqueNumber)
        {
            using (var rep = UnitOfWork.EncounterRepository)
            {
                return rep.Where(x => x.EncounterNumber.Contains(uniqueNumber)).Any();
            }

        }

        /// <summary>
        /// Gets the encounter detail by patient identifier.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public EncounterCustomModel GetEncounterDetailByPatientId(int patientId)
        {
            EncounterCustomModel vm = new EncounterCustomModel();
            using (var rep = UnitOfWork.EncounterRepository)
            {
                var encounterState = string.Empty;
                var bedMasterbal = new BedMasterBal();
                var encounter = rep.Where(x => x.PatientID == patientId).Include(p => p.PatientInfo).OrderByDescending(c => c.EncounterID).FirstOrDefault();
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
                            BedName = bedMasterbal.GetBedNameByInPatientEncounterId(Convert.ToString(encounter.EncounterID)),
                            BedId = bedMasterbal.GetBedByInPatientEncounterId(Convert.ToString(encounter.EncounterID)),
                            EncounterAttendingPhysician = encounter.EncounterAttendingPhysician,
                            EncounterPhysicianType = encounter.EncounterPhysicianType,
                            Age = CalculatePersonAge(dob, currentDateTime, Convert.ToInt32(encounter.EncounterFacility)),
                            OverrideBedType = bedMasterbal.GetOverRideBedTypeByInPatientEncounterId(Convert.ToString(encounter.EncounterID))
                        };
                    }
                    else
                    {
                        using (var patientRep = UnitOfWork.PatientInfoRepository)
                        {
                            var patientInfo = patientRep.Where(p => p.PatientID == patientId).FirstOrDefault();
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
                }
                return vm;
            }
        }

        /// <summary>
        /// Determines whether [is patient exist] [the specified patient identifier].
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public bool IsPatientExist(int patientId)
        {
            using (var rep = UnitOfWork.PatientInfoRepository)
                return rep.Where(p => p.PatientID == patientId).Any();
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

        /// <summary>
        /// Gets the encounter detail by identifier.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        public Encounter GetEncounterDetailById(Int32 encounterId)
        {
            using (var encounterRep = UnitOfWork.EncounterRepository)
            {
                var lstEncounter = encounterRep.Where(_ => _.EncounterID == encounterId).SingleOrDefault();
                return lstEncounter;
            }
        }

        /// <summary>
        /// Gets the encounter state by patient identifier.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public Encounter GetEncounterStateByPatientId(int patientId)
        {
            using (var rep = UnitOfWork.EncounterRepository)
            {
                var encounter =
                    rep.GetAll()
                        .Include(x => x.PatientInfo)
                        .ToList()
                        .Where(e => e.PatientID == patientId)
                        .OrderByDescending(x => x.EncounterStartTime)
                        .FirstOrDefault();
                return encounter;
            }
        }

        /// <summary>
        /// Gets the encounter by patient identifier and active.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public Encounter GetEncounterByPatientIdAndActive(int patientId)
        {
            using (var rep = UnitOfWork.EncounterRepository)
            {
                var encounterbyPatient =
                    rep.GetAll()
                        .Include(x => x.PatientInfo)
                        .ToList()
                        .Where(e => e.PatientID == patientId).ToList();
                var enocunter = encounterbyPatient.Any(x => x.EncounterEndTime == null) ? encounterbyPatient.FirstOrDefault(x => x.EncounterEndTime == null) :
                    encounterbyPatient.OrderByDescending(x => x.EncounterStartTime).FirstOrDefault();
                return enocunter;
            }
        }

        /// <summary>
        /// Gets the active encounters.
        /// </summary>
        /// <param name="common">The common.</param>
        /// <returns></returns>
        public List<EncounterCustomModel> GetActiveEncounters(CommonModel common)
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

            using (var mappingBedRep = UnitOfWork.MappingPatientBedRepository)
            {
                var gBal = new GlobalCodeBal();
                foreach (var item in encountersList)
                {
                    var enId = Convert.ToString(item.EncounterID);
                    var bedInfo = new EncounterCustomModel();

                    //var bedName = bedMasterBal.GetBedNameByInPatientEncounterId(enId);
                    if (item.EncounterPatientType == 2)
                        bedInfo = mappingBedRep.GetPatientBedInformation(Convert.ToInt32(item.PatientID));


                    //Check if current ENCOUNTER's primary diagnosis exists.
                    bool isPDiagnosisDone;
                    bool isDrgDone = false;
                    string primaryDiagnosis;
                    var expectedLengthofStay = "NA";
                    using (var diagnosisBal = new DiagnosisBal(DiagnosisTableNumber, DrgTableNumber))
                    {
                        var result = diagnosisBal.GetDiagnosisInfoByEncounterId(item.EncounterID);
                        isPDiagnosisDone = result != null && result.DiagnosisID > 0;
                        primaryDiagnosis = result != null ? result.DiagnosisCodeDescription : string.Empty;
                        var drgObj = diagnosisBal.GetDRGDiagnosisInfoByEncounterId(item.EncounterID);
                        if (drgObj != null)
                        {
                            isDrgDone = true;
                            var drgBal = new DRGCodesBal(DrgTableNumber);
                            var drgcodeObj = drgBal.GetDrgCodesById(Convert.ToInt32(drgObj.DRGCodeID));
                            expectedLengthofStay = drgcodeObj != null
                                ? drgcodeObj.Alos != null ? Convert.ToString(Math.Round(Convert.ToDecimal(drgcodeObj.Alos), 2, MidpointRounding.AwayFromZero)) : "NA"
                                : "NA";
                            expectedLengthofStay = IsBedAssignedLongTermCase(bedInfo.patientBedService)
                                ? "NA"
                                : expectedLengthofStay;
                        }
                    }
                    using (var authorizationbal = new AuthorizationBal())
                    {
                        var encounterAuhtorized = authorizationbal.GetAuthorizationByEncounterId(enId);
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
                            PhysicianName = authorizationbal.GetPhysicianName(Convert.ToInt32(item.EncounterAttendingPhysician)),
                            WaitingTime = GetDateDifference(Convert.ToDateTime(item.EncounterStartTime), Convert.ToInt32(item.EncounterPatientType), Convert.ToInt32(item.EncounterFacility)),
                            TriageValue = gBal.GetGlobalCodeNameByValueAndCategoryId("4952", item.Triage),
                            PatientStageName = gBal.GetGlobalCodeNameByValueAndCategoryId("4951", item.PatientState),
                            TriageSortingValue = gBal.GetGlobalCodeSotringByValueAndCategoryId("4952", item.Triage),
                        });
                        foreach (var itemobj in list)
                        {
                            itemobj.ActualMoreThanExpected = itemobj.AverageLengthofStay != "NA" &&
                                                             itemobj.ExpectedLengthofStay != "NA" && Convert.ToDecimal(itemobj.AverageLengthofStay) >
                                                             Convert.ToDecimal(itemobj.ExpectedLengthofStay);
                        }
                    }
                }
            }
            return list;
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

        /// <summary>
        /// Gets the encounter detail by encounter identifier.
        /// </summary>
        /// <param name="encounterid">The encounterid.</param>
        /// <returns></returns>
        public EncounterCustomModel GetEncounterDetailByEncounterID(int encounterid)
        {
            var vm = new EncounterCustomModel();
            using (var rep = UnitOfWork.EncounterRepository)
            {
                var encounterState = string.Empty;
                var bedMasterbal = new BedMasterBal();
                var encounter = rep.Where(x => x.EncounterID == encounterid).Include(p => p.PatientInfo).FirstOrDefault();
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
                    var globalcodeBal = new GlobalCodeBal();
                    var physicianBal = new PhysicianBal();
                    var globalCodeBal = new GlobalCodeBal();
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
                    vm.BedName = bedMasterbal.GetBedNameByInPatientEncounterId(Convert.ToString(encounter.EncounterID));
                    vm.BedId = bedMasterbal.GetBedByInPatientEncounterId(Convert.ToString(encounter.EncounterID));
                    vm.EncounterAttendingPhysician = encounter.EncounterAttendingPhysician;
                    vm.EncounterPhysicianType = encounter.EncounterPhysicianType;
                    vm.PersonMedicalRecordNumber = encounter.PatientInfo != null ? encounter.PatientInfo.PersonMedicalRecordNumber : string.Empty;
                    vm.PatientInfo = encounter.PatientInfo;
                    vm.EncounterPatientTypeName = globalcodeBal.GetNameByGlobalCodeValueAndCategoryValue(Convert.ToString((int)GlobalCodeCategoryValue.EncounterPatientType), encounter.EncounterPatientType.ToString());
                    vm.EncounterSpecialityName = globalcodeBal.GetNameByGlobalCodeValueAndCategoryValue(Convert.ToInt32(GlobalCodeCategoryValue.EncounterSpecialty).ToString(), encounter.EncounterSpecialty.ToString());
                    vm.EncounterModeOfArrivalName = globalcodeBal.GetNameByGlobalCodeValueAndCategoryValue(Convert.ToInt32(GlobalCodeCategoryValue.EncounterModeofArrival).ToString(), encounter.EncounterModeofArrival.ToString());
                    vm.EncounterServiceCategoryName = globalcodeBal.GetNameByGlobalCodeValueAndCategoryValue(Convert.ToInt32(GlobalCodeCategoryValue.EncounterServiceCategory).ToString(), encounter.EncounterServiceCategory.ToString());
                    vm.EncounterPhysicianTypeName = globalcodeBal.GetNameByGlobalCodeValueAndCategoryValue(Convert.ToInt32(GlobalCodeCategoryValue.EncounterPhysicianType).ToString(), encounter.EncounterPhysicianType.ToString());
                    vm.EncounterPhysicianName = physicianBal.GetPhysicianName(Convert.ToInt32(encounter.EncounterAttendingPhysician));
                    vm.EncounterAdmitTypeName = globalcodeBal.GetNameByGlobalCodeValueAndCategoryValue(Convert.ToInt32(GlobalCodeCategoryValue.EncounterAdmitType).ToString(), encounter.EncounterAdmitType.ToString());
                    vm.Age = CalculatePersonAge(encounter.PatientInfo.PersonBirthDate, currentDateTime, Convert.ToInt32(encounter.EncounterFacility));
                    vm.OverrideBedType = bedMasterbal.GetOverRideBedTypeByInPatientEncounterId(Convert.ToString(encounter.EncounterID));
                    vm.EncounterTypeName = globalCodeBal.GetNameByGlobalCodeValueAndCategoryValue(Convert.ToString(encountertypeCat), encounter.EncounterType.ToString());
                    vm.VirtuallyDischarge = encounter.EncounterDischargePlan != null ? "Discharge" : "";
                    vm.VirtuallyDischargeOn = encounter.EncounterDischargePlan != null ? encounter.EncounterDischargeLocation : null;

                    var insBal = new InsuranceCompanyBal();
                    if (encounter != null && encounter.PatientInfo != null && !string.IsNullOrEmpty(encounter.PatientInfo.PersonInsuranceCompany))
                    {
                        var ins = insBal.GetInsuranceDetailsByPayorId(encounter.PatientInfo.PersonInsuranceCompany);
                        if (ins != null)
                        {
                            vm.InsuranceCompanyName = ins.InsuranceCompanyName;
                            vm.InsuranceCompanyPhoneNumber = ins.InsuranceCompanyMainPhone;
                            vm.InsuranceCompanyFaxNumber = ins.InsuranceCompanyFax;
                            vm.InsuranceCompanyAddress = $"{ ins.InsuranceCompanyStreetAddress} { ins.InsuranceCompanyStreetAddress2}";
                        }
                    }
                    var authorizationBal = new AuthorizationBal();
                    vm.EncounterAuthorizationList = authorizationBal.GetAuthorizationsByEncounterId(vm.EncounterID.ToString());

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

        /// <summary>
        /// Gets the active encounter by pateint identifier.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public Encounter GetActiveEncounterByPateintId(int patientId)
        {
            try
            {
                using (var encounterRep = UnitOfWork.EncounterRepository)
                {
                    var lstEncounter = encounterRep.Where(_ => _.PatientID == patientId && _.EncounterEndTime == null)
                            .OrderByDescending(x => x.EncounterID).FirstOrDefault();
                    return lstEncounter;
                }
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
            try
            {
                using (var encounterRep = UnitOfWork.EncounterRepository)
                {
                    var lstEncounter =
                        encounterRep.Where(_ => _.PatientID == PatientId)
                            .OrderByDescending(x => x.EncounterID)
                            .FirstOrDefault();
                    return lstEncounter;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Function to Get Encounter number BY Encounter ID
        /// <summary>
        /// Gets the encounter number by encounter identifier.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        public string GetEncounterNumberByEncounterId(int encounterId)
        {
            try
            {
                using (var encounterRep = UnitOfWork.EncounterRepository)
                {
                    var encounter = encounterRep.Where(x => x.EncounterID == encounterId).FirstOrDefault();
                    return encounter != null ? encounter.EncounterNumber : string.Empty;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
            using (var rep = UnitOfWork.EncounterRepository)
            {
                var result = rep.GetEncounterChartData(facilityId, displayType, fromDate, tillDate);
                if (result != null && result.Count > 0)
                {
                    list.AddRange(result.Select(item => new EncounterExtension
                    {
                        Title = item.Title,
                        Code = item.Code,
                        XAxis = item.XAxis,
                        YAxis = item.YAxis
                    }));
                }
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
            using (var rep = UnitOfWork.EncounterRepository)
            {
                var list = rep.GetUnclosedEncounters(facilityId, corporateId).OrderBy(x => x.ErrorStatus).ThenByDescending(x => x.EncounterEndTime).ToList();
                var bal = new GlobalCodeBal();
                if (list.Count > 0)
                {
                    var globalCodeCategoryValue = Convert.ToString((int)GlobalCodeCategoryValue.EncounterPatientType);
                    list = list.Select(item =>
                    {
                        item.EncounterPatientTypeName = bal.GetNameByGlobalCodeValueAndCategoryValue(globalCodeCategoryValue, Convert.ToString(item.EncounterPatientType));
                        return item;
                    }).ToList();
                }
                return list;
            }
        }

        /// <summary>
        /// Gets the encounter end check.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        public int GetEncounterEndCheck(int encounterId, int userId)
        {
            using (var rep = UnitOfWork.EncounterRepository)
            {
                var result = rep.GetEncounterEndCheck(encounterId, userId);
                if (result.Any())
                {
                    var encounterCheckReturnStatus = result.FirstOrDefault();
                    return encounterCheckReturnStatus != null
                        ? encounterCheckReturnStatus.RetStatus
                        : 0;
                }
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
            using (var rep = UnitOfWork.MappingPatientBedRepository)
            {
                var result = rep.GetPatientBedInformationByBedId(patientId, bedId, serviceCodeValue);
                return result;
            }
        }

        /// <summary>
        /// Gets the patient bed information by patient identifier.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public EncounterCustomModel GetPatientBedInformationByPatientId(int patientId)
        {
            using (var rep = UnitOfWork.MappingPatientBedRepository)
            {
                var result = rep.GetPatientBedInformation(patientId);
                return result;
            }
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
            using (var rep = UnitOfWork.EncounterRepository)
            {
                var result = rep.GetActiveEncounterChartData(facilityId, displayType, fromDate, tillDate);
                return result;
            }

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
                using (var rep = UnitOfWork.EncounterRepository)
                {
                    var result = rep.Where(e => e.PatientID == patientId && !e.EncounterEndTime.HasValue).Any();
                    return result;
                }
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
            using (var encounterRep = UnitOfWork.EncounterRepository)
            {
                var lstEncounter = encounterRep.Where(_ => _.PatientID == patientId).OrderByDescending(x => x.EncounterStartTime).ToList();
                var globalCodeBal = new GlobalCodeBal();
                var encountertypeCat =
                    Convert.ToInt32(GlobalCodeCategoryValue.EncounterType);
                var encounterPatienttypeCat =
                    Convert.ToInt32(GlobalCodeCategoryValue.EncounterPatientType);

                list.AddRange(lstEncounter.Select(item => new EncounterCustomModel
                {
                    EncounterID = item.EncounterID,
                    EncounterNumber = item.EncounterNumber,
                    EncounterStartTime = item.EncounterStartTime,
                    EncounterEndTime = item.EncounterEndTime,
                    Charges = item.Charges,
                    Payment = item.Payment,
                    PatientID = item.PatientID,
                    EncounterTypeName =
                        globalCodeBal.GetNameByGlobalCodeValueAndCategoryValue(encountertypeCat.ToString(),
                            item.EncounterType.ToString()),
                    EncounterPatientTypeName =
                        globalCodeBal.GetNameByGlobalCodeValueAndCategoryValue(encounterPatienttypeCat.ToString(),
                            item.EncounterPatientType.ToString()),
                    BillHeaderId =
                        globalCodeBal.GetBillHeaderIdByEncounterAndPatientId(Convert.ToInt32(item.EncounterID),
                            Convert.ToInt32(item.PatientID))
                }));
            }
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

        /// <summary>
        /// Adds the bed charges for transfer patient.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <param name="currentdatetime">The currentdatetime.</param>
        /// <returns></returns>
        public bool AddBedChargesForTransferPatient(int encounterId, DateTime currentdatetime)
        {
            using (var rep = UnitOfWork.EncounterRepository)
            {
                return rep.AddBedChargesForTransferPatient(encounterId, currentdatetime);
                //rep.UpdateBillHeadersByEncounterId(encounterId);
            }
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
            using (var rep = UnitOfWork.EncounterRepository)
            {
                var resultData = rep.GetCMODashboardData(facilityId, corporateid).ToList();
                return resultData;
            }
        }


        public List<Encounter> GetEncounterByUserId(int userId)
        {
            using (var rep = UnitOfWork.EncounterRepository)
            {
                var list = rep.Where(x => x.CreatedBy == userId || x.ModifiedBy == userId).ToList();
                return list;
            }
        }

        public List<PatientEvaluationSetCustomModel> GetPreActiveEncounters(int encounterId, int patientId)
        {
            var list = new List<PatientEvaluationSetCustomModel>();
            var physicianName = string.Empty;
            var encounterNumber = string.Empty;

            using (var encounterRep = UnitOfWork.PatientEvaluationSetRepository)
            {
                var evaluationSetList = encounterRep.Where(i => i.PatientId == patientId && (i.EncounterId == encounterId || encounterId == 0)).ToList();
                if (evaluationSetList.Count > 0)
                {
                    using (var eBal = new EncounterBal())
                    {
                        var currentEncounter = eBal.GetEncounterByEncounterId(encounterId);
                        if (currentEncounter != null)
                        {
                            physicianName = eBal.GetPhysicianName(Convert.ToInt32(currentEncounter.EncounterAttendingPhysician));
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
                            CompletedBy = eBal.GetUserNameByUserId(Convert.ToInt32(item.CreatedBy))
                        }));
                    }
                }
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
            using (var rep = UnitOfWork.EncounterRepository)
            {
                if (encounterId > 0)
                {
                    var model = rep.Where(e => e.EncounterID == encounterId).FirstOrDefault();
                    if (model != null)
                        model.Triage = triageLevel;

                    rep.UpdateEntity(model, encounterId);
                    result = GetNameByGlobalCodeValue(triageLevel, Convert.ToString((int)GlobalCodeCategoryValue.PatientTriageLevels));
                }
            }
            return result;
        }
        #endregion

        public string UpdatePatitentStageInEncounter(int encounterId, string patientStage)
        {
            string result = string.Empty;
            using (var rep = UnitOfWork.EncounterRepository)
            {
                if (encounterId > 0)
                {
                    var model = rep.Where(e => e.EncounterID == encounterId).FirstOrDefault();
                    if (model != null)
                        model.PatientState = patientStage;

                    rep.UpdateEntity(model, encounterId);
                    result = GetNameByGlobalCodeValue(patientStage, Convert.ToString((int)GlobalCodeCategoryValue.PatientState));
                }
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
            using (var rep = UnitOfWork.EncounterRepository)
            {
                return rep.Where(x => x.PatientID == p).ToList();
            }

        }


        public List<PatientEvaluationSetCustomModel> GetNurseAssessmentData(int encounterId, int patientId)
        {
            var list = new List<PatientEvaluationSetCustomModel>();
            var physicianName = string.Empty;
            var encounterNumber = string.Empty;

            using (var encounterRep = UnitOfWork.PatientEvaluationSetRepository)
            {
                var evaluationSetList = encounterRep.Where(i => i.PatientId == patientId && (i.EncounterId == encounterId || encounterId == 0) && i.ExtValue2.Trim().Equals("99")).ToList();
                if (evaluationSetList.Count > 0)
                {
                    using (var eBal = new EncounterBal())
                    {
                        var currentEncounter = eBal.GetEncounterByEncounterId(encounterId);
                        if (currentEncounter != null)
                        {
                            physicianName =
                                eBal.GetPhysicianName(Convert.ToInt32(currentEncounter.EncounterAttendingPhysician));
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
                }
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
            using (var rep = UnitOfWork.EncounterRepository)
            {
                var result = rep.GetEncounterStatusBeforeUpdate(encounterId);
                if (result.Any())
                {
                    var encounterCheckReturnStatus = result.FirstOrDefault();
                    return encounterCheckReturnStatus != null
                        ? encounterCheckReturnStatus.RetStatus
                        : 0;
                }
            }
            return 0;
        }

        private void UpdateBillDateOnEncounterEnds(int encounterId)
        {
            using (var rep = UnitOfWork.EncounterRepository)
            {
                rep.UpdateBillDateOnEncounterEnds(encounterId);
            }
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
            using (var rep = UnitOfWork.EncounterRepository)
            {
                var result = rep.AddVirtualDischargeLog(encounterId, facilityId, corporateId);
                return result;
            }
        }

        /// <summary>
        /// Gets the encounter end check virtual discharge.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <param name="loggedInUserId">The logged in user identifier.</param>
        /// <returns></returns>
        public int GetEncounterEndCheckVirtualDischarge(int encounterId, int loggedInUserId)
        {
            using (var rep = UnitOfWork.EncounterRepository)
            {
                var result = rep.GetEncounterEndCheckVirtualDischarge(encounterId, loggedInUserId);
                if (result.Any())
                {
                    var encounterCheckReturnStatus = result.FirstOrDefault();
                    return encounterCheckReturnStatus != null
                        ? encounterCheckReturnStatus.RetStatus
                        : 0;
                }
            }
            return 0;
        }









        //public List<EncounterCustomModel> GetAllActiveEncounters1(string facilityNumber, List<int> patientTypes)
        //{
        //    var list = new List<EncounterCustomModel>();
        //    using (var eRep = UnitOfWork.EncounterRepository)
        //    {
        //        var encountersList =
        //            eRep.Where(
        //                x =>
        //                    x.EncounterFacility.Equals(facilityNumber) &&
        //                    patientTypes.Contains(x.EncounterPatientType.Value) && x.EncounterEndTime == null).
        //                    Include(p => p.PatientInfo)
        //                .OrderByDescending(m => m.EncounterStartTime)
        //                .ToList();

        //        using (var mappingBedRep = UnitOfWork.MappingPatientBedRepository)
        //        {
        //            var gBal = new GlobalCodeBal();
        //            foreach (var item in encountersList)
        //            {
        //                var enId = Convert.ToString(item.EncounterID);
        //                var bedInfo = new EncounterCustomModel();

        //                //var bedName = bedMasterBal.GetBedNameByInPatientEncounterId(enId);
        //                if (item.EncounterPatientType == 2)
        //                    bedInfo = mappingBedRep.GetPatientBedInformation(Convert.ToInt32(item.PatientID));


        //                //Check if current ENCOUNTER's primary diagnosis exists.
        //                bool isPDiagnosisDone;
        //                bool isDrgDone = false;
        //                string primaryDiagnosis;
        //                var expectedLengthofStay = "NA";
        //                using (var diagnosisBal = new DiagnosisBal(DiagnosisTableNumber, DrgTableNumber))
        //                {
        //                    var result = diagnosisBal.GetDiagnosisInfoByEncounterId(item.EncounterID);
        //                    isPDiagnosisDone = result != null && result.DiagnosisID > 0;
        //                    primaryDiagnosis = result != null ? result.DiagnosisCodeDescription : string.Empty;
        //                    var drgObj = diagnosisBal.GetDRGDiagnosisInfoByEncounterId(item.EncounterID);
        //                    if (drgObj != null)
        //                    {
        //                        isDrgDone = true;
        //                        var drgBal = new DRGCodesBal(DrgTableNumber);
        //                        var drgcodeObj = drgBal.GetDrgCodesById(Convert.ToInt32(drgObj.DRGCodeID));
        //                        expectedLengthofStay = drgcodeObj != null
        //                            ? drgcodeObj.Alos != null ? Convert.ToString(Math.Round(Convert.ToDecimal(drgcodeObj.Alos), 2, MidpointRounding.AwayFromZero)) : "NA"
        //                            : "NA";
        //                        expectedLengthofStay = IsBedAssignedLongTermCase(bedInfo.patientBedService)
        //                            ? "NA"
        //                            : expectedLengthofStay;
        //                    }
        //                }
        //                using (var authorizationbal = new AuthorizationBal())
        //                {
        //                    var encounterAuhtorized = authorizationbal.GetAuthorizationByEncounterId(enId);
        //                    var isEncounterauhtorized = encounterAuhtorized != null;

        //                    var en = new EncounterCustomModel
        //                    {
        //                        FirstName = item.PatientInfo.PersonFirstName,
        //                        LastName = item.PatientInfo.PersonLastName,
        //                        BirthDate = item.PatientInfo.PersonBirthDate,
        //                        PersonEmiratesIDNumber = item.PatientInfo.PersonEmiratesIDNumber,
        //                        EncounterNumber = item.EncounterNumber,
        //                        EncounterStartTime = item.EncounterStartTime,
        //                        EncounterID = item.EncounterID,
        //                        PatientID = item.PatientID,
        //                        PatientIsVIP = item.PatientInfo != null ? item.PatientInfo.PersonVIP : string.Empty,
        //                        IsPrimaryDiagnosisDone = isPDiagnosisDone,
        //                        IsEncounterAuthorized = isEncounterauhtorized,
        //                        PrimaryDiagnosisDescription = primaryDiagnosis,
        //                        patientBedService = bedInfo.patientBedService,
        //                        Room = bedInfo.Room,
        //                        DepartmentName = bedInfo.DepartmentName,
        //                        BedAssignedOn = bedInfo.BedAssignedOn,
        //                        BedRateApplicable = bedInfo.BedRateApplicable,
        //                        FloorName = bedInfo.FloorName,
        //                        BedName = bedInfo.BedName,
        //                        IsDRGExist = CheckIsLongTermService(isDrgDone, bedInfo.patientBedService),
        //                        AverageLengthofStay =
        //                            CalulateAverageLengthOfStay(Convert.ToDateTime(item.EncounterStartTime)),
        //                        ExpectedLengthofStay = expectedLengthofStay,
        //                        PhysicianName =
        //                            authorizationbal.GetPhysicianName(Convert.ToInt32(item.EncounterAttendingPhysician)),
        //                        WaitingTime =
        //                            GetDateDifference(Convert.ToDateTime(item.EncounterStartTime),
        //                                Convert.ToInt32(item.EncounterPatientType)),
        //                        TriageValue = gBal.GetGlobalCodeNameByValueAndCategoryId("4952", item.Triage),
        //                        PatientStageName = gBal.GetGlobalCodeNameByValueAndCategoryId("4951", item.PatientState),
        //                        TriageSortingValue = gBal.GetGlobalCodeSotringByValueAndCategoryId("4952", item.Triage),
        //                        EncounterPatientType = item.EncounterPatientType,
        //                        Age = Convert.ToInt32(item.PatientInfo.PersonAge),
        //                        EncounterAccidentDate = item.EncounterAccidentDate,
        //                        EncounterAccidentRelated = item.EncounterAccidentRelated,
        //                        EncounterAccidentType = item.EncounterAccidentType,
        //                        EncounterAccomodation = item.EncounterAccomodation,
        //                        EncounterAccomodationReason = item.EncounterAccomodationReason,
        //                        EncounterAccomodationRequest = item.EncounterAccomodationRequest,
        //                        EncounterAdmitReason = item.EncounterAdmitReason,
        //                        EncounterAdmitType = item.EncounterAdmitType,
        //                    };

        //                    en.ActualMoreThanExpected = en.AverageLengthofStay != "NA" &&
        //                                                en.ExpectedLengthofStay != "NA" &&
        //                                                Convert.ToDecimal(en.AverageLengthofStay) >
        //                                                Convert.ToDecimal(en.ExpectedLengthofStay);



        //                    list.Add(en);
        //                    //foreach (var itemobj in list)
        //                    //{
        //                    //    itemobj.ActualMoreThanExpected = itemobj.AverageLengthofStay != "NA" &&
        //                    //                                     itemobj.ExpectedLengthofStay != "NA" && Convert.ToDecimal(itemobj.AverageLengthofStay) >
        //                    //                                     Convert.ToDecimal(itemobj.ExpectedLengthofStay);
        //                    //}
        //                }
        //            }
        //        }
        //    }
        //    return list;
        //}




        public List<EncounterCustomModel> GetAllActiveEncounters(string facilityNumber, List<int> patientTypes)
        {
            var list = new List<EncounterCustomModel>();
            using (var fRep = UnitOfWork.EncounterRepository)
            {
                list = fRep.GetActiveEncounterData(facilityNumber);
            }
            return list;
        }


        public string GetTriageData(int encounterId)
        {
            var tValue = "999";
            using (var eRep = UnitOfWork.EncounterRepository)
            {
                var triData = eRep.Where(x => x.EncounterID == encounterId).FirstOrDefault();
                if (triData != null)
                    tValue = triData.Triage;
            }
            return tValue;
        }

        public string GetPatientStateData(int encounterId)
        {
            var sValue = "999";
            using (var eRep = UnitOfWork.EncounterRepository)
            {
                var stateData = eRep.Where(x => x.EncounterID == encounterId).FirstOrDefault();
                if (stateData != null)
                    sValue = stateData.PatientState;
            }
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
            using (var rep = UnitOfWork.EncounterRepository)
            {
                var encounterState = string.Empty;
                var bedMasterbal = new BedMasterBal();
                var encounter = rep.Where(x => x.EncounterID == encounterid).Include(p => p.PatientInfo).FirstOrDefault();
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
                    var globalcodeBal = new GlobalCodeBal();
                    var physicianBal = new PhysicianBal();
                    var globalCodeBal = new GlobalCodeBal();
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
                    vm.BedName = bedMasterbal.GetBedNameByInPatientEncounterId(Convert.ToString(encounter.EncounterID));
                    vm.BedId = bedMasterbal.GetBedByInPatientEncounterId(Convert.ToString(encounter.EncounterID));
                    vm.EncounterAttendingPhysician = encounter.EncounterAttendingPhysician;
                    vm.EncounterPhysicianType = encounter.EncounterPhysicianType;
                    vm.PersonMedicalRecordNumber = encounter.PatientInfo != null ? encounter.PatientInfo.PersonMedicalRecordNumber : string.Empty;
                    vm.PatientInfo = encounter.PatientInfo;
                    vm.EncounterPatientTypeName = globalcodeBal.GetNameByGlobalCodeValueAndCategoryValue(Convert.ToString((int)GlobalCodeCategoryValue.EncounterPatientType), encounter.EncounterPatientType.ToString());
                    vm.EncounterSpecialityName = globalcodeBal.GetNameByGlobalCodeValueAndCategoryValue(Convert.ToInt32(GlobalCodeCategoryValue.EncounterSpecialty).ToString(), encounter.EncounterSpecialty.ToString());
                    vm.EncounterModeOfArrivalName = globalcodeBal.GetNameByGlobalCodeValueAndCategoryValue(Convert.ToInt32(GlobalCodeCategoryValue.EncounterModeofArrival).ToString(), encounter.EncounterModeofArrival.ToString());
                    vm.EncounterServiceCategoryName = globalcodeBal.GetNameByGlobalCodeValueAndCategoryValue(Convert.ToInt32(GlobalCodeCategoryValue.EncounterServiceCategory).ToString(), encounter.EncounterServiceCategory.ToString());
                    vm.EncounterPhysicianTypeName = globalcodeBal.GetNameByGlobalCodeValueAndCategoryValue(Convert.ToInt32(GlobalCodeCategoryValue.EncounterPhysicianType).ToString(), encounter.EncounterPhysicianType.ToString());
                    vm.EncounterPhysicianName = physicianBal.GetPhysicianName(Convert.ToInt32(encounter.EncounterAttendingPhysician));
                    vm.EncounterAdmitTypeName = globalcodeBal.GetNameByGlobalCodeValueAndCategoryValue(Convert.ToInt32(GlobalCodeCategoryValue.EncounterAdmitType).ToString(), encounter.EncounterAdmitType.ToString());
                    vm.Age = CalculatePersonAge(encounter.PatientInfo.PersonBirthDate, currentDateTime, Convert.ToInt32(encounter.EncounterFacility));
                    vm.OverrideBedType = bedMasterbal.GetOverRideBedTypeByInPatientEncounterId(Convert.ToString(encounter.EncounterID));
                    vm.EncounterTypeName = globalCodeBal.GetNameByGlobalCodeValueAndCategoryValue(Convert.ToString(encountertypeCat), encounter.EncounterType.ToString());
                    vm.VirtuallyDischarge = encounter.EncounterDischargePlan != null ? "Discharge" : "";
                    vm.VirtuallyDischargeOn = encounter.EncounterDischargePlan != null ? encounter.EncounterDischargeLocation : null;

                    var insBal = new InsuranceCompanyBal();
                    if (encounter != null && encounter.PatientInfo != null && !string.IsNullOrEmpty(encounter.PatientInfo.PersonInsuranceCompany))
                    {
                        var ins = insBal.GetInsuranceDetailsByPayorId(encounter.PatientInfo.PersonInsuranceCompany);
                        if (ins != null)
                        {
                            vm.InsuranceCompanyName = ins.InsuranceCompanyName;
                            vm.InsuranceCompanyPhoneNumber = ins.InsuranceCompanyMainPhone;
                            vm.InsuranceCompanyFaxNumber = ins.InsuranceCompanyFax;
                            vm.InsuranceCompanyAddress = $"{ ins.InsuranceCompanyStreetAddress} { ins.InsuranceCompanyStreetAddress2}";
                        }
                    }
                    var authorizationBal = new AuthorizationBal();
                    vm.EncounterAuthorizationList = authorizationBal.GetAuthorizationsByEncounterId(vm.EncounterID.ToString());

                    var pId = Convert.ToInt32(vm.PatientID);
                    if (vm.EncounterAuthorizationList.Any())
                    {
                        using (var bal = new DocumentsTemplatesBal())
                        {
                            vm.AuthDocs = new List<DocumentsTemplates>();
                            var docs = await bal.GetPatientDocumentsList(pId);
                            if (docs.Any())
                                vm.AuthDocs = docs.Where(a => a.DocumentName.Equals("Authorization File")).ToList();
                        }
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
        }
    }
}
