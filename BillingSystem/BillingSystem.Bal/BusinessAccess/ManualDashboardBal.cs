using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Bal.Mapper;
using BillingSystem.Repository.UOW;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Common;
using Microsoft.Ajax.Utilities;

namespace BillingSystem.Bal.BusinessAccess
{
    public class ManualDashboardBal : BaseBal
    {
        /// <summary>
        /// Gets or sets the manual dashboard mapper.
        /// </summary>
        /// <value>
        /// The manual dashboard mapper.
        /// </value>
        private ManualDashboardMapper ManualDashboardMapper { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperatingRoomBal" /> class.
        /// </summary>
        public ManualDashboardBal()
        {
            ManualDashboardMapper = new ManualDashboardMapper();
        }

        public List<ManualDashboardCustomModel> GetManualDashboardList()
        {
            var list = new List<ManualDashboardCustomModel>();
            using (var manualDashboardRep = UnitOfWork.ManualDashboardRepository)
            {
                var lstManualDashboard = manualDashboardRep.GetAll().ToList();
                if (lstManualDashboard.Any())
                {
                    list.AddRange(
                        lstManualDashboard.Select(item => ManualDashboardMapper.MapModelToViewModel(item)));
                }
            }
            return list;
        }

        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<ManualDashboardCustomModel> GetManualDashboardList(int corporateId)
        {
            var list = new List<ManualDashboardCustomModel>();
            using (var manualDashboardRep = UnitOfWork.ManualDashboardRepository)
            {
                var lstManualDashboard = manualDashboardRep.Where(c => c.CorporateId == corporateId).ToList();
                if (lstManualDashboard.Any())
                {
                    list.AddRange(
                        lstManualDashboard.Select(item => ManualDashboardMapper.MapModelToViewModel(item)));
                }
            }
            return list;
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public List<ManualDashboardCustomModel> SaveManualDashboard(ManualDashboard model)
        {
            using (var rep = UnitOfWork.ManualDashboardRepository)
            {
                if (model.ID > 0)
                {
                    var current = rep.GetSingle(model.ID);
                    model.CreatedBy = current.CreatedBy;
                    model.CreatedDate = current.CreatedDate;
                    rep.UpdateEntity(model, model.ID);
                }
                else
                    rep.Create(model);

                var currentId = model.ID;
                var list = GetManualDashboardList(Convert.ToInt32(model.CorporateId));
                return list;
            }
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="id">The manual dashboard identifier.</param>
        /// <returns></returns>
        public ManualDashboard GetManualDashboardById(int? id)
        {
            using (var rep = UnitOfWork.ManualDashboardRepository)
            {
                var model = rep.Where(x => x.ID == id).FirstOrDefault();
                return model;
            }
        }

        /// <summary>
        /// Gets the indicators data for indicator number.
        /// </summary>
        /// <param name="corporateId"></param>
        /// <param name="facilityid"></param>
        /// <param name="year"></param>
        /// <param name="indicatorNumber"></param>
        /// <param name="budgetType"></param>
        /// <returns></returns>
        public List<DashboardIndicatorData> GetIndicatorsDataForIndicatorNumber(int corporateId, int facilityid, string year, string indicatorNumber, string budgetType, string subCategory1, string subCategory2)
        {
            using (var rep = UnitOfWork.DashboardIndicatorDataRepository)
            {
                var list = rep.Where(x => x.IndicatorNumber.Equals(indicatorNumber) &&
                    x.ExternalValue1.Equals(budgetType) && x.FacilityId == facilityid && x.CorporateId == corporateId && x.Year.Equals(year)
                    && x.ExternalValue3 == "1" && x.SubCategory1.Equals(subCategory1) && x.SubCategory2.Equals(subCategory2)).ToList();
                return list;
            }
        }

        /// <summary>
        /// Gets the manual indicator dashboard data list.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityid">The facilityid.</param>
        /// <param name="year">The year.</param>
        /// <returns></returns>
        public List<ManualDashboardCustomModel> GetManualIndicatorDashboardDataList(int corporateId, int facilityid, string year)
        {
            var list = new List<ManualDashboardCustomModel>();
            var manualDashboardMapper = new ManualDashboardCustomMapper1();
            using (var rep = UnitOfWork.DashboardBudgetRepository)
            {
                var charegsLists = facilityid != 0 ? rep.GetManualDashBoardDataList(facilityid, corporateId, year) : rep.GetManualDashBoardDataList(0, corporateId, year);
                if (charegsLists.Any())
                {
                    list.AddRange(
                        charegsLists.Select(manualDashboardMapper.MapModelToViewModel));
                }
                return list.OrderBy(x => x.Indicators).ToList();
            }
        }
        public List<ManualDashboardCustomModel> GetManualIndicatorDashboardDataListV1(int corporateId, int facilityid, string year)
        {
            var list = new List<ManualDashboardCustomModel>();
            var manualDashboardMapper = new ManualDashboardCustomMapper1();
            using (var rep = UnitOfWork.DashboardBudgetRepository)
            {
                var charegsLists = facilityid != 0 ? rep.GetManualDashBoardDataListV1(facilityid, corporateId, year) : rep.GetManualDashBoardDataList(0, corporateId, year);
                if (charegsLists.Any())
                {
                    list.AddRange(
                        charegsLists.Select(manualDashboardMapper.MapModelToViewModel));
                }
                return list.OrderBy(x => x.Indicators).ToList();
            }
        }
        /// <summary>
        /// Gets the manual dashboard data by indicator number.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityid">The facilityid.</param>
        /// <param name="year">The year.</param>
        /// <param name="indicatorNumber">The indicator number.</param>
        /// <param name="budgetType">Type of the budget.</param>
        /// <returns></returns>
        public ManualDashboardCustomModel GetManualDashboardDataByIndicatorNumber(int corporateId, int facilityid, string year, string indicatorNumber, string budgetType, string subCategory1, string subCategory2)
        {
            var vm = new ManualDashboardCustomModel();
            var mapper = new ManualDashboardCustomMapper();
            using (var rep = UnitOfWork.DashboardBudgetRepository)
            {
                var charegsLists = rep.GetManualDashBoardDataByIndicatorNumber(facilityid, corporateId, year, indicatorNumber, budgetType, subCategory1, subCategory2);
                if (charegsLists.Any())
                    vm = charegsLists.Select(mapper.MapModelToViewModel).FirstOrDefault();
            }
            return vm;
        }
        /// <summary>
        /// Gets the manual dashboard data by indicator number.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityid">The facilityid.</param>
        /// <param name="year">The year.</param>
        /// <param name="indicatorNumber">The indicator number.</param>
        /// <param name="budgetType">Type of the budget.</param>
        /// <returns></returns>
        public ManualDashboardCustomModel GetManualDashboardDataByIndicatorNumberV1(int corporateId, int facilityid, string year, string indicatorNumber, string budgetType)
        {
            var vm = new ManualDashboardCustomModel();
            var mapper = new ManualDashboardCustomMapper();
            using (var rep = UnitOfWork.DashboardBudgetRepository)
            {
                var charegsLists = rep.GetManualDashBoardDataByIndicatorNumberV1(facilityid, corporateId, year, indicatorNumber, budgetType);
                if (charegsLists.Any())
                    vm = charegsLists.Select(mapper.MapModelToViewModel).FirstOrDefault();
            }
            return vm;
        }
        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="expiryDate">The expiray date.</param>
        /// <returns></returns>
        public UserTokenCustomModel GetUserToken(string username, DateTime expiryDate)
        {
            using (var rep = UnitOfWork.ManualDashboardRepository)
            {
                var usertokenData = rep.GenerateUserToken(username, expiryDate);
                if (usertokenData != null)
                {
                    var userTokenSelected = usertokenData.FirstOrDefault();
                    return userTokenSelected ?? new UserTokenCustomModel();
                }
            }
            return new UserTokenCustomModel();
        }

        /// <summary>
        /// Gets the manual indicator dashboard data list.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityid">The facilityid.</param>
        /// <param name="year">The year.</param>
        /// <param name="indicator"></param>
        /// <param name="ownership"></param>
        /// <returns></returns>
        public List<ManualDashboardCustomModel> RebindManualIndicatorDashboardDataList(int corporateId, int facilityid, int year, int indicator, string ownership)
        {
            var list = new List<ManualDashboardCustomModel>();
            var manualDashboardMapper = new ManualDashboardCustomMapper1();
            using (var rep = UnitOfWork.DashboardBudgetRepository)
            {
                if (!string.IsNullOrEmpty(ownership))
                    ownership = ownership.ToLower().Trim();

                var charegsLists = rep.GetManualDashBoardDataListRebind(facilityid, corporateId, year, ownership, Convert.ToString(indicator));
                if (charegsLists.Any())
                    list.AddRange(charegsLists.Select(manualDashboardMapper.MapModelToViewModel));
                //return list.OrderBy(x => x.Indicators).ToList();
                return list.OrderBy(x => x.IndicatorTypeStr).ToList(); /*Updated By Krishna on 29072015*/
            }
        }
        /// <summary>
        /// Gets the manual indicator dashboard data list.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityid">The facilityid.</param>
        /// <param name="year">The year.</param>
        /// <param name="indicator"></param>
        /// <param name="ownership"></param>
        /// <returns></returns>
        public List<ManualDashboardCustomModel> RebindManualIndicatorDashboardDataListV1(int corporateId, int facilityid, int year, int indicator, string ownership)
        {
            var list = new List<ManualDashboardCustomModel>();
            var manualDashboardMapper = new ManualDashboardCustomMapper1();
            using (var rep = UnitOfWork.DashboardBudgetRepository)
            {
                if (!string.IsNullOrEmpty(ownership))
                    ownership = ownership.ToLower().Trim();

                var charegsLists = rep.GetManualDashBoardDataListRebindV1(facilityid, corporateId, year, ownership, Convert.ToString(indicator));
                if (charegsLists.Any())
                    list.AddRange(charegsLists.Select(manualDashboardMapper.MapModelToViewModel));
                //return list.OrderBy(x => x.Indicators).ToList();
                return list.OrderBy(x => x.IndicatorTypeStr).ToList(); /*Updated By Krishna on 29072015*/
            }
        }
        /// <summary>
        /// Gets the ownership list.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <returns></returns>
        public List<string> GetOwnershipList(int corporateId)
        {
            using (var rep = UnitOfWork.DashboardIndicatorsRepository)
            {
                var list = rep.Where(c => c.CorporateId == corporateId).Select(o => o.OwnerShip).Distinct().ToList();
                return list;
            }
        }

        /// <summary>
        /// Gets the indicators list.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="ownership">The ownership.</param>
        /// <returns></returns>
        public List<DashboardIndicators> GetIndicatorsList(int corporateId, string ownership = "")
        {
            using (var rep = UnitOfWork.DashboardIndicatorsRepository)
            {
                if (!string.IsNullOrEmpty(ownership))
                    ownership = ownership.ToLower().Trim();

                //var list = string.IsNullOrEmpty(ownership)
                //    ? rep.Where(c => c.CorporateId == corporateId && c.IsActive == 1)
                //        .DistinctBy(m => m.IndicatorNumber)
                //        .OrderBy(o => o.Dashboard)
                //        .ToList()
                //    : rep.Where(
                //        c =>
                //            c.CorporateId == corporateId && c.IsActive == 1 &&
                //            c.OwnerShip.ToLower().Trim().Equals(ownership))
                //        .DistinctBy(m => m.IndicatorNumber)
                //        .OrderBy(o => o.Dashboard)
                //        .ToList();

                var list = rep.Where(
                    c =>
                        c.CorporateId == corporateId && c.IsActive == 1 &&
                        (string.IsNullOrEmpty(ownership) || c.OwnerShip.ToLower().Trim().Equals(ownership)))
                    .DistinctBy(m => m.IndicatorNumber)
                    .OrderBy(o => o.Dashboard).ToList();
                return list;
            }
        }

        /// <summary>
        /// Updates the indicator v1.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public bool UpdateIndicatorV1(ManualDashboard model)
        {
            using (var manualDashrep = UnitOfWork.ManualDashboardRepository)
            {
                var status = manualDashrep.UpdateIndicatorV1(model);
                return status;
            }
        }



        public string SaveIndicatorsPlusData(ManualDashboard model)
        {
            using (var rep = UnitOfWork.DashboardIndicatorDataRepository)
            {
                var result = rep.SaveIndicatorsPlusData(model);
                return result != null ? result.IndicatorNumber : string.Empty;
            }
        }
    }
}
