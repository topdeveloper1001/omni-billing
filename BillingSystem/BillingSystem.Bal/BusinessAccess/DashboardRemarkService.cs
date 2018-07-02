using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Repository.Interfaces;
using AutoMapper;
using BillingSystem.Common.Common;
using System.Data.SqlClient;
using BillingSystem.Repository.Common;

namespace BillingSystem.Bal.BusinessAccess
{
    public class DashboardRemarkService : IDashboardRemarkService
    {
        private readonly IRepository<DashboardRemark> _repository;
        private readonly BillingEntities _context;
        private readonly IMapper _mapper;

        public DashboardRemarkService(IRepository<DashboardRemark> repository
            , IMapper mapper, BillingEntities context)
        {
            _repository = repository;
            _mapper = mapper;
            _context = context;
        }

        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<DashboardRemarkCustomModel> GetDashboardRemarkList(int corporateid)
        {
            //var list = new List<DashboardRemarkCustomModel>();
            //using (var dashboardRemarkRep = UnitOfWork.DashboardRemarkRepository)
            //{
            //    var lstDashboardRemark = dashboardRemarkRep.Where(x => x.CorporateId == corporateid && x.IsActive == true).ToList();
            //    if (lstDashboardRemark.Any())
            //    {
            //        list.AddRange(
            //            lstDashboardRemark.Select(item => DashboardRemarkMapper.MapModelToViewModel(item)));
            //    }
            //}
            //return list;

            var sqlParameters = new SqlParameter[1];
            sqlParameters[0] = new SqlParameter(InputParams.pCID.ToString(), corporateid);
            using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SprocDashboardRemarks.ToString(), isCompiled: false
                , parameters: sqlParameters))
            {
                var result = r.GetResultWithJson<DashboardRemarkCustomModel>(JsonResultsArray.DashboardResult.ToString());
                return result;
            }
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public List<DashboardRemarkCustomModel> SaveDashboardRemark(DashboardRemark model)
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

            var currentId = model.Id;
            var list = GetDashboardRemarkList(Convert.ToInt32(model.CorporateId));
            return list;
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="dashboardRemarkId">The dashboard remark identifier.</param>
        /// <returns></returns>
        public DashboardRemark GetDashboardRemarkByID(int? dashboardRemarkId)
        {
            var model = _repository.Where(x => x.Id == dashboardRemarkId).FirstOrDefault();
            return model;
        }

        /// <summary>
        /// Gets the dashboard remark list.
        /// </summary>
        /// <param name="corporateid">The corporateid.</param>
        /// <param name="facilityid">The facilityid.</param>
        /// <returns></returns>
        public List<DashboardRemarkCustomModel> GetDashboardRemarkList(int corporateid, int facilityid)
        {
            //var list = new List<DashboardRemarkCustomModel>();
            //using (var dashboardRemarkRep = UnitOfWork.DashboardRemarkRepository)
            //{
            //    var lstDashboardRemark = facilityid != 0 ?
            //        dashboardRemarkRep.Where(x => x.CorporateId == corporateid && x.FacilityId == facilityid && x.IsActive == true).ToList() :
            //        dashboardRemarkRep.Where(x => x.CorporateId == corporateid && x.IsActive == true).ToList();
            //    if (lstDashboardRemark.Any())
            //    {
            //        list.AddRange(
            //            lstDashboardRemark.Select(item => DashboardRemarkMapper.MapModelToViewModel(item)));
            //    }
            //}
            //return list;

            var sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter(InputParams.pCID.ToString(), corporateid);
            sqlParameters[1] = new SqlParameter(InputParams.pFID.ToString(), facilityid);
            using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SprocDashboardRemarks.ToString(), isCompiled: false
                , parameters: sqlParameters))
            {
                var result = r.GetResultWithJson<DashboardRemarkCustomModel>(JsonResultsArray.DashboardResult.ToString());
                return result;
            }
        }


        /// <summary>
        /// Gets the dashboard remark list.
        /// </summary>
        /// <param name="corporateid">The corporateid.</param>
        /// <param name="facilityid">The facilityid.</param>
        /// <param name="dashboardTypeId"></param>
        /// <returns></returns>
        public List<DashboardRemarkCustomModel> GetDashboardRemarkListByDashboardType(int corporateid, int facilityid, int dashboardTypeId)
        {
            //var list = new List<DashboardRemarkCustomModel>();
            //using (var dashboardRemarkRep = UnitOfWork.DashboardRemarkRepository)
            //{
            //    var lstDashboardRemark = facilityid != 0
            //        ? dashboardRemarkRep.Where(
            //            x =>
            //                x.CorporateId == corporateid && x.FacilityId == facilityid && x.IsActive == true &&
            //                x.DashboardType == dashboardTypeId).ToList()
            //        : dashboardRemarkRep.Where(
            //            x => x.CorporateId == corporateid && x.IsActive == true && x.DashboardType == dashboardTypeId)
            //            .ToList();
            //    if (lstDashboardRemark.Any())
            //    {
            //        list.AddRange(
            //            lstDashboardRemark.Select(item => DashboardRemarkMapper.MapModelToViewModel(item)));
            //    }
            //}
            //return list;

            var sqlParameters = new SqlParameter[3];
            sqlParameters[0] = new SqlParameter(InputParams.pCID.ToString(), corporateid);
            sqlParameters[1] = new SqlParameter(InputParams.pFID.ToString(), facilityid);
            sqlParameters[2] = new SqlParameter(InputParams.pTypeId.ToString(), dashboardTypeId);
            using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SprocDashboardRemarks.ToString(), isCompiled: false
                , parameters: sqlParameters))
            {
                var result = r.GetResultWithJson<DashboardRemarkCustomModel>(JsonResultsArray.DashboardResult.ToString());
                return result;
            }
        }

        /// <summary>
        /// Gets the dashboard remark list.
        /// </summary>
        /// <param name="corporateid">The corporateid.</param>
        /// <param name="facilityid">The facilityid.</param>
        /// <param name="dashboardTypeId"></param>
        /// <returns></returns>
        public List<DashboardRemarkCustomModel> GetDashboardRemarkListByDashboardTypeAndMonth(int corporateid, int facilityid, int dashboardTypeId, int month)
        {
            //var list = new List<DashboardRemarkCustomModel>();
            //using (var dashboardRemarkRep = UnitOfWork.DashboardRemarkRepository)
            //{
            //    var lstDashboardRemark = facilityid != 0
            //        ? dashboardRemarkRep.Where(
            //            x =>
            //                x.CorporateId == corporateid && x.FacilityId == facilityid && x.IsActive == true &&
            //                x.DashboardType == dashboardTypeId && x.Month == month).ToList()
            //        : dashboardRemarkRep.Where(
            //            x =>
            //                x.CorporateId == corporateid && x.IsActive == true && x.DashboardType == dashboardTypeId &&
            //                x.Month == month)
            //            .ToList();
            //    if (lstDashboardRemark.Any())
            //    {
            //        list.AddRange(
            //            lstDashboardRemark.Select(item => DashboardRemarkMapper.MapModelToViewModel(item)));
            //    }
            //}
            //return list;

            var sqlParameters = new SqlParameter[4];
            sqlParameters[0] = new SqlParameter(InputParams.pCID.ToString(), corporateid);
            sqlParameters[1] = new SqlParameter(InputParams.pFID.ToString(), facilityid);
            sqlParameters[2] = new SqlParameter(InputParams.pTypeId.ToString(), dashboardTypeId);
            sqlParameters[3] = new SqlParameter(InputParams.pMonth.ToString(), month);
            using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SprocDashboardRemarks.ToString(), isCompiled: false
                , parameters: sqlParameters))
            {
                var result = r.GetResultWithJson<DashboardRemarkCustomModel>(JsonResultsArray.DashboardResult.ToString());
                return result;
            }
        }
    }
}
