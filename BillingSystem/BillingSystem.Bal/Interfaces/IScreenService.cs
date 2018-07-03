using System.Collections.Generic;
using BillingSystem.Model;

namespace BillingSystem.Bal.Interfaces
{
    public interface IScreenService
    {
        int AddUpdateScreen(Screen screen);
        List<Screen> GetAllScreensList();
        Screen GetScreenDetailById(int screenID);
        List<Screen> GetScreenListByGroupId(int GroupID);
    }
}