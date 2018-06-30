using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Repository.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class DenialService : IDenialService
    {
        private readonly IRepository<Denial> _repository;
        private readonly IRepository<GlobalCodes> _gRepository;
        private readonly BillingEntities _context;
        private readonly IMapper _mapper;

        public DenialService(IRepository<Denial> repository, IRepository<GlobalCodes> gRepository, BillingEntities context, IMapper mapper)
        {
            _repository = repository;
            _gRepository = gRepository;
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Get the Service Code
        /// </summary>
        /// <returns>Return the ServiceCode View Model</returns>
        public List<DenialCodeCustomModel> GetDenial()
        {
            var lst = _repository.Where(x => x.IsDeleted == false).ToList();
            return lst.Select(x => _mapper.Map<DenialCodeCustomModel>(x)).ToList();
        }

        /// <summary>
        /// Method to add update the Denial in the database.
        /// </summary>
        /// <param name="ServiceCode"></param>
        /// <returns></returns>
        public int AddUpdateDenial(Denial Denial)
        {
            if (Denial.DenialSetNumber > 0)
                _repository.UpdateEntity(Denial, Denial.DenialSetNumber);
            else
                _repository.Create(Denial);
            return Denial.DenialSetNumber;
        }

        /// <summary>
        /// Method to add the Service Code in the database.
        /// </summary>
        /// <param name="shared"></param>
        /// <returns></returns>
        public Denial GetDenialById(int id)
        {
            var Denial = _repository.GetAll().Where(x => x.DenialSetNumber == id).FirstOrDefault();
            return Denial;
        }

        public List<GlobalCodes> GetGlobalCodesById(string id)
        {
            var list = _gRepository.Where(x => x.GlobalCodeCategoryValue == id).ToList();
            return list;
        }

        /// <summary>
        /// Method to load only Authroization Denail codes.
        /// </summary>
        /// <returns></returns>
        public List<Denial> GetAuthorizationDenialsCode()
        {
            var lstDenial = _repository.GetAll().Where(x => x.IsDeleted == false && x.DenialType.Equals("Authorization")).ToList();
            return lstDenial;
        }

        public List<DenialCodeCustomModel> GetFilteredDenialCodes(string text)
        {
            var lstDenial = _repository.Where(x => x.IsDeleted != true && (x.DenialCode.Contains(text) || x.DenialDescription.Contains(text))).ToList();
            return lstDenial.Select(x => _mapper.Map<DenialCodeCustomModel>(x)).ToList();
        }


        public List<DenialCodeCustomModel> BindDenialCodes(int takeValue)
        {
            var lstDenial = _repository.Where(x => x.IsDeleted == false).OrderBy(x => x.DenialSetNumber).Take(takeValue).ToList();
            return lstDenial.Select(x => _mapper.Map<DenialCodeCustomModel>(x)).ToList();

        }



        public List<DenialCodeCustomModel> GetListOnDemand(int blockNumber, int blockSize)
        {
            int startIndex = (blockNumber - 1) * blockSize;

            var lstDenailCode =
                _repository.Where(
                    x => x.IsDeleted == false)
                    .OrderBy(x => x.DenialSetNumber)
                    .Skip(startIndex)
                    .Take(blockSize)
                    .ToList();
            return lstDenailCode.Select(x => _mapper.Map<DenialCodeCustomModel>(x)).ToList();
        }
    }
}
