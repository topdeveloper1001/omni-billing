using System.Collections.Generic;
using System.Linq;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using System;

namespace BillingSystem.Bal.BusinessAccess
{
    public class ATCCodesBal : BaseBal
    {
        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<ATCCodesCustomModel> GetATCCodes(string text = "")
        {
            var list = new List<ATCCodesCustomModel>();
            using (var ATCCodesRep = UnitOfWork.ATCCodesRepository)
            {
                var lstATCCodes = ATCCodesRep.Where(a => a.IsActive &&
                (string.IsNullOrEmpty(text) || a.ATCCode.Contains(text) || a.DrugName.Contains(text) || a.Purpose.Contains(text) || a.CodeDescription.Contains(text))
                ).ToList();

                if (lstATCCodes.Count() > 0)
                {
                    list.AddRange(lstATCCodes.Select(item => new ATCCodesCustomModel
                    {
                        ATCCodeID = item.ATCCodeID,
                        CodeDescription = item.CodeDescription,
                        SubcodeDescription = item.SubcodeDescription,
                        SubCode = item.SubCode,
                        ATCCode = item.ATCCode,
                        DrugName = item.DrugName,
                        Purpose = item.Purpose,
                        DrugDescription = item.DrugDescription,
                        CodeEffectiveFrom = item.CodeEffectiveFrom,
                        CodeEffectiveTill = item.CodeEffectiveTill,
                        IsActive = item.IsActive,
                        CodeTableNumber = item.CodeTableNumber
                    }));
                }
            }
            return list;
        }
        /*Updated By Krishna on 30012015*/
        public int DeleteATCCode(ATCCodes model)
        {
            using (var rep = UnitOfWork.ATCCodesRepository)
            {
                return Convert.ToInt32(rep.Delete(model));
            }
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="ATCCodes"></param>
        /// <returns></returns>
        public int SaveATCCodes(ATCCodes model)
        {
            using (var rep = UnitOfWork.ATCCodesRepository)
            {
                if (model.ATCCodeID > 0)
                {
                    var m = rep.GetSingle(model.ATCCodeID);
                    model.CreatedDate = m.CreatedDate;
                    model.CreatedBy = m.CreatedBy;
                    rep.UpdateEntity(model, model.ATCCodeID);
                }
                else
                    rep.Create(model);
                return model.ATCCodeID;
            }
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="shared"></param>
        /// <returns></returns>
        public ATCCodes GetATCCodesByID(int? ATCCodesId)
        {
            using (var rep = UnitOfWork.ATCCodesRepository)
            {
                var model = rep.Where(x => x.ATCCodeID == ATCCodesId).FirstOrDefault();

                return model;
            }
        }
    }
}
