using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using AutoMapper;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Repository.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class PatientLoginDetailService : IPatientLoginDetailService
    {
        private readonly IRepository<PatientLoginDetail> _repository;
        private readonly IRepository<PatientInfo> _piRepository;
        private readonly IRepository<Facility> _fRepository;
        private readonly IMapper _mapper;

        public PatientLoginDetailService(IRepository<PatientLoginDetail> repository, IRepository<PatientInfo> piRepository, IRepository<Facility> fRepository, IMapper mapper)
        {
            _repository = repository;
            _piRepository = piRepository;
            _fRepository = fRepository;
            _mapper = mapper;
        }

        #region Public Methods and Operators

        /// <summary>
        /// The get patient login detail by patient id.
        /// </summary>
        /// <param name="patientId">
        /// The patient id.
        /// </param>
        /// <returns>
        /// The <see cref="PatientLoginDetailCustomModel"/>.
        /// </returns>
        public PatientLoginDetailCustomModel GetPatientLoginDetailByPatientId(int patientId)
        {
            var model =
                _repository.Where(m => m.PatientId == patientId && m.IsDeleted != true).FirstOrDefault();
            return model != null
                       ? MapValues(model)
                : new PatientLoginDetailCustomModel();
        }
        private List<PatientLoginDetailCustomModel> MapValues(List<PatientLoginDetail> m)
        {
            var lst = new List<PatientLoginDetailCustomModel>();
            foreach (var model in m)
            {
                var vm = _mapper.Map<PatientLoginDetailCustomModel>(model);
                if (vm != null)
                {
                    var pvm = _piRepository.Where(x => x.PatientID == vm.PatientId && x.IsDeleted != true).FirstOrDefault();
                    var facility = _fRepository.Get(Convert.ToInt32(pvm.FacilityId));
                    vm.Facility = facility.FacilityName;
                    vm.FacilityId = facility.FacilityId;
                    vm.FacilityNumber = facility.FacilityNumber;
                    vm.PatientName = pvm.PersonFirstName + " " + pvm.PersonLastName;
                    vm.FirstTimeUser = string.IsNullOrEmpty(model.Password);
                    vm.CorporateId = facility.CorporateID.HasValue ? facility.CorporateID.Value : 0;
                    vm.BirthDate = pvm.PersonBirthDate;
                    vm.EmriateId = pvm.PersonEmiratesIDNumber;
                }
                lst.Add(vm);
            }
            return lst;
        }
        private PatientLoginDetailCustomModel MapValues(PatientLoginDetail model)
        {
            var vm = _mapper.Map<PatientLoginDetailCustomModel>(model);
            if (vm != null)
            {
                var pvm = _piRepository.Where(x => x.PatientID == vm.PatientId && x.IsDeleted != true).FirstOrDefault();
                var facility = _fRepository.Get(Convert.ToInt32(pvm.FacilityId));
                vm.Facility = facility.FacilityName;
                vm.FacilityId = facility.FacilityId;
                vm.FacilityNumber = facility.FacilityNumber;
                vm.PatientName = pvm.PersonFirstName + " " + pvm.PersonLastName;
                vm.FirstTimeUser = string.IsNullOrEmpty(model.Password);
                vm.CorporateId = facility.CorporateID.HasValue ? facility.CorporateID.Value : 0;
                vm.BirthDate = pvm.PersonBirthDate;
                vm.EmriateId = pvm.PersonEmiratesIDNumber;
            }
            return vm;
        }
        /// <summary>
        /// The get patient login details by email.
        /// </summary>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <returns>
        /// The <see cref="PatientLoginDetailCustomModel"/>.
        /// </returns>
        public PatientLoginDetailCustomModel GetPatientLoginDetailsByEmail(string email)
        {
            var vm = new PatientLoginDetailCustomModel();
            var model =
                _repository.Where(m => m.Email.Equals(email) && (m.IsDeleted == null || m.IsDeleted == false))
                    .FirstOrDefault();
            if (model != null)
            {
                vm = MapValues(model);
            }

            return vm;
        }

        /// <summary>
        /// The save patient login details.
        /// </summary>
        /// <param name="vm">
        /// The vm.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int SavePatientLoginDetails(PatientLoginDetailCustomModel vm)
        {
            var model = _mapper.Map<PatientLoginDetail>(vm);
            if (vm.Id > 0)
            {
                var current = _repository.GetSingle(model.Id);
                model.Password = current.Password;
                model.CreatedBy = current.CreatedBy;
                model.CreatedDate = current.CreatedDate;
                _repository.UpdateEntity(model, model.Id);
            }
            else
            {
                model.Password = string.Empty;
                _repository.Create(model);
            }

            return model.Id;
        }

        /// <summary>
        /// The update patient login failed log.
        /// </summary>
        /// <param name="patientId">
        /// The patient id.
        /// </param>
        /// <param name="failedLoginAttempts">
        /// The failed login attempts.
        /// </param>
        /// <param name="lastInvalidLogin">
        /// The last invalid login.
        /// </param>
        public void UpdatePatientLoginFailedLog(int patientId, int failedLoginAttempts, DateTime lastInvalidLogin)
        {
            var vm = _repository.GetSingle(patientId);
            if (vm != null)
            {
                vm.LastInvalidLogin = lastInvalidLogin;
                vm.FailedLoginAttempts = failedLoginAttempts;
                _repository.UpdateEntity(vm, vm.Id);
            }
        }

        /// <summary>
        /// Updates the patient email identifier.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="emailId">The email identifier.</param>
        public void UpdatePatientEmailId(int patientId, string emailId)
        {
            var sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("@pId", patientId);
            sqlParameters[1] = new SqlParameter("@EmailId", emailId);
            _repository.ExecuteCommand(StoredProcedures.SProc_UpdatePatientEmailId.ToString(), sqlParameters);
        }

        /// <summary>
        /// Gets the patient email.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public string GetPatientEmail(int patientId)
        {
            var m = _repository.Where(x => x.PatientId == patientId && (x.IsDeleted == null || x.IsDeleted == false)).FirstOrDefault();
            return m != null ? m.Email : string.Empty;
        }
        #endregion
    }
}
