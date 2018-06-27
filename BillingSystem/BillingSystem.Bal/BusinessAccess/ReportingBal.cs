using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BillingSystem.Bal.BusinessAccess
{
    public class ReportingBal : BaseBal
    {
        private readonly IRepository<Users> _uRepository;
        private readonly IRepository<UserRole> _urRepository;
        //private PhysicianActivityMapper PhysicianActivityMapper { get; set; }

        public ReportingBal()
        {
            //PhysicianActivityMapper = new PhysicianActivityMapper();
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
            using (var rep = UnitOfWork.LoginTrackingRepository)
            {
                //var ltReports = rep.GetAll().ToList();
                var ltReports = userId > 0
                    ? rep.Where(
                        l =>
                            l.CreatedDate != null && ((DateTime)l.CreatedDate) >= fromDate &&
                            ((DateTime)l.CreatedDate) <= tillDate && l.ID == userId).ToList()
                    : rep.Where(
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

                    var userroleBal = new UserRoleBal();
                    foreach (var item in list)
                    {
                        var getUserRoles = userroleBal.GetUserRolesByUserId(item.ID);
                        var userRoles = UserRoles(getUserRoles);
                        var getUsersFacility = UserFacilities(getUserRoles);
                        item.AssignedRoles = userRoles;
                        item.AssignedFacilities = getUsersFacility;
                    }

                }
            }
            return list;
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
                            using (var facBal = new FacilityBal())
                            {
                                if (string.IsNullOrEmpty(facilityNames))
                                    facilityNames = facBal.GetFacilityNameById(f.FacilityId);
                                else
                                    facilityNames += string.Format(", {0}", facBal.GetFacilityNameById(f.FacilityId));
                            }
                        }
                    }
                }
            }
            return facilityNames;
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
            using (var rep = UnitOfWork.AuditLogRepository)
            {
                var ltReports = corporateId == 0
                    ? rep.Where(l => l.CreatedDate != null && l.CreatedDate >= fromDate && l.CreatedDate <= tillDate)
                        .ToList()
                    : (userId > 0
                        ? rep.Where(
                            l =>
                                l.CreatedDate != null && l.CreatedDate >= fromDate && l.CreatedDate <= tillDate &&
                                l.UserId == userId).ToList()
                        : rep.Where(l => l.CreatedDate != null && l.CreatedDate >= fromDate && l.CreatedDate <= tillDate)
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

                    var userroleBal = new UserRoleBal();
                    foreach (var item in list)
                    {
                        var getUserRoles = userroleBal.GetUserRolesByUserId(Convert.ToInt32(item.UserId));
                        var userRoles = UserRoles(getUserRoles);
                        var getUsersFacility = UserFacilities(getUserRoles);
                        item.AssignedRoles = userRoles;
                        item.AssignedFacilities = getUsersFacility;
                    }
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
            using (var rep = UnitOfWork.AuditLogRepository)
            {
                var ltReports = isAll //&& corporateId == 0
                    ? rep.Where(
                        l =>
                            //l.CreatedDate != null && l.CreatedDate >= fromDate &&
                            //l.CreatedDate <= tillDate && l.FieldName.ToLower().Equals("password")).ToList()
                            l.FieldName.ToLower().Equals("password")).ToList()
                    : rep.Where(
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

                    var userroleBal = new UserRoleBal();
                    foreach (var item in list)
                    {
                        var getUserRoles = userroleBal.GetUserRolesByUserId(Convert.ToInt32(item.UserId));
                        var userRoles = UserRoles(getUserRoles);
                        var getUsersFacility = UserFacilities(getUserRoles);
                        item.AssignedRoles = userRoles;
                        item.AssignedFacilities = getUsersFacility;
                    }
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
            using (var rep = UnitOfWork.PatientInfoRepository)
            {
                var result = rep.GetAgingReport(corporateid, facilityId, date, reportType);
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
            }
            return list;
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
            using (var rep = UnitOfWork.PatientInfoRepository)
            {
                var result = rep.GetReconciliationReport(corporateid, facilityId, date, viewtype, reportType);
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
            using (var rep = UnitOfWork.BillHeaderRepository)
            {
                var result = rep.GetRevenueForecastFacility(corporateId, facilityId, fromDate, tillDate);
                return result;
            }
        }

        /// <summary>
        /// Gets the revenue forecast facility by patient.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public List<RevenueForecast> GetRevenueForecastFacilityByPatient(int patientId)
        {
            using (var rep = UnitOfWork.BillHeaderRepository)
            {
                var result = rep.GetRevenueForecastFacilityByPatientId(patientId);
                return result;
            }
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
            using (var rep = UnitOfWork.LoginTrackingRepository)
            {
                var result = rep.GetLoginTimeDayNightWiseReport(corporateId, facilityId, fromDate, tillDate, userId);
                return result;
            }
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
            using (var rep = UnitOfWork.LoginTrackingRepository)
            {
                var ltReports = rep.GetAll().ToList();
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
                    var userroleBal = new UserRoleBal();
                    foreach (var item in list)
                    {
                        var getUserRoles = userroleBal.GetUserRolesByUserId(item.ID);
                        var userRoles = UserRoles(getUserRoles);
                        var getUsersFacility = UserFacilities(getUserRoles);
                        item.AssignedRoles = userRoles;
                        item.AssignedFacilities = getUsersFacility;
                    }
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
            using (var rep = UnitOfWork.BillHeaderRepository)
            {
                var transactiondetails = rep.GetClaimTransDetails(corporateId, facilityId, fromdate, tillDate, displayby);
                return transactiondetails;
            }
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
            using (var rep = UnitOfWork.DenialRepository)
            {
                var denialdetails = rep.GetDenialCodesReport(corporateId, facilityId, fromdate, tillDate, displayby);
                return denialdetails;
            }
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
            using (var rep = UnitOfWork.JournalEntrySupportRepository)
            {
                var journalEntrySupportDetails = rep.GetJournalEntrySupport(corporateId, facilityId, fromdate, tillDate, displayby);
                //var grosstotal = journalentrysupportdetails.sum(x => x.gross);
                //var journalentrysupportreportcustommodel = new journalentrysupportreportcustommodel()
                //{
                //    activitydate = null,
                //    activitytype = "total",
                //    activitycode = "",
                //    activitydescription = "",
                //    debitaccount = "",
                //    creditaccount = "",
                //    gross = grosstotal
                //};
                //journalentrysupportdetails.add(journalentrysupportreportcustommodel);
                return journalEntrySupportDetails;
            }
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
            using (var rep = UnitOfWork.PatientInfoRepository)
            {
                var result = rep.GetPatientAgeingPayorWise(corporateid, facilityId, date, companyId);
                if (result != null && result.Any())
                {
                    list = result.ToList();
                    //list.AddRange(result.Select(item => new AgingReportCustomModel
                    //{
                    //    ID = item.ID,
                    //    Name = item.Name,
                    //    OnTime = item.OnTime,
                    //    Days1To30 = item.Days1To30,
                    //    Days31To60 = item.Days31To60,
                    //    Days61To90 = item.Days61To90,
                    //    Days91To120 = item.Days91To120,
                    //    Days121To150 = item.Days121To150,
                    //    Days151To180 = item.Days151To180,
                    //    Days181More = item.Days181More,
                    //    Total = item.Total,
                    //    DueDate = item.DueDate,
                    //    EncounterEnd = item.EncounterEnd,
                    //    EncounterNumber = item.EncounterNumber
                    //}));
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
            var list = new List<ScrubEditTrackCustomModel>();
            using (var scrubreportrep = UnitOfWork.ScrubEditTrackRepository)
            {
                list = scrubreportrep.GetCorrectionLogData(corporateId, facilityId, fromdate, tillDate);
                //        scrubreportrep.Where(
                //            x =>
                //                x.CorporateId == corporateId && x.FacilityId == facilityId &&
                //                ((!displayall && x.CreatedDate >= fromdate && x.CreatedDate <= tillDate) || displayall))
                //            .ToList();

                //    if (scrubtrackList.Any())
                //    {
                //        list.AddRange(scrubtrackList.Select(item => new ScrubEditTrackCustomModel
                //        {
                //            ScrubEditTrackID = item.ScrubEditTrackID,
                //            TrackRuleMasterID = item.TrackRuleMasterID,
                //            TrackRuleStepID = item.TrackRuleStepID,
                //            TrackType = item.TrackType,
                //            TrackTable = item.TrackTable,
                //            TrackColumn = item.TrackColumn,
                //            TrackKeyColumn = item.TrackKeyColumn,
                //            TrackValueBefore = item.TrackValueBefore,
                //            TrackValueAfter = item.TrackValueAfter,
                //            TrackKeyIDValue = item.TrackKeyIDValue,
                //            TrackSide = item.TrackSide,
                //            IsActive = item.IsActive,
                //            CreatedBy = item.CreatedBy,
                //            CreatedDate = item.CreatedDate,
                //            CorporateId = item.CorporateId,
                //            FacilityId = item.FacilityId,
                //            TrackSideName = item.TrackSide == "LHS" ? "Left Hand Side" : "Right Hand Side",
                //            CreatedByName = GetNameByUserId(item.CreatedBy),
                //            CorrectionCode = item.CorrectionCode,
                //            CorrectionCodeText =
                //                GetNameByGlobalCodeValue(Convert.ToInt32(item.CorrectionCode).ToString(), "0101"),
                //            BillNumber =
                //                item.BillHeaderId == null
                //                    ? "NA"
                //                    : GetBillNumberByBillHeaderId(Convert.ToInt32(item.BillHeaderId))
                //        }));
                //    }
            }
            return list.OrderByDescending(x => x.CreatedDate).ThenBy(f => f.CreatedByName).ToList();
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
            using (var rep = UnitOfWork.DashboardTransactionCounterRepository)
            {
                var list = rep.GetChargesReport(corporateId, facilityId, fromdate, tillDate, true, type, departmentNumber);
                //list = departmentNumber != Convert.ToDecimal(0.00)
                //    ? list.Where(x => x.Department == departmentNumber).ToList()
                //    : list;
                return list;
            }
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
            using (var rep = UnitOfWork.DashboardTransactionCounterRepository)
            {
                var list = rep.GetChargesDetailReport(corporateId, facilityId, fromdate, tillDate, true, type, departmentNumber, payorId);
                //list = departmentNumber != Convert.ToDecimal(0.00)
                //     ? list.Where(x => x.Department == departmentNumber).ToList()
                //     : list;
                return list;
            }
        }


        private List<AuditLogCustomModel> GetFilteredListByCorporateId(int corporateId, List<AuditLogCustomModel> list, bool isAll)
        {
            if (corporateId > 0)
            {
                using (var rep = UnitOfWork.UsersRepository)
                {
                    var usersList = rep.Where(u => u.CorporateId == corporateId && u.IsActive == true && u.IsDeleted != true).Select(u1 => u1.UserID).ToList();
                    list = list.Where(u => u.UserId != null && usersList.Contains(u.UserId.Value)).ToList();
                }
            }
            else if (corporateId == 0 && !isAll)
            {
                using (var rep = UnitOfWork.UsersRepository)
                {
                    var usersList = rep.Where(u => u.CorporateId == 12 && u.IsActive == true && u.IsDeleted != true).Select(u1 => u1.UserID).ToList();
                    list = list.Where(u => u.UserId != null && usersList.Contains(u.UserId.Value)).ToList();
                }
            }
            return list;
        }



        public List<ScrubHeaderCustomModel> GetScrubberAndErrorSummaryReport(string reportingTypeId, DateTime? fromDate,
            DateTime? tillDate,
            int facilityId)
        {
            using (var rep = UnitOfWork.ScrubHeaderRepository)
            {

            }
            return new List<ScrubHeaderCustomModel>();
        }




        public List<PhysicianActivityCustomModel> GetPhysicianActivityReport(int corporateId, int facilityId, DateTime? fromDate,
            DateTime? tillDate, int physicianId)
        {
            var list = new List<PhysicianActivityCustomModel>();
            using (var rep = UnitOfWork.ScrubHeaderRepository)
            {
                list = rep.GetPhysicianActivityReport(corporateId, facilityId, fromDate, tillDate, physicianId);
            }
            return list;
        }


        public List<PhysicianDepartmentUtilizationCustomModel> GetPhysicianUtilizationReport(int corporateId, DateTime? fromDate,
            DateTime? tillDate, int displayflag, int facilityId, int physicianId, int departmentId)
        {
            var list = new List<PhysicianDepartmentUtilizationCustomModel>();
            using (var rep = UnitOfWork.ScrubHeaderRepository)
            {
                list = rep.GetUtilizationReport(corporateId, fromDate, tillDate, displayflag, facilityId, physicianId, departmentId);
            }
            return list;
        }


        public List<PhysicianDepartmentUtilizationCustomModel> GetDepartmentUtilizationReport(int corporateId, DateTime? fromDate,
            DateTime? tillDate, int displayflag, int facilityId, int physicianId, int departmentId)
        {
            var list = new List<PhysicianDepartmentUtilizationCustomModel>();
            using (var rep = UnitOfWork.ScrubHeaderRepository)
            {
                list = rep.GetUtilizationReport(corporateId, fromDate, tillDate, displayflag, facilityId, physicianId, departmentId);
            }
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
            var list = new List<FutureOpenOrderCustomModel>();
            using (var rep = UnitOfWork.ScrubHeaderRepository)
            {
                list = rep.GetFutureChargeReport(corporateId, fromDate, tillDate, facilityId);
            }
            return list;
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
            var list = new List<AuditLogCustomModel>();
            using (var rep = UnitOfWork.AuditLogRepository)
            {
                list = rep.GetPasswordChangesLog(fromDate, tillDate, isAll, corporateId, facilityId);
            }
            return list;
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
            var list = new List<AuditLogCustomModel>();
            using (var rep = UnitOfWork.AuditLogRepository)
            {
                list = rep.GetPasswordDisableLog(fromDate, tillDate, isAll, corporateId, facilityId);
            }
            return list;
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
            var list = new List<LoginTrackingCustomModel>();
            using (var rep = UnitOfWork.LoginTrackingRepository)
            {
                list = rep.UserLoginActivity_SP(fromDate, tillDate, userId, corporateId, facilityId);
            }
            return list;
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
            var list = new List<ReconcilationReportCustomModel>();
            using (var rep = UnitOfWork.PatientInfoRepository)
            {
                list = rep.GetReconciliationReport_Monthly(corporateid, facilityId, date, viewtype, reportType);
            }
            return list;
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
            var list = new List<ReconcilationReportCustomModel>();
            using (var rep = UnitOfWork.PatientInfoRepository)
            {
                list = rep.GetReconciliationReport_Weekly(corporateid, facilityId, date, viewtype, reportType);
            }
            return list;
        }



        public List<AuditLogCustomModel> GetSchedulingLogActivity(DateTime? fromDate, DateTime? tillDate, int corporateId, int facilityId)
        {
            var list = new List<AuditLogCustomModel>();
            using (var rep = UnitOfWork.PatientInfoRepository)
            {
                list = rep.GetSchedulingAuditLog(fromDate, tillDate, corporateId, facilityId);
            }
            return list;
        }



    }
}
