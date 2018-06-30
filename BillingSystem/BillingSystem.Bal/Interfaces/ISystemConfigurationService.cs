using BillingSystem.Model;

namespace BillingSystem.Bal.Interfaces
{
    public interface ISystemConfigurationService
    {
        SystemConfiguration getOfflineTime();
    }
}