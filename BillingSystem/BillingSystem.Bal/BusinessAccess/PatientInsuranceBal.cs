using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.BusinessAccess
{
    public class PatientInsuranceBal : BaseBal
    {

        /// <summary>
        /// Get the Patient Insurance
        /// </summary>
        /// <returns>Return the PatientInsurance </returns>
        public PatientInsuranceCustomModel GetPatientInsurance(int patientId)
        {
            var customModel = new PatientInsuranceCustomModel();
            try
            {
                using (var rep = UnitOfWork.PatientInsuranceRepository)
                {
                    var list = rep.Where(x => x.PatientID == patientId && !x.IsDeleted).OrderByDescending(m => m.CreatedDate).ToList();
                    if (list.Count > 0)
                    {
                        customModel = new PatientInsuranceCustomModel
                        {
                            InsuranceCompanyId = list[0].InsuranceCompanyId,
                            CompanyName = GetInsuranceCompanyNameById(list[0].InsuranceCompanyId),
                            InsurancePlanId = list[0].InsurancePlanId,
                            InsurancePolicyId = list[0].InsurancePolicyId,
                            PatientID = patientId,
                            Expirydate = list[0].Expirydate,
                            Startdate = list[0].Startdate,
                            PatientInsuraceID = list[0].PatientInsuraceID,
                            PlanName = GetInsurancePlanNameById(list[0].InsurancePlanId),
                            PolicyName = GetInsurancePolicyNameById(list[0].InsurancePolicyId),
                            PersonHealthCareNumber = list[0].PersonHealthCareNumber,
                        };

                        if (list.Count == 2)
                        {
                            customModel.PatientInsuranceId2 = list[1].PatientInsuraceID;
                            customModel.CompanyId2 = list[1].InsuranceCompanyId;
                            customModel.Plan2 = list[1].InsurancePlanId;
                            customModel.Policy2 = list[1].InsurancePolicyId;
                            customModel.PersonHealthCareNumber2 = list[1].PersonHealthCareNumber;
                            customModel.StartDate2 = list[1].Startdate;
                            customModel.EndDate2 = list[1].Expirydate;
                        }
                        else
                        {
                            customModel.StartDate2 =DateTime.Now;
                            customModel.EndDate2 = DateTime.Now;
                        }
                    }
                }
                return customModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get the Patient Insurance
        /// </summary>
        /// <returns>Return the PatientInsurance </returns>
        public bool IsInsuranceComapnyInUse(int InsuranceComapnyid)
        {
            try
            {
                using (var patientInsruanceRep = UnitOfWork.PatientInsuranceRepository)
                {
                    var patientInsurance = patientInsruanceRep.GetAll().Any(x => x.InsuranceCompanyId == InsuranceComapnyid && !(bool)x.IsDeleted);
                    return patientInsurance;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public PatientInsuranceCustomModel AddUpdatePatientInsurance(PatientInsuranceCustomModel vm)
        //{
        //    using (var rep = UnitOfWork.PatientInsuranceRepository)
        //    {
        //        if (vm != null)
        //        {
        //            PatientInsurance model;
        //            if (vm.PatientInsuraceID > 0)
        //            {
        //                model = rep.GetSingle(vm.PatientInsuraceID);
        //                model.PersonHealthCareNumber = vm.PersonHealthCareNumber;
        //                model.InsuranceCompanyId = vm.InsuranceCompanyId;
        //                model.InsurancePlanId = vm.InsurancePlanId;
        //                model.InsurancePolicyId = vm.InsurancePolicyId;
        //                model.Startdate = vm.Startdate;
        //                model.Expirydate = vm.Expirydate;
        //                model.IsActive = true;
        //                model.IsDeleted = false;
        //                model.ModifiedBy = vm.ModifiedBy;
        //                model.ModifiedDate = vm.ModifiedDate;
        //                model.PatientID = vm.PatientID;
        //            }
        //            else
        //            {
        //                model = new PatientInsurance
        //                {
        //                    PersonHealthCareNumber = vm.PersonHealthCareNumber,
        //                    InsuranceCompanyId = vm.InsuranceCompanyId,
        //                    InsurancePlanId = vm.InsurancePlanId,
        //                    InsurancePolicyId = vm.InsurancePolicyId,
        //                    Startdate = vm.Startdate,
        //                    Expirydate = vm.Expirydate,
        //                    IsActive = true,
        //                    IsDeleted = false,
        //                    PatientID = vm.PatientID,
        //                    CreatedBy = vm.CreatedBy,
        //                    CreatedDate = vm.CreatedDate,
        //                };
        //            }
        //            SaveInsuranceDetails(model);

        //            //In case Patient has 2nd Health Care 
        //            if (vm.CompanyId2 > 0 || vm.PersonHealthCareNumber2 > 0)
        //            {
        //                if (vm.PatientInsuranceId2 > 0)
        //                {
        //                    model = rep.GetSingle(vm.PatientInsuranceId2);
        //                    model.PersonHealthCareNumber = vm.PersonHealthCareNumber2;
        //                    model.InsuranceCompanyId = vm.CompanyId2;
        //                    model.InsurancePlanId = vm.Plan2;
        //                    model.InsurancePolicyId = vm.Policy2;
        //                    model.Startdate = vm.StartDate2;
        //                    model.Expirydate = vm.EndDate2;
        //                    model.IsActive = true;
        //                    model.IsDeleted = false;
        //                    model.ModifiedBy = vm.ModifiedBy;
        //                    model.ModifiedDate = vm.ModifiedDate;
        //                    model.PatientID = vm.PatientID;
        //                }
        //                else
        //                {
        //                    model = new PatientInsurance
        //                    {
        //                        PersonHealthCareNumber = vm.PersonHealthCareNumber2,
        //                        InsuranceCompanyId = vm.CompanyId2,
        //                        InsurancePlanId = vm.Plan2,
        //                        InsurancePolicyId = vm.Policy2,
        //                        Startdate = vm.StartDate2,
        //                        Expirydate = vm.EndDate2,
        //                        IsActive = true,
        //                        IsDeleted = false,
        //                        PatientID = vm.PatientID,
        //                        CreatedBy = vm.CreatedBy,
        //                        CreatedDate = vm.CreatedDate,
        //                    };
        //                }
        //                SaveInsuranceDetails(model);
        //            }
        //        }

        //        var updatePatientInsurance = GetPatientInsurance(vm.PatientID);
        //        return updatePatientInsurance;
        //    }
        //}


        /// <summary>
        /// Method to add the PatientInsurance in the database.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<int> SavePatientInsurance(PatientInsurance model)
        {
            using (var rep = UnitOfWork.PatientInsuranceRepository)
            {
                if (model != null)
                {
                    if (model.PatientInsuraceID > 0)
                    {
                        var current = rep.GetSingle(model.PatientInsuraceID);
                        model.CreatedBy = current.CreatedBy;
                        model.CreatedDate = current.CreatedDate;
                        rep.UpdateEntity(model, model.PatientInsuraceID);
                    }
                    else
                        rep.Create(model);

                    var list = rep.Where(p => p.PatientID == model.PatientID).OrderBy(m => m.CreatedDate).Select(m => m.PatientInsuraceID).ToList();
                    return list;
                }
            }
            return new List<int>();
        }

        public PatientInsuranceCustomModel GetPrimaryPatientInsurance(int patientId, bool isPrimary)
        {
            var cm = new PatientInsuranceCustomModel
            {
                IsDeleted = false,
                IsActive = true,
                StartDate2 = DateTime.Now,
                EndDate2 = DateTime.Now
            };

            using (var rep = UnitOfWork.PatientInsuranceRepository)
            {
                var result = isPrimary
                    ? rep.Where(p => p.PatientID == patientId && p.IsPrimary != false).FirstOrDefault()
                    : rep.Where(p => p.PatientID == patientId && p.IsPrimary != true).FirstOrDefault();

                if (result != null)
                {
                    cm = new PatientInsuranceCustomModel
                    {
                        InsuranceCompanyId = result.InsuranceCompanyId,
                        CompanyName = GetInsuranceCompanyNameById(result.InsuranceCompanyId),
                        InsurancePlanId = result.InsurancePlanId,
                        InsurancePolicyId = result.InsurancePolicyId,
                        PatientID = patientId,
                        Expirydate = result.Expirydate,
                        Startdate = result.Startdate,
                        PatientInsuraceID = result.PatientInsuraceID,
                        PlanName = GetInsurancePlanNameById(result.InsurancePlanId),
                        PolicyName = GetInsurancePolicyNameById(result.InsurancePolicyId),
                        PersonHealthCareNumber = result.PersonHealthCareNumber,
                        IsDeleted = result.IsDeleted,
                        IsActive = result.IsActive,
                    };
                }
            }
            return cm;
        }

        public PatientInsuranceCustomModel GetPatientInsuranceView(int patientId)
        {
            var customModel = GetPrimaryPatientInsurance(patientId, true);
            if (customModel != null)
            {
                var vm = GetPrimaryPatientInsurance(patientId, false);
                if (vm != null)
                {
                    customModel.PatientInsuranceId2 = vm.PatientInsuraceID;
                    customModel.CompanyId2 = vm.InsuranceCompanyId;
                    customModel.Plan2 = vm.InsurancePlanId;
                    customModel.Policy2 = vm.InsurancePolicyId;
                    customModel.PersonHealthCareNumber2 = vm.PersonHealthCareNumber;
                    customModel.StartDate2 = vm.Startdate;
                    customModel.EndDate2 = vm.Expirydate;
                    customModel.IsActive = vm.IsActive;
                    customModel.IsDeleted = vm.IsDeleted;
                   
                }
            }
            return customModel;
        }

    }
}
