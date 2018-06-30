using System;
using System.Linq;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Model;
using BillingSystem.Repository.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class LoginTrackingService : ILoginTrackingService
    {
        private readonly IRepository<LoginTracking> _repository;

        public LoginTrackingService(IRepository<LoginTracking> repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Get the Most Recent Login tracking info
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="loginType">Type of the login.</param>
        /// <returns></returns>
        public bool IsFirstTimeLoggedIn(int? userId, int loginType)
        {
            var isFirstTime = _repository.GetAll().Where(o => o.ID == userId && o.LoginUserType == loginType).OrderByDescending(m => m.LoginTime).Any();
            return !isFirstTime;
        }

        //Function to add update login tracking
        /// <summary>
        /// Adds the update login tracking data.
        /// </summary>
        /// <param name="loginTracking">The login tracking.</param>
        /// <returns></returns>
        public int AddUpdateLoginTrackingData(LoginTracking loginTracking)
        {
            try
            {
                if (loginTracking.LoginTrackingID > 0)
                    _repository.UpdateEntity(loginTracking, loginTracking.LoginTrackingID);
                else
                    _repository.Create(loginTracking);
                return loginTracking.LoginTrackingID;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public int UpdateLoginOutTime(int userId, DateTime logouttime)
        {
            var current = _repository.Where(l => l.ID == userId).OrderByDescending(m => m.LoginTime).FirstOrDefault();
            if (current != null)
            {
                current.LogoutTime = logouttime;
                _repository.UpdateEntity(current, current.LoginTrackingID);
                return current.LoginTrackingID;
            }

            return 0;
        }


    }
}
