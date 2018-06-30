// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XMLBillingController.cs" company="SPadez">
//   Omnihealthcare
// </copyright>
// <Screen Owner>
// Shashank (Last modified on 22nd Feb 2016)
// </Screen Owner>
// <summary>
//   The xml billing controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Text;
using System.Web.UI.WebControls;

namespace BillingSystem.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Web.Mvc;

    using Bal.BusinessAccess;
    using Common;
    using Model;
    using Model.CustomModel;
    using Models;
    using System.Xml;
    using System.Xml.Linq;

    /// <summary>
    /// The xml billing controller.
    /// </summary>
    public class XMLBillingController : BaseController
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Bind all the XFileHeader list
        /// </summary>
        /// <returns>action result with the partial view containing the XFileHeader list object</returns>
        [HttpPost]
        public ActionResult BindXFileHeaderList()
        {
            // Initialize the XFileHeader BAL object
            using (var xmlBillingBal = new XMLBillingService())
            {
                var coporateId = Helpers.GetSysAdminCorporateID();
                var facilityid = Helpers.GetDefaultFacilityId();

                // Get the facilities list
                var xmlBillingList = xmlBillingBal.GetXFileHeader(facilityid, coporateId);

                // Pass the ActionResult with List of XFileHeaderViewModel object to Partial View XFileHeaderList
                return PartialView(PartialViews.XMLBillingList, xmlBillingList);
            }
        }

        /// <summary>
        /// The check if file exists.
        /// </summary>
        /// <param name="filePath">
        /// The file path.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CheckIfFileExists(string filePath)
        {
            return Directory.Exists(filePath);
        }

        /// <summary>
        /// Delete the current XFileHeader based on the XFileHeader ID passed in the XFileHeaderModel
        /// </summary>
        /// <param name="XFileHeaderID">
        /// The X File Header ID.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult DeleteXFileHeader(int XFileHeaderID)
        {
            using (var xFileHeaderBal = new XMLBillingService())
            {
                // Get XFileHeader model object by current XFileHeader ID
                var currentXFileHeader = xFileHeaderBal.GetXFileHeaderByID(Convert.ToInt32(XFileHeaderID));
                var userId = Helpers.GetLoggedInUserId();

                // Check If XFileHeader model is not null
                if (currentXFileHeader != null)
                {
                    // currentXFileHeader.IsActive = false;
                    // currentXFileHeader.ModifiedBy = userId;
                    currentXFileHeader.ModifiedDate = Helpers.GetInvariantCultureDateTime();

                    // Update Operation of current XFileHeader
                    var result = xFileHeaderBal.AddUptdateXFileHeader(currentXFileHeader);

                    // return deleted ID of current XFileHeader as Json Result to the Ajax Call.
                    return Json(result);
                }
            }

            // Return the Json result as Action Result back JSON Call Success
            return Json(null);
        }

        /// <summary>
        /// Get the details of the current XFileHeader in the view model by ID
        /// </summary>
        /// <param name="XFileHeaderID">The x file header identifier.</param>
        /// <returns>
        /// The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult GetXFileHeader(int XFileHeaderID)
        {
            using (var xFileHeaderBal = new XMLBillingService())
            {
                // Call the AddXFileHeader Method to Add / Update current XFileHeader
                var currentXFileHeader = xFileHeaderBal.GetXFileHeaderByID(Convert.ToInt32(XFileHeaderID));

                // If the view is shown in ViewMode only, then ViewBag.ViewOnly is set to true otherwise false.
                // ViewBag.ViewOnly = !string.IsNullOrEmpty(model.ViewOnly);

                // Pass the ActionResult with the current XFileHeaderViewModel object as model to PartialView XFileHeaderAddEdit
                return PartialView(PartialViews.XMLBillingAddEdit, currentXFileHeader);
            }
        }

        /// <summary>
        /// Reset the XFileHeader View Model and pass it to XFileHeaderAddEdit Partial View.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult ResetXFileHeaderForm()
        {
            // Intialize the new object of XFileHeader ViewModel
            var xFileHeaderViewModel = new XFileHeader();

            // Pass the View Model as XFileHeaderViewModel to PartialView XFileHeaderAddEdit just to update the AddEdit partial view.
            return PartialView(PartialViews.XMLBillingAddEdit, xFileHeaderViewModel);
        }

        /// <summary>
        /// Add New or Update the XFileHeader based on if we pass the XFileHeader ID in the XFileHeaderViewModel object.
        /// </summary>
        /// <param name="XFileHeaderModel">pass the details of XFileHeader in the view model</param>
        /// <returns>
        /// returns the newly added or updated ID of XFileHeader row
        /// </returns>
        public ActionResult SaveXFileHeader(XFileHeader XFileHeaderModel)
        {
            // Initialize the newId variable 
            var newId = -1;
            var userId = Helpers.GetLoggedInUserId();

            // Check if XFileHeaderViewModel 
            if (XFileHeaderModel != null)
            {
                using (var xFileHeaderBal = new XMLBillingService())
                {
                    if (XFileHeaderModel.FileID > 0)
                    {
                        // XFileHeaderModel.ModifiedBy = userId;
                        XFileHeaderModel.ModifiedDate = Helpers.GetInvariantCultureDateTime();
                    }

                    // Call the AddXFileHeader Method to Add / Update current XFileHeader
                    newId = xFileHeaderBal.AddUptdateXFileHeader(XFileHeaderModel);
                }
            }

            return Json(newId);
        }

        // public ActionResult ViewFile(int id, string fileName)
        // {
        // using (var xFileHeaderBal = new XMLBillingBal())
        // {
        // var filePath = string.Format("{0}\\Documents\\Corporate-{1}\\{2}", Server.MapPath("~"), Helpers.GetDefaultCorporateId(), id);
        // var fileIsExists = Directory.Exists(filePath);
        // var xmlString = string.Empty;

        // //var filePath = string.Format("{0}/Documents/Corporate-{1}/Facility-{2}/Encounter-{3}/{}",Server.MapPath("~"),Helpers.GetDefaultCorporateId(),Helpers.GetDefaultFacilityId(),) 

        // //if (!fileIsExists)
        // //{
        // //    //Call the AddXFileHeader Method to Add / Update current XFileHeader
        // //    var currentXFileHeader = xFileHeaderBal.GetXFileHeaderByID(Convert.ToInt32(id));
        // //    XmlParser.SaveStringToXMLFile(filePath, currentXFileHeader.XFile, id.ToString());
        // //    //xmlString = Server.HtmlEncode(currentXFileHeader.XFile);
        // //}
        // xmlString = string.Format("/Documents/Corporate-{0}/{1}/{2}.xml", Helpers.GetDefaultCorporateId(), id, id);
        // return Json(xmlString);
        // //If the view is shown in ViewMode only, then ViewBag.ViewOnly is set to true otherwise false.
        // //ViewBag.ViewOnly = !string.IsNullOrEmpty(model.ViewOnly);
        // //return File(Encoding.UTF8.GetBytes(currentXFileHeader.XFile), "application/xml", currentXFileHeader.SenderID);
        // //Pass the ActionResult with the current XFileHeaderViewModel object as model to PartialView XFileHeaderAddEdit
        // }
        // }

        /// <summary>
        /// The view file.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ViewFile(int id)
        {
            using (var xFileHeaderBal = new XMLBillingService())
            {
                var xmlString = xFileHeaderBal.GetFormattedXmlStringByXFileId(id);

                // Response.Clear();
                // Response.Buffer = true;
                // Response.Charset = "";
                // Response.Cache.SetCacheability(HttpCacheability.NoCache);
                // Response.ContentType = "application/xml";
                // Response.WriteFile(xmlString);
                // Response.Flush();
                // Response.End();
                // xmlString = HttpUtility.HtmlEncode(xmlString);
                return xmlString;
            }
        }

        /// <summary>
        /// Get the details of the XFileHeader View in the Model XFileHeader such as XFileHeaderList, list of countries etc.
        /// </summary>
        /// <returns>
        /// returns the actionresult in the form of current object of the Model XFileHeader to be passed to View
        /// XFileHeader
        /// </returns>
        public ActionResult XMLBillingMain()
        {
            // Initialize the XFileHeader BAL object
            var xmlBillingBal = new XMLBillingService();
            var facilityId = Helpers.GetDefaultFacilityId();
            var corporateid = Helpers.GetSysAdminCorporateID();

            // Get the Entity list
            var xmlBillingList = xmlBillingBal.GetXFileHeaderCModel(facilityId, corporateid);

            // Intialize the View Model i.e. XFileHeaderView which is binded to Main View Index.cshtml under XFileHeader
            var xFileHeaderView = new XMLBillingView
                                      {
                                          XFileHeaderList = xmlBillingList,
                                          CurrentXFileHeader = new XFileHeader()
                                      };

            // Pass the View Model in ActionResult to View XFileHeader
            return View(xFileHeaderView);
        }


        public FileResult ExportToXml(int id)
        {
            using (var xFileHeaderBal = new XMLBillingService())
            {
                const string xmlDec = "<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>";
                var xmlObject = xFileHeaderBal.GetXmlStringById(id);
                var xmlString = xmlDec;
                xmlString += xmlObject.XFileXML1;
                var currentDateTime = Helpers.GetInvariantCultureDateTime();
                var fileName = string.Format("ExportedFile{0}.xml", currentDateTime.ToString("yy-MMM-dd ddd"));
                var path = Server.MapPath("~") + "Documents//XmlBillFiles//" + fileName;
                System.IO.File.WriteAllText(path, xmlString);
                return File(path, "application/vnd.ms-excel", fileName);
            }
        }
        #endregion
    }
}