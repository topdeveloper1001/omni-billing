using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Bal.Mapper;
using BillingSystem.Repository.UOW;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.BusinessAccess
{
    public class DrugAllergyLogBal : BaseBal
    {
         /// <summary>
        /// Gets or sets the dashboard indicators mapper.
        /// </summary>
        /// <value>
        /// The dashboard indicators mapper.
        /// </value>
        private DrugAllergyLogMapper DrugIntractionsMapper { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardIndicatorsService" /> class.
        /// </summary>
        public DrugAllergyLogBal()
        {
            DrugIntractionsMapper = new DrugAllergyLogMapper();
        }

        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<DrugAllergyLogCustomModel> GetDrugAllergyLogList(int corporateId, int facilityid)
        {
            var list = new List<DrugAllergyLogCustomModel>();
            using (var drugAllergyLogRep = UnitOfWork.DrugAllergyLogRepository)
            {
                var lstDrugAllergyLog = drugAllergyLogRep.Where(a => a.CorporateId == corporateId && a.FacilityId == facilityid).ToList();
                if (lstDrugAllergyLog.Count > 0)
                {
                    if (lstDrugAllergyLog.Any())
                    {
                        list.AddRange(
                            lstDrugAllergyLog.Select(item => DrugIntractionsMapper.MapModelToViewModel(item)));
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public List<DrugAllergyLogCustomModel> SaveDrugAllergyLog(DrugAllergyLog model)
        {
            using (var rep = UnitOfWork.DrugAllergyLogRepository)
            {
                if (model.Id > 0)
                {
                    var current = rep.GetSingle(model.Id);
                    model.CreatedBy = current.CreatedBy;
                    model.CreatedDate = current.CreatedDate;
                    rep.UpdateEntity(model, model.Id);
                }
                else
                    rep.Create(model);

                var list = GetDrugAllergyLogList(Convert.ToInt32(model.CorporateId),Convert.ToInt32(model.FacilityId));
                return list;
            }
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public DrugAllergyLog GetDrugAllergyLogById(int? id)
        {
            using (var rep = UnitOfWork.DrugAllergyLogRepository)
            {
                var model = rep.Where(x => x.Id == id).FirstOrDefault();
                return model;
            }
        }

        /// <summary>
        /// Saves the drug allergy log custom.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public bool SaveDrugAllergyLogCustom(DrugAllergyLog model)
        {
            try
            {
                using (var rep = UnitOfWork.DrugAllergyLogRepository)
                {
                    if (model.Id > 0)
                    {
                        var current = rep.GetSingle(model.Id);
                        model.CreatedBy = current.CreatedBy;
                        model.CreatedDate = current.CreatedDate;
                        rep.UpdateEntity(model, model.Id);
                    }
                    else
                        rep.Create(model);


                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
