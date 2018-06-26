using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Repository.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class BillingSystemParametersService : IBillingSystemParametersService
    {
        private readonly IRepository<BillingSystemParameters> _repository;
        private readonly IRepository<BillingCodeTableSet> _bRepository;
        private readonly BillingEntities _context;

        public BillingSystemParametersService(IRepository<BillingSystemParameters> repository, IRepository<BillingCodeTableSet> bRepository, BillingEntities context)
        {
            _repository = repository;
            _bRepository = bRepository;
            _context = context;
        }


        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int SaveBillingSystemParameters(BillingSystemParameters model)
        {
            if (model.Id > 0)
            {
                var oldValues = GetDetailsByBillingParameterId(model.Id);
                var current = _repository.GetSingle(model.Id);
                model.CreatedBy = current.CreatedBy;
                model.CreatedDate = current.CreatedDate;
                _repository.UpdateEntity(model, model.Id);
            }
            else
            {
                try
                {
                    _repository.Create(model);
                }
                catch (DbEntityValidationException dbEx)
                {
                    var raise = dbEx.EntityValidationErrors.Aggregate<DbEntityValidationResult, Exception>(dbEx,
                            (current1, validationErrors) =>
                                validationErrors.ValidationErrors.Select(
                                    validationError =>
                                        string.Format("{0}:{1}", validationErrors.Entry.Entity.ToString(),
                                            validationError.ErrorMessage))
                                    .Aggregate(current1,
                                        (current, message) => new InvalidOperationException(message, current)));
                    throw raise;
                }
            }

            return model.Id;
        }

        /// <summary>
        /// Gets the details by corporate and facility.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityNumber">The facility number.</param>
        /// <returns></returns>
        public BillingSystemParameters GetDetailsByCorporateAndFacility(int corporateId, string facilityNumber)
        {
            var item =
                _repository.Where(b => b.IsActive && b.CorporateId == corporateId && b.FacilityNumber.Equals(facilityNumber))
                    .OrderByDescending(b => b.Id).FirstOrDefault();
            return item ?? new BillingSystemParameters();
        }

        public BillingSystemParameters GetDetailsByBillingParameterId(int billingParameterId)
        {
            var spName = string.Format("EXEC {0} @pBillingSystemParametersId", StoredProcedures.SPROC_GetDetailsByBillingSystemParametersId.ToString());
            var sqlParameters = new SqlParameter[1];
            sqlParameters[0] = new SqlParameter("pBillingSystemParametersId", billingParameterId);
            BillingSystemParameters result = _context.Database.SqlQuery<BillingSystemParameters>(spName, sqlParameters).FirstOrDefault();
            return result ?? new BillingSystemParameters();
        }
        public bool SaveRecordsFortableNumber(string tableNumber, IEnumerable<string> selectedCodeid, string typeid)
        {
            try
            {
                var idslist = selectedCodeid.OfType<string>().ToList();
                var joinedstring = String.Join(",", idslist);

                var sqlParameters = new SqlParameter[3];
                sqlParameters[0] = new SqlParameter("ServiceCodeIds", joinedstring);
                sqlParameters[1] = new SqlParameter("tableNumber", tableNumber);
                sqlParameters[2] = new SqlParameter("type", typeid);
                _repository.ExecuteCommand(StoredProcedures.SPROC_SaveBillingTableDataForTableNumber.ToString(), sqlParameters);
                return true;
                //var savedData = SaveRecordsbyTableNumber(tableNumber, joinedstring, typeid);
                //return savedData == 1;
            }
            catch (Exception)
            {
                return true;
            }
        }
        public int SaveTableNumber(BillingCodeTableSet model)
        {
            var newId = 0;
            _bRepository.Create(model);
            newId = model.Id;

            return newId;
        }

    }
}
