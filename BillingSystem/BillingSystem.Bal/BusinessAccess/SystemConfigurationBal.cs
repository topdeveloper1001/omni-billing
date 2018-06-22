using BillingSystem.Model;
using System;
using System.Linq;


namespace BillingSystem.Bal.BusinessAccess
{
    public class SystemConfigurationBal : BaseBal
    {
        /// <summary>
        /// Get the offline time
        /// </summary>
        public SystemConfiguration getOfflineTime()
        {
            try
            {
                using (var systemConfigurationRep = UnitOfWork.SystemConfigurationRepository)
                {

                    var systemConfiguration = systemConfigurationRep.Where(f => f.IsDeleted != true).FirstOrDefault();
                    return systemConfiguration;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
