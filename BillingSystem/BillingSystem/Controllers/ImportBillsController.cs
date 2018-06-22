using System.IO;
using System.Web;
using System.Web.Mvc;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Common;
using BillingSystem.Models;
using System.Linq;

namespace BillingSystem.Controllers
{
    public class ImportBillsController : BaseController
    {
        //
        // GET: /FileUploader/
        /// <summary>
        /// Imports the bills uploader.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var bal = new TpFileHeaderBal();
            var corporateId = Helpers.GetDefaultCorporateId();
            var list = bal.TpFileHeaderList(corporateId);
            var importBillingView = new ImportBillsView { TpFileHeaderList = list };
            return View(importBillingView);
        }

        public ActionResult UploadXMLs(HttpPostedFileBase xmlFile)
        {
            if (xmlFile != null)
            {
                var corporateId = Helpers.GetSysAdminCorporateID();
                var facilityId = Helpers.GetDefaultFacilityId();
                var loggedInUserId = Helpers.GetLoggedInUserId();
                var fileN = Path.GetFileName(xmlFile.FileName);
                string savedFileName = string.Format(CommonConfig.ImportXMLBillingFilePath, corporateId, facilityId, loggedInUserId);
                if (!Directory.Exists(Server.MapPath(savedFileName)))
                    Directory.CreateDirectory(Server.MapPath(savedFileName));

                savedFileName = savedFileName + fileN;
                var completePath = Server.MapPath(savedFileName);
                xmlFile.SaveAs(completePath);

                var xml = Helpers.GetXML(completePath);
                if (!string.IsNullOrEmpty(xml))
                {
                    using (var bal = new XMLBillingBal())
                        bal.ImportXmlBills(xml, savedFileName, true);
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult BindTpXMLParsedData(int fileId)
        {
            using (var bal = new TPXMLParsedDataBal())
            {
                var list = bal.TPXMLParsedDataList(fileId);
                return PartialView(PartialViews.XMLParsedDataView, list);
            }
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file == null)
                    ModelState.AddModelError("File", "Please Upload Your file");

                else if (file.ContentLength > 0)
                {
                    const int maxContentLength = 1024 * 1024 * 5; //5 MB
                    string[] allowedFileExtensions = { ".xml" };

                    if (!allowedFileExtensions.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.'))))
                        ModelState.AddModelError("File", "Please file of type: " + string.Join(", ", allowedFileExtensions));

                    else if (file.ContentLength > maxContentLength)
                        ModelState.AddModelError("File", "Your file is too large, maximum allowed size is: " + maxContentLength + " MB");

                    else
                    {
                        //var fileName = Path.GetFileName(file.FileName);
                        //var path = Path.Combine(Server.MapPath("~/Upload"), fileName);
                        //file.SaveAs(path);
                        var result = SaveXmltoDb(file);
                        ModelState.Clear();
                        if (result)
                            ViewBag.Message = "File uploaded successfully!";
                    }
                }
            }
            var bal = new TpFileHeaderBal();
            var corporateId = Helpers.GetDefaultCorporateId();
            var list = bal.TpFileHeaderList(corporateId);
            var importBillingView = new ImportBillsView { TpFileHeaderList = list };
            return View(importBillingView);
        }

        private bool SaveXmltoDb(HttpPostedFileBase xmlFile)
        {
            var result = false;
            var corporateId = Helpers.GetSysAdminCorporateID();
            var facilityId = Helpers.GetDefaultFacilityId();
            var loggedInUserId = Helpers.GetLoggedInUserId();
            var fileN = Path.GetFileName(xmlFile.FileName);
            var savedFileName = string.Format(CommonConfig.ImportXMLBillingFilePath, corporateId, facilityId, loggedInUserId);
            if (!Directory.Exists(Server.MapPath(savedFileName)))
                Directory.CreateDirectory(Server.MapPath(savedFileName));

            savedFileName = savedFileName + fileN;
            var completePath = Server.MapPath(savedFileName);
            xmlFile.SaveAs(completePath);

            var xml = Helpers.GetXML(completePath);
            if (!string.IsNullOrEmpty(xml))
            {
                using (var bal = new XMLBillingBal())
                    result = bal.ImportXmlBills(xml, savedFileName, true);
            }
            return result;
        }
    }
}