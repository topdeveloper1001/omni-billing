// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseBal.cs" company="Spadez">
//   OmniHealth Care
// </copyright>
// <summary>
//   The queryable extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using BillingSystem.Bal.Mapper;
using Newtonsoft.Json;
using Omu.ValueInjecter;

namespace BillingSystem.Bal.BusinessAccess
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration.Conventions;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Runtime.Remoting.Messaging;

    using BillingSystem.Common.Common;
    using BillingSystem.Model;
    using BillingSystem.Model.CustomModel;
    using BillingSystem.Repository.UOW;
    using System.Threading.Tasks;

    /// <summary>
    /// The queryable extensions.
    /// </summary>
    public static class QueryableExtensions
    {
        //http://www.annew.com.au/net/left-joins-in-ef/

        public static IQueryable<TResult> LeftJoin<TOuter, TInner, TKey, TResult>(
            this IQueryable<TOuter> outer,
            IQueryable<TInner> inner,
            Expression<Func<TOuter, TKey>> outerKeySelector,
            Expression<Func<TInner, TKey>> innerKeySelector,
            Expression<Func<JoinResult<TOuter, TInner>, TResult>> resultSelector)
        {
            var result = outer
                .GroupJoin(inner, outerKeySelector, innerKeySelector, (outer1, inners) => new { outer1, inners = inners.DefaultIfEmpty() })
                .SelectMany(row => row.inners, (row, inner1) => new JoinResult<TOuter, TInner> { Outer = row.outer1, Inner = inner1 })
                .Select(resultSelector);

            return result;
        }

        /// <summary>
        /// The join result.
        /// </summary>
        /// <typeparam name="TOuter">
        /// </typeparam>
        /// <typeparam name="TInner">
        /// </typeparam>
        public class JoinResult<TOuter, TInner>
        {
            public TOuter Outer { get; set; }
            public TInner Inner { get; set; }
        }
    }

    /// <summary>
    /// The base bal.
    /// </summary>
    public class BaseBal : IDisposable
    {
        public string TableNumber { get; set; }

        public string TableDescription { get; set; }

        public string CptTableNumber { get; set; }

        public string ServiceCodeTableNumber { get; set; }

        public string DrgTableNumber { get; set; }

        public string DrugTableNumber { get; set; }

        public string HcpcsTableNumber { get; set; }

        public string DiagnosisTableNumber { get; set; }

        public string BillEditRuleTableNumber { get; set; }

        public DateTime? CodeEffectiveFrom { get; set; }

        public DateTime? CodeEffectiveTill { get; set; }

        public BaseBal(string billEditRuleTableNumber)
        {
            if (!string.IsNullOrEmpty(billEditRuleTableNumber)) BillEditRuleTableNumber = billEditRuleTableNumber;
        }

        public BaseBal()
        {

        }

        public BaseBal(
            string cptTableNumber,
            string serviceCodeTableNumber,
            string drgTableNumber,
            string drugTableNumber,
            string hcpcsTableNumber,
            string diagnosisTableNumber)
        {
            if (!string.IsNullOrEmpty(cptTableNumber)) CptTableNumber = cptTableNumber;

            if (!string.IsNullOrEmpty(serviceCodeTableNumber)) ServiceCodeTableNumber = serviceCodeTableNumber;

            if (!string.IsNullOrEmpty(drgTableNumber)) DrgTableNumber = drgTableNumber;

            if (!string.IsNullOrEmpty(drugTableNumber)) DrugTableNumber = drugTableNumber;

            if (!string.IsNullOrEmpty(hcpcsTableNumber)) HcpcsTableNumber = hcpcsTableNumber;

            if (!string.IsNullOrEmpty(diagnosisTableNumber)) DiagnosisTableNumber = diagnosisTableNumber;
        }

        private bool _disposed;

        public readonly UnitOfWork UnitOfWork = new UnitOfWork();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    UnitOfWork.Dispose();
                }
            }
            _disposed = true;
        }

        /// <summary>
        /// Gets the facility name by number.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns></returns>
        public string GetFacilityNameByNumber(string number)
        {
            using (var facilityRep = UnitOfWork.FacilityRepository)
            {
                var facility = facilityRep.Where(a => a.FacilityNumber.Equals(number)).FirstOrDefault();
                return (facility != null) ? facility.FacilityName : string.Empty;
            }
        }

        public Facility GetFacilityByFacilityId(int facilityId)
        {
            var facility = new Facility();
            var fid = Convert.ToInt32(facilityId);
            if (facilityId > 0)
            {
                using (var rep = UnitOfWork.FacilityRepository)
                {
                    facility = rep.Where(f => f.FacilityId == fid).FirstOrDefault();
                }
            }
            return facility;
        }

        public string GetFacilityNameByFacilityId(int facilityId)
        {
            Facility facility = null;
            if (facilityId > 0)
            {
                using (var rep = UnitOfWork.FacilityRepository) facility = rep.Where(f => f.FacilityId == facilityId).FirstOrDefault();
            }
            return facility != null ? facility.FacilityName : string.Empty;
        }

        public string GetRoleNameByRoleId(int roleId)
        {
            var roleName = string.Empty;
            if (roleId > 0)
            {
                using (var rep = UnitOfWork.RoleRepository) roleName = rep.Where(r => r.RoleID == roleId).Select(role => role.RoleName).FirstOrDefault();
            }
            return roleName;
        }

        public string GetNameByCorporateId(int id)
        {
            using (var rep = UnitOfWork.CorporateRepository)
            {
                var corp = rep.Where(c => c.CorporateID == id).FirstOrDefault();
                return corp != null ? corp.CorporateName : string.Empty;
            }
        }

        public int GetAgeByDate(DateTime dateValue)
        {
            var age = DateTime.Now - dateValue;
            return (int)(age.Days / 365.25);
        }

        public Physician GetPhysicianById(int id)
        {
            using (var rep = UnitOfWork.PhysicianRepository)
            {
                var phys = rep.Where(p => p.Id == id).FirstOrDefault();
                return phys ?? new Physician();
            }
        }

        /// <summary>
        /// Gets the global category name by identifier.
        /// </summary>
        /// <param name="categoryValue">The category value.</param>
        /// <returns></returns>
        public string GetGlobalCategoryNameById(string categoryValue, string facilityId = "")
        {
            if (!string.IsNullOrEmpty(categoryValue))
            {
                using (var rep = UnitOfWork.GlobalCodeCategoryRepository)
                {
                    var category = rep.Where(g => g.GlobalCodeCategoryValue.Equals(categoryValue) && g.IsDeleted.HasValue ? !g.IsDeleted.Value : false && (string.IsNullOrEmpty(facilityId) || g.FacilityNumber.Equals(facilityId))).FirstOrDefault();
                    return category != null ? category.GlobalCodeCategoryName : string.Empty;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the diagnose name by code identifier.
        /// </summary>
        /// <param name="diagnosisId">The diagnosis identifier.</param>
        /// <returns></returns>
        public string GetDiagnoseNameByCodeId(string diagnosisId)
        {
            var id = Convert.ToInt32(diagnosisId);
            using (var rep = UnitOfWork.DiagnosisRespository)
            {
                var diagnosis = rep.Where(d => d.DiagnosisID == id).FirstOrDefault();
                return diagnosis != null ? diagnosis.DiagnosisCodeDescription : string.Empty;
            }
        }

        /// <summary>
        /// Gets the name by global code identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public string GetNameByGlobalCodeId(int id)
        {
            using (var rep = UnitOfWork.GlobalCodeRepository)
            {
                var gl = rep.Where(g => g.GlobalCodeID == id).FirstOrDefault();
                return gl != null ? gl.GlobalCodeName : string.Empty;
            }
        }

        /// <summary>
        /// Gets the name by global code value.
        /// </summary>
        /// <param name="codeValue">The code value.</param>
        /// <param name="categoryValue">The category value.</param>
        /// <returns></returns>
        public string GetNameByGlobalCodeValue(string codeValue, string categoryValue, string fId = "")
        {
            if (!string.IsNullOrEmpty(codeValue))
            {
                using (var rep = UnitOfWork.GlobalCodeRepository)
                {
                    var gl =
                        rep.Where(
                            g => g.GlobalCodeValue.Equals(codeValue) && !g.IsDeleted.Value && g.GlobalCodeCategoryValue.Equals(categoryValue) && (string.IsNullOrEmpty(fId) || g.FacilityNumber.Equals(fId)))
                            .FirstOrDefault();
                    return gl != null ? gl.GlobalCodeName : string.Empty;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the name by global code value.
        /// </summary>
        /// <param name="codeValue">The code value.</param>
        /// <param name="categoryValue">The category value.</param>
        /// <returns></returns>
        public string GetNameByGlobalCodeValueAndSubCategory1(
            string codeValue,
            string categoryValue,
            string subCategory1)
        {
            if (!string.IsNullOrEmpty(codeValue))
            {
                using (var rep = UnitOfWork.GlobalCodeRepository)
                {
                    var gl =
                        rep.Where(
                            g =>
                            g.GlobalCodeValue.Equals(codeValue) && g.GlobalCodeCategoryValue.Equals(categoryValue)
                            && g.ExternalValue1.Equals(subCategory1)).FirstOrDefault();
                    return gl != null ? gl.GlobalCodeName : string.Empty;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the code description.
        /// </summary>
        /// <param name="orderCode">The order code.</param>
        /// <param name="orderType">Type of the order.</param>
        /// <returns></returns>
        public string GetCodeDescription(string orderCode, string orderType)
        {
            var codeDescription = string.Empty;

            if (!string.IsNullOrEmpty(orderCode) && !string.IsNullOrEmpty(orderType))
            {
                var codeType = (OrderType)Enum.Parse(typeof(OrderType), orderType);
                switch (codeType)
                {
                    case OrderType.CPT:
                        using (var bal = new CPTCodesBal(CptTableNumber))
                        {
                            codeDescription = bal.GetCPTCodeDescription(orderCode);
                            return codeDescription;
                        }
                    case OrderType.DRG:
                        using (var bal = new DRGCodesBal(DrgTableNumber))
                        {
                            codeDescription = bal.GetDrgDescriptionByCode(orderCode);
                            return codeDescription;
                        }
                    case OrderType.HCPCS:
                        using (var bal = new HCPCSCodesBal(HcpcsTableNumber))
                        {
                            codeDescription = bal.GetHCPCSCodeDescription(orderCode);
                            return codeDescription;
                        }
                    case OrderType.DRUG:
                        using (var bal = new DrugBal(DrugTableNumber))
                        {
                            codeDescription = bal.GetDRUGCodeDescription(orderCode);
                            return codeDescription;
                        }
                    case OrderType.BedCharges:
                        using (var bal = new ServiceCodeBal(ServiceCodeTableNumber))
                        {
                            codeDescription = bal.GetServiceCodeDescription(orderCode);
                            return codeDescription;
                        }
                    case OrderType.Diagnosis:
                        using (var bal = new DiagnosisCodeBal(DiagnosisTableNumber))
                        {
                            codeDescription = bal.GetDiagnosisCodeDescById(orderCode);
                            return codeDescription;
                        }
                    case OrderType.ServiceCode:
                        using (var bal = new ServiceCodeBal(ServiceCodeTableNumber))
                        {
                            codeDescription = bal.GetServiceCodeDescription(orderCode);
                            return codeDescription;
                        }
                    default:
                        break;
                }
            }
            return codeDescription;
        }

        /// <summary>
        /// Gets the patient identifier by encounter identifier.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        public int GetPatientIdByEncounterId(int encounterId)
        {
            if (encounterId > 0)
            {
                using (var rep = UnitOfWork.EncounterRepository)
                {
                    var en = rep.Where(e => e.EncounterID == encounterId).FirstOrDefault();
                    return en != null ? Convert.ToInt32(en.PatientID) : 0;
                }
            }
            return 0;
        }

        /// <summary>
        /// Gets the encounter number by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public string GetEncounterNumberById(int id)
        {
            using (var rep = UnitOfWork.EncounterRepository)
            {
                var encounter = rep.Where(e => e.EncounterID == id).FirstOrDefault();
                return encounter != null ? encounter.EncounterNumber : string.Empty;
            }
        }

        /// <summary>
        /// Gets the encounter status by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public string GetEncounterStatusById(int id)
        {
            using (var rep = UnitOfWork.EncounterRepository)
            {
                var encounter = rep.Where(e => e.EncounterID == id).FirstOrDefault();
                return encounter != null ? encounter.EncounterEndTime != null ? "Ended" : "Active" : "NA";
            }
        }

        /// <summary>
        /// Gets the encounter home care recuuring by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public bool GetEncounterHomeCareRecuuringById(int id)
        {
            using (var rep = UnitOfWork.EncounterRepository)
            {
                var encounter = rep.Where(e => e.EncounterID == id).FirstOrDefault();
                return encounter != null && (encounter.HomeCareRecurring ?? false);
            }
        }

        /// <summary>
        /// Gets the insurance company name by payer identifier.
        /// </summary>
        /// <param name="id">The payer identifier.</param>
        /// <returns></returns>
        public string GetInsuranceCompanyNameById(int id)
        {
            using (var rep = UnitOfWork.InsuranceCompanyRepository)
            {
                var ins = rep.Where(e => e.InsuranceCompanyId == id).FirstOrDefault();
                return ins != null ? ins.InsuranceCompanyName : string.Empty;
            }
        }

        public string GetInsuranceCompanyNameByPayerId(string payorId)
        {
            using (var rep = UnitOfWork.InsuranceCompanyRepository)
            {
                var ins = rep.Where(e => e.InsuranceCompanyLicenseNumber.Equals(payorId)).FirstOrDefault();
                return ins != null ? ins.InsuranceCompanyName : string.Empty;
            }
        }

        /// <summary>
        /// Get Insurance Company's License Number / ID Payer by current Company ID
        /// </summary>
        /// <param name="id">current Company ID</param>
        /// <returns>Insurance Company's License Number / ID Payer</returns>
        public string GetInsuranceIdPayerById(int id)
        {
            if (id > 0)
            {
                using (var rep = UnitOfWork.InsuranceCompanyRepository)
                {
                    var ins = rep.Where(e => e.InsuranceCompanyId == id).FirstOrDefault();
                    return ins != null ? ins.InsuranceCompanyLicenseNumber : string.Empty;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the insurance plan name by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public string GetInsurancePlanNameById(int? id)
        {
            var planId = id == null ? 0 : Convert.ToInt32(id);
            using (var rep = UnitOfWork.InsurancePlansRepository)
            {
                var ins = rep.Where(e => e.InsurancePlanId == planId).FirstOrDefault();
                return ins != null ? ins.PlanName : string.Empty;
            }
        }

        /// <summary>
        /// Gets the insurance policy name by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public string GetInsurancePolicyNameById(int? id)
        {
            var policyId = id == null ? 0 : Convert.ToInt32(id);
            using (var rep = UnitOfWork.InsurancePolicesRepository)
            {
                var ins = rep.Where(e => e.InsurancePolicyId == policyId).FirstOrDefault();
                return ins != null ? ins.PolicyName : string.Empty;
            }
        }

        /// <summary>
        /// Gets the DRG code by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public DRGCodes GetDRGCodeById(int id)
        {
            using (var rep = UnitOfWork.DRGCodesRepository)
            {
                var drg =
                    rep.Where(d => d.DRGCodesId == id && d.CodeTableNumber.Trim().Equals(DrgTableNumber))
                        .FirstOrDefault();
                return drg ?? new DRGCodes();
            }
        }

        /// <summary>
        /// addded by shashnak to get the user name
        /// </summary>
        /// <param name="UserID">The user identifier.</param>
        /// <returns></returns>
        public string GetNameByUserId(int? UserID)
        {
            try
            {
                using (var usersRep = UnitOfWork.UsersRepository)
                {
                    var usersModel = usersRep.Where(x => x.UserID == UserID && x.IsDeleted == false).FirstOrDefault();
                    return usersModel != null ? usersModel.FirstName + " " + usersModel.LastName : string.Empty;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets all ordering codes.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public List<GeneralCodesCustomModel> GetAllOrderingCodes(string text)
        {
            var finalList = new List<GeneralCodesCustomModel>();
            using (var cptBal = new CPTCodesBal(CptTableNumber))
            {
                var cptList = cptBal.GetAllCptCodes();
                finalList.AddRange(
                    cptList.Select(
                        item =>
                        new GeneralCodesCustomModel
                        {
                            Code = item.CodeNumbering,
                            Description = item.CodeDescription,
                            CodeDescription =
                                    string.Format(
                                        "{0} - {1}",
                                        item.CodeNumbering,
                                        item.CodeDescription),
                            CodeType = Convert.ToString((int)OrderType.CPT),
                            CodeTypeName = "CPT",
                            ExternalCode = item.CTPCodeRangeValue.ToString(),
                            ID = Convert.ToString(item.CPTCodesId)
                        }));
            }
            using (var hcpcBal = new HCPCSCodesBal(HcpcsTableNumber))
            {
                var hcpcList = hcpcBal.GetHCPCSCodes();
                finalList.AddRange(
                    hcpcList.Select(
                        item =>
                        new GeneralCodesCustomModel
                        {
                            Code = item.CodeNumbering,
                            Description = item.CodeDescription,
                            CodeDescription =
                                    string.Format(
                                        "{0} - {1}",
                                        item.CodeNumbering,
                                        item.CodeDescription),
                            CodeType = Convert.ToString((int)OrderType.HCPCS),
                            CodeTypeName = "HCPCS",
                            ExternalCode = string.Empty,
                            ID = Convert.ToString(item.HCPCSCodesId)
                        }));
            }
            using (var drugBal = new DrugBal(DrugTableNumber))
            {
                var drugList =
                    drugBal.GetDrugList().Where(x => x.InStock == true && x.DrugStatus.Trim() != "Deleted").ToList();

                finalList.AddRange(
                    drugList.Select(
                        item =>
                            new GeneralCodesCustomModel
                            {
                                Code = item.DrugCode,
                                Description = item.DrugDescription,
                                CodeDescription =
                                    string.Format(
                                        "{0} - {1} - {2} - {3}",
                                        item.DrugCode.Trim(),
                                        item.DrugGenericName.Trim(),
                                        !string.IsNullOrEmpty(item.DrugStrength)
                                            ? item.DrugStrength.Trim()
                                            : string.Empty,
                                        !string.IsNullOrEmpty(item.DrugDosage) ? item.DrugDosage.Trim() : string.Empty),
                                CodeType = Convert.ToString((int)OrderType.DRUG),
                                CodeTypeName = "DRUG",
                                ExternalCode =
                                    String.IsNullOrEmpty(item.BrandCode)
                                        ? "0"
                                        : Convert.ToString(item.BrandCode),
                                ID = Convert.ToString(item.Id),
                                DrugPackageName =
                                    !string.IsNullOrEmpty(item.DrugPackageName)
                                        ? item.DrugPackageName.Trim()
                                        : string.Empty
                            }));
            }

            if (finalList.Count > 0)
            {
                text = text.ToLower().Trim();
                finalList =
                    finalList.Where(
                        f => (f.CodeDescription.ToLower().Contains(text) ||
                             (!string.IsNullOrEmpty(f.DrugPackageName) &&
                              f.DrugPackageName.Trim().ToLower().Contains(text)))).ToList();
            }
            return finalList;
        }

        //GetOrderingCodesByCategory
        /// <summary>
        /// Gets the ordering codes by sub category.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="subCategoryId">The sub category identifier.</param>
        /// <returns></returns>
        public List<GeneralCodesCustomModel> GetOrderingCodesBySubCategory(string text, int subCategoryId)
        {
            var finalList = new List<GeneralCodesCustomModel>();
            using (var bal = new GlobalCodeBal())
            {
                var subCategory = bal.GetGlobalCodeByGlobalCodeId(subCategoryId);
                var type = OrderType.DRUG;
                if (!string.IsNullOrEmpty(subCategory.ExternalValue1))
                {
                    type = (OrderType)Enum.Parse(typeof(OrderType), subCategory.ExternalValue1);
                }
                switch (type)
                {
                    case OrderType.CPT:
                        using (var cbal = new CPTCodesBal(CptTableNumber))
                        {
                            var result = cbal.GetCodesByRange(
                                Convert.ToInt32(subCategory.ExternalValue2),
                                Convert.ToInt32(subCategory.ExternalValue3));
                            finalList.AddRange(
                                result.Select(
                                    item =>
                                    new GeneralCodesCustomModel
                                    {
                                        Code = item.CodeNumbering,
                                        Description = item.CodeDescription,
                                        CodeDescription =
                                                string.Format(
                                                    "{0} - {1}",
                                                    item.CodeDescription,
                                                    item.CodeNumbering),
                                        CodeType = Convert.ToInt32(OrderType.CPT).ToString(),
                                        CodeTypeName = "CPT",
                                        ExternalCode = item.CTPCodeRangeValue.ToString(),
                                        ID = item.CPTCodesId.ToString()
                                    }));
                        }
                        break;
                    case OrderType.HCPCS:
                        using (var cbal = new HCPCSCodesBal(HcpcsTableNumber))
                        {
                            var result = cbal.GetHCPCSCodes();
                            finalList.AddRange(
                                result.Select(
                                    item =>
                                    new GeneralCodesCustomModel
                                    {
                                        Code = item.CodeNumbering,
                                        Description = item.CodeDescription,
                                        CodeDescription =
                                                string.Format(
                                                    "{0} - {1}",
                                                    item.CodeDescription,
                                                    item.CodeNumbering),
                                        CodeType = Convert.ToInt32(OrderType.HCPCS).ToString(),
                                        CodeTypeName = "HCPCS",
                                        ExternalCode = string.Empty,
                                        ID = item.HCPCSCodesId.ToString()
                                    }));
                        }
                        break;
                    case OrderType.DRG:
                        break;
                    case OrderType.DRUG:
                        using (var dbal = new DrugBal(DrugTableNumber))
                        {
                            var result =
                                dbal.GetDrugList()
                                    .Where(x => x.InStock == true && x.DrugStatus.Trim() != "Deleted")
                                    .ToList();
                            finalList.AddRange(
                                result.Select(
                                    item =>
                                    new GeneralCodesCustomModel
                                    {
                                        Code = item.DrugCode,
                                        Description = item.DrugPackageName,
                                        CodeDescription =
                                                string.Format(
                                                    "{0} - {1} - {2} - {3}",
                                                    item.DrugCode,
                                                    item.DrugPackageName,
                                                    item.DrugStrength,
                                                    item.DrugDosage),
                                        CodeType = Convert.ToInt32(OrderType.DRUG).ToString(),
                                        CodeTypeName = "DRUG",
                                        ExternalCode =
                                                String.IsNullOrEmpty(item.BrandCode)
                                                    ? "0"
                                                    : item.BrandCode.ToString(),
                                        ID = item.Id.ToString()
                                    }));
                        }
                        break;
                    default:
                        break;
                }

                if (finalList.Count > 0)
                {
                    text = text.ToLower().Trim();
                    finalList = finalList.Where(a => a.CodeDescription.ToLower().Contains(text)).ToList();
                }

                return finalList;
            }
        }

        /// <summary>
        /// Gets the ordering codes by category.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="categoryId">The category identifier.</param>
        /// <returns></returns>
        //public List<GeneralCodesCustomModel> GetOrderingCodesByCategory(string text, int categoryId)
        //{
        //    var finalList = new List<GeneralCodesCustomModel>();
        //    using (var bal = new GlobalCodeBal())
        //    {
        //        switch (categoryId)
        //        {
        //            case 11001:
        //            case 11009:
        //            case 11090:
        //            case 11080:
        //            case 11070:
        //            case 11010:
        //                using (var cbal = new CPTCodesBal(CptTableNumber))
        //                {
        //                    var result = cbal.GetFilteredCodes(text);
        //                    finalList.AddRange(
        //                        result.Select(
        //                            item =>
        //                            new GeneralCodesCustomModel
        //                            {
        //                                Code = item.CodeNumbering,
        //                                Description = item.CodeDescription,
        //                                CodeDescription =
        //                                        string.Format(
        //                                            "{0} - {1}",
        //                                            item.CodeDescription,
        //                                            item.CodeNumbering),
        //                                CodeType = Convert.ToInt32(OrderType.CPT).ToString(),
        //                                CodeTypeName = "CPT",
        //                                ExternalCode = item.CTPCodeRangeValue.ToString(),
        //                                ID = item.CPTCodesId.ToString()
        //                            }));
        //                }
        //                break;
        //            case 11200:
        //                using (var cbal = new HCPCSCodesBal(HcpcsTableNumber))
        //                {
        //                    var result = cbal.GetFilteredHCPCSCodes(text);
        //                    finalList.AddRange(
        //                        result.Select(
        //                            item =>
        //                            new GeneralCodesCustomModel
        //                            {
        //                                Code = item.CodeNumbering,
        //                                Description = item.CodeDescription,
        //                                CodeDescription =
        //                                        string.Format(
        //                                            "{0} - {1}",
        //                                            item.CodeDescription,
        //                                            item.CodeNumbering),
        //                                CodeType = Convert.ToInt32(OrderType.HCPCS).ToString(),
        //                                CodeTypeName = "HCPCS",
        //                                ExternalCode = string.Empty,
        //                                ID = item.HCPCSCodesId.ToString()
        //                            }));
        //                }
        //                break;
        //            case 11100:
        //                using (var dbal = new DrugBal(DrugTableNumber))
        //                {
        //                    var result =
        //                        dbal.GetFilteredDrugCodes(text)
        //                            .Where(x => x.InStock == true && x.DrugStatus.Trim() != "Deleted")
        //                            .ToList();
        //                    finalList.AddRange(
        //                        result.Select(
        //                            item =>
        //                            new GeneralCodesCustomModel
        //                            {
        //                                Code = item.DrugCode,
        //                                Description = item.DrugPackageName,
        //                                CodeDescription =
        //                                        string.Format(
        //                                            "{0} - {1} - {2} - {3}",
        //                                            item.DrugCode,
        //                                            item.DrugPackageName,
        //                                            item.DrugStrength,
        //                                            item.DrugDosage),
        //                                CodeType = Convert.ToInt32(OrderType.DRUG).ToString(),
        //                                CodeTypeName = "DRUG",
        //                                ExternalCode =
        //                                        String.IsNullOrEmpty(item.BrandCode)
        //                                            ? "0"
        //                                            : item.BrandCode.ToString(),
        //                                ID = item.Id.ToString()
        //                            }));
        //                }
        //                break;
        //            default:
        //                break;
        //        }

        //        if (finalList.Count > 0)
        //        {
        //            text = text.ToLower().Trim();
        //            finalList = finalList.Where(a => a.CodeDescription.ToLower().Contains(text)).ToList();
        //        }

        //        return finalList;
        //    }
        //}

        protected string GetDescByRuleMasterId(int id)
        {
            using (var rep = UnitOfWork.RuleMasterRepository)
            {
                var model =
                    rep.Where(
                        a =>
                        a.RuleMasterID == id && !string.IsNullOrEmpty(a.ExtValue9)
                        && a.ExtValue9.Trim().Equals(BillEditRuleTableNumber)).FirstOrDefault();
                return model != null ? model.RuleDescription : string.Empty;
            }
        }

        public string GetDescByRuleStepId(int id)
        {
            using (var rep = UnitOfWork.RuleStepRepository)
            {
                var model = rep.Where(a => a.RuleStepID == id).FirstOrDefault();
                return model != null ? model.RuleStepDescription : string.Empty;
            }
        }

        public string GetDescByErrorId(int id)
        {
            using (var rep = UnitOfWork.ErrorMasterRepository)
            {
                var model = rep.Where(a => a.ErrorMasterID == id).FirstOrDefault();
                return model != null ? model.ErrorDescription : string.Empty;
            }
        }

        //public GeneralCodesCustomModel GetSelectedCodeParent(string code, string Type)
        //{
        //    var finalList = new GeneralCodesCustomModel();
        //    using (var bal = new GlobalCodeBal())
        //    {
        //        var type = (OrderType)Enum.Parse(typeof(OrderType), Type);
        //        switch (type)
        //        {
        //            case OrderType.CPT:
        //                using (var cptBal = new CPTCodesBal(CptTableNumber))
        //                {
        //                    var cptList = cptBal.GetCPTCodesByCode(code);
        //                    var cptCoderange = cptList.CTPCodeRangeValue;
        //                    if (cptCoderange != null)
        //                    {
        //                        var cptCoderangeInt = Convert.ToInt32(cptCoderange);
        //                        var cptCategory = bal.GetGeneralGlobalCodeByRangeVal(cptCoderangeInt, Type);
        //                        return cptCategory;
        //                    }
        //                }
        //                break;
        //            case OrderType.HCPCS:
        //                using (var hcpcsBal = new HCPCSCodesBal(HcpcsTableNumber))
        //                {
        //                    var hcpcsObj = hcpcsBal.GetFilteredHCPCSCodes(code);
        //                    if (hcpcsObj.Any())
        //                    {
        //                        var generalCodesCustomModel = new GeneralCodesCustomModel()
        //                        {
        //                            GlobalCodeCategoryName = "HCPCS Codes",
        //                            GlobalCodeCategoryId = "11200",
        //                            GlobalCodeName = "NA",
        //                            GlobalCodeId = 9090
        //                        };
        //                        return generalCodesCustomModel;
        //                    }
        //                }
        //                break;
        //            case OrderType.DRUG:
        //                using (var drugBal = new DrugBal(DrugTableNumber))
        //                {
        //                    var drugObj = drugBal.GetCurrentDrugByCode(code);
        //                    var drugBrandId = drugObj.BrandCode;
        //                    if (drugBrandId != null)
        //                    {
        //                        var drugBrandIdInt = Convert.ToInt32(drugBrandId);
        //                        var category = bal.GetGeneralGlobalCodeByRangeVal(drugBrandIdInt, Type);
        //                        return category;
        //                    }
        //                }
        //                break;
        //        }
        //    }



        //    //if (finalList.Count > 0)
        //    //{
        //    //    code = code.ToLower().Trim();
        //    //    finalList = finalList.Where(f => !string.IsNullOrEmpty(f.Code) && f.Code.ToLower().Contains(code)).ToList();
        //    //}
        //    return finalList;
        //}

        public List<string> GetColumnsByTableName(string tableName)
        {
            var list = new List<string>();
            var entity = (TableNames)Enum.Parse(typeof(TableNames), tableName);
            switch (entity)
            {
                case TableNames.Authorization:
                    list = typeof(Authorization).GetProperties().Select(i => i.Name).ToList();
                    break;
                case TableNames.BillHeader:
                    list = typeof(BillHeader).GetProperties().Select(i => i.Name).ToList();
                    break;
                case TableNames.Corporate:
                    list = typeof(Corporate).GetProperties().Select(i => i.Name).ToList();
                    break;
                case TableNames.Denial:
                    list = typeof(Denial).GetProperties().Select(i => i.Name).ToList();
                    break;
                case TableNames.Diagnosis:
                    list = typeof(Diagnosis).GetProperties().Select(i => i.Name).ToList();
                    break;
                case TableNames.DiagnosisCode:
                    list = typeof(DiagnosisCode).GetProperties().Select(i => i.Name).ToList();
                    break;
                case TableNames.Drug:
                    list = typeof(Drug).GetProperties().Select(i => i.Name).ToList();
                    break;
                case TableNames.Encounter:
                    list = typeof(Encounter).GetProperties().Select(i => i.Name).ToList();
                    break;
                case TableNames.Facility:
                    list = typeof(Facility).GetProperties().Select(i => i.Name).ToList();
                    break;
                case TableNames.HCPCSCodes:
                    list = typeof(HCPCSCodes).GetProperties().Select(i => i.Name).ToList();
                    break;
                case TableNames.InsuranceCompany:
                    list = typeof(InsuranceCompany).GetProperties().Select(i => i.Name).ToList();
                    break;
                case TableNames.InsurancePlans:
                    list = typeof(InsurancePlans).GetProperties().Select(i => i.Name).ToList();
                    break;
                case TableNames.InsurancePolices:
                    list = typeof(InsurancePolices).GetProperties().Select(i => i.Name).ToList();
                    break;
                case TableNames.MCContract:
                    list = typeof(ManagedCare).GetProperties().Select(i => i.Name).ToList();
                    break;
                case TableNames.OpenOrder:
                    list = typeof(OpenOrder).GetProperties().Select(i => i.Name).ToList();
                    break;
                case TableNames.OpenOrderActivitySchedule:
                    list = typeof(OpenOrderActivitySchedule).GetProperties().Select(i => i.Name).ToList();
                    break;
                case TableNames.OpenOrderActivityTime:
                    list = typeof(OpenOrderActivityTime).GetProperties().Select(i => i.Name).ToList();
                    break;
                case TableNames.OrderActivity:
                    list = typeof(OrderActivity).GetProperties().Select(i => i.Name).ToList();
                    break;
                case TableNames.PatientInfo:
                    list = typeof(PatientInfo).GetProperties().Select(i => i.Name).ToList();
                    break;
                case TableNames.RuleMaster:
                    list = typeof(RuleMaster).GetProperties().Select(i => i.Name).ToList();
                    break;
                case TableNames.PatientInsurance:
                    list = typeof(PatientInsurance).GetProperties().Select(i => i.Name).ToList();
                    break;
                default:
                    break;
            }
            return list;
        }

        public string GetKeyColmnNameByTableName(string tableName)
        {
            var keyColumn = string.Empty;
            using (var rep = UnitOfWork.GlobalCodeRepository)
            {
                var gc = rep.Where(a => a.GlobalCodeName.Equals(tableName)).FirstOrDefault();
                keyColumn = gc != null ? gc.ExternalValue1 : string.Empty;
            }
            return keyColumn;
        }

        public string GetPatientNameById(int patientId)
        {
            using (var rep = UnitOfWork.PatientInfoRepository)
            {
                var patient = rep.Where(p => p.PatientID == patientId).FirstOrDefault();
                return patient != null
                           ? string.Format("{0} {1}", patient.PersonFirstName, patient.PersonLastName)
                           : string.Empty;
            }
        }

        /// <summary>
        /// Gets the active encounter ids by patient identifier.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public List<int> GetActiveEncounterIdsByPatientId(int patientId)
        {
            var list = new List<int>();
            using (var rep = UnitOfWork.EncounterRepository)
                list =
                    rep.Where(e => (int)e.PatientID == patientId && e.EncounterEndTime == null)
                        .OrderByDescending(d => d.EncounterStartTime)
                        .Select(item => item.EncounterID)
                        .ToList();
            return list;
        }

        /// <summary>
        /// Gets the bill header status identifier by bill header identifier.
        /// </summary>
        /// <param name="billHeaderId">The bill header identifier.</param>
        /// <returns></returns>
        public string GetBillHeaderStatusIDByBillHeaderId(int billHeaderId)
        {
            using (var rep = UnitOfWork.BillHeaderRepository)
            {
                var bh = rep.Where(b => b.BillHeaderID == billHeaderId).FirstOrDefault();
                if (bh != null)
                {
                    var gc = GetNameByGlobalCodeValue(
                        bh.Status,
                        Convert.ToString(Convert.ToInt32(GlobalCodeCategoryValue.BillHeaderStatus)));
                    return gc;
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the section value by bill header identifier.
        /// </summary>
        /// <param name="billHeaderId">The bill header identifier.</param>
        /// <returns></returns>
        public int GetSectionValueByBillHeaderId(int billHeaderId)
        {
            var sectionValue = string.Empty;
            using (var rep = UnitOfWork.BillHeaderRepository)
            {
                var bh = rep.Where(b => b.BillHeaderID == billHeaderId).FirstOrDefault();
                if (bh != null)
                {
                    using (var gcRep = UnitOfWork.GlobalCodeRepository)
                    {
                        var status = Convert.ToString(Convert.ToInt32(GlobalCodeCategoryValue.BillHeaderStatus));
                        sectionValue =
                            gcRep.Where(
                                g => g.GlobalCodeValue.Equals(bh.Status) && g.GlobalCodeCategoryValue.Equals(status))
                                .Select(e => e.ExternalValue5)
                                .FirstOrDefault();
                    }
                }
                return string.IsNullOrEmpty(sectionValue) ? 0 : Convert.ToInt32(sectionValue);
            }
        }

        /// <summary>
        /// Gets the activity name by activity identifier.
        /// </summary>
        /// <param name="activityId">The activity identifier.</param>
        /// <returns></returns>
        public string GetActivityNameByActivityId(int activityId)
        {
            using (var rep = UnitOfWork.BillActivityRepository)
            {
                var encounter = activityId != 0 ? rep.GetAll().FirstOrDefault(e => e.BillActivityID == activityId) : null;
                //return encounter != null ? encounter.ActivityType : string.Empty;
                return encounter != null ? this.GetNameByGlobalCodeValue(encounter.ActivityType, "1201") : string.Empty;
            }
        }

        /// <summary>
        /// Gets the activity code by activity identifier.
        /// </summary>
        /// <param name="activityId">The activity identifier.</param>
        /// <returns></returns>
        public string GetActivityCodeByActivityId(int activityId)
        {
            using (var rep = UnitOfWork.BillActivityRepository)
            {
                var encounter = activityId != 0 ? rep.Where(e => e.BillActivityID == activityId).FirstOrDefault() : null;
                return encounter != null ? encounter.ActivityCode : string.Empty;
            }
        }

        /// <summary>
        /// Gets the order codes by order type identifier.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="orderTypeId">The order type identifier.</param>
        /// <returns></returns>
        public List<GeneralCodesCustomModel> GetOrderCodesByOrderTypeId(string text, int orderTypeId)
        {
            var finalList = new List<GeneralCodesCustomModel>();
            switch (orderTypeId)
            {
                case 1:
                    using (var cbal = new CPTCodesBal(CptTableNumber))
                    {
                        var result = cbal.GetFilteredCodes(text);
                        finalList.AddRange(
                            result.Select(
                                item =>
                                new GeneralCodesCustomModel
                                {
                                    Code = item.CodeNumbering,
                                    Description = item.CodeDescription,
                                    CodeDescription =
                                            string.Format(
                                                "{0} - {1}",
                                                item.CodeDescription,
                                                item.CodeNumbering),
                                    CodeType =
                                            Convert.ToString(Convert.ToInt32(OrderType.CPT)),
                                    CodeTypeName = "CPT",
                                    ExternalCode = Convert.ToString(item.CTPCodeRangeValue),
                                    ID = Convert.ToString(item.CPTCodesId)
                                }));
                    }
                    break;
                case 2:
                    using (var cbal = new HCPCSCodesBal(HcpcsTableNumber))
                    {
                        var result = cbal.GetHCPCSCodes();
                        finalList.AddRange(
                            result.Select(
                                item =>
                                new GeneralCodesCustomModel
                                {
                                    Code = item.CodeNumbering,
                                    Description = item.CodeDescription,
                                    CodeDescription =
                                            string.Format(
                                                "{0} - {1}",
                                                item.CodeDescription,
                                                item.CodeNumbering),
                                    CodeType = Convert.ToInt32(OrderType.HCPCS).ToString(),
                                    CodeTypeName = "HCPCS",
                                    ExternalCode = string.Empty,
                                    ID = item.HCPCSCodesId.ToString()
                                }));
                    }
                    break;
                case 4:
                    using (var cbal = new DRGCodesBal(DrgTableNumber))
                    {
                        var result = cbal.GetFilteredDRGCodes(text);
                        finalList.AddRange(
                            result.Select(
                                item =>
                                new GeneralCodesCustomModel
                                {
                                    Code = item.CodeNumbering,
                                    Description = item.CodeDescription,
                                    CodeDescription =
                                            string.Format(
                                                "{0} - {1}",
                                                item.CodeDescription,
                                                item.CodeNumbering),
                                    CodeType =
                                            Convert.ToString(Convert.ToInt32(OrderType.HCPCS)),
                                    CodeTypeName = "DRG",
                                    ExternalCode = string.Empty,
                                    ID = Convert.ToString(item.DRGCodesId)
                                }));
                    }
                    break;
                case 5:
                    using (var dbal = new DrugBal(DrugTableNumber))
                    {
                        var result = dbal.GetFilteredDrugCodes(text).Where(x => x.InStock == true).ToList();
                        finalList.AddRange(
                            result.Select(
                                item =>
                                new GeneralCodesCustomModel
                                {
                                    Code = item.DrugCode,
                                    Description = item.DrugPackageName,
                                    CodeDescription =
                                            string.Format(
                                                "{0} - {1} - {2} - {3}",
                                                item.DrugPackageName,
                                                item.DrugCode,
                                                item.DrugStrength,
                                                item.DrugDosage),
                                    CodeType =
                                            Convert.ToString(Convert.ToInt32(OrderType.DRUG)),
                                    CodeTypeName = "DRUG",
                                    ExternalCode = Convert.ToString(item.BrandCode),
                                    ID = Convert.ToString(item.Id)
                                }));
                    }
                    break;
            }
            return finalList;
        }

        /// <summary>
        /// Gets the selected code parent.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns></returns>
        public GeneralCodesCustomModel GetSelectedCodeParent(string code)
        {
            var finalList = new GeneralCodesCustomModel();
            var finalListAll = new List<GeneralCodesCustomModel>();
            using (var bal = new GlobalCodeBal())
            {
                using (var cptBal = new CPTCodesBal(CptTableNumber))
                {
                    var cpt = cptBal.GetCPTCodesByCode(code);
                    var cptCoderange = cpt != null ? cpt.CTPCodeRangeValue : null;
                    if (cptCoderange != null)
                    {
                        var cptCoderangeInt = Convert.ToInt32(cptCoderange);
                        finalListAll.Add(bal.GetGeneralGlobalCodeByRangeVal(cptCoderangeInt, Convert.ToInt32(OrderType.CPT).ToString()));
                    }
                }
                using (var drugBal = new DrugBal(DrugTableNumber))
                {
                    var drugObj = drugBal.GetCurrentDrugByCode(code);
                    var drugBrandId = drugObj != null ? drugObj.BrandCode : null;
                    if (drugBrandId != null)
                    {
                        var drugBrandIdInt = Convert.ToInt32(drugBrandId);
                        finalListAll.Add(
                            bal.GetGeneralGlobalCodeByRangeVal(
                                drugBrandIdInt,
                                Convert.ToInt32(OrderType.DRUG).ToString()));
                    }
                }
                using (var hcpcsBal = new HCPCSCodesBal(HcpcsTableNumber))
                {
                    var hcpcsObj = hcpcsBal.GetFilteredHCPCSCodes(code);
                    if (hcpcsObj.Any())
                    {
                        var generalCodesCustomModel = new GeneralCodesCustomModel
                        {
                            GlobalCodeCategoryName =
                                                                  "HCPCS Codes",
                            GlobalCodeCategoryId = "11200",
                            GlobalCodeName = "NA",
                            GlobalCodeId = 9090
                        };
                        finalListAll.Add(generalCodesCustomModel);
                    }
                }
            }
            if (finalListAll.Count > 0)
            {
                finalList = finalListAll.FirstOrDefault();
            }
            return finalList;
        }

        /// <summary>
        /// Gets the global code by global code category identifier global code value.
        /// </summary>
        /// <param name="gcCategoryValue">The global code category value.</param>
        /// <param name="gcvalue">The global codevalue.</param>
        /// <returns></returns>
        public GlobalCodes GetGlobalCodeByCategoryAndCodeValue(string gcCategoryValue, string gcvalue)
        {
            using (var gcRep = UnitOfWork.GlobalCodeRepository)
            {
                var globalCode =
                    gcRep.Where(
                        s =>
                        s.IsDeleted == false && s.GlobalCodeCategoryValue.Equals(gcCategoryValue)
                        && s.GlobalCodeValue.Equals(gcvalue)).FirstOrDefault();
                return globalCode ?? new GlobalCodes();
            }
        }

        /// <summary>
        /// Gets the table strutureby table identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public List<GlobalCodes> GetTableStruturebyTableId(string id)
        {
            using (var gcRep = UnitOfWork.GlobalCodeRepository)
            {
                var tableNameCategoryvalue = Convert.ToInt32(GlobalCodeCategoryValue.Column).ToString();
                var globalCodelist =
                    gcRep.Where(
                        s =>
                        s.IsDeleted == false && s.GlobalCodeCategoryValue.Equals(tableNameCategoryvalue)
                        && s.ExternalValue1.Equals(id) && s.IsActive).ToList();
                return globalCodelist;
            }
        }

        /// <summary>
        /// Gets the name of the column data type by table name column.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        public string GetColumnDataTypeByTableNameColumnName(string tableName, string columnName)
        {
            var list = string.Empty;
            var entity = (TableNames)Enum.Parse(typeof(TableNames), tableName);
            switch (entity)
            {
                case TableNames.Authorization:
                    var authorizationval =
                        typeof(Authorization).GetProperties().FirstOrDefault(i => i.Name == columnName);
                    if (authorizationval != null)
                        list = authorizationval.PropertyType.GenericTypeArguments.Length > 0
                                   ? authorizationval.PropertyType.GenericTypeArguments[0].Name
                                   : authorizationval.PropertyType.Name;
                    break;
                case TableNames.BillHeader:
                    var billHeaderVal = typeof(BillHeader).GetProperties().FirstOrDefault(i => i.Name == columnName);
                    if (billHeaderVal != null)
                        list = billHeaderVal.PropertyType.GenericTypeArguments.Length > 0
                                   ? billHeaderVal.PropertyType.GenericTypeArguments[0].Name
                                   : billHeaderVal.PropertyType.Name;
                    break;
                case TableNames.Corporate:
                    var corporateVal = typeof(Corporate).GetProperties().FirstOrDefault(i => i.Name == columnName);
                    if (corporateVal != null)
                        list = corporateVal.PropertyType.GenericTypeArguments.Length > 0
                                   ? corporateVal.PropertyType.GenericTypeArguments[0].Name
                                   : corporateVal.PropertyType.Name;
                    break;
                case TableNames.Denial:
                    var DenialVal = typeof(Denial).GetProperties().FirstOrDefault(i => i.Name == columnName);
                    if (DenialVal != null)
                        list = DenialVal.PropertyType.GenericTypeArguments.Length > 0
                                   ? DenialVal.PropertyType.GenericTypeArguments[0].Name
                                   : DenialVal.PropertyType.Name;
                    break;
                case TableNames.Diagnosis:
                    var DiagnosisVal = typeof(Diagnosis).GetProperties().FirstOrDefault(i => i.Name == columnName);
                    if (DiagnosisVal != null)
                        list = DiagnosisVal.PropertyType.GenericTypeArguments.Length > 0
                                   ? DiagnosisVal.PropertyType.GenericTypeArguments[0].Name
                                   : DiagnosisVal.PropertyType.Name;
                    break;
                case TableNames.DiagnosisCode:
                    var DiagnosisCodeVal =
                        typeof(DiagnosisCode).GetProperties().FirstOrDefault(i => i.Name == columnName);
                    if (DiagnosisCodeVal != null)
                        list = DiagnosisCodeVal.PropertyType.GenericTypeArguments.Length > 0
                                   ? DiagnosisCodeVal.PropertyType.GenericTypeArguments[0].Name
                                   : DiagnosisCodeVal.PropertyType.Name;
                    break;
                case TableNames.Drug:
                    var DrugVal = typeof(Drug).GetProperties().FirstOrDefault(i => i.Name == columnName);
                    if (DrugVal != null)
                        list = DrugVal.PropertyType.GenericTypeArguments.Length > 0
                                   ? DrugVal.PropertyType.GenericTypeArguments[0].Name
                                   : DrugVal.PropertyType.Name;
                    break;
                case TableNames.Encounter:
                    var EncounterVal = typeof(Encounter).GetProperties().FirstOrDefault(i => i.Name == columnName);
                    if (EncounterVal != null)
                        list = EncounterVal.PropertyType.GenericTypeArguments.Length > 0
                                   ? EncounterVal.PropertyType.GenericTypeArguments[0].Name
                                   : EncounterVal.PropertyType.Name;
                    break;
                case TableNames.Facility:
                    var FacilityVal = typeof(Facility).GetProperties().FirstOrDefault(i => i.Name == columnName);
                    if (FacilityVal != null)
                        list = FacilityVal.PropertyType.GenericTypeArguments.Length > 0
                                   ? FacilityVal.PropertyType.GenericTypeArguments[0].Name
                                   : FacilityVal.PropertyType.Name;
                    break;
                case TableNames.HCPCSCodes:
                    var HCPCSCodesVal = typeof(HCPCSCodes).GetProperties().FirstOrDefault(i => i.Name == columnName);
                    if (HCPCSCodesVal != null)
                        list = HCPCSCodesVal.PropertyType.GenericTypeArguments.Length > 0
                                   ? HCPCSCodesVal.PropertyType.GenericTypeArguments[0].Name
                                   : HCPCSCodesVal.PropertyType.Name;
                    break;
                case TableNames.InsuranceCompany:
                    var InsuranceCompanyVal =
                        typeof(InsuranceCompany).GetProperties().FirstOrDefault(i => i.Name == columnName);
                    if (InsuranceCompanyVal != null)
                        list = InsuranceCompanyVal.PropertyType.GenericTypeArguments.Length > 0
                                   ? InsuranceCompanyVal.PropertyType.GenericTypeArguments[0].Name
                                   : InsuranceCompanyVal.PropertyType.Name;
                    break;
                case TableNames.InsurancePlans:
                    var InsurancePlansVal =
                        typeof(InsurancePlans).GetProperties().FirstOrDefault(i => i.Name == columnName);
                    if (InsurancePlansVal != null)
                        list = InsurancePlansVal.PropertyType.GenericTypeArguments.Length > 0
                                   ? InsurancePlansVal.PropertyType.GenericTypeArguments[0].Name
                                   : InsurancePlansVal.PropertyType.Name;
                    break;
                case TableNames.InsurancePolices:
                    var InsurancePolicesVal =
                        typeof(InsurancePolices).GetProperties().FirstOrDefault(i => i.Name == columnName);
                    if (InsurancePolicesVal != null)
                        list = InsurancePolicesVal.PropertyType.GenericTypeArguments.Length > 0
                                   ? InsurancePolicesVal.PropertyType.GenericTypeArguments[0].Name
                                   : InsurancePolicesVal.PropertyType.Name;
                    break;
                case TableNames.MCContract:
                    var ManagedCareVal = typeof(MCContract).GetProperties().FirstOrDefault(i => i.Name == columnName);
                    if (ManagedCareVal != null)
                        list = ManagedCareVal.PropertyType.GenericTypeArguments.Length > 0
                                   ? ManagedCareVal.PropertyType.GenericTypeArguments[0].Name
                                   : ManagedCareVal.PropertyType.Name;
                    break;
                case TableNames.OpenOrder:
                    var OpenOrderVal = typeof(OpenOrder).GetProperties().FirstOrDefault(i => i.Name == columnName);
                    if (OpenOrderVal != null)
                        list = OpenOrderVal.PropertyType.GenericTypeArguments.Length > 0
                                   ? OpenOrderVal.PropertyType.GenericTypeArguments[0].Name
                                   : OpenOrderVal.PropertyType.Name;
                    break;
                case TableNames.OpenOrderActivitySchedule:
                    var OpenOrderActivityScheduleVal =
                        typeof(OpenOrderActivitySchedule).GetProperties().FirstOrDefault(i => i.Name == columnName);
                    if (OpenOrderActivityScheduleVal != null)
                        list = OpenOrderActivityScheduleVal.PropertyType.GenericTypeArguments.Length > 0
                                   ? OpenOrderActivityScheduleVal.PropertyType.GenericTypeArguments[0].Name
                                   : OpenOrderActivityScheduleVal.PropertyType.Name;
                    break;
                case TableNames.OpenOrderActivityTime:
                    var OpenOrderActivityTimeVal =
                        typeof(OpenOrderActivityTime).GetProperties().FirstOrDefault(i => i.Name == columnName);
                    if (OpenOrderActivityTimeVal != null)
                        list = OpenOrderActivityTimeVal.PropertyType.GenericTypeArguments.Length > 0
                                   ? OpenOrderActivityTimeVal.PropertyType.GenericTypeArguments[0].Name
                                   : OpenOrderActivityTimeVal.PropertyType.Name;
                    break;
                case TableNames.OrderActivity:
                    var OrderActivityVal =
                        typeof(OrderActivity).GetProperties().FirstOrDefault(i => i.Name == columnName);
                    if (OrderActivityVal != null)
                        list = OrderActivityVal.PropertyType.GenericTypeArguments.Length > 0
                                   ? OrderActivityVal.PropertyType.GenericTypeArguments[0].Name
                                   : OrderActivityVal.PropertyType.Name;
                    break;
                case TableNames.PatientInfo:
                    var PatientInfoVal = typeof(PatientInfo).GetProperties().FirstOrDefault(i => i.Name == columnName);
                    if (PatientInfoVal != null)
                        list = PatientInfoVal.PropertyType.GenericTypeArguments.Length > 0
                                   ? PatientInfoVal.PropertyType.GenericTypeArguments[0].Name
                                   : PatientInfoVal.PropertyType.Name;
                    break;
                case TableNames.RuleMaster:
                    var RuleMasterVal = typeof(RuleMaster).GetProperties().FirstOrDefault(i => i.Name == columnName);
                    if (RuleMasterVal != null)
                        list = RuleMasterVal.PropertyType.GenericTypeArguments.Length > 0
                                   ? RuleMasterVal.PropertyType.GenericTypeArguments[0].Name
                                   : RuleMasterVal.PropertyType.Name;
                    break;
                case TableNames.PatientInsurance:
                    var PatientInsuranceVal =
                        typeof(PatientInsurance).GetProperties().FirstOrDefault(i => i.Name == columnName);
                    if (PatientInsuranceVal != null)
                        list = PatientInsuranceVal.PropertyType.GenericTypeArguments.Length > 0
                                   ? PatientInsuranceVal.PropertyType.GenericTypeArguments[0].Name
                                   : PatientInsuranceVal.PropertyType.Name;
                    break;
                case TableNames.CPTCodes:
                    var CPTCodesVal = typeof(CPTCodes).GetProperties().FirstOrDefault(i => i.Name == columnName);
                    if (CPTCodesVal != null) //list = CPTCodesVal.PropertyType.GenericTypeArguments[0].Name;
                        list = CPTCodesVal.PropertyType.GenericTypeArguments.Length > 0
                                   ? CPTCodesVal.PropertyType.GenericTypeArguments[0].Name
                                   : CPTCodesVal.PropertyType.Name;
                    break;
                case TableNames.DRGCodes:
                    var DRGCodesVal = typeof(DRGCodes).GetProperties().FirstOrDefault(i => i.Name == columnName);
                    if (DRGCodesVal != null)
                        list = DRGCodesVal.PropertyType.GenericTypeArguments.Length > 0
                                   ? DRGCodesVal.PropertyType.GenericTypeArguments[0].Name
                                   : DRGCodesVal.PropertyType.Name;
                    break;
                case TableNames.ServiceCode:
                    var ServiceCodeVal = typeof(ServiceCode).GetProperties().FirstOrDefault(i => i.Name == columnName);
                    if (ServiceCodeVal != null)
                        list = ServiceCodeVal.PropertyType.GenericTypeArguments.Length > 0
                                   ? ServiceCodeVal.PropertyType.GenericTypeArguments[0].Name
                                   : ServiceCodeVal.PropertyType.Name;
                    break;
                default:
                    break;
            }
            return list;
        }

        /// <summary>
        /// Gets the invariant culture date time.
        /// </summary>
        /// <param name="facilityid">The facilityid.</param>
        /// <returns></returns>
        public DateTime GetInvariantCultureDateTime(int facilityid)
        {

            var facilitybal = new FacilityBal();
            var facilityObj = facilitybal.GetFacilityTimeZoneById(facilityid);
            var tzi = TimeZoneInfo.FindSystemTimeZoneById(facilityObj);
            var utcTime = DateTime.Now.ToUniversalTime();
            var convertedTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);
            //d = DateTime.Parse(s, CultureInfo.InvariantCulture);
            //string strDate = localTime.ToString(FMT);
            //DateTime now2 = DateTime.ParseExact(strDate, FMT, CultureInfo.InvariantCulture);
            // DateTime now2 = DateTime.Parse(s, CultureInfo.InvariantCulture);
            return convertedTime;
        }

        /// <summary>
        /// Gets the bill number by bill header identifier.
        /// </summary>
        /// <param name="billHeaderId">The bill header identifier.</param>
        /// <returns></returns>
        public string GetBillNumberByBillHeaderId(int billHeaderId)
        {
            var billheaderbal = new BillHeaderBal();
            var billheaderobj = billheaderbal.GetBillHeaderById(billHeaderId);
            return billheaderobj != null ? billheaderobj.BillNumber : "NA";
        }

        /// <summary>
        /// Gets the bill header status by status.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <returns></returns>
        public string GetBillHeaderStatusByStatus(string status)
        {
            var gc = GetNameByGlobalCodeValue(
                status,
                Convert.ToInt32(GlobalCodeCategoryValue.BillHeaderStatus).ToString());
            return !string.IsNullOrEmpty(gc) ? gc : "Initialized";
        }

        /// <summary>
        /// Gets the service codes.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public List<GeneralCodesCustomModel> GetServiceCodes(string text)
        {
            var finalList = new List<GeneralCodesCustomModel>();
            using (var serviceCodeBal = new ServiceCodeBal(ServiceCodeTableNumber))
            {
                var serviceCodesList = serviceCodeBal.GetServiceCodes();
                finalList.AddRange(
                    serviceCodesList.Select(
                        item =>
                        new GeneralCodesCustomModel
                        {
                            Code = item.ServiceCodeValue,
                            Description = item.ServiceCodeDescription,
                            CodeDescription =
                                    string.Format(
                                        "{0} - {1}",
                                        item.ServiceCodeValue,
                                        item.ServiceCodeDescription),
                            CodeType = Convert.ToString(OrderType.BedCharges),
                            CodeTypeName = "Service Code",
                            //ExternalCode = item.CTPCodeRangeValue.ToString(),
                            ID = item.ServiceCodeId.ToString()
                        }));
            }
            if (finalList.Count > 0)
            {
                text = text.ToLower().Trim();
                finalList = finalList.Where(f => f.CodeDescription.ToLower().Contains(text)).ToList();
            }
            return finalList;
        }

        /// <summary>
        /// Gets the insurance member identifier by patient identifier.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public string GetInsuranceMemberIdByPatientId(int patientId)
        {
            using (var rep = UnitOfWork.PatientInsuranceRepository)
            {
                var ins = rep.Where(e => e.PatientID == patientId).FirstOrDefault();
                return ins != null ? ins.PersonHealthCareNumber.ToString() : string.Empty;
            }
        }

        /// <summary>
        /// Gets the authorization identifier payer by patient identifier.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public string GetAuthorizationIdPayerByPatientId(int patientId)
        {
            using (var rep = UnitOfWork.PatientInsuranceRepository)
            {
                var ins = rep.Where(e => e.PatientID == patientId).FirstOrDefault();
                return ins != null ? ins.InsuranceCompanyId.ToString() : string.Empty;
            }
        }

        /// <summary>
        /// Gets the indicator name by indicatornumber.
        /// </summary>
        /// <param name="indicatornumber">The indicatornumber.</param>
        /// <returns></returns>
        public string GetIndicatorNameByIndicatornumber(string indicatornumber, Int32 corporateId)
        {
            using (var rep = UnitOfWork.DashboardIndicatorsRepository)
            {
                var ins = rep.Where(e => e.IndicatorNumber == indicatornumber && e.CorporateId == corporateId).FirstOrDefault();
                return ins != null ? ins.Description : string.Empty;
            }
        }

        /// <summary>
        /// Gets the name of the facility identifier from.
        /// </summary>
        /// <param name="facilityName">Name of the facility.</param>
        /// <returns></returns>
        public int GetFacilityIdFromName(string facilityName)
        {
            var id = 0;
            if (!string.IsNullOrEmpty(facilityName))
            {
                using (var rep = UnitOfWork.FacilityRepository)
                {
                    facilityName = facilityName.ToLower().Trim();
                    var obj = rep.Where(f => f.FacilityName.ToLower().Trim().Equals(facilityName)).FirstOrDefault();
                    if (obj != null) id = obj.FacilityId;
                }
            }
            return id;
        }

        /// <summary>
        /// Gets the name of the corporate identifier from.
        /// </summary>
        /// <param name="corpName">Name of the corp.</param>
        /// <returns></returns>
        public int GetCorporateIdFromName(string corpName)
        {
            var id = 0;
            if (!string.IsNullOrEmpty(corpName))
            {
                using (var rep = UnitOfWork.CorporateRepository)
                {
                    corpName = corpName.ToLower().Trim();
                    var obj = rep.Where(f => f.CorporateName.ToLower().Trim().Equals(corpName)).FirstOrDefault();
                    if (obj != null) id = obj.CorporateID;
                }
            }
            return id;
        }

        /// <summary>
        /// Gets the corporate name from identifier.
        /// </summary>
        /// <param name="corpId">The corp identifier.</param>
        /// <returns></returns>
        public string GetCorporateNameFromId(int corpId)
        {
            var corpName = "";
            using (var rep = UnitOfWork.CorporateRepository)
            {
                var obj = rep.Where(f => f.CorporateID == corpId).FirstOrDefault();
                if (obj != null) corpName = obj.CorporateName;
            }
            return corpName;
        }

        /// <summary>
        /// Gets the corporate identifier from facility identifier.
        /// </summary>
        /// <param name="facilityid">The facilityid.</param>
        /// <returns></returns>
        public int GetCorporateIdFromFacilityId(int facilityid)
        {
            var id = 0;
            using (var rep = UnitOfWork.FacilityRepository)
            {
                var obj = rep.Where(f => f.FacilityId == facilityid).FirstOrDefault();
                if (obj != null)
                {
                    id = Convert.ToInt32(obj.CorporateID);
                }
            }

            return id;
        }

        /// <summary>
        /// Gets the name of the global code identifier by.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="gcName">Name of the gc.</param>
        /// <returns></returns>
        public string GetGlobalCodeIdByName(string category, string gcName)
        {
            var id = string.Empty;
            if (!string.IsNullOrEmpty(gcName) && !string.IsNullOrEmpty(category))
            {
                gcName = gcName.ToLower().Trim();
                using (var rep = UnitOfWork.GlobalCodeRepository)
                {
                    var obj =
                        rep.Where(
                            g =>
                            g.GlobalCodeCategoryValue.Equals(category)
                            && g.GlobalCodeName.ToLower().Trim().Contains(gcName)).FirstOrDefault();

                    if (obj != null) id = obj.GlobalCodeValue;
                }
            }
            return id;
        }

        /// <summary>
        /// Saves the indicators.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public int SaveIndicators(DashboardIndicators model)
        {

            using (var rep = UnitOfWork.DashboardIndicatorsRepository)
            {
                var current =
                    rep.Where(
                        d => d.CorporateId == model.CorporateId && d.IndicatorNumber.Equals(model.IndicatorNumber))
                        .FirstOrDefault();
                if (current != null)
                {
                    current.OwnerShip = model.OwnerShip;
                    //current.Description = model.Description;
                    rep.UpdateEntity(current, current.IndicatorID);
                    return current.IndicatorID;
                }
            }
            return 0;
        }

        /// <summary>
        /// Calculates the type of the lab result speciman.
        /// </summary>
        /// <param name="ordercode">The ordercode.</param>
        /// <param name="resultminvalue">The resultminvalue.</param>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public string CalculateLabResultSpecimanType(string ordercode, decimal? resultminvalue, int? patientId)
        {
            try
            {
                using (var orderActivityRep = UnitOfWork.OrderActivityRepository)
                {
                    var lstOrderActivity = orderActivityRep.GetLabResultStatusString(
                        Convert.ToInt32(ordercode.Trim()),
                        Convert.ToDecimal(resultminvalue),
                        Convert.ToInt32(patientId));
                    var labresultStatus = lstOrderActivity.FirstOrDefault();
                    return labresultStatus != null
                               ? !string.IsNullOrEmpty(labresultStatus.LabTestSpecimanStr)
                                     ? labresultStatus.LabTestSpecimanStr
                                     : string.Empty
                               : string.Empty;
                }
            }
            catch (Exception)
            {
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the drug by code.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns></returns>
        protected Drug GetDrugByCode(string code)
        {
            using (var rep = UnitOfWork.DrugRepository)
            {
                var drug =
                    rep.Where(d => d.DrugCode.Trim().Equals(code) && d.DrugTableNumber.Trim().Equals(DrugTableNumber))
                        .FirstOrDefault();
                return drug ?? new Drug();
            }
        }

        /// <summary>
        /// Gets the patient information by encounter identifier.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        public PatientInfo GetPatientInfoByEncounterId(int encounterId)
        {
            var patientInfo = new PatientInfo();
            using (var rep = UnitOfWork.PatientInfoRepository)
            {
                var patientId = GetPatientIdByEncounterId(encounterId);

                if (patientId > 0) patientInfo = rep.Where(p => p.PatientID == patientId && p.IsDeleted == false).FirstOrDefault();
            }
            return patientInfo;
        }

        /// <summary>
        /// Gets the facility lic number by facility identifier.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public string GetFacilityLicNumberByFacilityId(int facilityId)
        {
            if (facilityId > 0)
            {
                using (var rep = UnitOfWork.FacilityRepository)
                {
                    var facility = rep.Where(f => f.FacilityId == facilityId).FirstOrDefault();
                    return facility != null ? facility.FacilityLicenseNumber : "";
                }
            }
            return "";
        }

        /// <summary>
        /// Gets the facility sender identifier by facility identifier.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public string GetFacilitySenderIdByFacilityId(int facilityId)
        {
            if (facilityId > 0)
            {
                using (var rep = UnitOfWork.FacilityRepository)
                {
                    var facility = rep.Where(f => f.FacilityId == facilityId).FirstOrDefault();
                    return facility != null ? facility.SenderID : "";
                }
            }
            return "";
        }

        /// <summary>
        /// Gets the user name by user identifier.
        /// </summary>
        /// <param name="UserID">The user identifier.</param>
        /// <returns></returns>
        public string GetUserNameByUserId(int? UserID)
        {
            try
            {
                using (var usersRep = UnitOfWork.UsersRepository)
                {
                    var usersModel = usersRep.Where(x => x.UserID == UserID && x.IsDeleted == false).FirstOrDefault();
                    return usersModel != null ? usersModel.UserName : string.Empty;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the sub categories2.
        /// </summary>
        /// <param name="gcValue1">The gc value1.</param>
        /// <returns></returns>
        public List<GlobalCodes> GetSubCategories2(string gcValue1)
        {
            var list = new List<GlobalCodes>();
            using (var rep = UnitOfWork.GlobalCodeRepository)
            {
                list =
                    rep.Where(
                        f => f.ExternalValue1.Trim().Equals(gcValue1) && f.GlobalCodeCategoryValue.Trim().Equals("4351"))
                        .ToList();

            }
            return list;
        }

        /// <summary>
        /// Gets the facilities by corporate identifier.
        /// </summary>
        /// <param name="cId">The c identifier.</param>
        /// <returns></returns>
        public List<int> GetFacilityIdsByCorporateId(int cId)
        {
            List<int> list;
            using (var rep = UnitOfWork.FacilityRepository) list = rep.Where(c => c.CorporateID == cId).Select(f => f.FacilityId).ToList();
            return list;
        }

        /// <summary>
        /// Gets the bill header status.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <returns></returns>
        public string GetBillHeaderStatus(string status)
        {
            var gc = GetNameByGlobalCodeValue(
                status,
                Convert.ToInt32(GlobalCodeCategoryValue.BillHeaderStatus).ToString());
            return !string.IsNullOrEmpty(gc) ? gc : "Initialized";
        }

        public bool CheckIfPrimaryDiagnosis(int patientId, int encounterId, int diagnosisType)
        {
            bool isExists;
            using (var rep = UnitOfWork.DiagnosisRespository)
            {
                isExists =
                    rep.Where(
                        d =>
                        d.PatientID == patientId && d.EncounterID == encounterId && d.IsDeleted != true
                        && d.DiagnosisType == diagnosisType).Any();
            }
            return isExists;
        }

        /// <summary>
        /// Gets the rule step details by identifier.
        /// </summary>
        /// <param name="ruleStepId">The rule step identifier.</param>
        /// <returns></returns>
        public RuleStep GetRuleStepDetailsById(int ruleStepId)
        {
            using (var rep = UnitOfWork.RuleStepRepository)
            {
                var current = rep.Where(r => r.RuleStepID == ruleStepId).FirstOrDefault();
                return current ?? new RuleStep();
            }
        }

        /// <summary>
        /// Saves the records fortable number.
        /// </summary>
        /// <param name="tableNumber">The table number.</param>
        /// <param name="selectedCodeid">The selected codeid.</param>
        /// <param name="typeid">The typeid.</param>
        /// <returns></returns>
        public bool SaveRecordsFortableNumber(string tableNumber, IEnumerable<string> selectedCodeid, string typeid)
        {
            try
            {
                var idslist = selectedCodeid.OfType<string>().ToList();
                var joinedstring = String.Join(",", idslist);
                var savedData = SaveRecordsbyTableNumber(tableNumber, joinedstring, typeid);
                return savedData == 1;
            }
            catch (Exception)
            {
                return true;
            }
        }

        /// <summary>
        /// Saves the recordsby table number.
        /// </summary>
        /// <param name="tableNumber">The table number.</param>
        /// <param name="selectedCodeid">The selected codeid.</param>
        /// <param name="typeid">The typeid.</param>
        /// <returns></returns>
        public int SaveRecordsbyTableNumber(string tableNumber, string selectedCodeid, string typeid)
        {
            using (var rep = UnitOfWork.ServiceCodeRepository)
            {
                var saveData = rep.SaveDataSaveBillingTableDataForTableNumber(tableNumber, selectedCodeid, typeid);
                return Convert.ToInt32(saveData);
            }
        }

        /// <summary>
        /// Gets the bill editor users.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public List<BillEditorUsersCustomModel> GetBillEditorUsers(int corporateId, int facilityId)
        {
            using (var userRep = UnitOfWork.UsersRepository)
            {
                var usersList = userRep.GetBillEditRoleUser(corporateId, facilityId);
                return usersList;
            }
        }

        /// <summary>
        /// Checks for duplicate table set.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="tableNumber">The table number.</param>
        /// <param name="typeId">The type identifier.</param>
        /// <returns></returns>
        public bool CheckForDuplicateTableSet(int id, string tableNumber, string typeId)
        {
            using (var rep = UnitOfWork.BillingCodeTableSetRepository)
            {
                return
                    rep.Where(
                        d =>
                        (d.Id == id || id == 0) && d.TableNumber.Trim().Equals(tableNumber)
                        && d.CodeTableType.Trim().Equals(typeId)).Any();
            }
        }

        /// <summary>
        /// Saves the table number.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public int SaveTableNumber(BillingCodeTableSet model)
        {
            var newId = 0;
            using (var rep = UnitOfWork.BillingCodeTableSetRepository)
            {
                rep.Create(model);
                newId = model.Id;
            }
            return newId;
        }

        /// <summary>
        /// Gets the table numbers list.
        /// </summary>
        /// <param name="typeId">The type identifier.</param>
        /// <returns></returns>
        public List<BillingCodeTableSet> GetTableNumbersList(string typeId)
        {
            using (var rep = UnitOfWork.BillingCodeTableSetRepository)
            {
                return
                    rep.Where(d => d.CodeTableType.Trim().Equals(typeId) || string.IsNullOrEmpty(typeId))
                        .OrderBy(f => f.CodeTableType)
                        .ToList();
            }
        }

        /// <summary>
        /// Gets the rule step details by identifier.
        /// </summary>
        /// <param name="ruleMasterId">The rule master identifier.</param>
        /// <returns></returns>
        public RuleMaster GetRuleMasterDetailsById(int ruleMasterId)
        {
            using (var rep = UnitOfWork.RuleMasterRepository)
            {
                var current = rep.Where(r => r.RuleMasterID == ruleMasterId).FirstOrDefault();
                return current ?? new RuleMaster();
            }
        }

        /// <summary>
        /// Gets the department name by identifier.
        /// </summary>
        /// <param name="facilityDeaprtmentId">The facility deaprtment identifier.</param>
        /// <returns></returns>
        public string GetDepartmentNameById(int facilityDeaprtmentId)
        {
            using (var facilityrep = UnitOfWork.FacilityStructureRepository)
            {
                var facilityDepartment =
                    facilityrep.Where(x => x.FacilityStructureId == facilityDeaprtmentId).FirstOrDefault();
                return facilityDepartment == null ? string.Empty : facilityDepartment.FacilityStructureName;
            }
        }

        /// <summary>
        /// Gets the faculty department.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public Physician GetFacultyByUserId(int userId)
        {
            var physicainBal = new PhysicianBal();
            var physcianObj = physicainBal.GetPhysicianByUserId(userId);
            return physcianObj;
        }

        /// <summary>
        /// Gets the week of year is o8601.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        public int GetWeekOfYearISO8601Bal(DateTime date)
        {
            var day = (int)CultureInfo.CurrentCulture.Calendar.GetDayOfWeek(date);
            return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                date.AddDays(4 - (day == 0 ? 7 : day)),
                CalendarWeekRule.FirstFourDayWeek,
                DayOfWeek.Monday);
        }

        /// <summary>
        /// Firsts the date of week.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <param name="weekNum">The week number.</param>
        /// <param name="rule">The rule.</param>
        /// <returns></returns>
        public DateTime FirstDateOfWeekBal(int year, int weekNum, CalendarWeekRule rule)
        {
            var jan1 = new DateTime(year, 1, 1);

            var daysOffset = DayOfWeek.Monday - jan1.DayOfWeek;
            var firstMonday = jan1.AddDays(daysOffset);
            var cal = CultureInfo.CurrentCulture.Calendar;
            var firstWeek = cal.GetWeekOfYear(firstMonday, rule, DayOfWeek.Monday);

            if (firstWeek > 0) weekNum -= 1;

            var result = firstMonday.AddDays(weekNum * 7);

            return result;
        }

        /// <summary>
        /// Gets the corporate physician roles.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <returns></returns>
        public List<UserRole> GetCorporatePhysicianRoles(int corporateId)
        {
            using (var rolebal = new RoleBal())
            {
                using (var userRoleBal = new UserRoleBal())
                {
                    var roleIdlist = rolebal.GetPhysicianRolesByCorporateId(corporateId);
                    var physicianTypeUserId =
                        userRoleBal.GetUserIdByCorporateAndRoleTypeId(corporateId, roleIdlist).ToList();
                    return physicianTypeUserId;
                }
            }
        }

        /// <summary>
        /// Gets the appointment name by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public string GetAppointmentNameById(int id, int corporateId, int facilityId)
        {
            using (var rep = UnitOfWork.AppointmentTypesRepository)
            {
                var model =
                    rep.Where(x => x.Id == id && x.CorporateId == corporateId && x.FacilityId == facilityId)
                        .FirstOrDefault();
                return model != null ? model.Name : string.Empty;
            }
        }

        /// <summary>
        /// Gets the appointment type time interval by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public string GetAppointmentTypeTimeIntervalById(int id, int corporateId, int facilityId)
        {
            using (var rep = UnitOfWork.AppointmentTypesRepository)
            {
                var model =
                    rep.Where(x => x.Id == id && x.CorporateId == corporateId && x.FacilityId == facilityId)
                        .FirstOrDefault();

                return model != null ? model.DefaultTime : string.Empty;
                //var defaulttime = model != null ? model.DefaultTime : string.Empty;
                //return this.GetNameByGlobalCodeValue(
                //     defaulttime,
                //     Convert.ToInt32(GlobalCodeCategoryValue.AppointmentDefaultTime).ToString());
            }
        }


        public string GetAppointmentTypeById(int id)
        {
            using (var rep = UnitOfWork.AppointmentTypesRepository)
            {
                var model = rep.Where(a => a.Id == id).FirstOrDefault();
                return model != null ? model.Name : string.Empty;
            }
        }


        /// <summary>
        /// Gets the patient custom model by patient identifier.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public PatientInfoCustomModel GetPatientCustomModelByPatientId(int patientId)
        {
            var pInfoCModel = new PatientInfoCustomModel();

            //Get the patient details from the table PatientInfo
            using (var pBal = UnitOfWork.PatientInfoRepository)
            {
                var pInfo = pBal.Where(p => p.PatientID == patientId).Include(p => p.PatientAddressRelation).Include(p => p.PatientPhone).FirstOrDefault();
                pInfoCModel.PatientInfo = pInfo;
                pInfoCModel.PatientName = pInfo != null ? string.Format("{0} {1}", pInfo.PersonFirstName, pInfo.PersonLastName) : string.Empty;
                pInfoCModel.PersonAge = GetAgeByDate(Convert.ToDateTime(pInfo != null ? pInfo.PersonBirthDate : DateTime.Now));
            }
            return pInfoCModel;
        }


        public string GetPhysicianName(int physicianId)
        {
            using (var rep = UnitOfWork.PhysicianRepository)
            {
                var phylist = rep.Where(x => x.Id == physicianId).FirstOrDefault();
                return phylist != null ? phylist.PhysicianName : string.Empty;
            }
        }

        public string GetTimeToRecurrance(Scheduling itemModel)
        {
            var timefrom = itemModel.RecurringDateFrom;
            var ticksforTimeFrom = Convert.ToDateTime(timefrom);
            var timeto = ticksforTimeFrom.AddTicks(Convert.ToInt64(itemModel.RecEventlength * 10000000));
            return timeto.ToString("HH:mm");
        }


        public string GetTimingAddedById(int id)
        {
            using (var fBal = UnitOfWork.DeptTimmingRepository)
            {
                var list = fBal.Where(x => x.FacilityStructureID == id && x.IsActive).FirstOrDefault();
                return list != null && list.IsActive ? "YES" : "NO";
            }
        }

        public string GetFacilityStructureNameById(int id)
        {
            using (var facilitySructureBal = new FacilityStructureBal())
            {
                var roomObj = facilitySructureBal.GetFacilityStructureById(id);
                return roomObj != null ? roomObj.FacilityStructureName : string.Empty;
            }
        }

        public List<GlobalCodes> GetGlobalCodesListByCategory(string gcc)
        {
            using (var rep = UnitOfWork.GlobalCodeRepository)
            {
                var list =
                    rep.Where(g => g.IsActive && g.IsDeleted != true && g.GlobalCodeCategoryValue.Trim().Equals(gcc)).ToList();
                return list;
            }
        }

        /// <summary>
        /// Encounters the open orders.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        public bool EncounterOpenOrders(int encounterId)
        {
            using (var oRep = UnitOfWork.OpenOrderRepository)
            {
                return
                    oRep.Where(p => p.CreatedBy != null && (int)p.EncounterID == encounterId && p.OrderStatus == "1")
                        .ToList().Any();
            }
        }


        /// <summary>
        /// Patients the encounter open orders.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public bool PatientEncounterOpenOrders(int patientId)
        {
            var patientCurrentEncounterid = this.GetPatientEncounterById(patientId);
            return this.EncounterOpenOrders(patientCurrentEncounterid);
        }

        /// <summary>
        /// Gets the patient encounter by identifier.
        /// </summary>
        /// <param name="pid">The pid.</param>
        /// <returns></returns>
        public int GetPatientEncounterById(int pid)
        {
            using (var rep = this.UnitOfWork.EncounterRepository)
            {
                var encounter = rep.Where(e => e.PatientID == pid && e.EncounterEndTime == null).FirstOrDefault();
                return encounter != null ? encounter.EncounterID : 0;
            }
        }

        public int GetBillHeaderIdByEncounterAndPatientId(int encounterId, int patientId)
        {
            using (var rep = UnitOfWork.BillHeaderRepository)
            {
                var current =
                    rep.Where(a => a.EncounterID == encounterId && a.PatientID == patientId).FirstOrDefault();
                return current != null ? Convert.ToInt32(current.BillHeaderID) : 0;
            }
        }

        public string GetPersonMotherNameById(int patientId)
        {
            using (var rep = UnitOfWork.PatientInfoRepository)
            {
                var patient = rep.Where(p => p.PatientID == patientId).FirstOrDefault();
                return patient != null
                           ? string.Format("{0}", patient.PersonMotherName)
                           : string.Empty;
            }
        }

        /// <summary>
        /// Gets the patient email address.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public string GetPatientEmailAddress(int patientId)
        {
            using (var patientLoginDetails = new PatientLoginDetailBal())
            {
                var patientEmailaddress = patientLoginDetails.GetPatientEmail(patientId);
                return patientEmailaddress;
            }
        }

        /// <summary>
        /// Gets the patient phone number by identifier.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public string GetPatientPhoneNumberById(int patientId)
        {
            using (var patientPhoneRep = UnitOfWork.PatientPhoneRepository)
            {
                var phoneType = Convert.ToInt32(PhoneType.MobilePhone);
                var patientPhoneModel = patientPhoneRep.Where(x => x.PatientID == patientId && x.IsDeleted == false && x.PhoneType == phoneType).FirstOrDefault();
                return patientPhoneModel.PhoneNo;
            }
        }

        /// <summary>
        /// Gets External Value1 value by identifier.
        /// </summary>
        /// <param name="categoryValue">The category value.</param>
        /// <returns></returns>
        public string GetExternalValue1ById(string categoryValue)
        {
            if (!string.IsNullOrEmpty(categoryValue))
            {
                using (var rep = UnitOfWork.GlobalCodeCategoryRepository)
                {
                    var category = rep.Where(g => g.GlobalCodeCategoryValue.Equals(categoryValue)).FirstOrDefault();
                    return category != null ? category.ExternalValue1 : string.Empty;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the payment status string.
        /// </summary>
        /// <param name="paystatus">
        /// The paystatus.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetPaymentStatusString(int? paystatus)
        {
            var stringToreturn = string.Empty;
            switch (paystatus)
            {
                case 20:
                    stringToreturn = "Denial at Activity Level";
                    break;
                case 50:
                    stringToreturn = "Partial Paid";
                    break;
                case 100:
                    stringToreturn = "Fully Paid";
                    break;
                case 900:
                    stringToreturn = "Claim Level Denied";
                    break;
            }

            return stringToreturn;
        }

        /// <summary>
        /// Gets the encounter type by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public string GetEncounterTypeById(int id)
        {
            using (var rep = this.UnitOfWork.EncounterRepository)
            {
                var encounter = rep.Where(e => e.EncounterID == id).FirstOrDefault();
                return encounter != null
                           ? this.GetNameByGlobalCodeValue(encounter.EncounterPatientType.ToString(), "1107")
                           : "NA";
            }
        }

        /// <summary>
        /// Gets the global codes by categories sp.
        /// </summary>
        /// <param name="gccValues">The GCC values.</param>
        /// <returns></returns>
        public List<GlobalCodes> GetGlobalCodesByCategoriesSp(string gccValues)
        {
            using (var gccRep = this.UnitOfWork.GlobalCodeRepository)
            {
                var listToreturn = gccRep.GetGlobalCodesByCategoriesSp(gccValues);
                return listToreturn;
            }
        }

        /// <summary>
        /// Gets the ordering codes sp.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="categoryid">The categoryid.</param>
        /// <param name="subcategoryId">The subcategory identifier.</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public List<GeneralCodesCustomModel> GetOrderingCodesSP(string text, int categoryid, int subcategoryId, int corporateId, int facilityId)
        {
            var finalList = new List<GeneralCodesCustomModel>();
            using (var gccRep = this.UnitOfWork.GlobalCodeRepository)
            {
                finalList = gccRep.GetOrderingCodesSP(text, categoryid, subcategoryId, corporateId, facilityId);
            }
            return finalList;
        }


        protected static Dictionary<TKey, TValue> ToDictionary<TSource, TKey, TValue>(IEnumerable<TSource> items, Converter<TSource, TKey> keySelector, Converter<TSource, TValue> valueSelector)
        {
            return items.ToDictionary(item => keySelector(item), item => valueSelector(item));
        }

        ///// <summary>
        ///// Gets the patient details by patient identifier.
        ///// </summary>
        ///// <param name="patientId">The patient identifier.</param>
        ///// <returns></returns>
        //public PatientInfoCustomModel GetPatientDetailsByPatientId(int patientId)
        //{
        //    var pInfoCModel = new PatientInfoCustomModel();

        //    //Get the patient details from the table PatientInfo
        //    using (var pBal = UnitOfWork.PatientInfoRepository)
        //    {
        //        var pInfo = pBal.Where(p => p.PatientID == patientId).FirstOrDefault();
        //        pInfoCModel.PatientInfo = pInfo;
        //        pInfoCModel.PatientName = string.Format("{0} {1}", pInfo.PersonFirstName, pInfo.PersonLastName);
        //        //pInfoCModel.PersonAge =  GetAgeByDate(Convert.ToDateTime(pInfo.PersonBirthDate));

        //        var fac = GetFacilityByFacilityId(Convert.ToInt32(pInfo.FacilityId));
        //        if (fac != null)
        //        {
        //            pInfoCModel.FacilityName = fac.FacilityName;
        //            pInfoCModel.CorporateId = Convert.ToInt32(fac.CorporateID);
        //            pInfoCModel.CorporateName = GetNameByCorporateId(Convert.ToInt32(fac.CorporateID));
        //        }
        //    }

        //    using (var docBal = new DocumentsTemplatesBal())
        //    {
        //        var profileImage = docBal.GetDocumentByTypeAndPatientId(
        //            Convert.ToInt32(AttachmentType.ProfilePicture),
        //            patientId);
        //        if (profileImage != null)
        //        {
        //            pInfoCModel.ProfilePicImagePath = profileImage.FilePath;
        //            pInfoCModel.DocumentTemplateId = profileImage.DocumentsTemplatesID;
        //        }
        //        else
        //        {
        //            pInfoCModel.ProfilePicImagePath = "/images/BlankProfilePic.png";
        //        }
        //    }

        //    //Get the encounter details of current patient from the table Encounter.
        //    using (var bal = UnitOfWork.EncounterRepository)
        //    {
        //        var encounter =
        //            bal.GetAll()
        //                .OrderByDescending(e => e.EncounterInpatientAdmitDate)
        //                .FirstOrDefault(e => e.PatientID == patientId);
        //        if (encounter != null)
        //        {
        //            if (encounter.EncounterPatientType != null && (int)encounter.EncounterPatientType == 2)
        //            {
        //                using (var mRep = UnitOfWork.MappingPatientBedRepository)
        //                {
        //                    var encounterCustomModel = mRep.GetPatientBedInformation(patientId)
        //                                               ?? new EncounterCustomModel();

        //                    pInfoCModel.patientBedService = encounterCustomModel.patientBedService;
        //                    pInfoCModel.Room = encounterCustomModel.Room;
        //                    pInfoCModel.DepartmentName = encounterCustomModel.DepartmentName;
        //                    pInfoCModel.BedAssignedOn = encounterCustomModel.BedAssignedOn;
        //                    pInfoCModel.BedRateApplicable = encounterCustomModel.BedRateApplicable;
        //                    pInfoCModel.FloorName = encounterCustomModel.FloorName;
        //                    pInfoCModel.BedName = encounterCustomModel.BedName;
        //                }
        //            }
        //            pInfoCModel.CurrentEncounter = encounter;
        //            if (encounter.EncounterAttendingPhysician != null
        //                && Convert.ToInt32(encounter.EncounterAttendingPhysician) > 0)
        //            {
        //                var physician = GetPhysicianById(Convert.ToInt32(encounter.EncounterAttendingPhysician));
        //                pInfoCModel.PhysicianId = physician.Id;
        //                pInfoCModel.PrimaryPhysician = physician.PhysicianName;
        //            }
        //            var orderactivityBal = new OrderActivityBal();
        //            var orderActivitylist = orderactivityBal.GetOrderActivitiesByEncounterId(encounter.EncounterID);
        //            var isOrderPending =
        //                orderActivitylist.Any(
        //                    x =>
        //                    (Convert.ToDateTime(x.OrderScheduleDate) <= DateTime.Now)
        //                    && (Convert.ToInt32(x.OrderActivityStatus) == Convert.ToInt32(OpenOrderActivityStatus.Open)));
        //            pInfoCModel.OrdersPendingToAdminister = isOrderPending;
        //        }

        //        return pInfoCModel;
        //    }
        //}

        //public PatientInfoCustomModel GetPatientDetailsByPatientId(int patientId, int encounterId)
        //{
        //    var pInfoCModel = new PatientInfoCustomModel();

        //    //Get the patient details from the table PatientInfo
        //    using (var pBal = UnitOfWork.PatientInfoRepository)
        //    {
        //        var pInfo = pBal.Where(p => p.PatientID == patientId).FirstOrDefault();
        //        pInfoCModel.PatientInfo = pInfo;
        //        pInfoCModel.PatientName = string.Format("{0} {1}", pInfo.PersonFirstName, pInfo.PersonLastName);
        //        var fac = GetFacilityByFacilityId(Convert.ToInt32(pInfo.FacilityId));
        //        if (fac != null)
        //        {
        //            pInfoCModel.FacilityName = fac.FacilityName;
        //            pInfoCModel.CorporateId = Convert.ToInt32(fac.CorporateID);
        //            pInfoCModel.CorporateName = GetNameByCorporateId(Convert.ToInt32(fac.CorporateID));
        //        }
        //    }

        //    using (var docBal = new DocumentsTemplatesBal())
        //    {
        //        var profileImage = docBal.GetDocumentByTypeAndPatientId(
        //            Convert.ToInt32(AttachmentType.ProfilePicture),
        //            patientId);
        //        if (profileImage != null)
        //        {
        //            pInfoCModel.ProfilePicImagePath = profileImage.FilePath;
        //            pInfoCModel.DocumentTemplateId = profileImage.DocumentsTemplatesID;
        //        }
        //        else
        //        {
        //            pInfoCModel.ProfilePicImagePath = "/images/BlankProfilePic.png";
        //        }
        //    }

        //    //Get the encounter details of current patient from the table Encounter.
        //    using (var bal = UnitOfWork.EncounterRepository)
        //    {
        //        var encounter = bal.Where(e1 => e1.EncounterID == encounterId).FirstOrDefault();
        //        if (encounter != null)
        //        {
        //            if (encounter.EncounterPatientType != null && (int)encounter.EncounterPatientType == 2)
        //            {
        //                using (var mRep = UnitOfWork.MappingPatientBedRepository)
        //                {
        //                    var encounterCustomModel = mRep.GetPatientBedInformation(patientId)
        //                                               ?? new EncounterCustomModel();

        //                    pInfoCModel.patientBedService = encounterCustomModel.patientBedService;
        //                    pInfoCModel.Room = encounterCustomModel.Room;
        //                    pInfoCModel.DepartmentName = encounterCustomModel.DepartmentName;
        //                    pInfoCModel.BedAssignedOn = encounterCustomModel.BedAssignedOn;
        //                    pInfoCModel.BedRateApplicable = encounterCustomModel.BedRateApplicable;
        //                    pInfoCModel.FloorName = encounterCustomModel.FloorName;
        //                    pInfoCModel.BedName = encounterCustomModel.BedName;
        //                }
        //            }
        //            pInfoCModel.CurrentEncounter = encounter;
        //            if (encounter.EncounterAttendingPhysician != null
        //                && Convert.ToInt32(encounter.EncounterAttendingPhysician) > 0)
        //            {
        //                var physician = GetPhysicianById(Convert.ToInt32(encounter.EncounterAttendingPhysician));
        //                pInfoCModel.PhysicianId = physician.Id;
        //                pInfoCModel.PrimaryPhysician = physician.PhysicianName;
        //            }
        //            var orderactivityBal = new OrderActivityBal();
        //            var orderActivitylist = orderactivityBal.GetOrderActivitiesByEncounterId(encounter.EncounterID);
        //            var isOrderPending =
        //                orderActivitylist.Any(
        //                    x =>
        //                    (Convert.ToDateTime(x.OrderScheduleDate) <= DateTime.Now)
        //                    && (Convert.ToInt32(x.OrderActivityStatus) == Convert.ToInt32(OpenOrderActivityStatus.Open)));
        //            pInfoCModel.OrdersPendingToAdminister = isOrderPending;
        //        }

        //        return pInfoCModel;
        //    }
        //}


        /// <summary>
        /// Gets the patient details by patient identifier.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="encounterId">The Current Encounter ID (optional).</param>
        /// <returns></returns>
        public PatientInfoCustomModel GetPatientDetailsByPatientId(int patientId, int encounterId = 0, bool showEncounters = false)
        {
            using (var rep = UnitOfWork.PatientInfoRepository)
            {
                var vm = rep.GetPatientDetails(patientId, encounterId, showEncounters);
                return vm;
            }
        }


        public async Task<string> GetGlobalCodeNameAsync(string codeValue, string categoryValue)
        {
            if (!string.IsNullOrEmpty(codeValue))
            {
                using (var rep = UnitOfWork.GlobalCodeRepository)
                {
                    var gl = await
                        rep.Where(
                            g => g.GlobalCodeValue.Equals(codeValue) && g.GlobalCodeCategoryValue.Equals(categoryValue))
                            .FirstOrDefaultAsync();
                    return gl != null ? gl.GlobalCodeName : string.Empty;
                }
            }
            return string.Empty;
        }


        public GeneralCodesCustomModel GetSelectedCodeParent1(string orderCode, string codeType, long facilityId, string tableNumber)
        {
            using (var rep = UnitOfWork.GlobalCodeRepository)
            {

                var result = rep.GetSelectedCodeParent(orderCode, codeType, facilityId, tableNumber);
                return result;
            }
        }

    }
}
