using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Transactions;
using BillingSystem.Repository.GenericRepository;
using BillingSystem.Repository.UOW;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.BusinessAccess
{
    public class BedChargesBal : BaseBal
    {
        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<BedChargesCustomModel> GetBedChargesList()
        {
            var list = new List<BedChargesCustomModel>();
            using (var BedChargesRep = UnitOfWork.BedChargesRepository)
            {
                var lstBedCharges = BedChargesRep.Where(a => a.BCIsActive == null || (bool)a.BCIsActive).ToList();
                if (lstBedCharges.Count > 0)
                {
                    list.AddRange(lstBedCharges.Select(item => new BedChargesCustomModel
                    {

                    }));
                }
            }
            return list;
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="BedCharges"></param>
        /// <returns></returns>
        public List<BedChargesCustomModel> SaveBedCharges(BedCharges model)
        {
            using (var rep = UnitOfWork.BedChargesRepository)
            {
                if (model.BedChargesID > 0)
                {
                    var current = rep.GetSingle(model.BedChargesID);
                    model.BCCreatedBy = current.BCCreatedBy;
                    model.BCCreatedDate = current.BCCreatedDate;
                    rep.UpdateEntity(model, model.BedChargesID);
                }
                else
                    rep.Create(model);

                var currentId = model.BedChargesID;
                var list = GetBedChargesList();
                return list;
            }
        }

        /// <summary>
        /// Adds the uptdate order activity.
        /// </summary>
        /// <param name="model">The order activity.</param>
        /// <returns></returns>
        public bool SaveBedChargesList(List<BedCharges> model)
        {
            bool status;
            try
            {
                var result = new int[model.Count()];
                using (var transScope = new TransactionScope())
                {
                    using (var orderActivityRep = UnitOfWork.BedChargesRepository)
                    {
                        for (int index = 0; index < model.Count(); index++)
                        {
                            var openOrderactivity = model[index];
                            try
                            {
                                orderActivityRep.Create(openOrderactivity);
                            }
                            catch
                            {
                                break;
                            }
                            result[index] = Convert.ToInt32(openOrderactivity.BCEncounterID);
                        }
                        transScope.Complete();
                        status = true;
                    }
                }

                if (status)
                {
                    using (var rep = UnitOfWork.BillHeaderRepository)
                    {
                        //Apply Bed Charges to the current Encounter ID
                        var appliedCharges = rep.ApplyBedCharges(result[0]);
                        status = appliedCharges;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return status;
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="BedChargesId">The bed charges identifier.</param>
        /// <returns></returns>
        public BedCharges GetBedChargesByID(int? BedChargesId)
        {
            using (var rep = UnitOfWork.BedChargesRepository)
            {
                var model = rep.Where(x => x.BedChargesID == BedChargesId).FirstOrDefault();
                return model;
            }
        }

        /// <summary>
        /// Checks the bed charge exist.
        /// </summary>
        /// <param name="encounterid">The encounterid.</param>
        /// <param name="patietid">The patietid.</param>
        /// <param name="datestart">The datestart.</param>
        /// <returns></returns>
        public bool CheckBedChargeExist(int encounterid, int patietid, DateTime? datestart)
        {
            using (var rep = UnitOfWork.BedChargesRepository)
            {
                var model = rep.Where(x => x.BCEncounterID == encounterid).ToList();
                var isExist =
                    model.Any(x => x.BCTransactionDate.Value.ToShortDateString() == datestart.Value.ToShortDateString());
                return isExist;
            }
        }

        /// <summary>
        /// Deletes the rule master.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public int DeleteBedCharges(BedCharges model)
        {
            using (var rep = UnitOfWork.BedChargesRepository)
            {
                return Convert.ToInt32(rep.Delete(model));
            }
        }
    }
}
