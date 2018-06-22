using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Repository.UOW;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.BusinessAccess
{
    using BillingSystem.Common;

    public class XPaymentFileXMLBal : BaseBal
    {
        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<XPaymentFileXMLCustomModel> GetXPaymentFileXML(int claimid)
        {
            var list = new List<XPaymentFileXMLCustomModel>();
            using (var xPaymentFileXmlRep = UnitOfWork.XPaymentFileXMLRepository)
            {
                var lstXPaymentFileXml = xPaymentFileXmlRep.Where(x => x.XClaimID == claimid).OrderByDescending(x => x.XPaymentFileXMLID).ToList();   
                if (lstXPaymentFileXml.Count > 0)
                {
                    list.AddRange(lstXPaymentFileXml.Select(item => new XPaymentFileXMLCustomModel
                    {
                        XPaymentFileXMLID = item.XPaymentFileXMLID,
                        XClaimID = item.XClaimID,
                        XPaymentFileXML1 = item.XPaymentFileXML1,
                        XModifiedDate = item.XModifiedDate,
                        XModifiedBy = item.XModifiedBy,
                        XStatus = item.XStatus,
                    }));
                }
            }
            return list;
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="XPaymentFileXML"></param>
        /// <returns></returns>
        public Int64 SaveXPaymentFileXML(XPaymentFileXML model)
        {
            using (var rep = UnitOfWork.XPaymentFileXMLRepository)
            {
                if (model.XPaymentFileXMLID > 0)
                    rep.UpdateEntity(model,Convert.ToInt32(model.XPaymentFileXMLID));
                else
                    rep.Create(model);
                return model.XPaymentFileXMLID;
            }
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="shared"></param>
        /// <returns></returns>
        public XPaymentFileXML GetXPaymentFileXMLByID(int? XPaymentFileXMLId)
        {
            using (var rep = UnitOfWork.XPaymentFileXMLRepository)
            {
                var model = rep.Where(x => x.XPaymentFileXMLID == XPaymentFileXMLId).FirstOrDefault();
                return model;
            }
        }

        /// <summary>
        /// Gets the first x payment file XML.
        /// </summary>
        /// <param name="claimid">The claimid.</param>
        /// <returns></returns>
        public string GetFirstXPaymentFileXML(int claimid)
        {
            using (var xPaymentFileXmlRep = UnitOfWork.XPaymentFileXMLRepository)
            {
                var lstXPaymentFileXml = xPaymentFileXmlRep.Where(x => x.XClaimID == claimid).OrderByDescending(x => x.XPaymentFileXMLID).FirstOrDefault();
                if (lstXPaymentFileXml != null)
                {
                    var formattedXml = XmlParser.GetFormattedXml(lstXPaymentFileXml.XPaymentFileXML1);
                    return formattedXml;
                }
                return string.Empty;
            }
        }
    }
}
