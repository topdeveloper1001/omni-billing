using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

using AutoMapper;
using System.Data.SqlClient;
using BillingSystem.Common.Common;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class BedChargesService : IBedChargesService
    {
        private readonly IRepository<BedCharges> _repository;
        private readonly BillingEntities _context;
        private readonly IMapper _mapper;

        public BedChargesService(IRepository<BedCharges> repository, BillingEntities context, IMapper mapper)
        {
            _repository = repository;
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<BedChargesCustomModel> GetBedChargesList()
        {
            var list = new List<BedChargesCustomModel>();
            var lstBedCharges = _repository.Where(a => a.BCIsActive == null || (bool)a.BCIsActive).ToList();
            if (lstBedCharges.Count > 0)
            {
                list.AddRange(lstBedCharges.Select(item => new BedChargesCustomModel
                {

                }));
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
            if (model.BedChargesID > 0)
            {
                var current = _repository.GetSingle(model.BedChargesID);
                model.BCCreatedBy = current.BCCreatedBy;
                model.BCCreatedDate = current.BCCreatedDate;
                _repository.UpdateEntity(model, model.BedChargesID);
            }
            else
                _repository.Create(model);

            var currentId = model.BedChargesID;
            var list = GetBedChargesList();
            return list;
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
                for (int index = 0; index < model.Count(); index++)
                {
                    var openOrderactivity = model[index];
                    try
                    {
                        _repository.Create(openOrderactivity);
                    }
                    catch
                    {
                        break;
                    }
                    result[index] = Convert.ToInt32(openOrderactivity.BCEncounterID);
                }
                status = true;

                if (status)
                {
                    var sqlParameters = new SqlParameter[1];
                    sqlParameters[0] = new SqlParameter("pEncounuterID", result[0]);
                    _repository.ExecuteCommand(StoredProcedures.SPROC_ApplyBedChargesToBill.ToString(), sqlParameters);
                    //var appliedCharges = _repository.ApplyBedCharges(result[0]);
                    status = true;// appliedCharges;
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
            var model = _repository.Where(x => x.BedChargesID == BedChargesId).FirstOrDefault();
            return model;
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
            var model = _repository.Where(x => x.BCEncounterID == encounterid).ToList();
            var isExist = model.Any(x => x.BCTransactionDate.Value.ToShortDateString() == datestart.Value.ToShortDateString());
            return isExist;
        }

        /// <summary>
        /// Deletes the rule master.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public int DeleteBedCharges(BedCharges model)
        {
            return _repository.Delete(model);
        }
    }
}
