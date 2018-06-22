using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Bal.Mapper;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;


namespace BillingSystem.Bal.BusinessAccess
{
    public class DenialBal : BaseBal
    {
        public DenialBal()
        {
            DenialMapper = new DenialMapper();

        }

        private DenialMapper DenialMapper { get; set; }
        /// <summary>
        /// Get the Service Code
        /// </summary>
        /// <returns>Return the ServiceCode View Model</returns>
        public List<DenialCodeCustomModel> GetDenial()
        {
            var list = new List<DenialCodeCustomModel>();
            try
            {
                using (var denialRep = UnitOfWork.DenialRepository)
                {
                    //updated by jagjeet 14102014
                    var lstDenial = denialRep.Where(x => x.IsDeleted == false).ToList();
                    if (lstDenial.Count > 0)
                        list.AddRange(lstDenial.Select(item => DenialMapper.MapModelToViewModel(item)));
                }
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Method to add update the Denial in the database.
        /// </summary>
        /// <param name="ServiceCode"></param>
        /// <returns></returns>
        public int AddUpdateDenial(Denial Denial)
        {
            using (var DenialRep = UnitOfWork.DenialRepository)
            {
                if (Denial.DenialSetNumber > 0)
                    DenialRep.UpdateEntity(Denial, Denial.DenialSetNumber);
                else
                    DenialRep.Create(Denial);
                return Denial.DenialSetNumber;
            }
        }

        /// <summary>
        /// Method to add the Service Code in the database.
        /// </summary>
        /// <param name="shared"></param>
        /// <returns></returns>
        public Denial GetDenialById(int id)
        {
            using (var DenialRep = UnitOfWork.DenialRepository)
            {
                var Denial = DenialRep.GetAll().Where(x => x.DenialSetNumber == id).FirstOrDefault();
                return Denial;
            }
        }

        public List<GlobalCodes> GetGlobalCodesById(string id)
        {
            using (var globalRep = UnitOfWork.GlobalCodeRepository)
            {
                var list = globalRep.GetAll().Where(x => x.GlobalCodeCategoryValue == id).ToList();
                return list;
            }
        }

        /// <summary>
        /// Method to load only Authroization Denail codes.
        /// </summary>
        /// <returns></returns>
        public List<Denial> GetAuthorizationDenialsCode()
        {
            using (var denialRep = UnitOfWork.DenialRepository)
            {
                var lstDenial = denialRep.GetAll().Where(x => x.IsDeleted == false && x.DenialType.Equals("Authorization")).ToList();
                return lstDenial;
            }
        }

        public List<DenialCodeCustomModel> GetFilteredDenialCodes(string text)
        {
            var list = new List<DenialCodeCustomModel>();
            try
            {
                using (var denialRep = UnitOfWork.DenialRepository)
                {
                    var lstDenial = denialRep.Where(x => x.IsDeleted != true
                    && (x.DenialCode.Contains(text) || x.DenialDescription.Contains(text))).ToList();
                    if (lstDenial.Count > 0)
                        list.AddRange(lstDenial.Select(item => DenialMapper.MapModelToViewModel(item)));
                }
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<DenialCodeCustomModel> BindDenialCodes(int takeValue)
        {
            var list = new List<DenialCodeCustomModel>();
            using (var denialRep = UnitOfWork.DenialRepository)
            {
                var lstDenial = denialRep.Where(x => x.IsDeleted == false).OrderBy(x => x.DenialSetNumber).Take(takeValue).ToList();
                if (lstDenial.Count > 0)
                    list.AddRange(lstDenial.Select(item => DenialMapper.MapModelToViewModel(item)));
            }
            return list;
        }



        public List<DenialCodeCustomModel> GetListOnDemand(int blockNumber, int blockSize)
        {
            var list = new List<DenialCodeCustomModel>();

            try
            {
                int startIndex = (blockNumber - 1) * blockSize;
                using (var denialCodeRep = UnitOfWork.DenialRepository)
                {
                    var lstDenailCode =
                        denialCodeRep.Where(
                            x => x.IsDeleted == false)
                            .OrderBy(x => x.DenialSetNumber)
                            .Skip(startIndex)
                            .Take(blockSize)
                            .ToList();
                    if (lstDenailCode.Count > 0)
                        list.AddRange(lstDenailCode.Select(item => DenialMapper.MapModelToViewModel(item)));
                    return list;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
