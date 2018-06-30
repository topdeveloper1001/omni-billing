using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AutoMapper;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Repository.Common;
using BillingSystem.Repository.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class PatientEvaluationService : IPatientEvaluationService
    {
        private readonly IRepository<PatientEvaluation> _repository;
        private readonly IRepository<PatientEvaluationSet> _prepository;
        private readonly IRepository<GlobalCodes> _gRepository;
        private readonly IRepository<GlobalCodeCategory> _ggRepository;
        private readonly IRepository<Users> _uRepository;
        private readonly IMapper _mapper;
        private readonly BillingEntities _context;

        public PatientEvaluationService(IRepository<PatientEvaluation> repository, IRepository<PatientEvaluationSet> prepository, IRepository<GlobalCodes> gRepository, IRepository<GlobalCodeCategory> ggRepository, IRepository<Users> uRepository, IMapper mapper, BillingEntities context)
        {
            _repository = repository;
            _prepository = prepository;
            _gRepository = gRepository;
            _ggRepository = ggRepository;
            _uRepository = uRepository;
            _mapper = mapper;
            _context = context;
        }

        public List<PatientEvaluationCustomModel> ListPatientEvaluation(Int32 patientId, Int32 encounterId)
        {
            var list = new List<PatientEvaluationCustomModel>();

            var patientEvaluationList = _repository.Where(i => i.PatientId == patientId && i.EncounterId == encounterId && !string.IsNullOrEmpty(i.Value) && i.Value != "0").ToList();
            if (patientEvaluationList.Any())
                list.AddRange(MapValues(patientEvaluationList));
            return list;
        }
        private List<PatientEvaluationCustomModel> MapValues(List<PatientEvaluation> m)
        {
            var lst = new List<PatientEvaluationCustomModel>();
            foreach (var model in m)
            {
                var vm = _mapper.Map<PatientEvaluationCustomModel>(model);
                if (vm != null)
                {
                    vm.GlobalCodeName = GetNameByGlobalCodeValue(model.CodeValue, model.CategoryValue);
                    vm.SubSection = !string.IsNullOrEmpty(vm.ParentCodeValue)
                        ? GetNameByGlobalCodeValue(vm.ParentCodeValue, model.CategoryValue)
                        : string.Empty;

                    vm.GlobalCodeCategoryName = GetGlobalCategoryNameById(model.CategoryValue);
                    vm.Value = model.Value.Equals("1") ? "Yes" : model.Value;
                    vm.EnteredBy = GetUserNameByUserId(vm.CreatedBy);
                }
                lst.Add(vm);
            }
            return lst;
        }
        private string GetUserNameByUserId(int? UserID)
        {
            var usersModel = _uRepository.Where(x => x.UserID == UserID && x.IsDeleted == false).FirstOrDefault();
            return usersModel != null ? usersModel.UserName : string.Empty;
        }
        private string GetGlobalCategoryNameById(string categoryValue, string facilityId = "")
        {
            if (!string.IsNullOrEmpty(categoryValue))
            {
                var category = _ggRepository.Where(g => g.GlobalCodeCategoryValue.Equals(categoryValue) && g.IsDeleted.HasValue ? !g.IsDeleted.Value : false && (string.IsNullOrEmpty(facilityId) || g.FacilityNumber.Equals(facilityId))).FirstOrDefault();
                return category != null ? category.GlobalCodeCategoryName : string.Empty;
            }
            return string.Empty;
        }
        private string GetNameByGlobalCodeValue(string codeValue, string categoryValue, string fId = "")
        {
            if (!string.IsNullOrEmpty(codeValue))
            {
                var gl = _gRepository.Where(g => g.GlobalCodeValue.Equals(codeValue) && !g.IsDeleted.Value && g.GlobalCodeCategoryValue.Equals(categoryValue) && (string.IsNullOrEmpty(fId) || g.FacilityNumber.Equals(fId))).FirstOrDefault();
                return gl != null ? gl.GlobalCodeName : string.Empty;
            }
            return string.Empty;
        }
         
        public int SaveEvaluationSet(PatientEvaluationSet model)
        {
            try
            {
                if (model.SetId > 0)
                {
                    var current = _prepository.Where(f => f.SetId == model.SetId).FirstOrDefault();
                    if (current != null)
                    {
                        model.UpdateBy = current.CreatedBy;
                        model.UpdateDate = current.CreatedDate;
                        model.CreatedBy = current.CreatedBy;
                        model.CreatedDate = current.CreatedDate;
                    }

                    _prepository.UpdateEntity(model, model.SetId);
                }
                else
                    _prepository.Create(model);

            }
            catch (Exception)
            {

            }
            return model.SetId;
        }




        public string GetSignaturePath(int ecounterId, int patinetId, string setId)
        {

            var ipath = _repository.Where(x => x.EncounterId == ecounterId && x.PatientId == patinetId && x.ExternalValue2.Equals(setId)).FirstOrDefault();
            return ipath != null ? ipath.ExternalValue3 : string.Empty;
        }

        public int GetCreatedByFromEvaluationSet(int setId, int patientId)
        {
            var createdBy = _prepository.Where(x => x.PatientId == patientId && x.SetId == setId).FirstOrDefault();
            return createdBy != null ? Convert.ToInt32(createdBy.CreatedBy) : 0;
        }

        public ResponseData SavePatientEvaluationData(List<string> data, long patientId, long eId, long cId, long fId, long userId, long setId, string eStatus, string imagePath)
        {
                var dt = ExtensionMethods.CreateCommonDatatable();

                foreach (var item in data)
                    dt.Rows.Add(new object[] { 0, item.Split('-')[2], item.Split('-')[1], item.Split('-')[3]
                        , item.Split('-')[0], eStatus, imagePath, string.Empty });

            var sqlParameters = new SqlParameter[7];
            sqlParameters[0] = new SqlParameter
            {
                ParameterName = "pDataArray",
                SqlDbType = SqlDbType.Structured,
                Value = data,
                TypeName = "ValuesArrayT"
            };
            sqlParameters[1] = new SqlParameter("pPatientId", patientId);
            sqlParameters[2] = new SqlParameter("pEncounterId", eId);
            sqlParameters[3] = new SqlParameter("pCId", cId);
            sqlParameters[4] = new SqlParameter("pFId", fId);
            sqlParameters[5] = new SqlParameter("pUserId", userId);
            sqlParameters[6] = new SqlParameter("pSetId", setId);

            using (var ms = _context.MultiResultSetSqlQuery(StoredProcedures.SprocSavePatientEvaluation.ToString(), parameters: sqlParameters, isCompiled: false))
            {
                var result = ms.SingleResultSetFor<ResponseData>();
                return result;
            }
         }

    }
}
