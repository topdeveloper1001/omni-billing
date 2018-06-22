using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Repository.Common;
using BillingSystem.Model.EntityDto;
using System.Threading.Tasks;
using System.Text;
using System.Data.Common;
using BillingSystem.Common;

namespace BillingSystem.Repository.GenericRepository
{
    public class PatientInfoRepository : GenericRepository<PatientInfo>
    {
        private readonly DbContext _context;

        public PatientInfoRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
            _context = context;
        }

        /// <summary>
        ///     Gets the ageing report.
        /// </summary>
        /// <param name="CorporateID">The corporate identifier.</param>
        /// <param name="FacilityID">The facility identifier.</param>
        /// <param name="date">The date.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public List<AgingReportCustomModel> GetAgingReport(int CorporateID, int FacilityID, DateTime? date, int type)
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
                    IEnumerable<AgingReportCustomModel> result =
                        _context.Database.SqlQuery<AgingReportCustomModel>(spName, sqlParameters);
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
        /// Gets the ageing report.
        /// </summary>
        /// <param name="CorporateID">The corporate identifier.</param>
        /// <param name="FacilityID">The facility identifier.</param>
        /// <param name="date">The date.</param>
        /// <param name="companyid">The companyid.</param>
        /// <returns></returns>
        public List<AgingReportCustomModel> GetPatientAgeingPayorWise(int CorporateID, int FacilityID, DateTime? date, string companyid)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @pCorporateID, @pFacilityID, @pAsOnDate, @PayorID",
                        StoredProcedures.SPROC_GetPatientAgeingPayorWise);

                    var sqlParameters = new SqlParameter[4];
                    sqlParameters[0] = new SqlParameter("pCorporateID", CorporateID);
                    sqlParameters[1] = new SqlParameter("pFacilityID", FacilityID);
                    sqlParameters[2] = new SqlParameter("pAsOnDate", SqlDbType.VarChar);
                    sqlParameters[2].Value = (date != null)
                        ? date.Value.ToString("yyyy-MM-dd HH:mm:ss")
                        : sqlParameters[2].Value = null;
                    sqlParameters[3] = new SqlParameter("PayorID", companyid);
                    IEnumerable<AgingReportCustomModel> result =
                        _context.Database.SqlQuery<AgingReportCustomModel>(spName, sqlParameters);
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
        ///     Gets the reconciliation report.
        /// </summary>
        /// <param name="CorporateID">The corporate identifier.</param>
        /// <param name="FacilityID">The facility identifier.</param>
        /// <param name="date">The date.</param>
        /// <param name="viewType">Type of the view.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public List<ReconcilationReportCustomModel> GetReconciliationReport(int CorporateID, int FacilityID,
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
                    DbRawSqlQuery<ReconcilationReportCustomModel> result =
                        _context.Database.SqlQuery<ReconcilationReportCustomModel>(spName, sqlParameters);
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
        /// Gets the x payment return denial claims.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public List<PatientInfoXReturnPaymentCustomModel> GetXPaymentReturnDenialClaims(int corporateId, int facilityId)
        {
            //SPROC_GetXPaymentReturnDenialClaims
            try
            {
                if (_context != null)
                {
                    string spName = string.Format("EXEC {0} @pCorporateID, @pFacilityID",
                        StoredProcedures.SPROC_GetXPaymentReturnDenialClaims);
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
        /// Gets the XML missing data.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public List<MissingDataCustomModel> GetXMLMissingData(int corporateId, int facilityId)
        {
            try
            {
                if (_context != null)
                {
                    string spName = string.Format("EXEC {0} @pCorporateID, @pFacilityID",
                        StoredProcedures.SPROC_GetXMLMissingDataDetail);
                    var sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("pCorporateID", corporateId);
                    sqlParameters[1] = new SqlParameter("pFacilityID", facilityId);
                    IEnumerable<MissingDataCustomModel> result =
                        _context.Database.SqlQuery<MissingDataCustomModel>(spName, sqlParameters);
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
        /// Gets the patient encounter detail.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public List<PatientInfoXReturnPaymentCustomModel> GetPatientEncounterDetail(int corporateId, int facilityId)
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
        /// Gets the x payment return denial claims by patient identifier.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <param name="billHeaderId">The bill header identifier.</param>
        /// <returns></returns>
        public List<PatientInfoXReturnPaymentCustomModel> GetXPaymentReturnDenialClaimsByPatientId(int patientId, int encounterId, int billHeaderId)
        {
            //SPROC_GetXPaymentReturnDenialClaims
            try
            {
                if (_context != null)
                {
                    string spName = string.Format("EXEC {0} @pPatientId, @pEncounterId, @pBillHeaderId",
                        StoredProcedures.SPROC_GetXPaymentReturnDenialClaimsByPatientId);
                    var sqlParameters = new SqlParameter[3];
                    sqlParameters[0] = new SqlParameter("pPatientId", patientId);
                    sqlParameters[1] = new SqlParameter("pEncounterId", encounterId);
                    sqlParameters[2] = new SqlParameter("pBillHeaderId", billHeaderId);
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
        /// Gets the reconciliation report_ monthly.
        /// </summary>
        /// <param name="CorporateID">The corporate identifier.</param>
        /// <param name="FacilityID">The facility identifier.</param>
        /// <param name="date">The date.</param>
        /// <param name="viewType">Type of the view.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public List<ReconcilationReportCustomModel> GetReconciliationReport_Monthly(
            int CorporateID,
            int FacilityID,
            DateTime? date,
            string viewType,
            int type)
        {
            try
            {
                if (_context != null)
                {
                    string spName = string.Empty;
                    switch (type)
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
                    sqlParameters[0] = new SqlParameter("pCorporateID", CorporateID);
                    sqlParameters[1] = new SqlParameter("pFacilityID", FacilityID);
                    sqlParameters[2] = new SqlParameter("pAsOnDate", SqlDbType.VarChar);
                    sqlParameters[2].Value = (date != null)
                                                 ? date.Value.ToString("yyyy-MM-dd HH:mm:ss")
                                                 : sqlParameters[2].Value = null;
                    sqlParameters[3] = new SqlParameter("pViewType", viewType);
                    DbRawSqlQuery<ReconcilationReportCustomModel> result =
                        _context.Database.SqlQuery<ReconcilationReportCustomModel>(spName, sqlParameters);
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
        /// Gets the reconciliation report_ weekly.
        /// </summary>
        /// <param name="CorporateID">The corporate identifier.</param>
        /// <param name="FacilityID">The facility identifier.</param>
        /// <param name="date">The date.</param>
        /// <param name="viewType">Type of the view.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public List<ReconcilationReportCustomModel> GetReconciliationReport_Weekly(int CorporateID, int FacilityID,
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
                                StoredProcedures.SPROC_GetReconcilationARPayorWise_Weekly);
                            break;
                        case 10: //Patinet WISE Reconciliation
                            spName = string.Format("EXEC {0} @pCorporateID, @pFacilityID, @pAsOnDate, @pViewType",
                                StoredProcedures.SPROC_GetReconcilationARPatientWise_Weekly);
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
                    DbRawSqlQuery<ReconcilationReportCustomModel> result =
                        _context.Database.SqlQuery<ReconcilationReportCustomModel>(spName, sqlParameters);
                    return result.ToList();
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<AuditLogCustomModel> GetSchedulingAuditLog(DateTime? fromDate, DateTime? tillDate, int corporateId, int facilityId)
        {
            //SPROC_GetXPaymentReturnDenialClaims
            try
            {
                if (_context != null)
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
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public PatientInfoCustomModel GetPatientDetails(long patientId, long encounterId = 0, bool showEncounters = false)
        {
            PatientInfoCustomModel vm = null;
            try
            {
                if (_context != null)
                {
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
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// This function returns all data related to Patient Info
        /// </summary>
        /// <param name="encId">Current Encounter ID</param>
        /// <param name="patientId">Current Patient ID</param>
        /// <param name="physicianId">Current Physician ID</param>
        /// <returns></returns>
        public PatientInfoViewData GetPatientInfoAll(long encId, long patientId)
        {
            try
            {
                var sqlParameters = new SqlParameter[2];
                sqlParameters[0] = new SqlParameter("PId", patientId);
                sqlParameters[1] = new SqlParameter("EId", encId);

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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<UserDto> SavePatientInfoAsync(PatientDto p)
        {
            //If the request goes for new patient, then Email is mandatory otherwise not.
            var email = p.PatientID == 0 ? p.Email : Convert.ToString(p.Email);
            var dt = string.IsNullOrEmpty(p.DeviceToken) ? string.Empty : p.DeviceToken;
            var pl = string.IsNullOrEmpty(p.Platform) ? string.Empty : p.Platform;
            var city = !string.IsNullOrEmpty(p.City) ? p.City : string.Empty;

            var sqlParameters = new SqlParameter[14];
            sqlParameters[0] = new SqlParameter("pPid", p.PatientID);
            sqlParameters[1] = new SqlParameter("pFirstName", p.FirstName);
            sqlParameters[2] = new SqlParameter("pLastName", p.LastName);
            sqlParameters[3] = new SqlParameter("pDob", p.DOB);
            sqlParameters[4] = new SqlParameter("pEmail", email);
            sqlParameters[5] = new SqlParameter("pEmirates", p.EmiratesID);
            sqlParameters[6] = new SqlParameter("pPhone", p.PhoneNo);
            sqlParameters[7] = new SqlParameter("pPwd", p.Password);
            sqlParameters[8] = new SqlParameter("pGender", p.Gender);
            sqlParameters[9] = new SqlParameter("pDeviceToken", dt);
            sqlParameters[10] = new SqlParameter("pPlatform", pl);
            sqlParameters[11] = new SqlParameter("pCityId", p.CityId);
            sqlParameters[12] = new SqlParameter("pStateId", p.StateId);
            sqlParameters[13] = new SqlParameter("pCountryId", p.CountryId);

            using (var ms = _context.MultiResultSetSqlQuery(StoredProcsiOS.iSprocSavePatientInfo.ToString(), parameters: sqlParameters, isCompiled: false))
            {
                var isSaved = ms.ResultSetFor<bool>().FirstOrDefault();
                if (isSaved)
                {
                    var list = await ms.GetResultWithJsonAsync<UserDto>(JsonResultsArray.UserDto.ToString());
                    return list.FirstOrDefault();
                }
            }

            return Enumerable.Empty<UserDto>().FirstOrDefault();
        }

        public IEnumerable<PatientInfo> GetPatientSearchResults(long? facilityId, long? corporateId, string keyword)
        {
            var sqlParameters = new SqlParameter[3];
            sqlParameters[0] = new SqlParameter("pText", keyword);
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

        public async Task<UserDto> AuthenticateAsync(string username, string password, string deviceToken, string platform, bool isEmail, long userId = 0, bool isPatient = false)
        {
            var sqlParameters = new SqlParameter[7];
            sqlParameters[0] = new SqlParameter("pUsername", username);
            sqlParameters[1] = new SqlParameter("pPassword", password);
            sqlParameters[2] = new SqlParameter("pDeviceToken", deviceToken);
            sqlParameters[3] = new SqlParameter("pPlatform", platform);
            sqlParameters[4] = new SqlParameter("pIsEmail", isEmail);
            sqlParameters[5] = new SqlParameter("pUserId", userId);
            sqlParameters[6] = new SqlParameter("pIsPatient", isPatient);

            using (var multiResultSet = _context.MultiResultSetSqlQuery(StoredProcsiOS.iSprocAuthenticateUser.ToString(), parameters: sqlParameters, isCompiled: false))
            {
                var reader = await multiResultSet.GetReaderAsync();
                var isAuthenticated = multiResultSet.ResultSetFor<bool>(reader).FirstOrDefault();
                var isNext = isAuthenticated ? await reader.NextResultAsync() : false;
                if (isAuthenticated && isNext)
                {
                    var result = GenericHelper.GetJsonResponse<UserDto>(reader, "UserDto");
                    return result != null && result.Count > 0 ? result.FirstOrDefault() : null;
                }
            }
            return Enumerable.Empty<UserDto>().FirstOrDefault();
        }

        public async Task<DefaultDataDto> GetDefaultDataAsync(long userId)
        {
            var dto = new DefaultDataDto();
            var sqlParams = new SqlParameter[1] { new SqlParameter("pUserId", userId) };
            using (var ms = _context.MultiResultSetSqlQuery(StoredProcsiOS.iSprocGetDefaultData.ToString(), false, sqlParams))
            {
                //var result = await ms.GetResultWithJsonAsync<DefaultDataDto>(JsonResultsArray.Defaults.ToString());
                //return result.FirstOrDefault();
                dto.Country = (await ms.ResultSetForAsync<SelectList>()).FirstOrDefault();
                dto.State = (await ms.ResultSetForAsync<SelectList>()).FirstOrDefault();
                dto.City = (await ms.ResultSetForAsync<SelectList>()).FirstOrDefault();
            }
            return dto;
        }

        public async Task<List<PatientDto>> GetPatientsByUserIdAsync(long userId = 0)
        {
            var sqlParams = new SqlParameter[1];
            sqlParams[0] = new SqlParameter("pUserId", userId);
            using (var ms = _context.MultiResultSetSqlQuery(StoredProcsiOS.iSprocGetPatientsByUserId.ToString(), isCompiled: false, parameters: sqlParams))
            {
                var result = await ms.GetResultWithJsonAsync<PatientDto>(JsonResultsArray.PatientSearchResults.ToString());
                return result;
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


        public async Task<ResponseData> SavePatientInfo(PatientInfo obj, PatientInsuranceCustomModel insurance, PatientAddressRelation addressRelation
            , int userId, DateTime currentDate, string tokenId, string codeValue, DataTable dtDocsData)
        {
            if (addressRelation == null)
                addressRelation = new PatientAddressRelation();

            if (insurance.Startdate.Year == 1 || insurance.Expirydate.Year == 1)
            {
                var dtNow = DateTime.Now;
                insurance.Startdate = new DateTime(dtNow.Year, dtNow.Month, 1);
                insurance.Expirydate = insurance.Startdate.AddMonths(1).AddDays(-1);
            }

            SqlParameter[] sqlParams = new SqlParameter[47]
                {
                    new SqlParameter("@pPatientId", obj.PatientID),
                    new SqlParameter("@pFId", obj.FacilityId.GetValueOrDefault()),
                    new SqlParameter("@pUserId", userId),
                    new SqlParameter("@pCurrentDate", currentDate),
                    new SqlParameter("@pCId", obj.CorporateId.GetValueOrDefault()),
                    new SqlParameter("@pInsCompanyId", insurance.InsuranceCompanyId),
                    new SqlParameter("@pInsurancePlanId", insurance.InsurancePlanId),
                    new SqlParameter("@pMemberId", !string.IsNullOrEmpty(insurance.PersonHealthCareNumber)? insurance.PersonHealthCareNumber:string.Empty),
                    new SqlParameter("@pEmail", !string.IsNullOrEmpty(obj.PersonEmailAddress)? obj.PersonEmailAddress:string.Empty),
                    new SqlParameter("@pSSN", !string.IsNullOrEmpty(obj.PersonEmiratesIDNumber)? obj.PersonEmiratesIDNumber:string.Empty),
                    new SqlParameter("@pMRN", !string.IsNullOrEmpty(obj.PersonMedicalRecordNumber)? obj.PersonMedicalRecordNumber:string.Empty),
                    new SqlParameter("@pFinanceNo", !string.IsNullOrEmpty(obj.PersonFinancialNumber)? obj.PersonFinancialNumber:string.Empty),
                    new SqlParameter("@pMasterPatientNo", !string.IsNullOrEmpty(obj.PersonMasterPatientNumber)? obj.PersonMasterPatientNumber:string.Empty),
                    new SqlParameter("@pPassportNo", !string.IsNullOrEmpty(obj.PersonPassportNumber)? obj.PersonPassportNumber:string.Empty),
                    new SqlParameter("@pPassportExpiry", obj.PersonPassportExpirtyDate==null ? DateTime.Now.AddDays(-10) :obj.PersonPassportExpirtyDate.GetValueOrDefault()),
                    new SqlParameter("@pLastName", !string.IsNullOrEmpty(obj.PersonLastName)? obj.PersonLastName:string.Empty),
                    new SqlParameter("@pFirstName", !string.IsNullOrEmpty(obj.PersonFirstName)? obj.PersonFirstName:string.Empty),
                    new SqlParameter("@pNationality", !string.IsNullOrEmpty(obj.PersonNationality)? obj.PersonNationality:"199"),
                    new SqlParameter("@pVipStatus", !string.IsNullOrEmpty(obj.PersonVIP)? obj.PersonVIP:string.Empty),
                    new SqlParameter("@pGender", !string.IsNullOrEmpty(obj.PersonGender)?obj.PersonGender:string.Empty),
                    new SqlParameter("@pContactNo", !string.IsNullOrEmpty(obj.PersonContactNumber)?obj.PersonContactNumber:string.Empty),
                    new SqlParameter("@pDOB", obj.PersonBirthDate.GetValueOrDefault()),
                    new SqlParameter("@pAgeInYears", obj.PersonAge.GetValueOrDefault()),
                    new SqlParameter("@pMaritalStatus", !string.IsNullOrEmpty(obj.PersonMaritalStatus) ? obj.PersonMaritalStatus:string.Empty),
                    new SqlParameter("@pInsuranceStart", insurance.Startdate),
                    new SqlParameter("@pInsuranceEnd", insurance.Expirydate),
                    new SqlParameter("@pInsurancePolicyId", insurance.InsurancePolicyId),
                    new SqlParameter("@pInsuranceCompanyId2", insurance.CompanyId2),
                    new SqlParameter("@pInsurancePlanId2", insurance.Plan2),
                    new SqlParameter("@pMemberId2", !string.IsNullOrEmpty(insurance.PersonHealthCareNumber2)? insurance.PersonHealthCareNumber2:string.Empty),
                    new SqlParameter("@pInsuranceStart2", insurance.Policy2 <=0 ? DateTime.Now : insurance.StartDate2),
                    new SqlParameter("@pInsuranceEnd2", insurance.Policy2 <=0 ? DateTime.Now :insurance.EndDate2),
                    new SqlParameter("@pInsurancePolicyId2", insurance.Policy2),
                    new SqlParameter {
                                        ParameterName = "@pDataArray",
                                        SqlDbType = SqlDbType.Structured,
                                        Value = dtDocsData,
                                        TypeName = "ValuesArrayT"
                                    },
                    new SqlParameter("@pStreetAddress1", !string.IsNullOrEmpty(addressRelation.StreetAddress1)?addressRelation.StreetAddress1:string.Empty),
                    new SqlParameter("@pStreetAddress2", !string.IsNullOrEmpty(addressRelation.StreetAddress2)?addressRelation.StreetAddress2:string.Empty),
                    new SqlParameter("@pPOBox", string.IsNullOrEmpty(addressRelation.POBox) ? string.Empty : addressRelation.POBox),
                    new SqlParameter("@pCityId", addressRelation.CityID),
                    new SqlParameter("@pStateId", addressRelation.StateID),
                    new SqlParameter("@pCountryId", addressRelation.CountryID),
                    new SqlParameter("@pAddressFirstName", string.IsNullOrEmpty(addressRelation.FirstName) ? string.Empty : addressRelation.FirstName),
                    new SqlParameter("@pAddressLastName", string.IsNullOrEmpty(addressRelation.LastName) ? string.Empty : addressRelation.LastName),
                    new SqlParameter("@ZipCode", string.IsNullOrEmpty(addressRelation.ZipCode) ? string.Empty : addressRelation.ZipCode),
                    new SqlParameter("@pRelationType", addressRelation.PatientAddressRelationType),
                    new SqlParameter("@pToken", !string.IsNullOrEmpty(tokenId)? tokenId:string.Empty),
                    new SqlParameter("@pCode", !string.IsNullOrEmpty(codeValue)? codeValue:string.Empty),
                    new SqlParameter("@pEmiratesIDExpiry", obj.PersonEmiratesIDExpiration==null ? DateTime.Now.AddDays(-10) : obj.PersonEmiratesIDExpiration)
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
                    if ((insurance.Startdate != null && insurance.Startdate.Year == 1)
                        || (insurance.Expirydate != null && insurance.Expirydate.Year == 1))
                        return new ResponseData { Message = "Insurance Dates are not valid", Status = -3 };


                    return new ResponseData { Message = "Error in the execution of the Save Patient Info API", Status = 0 };
                }
            }
        }

    }
}