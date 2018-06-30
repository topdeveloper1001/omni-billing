using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Common;
using BillingSystem.Repository.Interfaces;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class XPaymentFileXMLService : IXPaymentFileXMLService
    {
        private readonly IRepository<XPaymentFileXML> _repository;

        public XPaymentFileXMLService(IRepository<XPaymentFileXML> repository)
        {
            _repository = repository;
        }


        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<XPaymentFileXMLCustomModel> GetXPaymentFileXML(int claimid)
        {
            var list = new List<XPaymentFileXMLCustomModel>();
            var lstXPaymentFileXml = _repository.Where(x => x.XClaimID == claimid).OrderByDescending(x => x.XPaymentFileXMLID).ToList();
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
            return list;
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="XPaymentFileXML"></param>
        /// <returns></returns>
        public Int64 SaveXPaymentFileXML(XPaymentFileXML model)
        {
            if (model.XPaymentFileXMLID > 0)
                _repository.UpdateEntity(model, Convert.ToInt32(model.XPaymentFileXMLID));
            else
                _repository.Create(model);
            return model.XPaymentFileXMLID;
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="shared"></param>
        /// <returns></returns>
        public XPaymentFileXML GetXPaymentFileXMLByID(int? XPaymentFileXMLId)
        {
            var model = _repository.Where(x => x.XPaymentFileXMLID == XPaymentFileXMLId).FirstOrDefault();
            return model;
        }

        /// <summary>
        /// Gets the first x payment file XML.
        /// </summary>
        /// <param name="claimid">The claimid.</param>
        /// <returns></returns>
        public string GetFirstXPaymentFileXML(int claimid)
        {
            var lstXPaymentFileXml = _repository.Where(x => x.XClaimID == claimid).OrderByDescending(x => x.XPaymentFileXMLID).FirstOrDefault();
            if (lstXPaymentFileXml != null)
            {
                var formattedXml = XmlParser.GetFormattedXml(lstXPaymentFileXml.XPaymentFileXML1);
                return formattedXml;
            }
            return string.Empty;
        }
    }
}
