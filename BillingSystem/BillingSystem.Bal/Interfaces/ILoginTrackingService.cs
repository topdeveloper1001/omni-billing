using System;
using BillingSystem.Model;

namespace BillingSystem.Bal.Interfaces
{
    public interface ILoginTrackingService
    {
        int AddUpdateLoginTrackingData(LoginTracking loginTracking);
        bool IsFirstTimeLoggedIn(int? userId, int loginType);
        int UpdateLoginOutTime(int userId, DateTime logouttime);
    }
}