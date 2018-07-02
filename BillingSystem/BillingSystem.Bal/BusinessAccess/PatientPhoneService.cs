using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;
using BillingSystem.Common.Common;

using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class PatientPhoneService : IPatientPhoneService
    {
        private readonly IRepository<PatientPhone> _repository;
        private readonly IRepository<GlobalCodes> _gRepository;

        public PatientPhoneService(IRepository<PatientPhone> repository, IRepository<GlobalCodes> gRepository)
        {
            _repository = repository;
            _gRepository = gRepository;
        }


        /// <summary>
        /// Get the Patient Phone
        /// </summary>
        public IEnumerable<PatientPhoneCustomModel> GetPatientPhoneList(int patientId)
        {
            var lstPatientPhone = _repository.Where(x => x.PatientID == patientId && !(bool)x.IsDeleted).ToList();
            var patientphoneCategorycode =
                Convert.ToInt32(GlobalCodeCategoryValue.PatientPhoneTypes).ToString();
            var lstglobalcode = _gRepository.Where(x => x.GlobalCodeCategoryValue == patientphoneCategorycode && !(bool)x.IsDeleted).ToList();
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

        /// <summary>
        /// Deletes the patient phone.
        /// </summary>
        /// <param name="model">The patient phone model.</param>
        /// <returns></returns>
        public IEnumerable<PatientPhoneCustomModel> DeletePatientPhone(PatientPhone model)
        {
            _repository.Delete(model);
            var list = GetPatientPhoneList(model.PatientID);
            return list;
        }

        /// <summary>
        /// Adds the patient phone.
        /// </summary>
        /// <param name="model">The patient phone model.</param>
        /// <returns></returns>
        public int SavePatientPhone(PatientPhone model)
        {
            if (_repository.Where(p => p.IsPrimary == true && !p.IsDeleted && p.PatientID == model.PatientID && model.IsPrimary == true).Any())
                return -1;

            if (model.PatientPhoneId > 0)
            {
                var current = _repository.GetSingle(model.PatientPhoneId);
                if (current != null)
                {
                    model.CreatedBy = current.CreatedBy;
                    model.CreatedDate = current.CreatedDate;
                }
                _repository.UpdateEntity(model, model.PatientPhoneId);
            }
            else
                _repository.Create(model);

            return model.PatientPhoneId;
        }

        public PatientPhone GetPatientPhoneById(int patientphoneId)
        {
            var m = _repository.Where(x => x.PatientPhoneId == patientphoneId).FirstOrDefault();
            return m;
        }

        public PatientPhone GetPatientPersonalPhoneByPateintId(int patientId)
        {
            int phoneType = Convert.ToInt32(PhoneType.MobilePhone);
            var patientPhoneModel = _repository.Where(x => x.PatientID == patientId && x.IsDeleted == false && x.PhoneType == phoneType).FirstOrDefault();
            return patientPhoneModel;
        }

        public int GetPhoneId(int patientId)
        {
            if (patientId > 0)
            {
                var phoneId = _repository.Where(p => p.PatientID == patientId).Select(m => m.PatientPhoneId).FirstOrDefault();
                return phoneId;
            }
            return 0;
        }

        public int SavePatientPhoneInfo(int patientId, int phoneType, string phone, int userId, DateTime currentDateTime)
        {
            var result = 0;
            var current =
                _repository.Where(x => x.PatientID == patientId && x.IsDeleted == false && x.PhoneType == phoneType)
                    .FirstOrDefault();
            if (current != null)
            {
                current.PhoneNo = phone;
                _repository.Update(current);
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
                _repository.Create(current);
                result = current.PatientPhoneId;
            }
            return result;
        }
    }
}
