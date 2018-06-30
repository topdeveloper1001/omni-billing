using System;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using AutoMapper;
using BillingSystem.Repository.Interfaces;
using System.Data.SqlClient;
using BillingSystem.Common.Common;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class FacilityService : IFacilityService
    {
        private readonly IRepository<Facility> _repository;
        private readonly IRepository<PatientInfo> _piRepository;
        private readonly IMapper _mapper;
        private readonly BillingEntities _context;

        public FacilityService(IRepository<Facility> repository, IRepository<PatientInfo> piRepository, IMapper mapper, BillingEntities context)
        {
            _repository = repository;
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
            var facilityObj = _repository.Where(f => f.FacilityId == Convert.ToInt32(facilityid)).FirstOrDefault() != null ? _repository.Where(f => f.FacilityId == Convert.ToInt32(facilityid)).FirstOrDefault().FacilityTimeZone : TimeZoneInfo.Utc.ToString();
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

        /// <summary>
        /// Method to add/Update the facility in the database.
        /// </summary>
        /// <param name="facility"></param>
        /// <param name="facilityId"></param>
        /// <returns></returns>
        public List<FacilityCustomModel> AddUpdateFacility(Facility facility, out int facilityId)
        {
            facilityId = 0;
            if (facility.FacilityId > 0)
            {
                var current = _repository.GetSingle(facility.FacilityId);
                facility.CreatedBy = current.CreatedBy;
                facility.CreatedDate = current.CreatedDate;
                facility.LoggedInID = current.LoggedInID;
                _repository.UpdateEntity(facility, facility.FacilityId);
            }
            else
                _repository.Create(facility);
            facilityId = facility.FacilityId;

            var list = GetFacilityList(Convert.ToInt32(facility.CorporateID));
            return list;
        }



        /// <summary>
        /// Method to add the facility in the database.
        /// </summary>
        /// <returns></returns>
        public Facility GetFacilityById(int id)
        {
            return _repository.Where(f => f.FacilityId == id).FirstOrDefault();
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
