using BillingSystem.Models;
using BillingSystem.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Controllers
{
    public class TPXMLParsedDataController : BaseController
    {
        /// <summary>
        /// Get the details of the TPXMLParsedData View in the Model TPXMLParsedData such as TPXMLParsedDataList, list of countries etc.
        /// </summary>
        /// <param name="shared">passed the input object</param>
        /// <returns>returns the actionresult in the form of current object of the Model TPXMLParsedData to be passed to View TPXMLParsedData</returns>
        public ActionResult Index()
        {
            //Initialize the TPXMLParsedData BAL object
            using (var bal = new TPXMLParsedDataBal())
            {
                //Get the Entity list

                //Intialize the View Model i.e. TPXMLParsedDataView which is binded to Main View Index.cshtml under TPXMLParsedData
                var viewModel = new TPXMLParsedDataView
                {
                    TPXMLParsedDataList = new List<TPXMLParsedDataCustomModel>(),
                    CurrentTPXMLParsedData = new TPXMLParsedData()
                };

                //Pass the View Model in ActionResult to View TPXMLParsedData
                return View(viewModel);
            }
        }

        
    }
}
