using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Bal.Mapper;
using BillingSystem.Repository.UOW;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.BusinessAccess
{
    public class DrugInteractionsBal : BaseBal
    {
        /// <summary>
        /// Gets or sets the dashboard indicators mapper.
        /// </summary>
        /// <value>
        /// The dashboard indicators mapper.
        /// </value>
        private DrugIntractionsMapper DrugIntractionsMapper { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardIndicatorsBal" /> class.
        /// </summary>
        public DrugInteractionsBal()
        {
            DrugIntractionsMapper = new DrugIntractionsMapper();
        }

        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>
        /// Return the Entity List
        /// </returns>
        public List<DrugInteractionsCustomModel> GetDrugInteractionsList()
        {
            var list = new List<DrugInteractionsCustomModel>();
            using (var drugInteractionsRep = UnitOfWork.DrugInteractionsRepository)
            {
                var lstDrugInteractions = drugInteractionsRep.Where(a => !a.IsDeleted).ToList();
                if (lstDrugInteractions.Count > 0)
                {
                    if (lstDrugInteractions.Any())
                    {
                        list.AddRange(
                            lstDrugInteractions.Select(item => DrugIntractionsMapper.MapModelToViewModel(item)));
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
        public List<DrugInteractionsCustomModel> SaveDrugInteractions(DrugInteractions model)
        {
            using (var rep = UnitOfWork.DrugInteractionsRepository)
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

                var currentId = model.Id;
                var list = GetDrugInteractionsList();
                return list;
            }
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public DrugInteractions GetDrugInteractionsById(int? id)
        {
            using (var rep = UnitOfWork.DrugInteractionsRepository)
            {
                var model = rep.Where(x => x.Id == id).FirstOrDefault();
                return model;
            }
        }
    }
}
