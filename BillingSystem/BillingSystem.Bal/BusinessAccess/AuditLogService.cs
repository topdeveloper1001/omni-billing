using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Model;
using BillingSystem.Repository.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IRepository<AuditLog> _repository;

        public AuditLogService(IRepository<AuditLog> repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Adds the uptdate audit log.
        /// </summary>
        /// <param name="auditLog">The audit log.</param>
        /// <returns></returns>
        public int AddUptdateAuditLog(AuditLog auditLog)
        {
            if (auditLog.AuditLogID > 0)
                _repository.UpdateEntity(auditLog, auditLog.AuditLogID);
            else
                _repository.Create(auditLog);
            return auditLog.AuditLogID;
        }
    }
}
