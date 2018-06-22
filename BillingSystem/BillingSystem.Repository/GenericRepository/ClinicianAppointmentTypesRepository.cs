using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Repository.Common;
using System.Data;
using System;

namespace BillingSystem.Repository.GenericRepository
{
    public class ClinicianAppointmentTypesRepository : GenericRepository<ClinicianAppointmentType>
    {
        private readonly DbContext _context;

        public ClinicianAppointmentTypesRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
            _context = context;
        }

        public IEnumerable<ClinicianAppTypesCustomModel> GetList(long facilityId, long userId, long clinicianId = 0)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @FId,@LoggedInUserId,@ClinicianId", StoredProcedures.SprocGetClinicianAppointmentTypes);
                    var sqlParameters = new SqlParameter[3];
                    sqlParameters[0] = new SqlParameter("FId", facilityId);
                    sqlParameters[1] = new SqlParameter("LoggedInUserId", userId);
                    sqlParameters[2] = new SqlParameter("ClinicianId", clinicianId);

                    using (var r = _context.MultiResultSetSqlQuery(spName, parameters: sqlParameters))
                    {
                        var list = r.ResultSetFor<ClinicianAppTypesCustomModel>().ToList();
                        if (list.Any())
                        {
                            var mainList = list.Select(a => new ClinicianAppTypesCustomModel
                            {
                                ClinicianName = a.ClinicianName,
                                ClinicianId = a.ClinicianId
                            }).GroupBy(g => g.ClinicianId).Select(s => s.FirstOrDefault()).ToList();


                            foreach (var item in mainList)
                            {
                                item.AppointmentTypes = list.Where(l => l.ClinicianId == item.ClinicianId)
                                    .Select(aa => new DropdownListData
                                    {
                                        Text = aa.AppointmentType,
                                        Value = Convert.ToString(aa.AppointmentTypeId),
                                        SortOrder = Convert.ToInt32(aa.ClinicianId),
                                        Id = aa.Id
                                    });
                            }

                            return mainList;

                        }
                    }
                    return new List<ClinicianAppTypesCustomModel>();
                }
                return null;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public ClinicianAppTypesCustomModel GetDataOnViewLoad(long facilityId, long loggedInUserId)
        {
            var vm = new ClinicianAppTypesCustomModel();
            var spName = $"EXEC {StoredProcedures.SprocLoadClinicianAppointmentTypesViewData} @FId, @UserId";

            var sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("FId", facilityId);
            sqlParameters[1] = new SqlParameter("UserId", loggedInUserId);

            using (var r = _context.MultiResultSetSqlQuery(spName, parameters: sqlParameters))
            {
                var physicians = r.ResultSetFor<PhysicianViewModel>().ToList();

                if (physicians.Any())
                    vm.Physicians = physicians.Select(p => new DropdownListData
                    {
                        Text = p.PhysicianName,
                        Value = Convert.ToString(p.Id)
                    }).ToList();


                vm.AppointmentTypes = r.ResultSetFor<DropdownListData>().ToList();
            }
            return vm;
        }

        public IEnumerable<ClinicianAppTypesCustomModel> Save(DataTable dt, long userId)
        {
            try
            {
                dt.Columns.Remove("CreatedBy");
                dt.Columns.Remove("CreatedDate");
                dt.Columns.Remove("ModifiedDate");


                var sqlParameters = new SqlParameter[2];
                sqlParameters[0] = new SqlParameter
                {
                    ParameterName = "@ClinicianAppointmentType",
                    SqlDbType = SqlDbType.Structured,
                    Value = dt,
                    TypeName = "ClinicianAppointmentTypeT"
                };

                sqlParameters[1] = new SqlParameter("@LoggedInUserId", userId);

                //var paramsString = string.Join(",", sqlParameters.Select(s => $"@{s.ParameterName}"));
                //var query = $"Exec {StoredProcedures.SprocSaveClinicianAppointmentTypeAssignments} {paramsString}";

                using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SprocSaveClinicianAppointmentTypeAssignments.ToString(), false, parameters: sqlParameters))
                {
                    var result = r.ResultSetFor<ClinicianAppTypesCustomModel>().ToList();
                    return result;
                }
                //var result = _context.Database.SqlQuery<ClinicianAppTypesCustomModel>(query, sqlParameters);
                //return result.ToList();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
    }
}
