using BillingSystem.Model;

namespace BillingSystem.Bal.Interfaces
{
    public interface IAuditLogService
    {
        int AddUptdateAuditLog(AuditLog auditLog);
    }
}