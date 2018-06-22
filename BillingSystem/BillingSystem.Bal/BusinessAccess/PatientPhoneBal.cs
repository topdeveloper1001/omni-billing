using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;
using BillingSystem.Common.Common;

namespace BillingSystem.Bal.BusinessAccess
{
    public class PatientPhoneBal : BaseBal
    {
        /// <summary>
        /// Get the Patient Phone
        /// </summary>
        public IEnumerable<PatientPhoneCustomModel> GetPatientPhoneList(int patientId)
        {
            try
            {
                using (var patientPhoneRep = UnitOfWork.PatientPhoneRepository)
                {
                    using (var globalcodeRep = UnitOfWork.GlobalCodeRepository)
                    {
                        var lstPatientPhone = patientPhoneRep.Where(x => x.PatientID == patientId && !(bool)x.IsDeleted).ToList();
                        var patientphoneCategorycode =
                            Convert.ToInt32(GlobalCodeCategoryValue.PatientPhoneTypes).ToString();
                        var lstglobalcode = globalcodeRep.Where(x => x.GlobalCodeCategoryValue == patientphoneCategorycode && !(bool)x.IsDeleted).ToList();
                        var list = (from p in lstPatientPhone
                                    join fac in lstglobalcode
                                    on p.PhoneType equals Convert.ToInt32(fac.GlobalCodeValue) into joinedPrimaryFacility
                                    from fac in joinedPrimaryFacility.DefaultIfEmpty()
                                    select new PatientPhoneCustomModel
                                    {
                                        PatientPhone = p,
                                        PatientPhoneTypeName = fac != null ? fac.GlobalCodeName : string.Empty,
                                    }).ToList();
                        return list;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Deletes the patient phone.
        /// </summary>
        /// <param name="model">The patient phone model.</param>
        /// <returns></returns>
        public IEnumerable<PatientPhoneCustomModel> DeletePatientPhone(PatientPhone model)
        {
            using (var rep = UnitOfWork.PatientPhoneRepository)
            {
                rep.Delete(model);
                var list = GetPatientPhoneList(model.PatientID);
                return list;
            }
        }

        /// <summary>
        /// Adds the patient phone.
        /// </summary>
        /// <param name="model">The patient phone model.</param>
        /// <returns></returns>
        public int SavePatientPhone(PatientPhone model)
        {
            using (var rep = UnitOfWork.PatientPhoneRepository)
            {
                if (rep.Where(p => p.IsPrimary == true && !p.IsDeleted && p.PatientID == model.PatientID && model.IsPrimary == true).Any())
                    return -1;

                if (model.PatientPhoneId > 0)
                {
                    var current = rep.GetSingle(model.PatientPhoneId);
                    if (current != null)
                    {
                        model.CreatedBy = current.CreatedBy;
                        model.CreatedDate = current.CreatedDate;
                    }
                    rep.UpdateEntity(model, model.PatientPhoneId);
                }
                else
                    rep.Create(model);

                return model.PatientPhoneId;
            }
        }

        public PatientPhone GetPatientPhoneById(int patientphoneId)
        {
            using (var patientPhoneRep = UnitOfWork.PatientPhoneRepository)
            {
                var patientPhoneModel = patientPhoneRep.Where(x => x.PatientPhoneId == patientphoneId).FirstOrDefault();
                return patientPhoneModel;
            }
        }

        public PatientPhone GetPatientPersonalPhoneByPateintId(int patientId)
        {
            using (var patientPhoneRep = UnitOfWork.PatientPhoneRepository)
            {
                int phoneType = Convert.ToInt32(PhoneType.MobilePhone);
                var patientPhoneModel = patientPhoneRep.Where(x => x.PatientID == patientId && x.IsDeleted == false && x.PhoneType == phoneType).FirstOrDefault();
                return patientPhoneModel;
            }
        }

        public int GetPhoneId(int patientId)
        {
            if (patientId > 0)
            {
                using (var rep = UnitOfWork.PatientPhoneRepository)
                {
                    var phoneId = rep.Where(p => p.PatientID == patientId).Select(m => m.PatientPhoneId).FirstOrDefault();
                    return phoneId;
                }
            }
            return 0;
        }

        public int SavePatientPhoneInfo(int patientId, int phoneType, string phone, int userId, DateTime currentDateTime)
        {
            var result = 0;
            using (var rep = UnitOfWork.PatientPhoneRepository)
            {
                var current =
                    rep.Where(x => x.PatientID == patientId && x.IsDeleted == false && x.PhoneType == phoneType)
                        .FirstOrDefault();
                if (current != null)
                {
                    current.PhoneNo = phone;
                    rep.Update(current);
                    result = current.PatientPhoneId;
                }
                else
                {
                    current = new PatientPhone
                    {
                        CreatedBy = userId,
                        CreatedDate = currentDateTime,
                        PatientID = patientId,
                        IsPrimary = true,
                        PhoneType = phoneType,
                        IsDeleted = false
                    };
                    rep.Create(current);
                    result = current.PatientPhoneId;
                }
            }
            return result;
        }
    }
}
