using System.Collections.Generic;
using System.Linq;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using System;
using BillingSystem.Common.Common;

using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class LabTestOrderSetService : ILabTestOrderSetService
    {
        private readonly IRepository<LabTestOrderSet> _repository;
        private readonly IRepository<GlobalCodes> _gRepository;

        public LabTestOrderSetService(IRepository<LabTestOrderSet> repository, IRepository<GlobalCodes> gRepository)
        {
            _repository = repository;
            _gRepository = gRepository;
        }

        private string GetNameByGlobalCodeValue(string codeValue, string categoryValue, string fId = "")
        {
            if (!string.IsNullOrEmpty(codeValue))
            {
                var gl = _gRepository.Where(g => g.GlobalCodeValue.Equals(codeValue) && !g.IsDeleted.Value && g.GlobalCodeCategoryValue.Equals(categoryValue) && (string.IsNullOrEmpty(fId) || g.FacilityNumber.Equals(fId))).FirstOrDefault();
                return gl != null ? gl.GlobalCodeName : string.Empty;
            }
            return string.Empty;
        }


        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<LabTestOrderSetCustomModel> GetLabOrderSetList()
        {
            var list = new List<LabTestOrderSetCustomModel>();
            var lstLabTestOrderSet = _repository.Where(a => a.IsDeleted == null || !(bool)a.IsDeleted).ToList();
            if (lstLabTestOrderSet.Count > 0)
            {
                var category = Convert.ToString(Convert.ToInt32(GlobalCodeCategoryValue.CodeTime));
                list.AddRange(lstLabTestOrderSet.Select(item => new LabTestOrderSetCustomModel
                {
                    CodeTime1 = GetNameByGlobalCodeValue(item.CodeTime1, category),
                    CodeTime2 = GetNameByGlobalCodeValue(item.CodeTime2, category),
                    CodeTime3 = GetNameByGlobalCodeValue(item.CodeTime3, category),
                    CodeTime4 = GetNameByGlobalCodeValue(item.CodeTime4, category),
                    CodeTime5 = GetNameByGlobalCodeValue(item.CodeTime5, category),
                    CodeTime6 = GetNameByGlobalCodeValue(item.CodeTime6, category),
                    CodeTime7 = GetNameByGlobalCodeValue(item.CodeTime7, category),
                    CodeTime8 = GetNameByGlobalCodeValue(item.CodeTime8, category),
                    CodeTime9 = GetNameByGlobalCodeValue(item.CodeTime9, category),
                    CodeTime10 = GetNameByGlobalCodeValue(item.CodeTime10, category),
                    CodeTime11 = GetNameByGlobalCodeValue(item.CodeTime11, category),
                    CodeTime12 = GetNameByGlobalCodeValue(item.CodeTime12, category),
                    CodeTime13 = GetNameByGlobalCodeValue(item.CodeTime13, category),
                    CodeTime14 = GetNameByGlobalCodeValue(item.CodeTime14, category),
                    CodeTime15 = GetNameByGlobalCodeValue(item.CodeTime15, category),
                    CodeValue1 = item.CodeValue1,
                    CodeValue2 = item.CodeValue2,
                    CodeValue3 = item.CodeValue3,
                    CodeValue4 = item.CodeValue4,
                    CodeValue5 = item.CodeValue5,
                    CodeValue6 = item.CodeValue6,
                    CodeValue7 = item.CodeValue7,
                    CodeValue8 = item.CodeValue8,
                    CodeValue9 = item.CodeValue9,
                    CodeValue10 = item.CodeValue10,
                    CodeValue11 = item.CodeValue11,
                    CodeValue12 = item.CodeValue12,
                    CodeValue13 = item.CodeValue13,
                    CodeValue14 = item.CodeValue14,
                    CodeValue15 = item.CodeValue15,
                    CreatedBy = item.CreatedBy,
                    CreatedDate = item.CreatedDate,
                    DeletedBy = item.DeletedBy,
                    DeletedDate = item.DeletedDate,
                    Description = item.Description,
                    Id = item.Id,
                    IsDeleted = item.IsDeleted,
                    ModifiedBy = item.ModifiedBy,
                    ModifiedDate = item.ModifiedDate,
                    OrderSetTableName = item.OrderSetTableName,
                    OrderSetTableNumber = item.OrderSetTableNumber,
                    OrderSetValue = item.OrderSetValue,
                    Type = item.Type,
                }));
            }
            return list;
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<LabTestOrderSetCustomModel> SaveLabTestOrderSet(LabTestOrderSet model)
        {
            if (model.Id > 0)
                _repository.UpdateEntity(model, model.Id);
            else
                _repository.Create(model);

            var list = GetLabOrderSetList();
            return list;
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public LabTestOrderSet GetDetailById(int? id)
        {
            var m = _repository.Where(x => x.Id == id).FirstOrDefault();
            return m;
        }
    }
}
