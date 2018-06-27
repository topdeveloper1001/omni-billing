using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;
using BillingSystem.Repository.Interfaces;
using System.Data.SqlClient;
using BillingSystem.Common.Common;
using BillingSystem.Repository.Common;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class ClinicianAppointmentTypesService : IClinicianAppointmentTypesService
    {
        private readonly IRepository<ClinicianAppointmentType> _repository;
        private readonly BillingEntities _context;

        public ClinicianAppointmentTypesService(IRepository<ClinicianAppointmentType> repository, BillingEntities context)
        {
            _repository = repository;
            _context = context;
        }

        public IEnumerable<ClinicianAppTypesCustomModel> GetList(long facilityId, long userId, long clinicianId = 0)
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

        public ClinicianAppTypesCustomModel GetDataOnViewLoad(long facilityId, long userId)
        {
            var vm = new ClinicianAppTypesCustomModel();
            var spName = $"EXEC {StoredProcedures.SprocLoadClinicianAppointmentTypesViewData} @FId, @UserId";

            var sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("FId", facilityId);
            sqlParameters[1] = new SqlParameter("UserId", userId);

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

        public long Save(ClinicianAppTypesCustomModel vm)
        {
            long queryStatus = 0;
            if (vm.AppointmentTypes.Any())
            {
                foreach (var item in vm.AppointmentTypes)
                {
                    var appTypeId = Convert.ToInt32(item.Value);
                    var current = _repository.Where(a => a.ClinicianId == vm.ClinicianId &&
                                            a.AppointmentTypeId == appTypeId
                                            ).FirstOrDefault();

                    if (current == null)
                    {
                        var m = new ClinicianAppointmentType
                        {
                            ClinicianId = vm.ClinicianId,
                            AppointmentTypeId = appTypeId,
                            CreatedBy = vm.CreatedBy,
                            CreatedDate = DateTime.Now,
                            ModifiedBy = null,
                            ModifiedDate = null,
                            IsDeleted = false
                        };
                        _repository.Create(m);
                        queryStatus = m.Id;
                    }
                    else
                    {
                        current.ModifiedBy = vm.CreatedBy;
                        current.ModifiedDate = DateTime.Now;
                        current.IsDeleted = vm.IsDeleted;

                        _repository.UpdateEntity(current, current.Id);
                        queryStatus = current.Id;
                    }
                }
            }
            else
            {
                if (vm.IsDeleted)
                {
                    var m = _repository.Where(a => a.Id == vm.Id).FirstOrDefault();
                    m.ModifiedBy = vm.CreatedBy;
                    m.ModifiedDate = DateTime.Now;
                    m.IsDeleted = vm.IsDeleted;

                    _repository.UpdateEntity(m, m.Id);
                    queryStatus = m.Id;
                }
            }
            return queryStatus;

        }
    }
}
