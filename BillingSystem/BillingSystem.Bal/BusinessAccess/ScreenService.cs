using System.Linq;
using BillingSystem.Model;
using System.Collections.Generic;
using System;

using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class ScreenService : IScreenService
    {
        private readonly IRepository<Screen> _repository;

        public ScreenService(IRepository<Screen> repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Get the All screens
        /// </summary>
        public List<Screen> GetAllScreensList()
        {
            var list = _repository.Where(s => !s.IsDeleted).ToList();
            return list;
        }
        //function to get ScreenList by GroupID
        public List<Screen> GetScreenListByGroupId(int GroupID)
        {
            var list = _repository.Where(s => s.IsDeleted == false && s.ScreenGroup == GroupID).ToList();
            return list;
        }
        //Function to get screen by screen ID
        public Screen GetScreenDetailById(int screenID)
        {
            var screen = _repository.Where(s => s.IsDeleted == false && s.ScreenId == screenID).FirstOrDefault();
            return screen;
        }
        //Function to add Update Screen
        public int AddUpdateScreen(Screen screen)
        {
            try
            {
                if (screen.ScreenId > 0)
                    _repository.UpdateEntity(screen, screen.ScreenId);
                else
                    _repository.Create(screen);
                return screen.ScreenId;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
