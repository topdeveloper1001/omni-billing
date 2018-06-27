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
using BillingSystem.Repository.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class BedMasterService : IBedMasterService
    {
        private readonly IRepository<UBedMaster> _repository;
        private readonly IRepository<BedRateCard> _brRepository;
        private readonly IRepository<FacilityStructure> _fRepository;
        private readonly IRepository<MappingPatientBed> _mpRepository;
        private readonly IRepository<ServiceCode> _sRepository;

        private readonly BillingEntities _context;
        private readonly IMapper _mapper;

        public BedMasterService(IRepository<UBedMaster> repository, IRepository<BedRateCard> brRepository, IRepository<FacilityStructure> fRepository, IRepository<MappingPatientBed> mpRepository, IRepository<ServiceCode> sRepository, BillingEntities context, IMapper mapper)
        {
            _repository = repository;
            _brRepository = brRepository;
            _fRepository = fRepository;
            _mpRepository = mpRepository;
            _sRepository = sRepository;
            _context = context;
            _mapper = mapper;
        }


        /// <summary>
        /// Method to add/Update the BedRateCard in the database.
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public int AddUpdateBedMaster(UBedMaster m)
        {
            if (m.BedId > 0)
                _repository.UpdateEntity(m, m.BedId);
            else
                _repository.Create(m);
            return m.BedId;
        }

        /// <summary>
        /// Method to add the BedRateCard in the database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UBedMaster GetBedMasterById(int id)
        {
            var m = _repository.Where(x => x.BedId == id).FirstOrDefault();
            return m;
        }

        /// <summary>
        /// Method to add the BedRateCard in the database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UBedMaster GetBedMasterByStructureId(int id)
        {
            var m = _repository.Where(x => x.FacilityStructureId == id).FirstOrDefault();
            return m;
        }

        /// <summary>
        /// Method to add the BedRateCard in the database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UBedMaster GetBedMasterIdByStructureId(int id)
        {
            var m = _repository.Where(x => x.FacilityStructureId == id).FirstOrDefault();
            return m ?? new UBedMaster();
        }

        //private DateTime? GetExpectedEndDate(string bedid)
        //{
        //    var mappingPatientBed = new MappingPatientBedBal();
        //    var mappingPatientBedObj = mappingPatientBed.GetAllMappingPatientBedById(bedid);
        //    return mappingPatientBedObj == null ? null : mappingPatientBed.GetAllMappingPatientBedById(bedid).ExpectedEndDate;
        //}

        private string GetBedNameFromFacilityStructure(int facilityStructureId)
        {
            var list = _fRepository.Where(f => f.FacilityStructureId == facilityStructureId).FirstOrDefault();
            return list != null ? list.FacilityStructureName : string.Empty;

        }

        public string GetBedNameFromBedId(int bedId)
        {
            var m = GetBedMasterById(bedId);
            if (m != null)
            {
                var list = _fRepository.Where(f => f.FacilityStructureId == m.FacilityStructureId).FirstOrDefault();
                return list != null ? list.FacilityStructureName : string.Empty;
            }
            return string.Empty;

        }

        public string GetBedNameByInPatientEncounterId(string encounterId)
        {
            var bedName = string.Empty;
            var bedTypeAssigned = _mpRepository.Where(m => m.EncounterID.Equals(encounterId) && m.EndDate == null).OrderByDescending(x => x.PatientID).FirstOrDefault();
            if (bedTypeAssigned != null)
            {
                var bedId = Convert.ToInt32(bedTypeAssigned.BedNumber);

                var bedMaster = _repository.Where(b => b.BedId == bedId).FirstOrDefault();
                if (bedMaster != null)
                {
                    var fsId = Convert.ToInt32(bedMaster.FacilityStructureId);

                    var fsStructure = _fRepository.Where(f => f.FacilityStructureId == fsId).FirstOrDefault();
                    if (fsStructure != null)
                        bedName = fsStructure.FacilityStructureName;

                }


            }
            return bedName;
        }

        public string GetBedByInPatientEncounterId(string encounterId)
        {
            var bedmaster = "0";
            var bedTypeAssigned = _mpRepository.Where(m => m.EncounterID.Equals(encounterId) && m.EndDate == null).FirstOrDefault();
            // Commented the Below line as there is error of not checking the ENd time is null then it will always picks the first value of the encounter mapping
            // var bedTypeAssigned = mappingBedRep.Where(m => m.EncounterID.Equals(encounterId)).FirstOrDefault();
            if (bedTypeAssigned != null)
            {
                var bedId = Convert.ToInt32(bedTypeAssigned.BedNumber);

                var bedMaster = _repository.Where(b => b.BedId == bedId).FirstOrDefault();
                if (bedMaster != null)
                {
                    bedmaster = bedMaster.BedId.ToString();
                }

            }
            return bedmaster;
        }

        /// <summary>
        /// Get the GetBedRateCardsList
        /// </summary>
        /// <returns>Return the BedRateCard List</returns>
        public IEnumerable<BedMasterCustomModel> GetBedMasterListByRole(int facilityId, int corporateid)
        {
            var lstBedMaster = new List<BedMasterCustomModel>();
            var bedList = _repository.GetAll().Where(x => (x.IsDeleted == null || !(bool)x.IsDeleted) && x.IsActive && x.FacilityId == facilityId).ToList();

            var facBal = new FacilityBal();
            var globalBal = new GlobalCodeBal();

            lstBedMaster.AddRange(bedList.Select(item => new BedMasterCustomModel
            {
                BedMaster = item,
                FacilityName = facBal.GetFacilityNameById(Convert.ToInt32(item.FacilityId)),
                BedTypeName =
                    globalBal.GetNameByGlobalCodeValueAndCategoryValue(
                        Convert.ToInt32(Common.Common.GlobalCodeCategoryValue.Bedtypes).ToString(),
                        Convert.ToString(item.BedType)),
                BedName = GetBedNameFromFacilityStructure(Convert.ToInt32(item.FacilityStructureId))
            }));

            return lstBedMaster;

        }

        public string GetOverRideBedTypeByInPatientEncounterId(string encounterId)
        {
            var str = string.Empty;
            var bedTypeAssigned = _mpRepository.Where(m => m.EncounterID.Equals(encounterId)).FirstOrDefault();
            if (bedTypeAssigned != null)
            {
                str = Convert.ToInt32(bedTypeAssigned.OverrideBedType).ToString();
            }
            return str;
        }

        public bool CheckIdBedOccupied(int bedid)
        {
            var result = _repository.Where(x => x.BedId == bedid && x.IsOccupied != false).FirstOrDefault();
            return result != null;
        }



        /// <summary>
        /// Deletes the bed master by identifier.
        /// </summary>
        /// <param name="facilityStructureId">The facility structure identifier.</param>
        /// <returns></returns>
        public bool DeleteBedMasterById(int facilityStructureId)
        {
            var m = _repository.Where(x => x.FacilityStructureId == facilityStructureId).FirstOrDefault();
            return _repository.Delete(m) > 0;
        }



        /// <summary>
        /// Gets the bed struture for facility.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="corporateid">The corporateid.</param>
        /// <returns></returns>
        public List<FacilityBedStructureCustomModel> GetBedStrutureForFacility(int facilityId, int corporateid,string ServiceCodeTableNumber)
        {
            var spName = string.Format("EXEC {0} @pCorporateID, @pFacilityID", StoredProcedures.SPROC_GetDBBedStruture);
            var sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("pCorporateID", corporateid);
            sqlParameters[1] = new SqlParameter("pFacilityID", facilityId);
            IEnumerable<FacilityBedStructureCustomModel> result = _context.Database.SqlQuery<FacilityBedStructureCustomModel>(spName, sqlParameters);
            var allBeds = result.ToList();
            if (allBeds.Any())
            {

                if (allBeds.Count > 0)
                {
                    allBeds = allBeds.Select(item =>
                    {
                        item.BedOverrideTypeList = item.CanOverRide && !string.IsNullOrEmpty(item.OverRideWith)
                            ? GetOverrideBedsListInEncounters(item.OverRideWith, ServiceCodeTableNumber)
                            : new List<DropdownListData>();
                        return item;
                    }).ToList();
                }
            }
            return allBeds;
        }

        /// <summary>
        /// Gets the type of the custom overrideable bed service.
        /// </summary>
        /// <param name="serviceCodesString">The service codes string.</param>
        /// <returns></returns>
        private IEnumerable<DropdownListData> GetCustomOverrideableBedServiceType(string serviceCodesString, string ServiceCodeTableNumber)
        {
            var overRideBedChargesDdl = new List<DropdownListData>();

            var overirdeWithServiceCodes = !string.IsNullOrEmpty(serviceCodesString)
                ? serviceCodesString
                : string.Empty;
            //var serviceCodesList = serviceCodeRepository.GetAll();
            var serviceCodesList = _sRepository.Where(s => s.IsActive == true && s.ServiceCodeTableNumber.Trim().Equals(ServiceCodeTableNumber)).ToList();
            var overrideServiceCodesList = new List<ServiceCode>();
            if (overirdeWithServiceCodes.Contains(","))
            {
                var serviceCodesArray = overirdeWithServiceCodes.Split(',');
                foreach (var item in serviceCodesArray)
                {
                    overrideServiceCodesList.AddRange(
                        serviceCodesList.Where(x => x.ServiceCodeValue.ToLower().Equals(item.ToLower())));
                }
            }
            else
            {
                overrideServiceCodesList.AddRange(
                    serviceCodesList.Where(
                        x => x.ServiceCodeValue.ToLower().Equals(overirdeWithServiceCodes.ToLower())));
            }
            overrideServiceCodesList = overrideServiceCodesList.Distinct().ToList();
            overRideBedChargesDdl.AddRange(overrideServiceCodesList.Select(item => new DropdownListData
            {
                Text = item.ServiceCodeValue + " " + item.ServiceCodeDescription,
                Value = item.ServiceCodeValue,
            }));
            return overRideBedChargesDdl;

        }

        /// <summary>
        /// Gets the bed type by service code.
        /// </summary>
        /// <param name="serviceCode">The service code.</param>
        /// <returns></returns>
        public int? GetBedTypeByServiceCode(string serviceCode)
        {
            var bedMaster = _brRepository.GetAll().FirstOrDefault(x => x.ServiceCodeValue.Equals(serviceCode));
            return bedMaster != null ? (bedMaster.BedTypes) : null;
        }



        /// <summary>
        /// Gets the type of the custom overrideable bed service.
        /// </summary>
        /// <param name="serviceCodeValues">The service codes string.</param>
        /// <returns></returns>
        public IEnumerable<DropdownListData> GetOverrideBedsListInEncounters(string serviceCodeValues, string ServiceCodeTableNumber)
        {
            var currentDateTime = DateTime.Now.Date;
            var ddlData = new List<DropdownListData>();
            serviceCodeValues = !string.IsNullOrEmpty(serviceCodeValues) ? serviceCodeValues.Trim() : string.Empty;
            if (!string.IsNullOrEmpty(serviceCodeValues))
            {
                var scArray = serviceCodeValues.Split(',').ToList();
                var scList = _sRepository.Where(s => s.IsActive == true && s.ServiceCodeTableNumber.Trim().Equals(ServiceCodeTableNumber) && scArray.Contains(s.ServiceCodeValue.Trim())).ToList();

                var list =
                    _brRepository.Where(b => scArray.Contains(b.ServiceCodeValue.Trim()) && b.IsDeleted != true && b.IsActive && b.EffectiveFrom <= currentDateTime && (b.EffectiveTill == null || b.EffectiveTill >= currentDateTime)).Select(s => s.ServiceCodeValue.Trim()).ToList();

                scList = scList.Where(s => list.Contains(s.ServiceCodeValue.Trim())).Distinct().ToList();

                if (scList.Count > 0)
                    scList = scList.OrderBy(v => v.ServiceCodeValue, new NumericComparer()).ToList();

                if (scList.Count > 0)
                {
                    ddlData.AddRange(scList.Select(item => new DropdownListData
                    {
                        Text = item.ServiceCodeValue + " " + item.ServiceCodeDescription,
                        Value = item.ServiceCodeValue
                    }));
                }
            }
            return ddlData;
        }
    }
}
