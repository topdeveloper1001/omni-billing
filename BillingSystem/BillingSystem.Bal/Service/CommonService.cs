using BillingSystem.Bal.Interfaces;
using BillingSystem.Common;
using BillingSystem.Model;

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace BillingSystem.Bal.Service
{
    public class CommonService : ICommonService
    {
        private readonly IRepository<GlobalCodes> _gRepository;

        public CommonService(IRepository<GlobalCodes> gRepository)
        {
            _gRepository = gRepository;
        }

        public async Task<List<SelectList>> GetListByCategory(string category, List<string> exclusions = null)
        {
            List<SelectList> list = new List<SelectList>();
            if (exclusions == null)
                exclusions = new List<string>();

            var result = await _gRepository.Where(g => g.GlobalCodeCategoryValue.Equals(category)
             && g.IsActive && g.IsDeleted != true
             && (!exclusions.Any() || !exclusions.Contains(g.GlobalCodeValue))
             ).ToListAsync();

            if (result.Any())
            {
                list = result.Select(m => new SelectList
                {
                    Value = Convert.ToInt64(m.GlobalCodeValue),
                    Name = m.GlobalCodeName
                }).ToList();
            }
            return list;
        }

    }
}
