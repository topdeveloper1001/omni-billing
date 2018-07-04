using System.Collections.Generic;
using System.Linq;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using System;

using AutoMapper;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class ATCCodesService : IATCCodesService
    {
        private readonly IRepository<ATCCodes> _repository;
        private readonly BillingEntities _context;
        private readonly IMapper _mapper;

        public ATCCodesService(IRepository<ATCCodes> repository, BillingEntities context, IMapper mapper)
        {
            _repository = repository;
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<ATCCodesCustomModel> GetATCCodes(string text = "")
        {
            var list = new List<ATCCodesCustomModel>();

            var lstATCCodes = _repository.Where(a => a.IsActive &&
            (string.IsNullOrEmpty(text) || a.ATCCode.Contains(text) || a.DrugName.Contains(text) || a.Purpose.Contains(text) || a.CodeDescription.Contains(text))
            ).ToList();

            if (lstATCCodes.Count() > 0)
            {
                list = lstATCCodes.Select(x => _mapper.Map<ATCCodesCustomModel>(x)).ToList();

                //list.AddRange(lstATCCodes.Select(item => new ATCCodesCustomModel
                //{
                //    ATCCodeID = item.ATCCodeID,
                //    CodeDescription = item.CodeDescription,
                //    SubcodeDescription = item.SubcodeDescription,
                //    SubCode = item.SubCode,
                //    ATCCode = item.ATCCode,
                //    DrugName = item.DrugName,
                //    Purpose = item.Purpose,
                //    DrugDescription = item.DrugDescription,
                //    CodeEffectiveFrom = item.CodeEffectiveFrom,
                //    CodeEffectiveTill = item.CodeEffectiveTill,
                //    IsActive = item.IsActive,
                //    CodeTableNumber = item.CodeTableNumber
                //}));
            }

            return list;
        }
        /*Updated By Krishna on 30012015*/
        public int DeleteATCCode(ATCCodes model)
        {
            return _repository.Delete(model);
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="ATCCodes"></param>
        /// <returns></returns>
        public int SaveATCCodes(ATCCodes model)
        {
            if (model.ATCCodeID > 0)
            {
                var m = _repository.GetSingle(model.ATCCodeID);
                model.CreatedDate = m.CreatedDate;
                model.CreatedBy = m.CreatedBy;
                _repository.Update(model, model.ATCCodeID);
            }
            else
                _repository.Create(model);
            return model.ATCCodeID;

        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="shared"></param>
        /// <returns></returns>
        public ATCCodes GetATCCodesByID(int? ATCCodesId)
        { 
                var model = _repository.Where(x => x.ATCCodeID == ATCCodesId).FirstOrDefault();

                return model;
             
        }
    }
}
