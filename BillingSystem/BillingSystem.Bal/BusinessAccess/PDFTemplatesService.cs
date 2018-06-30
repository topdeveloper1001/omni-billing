using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using AutoMapper;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.Model;
using BillingSystem.Repository.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class PDFTemplatesService : IPDFTemplatesService
    {

        private readonly IRepository<OtherPatientForm> _repository;
        private readonly IRepository<PatientEvaluationSet> _peRepository;

        public PDFTemplatesService(IRepository<OtherPatientForm> repository, IRepository<PatientEvaluationSet> peRepository)
        {
            _repository = repository;
            _peRepository = peRepository;
        }

        public int SavePDFTemplates(OtherPatientForm oPatientNusre)
        {
            _repository.Create(oPatientNusre);
            return oPatientNusre.Id;
        }

        public List<OtherPatientForm> ListNurseAssessmentForm(int patientId, int encounterId, string globalCodeCategory, string setId)
        {
            var list = _repository.Where(
                    i =>
                        i.PatientId == patientId && i.EncounterId == encounterId &&
                        i.CategoryValue == globalCodeCategory && i.ExternalValue1 == "1" && i.ExternalValue2.Trim().Equals(setId))
                    .ToList();

            if (list.Any())
                list = list.OrderBy(g => int.Parse(g.CodeValue)).ToList();
            return list.Any() ? list : new List<OtherPatientForm>();
        }

        public string GetNewFormDetailsByFormType()
        {
            Random rnd = new Random();
            var rn = rnd.Next(1, 1000000);
            var formNumber = string.Empty;
            var isExists = true;
            formNumber =
            string.Format("OPT#{0}", rn);
            do
            {
                isExists = _peRepository.Where(r => r.FormNumber == formNumber).Any();
            }
            while (isExists);
            return formNumber;
        }

        public int UpdateOutPatientAssessment(OtherPatientForm m)
        {
            var sqlParameters = new SqlParameter[7];

            sqlParameters[0] = new SqlParameter("PatientId", m.PatientId);
            sqlParameters[1] = new SqlParameter("EncounterId", m.EncounterId);
            sqlParameters[2] = new SqlParameter("GlobalCodeCategory", m.CategoryValue);
            sqlParameters[3] = new SqlParameter("GlobalCode", m.CodeValue);
            sqlParameters[4] = new SqlParameter("Value", m.Value);
            sqlParameters[5] = new SqlParameter("ExternalValue1", m.ExternalValue1);
            sqlParameters[6] = m.ExternalValue3 == null
                ? new SqlParameter("ExternalValue3", DBNull.Value)
                : new SqlParameter("ExternalValue3", m.ExternalValue3);
            _repository.ExecuteCommand(StoredProcedures.SPROC_UpdateOutPatientNurseAssessmentForm.ToString(), sqlParameters);

            return m.Id;
        }


        public string GetSignaturePathNurseForm(int ecounterId, int patinetId, string setId)
        {

            var ipath = _repository.Where(x => x.EncounterId == ecounterId && x.PatientId == patinetId && x.ExternalValue2.Equals(setId)).FirstOrDefault();

            return ipath != null ? ipath.ExternalValue3 : string.Empty;

        }

        public List<OtherPatientForm> GetNursingAssessmentFormData(int patientId, int encounterId, string setId)
        {
            List<OtherPatientForm> list;
            var gcc = new List<string>
                {
                    "6100",
                    "6101",
                    "6102",
                    "6103",
                    "6104",
                    "6105",
                    "6106",
                    "6107",
                    "6108",
                    "6109",
                    "6110",
                    "6111",
                    "6112",
                    "6113",
                    //"6114",
                    //"6115",
                    "6116"
                };
            list =
                _repository.Where(
                    i =>
                        i.PatientId == patientId && i.EncounterId == encounterId && gcc.Contains(i.CategoryValue) &&
                        i.ExternalValue1 == "1" && i.ExternalValue2.Trim().Equals(setId))
                    .ToList();

            if (list.Any())
                list = list.OrderBy(g => int.Parse(g.CodeValue)).ToList();
            return list.Any() ? list : new List<OtherPatientForm>();
        }
    }
}
