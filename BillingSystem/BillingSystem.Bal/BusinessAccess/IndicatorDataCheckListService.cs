using System.Collections.Generic;
using System.Linq;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Repository.Interfaces;
using BillingSystem.Bal.Interfaces;
using AutoMapper;
using System.Data.SqlClient;
using BillingSystem.Repository.Common;
using BillingSystem.Common.Common;
using System;

namespace BillingSystem.Bal.BusinessAccess
{
    public class IndicatorDataCheckListService : IIndicatorDataCheckListService
    {
        private readonly IRepository<IndicatorDataCheckList> _repository;
        private readonly BillingEntities _context;
        private readonly IMapper _mapper;


        public IndicatorDataCheckListService(IRepository<IndicatorDataCheckList> repository
            , IMapper mapper, BillingEntities context)
        {
            _repository = repository;
            _mapper = mapper;
            _context = context;
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
            var sqlParameters = new SqlParameter[1];
            sqlParameters[0] = new SqlParameter(InputParams.pCID.ToString(), corporateid);
            using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetIndicatorDataChecklist.ToString(), isCompiled: false
                , parameters: sqlParameters))
            {
                var result = r.GetResultWithJson<IndicatorDataCheckListCustomModel>(JsonResultsArray.IndicatorData.ToString());
                return result;
            }
        }

        public List<IndicatorDataCheckListCustomModel> GetDataFromIndicatorDataCheckList(int corporateid, int facilityid, int budgetType, int year, string month)
        {
            //var list = new List<IndicatorDataCheckListCustomModel>();
            //using (var indicatorDataCheckListRep = UnitOfWork.IndicatorDataCheckListRepository)
            //{
            //    var lstIndicatorDataCheckList =
            //        indicatorDataCheckListRep.Where(
            //            a => a.CorporateId == corporateid && a.BudgetType == budgetType && a.Year == year).ToList();
            //    list.AddRange(lstIndicatorDataCheckList.Select(item => IndicatorDataCheckListMapper.MapModelToViewModel(item)));
            //}
            //return list;

            var sqlParameters = new SqlParameter[4];
            sqlParameters[0] = new SqlParameter(InputParams.pCID.ToString(), corporateid);
            sqlParameters[1] = new SqlParameter(InputParams.pFID.ToString(), facilityid);
            sqlParameters[2] = new SqlParameter(InputParams.pYear.ToString(), year);
            sqlParameters[3] = new SqlParameter(InputParams.pBudgetType.ToString(), budgetType);
            using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetIndicatorDataChecklist.ToString(), isCompiled: false
                , parameters: sqlParameters))
            {
                var result = r.GetResultWithJson<IndicatorDataCheckListCustomModel>(JsonResultsArray.IndicatorData.ToString());
                return result;
            }
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
            var spName = string.Format("EXEC {0} @CorporateId, @FacilityId,@Year,@BudgetType ", StoredProcedures.SPROC_DeleteIndicatorDataCheckList);
            var sqlParameters = new SqlParameter[4];
            sqlParameters[0] = new SqlParameter("CorporateId", corporateId);
            sqlParameters[1] = new SqlParameter("FacilityId", facilityId);
            sqlParameters[2] = new SqlParameter("Year", year);
            sqlParameters[3] = new SqlParameter("BudgetType", budgetType);
            _repository.ExecuteCommand(spName, sqlParameters);
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
            var model = _repository.Where(x => x.FacilityId == facilityId && x.CorporateId == corporateId &&
                            x.Year == year && x.IsActive == true).FirstOrDefault();
            return model;
        }

        public List<int> GetDefaultMonthAndYearByFacilityId(int facilityId, int corporateId)
        {
            var result = new List<int>();
            var current = _repository.Where(f => f.FacilityId == facilityId && f.CorporateId == corporateId && f.Year == DateTime.Now.Year)
                        .FirstOrDefault();

            if (current != null)
            {
                result.Add(Convert.ToInt32(current.ExternalValue1));        //Default Year
                result.Add(Convert.ToInt32(current.ExternalValue2));        //Default Month
            }
            return result;
        }
    }
}
