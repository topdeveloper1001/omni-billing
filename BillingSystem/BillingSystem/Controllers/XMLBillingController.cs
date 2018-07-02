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


namespace BillingSystem.Controllers
{
    using System;
    using System.IO;
    using System.Web.Mvc;

    using Bal.BusinessAccess;
    using BillingSystem.Bal.Interfaces;
    using Common;
    using Model;
    using Models;

    /// <summary>
    /// The xml billing controller.
    /// </summary>
    public class XMLBillingController : BaseController
    {
        private readonly IXMLBillingService _service;


        #region Public Methods and Operators

        /// <summary>
        ///     Bind all the XFileHeader list
        /// </summary>
        /// <returns>action result with the partial view containing the XFileHeader list object</returns>
        [HttpPost]
        public ActionResult BindXFileHeaderList()
        {
            var coporateId = Helpers.GetSysAdminCorporateID();
            var facilityid = Helpers.GetDefaultFacilityId();

            // Get the facilities list
            var xmlBillingList = _service.GetXFileHeader(facilityid, coporateId);

            // Pass the ActionResult with List of XFileHeaderViewModel object to Partial View XFileHeaderList
            return PartialView(PartialViews.XMLBillingList, xmlBillingList);
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
            var currentXFileHeader = _service.GetXFileHeaderByID(Convert.ToInt32(XFileHeaderID));
            var userId = Helpers.GetLoggedInUserId();

            // Check If XFileHeader model is not null
            if (currentXFileHeader != null)
            {
                // currentXFileHeader.IsActive = false;
                // currentXFileHeader.ModifiedBy = userId;
                currentXFileHeader.ModifiedDate = Helpers.GetInvariantCultureDateTime();

                // Update Operation of current XFileHeader
                var result = _service.AddUptdateXFileHeader(currentXFileHeader);

                // return deleted ID of current XFileHeader as Json Result to the Ajax Call.
                return Json(result);
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
            var currentXFileHeader = _service.GetXFileHeaderByID(Convert.ToInt32(XFileHeaderID));
            return PartialView(PartialViews.XMLBillingAddEdit, currentXFileHeader);
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
                if (XFileHeaderModel.FileID > 0)
                {
                    XFileHeaderModel.ModifiedDate = Helpers.GetInvariantCultureDateTime();
                }

                // Call the AddXFileHeader Method to Add / Update current XFileHeader
                newId = _service.AddUptdateXFileHeader(XFileHeaderModel);
            }

            return Json(newId);
        }

        // public ActionResult ViewFile(int id, string fileName)
        // {
        // using (var _service = new XMLBillingBal())
        // {
        // var filePath = string.Format("{0}\\Documents\\Corporate-{1}\\{2}", Server.MapPath("~"), Helpers.GetDefaultCorporateId(), id);
        // var fileIsExists = Directory.Exists(filePath);
        // var xmlString = string.Empty;

        // //var filePath = string.Format("{0}/Documents/Corporate-{1}/Facility-{2}/Encounter-{3}/{}",Server.MapPath("~"),Helpers.GetDefaultCorporateId(),Helpers.GetDefaultFacilityId(),) 

        // //if (!fileIsExists)
        // //{
        // //    //Call the AddXFileHeader Method to Add / Update current XFileHeader
        // //    var currentXFileHeader = _service.GetXFileHeaderByID(Convert.ToInt32(id));
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
            var xmlString = _service.GetFormattedXmlStringByXFileId(id);
            return xmlString;
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
            var facilityId = Helpers.GetDefaultFacilityId();
            var corporateid = Helpers.GetSysAdminCorporateID();

            // Get the Entity list
            var xmlBillingList = _service.GetXFileHeaderCModel(facilityId, corporateid);

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
            const string xmlDec = "<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>";
            var xmlObject = _service.GetXmlStringById(id);
            var xmlString = xmlDec;
            xmlString += xmlObject.XFileXML1;
            var currentDateTime = Helpers.GetInvariantCultureDateTime();
            var fileName = string.Format("ExportedFile{0}.xml", currentDateTime.ToString("yy-MMM-dd ddd"));
            var path = Server.MapPath("~") + "Documents//XmlBillFiles//" + fileName;
            System.IO.File.WriteAllText(path, xmlString);
            return File(path, "application/vnd.ms-excel", fileName);
        }
        #endregion
    }
}