using System.Collections.Generic;
using System.Linq;
using BillingSystem.Bal.Mapper;
using BillingSystem.Repository.UOW;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.BusinessAccess
{
    public class IndicatorDataCheckListBal : BaseBal
    {

        private IndicatorDataCheckListMapper IndicatorDataCheckListMapper { get; set; }

        public IndicatorDataCheckListBal()
        {
            IndicatorDataCheckListMapper = new IndicatorDataCheckListMapper();
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
            using (var indicatorDataCheckListRep = UnitOfWork.IndicatorDataCheckListRepository)
            {
                var lstIndicatorDataCheckList = indicatorDataCheckListRep.Where(a => a.CorporateId == corporateid).ToList();
                list.AddRange(lstIndicatorDataCheckList.Select(item => IndicatorDataCheckListMapper.MapModelToViewModel(item)));
            }
            return list;
        }

        public List<IndicatorDataCheckListCustomModel> GetDataFromIndicatorDataCheckList(int corporateid, int facilityid, int budgetType, int year, string month)
        {
            var list = new List<IndicatorDataCheckListCustomModel>();
            using (var indicatorDataCheckListRep = UnitOfWork.IndicatorDataCheckListRepository)
            {
                var lstIndicatorDataCheckList =
                    indicatorDataCheckListRep.Where(
                        a => a.CorporateId == corporateid && a.BudgetType == budgetType && a.Year == year).ToList();
                list.AddRange(lstIndicatorDataCheckList.Select(item => IndicatorDataCheckListMapper.MapModelToViewModel(item)));
            }
            return list;
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public List<IndicatorDataCheckListCustomModel> SaveIndicatorDataCheckList(IndicatorDataCheckList model)
        {
            using (var rep = UnitOfWork.IndicatorDataCheckListRepository)
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

                var list = GetIndicatorDataCheckListList(model.CorporateId, model.FacilityId);
                return list;
            }
        }

        public bool DeleteIndicatorDataCheckList(string corporateId, string facilityId, int budgetType, int year, int month)
        {
            using (var rep = UnitOfWork.IndicatorDataCheckListRepository)
            {
                return rep.DeleteIndicatorDataCheckList(corporateId, facilityId, budgetType, year, month);
            }
        }
        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public IndicatorDataCheckList GetIndicatorDataCheckListById(int? id)
        {
            using (var rep = UnitOfWork.IndicatorDataCheckListRepository)
            {
                var model = rep.Where(x => x.Id == id).FirstOrDefault();
                return model;
            }
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public List<IndicatorDataCheckListCustomModel> SaveIndicatorDataCheckListInDB(IndicatorDataCheckList model)
        {
            using (var rep = UnitOfWork.IndicatorDataCheckListRepository)
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

                var list = GetIndicatorDataCheckListList(model.CorporateId, model.FacilityId);
                return list;
            }
        }


        public IndicatorDataCheckList GetIndicatorDataCheckListSingle(int facilityId, int corporateId, int budgetType, int year)
        {
            using (var rep = UnitOfWork.IndicatorDataCheckListRepository)
            {
                var model =
                    rep.Where(
                        x =>
                            x.FacilityId == facilityId && x.CorporateId == corporateId &&
                            x.Year == year && x.IsActive == true).FirstOrDefault();
                return model;
            }
        }
    }
}
