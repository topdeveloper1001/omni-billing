using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Bal.Mapper;
using BillingSystem.Repository.UOW;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.BusinessAccess
{
    public class DrugInstructionAndDosingBal : BaseBal
    {
        /// <summary>
        /// Gets or sets the dashboard indicators mapper.
        /// </summary>
        /// <value>
        /// The dashboard indicators mapper.
        /// </value>
        private DrugInstructionAndDosingMapper DrugInstructionAndDosingMapper { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardIndicatorsBal" /> class.
        /// </summary>
        public DrugInstructionAndDosingBal()
        {
            DrugInstructionAndDosingMapper = new DrugInstructionAndDosingMapper();
        }


        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<DrugInstructionAndDosingCustomModel> GetDrugInstructionAndDosingList()
        {
            var list = new List<DrugInstructionAndDosingCustomModel>();
            using (var drugInstructionAndDosingRep = UnitOfWork.DrugInstructionAndDosingRepository)
            {
                var lstDrugInstructionAndDosing = drugInstructionAndDosingRep.Where(a => a.IsDeleted == null || !(bool)a.IsDeleted).ToList();
                if (lstDrugInstructionAndDosing.Count > 0)
                {
                    if (lstDrugInstructionAndDosing.Any())
                    {
                        list.AddRange(
                            lstDrugInstructionAndDosing.Select(item => DrugInstructionAndDosingMapper.MapModelToViewModel(item)));
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
        public List<DrugInstructionAndDosingCustomModel> SaveDrugInstructionAndDosing(DrugInstructionAndDosing model)
        {
            using (var rep = UnitOfWork.DrugInstructionAndDosingRepository)
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
                var list = GetDrugInstructionAndDosingList();
                return list;
            }
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public DrugInstructionAndDosing GetDrugInstructionAndDosingById(int? id)
        {
            using (var rep = UnitOfWork.DrugInstructionAndDosingRepository)
            {
                var model = rep.Where(x => x.Id == id).FirstOrDefault();
                return model;
            }
        }
    }
}
