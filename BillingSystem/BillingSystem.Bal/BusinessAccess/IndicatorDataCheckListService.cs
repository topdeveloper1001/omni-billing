using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using AutoMapper;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Repository.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class IndicatorDataCheckListService : IIndicatorDataCheckListService
    {
        private readonly IRepository<IndicatorDataCheckList> _repository;
        private readonly IMapper _mapper;

        public IndicatorDataCheckListService(IRepository<IndicatorDataCheckList> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <param name="corporateid">The corporateid.</param>
        /// <param name="facilityid">The facilityid.</param>
        /// <returns>
        /// Return the Entity List
        /// </returns>
        public List<IndicatorDataCheckListCustomModel> GetIndicatorDataCheckListList(int corporateid, int facilityid)
        {
            var list = new List<IndicatorDataCheckListCustomModel>();
            var lstIndicatorDataCheckList = _repository.Where(a => a.CorporateId == corporateid).ToList();
            list.AddRange(MapValues(lstIndicatorDataCheckList));
            return list;
        }

        private List<IndicatorDataCheckListCustomModel> MapValues(List<IndicatorDataCheckList> m)
        {
            var list = new List<IndicatorDataCheckListCustomModel>();
            if (m != null && m.Any())
            {
                foreach (var model in m)
                {
                    var vm = _mapper.Map<IndicatorDataCheckListCustomModel>(model);
                    if (vm != null)
                    {
                        using (var bal = new BaseBal())
                        {
                            vm.FacilityName = bal.GetFacilityNameByFacilityId(Convert.ToInt32(model.FacilityId));
                            vm.CusM1 = model.M1 == "1";
                            vm.CusM2 = model.M2 == "1";
                            vm.CusM3 = model.M3 == "1";
                            vm.CusM4 = model.M4 == "1";
                            vm.CusM5 = model.M5 == "1";
                            vm.CusM6 = model.M6 == "1";
                            vm.CusM7 = model.M7 == "1";
                            vm.CusM8 = model.M8 == "1";
                            vm.CusM9 = model.M9 == "1";
                            vm.CusM10 = model.M10 == "1";
                            vm.CusM11 = model.M11 == "1";
                            vm.CusM12 = model.M12 == "1";
                            if (model.ExternalValue2 != null)
                                vm.CusMonth = Convert.ToInt32(model.ExternalValue2 == "" ? "0" : model.ExternalValue2) > 0 ? true : false;
                        }
                    }
                    list.Add(vm);
                }
            }

            return list;
        }

        public List<IndicatorDataCheckListCustomModel> GetDataFromIndicatorDataCheckList(int corporateid, int facilityid, int budgetType, int year, string month)
        {
            var list = new List<IndicatorDataCheckListCustomModel>();
            var lstIndicatorDataCheckList = _repository.Where(a => a.CorporateId == corporateid && a.BudgetType == budgetType && a.Year == year).ToList();
            list.AddRange(MapValues(lstIndicatorDataCheckList));
            return list;
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public List<IndicatorDataCheckListCustomModel> SaveIndicatorDataCheckList(IndicatorDataCheckList model)
        {
            if (model.Id > 0)
            {
                var current = _repository.GetSingle(model.Id);
                model.CreatedBy = current.CreatedBy;
                model.CreatedDate = current.CreatedDate;
                _repository.UpdateEntity(model, model.Id);
            }
            else
                _repository.Create(model);

            var list = GetIndicatorDataCheckListList(model.CorporateId, model.FacilityId);
            return list;
        }

        public bool DeleteIndicatorDataCheckList(string corporateId, string facilityId, int budgetType, int year, int month)
        {
            var sqlParameters = new SqlParameter[4];
            sqlParameters[0] = new SqlParameter("CorporateId", corporateId);
            sqlParameters[1] = new SqlParameter("FacilityId", facilityId);
            sqlParameters[2] = new SqlParameter("Year", year);
            sqlParameters[3] = new SqlParameter("BudgetType", budgetType);
            _repository.ExecuteCommand(StoredProcedures.SPROC_DeleteIndicatorDataCheckList.ToString(), sqlParameters);
            return true;
        }
        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public IndicatorDataCheckList GetIndicatorDataCheckListById(int? id)
        {
            var model = _repository.Where(x => x.Id == id).FirstOrDefault();
            return model;
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public List<IndicatorDataCheckListCustomModel> SaveIndicatorDataCheckListInDB(IndicatorDataCheckList model)
        {
            if (model.Id > 0)
            {
                var current = _repository.GetSingle(model.Id);
                model.CreatedBy = current.CreatedBy;
                model.CreatedDate = current.CreatedDate;
                _repository.UpdateEntity(model, model.Id);
            }
            else
                _repository.Create(model);

            var list = GetIndicatorDataCheckListList(model.CorporateId, model.FacilityId);
            return list;
        }


        public IndicatorDataCheckList GetIndicatorDataCheckListSingle(int facilityId, int corporateId, int budgetType, int year)
        {
            var model = _repository.Where(x => x.FacilityId == facilityId && x.CorporateId == corporateId && x.Year == year && x.IsActive == true).FirstOrDefault();
            return model;
        }
    }
}
