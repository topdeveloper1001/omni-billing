using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using BillingSystem.Bal.Mapper;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Repository.Common;

namespace BillingSystem.Bal.BusinessAccess
{
    public class PatientEvaluationBal : BaseBal
    {
        private PatientEvaluationMapper PatientEvaluationMapper { get; set; }

        public PatientEvaluationBal()
        {
            PatientEvaluationMapper = new PatientEvaluationMapper();
        }

        public List<PatientEvaluationCustomModel> ListPatientEvaluation(Int32 patientId, Int32 encounterId)
        {
            var list = new List<PatientEvaluationCustomModel>();

            using (var oPatientEvaluationRepository = UnitOfWork.PatientEvaluationRepository)
            {
                var patientEvaluationList =
                    oPatientEvaluationRepository.Where(
                        i =>
                            i.PatientId == patientId && i.EncounterId == encounterId &&
                            !string.IsNullOrEmpty(i.Value) &&
                            i.Value != "0")
                        .ToList();
                if (patientEvaluationList.Any())
                {
                    list.AddRange(
                        patientEvaluationList.Select(item => PatientEvaluationMapper.MapModelToViewModel(item)));
                }
            }

            return list;
        }


        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //public int SaveEvaluationManagement(PatientEvaluation model)
        //{
        //    using (var rep = UnitOfWork.PatientEvaluationRepository)
        //    {
        //        rep.Create(model);
        //    }
        //    return model.Id;
        //}

        //public int UpdateEvaluationManagement(PatientEvaluation model)
        //{
        //    using (var rep = UnitOfWork.PatientEvaluationRepository)
        //    {
        //        rep.UpdateEvaluationManagement(model);
        //    }
        //    return model.Id;
        //}

        public int SaveEvaluationSet(PatientEvaluationSet model)
        {
            using (var rep = UnitOfWork.PatientEvaluationSetRepository)
            {
                try
                {
                    if (model.SetId > 0)
                    {
                        var current = rep.Where(f => f.SetId == model.SetId).FirstOrDefault();
                        if (current != null)
                        {
                            model.UpdateBy = current.CreatedBy;
                            model.UpdateDate = current.CreatedDate;
                            model.CreatedBy = current.CreatedBy;
                            model.CreatedDate = current.CreatedDate;
                        }

                        rep.UpdateEntity(model, model.SetId);
                    }
                    else
                    {
                        rep.Create(model);
                    }

                }
                catch (Exception)
                {

                }
            }
            return model.SetId;
        }




        public string GetSignaturePath(int ecounterId, int patinetId, string setId)
        {

            using (var erep = UnitOfWork.PatientEvaluationRepository)
            {
                var ipath =
                      erep.Where(
                          x => x.EncounterId == ecounterId && x.PatientId == patinetId && x.ExternalValue2.Equals(setId))
                          .FirstOrDefault();

                return ipath != null ? ipath.ExternalValue3 : string.Empty;
            }

        }

        public int GetCreatedByFromEvaluationSet(int setId, int patientId)
        {
            using (var eRep = UnitOfWork.PatientEvaluationSetRepository)
            {
                var createdBy =
                    eRep.Where(x => x.PatientId == patientId && x.SetId == setId)
                        .FirstOrDefault();
                return createdBy != null ? Convert.ToInt32(createdBy.CreatedBy) : 0;
            }
        }

        public ResponseData SavePatientEvaluationData(List<string> data, long patientId, long eId, long cId, long fId, long userId, long setId, string eStatus, string imagePath)
        {
            using (var rep = UnitOfWork.PatientEvaluationRepository)
            {
                var dt = ExtensionMethods.CreateCommonDatatable();

                foreach (var item in data)
                    dt.Rows.Add(new object[] { 0, item.Split('-')[2], item.Split('-')[1], item.Split('-')[3]
                        , item.Split('-')[0], eStatus, imagePath, string.Empty });

                var result = rep.SavePatientEvaluationManagement(dt, patientId, eId, cId, fId, userId, setId);
                return result;
            }
        }

    }
}
