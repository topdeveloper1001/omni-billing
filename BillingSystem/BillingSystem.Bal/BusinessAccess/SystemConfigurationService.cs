using BillingSystem.Bal.Interfaces;
using BillingSystem.Model;
using BillingSystem.Repository.Interfaces;
using System.Linq;

namespace BillingSystem.Bal.BusinessAccess
{
    public class SystemConfigurationService : ISystemConfigurationService
    {
        private readonly IRepository<SystemConfiguration> _repository;

        public SystemConfigurationService(IRepository<SystemConfiguration> repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Get the offline time
        /// </summary>
        public SystemConfiguration getOfflineTime()
        {
            var m = _repository.Where(f => f.IsDeleted != true).FirstOrDefault();
            return m;
        }
    }
}
