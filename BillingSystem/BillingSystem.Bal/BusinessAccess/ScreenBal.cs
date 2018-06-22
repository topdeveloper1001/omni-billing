using System.Linq;
using BillingSystem.Model;
using System.Collections.Generic;
using System;
namespace BillingSystem.Bal.BusinessAccess
{
    public class ScreenBal : BaseBal
    {
        /// <summary>
        /// Get the All screens
        /// </summary>
        public List<Screen> GetAllScreensList()
        {
            try
            {
                using (var screenRepository = UnitOfWork.ScreenRepository)
                {
                    var list = screenRepository.Where(s => !s.IsDeleted).ToList();
                    return list;
                }
            }
            catch (Exception x)
            {

                throw x;
            }
        }
        //function to get ScreenList by GroupID
        public List<Screen> GetScreenListByGroupId(int GroupID)
        {
            using (var screenRepository = UnitOfWork.ScreenRepository)
            {
                var list = screenRepository.Where(s => s.IsDeleted == false && s.ScreenGroup == GroupID).ToList();
                return list;
            }
        }
        //Function to get screen by screen ID
        public Screen GetScreenDetailById(int screenID)
        {
            using (var screenRepository = UnitOfWork.ScreenRepository)
            {
                var screen = screenRepository.Where(s => s.IsDeleted == false && s.ScreenId == screenID).FirstOrDefault();
                return screen;
            }

        }
        //Function to add Update Screen
        public int AddUpdateScreen(Screen screen)
        {
            try
            {
                using (var screenRepository = UnitOfWork.ScreenRepository)
                {
                    screenRepository.AutoSave = true;
                    if (screen.ScreenId > 0)
                        screenRepository.UpdateEntity(screen, screen.ScreenId);
                    else
                        screenRepository.Create(screen);
                    return screen.ScreenId;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
