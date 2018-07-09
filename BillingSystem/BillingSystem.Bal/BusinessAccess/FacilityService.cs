using System;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using AutoMapper;

using System.Data.SqlClient;
using BillingSystem.Common.Common;
using BillingSystem.Bal.Interfaces;
using System.Data;

namespace BillingSystem.Bal.BusinessAccess
{
    public class FacilityService : IFacilityService
    {
        private readonly IRepository<Facility> _repository;
        private readonly IRepository<FacilityContact> _fcRepository;
        private readonly IRepository<PatientInfo> _piRepository;
        private readonly IMapper _mapper;
        private readonly BillingEntities _context;

        public FacilityService(IRepository<Facility> repository, IRepository<FacilityContact> fcRepository, IRepository<PatientInfo> piRepository, IMapper mapper, BillingEntities context)
        {
            _repository = repository;
            _fcRepository = fcRepository;
            _piRepository = piRepository;
            _mapper = mapper;
            _context = context;
        }

        /// <summary>
        /// Get the facilities
        /// </summary>
        /// <returns>Return the Facility View Model</returns>
        public List<Facility> GetFacilities(int corporateId)
        {
            var list = _repository.Where(f => !f.IsDeleted && f.IsActive && (f.CorporateID == corporateId || corporateId == 0) && f.CorporateID != null).Include(f => f.Country).OrderBy(x => x.FacilityName).ToList();
            return list;
        }

        /// <summary>
        /// Gets the facilities.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public List<Facility> GetFacilities(int corporateId, int facilityId)
        {
            if (facilityId > 0)
            {
                var list = _repository.Where(f => !f.IsDeleted && f.IsActive && f.FacilityId == facilityId
                                                  && f.CorporateID != null &&
                                                  (corporateId == 0 || f.CorporateID == corporateId)
                    ).Include(f => f.Country).ToList();
                return list;
            }
            return GetFacilities(corporateId);
        }
        public DateTime GetInvariantCultureDateTime(int facilityid)
        {
            var facilityObj = _repository.Where(f => f.FacilityId == facilityid).FirstOrDefault() != null ?
                _repository.Where(f => f.FacilityId == facilityid).FirstOrDefault().FacilityTimeZone :
                TimeZoneInfo.Utc.ToString();
            var tzi = TimeZoneInfo.FindSystemTimeZoneById(facilityObj);
            var utcTime = DateTime.Now.ToUniversalTime();
            var convertedTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);
            return convertedTime;
        }
        /// <summary>
        /// Gets the facility name by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public string GetFacilityNameById(int id)
        {
            var facility = GetFacilityById(id);
            return (facility != null) ? facility.FacilityName : string.Empty;
        }

        public string GetFacilityNameByNumber(string number)
        {
            var facility = _repository.Where(a => a.FacilityNumber.Equals(number)).FirstOrDefault();
            return (facility != null) ? facility.FacilityName : string.Empty;
        }
        /// <summary>
        /// Method to add/Update the facility in the database.
        /// </summary>
        /// <param name="facility"></param>
        /// <param name="facilityId"></param>
        /// <returns></returns>
        public List<FacilityCustomModel> AddUpdateFacility(FacilityCustomModel vm, DataTable dt, out int facilityId)
        {
            IEnumerable<FacilityCustomModel> list = null;
            try
            {
                facilityId = vm.FacilityId;
                var facility = _mapper.Map<Facility>(vm);

                var sqlParameters = new SqlParameter[27];
                sqlParameters[0] = new SqlParameter(InputParams.pId.ToString(), facility.FacilityId);
                sqlParameters[1] = new SqlParameter(InputParams.pNumber.ToString(), facility.FacilityNumber);
                sqlParameters[2] = new SqlParameter(InputParams.pName.ToString(), facility.FacilityName);
                sqlParameters[3] = new SqlParameter(InputParams.pStreetAddress.ToString(), facility.FacilityStreetAddress);
                sqlParameters[4] = new SqlParameter(InputParams.pStreetAddress2.ToString(), !string.IsNullOrEmpty(facility.FacilityStreetAddress2) ? facility.FacilityStreetAddress2 : string.Empty);
                sqlParameters[5] = new SqlParameter(InputParams.pCity.ToString(), facility.FacilityCity);
                sqlParameters[6] = new SqlParameter(InputParams.pState.ToString(), facility.FacilityState);
                sqlParameters[7] = new SqlParameter(InputParams.pZipCode.ToString(), !string.IsNullOrEmpty(facility.FacilityZipCode.ToString()) ? facility.FacilityZipCode : 0);
                sqlParameters[8] = new SqlParameter(InputParams.pMainPhone.ToString(), !string.IsNullOrEmpty(facility.FacilityMainPhone) ? facility.FacilityMainPhone : string.Empty);
                sqlParameters[9] = new SqlParameter(InputParams.pFax.ToString(), !string.IsNullOrEmpty(facility.FacilityFax) ? facility.FacilityFax : string.Empty);
                sqlParameters[10] = new SqlParameter(InputParams.pSecondPhone.ToString(), !string.IsNullOrEmpty(facility.FacilitySecondPhone) ? facility.FacilitySecondPhone : string.Empty);
                sqlParameters[11] = new SqlParameter(InputParams.pPOBox.ToString(), !string.IsNullOrEmpty(facility.FacilityPOBox) ? facility.FacilityPOBox : string.Empty);
                sqlParameters[12] = new SqlParameter(InputParams.pLicenseNumber.ToString(), !string.IsNullOrEmpty(facility.FacilityLicenseNumber) ? facility.FacilityLicenseNumber : string.Empty);
                sqlParameters[13] = new SqlParameter(InputParams.pLicenseNumberExpire.ToString(), !string.IsNullOrEmpty(facility.FacilityLicenseNumberExpire.ToString()) ? facility.FacilityLicenseNumberExpire : DateTime.Now);
                sqlParameters[14] = new SqlParameter(InputParams.pTypeLicense.ToString(), !string.IsNullOrEmpty(facility.FacilityTypeLicense) ? facility.FacilityTypeLicense : string.Empty);
                sqlParameters[15] = new SqlParameter(InputParams.pFacilityRelated.ToString(), !string.IsNullOrEmpty(facility.FacilityRelated) ? facility.FacilityRelated : string.Empty);
                sqlParameters[16] = new SqlParameter(InputParams.pTotalLicenseBed.ToString(), !string.IsNullOrEmpty(facility.FacilityTotalLicenseBed.ToString()) ? facility.FacilityTotalLicenseBed : 0);
                sqlParameters[17] = new SqlParameter(InputParams.pTotalStaffedBed.ToString(), !string.IsNullOrEmpty(facility.FacilityTotalStaffedBed.ToString()) ? facility.FacilityTotalStaffedBed : 0);
                sqlParameters[18] = new SqlParameter(InputParams.pAffiliationNumber.ToString(), !string.IsNullOrEmpty(facility.FacilityAffiliationNumber.ToString()) ? facility.FacilityAffiliationNumber : 0);
                sqlParameters[19] = new SqlParameter(InputParams.pRegionId.ToString(), !string.IsNullOrEmpty(facility.RegionId) ? facility.RegionId : string.Empty);
                sqlParameters[20] = new SqlParameter(InputParams.pCountryID.ToString(), facility.CountryID);
                sqlParameters[21] = new SqlParameter(InputParams.pCorporateID.ToString(), facility.CorporateID);
                sqlParameters[22] = new SqlParameter(InputParams.pTimeZone.ToString(), facility.FacilityTimeZone);
                sqlParameters[23] = new SqlParameter(InputParams.pSenderID.ToString(), !string.IsNullOrEmpty(facility.SenderID) ? facility.SenderID : string.Empty);
                sqlParameters[24] = new SqlParameter(InputParams.pLoggedInUserId.ToString(), facility.FacilityId > 0 ? facility.ModifiedBy : facility.CreatedBy);
                sqlParameters[25] = new SqlParameter(InputParams.pCurrentDate.ToString(), facility.FacilityId > 0 ? facility.ModifiedDate : facility.CreatedDate);
                sqlParameters[26] = new SqlParameter
                {
                    ParameterName = InputParams.pContact.ToString(),
                    SqlDbType = SqlDbType.Structured,
                    Value = dt,
                    TypeName = "FacilityContactT"
                };

                using (var ms = _context.MultiResultSetSqlQuery(StoredProcedures.SprocSaveFacility.ToString(), false, sqlParameters))
                {
                    var newId = ms.ResultSetFor<int>().FirstOrDefault();
                    if (newId > 0)
                    {
                        facilityId = newId;
                        list = ms.GetResultWithJson<FacilityCustomModel>(JsonResultsArray.Facility.ToString());
                        return list.ToList();
                    }
                }
                return null;

            }
            catch (Exception ex)
            {
                facilityId = 0;
                return null;
            }
            //var facility = _mapper.Map<Facility>(vm);
            //facilityId = 0;
            //if (facility.FacilityId > 0)
            //{
            //    var current = _repository.GetSingle(facility.FacilityId);
            //    facility.CreatedBy = current.CreatedBy;
            //    facility.CreatedDate = current.CreatedDate;
            //    facility.LoggedInID = current.LoggedInID;
            //    _repository.UpdateEntity(facility, facility.FacilityId);
            //}
            //else
            //    _repository.Create(facility);
            //facilityId = facility.FacilityId;

            //var list = GetFacilityList(Convert.ToInt32(facility.CorporateID));
            //return list;
        }



        /// <summary>
        /// Method to add the facility in the database.
        /// </summary>
        /// <returns></returns>
        public FacilityCustomModel GetFacilityById(int id)
        {
            var mlst = _repository.Where(f => f.FacilityId == id).FirstOrDefault();
            var vm = _mapper.Map<FacilityCustomModel>(mlst);
            vm.FacilityContact = _fcRepository.Where(x => x.FacilityId == id).ToList();
            return vm;

        }

        //function to validate facility number and Licence number
        /// <summary>
        /// Validates the facility number license number.
        /// </summary>
        /// <param name="facilityNumber">The facility number.</param>
        /// <param name="lic">The facility license number.</param>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public int CheckDuplicateFacilityNoAndLicenseNo(string facilityNumber, string lic, int id, int corporateId)
        {
            int result = 0;

            var isFNumberAndLicSame =
                _repository.Where(
                    f =>
                        f.FacilityNumber.Equals(facilityNumber) && f.CorporateID == corporateId &&
                        f.FacilityLicenseNumber.Equals(lic) && (id == 0 || f.FacilityId != id) && f.IsActive && !f.IsDeleted)
                    .Any();
            if (isFNumberAndLicSame)
                result = 1;                 //1 means facility number and License number matched



            var isfacNumberSame = _repository.Where(
                f => f.FacilityNumber.Equals(facilityNumber) && f.CorporateID == corporateId && (id == 0 || f.FacilityId != id) && f.IsActive &&
                     !f.IsDeleted).Any();
            if (isfacNumberSame)
                result = 2;                 //2 means facility number matched


            var isLicSame =
                _repository.Where(
                    x =>
                        (id == 0 || x.FacilityId != id) && x.FacilityLicenseNumber.Equals(lic) &&
                        x.CorporateID == corporateId &&
                        x.IsDeleted != true).Any();

            if (isLicSame)
                result = 3;//3 means License number matched
            return result;
        }

        /// <summary>
        /// Gets the facilities by corporate identifier.
        /// </summary>
        /// <param name="corpId">The corp identifier.</param>
        /// <returns></returns>
        public List<Facility> GetFacilitiesByCorporateId(int corpId)
        {
            var list = _repository.Where(f => !f.IsDeleted && f.IsActive && f.CorporateID != null && f.CorporateID == corpId).ToList();
            return list;
        }

        /// <summary>
        /// Get the facilities
        /// </summary>
        /// <returns>Return the Facility View Model</returns>
        public List<Facility> GetFacilitiesByRoles(int facilityId, int corporateId)
        {
            var list = _repository.GetAll().Where(f => !f.IsDeleted && f.IsActive
                                                       // && f.FacilityId == facilityId 
                                                       && f.CorporateID == corporateId).ToList();
            return list;
        }

        /// <summary>
        /// Gets the facilities by corporate identifier without countries.
        /// </summary>
        /// <param name="corpId">The corp identifier.</param>
        /// <returns></returns>
        public List<Facility> GetFacilitiesByCorporateIdWithoutCountries(int corpId)
        {
            var list = _repository.Where(f => !f.IsDeleted && f.IsActive && f.CorporateID == corpId).ToList();
            return list;
        }

        /// <summary>
        /// Gets the facility number by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public string GetFacilityNumberById(int id)
        {
            var facility = _repository.Where(a => a.FacilityId == id).FirstOrDefault();
            return (facility != null) ? facility.FacilityNumber : string.Empty;
        }

        /// <summary>
        /// Gets the facility time zone by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public string GetFacilityTimeZoneById(int id)
        {
            var facilitytimeZone = TimeZoneInfo.Utc.ToString();
            if (id <= 0) return facilitytimeZone;
            var facilityobj = GetFacilityById(id);
            if (facilityobj != null)
                facilitytimeZone = string.IsNullOrEmpty(facilityobj.FacilityTimeZone)
                    ? TimeZoneInfo.Utc.ToString()
                    : facilityobj.FacilityTimeZone;
            return facilitytimeZone;
        }

        /// <summary>
        /// Gets the facilities by corpoarte identifier.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <returns></returns>
        public IEnumerable<Facility> GetFacilitiesByCorpoarteId(int corporateId)
        {
            var list = _repository.GetAll()
                .Include(f => f.Country)
                .Where(
                    f => !f.IsDeleted && f.IsActive && f.CorporateID == corporateId && f.CorporateID != null)
                .ToList();
            return list;
        }

        public Facility GetFacilityDetailByPatientId(int patientId)
        {
            var facilityId = _piRepository.Where(p => p.PatientID == patientId).Select(x => x.FacilityId).FirstOrDefault();
            var m = GetFacilityById(Convert.ToInt32(facilityId));
            return m ?? new Facility();
        }

        public List<FacilityCustomModel> GetFacilityList(int corporateId)
        {
            var list = new List<FacilityCustomModel>();
            var facilities = corporateId > 0
                ? _repository.Where(
                    f => !f.IsDeleted && f.IsActive && f.CorporateID == corporateId && f.CorporateID != null)
                    .Include(c => c.Country)
                    .ToList()
                : _repository.Where(f => !f.IsDeleted && f.IsActive).Include(f => f.Country).ToList();
            if (facilities.Count > 0)
                list.AddRange(facilities.Select(x => _mapper.Map<FacilityCustomModel>(x)));
            return list;
        }

        /// <summary>
        /// Get the facilities
        /// </summary>
        /// <returns>Return the Facility View Model</returns>
        public List<Facility> GetFacilitiesWithoutCorporateFacility(int corporateId)
        {
            var list = corporateId > 0
                ? _repository.GetAll()
                    .Include(f => f.Country)
                    .Where(
                        f => !f.IsDeleted && f.IsActive && f.CorporateID == corporateId && f.CorporateID != null && f.LoggedInID != 1)
                    .ToList().OrderBy(x => x.FacilityName).ToList()
                : _repository.GetAll().Include(f => f.Country).Where(f => !f.IsDeleted && f.IsActive).ToList().OrderBy(x => x.FacilityName).ToList();
            return list;
        }

        /// <summary>
        /// Gets the facilities.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public IEnumerable<Facility> GetFacilitiesWithoutCorporateFacility(int corporateId, int facilityId)
        {
            if (facilityId > 0)
            {
                var list = corporateId == 0
                    ? _repository.Where(
                        f =>
                            !f.IsDeleted && f.IsActive &&
                            f.FacilityId == facilityId &&
                            f.CorporateID != null && f.LoggedInID != 1)
                        .Include(f => f.Country)
                        .OrderBy(x => x.FacilityName)
                        .ToList()
                    : _repository.Where(
                        f =>
                            !f.IsDeleted && f.IsActive && f.CorporateID == corporateId &&
                            f.FacilityId == facilityId &&
                            f.CorporateID != null && f.LoggedInID == 1)
                        .Include(f => f.Country)
                        .OrderBy(x => x.FacilityName)
                        .ToList();
                return list;
            }
            return GetFacilities(corporateId);
        }



        public bool DeleteFacilityData(string facilityId)
        {
            var sqlParameters = new SqlParameter[1];
            sqlParameters[0] = new SqlParameter("pFacilityId", facilityId);
            _repository.ExecuteCommand(StoredProcedures.SPROC_DeleteFacilityData.ToString(), sqlParameters);
            return true;
        }

        public List<DropdownListData> GetFacilitiesForDashboards(int facilityId, int corporateId, bool userIsAdmin)
        {
            var list = new List<DropdownListData>();
            var facilities = _repository.Where(f => f.IsActive && f.IsDeleted == false && ((corporateId > 0 && f.CorporateID == corporateId) || corporateId == 0) && ((!userIsAdmin && corporateId != 0 && f.FacilityId == facilityId) || (userIsAdmin && f.LoggedInID != 1))).ToList();

            list.AddRange(facilities.Select(item => new DropdownListData
            {
                Text = item.FacilityName,
                Value = Convert.ToString(item.FacilityId)
            }));
            return list;
        }


        private IEnumerable<Facility> GetFacilityListByCorporateId(int corporateId, int facilityId)
        {
            if (corporateId == 0)
                facilityId = 0;

            if (facilityId > 0)
            {
                var list =
                    _repository.Where(
                        f =>
                            f.IsActive && !f.IsDeleted &&
                            (corporateId == 0 || f.CorporateID == corporateId) &&
                            (facilityId == 0 || f.FacilityId == facilityId)).ToList();
                return list;
            }
            return GetFacilities(corporateId);
        }


        public List<DropdownListData> GetFacilityDropdownData(int corporateId, int facilityId)
        {
            var list = new List<DropdownListData>();
            var facilities = GetFacilityListByCorporateId(corporateId, facilityId);
            list.AddRange(facilities.Select(item => new DropdownListData
            {
                Text = item.FacilityName,
                Value = Convert.ToString(item.FacilityId)
            }));
            return list;
        }


        public void CreateDefaultFacilityItems(int fId, string fName, int userId)
        {
            var sqlParameters = new SqlParameter[3];
            sqlParameters[0] = new SqlParameter("aFacilityID", fId);
            sqlParameters[1] = new SqlParameter("aFacilityName", fName);
            sqlParameters[2] = new SqlParameter("aCreatedBy", userId);
            _repository.ExecuteCommand(StoredProcedures.SPROC_DefaultFacilityItems.ToString(), sqlParameters);
        }

    }
}
