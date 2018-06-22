using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BillingSystem.Model;
using BillingSystem.Model.Model;
using BillingSystem.Repository.UOW;

namespace BillingSystem.Bal.BusinessAccess
{
    public class PDFTemplatesBal : BaseBal
    {

        public int SavePDFTemplates(OtherPatientForm oPatientNusre)
        {
            using (var rep = UnitOfWork.PDFTemplatesRepository)
            {
                try
                {
                    rep.Create(oPatientNusre);
                }
                catch (Exception )
                {

                }
            }
            return oPatientNusre.Id;
        }

        public List<OtherPatientForm> ListNurseAssessmentForm(int patientId, int encounterId, string globalCodeCategory, string setId)
        {
            List<OtherPatientForm> list;
            using (var rep = UnitOfWork.PDFTemplatesRepository)
            {
                list =
                    rep.Where(
                        i =>
                            i.PatientId == patientId && i.EncounterId == encounterId &&
                            i.CategoryValue == globalCodeCategory && i.ExternalValue1 == "1" && i.ExternalValue2.Trim().Equals(setId))
                        .ToList();

                if (list.Any())
                    list = list.OrderBy(g => int.Parse(g.CodeValue)).ToList();
            }
            return list.Any() ? list : new List<OtherPatientForm>();
        }

        public string GetNewFormDetailsByFormType()
        {
            Random rnd = new Random();
            var rn = rnd.Next(1, 1000000);
            var formNumber = string.Empty;
            var isExists = true;
            using (var rep = UnitOfWork.PatientEvaluationSetRepository)
            {
                formNumber =
                string.Format("OPT#{0}", rn);
                do
                {
                    isExists = rep.Where(r => r.FormNumber == formNumber).Any();
                }
                while (isExists);
            }
            return formNumber;
        }

        public int UpdateOutPatientAssessment(OtherPatientForm model)
        {
            using (var rep = UnitOfWork.PDFTemplatesRepository)
            {
                rep.UpdateOutPatientAssessment(model);
            }
            return model.Id;
        }


        public string GetSignaturePathNurseForm(int ecounterId, int patinetId, string setId)
        {

            using (var erep = UnitOfWork.PDFTemplatesRepository)
            {
                var ipath =
                      erep.Where(
                          x => x.EncounterId == ecounterId && x.PatientId == patinetId && x.ExternalValue2.Equals(setId))
                          .FirstOrDefault();

                return ipath != null ? ipath.ExternalValue3 : string.Empty;
            }

        }

        public List<OtherPatientForm> GetNursingAssessmentFormData(int patientId, int encounterId, string setId)
        {
            List<OtherPatientForm> list;
            using (var rep = UnitOfWork.PDFTemplatesRepository)
            {
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
                    rep.Where(
                        i =>
                            i.PatientId == patientId && i.EncounterId == encounterId && gcc.Contains(i.CategoryValue) &&
                            i.ExternalValue1 == "1" && i.ExternalValue2.Trim().Equals(setId))
                        .ToList();

                if (list.Any())
                    list = list.OrderBy(g => int.Parse(g.CodeValue)).ToList();
            }
            return list.Any() ? list : new List<OtherPatientForm>();
        }
    }
}
