using BillingSystem.Model;

namespace BillingSystem.Bal.BusinessAccess
{
    public interface IAuditLogService
    {
        int AddUptdateAuditLog(AuditLog auditLog);
    }
}