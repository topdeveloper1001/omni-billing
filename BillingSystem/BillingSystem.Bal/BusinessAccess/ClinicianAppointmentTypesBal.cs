using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;
using BillingSystem.Repository.Common;

namespace BillingSystem.Bal.BusinessAccess
{
    public class ClinicianAppointmentTypesBal : BaseBal
    {
        public ClinicianAppointmentTypesBal()
        {
        }

        public IEnumerable<ClinicianAppTypesCustomModel> GetList(long facilityId, long userId, long clinicianId = 0)
        {
            using (var rep = UnitOfWork.ClinicianAppointmentTypesRepository)
            {
                var list = rep.GetList(facilityId, userId, clinicianId);
                return list;
            }
        }

        public ClinicianAppTypesCustomModel GetDataOnViewLoad(long facilityId, long userId)
        {
            using (var rep = UnitOfWork.ClinicianAppointmentTypesRepository)
            {
                var vm = rep.GetDataOnViewLoad(facilityId, userId);
                return vm;
            }
        }

        public long Save(ClinicianAppTypesCustomModel vm)
        {
            long queryStatus = 0;
            using (var rep = UnitOfWork.ClinicianAppointmentTypesRepository)
            {
                if (vm.AppointmentTypes.Any())
                {
                    foreach (var item in vm.AppointmentTypes)
                    {
                        var appTypeId = Convert.ToInt32(item.Value);
                        var current = rep.Where(a => a.ClinicianId == vm.ClinicianId &&
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
                            rep.Create(m);
                            queryStatus = m.Id;
                        }
                        else
                        {
                            current.ModifiedBy = vm.CreatedBy;
                            current.ModifiedDate = DateTime.Now;
                            current.IsDeleted = vm.IsDeleted;

                            rep.UpdateEntity(current, current.Id);
                            queryStatus = current.Id;
                        }
                    }
                }
                else
                {
                    if (vm.IsDeleted)
                    {
                        var m = rep.Where(a => a.Id == vm.Id).FirstOrDefault();
                        m.ModifiedBy = vm.CreatedBy;
                        m.ModifiedDate = DateTime.Now;
                        m.IsDeleted = vm.IsDeleted;

                        rep.UpdateEntity(m, m.Id);
                        queryStatus = m.Id;
                    }
                }
                return queryStatus;
            }
        }
    }
}
