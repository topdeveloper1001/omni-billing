using AutoMapper;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;

namespace BillingSystem.Bal.BusinessAccess
{
    public class ReportingService : IReportingService
    {
        private readonly IRepository<ProjectTaskTargets> _repository;
        private readonly IRepository<GlobalCodes> _gRepository;
        private readonly IRepository<Users> _uRepository;
        private readonly IRepository<UserRole> _urRepository;
        private readonly IRepository<LoginTracking> _ltRepository;
        private readonly IRepository<PatientInfo> _piRepository;
        private readonly IRepository<AuditLog> _alRepository;
        private readonly IRepository<Facility> _fRepository;

        private readonly IMapper _mapper;
        private readonly BillingEntities _context;

        public ReportingService(IRepository<ProjectTaskTargets> repository, IRepository<GlobalCodes> gRepository, IRepository<Users> uRepository, IRepository<UserRole> urRepository, IRepository<LoginTracking> ltRepository, IRepository<PatientInfo> piRepository, IRepository<AuditLog> alRepository, IRepository<Facility> fRepository, IMapper mapper, BillingEntities context)
        {
            _repository = repository;
            _gRepository = gRepository;
            _uRepository = uRepository;
            _urRepository = urRepository;
            _ltRepository = ltRepository;
            _piRepository = piRepository;
            _alRepository = alRepository;
            _fRepository = fRepository;
            _mapper = mapper;
            _context = context;
        }

        /// <summary>
        /// Gets the user login activity list.
        /// </summary>
        /// <param name="fromDate">From date.</param>
        /// <param name="tillDate">The till date.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public List<LoginTrackingCustomModel> GetUserLoginActivityList(DateTime fromDate, DateTime tillDate, int userId)
        {
            var list = new List<LoginTrackingCustomModel>();
            var ltReports = userId > 0
                ? _ltRepository.Where(
                    l =>
                        l.CreatedDate != null && ((DateTime)l.CreatedDate) >= fromDate &&
                        ((DateTime)l.CreatedDate) <= tillDate && l.ID == userId).ToList()
                : _ltRepository.Where(
                    l =>
                        l.CreatedDate != null && ((DateTime)l.CreatedDate) >= fromDate &&
                        ((DateTime)l.CreatedDate) <= tillDate).ToList();
            if (ltReports.Count > 0)
            {
                list.AddRange(ltReports.Select(item => new LoginTrackingCustomModel
                {
                    CreatedBy = item.CreatedBy,
                    CreatedDate = item.CreatedDate,
                    DeletedBy = item.DeletedBy,
                    DeletedDate = item.DeletedDate,
                    ID = item.ID,
                    IPAddress = item.IPAddress,
                    IsActive = item.IsActive,
                    IsDeleted = item.IsDeleted,
                    LoginTime = item.LoginTime,
                    LoginTrackingID = item.LoginTrackingID,
                    LogoutTime = item.LogoutTime,
                    ModifiedBy = item.ModifiedBy,
                    ModifiedDate = item.ModifiedDate,
                    UserName = GetNameByUserId(item.ID),
                }));

                foreach (var item in list)
                {
                    var getUserRoles = GetUserRolesByUserId(item.ID);
                    var userRoles = UserRoles(getUserRoles);
                    var getUsersFacility = UserFacilities(getUserRoles);
                    item.AssignedRoles = userRoles;
                    item.AssignedFacilities = getUsersFacility;
                }
            }
            return list;
        }
        private List<UserRole> GetUserRolesByUserId(int userId)
        {
            var userRoles = _urRepository.Where(r => r.UserID == userId && r.IsActive && r.IsDeleted == false).Include(x => x.Role).Include(x => x.Role.FacilityRole).ToList();
            return userRoles;
        }
        private string GetNameByUserId(int? UserID)
        {
            var usersModel = _uRepository.Where(x => x.UserID == UserID && x.IsDeleted == false).FirstOrDefault();
            return usersModel != null ? usersModel.FirstName + " " + usersModel.LastName : string.Empty;
        }
        private string UserFacilities(IEnumerable<UserRole> userRoles)
        {
            var facilityNames = string.Empty;
            var ids = new List<int>();
            foreach (var item in userRoles)
            {
                if (item.Role != null)
                {
                    foreach (var f in item.Role.FacilityRole.ToList())
                    {
                        if (ids.All(fac => fac != f.FacilityId))
                        {
                            ids.Add(f.FacilityId);
                            if (string.IsNullOrEmpty(facilityNames))
                                facilityNames = GetFacilityNameById(f.FacilityId);
                            else
                                facilityNames += string.Format(", {0}", GetFacilityNameById(f.FacilityId));
                        }
                    }
                }
            }
            return facilityNames;
        }
        private string GetFacilityNameById(int id)
        {
            var facility = _fRepository.Get(id);
            return (facility != null) ? facility.FacilityName : string.Empty;
        }
        private string UserRoles(IEnumerable<UserRole> roles)
        {
            var roleNames = string.Empty;
            foreach (var item in roles)
            {
                if (string.IsNullOrEmpty(roleNames))
                    roleNames = item.Role.RoleName;
                else
                    roleNames += string.Format(", {0}", item.Role.RoleName);
            }
            return roleNames;
        }
        /// <summary>
        /// Gets the password disabled log.
        /// </summary>
        /// <param name="fromDate">From date.</param>
        /// <param name="tillDate">The till date.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="corporateId"></param>
        /// <returns></returns>
        public List<AuditLogCustomModel> GetPasswordDisabledLog(DateTime fromDate, DateTime tillDate, int userId, int corporateId, bool isAll)
        {
            var list = new List<AuditLogCustomModel>();
            var ltReports = corporateId == 0
                ? _alRepository.Where(l => l.CreatedDate != null && l.CreatedDate >= fromDate && l.CreatedDate <= tillDate)
                    .ToList()
                : (userId > 0
                    ? _alRepository.Where(
                        l =>
                            l.CreatedDate != null && l.CreatedDate >= fromDate && l.CreatedDate <= tillDate &&
                            l.UserId == userId).ToList()
                    : _alRepository.Where(l => l.CreatedDate != null && l.CreatedDate >= fromDate && l.CreatedDate <= tillDate)
                        .ToList());

            if (ltReports.Count > 0)
            {
                list.AddRange(ltReports.Select(item => new AuditLogCustomModel
                {
                    CreatedDate = item.CreatedDate,
                    AuditLogID = item.AuditLogID,
                    FieldName = item.FieldName,
                    NewValue = item.NewValue,
                    OldValue = item.OldValue,
                    PrimaryKey = item.PrimaryKey,
                    TableName = item.TableName,
                    UserId = item.UserId,
                    Users = item.Users,
                    UserName = GetNameByUserId(item.UserId),
                }));

                list = GetFilteredListByCorporateId(corporateId, list, isAll);

                foreach (var item in list)
                {
                    var getUserRoles = GetUserRolesByUserId(Convert.ToInt32(item.UserId));
                    var userRoles = UserRoles(getUserRoles);
                    var getUsersFacility = UserFacilities(getUserRoles);
                    item.AssignedRoles = userRoles;
                    item.AssignedFacilities = getUsersFacility;
                }
            }
            return list;
        }

        /// <summary>
        /// Gets the password change log.
        /// </summary>
        /// <param name="fromDate">From date.</param>
        /// <param name="tillDate">The till date.</param>
        /// <param name="isAll">if set to <c>true</c> [is all].</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="corporateId"></param>
        /// <returns></returns>
        public List<AuditLogCustomModel> GetPasswordChangeLog(DateTime fromDate, DateTime tillDate, bool isAll, int corporateId)
        {
            var list = new List<AuditLogCustomModel>();
            var ltReports = isAll //&& corporateId == 0
                ? _alRepository.Where(
                    l =>
                        //l.CreatedDate != null && l.CreatedDate >= fromDate &&
                        //l.CreatedDate <= tillDate && l.FieldName.ToLower().Equals("password")).ToList()
                        l.FieldName.ToLower().Equals("password")).ToList()
                : _alRepository.Where(
                    l =>
                         l.CreatedDate != null && l.CreatedDate >= fromDate &&
                        l.CreatedDate <= tillDate && l.FieldName.ToLower().Equals("password")).ToList();
            /*Updated By Krishna On 21072015*/
            //l.CreatedDate != null && l.CreatedDate >= fromDate &&
            //  l.CreatedDate <= tillDate && l.FieldName.ToLower().Equals("password")).ToList();

            if (ltReports.Count > 0)
            {
                list.AddRange(ltReports.Select(item => new AuditLogCustomModel
                {
                    CreatedDate = item.CreatedDate,
                    AuditLogID = item.AuditLogID,
                    FieldName = item.FieldName,
                    NewValue = item.NewValue,
                    OldValue = item.OldValue,
                    PrimaryKey = item.PrimaryKey,
                    TableName = item.TableName,
                    UserId = item.UserId,
                    Users = item.Users,
                    UserName = GetNameByUserId(item.UserId),
                }));

                list = GetFilteredListByCorporateId(corporateId, list, isAll);

                foreach (var item in list)
                {
                    var getUserRoles = GetUserRolesByUserId(Convert.ToInt32(item.UserId));
                    var userRoles = UserRoles(getUserRoles);
                    var getUsersFacility = UserFacilities(getUserRoles);
                    item.AssignedRoles = userRoles;
                    item.AssignedFacilities = getUsersFacility;
                }
            }
            return list;
        }

        /// <summary>
        /// Gets the ageing report.
        /// </summary>
        /// <param name="corporateid">The corporateid.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="date">The date.</param>
        /// <param name="reportType">Type of the report.</param>
        /// <returns></returns>
        public List<AgingReportCustomModel> GetAgeingReport(int corporateid, int facilityId, DateTime? date, int reportType)
        {
            var list = new List<AgingReportCustomModel>();
            var result = GetAgingReport(corporateid, facilityId, date, reportType);
            if (result != null && result.Any())
            {
                var reportingType = (ReportingType)Enum.Parse(typeof(ReportingType), reportType.ToString());
                switch (reportingType)
                {
                    case ReportingType.PayorWiseAgeingReport: //PAYOR WISE Ageing
                        list.AddRange(result.Select(item => new AgingReportCustomModel
                        {
                            ID = item.ID,
                            Name = item.Name,
                            OnTime = item.OnTime,
                            Days1To30 = item.Days1To30,
                            Days31To60 = item.Days31To60,
                            Days61To90 = item.Days61To90,
                            Days91To120 = item.Days91To120,
                            Days121To150 = item.Days121To150,
                            Days151To180 = item.Days151To180,
                            Days181More = item.Days181More,
                            Total = item.Total,
                        }));
                        var listPayorLastvalue = list.FirstOrDefault(_ => _.ID == 99999999);
                        if (listPayorLastvalue != null)
                        {
                            var newlistitem = new AgingReportCustomModel
                            {
                                ID = 0,
                                Name = string.Empty,
                                OnTime = listPayorLastvalue.OnTime,
                                Days1To30 = listPayorLastvalue.Days1To30,
                                Days31To60 = listPayorLastvalue.Days31To60,
                                Days61To90 = listPayorLastvalue.Days61To90,
                                Days91To120 = listPayorLastvalue.Days91To120,
                                Days121To150 = listPayorLastvalue.Days121To150,
                                Days151To180 = listPayorLastvalue.Days151To180,
                                Days181More = listPayorLastvalue.Days181More,
                                Total = listPayorLastvalue.Total
                            };
                            list.RemoveAt(list.FindLastIndex(x => x.ID == 99999999));
                            list.Add(newlistitem);
                        }

                        break;
                    case ReportingType.PatientWiseAgeingReport: //Patient WISE Ageing
                        list.AddRange(result.Select(item => new AgingReportCustomModel
                        {
                            ID = item.ID,
                            Name = item.Name,
                            EmirateID = item.EmirateID,
                            OnTime = item.OnTime,
                            Days1To30 = item.Days1To30,
                            Days31To60 = item.Days31To60,
                            Days61To90 = item.Days61To90,
                            Days91To120 = item.Days91To120,
                            Days121To150 = item.Days121To150,
                            Days151To180 = item.Days151To180,
                            Days181More = item.Days181More,
                            Total = item.Total,
                            DueDate = item.DueDate,
                            EncounterEnd = item.EncounterEnd,
                            EncounterNumber = item.EncounterNumber
                        }));
                        var listPatientLastvalue = list.FirstOrDefault(_ => _.ID == 99999999);
                        if (listPatientLastvalue != null)
                        {
                            var newlistitem = new AgingReportCustomModel
                            {
                                ID = 0,
                                Name = string.Empty,
                                OnTime = listPatientLastvalue.OnTime,
                                Days1To30 = listPatientLastvalue.Days1To30,
                                Days31To60 = listPatientLastvalue.Days31To60,
                                Days61To90 = listPatientLastvalue.Days61To90,
                                Days91To120 = listPatientLastvalue.Days91To120,
                                Days121To150 = listPatientLastvalue.Days121To150,
                                Days151To180 = listPatientLastvalue.Days151To180,
                                Days181More = listPatientLastvalue.Days181More,
                                Total = listPatientLastvalue.Total,
                                DueDate = null,
                                EncounterEnd = null,
                                EncounterNumber = string.Empty
                            };
                            list.RemoveAt(list.FindLastIndex(x => x.ID == 99999999));
                            list.Add(newlistitem);
                        }
                        break;
                    case ReportingType.DepartmentWiseAgeingReport: //Department WISE Ageing
                        list.AddRange(result.Select(item => new AgingReportCustomModel
                        {
                            //DepartmentNumber = item.DepartmentNumber,
                            //DepartmentName = item.DepartmentName,
                            EmirateID = item.EmirateID,
                            OnTime = item.OnTime,
                            Days1To30 = item.Days1To30,
                            Days31To60 = item.Days31To60,
                            Days61To90 = item.Days61To90,
                            Days91To120 = item.Days91To120,
                            Days121To150 = item.Days121To150,
                            Days151To180 = item.Days151To180,
                            Days181More = item.Days181More,
                            Total = item.Total
                        }));
                        break;
                }
            }
            return list;
        }
        private List<AgingReportCustomModel> GetAgingReport(int CorporateID, int FacilityID, DateTime? date, int type)
        {
            try
            {
                if (_context != null)
                {
                    string spName = string.Empty;
                    switch (type)
                    {
                        case 6: //Payor WISE Ageing
                            //spName = string.Format("EXEC {0} 6, 4, {1}", StoredProcedures.SPROC_GetAgeingPayorWise, DateTime.Now);
                            spName = string.Format("EXEC {0} @pCorporateID, @pFacilityID, @pAsOnDate",
                                StoredProcedures.SPROC_GetAgeingPayorWise);
                            break;
                        case 7: //Patinet WISE Ageing
                            spName = string.Format("EXEC {0} @pCorporateID, @pFacilityID, @pAsOnDate",
                                StoredProcedures.SPROC_GetAgeingPatientWise);
                            break;
                        case 8: //Department WISE Ageing
                            spName = string.Format("EXEC {0} @pCorporateID, @pFacilityID, @pAsOnDate",
                                StoredProcedures.SPROC_GetAgeingDepartmentWise);
                            break;
                    }

                    var sqlParameters = new SqlParameter[3];
                    sqlParameters[0] = new SqlParameter("pCorporateID", CorporateID);
                    sqlParameters[1] = new SqlParameter("pFacilityID", FacilityID);
                    sqlParameters[2] = new SqlParameter("pAsOnDate", SqlDbType.VarChar);
                    sqlParameters[2].Value = (date != null)
                        ? date.Value.ToString("yyyy-MM-dd HH:mm:ss")
                        : sqlParameters[2].Value = null;
                    var result = _context.Database.SqlQuery<AgingReportCustomModel>(spName, sqlParameters);
                    return result.ToList();
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private List<ReconcilationReportCustomModel> GetReconciliationReportRep(int CorporateID, int FacilityID,
           DateTime? date, string viewType, int type)
        {
            try
            {
                if (_context != null)
                {
                    string spName = string.Empty;
                    switch (type)
                    {
                        case 9: //Payor WISE Reconciliation
                            spName = string.Format("EXEC {0} @pCorporateID, @pFacilityID, @pAsOnDate, @pViewType",
                                StoredProcedures.SPROC_GetReconcilationARPayorWise);
                            break;
                        case 10: //Patinet WISE Reconciliation
                            spName = string.Format("EXEC {0} @pCorporateID, @pFacilityID, @pAsOnDate, @pViewType",
                                StoredProcedures.SPROC_GetReconcilationARPatientWise);
                            break;
                        case 11: //Department WISE Reconciliation
                            spName = string.Format("EXEC {0} @pCorporateID, @pFacilityID, @pAsOnDate, @pViewType",
                                StoredProcedures.SPROC_GetReconcilationARDepartmentWise);
                            break;
                    }
                    var sqlParameters = new SqlParameter[4];
                    sqlParameters[0] = new SqlParameter("pCorporateID", CorporateID);
                    sqlParameters[1] = new SqlParameter("pFacilityID", FacilityID);
                    sqlParameters[2] = new SqlParameter("pAsOnDate", SqlDbType.VarChar);
                    sqlParameters[2].Value = (date != null)
                        ? date.Value.ToString("yyyy-MM-dd HH:mm:ss")
                        : sqlParameters[2].Value = null;
                    sqlParameters[3] = new SqlParameter("pViewType", viewType);
                    var result = _context.Database.SqlQuery<ReconcilationReportCustomModel>(spName, sqlParameters);
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
        /// Gets the reconciliation report.
        /// </summary>
        /// <param name="corporateid">The corporateid.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="date">The date.</param>
        /// <param name="viewtype">The viewtype.</param>
        /// <param name="reportType">Type of the report.</param>
        /// <returns></returns>
        public List<ReconcilationReportCustomModel> GetReconciliationReport(int corporateid, int facilityId, DateTime? date, string viewtype, int reportType)
        {
            var list = new List<ReconcilationReportCustomModel>();
            var result = GetReconciliationReportRep(corporateid, facilityId, date, viewtype, reportType);
            if (result != null && result.Any())
            {
                switch (viewtype)
                {
                    case "M"://Month WISE Reconcilation
                        list.AddRange(result.Select(item => new ReconcilationReportCustomModel
                        {
                            ID = item.ID,
                            Name = item.Name,
                            D1 = item.D1,
                            D2 = item.D2,
                            D3 = item.D3,
                            D4 = item.D4,
                            D5 = item.D5,
                            D6 = item.D6,
                            D7 = item.D7,
                            D8 = item.D8,
                            D9 = item.D9,
                            D10 = item.D10,
                            D11 = item.D11,
                            D12 = item.D12,
                            D13 = item.D13,
                            D14 = item.D14,
                            D15 = item.D15,
                            D16 = item.D16,
                            D17 = item.D17,
                            D18 = item.D18,
                            D19 = item.D19,
                            D20 = item.D20,
                            D21 = item.D21,
                            D22 = item.D22,
                            D23 = item.D23,
                            D24 = item.D24,
                            D25 = item.D25,
                            D26 = item.D26,
                            D27 = item.D27,
                            D28 = item.D28,
                            D29 = item.D29,
                            D30 = item.D30,
                            D31 = item.D31,
                        }));
                        break;
                    case "W"://Week WISE Reconcilation
                        list.AddRange(result.Select(item => new ReconcilationReportCustomModel
                        {
                            ID = item.ID,
                            Name = item.Name,
                            D1 = item.D1,
                            D2 = item.D2,
                            D3 = item.D3,
                            D4 = item.D4,
                            D5 = item.D5,
                            D6 = item.D6,
                            D7 = item.D7,
                        }));
                        break;
                    case "Y"://Year WISE Reconcilation
                        list.AddRange(result.Select(item => new ReconcilationReportCustomModel
                        {
                            ID = item.ID,
                            Name = item.Name,
                            D1 = item.D1,
                            D2 = item.D2,
                            D3 = item.D3,
                            D4 = item.D4,
                            D5 = item.D5,
                            D6 = item.D6,
                            D7 = item.D7,
                            D8 = item.D8,
                            D9 = item.D9,
                            D10 = item.D10,
                            D11 = item.D11,
                            D12 = item.D12,
                        }));
                        break;
                }
            }
            return list;
        }

        /// <summary>
        /// Gets the revenue forecast facility.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="tillDate">The till date.</param>
        /// <returns></returns>
        public List<RevenueForecast> GetRevenueForecastFacility(int corporateId, int facilityId, DateTime? fromDate, DateTime? tillDate)
        {
            var spName = string.Format("EXEC {0} @pCorporateID , @pFacilityID, @pFromDate, @pTillDate", StoredProcedures.SPROC_GetRevenueForecastFacility);
            var sqlParameters = new SqlParameter[4];
            sqlParameters[0] = new SqlParameter("pCorporateID", corporateId);
            sqlParameters[1] = new SqlParameter("pFacilityID", facilityId);
            sqlParameters[2] = new SqlParameter("pFromDate", fromDate);
            sqlParameters[3] = new SqlParameter("pTillDate", tillDate);
            var result = _context.Database.SqlQuery<RevenueForecast>(spName, sqlParameters);
            return result.ToList();
        }

        /// <summary>
        /// Gets the revenue forecast facility by patient.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public List<RevenueForecast> GetRevenueForecastFacilityByPatient(int patientId)
        {
            var spName = string.Format("EXEC {0} @pPatientID", StoredProcedures.SPROC_GetRevenueForecastbyPatient);
            var sqlParameters = new SqlParameter[1];
            sqlParameters[0] = new SqlParameter("pPatientID", patientId);
            IEnumerable<RevenueForecast> result = _context.Database.SqlQuery<RevenueForecast>(spName, sqlParameters);
            return result.ToList();
        }

        /// <summary>
        /// Gets the login time day night shift.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="tillDate">The till date.</param>
        /// <returns></returns>
        public List<LoginActivityReportCustomModel> GetLoginTimeDayNightShift(int corporateId, int facilityId,
            DateTime? fromDate, DateTime? tillDate, int userId)
        {
            var spName = string.Format("EXEC {0} @pCorporateID,@pFacilityID, @pFromDate ,@pTillDate, @pUserId", StoredProcedures.SPROC_Get_REP_LoginTimeDayNightWise);
            var sqlParameters = new SqlParameter[5];
            sqlParameters[0] = new SqlParameter("pCorporateID", corporateId);
            sqlParameters[1] = new SqlParameter("pFacilityID", facilityId);
            sqlParameters[2] = new SqlParameter("pFromDate", SqlDbType.VarChar);
            sqlParameters[2].Value = (fromDate != null) ? fromDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : sqlParameters[2].Value = null;
            sqlParameters[3] = new SqlParameter("pTillDate", SqlDbType.VarChar);
            sqlParameters[3].Value = (tillDate != null) ? tillDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : sqlParameters[3].Value = null;
            sqlParameters[4] = new SqlParameter("pUserId", userId);
            IEnumerable<LoginActivityReportCustomModel> result = _context.Database.SqlQuery<LoginActivityReportCustomModel>(spName, sqlParameters);
            return result.ToList();
        }

        /// <summary>
        /// Gets the user login activity detail list.
        /// </summary>
        /// <param name="tillDate">The till date.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public List<LoginTrackingCustomModel> GetUserLoginActivityDetailList(int userId, DateTime tillDate)
        {
            var list = new List<LoginTrackingCustomModel>();
            var ltReports = _ltRepository.GetAll().ToList();
            ltReports = ltReports.Where(l =>
                   l.CreatedDate != null && ((DateTime)l.CreatedDate).ToShortDateString().Equals(tillDate.ToShortDateString()) && l.ID == userId).ToList();
            if (ltReports.Any())
            {
                list.AddRange(ltReports.Select(item => new LoginTrackingCustomModel
                {
                    CreatedBy = item.CreatedBy,
                    CreatedDate = item.CreatedDate,
                    DeletedBy = item.DeletedBy,
                    DeletedDate = item.DeletedDate,
                    ID = item.ID,
                    IPAddress = item.IPAddress,
                    IsActive = item.IsActive,
                    IsDeleted = item.IsDeleted,
                    LoginTime = item.LoginTime,
                    LoginTrackingID = item.LoginTrackingID,
                    LogoutTime = item.LogoutTime,
                    ModifiedBy = item.ModifiedBy,
                    ModifiedDate = item.ModifiedDate,
                    UserName = GetNameByUserId(item.ID),
                }));
                foreach (var item in list)
                {
                    var getUserRoles = GetUserRolesByUserId(item.ID);
                    var userRoles = UserRoles(getUserRoles);
                    var getUsersFacility = UserFacilities(getUserRoles);
                    item.AssignedRoles = userRoles;
                    item.AssignedFacilities = getUsersFacility;
                }
            }
            return list.OrderByDescending(x => x.LoginTime).ToList();
        }

        /// <summary>
        /// Gets the claim trans details.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="fromdate">The fromdate.</param>
        /// <param name="tillDate">The till date.</param>
        /// <param name="displayby">The displayby.</param>
        /// <returns></returns>
        public List<BillTransmissionReportCustomModel> GetClaimTransDetails(int corporateId, int facilityId, DateTime? fromdate, DateTime? tillDate, Int32? displayby)
        {
            var spName = string.Format("EXEC {0} @pCorporateID , @pFacilityID, @pFromDate, @pTillDate ,@pDisplayBy ", StoredProcedures.SPROC_Get_REP_ClaimTransDetails);
            var sqlParameters = new SqlParameter[5];
            sqlParameters[0] = new SqlParameter("pCorporateID", corporateId);
            sqlParameters[1] = new SqlParameter("pFacilityID", facilityId);
            sqlParameters[2] = new SqlParameter("pFromDate", fromdate);
            sqlParameters[3] = new SqlParameter("pTillDate", tillDate);
            sqlParameters[4] = new SqlParameter("pDisplayBy", displayby);
            var result = _context.Database.SqlQuery<BillTransmissionReportCustomModel>(spName, sqlParameters);
            return result.ToList();
        }

        /// <summary>
        /// Gets the denial codes report.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="fromdate">The fromdate.</param>
        /// <param name="tillDate">The till date.</param>
        /// <param name="displayby">The displayby.</param>
        /// <returns></returns>
        public List<DenialReportCustomModel> GetDenialCodesReport(int corporateId, int facilityId, DateTime? fromdate, DateTime? tillDate, Int32? displayby)
        {
            var spName = string.Format("EXEC {0} @pCorporateID , @pFacilityID, @pFromDate, @pTillDate, @pDisplayBy", StoredProcedures.SPROC_Get_REP_DenialCode.ToString());
            var sqlParameters = new SqlParameter[5];
            sqlParameters[0] = new SqlParameter("pCorporateID", corporateId);
            sqlParameters[1] = new SqlParameter("pFacilityID", facilityId);
            sqlParameters[2] = new SqlParameter("pFromDate", SqlDbType.DateTime);
            sqlParameters[2].Value = (fromdate != null) ? fromdate.Value.ToString("MM-dd-yyyy") : sqlParameters[2].Value = null;
            sqlParameters[3] = new SqlParameter("pTillDate", SqlDbType.DateTime);
            sqlParameters[3].Value = (tillDate != null) ? tillDate.Value.ToString("MM-dd-yyyy") : sqlParameters[3].Value = null;
            sqlParameters[4] = new SqlParameter("pDisplayBy", displayby);
            var result = _context.Database.SqlQuery<DenialReportCustomModel>(spName, sqlParameters);
            return result.ToList();
        }

        /// <summary>
        /// Gets the Journal Entry Support report.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="fromdate">The fromdate.</param>
        /// <param name="tillDate">The till date.</param>
        /// <param name="displayby">The displayby.</param>
        /// <returns></returns>
        public List<JournalEntrySupportReportCustomModel> GetJournalEntrySupport(int corporateId, int facilityId, DateTime? fromdate, DateTime? tillDate, Int32? displayby)
        {
            var spName = string.Format("EXEC {0} @pCorporateID , @pFacilityID, @pFromDate, @pTillDate, @pDisplayBy", StoredProcedures.SPROC_Get_REP_JEByDepartment.ToString());
            var sqlParameters = new SqlParameter[5];
            sqlParameters[0] = new SqlParameter("pCorporateID", corporateId);
            sqlParameters[1] = new SqlParameter("pFacilityID", facilityId);
            sqlParameters[2] = new SqlParameter("pFromDate", SqlDbType.DateTime);
            sqlParameters[2].Value = (fromdate != null)
                ? fromdate.Value.ToString("MM-dd-yyyy")
                : sqlParameters[2].Value = DateTime.Today;
            sqlParameters[3] = new SqlParameter("pTillDate", SqlDbType.DateTime);
            sqlParameters[3].Value = (tillDate != null)
                ? tillDate.Value.ToString("MM-dd-yyyy")
                : sqlParameters[3].Value = DateTime.Today.AddDays(1);
            sqlParameters[4] = new SqlParameter("pDisplayBy", displayby);
            var result = _context.Database.SqlQuery<JournalEntrySupportReportCustomModel>(spName, sqlParameters);
            return result.ToList();
        }

        /// <summary>
        /// Gets the patient ageing payor wise.
        /// </summary>
        /// <param name="corporateid">The corporateid.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="date">The date.</param>
        /// <param name="companyId">The company identifier.</param>
        /// <returns></returns>
        public List<AgingReportCustomModel> GetPatientAgeingPayorWise(int corporateid, int facilityId, DateTime? date,
            string companyId)
        {
            var list = new List<AgingReportCustomModel>();
            var spName = string.Format("EXEC {0} @pCorporateID, @pFacilityID, @pAsOnDate, @PayorID",
                        StoredProcedures.SPROC_GetPatientAgeingPayorWise);

            var sqlParameters = new SqlParameter[4];
            sqlParameters[0] = new SqlParameter("pCorporateID", corporateid);
            sqlParameters[1] = new SqlParameter("pFacilityID", facilityId);
            sqlParameters[2] = new SqlParameter("pAsOnDate", SqlDbType.VarChar);
            sqlParameters[2].Value = (date != null)
                ? date.Value.ToString("yyyy-MM-dd HH:mm:ss")
                : sqlParameters[2].Value = null;
            sqlParameters[3] = new SqlParameter("PayorID", companyId);
            IEnumerable<AgingReportCustomModel> result =
                _context.Database.SqlQuery<AgingReportCustomModel>(spName, sqlParameters);
            if (result != null && result.Any())
            {
                list = result.ToList();
                var listPayorLastvalue = list.FirstOrDefault(_ => _.ID == 99999999);
                if (listPayorLastvalue != null)
                {
                    var newlistitem = new AgingReportCustomModel
                    {
                        ID = 0,
                        Name = string.Empty,
                        OnTime = listPayorLastvalue.OnTime,
                        Days1To30 = listPayorLastvalue.Days1To30,
                        Days31To60 = listPayorLastvalue.Days31To60,
                        Days61To90 = listPayorLastvalue.Days61To90,
                        Days91To120 = listPayorLastvalue.Days91To120,
                        Days121To150 = listPayorLastvalue.Days121To150,
                        Days151To180 = listPayorLastvalue.Days151To180,
                        Days181More = listPayorLastvalue.Days181More,
                        Total = listPayorLastvalue.Total,
                        DueDate = listPayorLastvalue.DueDate,
                        EncounterEnd = listPayorLastvalue.EncounterEnd,
                        EncounterNumber = listPayorLastvalue.EncounterNumber
                    };
                    list.RemoveAt(list.FindLastIndex(x => x.ID == 99999999));
                    list.Add(newlistitem);
                }
            }
            return list;
        }

        /// <summary>
        /// Gets the bill edit correction logs.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="fromdate">The fromdate.</param>
        /// <param name="tillDate">The till date.</param>
        /// <param name="displayall">if set to <c>true</c> [displayall].</param>
        /// <returns></returns>
        public List<ScrubEditTrackCustomModel> GetBillEditCorrectionLogs(int corporateId, int facilityId, DateTime? fromdate, DateTime? tillDate, bool displayall)
        {
            var spName = string.Format("EXEC {0} @pFromDate,@pTillDate,@pFID, @pCID", StoredProcedures.SPROC_Get_Rep_CorrectionLog.ToString());
            var sqlParameters = new SqlParameter[4];
            sqlParameters[0] = new SqlParameter("pFromDate", fromdate);
            sqlParameters[1] = new SqlParameter("pTillDate", tillDate);
            sqlParameters[2] = new SqlParameter("pFID", facilityId);
            sqlParameters[3] = new SqlParameter("pCID", corporateId);
            IEnumerable<ScrubEditTrackCustomModel> result = _context.Database.SqlQuery<ScrubEditTrackCustomModel>(spName, sqlParameters);
            return result.OrderByDescending(x => x.CreatedDate).ThenBy(f => f.CreatedByName).ToList();
        }


        /// <summary>
        /// Gets the charges report.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="fromdate">The fromdate.</param>
        /// <param name="tillDate">The till date.</param>
        /// <param name="departmentNumber">The department number.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public List<ChargesReportCustomModel> GetChargesReport(int corporateId, int facilityId, DateTime? fromdate, DateTime? tillDate,
            decimal? departmentNumber, int type)
        {
            var spName = string.Format("EXEC {0} @pCorporateId, @pFacilityId,@pReportType,@pFromDate,@pTillDate,@pDepartmentNumber", StoredProcedures.SPROC_Get_REP_ChargeReport);
            var sqlParameters = new SqlParameter[6];
            sqlParameters[0] = new SqlParameter("pCorporateId", corporateId);
            sqlParameters[1] = new SqlParameter("pFacilityId", facilityId);
            sqlParameters[2] = new SqlParameter("pReportType", type);
            sqlParameters[3] = new SqlParameter("pFromDate", fromdate);
            sqlParameters[4] = new SqlParameter("pTillDate", tillDate);
            sqlParameters[5] = new SqlParameter("pDepartmentNumber", departmentNumber);
            var result = _context.Database.SqlQuery<ChargesReportCustomModel>(spName, sqlParameters);
            return result.ToList();
        }

        /// <summary>
        /// Gets the charges detail report.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="fromdate">The fromdate.</param>
        /// <param name="tillDate">The till date.</param>
        /// <param name="departmentNumber">The department number.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public List<ChargesReportCustomModel> GetChargesDetailReport(int corporateId, int facilityId, DateTime? fromdate, DateTime? tillDate,
            decimal? departmentNumber, int type, int payorId)
        {
            var spName = string.Format("EXEC {0} @pCorporateId, @pFacilityId,@pReportType,@pFromDate,@pTillDate,@pDepartmentNumber,@pPayorId", StoredProcedures.SPROC_GET_REP_CHargeReportDetails);
            var sqlParameters = new SqlParameter[7];
            sqlParameters[0] = new SqlParameter("pCorporateId", corporateId);
            sqlParameters[1] = new SqlParameter("pFacilityId", facilityId);
            sqlParameters[2] = new SqlParameter("pReportType", type);
            sqlParameters[3] = new SqlParameter("pFromDate", fromdate);
            sqlParameters[4] = new SqlParameter("pTillDate", tillDate);
            sqlParameters[5] = new SqlParameter("pDepartmentNumber", departmentNumber);
            sqlParameters[6] = new SqlParameter("pPayorId", payorId);
            var result = _context.Database.SqlQuery<ChargesReportCustomModel>(spName, sqlParameters);
            return result.ToList();

        }


        private List<AuditLogCustomModel> GetFilteredListByCorporateId(int corporateId, List<AuditLogCustomModel> list, bool isAll)
        {
            if (corporateId > 0)
            {
                var usersList = _uRepository.Where(u => u.CorporateId == corporateId && u.IsActive == true && u.IsDeleted != true).Select(u1 => u1.UserID).ToList();
                list = list.Where(u => u.UserId != null && usersList.Contains(u.UserId.Value)).ToList();

            }
            else if (corporateId == 0 && !isAll)
            {
                var usersList = _uRepository.Where(u => u.CorporateId == 12 && u.IsActive == true && u.IsDeleted != true).Select(u1 => u1.UserID).ToList();
                list = list.Where(u => u.UserId != null && usersList.Contains(u.UserId.Value)).ToList();
            }
            return list;
        }



        public List<ScrubHeaderCustomModel> GetScrubberAndErrorSummaryReport(string reportingTypeId, DateTime? fromDate,
            DateTime? tillDate,
            int facilityId)
        {
            return new List<ScrubHeaderCustomModel>();
        }




        public List<PhysicianActivityCustomModel> GetPhysicianActivityReport(int corporateId, int facilityId, DateTime? fromDate,
            DateTime? tillDate, int physicianId)
        {
            var spName = string.Format("EXEC {0} @pCorporateId, @pFacilityId, @pFromDate, @pTillDate,@pPhysicianId", StoredProcedures.SPROC_GET_REP_PhysicianChargeReportDetails);
            var sqlParameters = new SqlParameter[5];
            sqlParameters[0] = new SqlParameter("pCorporateId", corporateId);
            sqlParameters[1] = new SqlParameter("pFacilityId", facilityId);
            sqlParameters[2] = new SqlParameter("pFromDate", fromDate);
            sqlParameters[3] = new SqlParameter("pTillDate", tillDate);
            sqlParameters[4] = new SqlParameter("pPhysicianId", physicianId);
            var result = _context.Database.SqlQuery<PhysicianActivityCustomModel>(spName, sqlParameters);
            return result.ToList();
        }


        public List<PhysicianDepartmentUtilizationCustomModel> GetPhysicianUtilizationReport(int corporateId, DateTime? fromDate,
        DateTime? tillDate, int displayflag, int facilityId, int physicianId, int departmentId)
        {
            var list = GetUtilizationReport(corporateId, fromDate, tillDate, displayflag, facilityId, physicianId, departmentId);

            return list;
        }
        private List<PhysicianDepartmentUtilizationCustomModel> GetUtilizationReport(int corporateId, DateTime? fromDate,
          DateTime? tillDate, int displayflag, int facilityId, int physicianId, int departmentId)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @pCorporateID,@pFromDate, @pTillDate,@pDisplayFlag,@pFacilityID,@pPhysicianID,@pDepartmentID", StoredProcedures.SPROC_GetUtilization);
                    var sqlParameters = new SqlParameter[7];
                    sqlParameters[0] = new SqlParameter("pCorporateID", corporateId);

                    sqlParameters[1] = new SqlParameter("pFromDate", fromDate);
                    sqlParameters[2] = new SqlParameter("pTillDate", tillDate);
                    sqlParameters[3] = new SqlParameter("pDisplayFlag", displayflag);
                    sqlParameters[4] = new SqlParameter("pFacilityID", facilityId);
                    sqlParameters[5] = new SqlParameter("pPhysicianID", physicianId);
                    sqlParameters[6] = new SqlParameter("pDepartmentID", departmentId);
                    IEnumerable<PhysicianDepartmentUtilizationCustomModel> result = _context.Database.SqlQuery<PhysicianDepartmentUtilizationCustomModel>(spName, sqlParameters);
                    return result.ToList();
                }
            }
            catch (Exception)
            {
                //throw ex;
            }
            return new List<PhysicianDepartmentUtilizationCustomModel>();
        }


        public List<PhysicianDepartmentUtilizationCustomModel> GetDepartmentUtilizationReport(int corporateId, DateTime? fromDate,
            DateTime? tillDate, int displayflag, int facilityId, int physicianId, int departmentId)
        {
            var list = GetUtilizationReport(corporateId, fromDate, tillDate, displayflag, facilityId, physicianId, departmentId);
            return list;
        }

        /// <summary>
        /// Gets the future charge report.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="tillDate">The till date.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public List<FutureOpenOrderCustomModel> GetFutureChargeReport(int corporateId, DateTime? fromDate, DateTime? tillDate, int facilityId)
        {
            var spName =
                        string.Format(
                            "EXEC {0} @pFromDate, @pTillDate,@pCID,@pFID",
                            StoredProcedures.SPROC_GetFutureChargeReport);
            var sqlParameters = new SqlParameter[4];
            //sqlParameters[0] = new SqlParameter("pPatientId", corporateId);
            sqlParameters[0] = new SqlParameter("pFromDate", fromDate);
            sqlParameters[1] = new SqlParameter("pTillDate", tillDate);
            sqlParameters[2] = new SqlParameter("pCID", corporateId);
            sqlParameters[3] = new SqlParameter("pFID", facilityId);
            var result = _context.Database.SqlQuery<FutureOpenOrderCustomModel>(spName, sqlParameters);
            return result.ToList();
        }


        /// <summary>
        /// Gets the password change log_ sp.
        /// </summary>
        /// <param name="fromDate">From date.</param>
        /// <param name="tillDate">The till date.</param>
        /// <param name="isAll">if set to <c>true</c> [is all].</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public List<AuditLogCustomModel> GetPasswordChangeLog_SP(DateTime fromDate, DateTime tillDate, bool isAll, int corporateId, int facilityId)
        {
            string spName = string.Format(
                       "EXEC {0} @lDateFrom, @lDateTill,@lCID,@lFID",
                       StoredProcedures.SPROC_REP_PasswordChangelog);
            var sqlParameters = new SqlParameter[4];
            sqlParameters[0] = new SqlParameter("lDateFrom", fromDate);
            sqlParameters[1] = new SqlParameter("lDateTill", tillDate);
            sqlParameters[2] = new SqlParameter("lCID", corporateId);
            sqlParameters[3] = new SqlParameter("lFID", facilityId);
            var result = _context.Database.SqlQuery<AuditLogCustomModel>(spName, sqlParameters);
            return result.ToList();
        }

        /// <summary>
        /// Gets the password disable log_ sp.
        /// </summary>
        /// <param name="fromDate">From date.</param>
        /// <param name="tillDate">The till date.</param>
        /// <param name="isAll">if set to <c>true</c> [is all].</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public List<AuditLogCustomModel> GetPasswordDisableLog_SP(DateTime fromDate, DateTime tillDate, bool isAll, int corporateId, int facilityId)
        {
            string spName = string.Format(
                         "EXEC {0} @lDateFrom, @lDateTill,@lCID,@lFID",
                         StoredProcedures.SPROC_REP_PasswordDisablelog);
            var sqlParameters = new SqlParameter[4];
            sqlParameters[0] = new SqlParameter("lDateFrom", fromDate);
            sqlParameters[1] = new SqlParameter("lDateTill", tillDate);
            sqlParameters[2] = new SqlParameter("lCID", corporateId);
            sqlParameters[3] = new SqlParameter("lFID", facilityId);
            IEnumerable<AuditLogCustomModel> result = _context.Database.SqlQuery<AuditLogCustomModel>(spName, sqlParameters);
            return result.ToList();
        }

        /// <summary>
        /// Gets the user login activity list_ sp.
        /// </summary>
        /// <param name="fromDate">From date.</param>
        /// <param name="tillDate">The till date.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public List<LoginTrackingCustomModel> GetUserLoginActivityList_SP(DateTime? fromDate, DateTime? tillDate, int userId, int corporateId, int facilityId)
        {
            var spName = string.Format("EXEC {0} @lDateFrom,@lDateTill,@lUserId,@lCId,@lFId", StoredProcedures.SPROC_REP_UserActivityLog);
            var sqlParameters = new SqlParameter[5];
            sqlParameters[0] = new SqlParameter("lDateFrom", SqlDbType.VarChar);
            sqlParameters[0].Value = (fromDate != null) ? fromDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : sqlParameters[0].Value = null;
            sqlParameters[1] = new SqlParameter("lDateTill", SqlDbType.VarChar);
            sqlParameters[1].Value = (tillDate != null) ? tillDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : sqlParameters[1].Value = null;
            sqlParameters[2] = new SqlParameter("lUserId", userId);
            sqlParameters[3] = new SqlParameter("lCId", corporateId);
            sqlParameters[4] = new SqlParameter("lFId", facilityId);
            IEnumerable<LoginTrackingCustomModel> result = _context.Database.SqlQuery<LoginTrackingCustomModel>(spName, sqlParameters);
            return result.ToList();
        }


        /// <summary>
        /// Gets the reconciliation report_ monthly.
        /// </summary>
        /// <param name="corporateid">The corporateid.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="date">The date.</param>
        /// <param name="viewtype">The viewtype.</param>
        /// <param name="reportType">Type of the report.</param>
        /// <returns></returns>
        public List<ReconcilationReportCustomModel> GetReconciliationReport_Monthly(int corporateid, int facilityId, DateTime? date, string viewtype, int reportType)
        {
            string spName = string.Empty;
            switch (reportType)
            {
                case 9: //Payor WISE Reconciliation
                    spName = string.Format(
                        "EXEC {0} @pCorporateID, @pFacilityID, @pAsOnDate, @pViewType",
                        StoredProcedures.SPROC_GetReconcilationARPayorWise_Monthly);
                    break;
                case 10: //Patinet WISE Reconciliation
                    spName = string.Format(
                        "EXEC {0} @pCorporateID, @pFacilityID, @pAsOnDate, @pViewType",
                        StoredProcedures.SPROC_GetReconcilationARPatientWise_Monthly);
                    break;
            }
            var sqlParameters = new SqlParameter[4];
            sqlParameters[0] = new SqlParameter("pCorporateID", corporateid);
            sqlParameters[1] = new SqlParameter("pFacilityID", facilityId);
            sqlParameters[2] = new SqlParameter("pAsOnDate", SqlDbType.VarChar);
            sqlParameters[2].Value = (date != null)
                                         ? date.Value.ToString("yyyy-MM-dd HH:mm:ss")
                                         : sqlParameters[2].Value = null;
            sqlParameters[3] = new SqlParameter("pViewType", viewtype);
            var result = _context.Database.SqlQuery<ReconcilationReportCustomModel>(spName, sqlParameters);
            return result.ToList();
        }

        /// <summary>
        /// Gets the reconciliation report_ weekly.
        /// </summary>
        /// <param name="corporateid">The corporateid.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="date">The date.</param>
        /// <param name="viewtype">The viewtype.</param>
        /// <param name="reportType">Type of the report.</param>
        /// <returns></returns>
        public List<ReconcilationReportCustomModel> GetReconciliationReport_Weekly(int corporateid, int facilityId, DateTime? date, string viewtype, int reportType)
        {
            string spName = string.Empty;
            switch (reportType)
            {
                case 9: //Payor WISE Reconciliation
                    spName = string.Format("EXEC {0} @pCorporateID, @pFacilityID, @pAsOnDate, @pViewType",
                        StoredProcedures.SPROC_GetReconcilationARPayorWise_Weekly);
                    break;
                case 10: //Patinet WISE Reconciliation
                    spName = string.Format("EXEC {0} @pCorporateID, @pFacilityID, @pAsOnDate, @pViewType",
                        StoredProcedures.SPROC_GetReconcilationARPatientWise_Weekly);
                    break;
            }
            var sqlParameters = new SqlParameter[4];
            sqlParameters[0] = new SqlParameter("pCorporateID", corporateid);
            sqlParameters[1] = new SqlParameter("pFacilityID", facilityId);
            sqlParameters[2] = new SqlParameter("pAsOnDate", SqlDbType.VarChar);
            sqlParameters[2].Value = (date != null)
                ? date.Value.ToString("yyyy-MM-dd HH:mm:ss")
                : sqlParameters[2].Value = null;
            sqlParameters[3] = new SqlParameter("pViewType", viewtype);
            DbRawSqlQuery<ReconcilationReportCustomModel> result =
                _context.Database.SqlQuery<ReconcilationReportCustomModel>(spName, sqlParameters);
            return result.ToList();
        }



        public List<AuditLogCustomModel> GetSchedulingLogActivity(DateTime? fromDate, DateTime? tillDate, int corporateId, int facilityId)
        {
            string spName = string.Format("EXEC {0} @pFromDate, @pTillDate, @pFacilityId, @pCorporateId",
                      StoredProcedures.SPROC_GetSchedulingAuditLog);
            var sqlParameters = new SqlParameter[4];
            sqlParameters[0] = new SqlParameter("pFromDate", fromDate);
            sqlParameters[1] = new SqlParameter("pTillDate", tillDate);
            sqlParameters[2] = new SqlParameter("pFacilityId", facilityId);
            sqlParameters[3] = new SqlParameter("pCorporateId", corporateId);

            IEnumerable<AuditLogCustomModel> result =
                _context.Database.SqlQuery<AuditLogCustomModel>(spName, sqlParameters);
            return result.ToList();
        }



    }
}
